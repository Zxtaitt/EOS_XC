using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net.Sockets;

namespace BoardDriver
{
    public class BoardDriver_FW0422400 : IBoardDriver
    {
        protected Dictionary<FunctionCode, byte> FunctionCodeDic = new Dictionary<FunctionCode, byte>()
        {
            { FunctionCode.读取设备信息, 0x0E},
            { FunctionCode.读取输入寄存器, 0x04},
            { FunctionCode.写入配置寄存器, 0x46},
            { FunctionCode.读取配置寄存器, 0x47},
            { FunctionCode.读取保持寄存器, 0x03},
            { FunctionCode.写入保持寄存器, 0x06},
            { FunctionCode.校准寄存器, 0x50},
        };

        protected Dictionary<DataType, byte> DataTypeDic = new Dictionary<DataType, byte>()
        {
            { DataType.Null, 0x00},
            { DataType.Bool, 0x01},
            { DataType.U8, 0x02},
            { DataType.U16, 0x03},
            { DataType.Float, 0x05},
            { DataType.U32, 0x04},
            {DataType.U8Array,0x82},
            {DataType.FloatArray,0X85},
        };

        protected Dictionary<CommandID, byte[]> CommandIDDic = new Dictionary<CommandID, byte[]>()
        {
            { CommandID.通道是否输出, new byte[] { 0x01,0x00 } },
            { CommandID.设置通道DAC值,  new byte[] { 0x03,0x07 }},
            { CommandID.设置通道真实值,  new byte[] { 0x03,0x06 }},
            { CommandID.设置通道源类型,  new byte[] { 0x03,0x05 }},
            { CommandID.通道ID,  new byte[] { 0x00,0x00 }},
            { CommandID.通道类型,  new byte[] { 0x00,0x01 }},
            { CommandID.源工作模式, new byte[] { 0x03,0x04 }},
            { CommandID.插拔检测使能,  new byte[] { 0x00,0x98 }},
            { CommandID.开始读取通道值,  new byte[] { 0xF0,0x01 }},
            { CommandID.参数设置命令是否执行完毕,  new byte[] { 0x00,0x00 }},
            { CommandID.保存校准系数,  new byte[] { 0x00,0xE0 }},
            { CommandID.校准系数失能,  new byte[] { 0x00,0xE8 }},
            { CommandID.风扇控制开关,  new byte[] { 0x00,0x83 }},
            { CommandID.风扇控制转速,  new byte[] { 0x00,0x82 }},
            { CommandID.风扇PWM占空比控制,  new byte[] { 0x00,0x82 }},
            { CommandID.插拔检测状态,  new byte[] { 0xF0,0x00 }},
            { CommandID.LIV扫描状态,  new byte[] { 0xFF,0xFE }},
            { CommandID.电流正向钳制设置,  new byte[] { 0x00,0xA0 }},
            { CommandID.电流反向钳制设置,  new byte[] { 0x00,0xA1}},
            { CommandID.电压正向钳制设置,  new byte[] { 0x00,0xA2 }},
            { CommandID.电压反向钳制设置,  new byte[] { 0x00,0xA3 }},
            { CommandID.扫描的停止值,  new byte[] { 0x04,0x04 }},
            { CommandID.扫描的开始值,  new byte[] { 0x04,0x03 }},
            { CommandID.扫描阶梯次数,  new byte[] { 0x04,0x02 }},
            { CommandID.扫描阶梯等待时间,  new byte[] { 0x04,0x00 }},
            { CommandID.扫描阶梯步长,  new byte[] { 0x04,0x01 }},
            { CommandID.上电模式,  new byte[] { 0x03,0x01 }},
            { CommandID.开始扫描,  new byte[] { 0xF0,0x00 }},
            { CommandID.指示灯编号,  new byte[] { 0x00,0x8C}},
            { CommandID.指示灯状态,  new byte[] { 0x00,0x8E}},
            { CommandID.指示灯出厂模式,  new byte[] { 0x00,0x8D}},
            { CommandID.MeasureController复位,  new byte[] { 0x00,0xD0 }},
            { CommandID.MeasureSweep启动,  new byte[] { 0x00,0xD1 }},
            { CommandID.添加激励源通道,  new byte[] { 0x00,0xD2 }},
            { CommandID.采样数据类型,  new byte[] { 0x00,0xD3 }},
            { CommandID.采样通道,  new byte[] { 0x00,0xD4 }},
            { CommandID.LIV扫描的结束值,  new byte[] { 0x00,0xD5 }},
            { CommandID.LIV扫描的步进值,  new byte[] { 0x00,0xD6}},
            { CommandID.LIV扫描通道切换延迟值,  new byte[] { 0x00,0xD7 }},
            { CommandID.SwapRam数据类型,  new byte[] { 0x00,0xD8 }},
            { CommandID.SwapRamBlockId,  new byte[] { 0x00,0xD9 }},
            { CommandID.选择TOMPD位号,      new byte[] { 0x00,0xA3 }},
            { CommandID.切换默认通道,  new byte[] { 0x00,0x00 }},
            { CommandID.切换PD通道,  new byte[] { 0x00,0x01 }},
            { CommandID.切换MPD通道,  new byte[] { 0x00,0x02 }},
            { CommandID.切换NTC通道,  new byte[] { 0x00,0x03 }},
            { CommandID.电流对象ID,  new byte[] { 0x00,0x00 }},
            { CommandID.电压对象ID,  new byte[] { 0x00,0x01 }},
            { CommandID.TO芯片PIN1脚状态,  new byte[] { 0x00,0x88 }},
            { CommandID.TO芯片PIN2脚状态,  new byte[] { 0x00,0x89 }},
            { CommandID.TO芯片PIN3脚状态,  new byte[] { 0x00,0x8A }},
            { CommandID.TO芯片PIN4脚状态,  new byte[] { 0x00,0x8B }},
            { CommandID.切换抽屉板,      new byte[] { 0x00,0xED }},
            { CommandID.切换功能通道电压支持的方向,      new byte[] { 0x00,0xEC }},
            { CommandID.电源输出,  new byte[] { 0x03,0x03 }},
            { CommandID.插拔检测失能,  new byte[] { 0x00,0x8F }},
            { CommandID.扫描总点数,  new byte[] { 0x90,0x00 }},
            { CommandID.当前扫描的进度,  new byte[] { 0x90,0x01 }},
            { CommandID.判断扫描是否完成,  new byte[] { 0x90,0x02 }},
            { CommandID.SwapRamID,  new byte[] { 0x10,0x00 }},
            { CommandID.PD电压对象ID,  new byte[] { 0x00,0x03 }},
            { CommandID.PD电流对象ID,  new byte[] { 0x00,0x02 }}
        };

