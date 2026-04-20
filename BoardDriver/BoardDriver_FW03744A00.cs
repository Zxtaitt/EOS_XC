using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace BoardDriver
{
    /// <summary>
    /// PB3744A 驱动板（325G 协议扩展），仅 TCP 通讯。
    /// 从 BurninPlatform 移植并精简为 EOS IBoardDriver 接口。
    /// </summary>
    public class BoardDriver_FW03744A00 : IBoardDriver
    {
        #region IBoardDriver 属性

        public string BoardClientConStr { get; set; }
        public SerialPort BoardSerialPort { get; set; }
        public TcpClient BoardClient { get; set; }
        public byte BoardAddress { get; set; }

        #endregion

        #region 默认参数（对应 BurninPlatform BoardClampConfig 推荐值）

        private const float CspVoltage = 7000f;
        private const float CsnVoltage = 7000f;
        private const float ClampVoltagePos = 3000f;
        private const float ClampVoltageNeg = 3000f;
        private const float ClampCurrentPos = 450f;
        private const float ClampCurrentNeg = 450f;
        private const int StepDelayMs = 50;
        private const int RetryCount = 3;
        private const int ResponseTimeoutMs = 5000;

        #endregion

        #region 内部状态

        private bool _initialized;

        #endregion

        #region IBoardDriver 实现

        public void SetBoardAdjustDriveVoltage()
        {
            if (_initialized) return;

            SendInitialize();
            CtrlCspCsn(true);
            SendClamp(ClampVoltagePos, ClampVoltageNeg, ClampCurrentPos, ClampCurrentNeg);
     
            // 9.2: 整体上电前保持全关，后续仅由 SetBoardClamp 打开待测通道
            CtrlAllChannelOutput(false);

            _initialized = true;
        }

        public void SetBoardClamp(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.Direction Direction)
        {
          

        }

        public void SetBoardOnPower(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.PowerMethod PowerMethod, int Step, double SetValue)
        {
            SendOutputModeForSingleChannel(Channel, SourceType);
            CtrlSingleChannelOutput(Channel, true);
            double value = ToFirmwareValue(SetValue, SourceType);
            int stepCount = PowerMethod == BoardDriverEnum.PowerMethod.Single ? 1 : Step;
            StepPower(Channel, value, stepCount, true);
        }

        public void SetBoardOFFPower(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.PowerMethod PowerMethod, int Step, double SetValue)
        {
            double value = ToFirmwareValue(SetValue, SourceType);
            int stepCount = PowerMethod == BoardDriverEnum.PowerMethod.Single ? 1 : Step;
            StepPower(Channel, value, stepCount, false);

            // 9.4: 下电完成后，执行 3.1 + 3.2
            // 3.1 设置 LD 开关状态（全部关闭）
            CtrlAllChannelOutput(false);
            // 3.2 设置 LD 电源模式（全部切换为电压源，bit=1）
            SendOutputModeForAllChannels(BoardDriverEnum.SourceType.VoltageSource);
        }

        public void CloseBoardSerialPort()
        {
 

        }

        /// <summary>
        /// 9.5 老化结束后通用配置（2.3 -> 2.2 -> 2.1）。
        /// </summary>
        public void RunEndOfTestInitialization()
        {
            // 2.3 设置 LD 通道钳位电压/电流为 0
            SendClamp(0f, 0f, 0f, 0f);
            // 2.2 设置 CSP/CSN 开关状态为断开
            CtrlCspCsn(false);
            // 2.1 初始化
            SendInitialize();

            // 允许下一次流程重新执行初始化阶段
            _initialized = false;
        }

        public void CloseRelay(int channel)
        {
            CtrlSingleChannelOutput(channel, true);
        }

        public void OpenRelay(int channel)
        {
            CtrlSingleChannelOutput(channel, false);
        }

        #endregion

        #region 325G 指令封装

        /// <summary>
        /// 初始化命令 0x0A04
        /// </summary>
        private void SendInitialize()
        {
            Send325GCommand(new byte[] { 0x04, 0x0A });
        }

        /// <summary>
        /// CSP/CSN 电源控制 0x0011
        /// </summary>
        private void CtrlCspCsn(bool open)
        {
            uint flag = (uint)(open ? 1 : 0);
            float safeVoltage = 3000f;

            List<byte> cmd = new List<byte> { 0x11, 0x00 };
            float cspVal = open ? CspVoltage : safeVoltage;
            float csnVal = open ? CsnVoltage : safeVoltage;

            for (int i = 0; i < 2; i++)
            {
                cmd.AddRange(BitConverter.GetBytes(flag));
                cmd.AddRange(BitConverter.GetBytes(cspVal));
            }
            for (int i = 0; i < 2; i++)
            {
                cmd.AddRange(BitConverter.GetBytes(flag));
                cmd.AddRange(BitConverter.GetBytes(csnVal));
            }
            Send325GCommand(cmd.ToArray());
        }

        /// <summary>
        /// 设置所有通道输出模式 0x0112
        /// bit=0: 电流源, bit=1: 电压源
        /// </summary>
        private void SendOutputMode(byte[] modeMask)
        {
            byte[] cmd = new byte[] { 0x12, 0x01 }.Concat(modeMask).ToArray();
            Send325GCommand(cmd);
        }

        /// <summary>
        /// 设置所有通道输出开关 0x0111（全开/全关）
        /// </summary>
        private void CtrlAllChannelOutput(bool open)
        {
            byte fill = open ? (byte)0xFF : (byte)0x00;
            byte[] param = new byte[6];
            if (open) for (int i = 0; i < 6; i++) param[i] = fill;
            byte[] cmd = new byte[] { 0x11, 0x01 }.Concat(param).ToArray();
            Send325GCommand(cmd);
        }

        /// <summary>
        /// 设置单通道输出开关 0x0111（仅指定通道开，其余全关）
        /// </summary>
        private void CtrlSingleChannelOutput(int channel, bool open)
        {
            ValidateChannelRange(channel);
            byte[] param = open ? BuildSingleChannelMask(channel) : new byte[6];
            byte[] cmd = new byte[] { 0x11, 0x01 }.Concat(param).ToArray();
            Send325GCommand(cmd);
        }

        /// <summary>
        /// 设置钳位电压/电流 0x0021
        /// </summary>
        private void SendClamp(float voltagePos, float voltageNeg, float currentPos, float currentNeg)
        {
            List<byte> cmd = new List<byte> { 0x21, 0x00 };
            cmd.AddRange(BitConverter.GetBytes(voltagePos));
            cmd.AddRange(BitConverter.GetBytes(voltageNeg));
            cmd.AddRange(BitConverter.GetBytes(currentPos));
            cmd.AddRange(BitConverter.GetBytes(currentNeg));
            Send325GCommand(cmd.ToArray());
        }

        /// <summary>
        /// 阶梯上电/下电
        /// 1. 设置上下电通道 0x0121
        /// 2. 设置参数并启动 0x0122
        /// 3. 查询完成状态 0x0131
        /// </summary>
        private void StepPower(int channel, double value, int stepCount, bool powerOn)
        {
            if (stepCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(stepCount), "stepCount must be greater than 0.");

            float fValue = (float)Math.Round(value, 2);
            float startValue = powerOn ? 0f : fValue;
            float endValue = powerOn ? fValue : 0f;
            // 步进值由起止点推导，保证升降和正负值场景下方向正确
            float stepSize = (float)Math.Round((endValue - startValue) / stepCount, 2);

            List<byte> cmd1 = new List<byte> { 0x21, 0x01, (byte)channel, 0xFF };
            Send325GCommand(cmd1.ToArray());

            List<byte> cmd2 = new List<byte> { 0x22, 0x01 };
            cmd2.AddRange(BitConverter.GetBytes(startValue));
            cmd2.AddRange(BitConverter.GetBytes(stepSize));
            cmd2.AddRange(BitConverter.GetBytes(endValue));
            cmd2.AddRange(BitConverter.GetBytes(stepCount));
            cmd2.AddRange(BitConverter.GetBytes(StepDelayMs));
            Send325GCommand(cmd2.ToArray());

            Thread.Sleep(StepDelayMs * stepCount);
            for (int i = 0; i < 3; i++)
            {
                byte[] data = Send325GCommand(new byte[] { 0x31, 0x01 }, false);
                if (data != null && data.Length >= 1 && data[0] == 0x00)
                    return;
                Thread.Sleep(1000);
            }
            throw new Exception($"Board step power {(powerOn ? "on" : "off")} timeout");
        }

        #endregion

        #region 辅助方法

        private void SendOutputModeForSingleChannel(int channel, BoardDriverEnum.SourceType sourceType)
        {
            ValidateChannelRange(channel);

            // 默认电压源（bit=1）
            byte[] modeMask = Enumerable.Repeat((byte)0xFF, 6).ToArray();
            int byteIndex = channel / 8;
            byte channelBit = GetChannelBitMask(channel);
            if (sourceType == BoardDriverEnum.SourceType.VoltageSource)
                modeMask[byteIndex] |= channelBit;
            else
                modeMask[byteIndex] &= (byte)~channelBit;

            SendOutputMode(modeMask);
        }

        private void SendOutputModeForAllChannels(BoardDriverEnum.SourceType sourceType)
        {
            byte fill = sourceType == BoardDriverEnum.SourceType.VoltageSource ? (byte)0xFF : (byte)0x00;
            byte[] modeMask = Enumerable.Repeat(fill, 6).ToArray();
            SendOutputMode(modeMask);
        }

        /// <summary>
        /// 通道位映射：
        /// 每个 byte 的 bit 对应通道为 [0,1,2,3,4,5,6,7]。
        /// 即 channel0->bit0(0x01), channel7->bit7(0x80), channel8->byte1.bit0。
        /// </summary>
        private static byte GetChannelBitMask(int channel)
        {
            int bitIndex = channel % 8;
            return (byte)(1 << bitIndex);
        }

        private static byte[] BuildSingleChannelMask(int channel)
        {
            byte[] mask = new byte[6];
            mask[channel / 8] = GetChannelBitMask(channel);
            return mask;
        }

        private static void ValidateChannelRange(int channel)
        {
            if (channel < 0 || channel >= 48)
                throw new ArgumentOutOfRangeException(nameof(channel), "channel must be in range [0, 47].");
        }

        private static double ToFirmwareValue(double value, BoardDriverEnum.SourceType sourceType)
        {
            return sourceType == BoardDriverEnum.SourceType.VoltageSource
                ? value * 1000
                : value;
        }

        #endregion

        #region 325G 协议通讯层

        /// <summary>
        /// 发送 325G 指令并返回响应数据段。
        /// </summary>
        /// <param name="cmdPayload">指令字节（不含帧头/地址/CRC）</param>
        /// <param name="checkAck">是否做 ACK 校验（查询类指令传 false）</param>
        /// <returns>响应数据段（帧头/地址/指令回显/CRC/帧尾 已剥离）</returns>
        private byte[] Send325GCommand(byte[] cmdPayload, bool checkAck = true)
        {
            byte[] frame = WrapFrame(cmdPayload);

            for (int retry = 0; retry < RetryCount; retry++)
            {
                try
                {
                    byte[] response = TcpSendReceive(frame);
                    if (response == null || response.Length < 13) continue;

                    byte[] data = ValidateAndExtract(response);
                    return data;
                }
                catch
                {
                    Thread.Sleep(100);
                }
            }
            if (checkAck)
                throw new Exception("325G command failed after retries");
            return null;
        }

        /// <summary>
        /// 325G 帧封装：AA 55 [len_lo len_hi] [addr 00] [cmd...] [crc_lo crc_hi]
        /// </summary>
        private byte[] WrapFrame(byte[] cmdPayload)
        {
            List<byte> buf = new List<byte>();
            buf.Add(0xAA);
            buf.Add(0x55);
            buf.Add(0x00); // length placeholder
            buf.Add(0x00);
            buf.Add(BoardAddress);
            buf.Add(0x00);
            buf.AddRange(cmdPayload);

            int totalLen = buf.Count + 2; // +2 for CRC
            buf[2] = (byte)(totalLen & 0xFF);
            buf[3] = (byte)((totalLen >> 8) & 0xFF);

            byte[] crc = Crc16Modbus(buf.ToArray());
            buf.AddRange(crc);
            return buf.ToArray();
        }

        /// <summary>
        /// 校验 325G 响应帧并提取数据段。
        /// 响应格式：AA 55 [len 2B] [addr 2B] [cmd 2B] [data...] [CRC 2B] [0A 0D]
        /// </summary>
        private byte[] ValidateAndExtract(byte[] response)
        {
            if (response[0] != 0xAA || response[1] != 0x55)
                throw new Exception("Invalid 325G response header");
            if (response[response.Length - 2] != 0x0A || response[response.Length - 1] != 0x0D)
                throw new Exception("Invalid 325G response tail");

            int crcDataLen = response.Length - 4;
            byte[] crcInput = new byte[crcDataLen];
            Array.Copy(response, 0, crcInput, 0, crcDataLen);
            byte[] crcCalc = Crc16Modbus(crcInput);
            if (response[crcDataLen] != crcCalc[0] || response[crcDataLen + 1] != crcCalc[1])
                throw new Exception("325G response CRC mismatch");

            const int frontLen = 8; // AA(1)+55(1)+len(2)+addr(2)+cmd(2)
            int dataLen = crcDataLen - frontLen;
            if (dataLen <= 0) return new byte[0];

            byte[] data = new byte[dataLen];
            Array.Copy(response, frontLen, data, 0, dataLen);
            return data;
        }

        /// <summary>
        /// TCP 收发：发送帧数据，根据帧头长度字段动态读取完整响应。
        /// </summary>
        private byte[] TcpSendReceive(byte[] frameData)
        {
            lock (BoardClient)
            {
                try
                {
                    EnsureConnected();
                    NetworkStream stream = BoardClient.GetStream();

                    if (stream.DataAvailable)
                    {
                        byte[] discard = new byte[BoardClient.Available];
                        stream.Read(discard, 0, discard.Length);
                    }

                    stream.Write(frameData, 0, frameData.Length);
                    Thread.Sleep(50);

                    return ReadFullResponse(stream);
                }
                catch
                {
                    try
                    {
                        if (BoardClient != null && !BoardClient.Connected)
                            BoardClient.Close();
                    }
                    catch { }
                    return null;
                }
            }
        }

        private byte[] ReadFullResponse(NetworkStream stream)
        {
            List<byte> buf = new List<byte>();
            int elapsed = 0;
            int targetLen = -1;

            while (elapsed < ResponseTimeoutMs)
            {
                if (BoardClient.Available > 0)
                {
                    byte[] tmp = new byte[BoardClient.Available];
                    int read = stream.Read(tmp, 0, tmp.Length);
                    for (int i = 0; i < read; i++) buf.Add(tmp[i]);

                    if (targetLen < 0 && buf.Count >= 4
                        && buf[0] == 0xAA && buf[1] == 0x55)
                    {
                        int frameLen = buf[2] | (buf[3] << 8);
                        targetLen = frameLen + 2; // +0A 0D tail
                    }

                    if (targetLen > 0 && buf.Count >= targetLen)
                        return buf.GetRange(0, targetLen).ToArray();
                }
                Thread.Sleep(10);
                elapsed += 10;
            }
            return buf.Count > 0 ? buf.ToArray() : null;
        }

        private void EnsureConnected()
        {
            if (BoardClient.Connected) return;

            string[] parts = BoardClientConStr.Split(':');
            BoardClient.Connect(parts[0], Convert.ToInt32(parts[1]));
            NetworkStream ns = BoardClient.GetStream();
            ns.ReadTimeout = 3000;
            ns.WriteTimeout = 3000;
        }

        #endregion

        #region CRC16 (Modbus, 小端，与 BurninPlatform ByteHelper.CRC16_C 一致)

        private static byte[] Crc16Modbus(byte[] buffer)
        {
            byte lo = 0xFF, hi = 0xFF;
            const byte polyLo = 1, polyHi = 160;

            for (int i = 0; i < buffer.Length; i++)
            {
                lo ^= buffer[i];
                for (int j = 0; j < 8; j++)
                {
                    byte prevHi = hi, prevLo = lo;
                    hi = (byte)(hi >> 1);
                    lo = (byte)(lo >> 1);
                    if ((prevHi & 1) == 1) lo |= 0x80;
                    if ((prevLo & 1) == 1)
                    {
                        hi ^= polyHi;
                        lo ^= polyLo;
                    }
                }
            }
            return new byte[] { lo, hi };
        }

        #endregion
    }
}
