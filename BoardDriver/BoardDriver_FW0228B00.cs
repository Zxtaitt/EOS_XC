using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;

namespace BoardDriver
{

    public class BoardDriver_FW0228B00 : IBoardDriver
    {
     
        enum FunctionCode
        {
            读取设备信息,
            读取输入寄存器,
            写入配置寄存器,
            读取配置寄存器,
            读取保持寄存器,
            写入保持寄存器,
            校准寄存器,
        }
        enum DataType
        {
            Null,
            Bool,
            U8,
            U16,
            Float

        }
        enum CommandID
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
            风扇控制,
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

        }
        Dictionary<FunctionCode, byte> FunctionCodeDic = new Dictionary<FunctionCode, byte>()
        {
            { FunctionCode.读取设备信息, 0x0E},
            { FunctionCode.读取输入寄存器, 0x04},
            { FunctionCode.写入配置寄存器, 0x46},
            { FunctionCode.读取配置寄存器, 0x47},
            { FunctionCode.读取保持寄存器, 0x03},
            { FunctionCode.写入保持寄存器, 0x06},
            { FunctionCode.校准寄存器, 0x50},
        };
        Dictionary<DataType, byte> DataTypeDic = new Dictionary<DataType, byte>()
        {
            { DataType.Null, 0x00},
            { DataType.Bool, 0x01},
            { DataType.U8, 0x02},
            { DataType.U16, 0x03},
            { DataType.Float, 0x04},

        };
        Dictionary<CommandID, byte[]> CommandIDDic = new Dictionary<CommandID, byte[]>()
        {
            { CommandID.通道是否输出, new byte[] { 0x01,0x00 } },
            { CommandID.设置通道DAC值,  new byte[] { 0x03,0x07 }},
            { CommandID.设置通道真实值,  new byte[] { 0x03,0x06 }},
            { CommandID.设置通道源类型,  new byte[] { 0x03,0x05 }},
            { CommandID.源工作模式, new byte[] { 0x03,0x04 }},
            { CommandID.插拔检测使能,  new byte[] { 0x00,0x98 }},
            { CommandID.开始读取通道值,  new byte[] { 0xF0,0x01 }},
            { CommandID.参数设置命令是否执行完毕,  new byte[] { 0x00,0x00 }},
            { CommandID.保存校准系数,  new byte[] { 0x00,0xE0 }},
            { CommandID.风扇控制,  new byte[] { 0x00,0x90 }},
            { CommandID.插拔检测状态,  new byte[] { 0xFF,0xFF }},
            { CommandID.LIV扫描状态,  new byte[] { 0xFF,0xFE }},
            { CommandID.电流正向钳制设置,  new byte[] { 0x00,0x80 }},
            { CommandID.电流反向钳制设置,  new byte[] { 0x00,0x81}},
            { CommandID.电压正向钳制设置,  new byte[] { 0x00,0x82 }},
            { CommandID.电压反向钳制设置,  new byte[] { 0x00,0x83 }},
            { CommandID.扫描的停止值,  new byte[] { 0x03,0x0C }},
            { CommandID.扫描的开始值,  new byte[] { 0x03,0x0B }},
            { CommandID.扫描阶梯次数,  new byte[] { 0x03,0x0A }},
            { CommandID.扫描阶梯等待时间,  new byte[] { 0x03,0x08 }},
            { CommandID.扫描阶梯步长,  new byte[] { 0x03,0x09 }},
            { CommandID.上电模式,  new byte[] { 0x03,0x04 }},
            { CommandID.开始扫描,  new byte[] { 0xF0,0x00 }},
            { CommandID.红灯状态,  new byte[] { 0x00,0x9D }},
            { CommandID.绿灯状态,  new byte[] { 0x00,0x9C }},
            { CommandID.添加LIV的LD通道,  new byte[] { 0x00,0xD0 }},
            { CommandID.添加LIV的FIXED通道,  new byte[] { 0x00,0xD1 }},
            { CommandID.添加LIV的MPD通道,  new byte[] { 0x00,0xD2 }},
            { CommandID.添加LIV的PD通道,  new byte[] { 0x00,0xD3 }},
            { CommandID.LIV扫描的开始值,  new byte[] { 0x00,0xD4 }},
            { CommandID.LIV扫描的结束值,  new byte[] { 0x00,0xD5 }},
            { CommandID.LIV扫描的步进值,  new byte[] { 0x00,0xD6}},
            { CommandID.LIV扫描通道切换延迟值,  new byte[] { 0x00,0xD7 }},
            { CommandID.LIV扫描开始,  new byte[] { 0x00,0xD8 }},
             { CommandID.LIV扫描结束,  new byte[] { 0x00,0xD9 }},
        };
      