        protected enum FunctionCode
        {
            读取设备信息,
            读取输入寄存器,
            写入配置寄存器,
            读取配置寄存器,
            读取保持寄存器,
            写入保持寄存器,
            校准寄存器,
        }

        protected enum DataType
        {
            Null,
            Bool,
            U8,
            U16,
            U32,
            Float,
            U8Array,
            FloatArray
        }

        protected enum CommandID
        {
            通道是否输出,
            设置通道DAC值,
            设置通道真实值,
            设置通道源类型,
            源工作模式,
            插拔检测使能,
            开始读取通道值,
            获取通道值,
            参数设置命令是否执行完毕,
            保存校准系数,
            风扇控制开关,
            风扇控制转速,
            风扇PWM占空比控制,
            插拔检测状态,
            LIV扫描状态,
            电流正向钳制设置,
            电流反向钳制设置,
            电压正向钳制设置,
            电压反向钳制设置,
            扫描的停止值,//老化为目标值
            扫描的开始值,//老化不要设
            扫描阶梯次数,
            上电模式,
            开始扫描,
            红灯状态,
            绿灯状态,
            指示灯编号,
            指示灯出厂模式,
            指示灯状态,
            扫描阶梯等待时间,
            扫描阶梯步长,
            添加LIV的LD通道,
            添加LIV的FIXED通道,
            添加LIV的MPD通道,
            添加LIV的PD通道,
            LIV扫描的开始值,
            LIV扫描的结束值,
            LIV扫描的步进值,
            LIV扫描通道切换延迟值,
            LIV扫描开始,
            LIV扫描结束,
            MeasureController复位,
            MeasureSweep启动,
            添加激励源通道,
            采样数据类型,
            采样通道,
            SwapRam数据类型,
            SwapRamBlockId,
            SwapRamID,
            切换默认通道,
            切换PD通道,
            切换MPD通道,
            切换NTC通道,
            TO芯片PIN1脚状态,
            TO芯片PIN2脚状态,
            TO芯片PIN3脚状态,
            TO芯片PIN4脚状态,
            切换抽屉板,
            切换功能通道电压支持的方向,
            选择TOMPD位号,
            插拔检测失能,
            校准系数失能,
            电源输出,
            切换TH通道,
            通道ID,
            通道类型,
            电流对象ID,
            电压对象ID,
            PD电流对象ID,
            PD电压对象ID,
            MPD电流对象ID,
            MPD电压对象ID,
            TH电流对象ID,
            TH电压对象ID,
            扫描总点数,
            当前扫描的进度,
            判断扫描是否完成,
            电流采样档位
        }

        public TcpClient BoardClient { get; set; }

        public byte BoardAddress
        { get; set; }

        public SerialPort BoardSerialPort
        { get; set; }

        public string BoardClientConStr { get; set; }

        protected byte[] WaitCommandEnd(byte[] Send, int Sleep)
        {
            try
            {
                Send = WrapCmd(Send);
                if (!BoardSerialPort.IsOpen)
                {
                    BoardSerialPort.ReadTimeout = 1500;
                    BoardSerialPort.WriteTimeout = 1500;
                    BoardSerialPort.Open();
                }
                BoardSerialPort.DiscardInBuffer();
                BoardSerialPort.DiscardOutBuffer();
                BoardSerialPort.Write(Send, 0, Send.Length);
                System.Threading.Thread.Sleep(Sleep);
                var Length = 0;
                for (int i = 0; i < 500; i++)
                {
                    if (BoardSerialPort.BytesToRead >= 4)
                    {
                        byte[] readBuffer = new byte[4];
                        BoardSerialPort.Read(readBuffer, 0, 4);
                        Length = BitConverter.ToInt16(new byte[] { readBuffer[3], readBuffer[2] }, 0);
                        Length -= 4;
                        break;
                    }
                    System.Threading.Thread.Sleep(2);
                }
                if (Length == 0)
                {
                    return new byte[1];
                }
                for (int i = 0; i < 500; i++)
                {
                    if (BoardSerialPort.BytesToRead == Length)
                    {
                        byte[] readBuffer = new byte[Length];
                        BoardSerialPort.Read(readBuffer, 0, readBuffer.Length);
                        return readBuffer;
                    }
                    System.Threading.Thread.Sleep(2);
                }
                return new byte[1];
            }
            catch
            {
                if (!BoardSerialPort.IsOpen)
                {
                    BoardSerialPort.Close();
                }
                return new byte[1];
            }
        }

        /// <summary>
        /// 获取驱动板电流
        /// </summary>
        /// <param name="Channel"></param>
        /// <returns></returns>
        protected double GetBoardCurrent(int Channel)
        {
            int retryOut = 3;
            int retry = 0;
            do
            {
                List<byte> sendCmd = new List<byte>();
                sendCmd.Add(FunctionCodeDic[FunctionCode.写入配置寄存器]);
                sendCmd.Add(0x03);//动作数

                sendCmd.AddRange(CommandIDDic[CommandID.通道ID]);
                sendCmd.Add(DataTypeDic[DataType.U8Array]);//0x82
                sendCmd.Add(0x00);
                sendCmd.Add(0x01);

                sendCmd.AddRange(new byte[] { (byte)Channel });
                sendCmd.AddRange(CommandIDDic[CommandID.通道类型]);
                sendCmd.Add(DataTypeDic[DataType.U8]);
                sendCmd.Add(0x00);//PD通道类型

                sendCmd.AddRange(CommandIDDic[CommandID.开始读取通道值]);
                sendCmd.Add(DataTypeDic[DataType.Null]);
                byte[] response = this.QueryProduct(sendCmd.ToArray(), 0);
                if (response.Length > 1 && CheckFeedback(response[1]))
                {
                    break;
                }

                retry++;
            } while (retry < retryOut);
            if (retry == retryOut)
            {
                throw new Exception("Start Reading Single Channel Current Exception");
            }
            retry = 0;
            do
            {
                List<byte> sendCmd = new List<byte>();
                sendCmd.Add(FunctionCodeDic[FunctionCode.读取输入寄存器]);

                sendCmd.Add(0x00);
                sendCmd.AddRange(CommandIDDic[CommandID.电流对象ID]);
                byte[] response = this.QueryProduct(sendCmd.ToArray(), 0);
                if (response.Length > 1 && CheckFeedback(response[1]))
                {
                    List<double> currentList = new List<double>();

                    //double ret = 0;
                    for (int i = 9; i < (96 * 4) + 9; i++)
                    {
                        var Dac = BitConverter.ToSingle(new byte[] { response[i], response[i + 1], response[i + 2], response[i + 3] }, 0);

                        //ret = Tool.FormulaToValue(TheFormula, Convert.ToDouble(Dac));
                        //ret = ret * 1e-3;
                        currentList.Add(Dac);
                        i = i + 3;
                    }

                    // var Dac = BitConverter.ToUInt16(new byte[] { response[8], response[7] },0);

                    return currentList[Channel];
                }

                retry++;
            } while (retry < retryOut);
            throw new Exception("Reading Single Channel Real Current Exception");
        }