        public byte BoardAddress
        { get; set; }
        public SerialPort BoardSerialPort
        { get; set; }
        public string BoardClientConStr { get ; set ; }
        public TcpClient BoardClient { get; set; }
        public void SetBoardAdjustDriveVoltage()
        {

        }
        private byte[] WaitCommandEnd(byte[] Send, int Sleep)
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
        protected byte[] QueryProduct(byte[] Send, int Sleep)
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
                            if (readBuffer[1] == 0x05)
                            {
                                while (true)
                                {
                                    List<byte> sendCmd = new List<byte>();
                                    sendCmd.Add(FunctionCodeDic[FunctionCode.读取保持寄存器]);
                                    sendCmd.AddRange(CommandIDDic[CommandID.参数设置命令是否执行完毕]);
                                    sendCmd.AddRange(new byte[] { 0x00, 0x01 });
                                    byte[] response1 = this.WaitCommandEnd(sendCmd.ToArray(), 0);
                                    switch (response1[5])
                                    {
                                        case 0x01:
                                            return readBuffer;
                                        case 0x02:
                                            throw new Exception("Command Execution Failed");
                                    }

                                }
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

        protected virtual void SetBoardOnSingleChannel(int Channel)
        {
            int retry = 0;
            do
            {


                List<byte> sendCmd = new List<byte>();
                sendCmd.Add(FunctionCodeDic[FunctionCode.写入配置寄存器]);
                sendCmd.Add(0x02);//动作数
                sendCmd.AddRange(new byte[] { 0x00, (byte)Channel });
                sendCmd.Add(DataTypeDic[DataType.Bool]);
                sendCmd.Add(0x01);
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
        public void SetBoardOffSingleChannel(int Channel)
        {
            int retry = 0;
            do
            {
                List<byte> sendCmd = new List<byte>();
                sendCmd.Add(FunctionCodeDic[FunctionCode.写入配置寄存器]);
                sendCmd.Add(0x02);//动作数
                sendCmd.AddRange(new byte[] { 0x00, (byte)Channel });
                sendCmd.Add(DataTypeDic[DataType.Bool]);
                sendCmd.Add(0x01);
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

      


      
        protected virtual void SetChannelValue(int Channel, double Value, BoardDriverEnum.PowerMethod PowerMethod, int StepCount)
        {
            int retry = 0;
            do
            {

                byte[] Byte = BitConverter.GetBytes(Convert.ToSingle(Value));
                List<byte> sendCmd = new List<byte>();
                sendCmd.Add(FunctionCodeDic[FunctionCode.写入配置寄存器]);
                sendCmd.Add(0x06);
                sendCmd.AddRange(new byte[] { 0x00, (byte)Channel });
                sendCmd.Add(DataTypeDic[DataType.Bool]);
                sendCmd.Add(0x01);
                sendCmd.AddRange(CommandIDDic[CommandID.上电模式]);
                sendCmd.Add(DataTypeDic[DataType.U8]);
                sendCmd.Add(0x02);
                sendCmd.AddRange(CommandIDDic[CommandID.扫描的停止值]);
                sendCmd.Add(DataTypeDic[DataType.Float]);
                sendCmd.AddRange(Byte);

                switch (PowerMethod)
                {
                    case BoardDriverEnum.PowerMethod.Single:
                        StepCount = 1;
                        break;
                    case BoardDriverEnum.PowerMethod.Step:
                        StepCount = StepCount;
                        break;
                    default:
                        break;
                }

                sendCmd.AddRange(CommandIDDic[CommandID.扫描阶梯次数]);
                sendCmd.Add(DataTypeDic[DataType.U16]);
                sendCmd.AddRange(new byte[] { 0x00, (byte)StepCount });

                sendCmd.AddRange(CommandIDDic[CommandID.扫描阶梯等待时间]);
                sendCmd.Add(DataTypeDic[DataType.U16]);
                sendCmd.AddRange(new byte[] { 0x00, 0x32 });

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

        private void CloseBoardPlugDetection()
        {
            int retryOut = 9;
            int retry = 0;
            do
            {


                List<byte> sendCmd = new List<byte>();
                sendCmd.Add(FunctionCodeDic[FunctionCode.写入配置寄存器]);
                sendCmd.Add(0x01); ;
                sendCmd.AddRange(CommandIDDic[CommandID.插拔检测使能]);
                sendCmd.Add(DataTypeDic[DataType.Bool]);
                sendCmd.Add(0x00);
                byte[] response = this.QueryProduct(sendCmd.ToArray(), 0);
                if (response.Length > 1 && CheckFeedback(response[1]))
                {
                    return;
                }
                retry++;
            } while (retry < retryOut);
            throw new Exception("Close DVB Board Plug Detection Exception");
        }

        public void SetBoardClamp(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.Direction Direction)
        {
            CloseBoardPlugDetection();
            SetBoardOffAllChannel();
            //SetBoardClampBeforeOpenChannel();
            //OpenChannel(Channel, SourceType, Direction);
            //SetBoardClampAfterOpenChannel(SourceType, Direction);
            SetBoardOnSingleChannel(Channel);

        }

        public void SetBoardOnPower(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.PowerMethod PowerMethod, int Step, double SetValue)
        {
            SetChannelValue(Channel, SetValue, PowerMethod, Step);
        }

        public void SetBoardOFFPower(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.PowerMethod PowerMethod, int Step, double SetValue)
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
         

            SetChannelValue(Channel, ElectricityValue, PowerMethod, Step);

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
            //int retryOut = 3;
            int retry = 0;
            do
            {


                List<byte> sendCmd = new List<byte>();
                sendCmd.Add(FunctionCodeDic[FunctionCode.写入配置寄存器]);
                sendCmd.Add(49);//动作数
                for (int i = 0; i < 48; i++)
                {


                    sendCmd.AddRange(new byte[] { 0x00, (byte)i });
                    sendCmd.Add(DataTypeDic[DataType.Bool]);
                    sendCmd.Add(0x01);

                }
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
            var VoltageClamp_Pos = BitConverter.GetBytes(0f);
            var VoltageClamp_Neg = BitConverter.GetBytes(0f);
            var CurrentClamp_Pos = BitConverter.GetBytes(25f);
            var CurrentClamp_Neg = BitConverter.GetBytes(25f);
            List<byte> sendCmd = new List<byte>();
            sendCmd.Add(FunctionCodeDic[FunctionCode.写入配置寄存器]);
            sendCmd.Add(0x04);

            sendCmd.AddRange(CommandIDDic[CommandID.电压正向钳制设置]);
            sendCmd.Add(DataTypeDic[DataType.Float]);
            sendCmd.AddRange(VoltageClamp_Pos);
            sendCmd.AddRange(CommandIDDic[CommandID.电压反向钳制设置]);
            sendCmd.Add(DataTypeDic[DataType.Float]);
            sendCmd.AddRange(VoltageClamp_Neg);
            sendCmd.AddRange(CommandIDDic[CommandID.电流正向钳制设置]);
            sendCmd.Add(DataTypeDic[DataType.Float]);
            sendCmd.AddRange(CurrentClamp_Pos);
            sendCmd.AddRange(CommandIDDic[CommandID.电流反向钳制设置]);
            sendCmd.Add(DataTypeDic[DataType.Float]);
            sendCmd.AddRange(CurrentClamp_Neg);
            byte[] response = this.QueryProduct(sendCmd.ToArray(), 0);
        }

        public void OpenChannel(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.Direction Direction)
        {
            //var IChannelList = BoardSetting.ChannelSourceIsCurrentDictionary.Where(a => a.Value).Select(a => a.Key).ToList();

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

                    List<byte> sendCmdI = new List<byte>();
                    sendCmdI.Add(FunctionCodeDic[FunctionCode.写入配置寄存器]);
                    sendCmdI.Add((byte)4);//动作数
                    sendCmdI.AddRange(new byte[] { 0x00, (byte)Channel });
                    sendCmdI.Add(DataTypeDic[DataType.Bool]);
                    sendCmdI.Add(0x01);
                    sendCmdI.AddRange(CommandIDDic[CommandID.设置通道源类型]);
                    sendCmdI.Add(DataTypeDic[DataType.U8]);
                    sendCmdI.Add(0x00);//电流源
                    byte[] Byte = BitConverter.GetBytes(Convert.ToSingle(BeforeValue));
                    sendCmdI.AddRange(CommandIDDic[CommandID.设置通道真实值]);
                    sendCmdI.Add(DataTypeDic[DataType.Float]);
                    sendCmdI.AddRange(Byte);
                    sendCmdI.AddRange(CommandIDDic[CommandID.通道是否输出]);
                    sendCmdI.Add(DataTypeDic[DataType.Bool]);
                    sendCmdI.Add(0x01);
                    byte[] response = this.QueryProduct(sendCmdI.ToArray(), 0);
                    break;
                case BoardDriverEnum.SourceType.VoltageSource:

                    List<byte> sendCmdV = new List<byte>();
                    sendCmdV.Add(FunctionCodeDic[FunctionCode.写入配置寄存器]);
                    sendCmdV.Add((byte)4);//动作数
                    sendCmdV.AddRange(new byte[] { 0x00, (byte)Channel });
                    sendCmdV.Add(DataTypeDic[DataType.Bool]);
                    sendCmdV.Add(0x01);

                    sendCmdV.AddRange(CommandIDDic[CommandID.设置通道源类型]);
                    sendCmdV.Add(DataTypeDic[DataType.U8]);
                    sendCmdV.Add(0x01);//电压源
                    Byte = BitConverter.GetBytes(Convert.ToSingle(0));
                    sendCmdV.AddRange(CommandIDDic[CommandID.设置通道真实值]);
                    sendCmdV.Add(DataTypeDic[DataType.Float]);
                    sendCmdV.AddRange(Byte);
                    sendCmdV.AddRange(CommandIDDic[CommandID.通道是否输出]);
                    sendCmdV.Add(DataTypeDic[DataType.Bool]);
                    sendCmdV.Add(0x01);
                    response = this.QueryProduct(sendCmdV.ToArray(), 0);
                    break;
                default:
                    break;
            }

          
             
               
        }

        public void SetBoardClampAfterOpenChannel(BoardDriverEnum.SourceType SourceType, BoardDriverEnum.Direction Direction)
        {

            var VoltageClamp_Pos = BitConverter.GetBytes(3000f);
            var VoltageClamp_Neg = BitConverter.GetBytes(0f);
            var CurrentClamp_Pos = BitConverter.GetBytes(0f);
            var CurrentClamp_Neg = BitConverter.GetBytes(0f);

            switch (SourceType)
            {
                case BoardDriverEnum.SourceType.CurrentSource:

                    switch (Direction)
                    {
                        case BoardDriverEnum.Direction.Positive:


                            VoltageClamp_Pos = BitConverter.GetBytes(3000f);
                            VoltageClamp_Neg = BitConverter.GetBytes(0f);
                            CurrentClamp_Pos = BitConverter.GetBytes(0f);
                            CurrentClamp_Neg = BitConverter.GetBytes(0f);
                            break;
                        case BoardDriverEnum.Direction.Negative:
                            VoltageClamp_Pos = BitConverter.GetBytes(0f);
                            VoltageClamp_Neg = BitConverter.GetBytes(-3000f);
                            CurrentClamp_Pos = BitConverter.GetBytes(0f);
                            CurrentClamp_Neg = BitConverter.GetBytes(0f);
                            break;
                    }
                    break;

                case BoardDriverEnum.SourceType.VoltageSource:
                    switch (Direction)
                    {
                        case BoardDriverEnum.Direction.Positive:


                            VoltageClamp_Pos = BitConverter.GetBytes(0f);
                            VoltageClamp_Neg = BitConverter.GetBytes(0f);
                            CurrentClamp_Pos = BitConverter.GetBytes(300f);
                            CurrentClamp_Neg = BitConverter.GetBytes(0f);
                            break;
                        case BoardDriverEnum.Direction.Negative:
                            VoltageClamp_Pos = BitConverter.GetBytes(0f);
                            VoltageClamp_Neg = BitConverter.GetBytes(0f);
                            CurrentClamp_Pos = BitConverter.GetBytes(0f);
                            CurrentClamp_Neg = BitConverter.GetBytes(-300f);
                            break;
                    }
                    break;
            }
            List<byte> sendCmd = new List<byte>();
                sendCmd.Add(FunctionCodeDic[FunctionCode.写入配置寄存器]);
                sendCmd.Add(0x04);

                sendCmd.AddRange(CommandIDDic[CommandID.电压正向钳制设置]);
                sendCmd.Add(DataTypeDic[DataType.Float]);
                sendCmd.AddRange(VoltageClamp_Pos);
                sendCmd.AddRange(CommandIDDic[CommandID.电压反向钳制设置]);
                sendCmd.Add(DataTypeDic[DataType.Float]);
                sendCmd.AddRange(VoltageClamp_Neg);
                sendCmd.AddRange(CommandIDDic[CommandID.电流正向钳制设置]);
                sendCmd.Add(DataTypeDic[DataType.Float]);
                sendCmd.AddRange(CurrentClamp_Pos);
                sendCmd.AddRange(CommandIDDic[CommandID.电流反向钳制设置]);
                sendCmd.Add(DataTypeDic[DataType.Float]);
                sendCmd.AddRange(CurrentClamp_Neg);
                byte[] response = this.QueryProduct(sendCmd.ToArray(), 0);
               
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