        protected virtual byte[] QueryProduct(byte[] Send, int Sleep)
        {
            lock (BoardSerialPort)
            {
                try
                {
                    Send = WrapCmd(Send);
                    if (!BoardSerialPort.IsOpen)
                    {
                        BoardSerialPort.ReadTimeout = 1500;
                        BoardSerialPort.WriteTimeout = 1500;
                        BoardSerialPort.Open();
                    }
                    BoardSerialPort.DiscardInBuffer();
                    BoardSerialPort.DiscardOutBuffer();
                    BoardSerialPort.Write(Send, 0, Send.Length);
                    System.Threading.Thread.Sleep(200);
                    var Length = 0;
                    for (int i = 0; i < 1000; i++)
                    {
                        if (BoardSerialPort.BytesToRead >= 4)
                        {
                            byte[] readBuffer = new byte[4];
                            BoardSerialPort.Read(readBuffer, 0, 4);
                            Length = BitConverter.ToInt16(new byte[] { readBuffer[3], readBuffer[2] }, 0);
                            Length -= 4;
                            break;
                        }
                        System.Threading.Thread.Sleep(2);
                    }
                    if (Length == 0)
                    {
                        return new byte[1];
                    }
                    for (int i = 0; i < 1500; i++)
                    {
                        if (BoardSerialPort.BytesToRead == Length)
                        {
                            byte[] readBuffer = new byte[Length];
                            BoardSerialPort.Read(readBuffer, 0, readBuffer.Length);
                            if (readBuffer[1] == 0x05)
                            {
                                for (int j = 0; j < 15000; j++)
                                {
                                    List<byte> sendCmd = new List<byte>();
                                    sendCmd.Add(FunctionCodeDic[FunctionCode.读取输入寄存器]);
                                    sendCmd.Add(0x0F);
                                    sendCmd.AddRange(new byte[] { 0xFF, 0xFF });
                                    byte[] response1 = this.WaitCommandEnd(sendCmd.ToArray(), 20);
                                    if (response1.Length >= 12)
                                    {
                                        switch (response1[11])
                                        {
                                            case 0x00:
                                                return readBuffer;

                                            case 0x02:
                                                throw new Exception("Command Execution Failed");
                                        }
                                    }
                                }
                                throw new Exception("Command Execution Failed");
                            }
                            return readBuffer;
                        }
                        System.Threading.Thread.Sleep(2);
                    }
                    return new byte[1];
                }
                catch
                {
                    if (!BoardSerialPort.IsOpen)
                    {
                        BoardSerialPort.Close();
                    }
                    return new byte[1];
                }
            }
        }

        protected virtual bool CheckFeedback(byte response)
        {
            switch (response)
            {
                case 0x00:
                    return true;

                case 0x05:

                    return true;

                default:
                    return false;
            }
        }

        protected virtual byte[] WrapCmd(byte[] cmd)
        {
            List<byte> sendCmd = new List<byte>();
            short Length = Convert.ToInt16(cmd.Length + 7);
            var LengthBytes = BitConverter.GetBytes(Length);
            Array.Reverse(LengthBytes);
            sendCmd.AddRange(new byte[] { 0xAA, 0x55 });
            sendCmd.AddRange(LengthBytes);
            sendCmd.AddRange(new byte[] { BoardAddress });
            sendCmd.AddRange(cmd);
            byte[] Byte = Tool.StringToHexByte(Tool.ToModbusCRC16(sendCmd.ToArray()));
            Array.Reverse(Byte);
            sendCmd.AddRange(Byte);
            return sendCmd.ToArray();
        }

        protected virtual void SetChannelOffValue(int Channel, double Value, bool PowerMethodIsSingle, int StepCount)
        {
            int retry = 0;
            do
            {
                byte[] Byte = BitConverter.GetBytes(Convert.ToSingle(Value));
                List<byte> sendCmd = new List<byte>();
                sendCmd.Add(FunctionCodeDic[FunctionCode.写入配置寄存器]);
                sendCmd.Add(0x07);

                sendCmd.AddRange(CommandIDDic[CommandID.通道ID]);
                sendCmd.Add(DataTypeDic[DataType.U8Array]);//数据表达方式0x82 FLOAT ARRAY
                sendCmd.Add(0x00);
                sendCmd.Add(0x01);
                sendCmd.AddRange(new byte[] { (byte)Channel });

                sendCmd.AddRange(CommandIDDic[CommandID.上电模式]);
                sendCmd.Add(DataTypeDic[DataType.U8]);

                sendCmd.Add(0x01);//4224 0:直接上电 1：Sweep 2：list

                sendCmd.AddRange(CommandIDDic[CommandID.扫描的开始值]);
                sendCmd.Add(DataTypeDic[DataType.Float]);
                sendCmd.AddRange(Byte);

                sendCmd.AddRange(CommandIDDic[CommandID.扫描的停止值]);
                sendCmd.Add(DataTypeDic[DataType.Float]);
                sendCmd.AddRange(BitConverter.GetBytes(Convert.ToSingle(0)));
                if (PowerMethodIsSingle)
                {
                    StepCount = 1;
                }
                sendCmd.AddRange(CommandIDDic[CommandID.扫描阶梯次数]);
                sendCmd.Add(DataTypeDic[DataType.U16]);
                sendCmd.AddRange(new byte[] { (byte)StepCount, 0x00, });

                sendCmd.AddRange(CommandIDDic[CommandID.扫描阶梯等待时间]);
                sendCmd.Add(DataTypeDic[DataType.U16]);
                sendCmd.AddRange(new byte[] { 0x32, 0x00, });//为什么这里32

                sendCmd.AddRange(CommandIDDic[CommandID.开始扫描]);
                sendCmd.Add(DataTypeDic[DataType.Null]);

                byte[] response = this.QueryProduct(sendCmd.ToArray(), 200);
                if (response.Length > 1 && CheckFeedback(response[1]))
                {
                    return;
                }
                retry++;
            } while (retry < 9);

            throw new Exception("Set Board Value Exception");
        }

        protected virtual void SetChannelValue(int Channel, double Value, BoardDriverEnum.PowerMethod PowerMethod, int StepCount)
        {
            int retry = 0;
            do
            {
                byte[] Byte = BitConverter.GetBytes(Convert.ToSingle(Value));
                List<byte> sendCmd = new List<byte>();
                sendCmd.Add(FunctionCodeDic[FunctionCode.写入配置寄存器]);
                sendCmd.Add(0x07);

                sendCmd.AddRange(CommandIDDic[CommandID.通道ID]);
                sendCmd.Add(DataTypeDic[DataType.U8Array]);//数据表达方式0x82 FLOAT ARRAY
                sendCmd.Add(0x00);
                sendCmd.Add(0x01);
                sendCmd.AddRange(new byte[] { (byte)Channel });

                sendCmd.AddRange(CommandIDDic[CommandID.上电模式]);
                sendCmd.Add(DataTypeDic[DataType.U8]);

                sendCmd.Add(0x01);//4224 0:直接上电 1：Sweep 2：list

                sendCmd.AddRange(CommandIDDic[CommandID.扫描的开始值]);
                sendCmd.Add(DataTypeDic[DataType.Float]);
                sendCmd.AddRange(BitConverter.GetBytes(Convert.ToSingle(0)));

                sendCmd.AddRange(CommandIDDic[CommandID.扫描的停止值]);
                sendCmd.Add(DataTypeDic[DataType.Float]);
                sendCmd.AddRange(Byte);

                sendCmd.AddRange(CommandIDDic[CommandID.扫描阶梯次数]);
                sendCmd.Add(DataTypeDic[DataType.U16]);
                sendCmd.AddRange(new byte[] { (byte)StepCount, 0x00, });

                sendCmd.AddRange(CommandIDDic[CommandID.扫描阶梯等待时间]);
                sendCmd.Add(DataTypeDic[DataType.U16]);
                sendCmd.AddRange(new byte[] { 0x32, 0x00, });//为什么这里32

                sendCmd.AddRange(CommandIDDic[CommandID.开始扫描]);
                sendCmd.Add(DataTypeDic[DataType.Null]);

                byte[] response = this.QueryProduct(sendCmd.ToArray(), 200);
                if (response.Length > 1 && CheckFeedback(response[1]))
                {
                    return;
                }
                retry++;
            } while (retry < 9);

            throw new Exception("Set Board Value Exception");
        }

        /// <summary>
        /// 设置单通道
        /// </summary>
        /// <param name="Channel"></param>
        protected void SetBoardOnSingleChannel(int Channel)
        {
            int retry = 0;
            do
            {
                List<byte> sendCmd = new List<byte>();

                sendCmd.Add(FunctionCodeDic[FunctionCode.写入配置寄存器]);
                sendCmd.Add(0x02);//对象数量
                sendCmd.AddRange(CommandIDDic[CommandID.通道ID]);
                sendCmd.Add(DataTypeDic[DataType.U8Array]);//0x82
                sendCmd.Add(0x00);
                sendCmd.Add(0x01);
                sendCmd.AddRange(new byte[] { 0xFF });

                sendCmd.AddRange(CommandIDDic[CommandID.通道是否输出]);
                sendCmd.Add(DataTypeDic[DataType.Bool]);
                sendCmd.Add(0x01);
                byte[] response = this.QueryProduct(sendCmd.ToArray(), 0);
                if (response.Length > 1 && CheckFeedback(response[1]))
                {
                    return;
                }
                retry++;
            } while (retry < 9);
            throw new Exception("Set DVB Board On Single Exception");
        }

        //}
        public virtual void SetBoardOffSingleChannel(int Channel)
        {
            int retry = 0;
            do
            {
                List<byte> sendCmd = new List<byte>();
                sendCmd.Add(FunctionCodeDic[FunctionCode.写入配置寄存器]);
                sendCmd.Add(0x03);//动作数
                sendCmd.AddRange(CommandIDDic[CommandID.通道ID]);
                sendCmd.Add(DataTypeDic[DataType.U8Array]);//0x82
                sendCmd.Add(0x00);
                sendCmd.Add(0x01);
                sendCmd.AddRange(new byte[] { (byte)Channel });
                sendCmd.AddRange(CommandIDDic[CommandID.通道类型]);
                sendCmd.Add(DataTypeDic[DataType.U8]);
                sendCmd.Add(0x00);

                sendCmd.AddRange(CommandIDDic[CommandID.通道是否输出]);
                sendCmd.Add(DataTypeDic[DataType.Bool]);
                sendCmd.Add(0x00);
                byte[] response = this.QueryProduct(sendCmd.ToArray(), 0);
                if (response.Length > 1 && CheckFeedback(response[1]))
                {
                    return;
                }
                retry++;
            } while (retry < 9);
            throw new Exception("Set DVB Board Off Single Exception");
        }

        public virtual void SetBoardClamp(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.Direction Direction)
        {
            SetBoardOffAllChannel();
            SetBoardClampBeforeOpenChannel();
            OpenChannel(Channel, SourceType, Direction);
            SetBoardClampAfterOpenChannel(SourceType, Direction);
        }

        public void SetBoardOnPower(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.PowerMethod PowerMethod, int Step, double SetValue)
        {
            SetChannelValue(Channel, SetValue, PowerMethod, Step);
        }
        public void SetBoardAdjustDriveVoltage()
        {

        }
        public virtual void SetBoardOFFPower(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.PowerMethod PowerMethod, int Step, double SetValue)
        {
            double ElectricityValue = -0.5;
            if (SetValue > 0)
            {
                ElectricityValue = 0.5;
            }
            switch (SourceType)
            {
                case BoardDriverEnum.SourceType.VoltageSource:
                    ElectricityValue = 0;
                    break;

                default: break;
            }

            SetValue = GetBoardCurrent(Channel) - 10;
            //SetChannelValue(Channel, ElectricityValue, PowerMethod, Step);
            SetChannelOffValue(Channel, SetValue, false, Step);

            SetBoardOffSingleChannel(Channel);
        }

        public void CloseBoardSerialPort()
        {
            lock (BoardSerialPort)
            {
                if (BoardSerialPort.IsOpen)
                {
                    BoardSerialPort.Close();
                }
            }
        }

        public void SetBoardOffAllChannel()
        {
            int retry = 0;
            do
            {
                List<byte> sendCmd = new List<byte>();
                sendCmd.Add(FunctionCodeDic[FunctionCode.写入配置寄存器]);
                sendCmd.Add(0x02);//动作数

                sendCmd.Add(0x00);
                sendCmd.Add(0x00);
                sendCmd.Add(DataTypeDic[DataType.U8Array]);//0x82
                sendCmd.Add(0x00);
                sendCmd.Add(0x01);
                sendCmd.AddRange(new byte[] { 0xFF });//选中所有通道

                sendCmd.AddRange(CommandIDDic[CommandID.通道是否输出]);
                sendCmd.Add(DataTypeDic[DataType.Bool]);
                sendCmd.Add(0x00);
                byte[] response = this.QueryProduct(sendCmd.ToArray(), 0);
                if (response.Length > 1 && CheckFeedback(response[1]))
                {
                    return;
                }
                retry++;
            } while (retry < 9);
            throw new Exception("Set DVB Board Off All Channels Exception");
        }

        public void SetBoardClampBeforeOpenChannel()
        {
        }

        public virtual void OpenChannel(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.Direction Direction)
        {
            double BeforeValue = 0;
            switch (SourceType)
            {
                case BoardDriverEnum.SourceType.CurrentSource:

                    switch (Direction)
                    {
                        case BoardDriverEnum.Direction.Positive:
                            BeforeValue = -1;
                            break;

                        case BoardDriverEnum.Direction.Negative:
                            BeforeValue = 1;
                            break;
                    }
                    List<byte> sendCmd = new List<byte>();
                    sendCmd.Add(FunctionCodeDic[FunctionCode.写入配置寄存器]);

                    sendCmd.Add(0x03);//对象数量
                    sendCmd.AddRange(CommandIDDic[CommandID.通道ID]);
                    sendCmd.Add(DataTypeDic[DataType.U8Array]);//数据表达方式0x82 FLOAT ARRAY

                    sendCmd.Add(0x00);
                    sendCmd.Add(0x01);//选中通道数量

                    sendCmd.Add(Convert.ToByte(Channel)); //选中的通道
                    sendCmd.AddRange(CommandIDDic[CommandID.通道类型]);
                    sendCmd.Add(DataTypeDic[DataType.U8]);
                    sendCmd.Add(0x00);

                    byte[] Byte = BitConverter.GetBytes(Convert.ToSingle(BeforeValue));

                    sendCmd.AddRange(CommandIDDic[CommandID.通道是否输出]);
                    sendCmd.Add(DataTypeDic[DataType.Bool]);
                    sendCmd.Add(0x01);
                    byte[] response = this.QueryProduct(sendCmd.ToArray(), 0);

                    break;

                case BoardDriverEnum.SourceType.VoltageSource:

                    sendCmd = new List<byte>();
                    sendCmd.Add(FunctionCodeDic[FunctionCode.写入配置寄存器]);

                    sendCmd.Add(0x03);//对象数量
                    sendCmd.AddRange(CommandIDDic[CommandID.通道ID]);
                    sendCmd.Add(DataTypeDic[DataType.U8Array]);//数据表达方式0x82 FLOAT ARRAY

                    sendCmd.Add(0x00);
                    sendCmd.Add(0x01);//选中通道数量

                    sendCmd.Add(Convert.ToByte(Channel)); //选中的通道
                    sendCmd.AddRange(CommandIDDic[CommandID.通道类型]);
                    sendCmd.Add(DataTypeDic[DataType.U8]);
                    sendCmd.Add(0x00);

                    Byte = BitConverter.GetBytes(Convert.ToSingle(BeforeValue));

                    sendCmd.AddRange(CommandIDDic[CommandID.通道是否输出]);
                    sendCmd.Add(DataTypeDic[DataType.Bool]);
                    sendCmd.Add(0x01);
                    response = this.QueryProduct(sendCmd.ToArray(), 0);
                    break;

                default:
                    break;
            }
        }

        public void SetBoardClampAfterOpenChannel(BoardDriverEnum.SourceType SourceType, BoardDriverEnum.Direction Direction)
        {
        }

        public void CloseRelay(int channel)
        {
            throw new NotImplementedException();
        }

        public void OpenRelay(int channel)
        {
            throw new NotImplementedException();
        }
    }
}