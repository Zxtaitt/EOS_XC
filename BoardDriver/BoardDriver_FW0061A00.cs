using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net.Sockets;
using System.Threading;

namespace BoardDriver
{

    public class BoardDriver_FW0061A00 : IBoardDriver
    {
        protected enum Supply
        {
            Positive,
            Negetive
        }
        public BaseBoardSetting BoardSetting { get; set; } = new BaseBoardSetting();
        public virtual List<ClampConfig> BoardClampConfig { get; set; } = new List<ClampConfig>()
        {
            new ClampConfig()
            {
                ClampName = "CSPVoltage", Browsable = true, DisplayName = "01.CSPVoltage(mV)",
                Description = "CSPVoltageRange 3500-8000mV，Recommend 3600mV", Value = new double[] {3600,3600,3600,3600,3600 }
            },
            new ClampConfig()
            {
                ClampName = "CSNVoltage", Browsable = true, DisplayName = "02.CSNVoltage(mV)",
                Description = "CSNVoltageRange -8000-(-3500)mV，Recommend -3600mV", Value = new double[]{-3600,-3600 }
            }
        };
        public SerialPort BoardSerialPort
        { get; set; }
        public byte BoardAddress
        { get; set; }
        public string BoardClientConStr { get; set ; }
        public TcpClient BoardClient { get ; set ; }

        protected const int retryOut = 9;
        public virtual void SetBoardClamp(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.Direction Direction)
        {
            SetBoardOffChannel();
            double BeforeValue = 0;
            double BoforeMuzzle = 0;
            double AfterMuzzle = 0;
            switch (SourceType)
            {
                case BoardDriverEnum.SourceType.CurrentSource:
                    switch (Direction)
                    {
                        case BoardDriverEnum.Direction.Positive:
                            if (Channel >= 27)
                            {
                                BeforeValue = 0;
                            }
                            else
                            {
                                BeforeValue = -1;
                            }
                            BoforeMuzzle = 1;
                            AfterMuzzle = 3;
                            break;
                        case BoardDriverEnum.Direction.Negative:
                            BeforeValue = 1;
                            BoforeMuzzle = -1;
                            AfterMuzzle = -3;
                            break;
                        default: break;
                    }
                    break;
                case BoardDriverEnum.SourceType.VoltageSource:
                    switch (Direction)
                    {
                        case BoardDriverEnum.Direction.Positive:
                            BeforeValue = 0;
                            BoforeMuzzle = 5;
                            AfterMuzzle = 60;
                            break;
                        case BoardDriverEnum.Direction.Negative:
                            BeforeValue = 0;
                            BoforeMuzzle = -5;
                            AfterMuzzle = -60;
                            break;
                        default: break;
                    }
                    break;
                default: break;
            }
            SetBoardSourceType(Channel, SourceType);
            SetBoardMuzzle(Channel, BoforeMuzzle);
            SetBoardValue(Channel, BoardDriverEnum.PowerMethod.Single, BeforeValue, 0);
            SetBoardOnSingleChannel(Channel);
            SetBoardMuzzle(Channel, AfterMuzzle);
           
        }
        public virtual void SetBoardOnPower(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.PowerMethod PowerMethod, int Step, double SetValue)
        {
            SetBoardValue(Channel, PowerMethod, SetValue, Step);
        }

        protected virtual void SetBoardSupplyVoltage(Supply supply, int Index, double Voltage)
        {
           
            int espectLength = 8;
            int retry = 0;
            byte CSByte = 0x91;

            if (supply == Supply.Positive)
            {
                CSByte = 0x90;
            }

            do
            {
                float Value = Convert.ToSingle(Voltage / 1000);
                byte[] Byte = BitConverter.GetBytes(Value);

                var cmd = new byte[] { 0x00, 0x0E, 0x10, CSByte, 0x00, (byte)Index, 0x01 };
                List<byte> sendCmd = new List<byte>();
                sendCmd.AddRange(cmd);
                sendCmd.AddRange(Byte);
                byte[] response = QueryProduct(sendCmd.ToArray(), espectLength);
                if ((response.Length == espectLength) && Tool.CheckFeedback(response[5]))
                {
                    return;
                }
                retry++;
                Thread.Sleep(20);
            } while (retry < retryOut);

            throw new Exception($"Set Board Supply Voltage Exception,Adress:{BoardAddress},Supply:{supply},Index:{Index}");
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
                default:break;
            }
            SetBoardValue(Channel, PowerMethod, ElectricityValue, Step);
            SetBoardOffSingleChannel(Channel);
        }
        public virtual void CloseBoardSerialPort()
        {
            lock (BoardSerialPort)
            {
                if (BoardSerialPort.IsOpen)
                {
                    BoardSerialPort.Close();
                }
            }
        }
        protected virtual void SetBoardOnSingleChannel(int Channel)
        {
            int espectLength = 8;
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x00, 0x0A, 0x10, 0x85, 0x00, (byte)Channel, 0x01 };
                byte[] response = this.QueryProduct(cmd, espectLength);
                if ((response.Length == espectLength) && Tool.CheckFeedback(response[5]))
                {
                    return;
                }
                retry++;
                System.Threading.Thread.Sleep(20);
            } while (retry < retryOut);
            throw new Exception("Set Board On Single Channel Exception");
        }
        protected virtual void SetBoardOffSingleChannel(int Channel)
        {
            int espectLength = 8;
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x00, 0x0A, 0x10, 0x85, 0x00, (byte)Channel, 0x00 };
                byte[] response = this.QueryProduct(cmd, espectLength);
                if ((response.Length == espectLength) && Tool.CheckFeedback(response[5]))
                {
                    return;
                }
                retry++;
                System.Threading.Thread.Sleep(20);
            } while (retry < retryOut);
            throw new Exception("Set Board Off Single Channel Exception");
        }
        protected virtual void SetBoardOffChannel()
        {
            int espectLength = 8;
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x00, 0x0A, 0x10, 0x85, 0x00, 0xFF, 0x00 };
                byte[] response = this.QueryProduct(cmd, espectLength);
                if ((response.Length == espectLength) && Tool.CheckFeedback(response[5]))
                {
                    return;
                }
                retry++;
                System.Threading.Thread.Sleep(20);
            } while (retry < retryOut);
            throw new Exception("Set Board Off All Channel Exception");
        }
        protected virtual void SetBoardSourceType(int Channel, BoardDriverEnum.SourceType SourceType)
        {
            if (Channel >= 27)
            {
                return;
            }
            int espectLength = 8;
            int retry = 0;
            do
            {
                int Model = 0;
                if (SourceType == BoardDriverEnum.SourceType.CurrentSource)
                {
                    Model = 1;
                }
                var cmd = new byte[] { 0x00, 0x0A, 0x10, 0x84, 0x00, (byte)Channel, (byte)Model };
                byte[] response = this.QueryProduct(cmd, espectLength);
                if ((response.Length == espectLength) && Tool.CheckFeedback(response[5]))
                {
                    return;
                }
                retry++;
                System.Threading.Thread.Sleep(20);
            } while (retry < retryOut);
            throw new Exception("Set Board Source Type Exception");
        }
        protected virtual void SetBoardMuzzle(int Channel, double Muzzle)
        {
            if (Channel >= 27)
            {
                return;
            }
            int espectLength = 8;
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x00, 0x0E, 0x10, 0x92, 0x00, (byte)Channel, 0x01 };
                byte[] Byte = BitConverter.GetBytes(Convert.ToSingle(Muzzle));
                List<byte> sendCmd = new List<byte>();
                sendCmd.AddRange(cmd);
                sendCmd.AddRange(Byte);
                byte[] response = this.QueryProduct(sendCmd.ToArray(), espectLength);
                if ((response.Length == espectLength) && Tool.CheckFeedback(response[5]))
                {
                    return;
                }
                retry++;
                System.Threading.Thread.Sleep(20);
            } while (retry < retryOut);
            throw new Exception("Set Board Muzzle Exception");
        }

        protected virtual void SetBoardValue(int Channel, BoardDriverEnum.PowerMethod PowerMethod, double Value, double Step)
        {
            int espectLength = 8;
            int retry = 0;
            do
            {
                byte[] Byte = BitConverter.GetBytes(Convert.ToSingle(Value));
                byte[] cmd = new byte[] { 0x00, 0x13, 0x10, 0x93, 0x00, (byte)Channel, 0x01 };
                List<byte> sendCmd = new List<byte>();
                sendCmd.AddRange(cmd);
                sendCmd.AddRange(Byte);
                switch (PowerMethod)
                {
                    case BoardDriverEnum.PowerMethod.Single:
                        sendCmd.Add(0x00);//直接上电
                        sendCmd.AddRange(new byte[] { 0x00, 0x00 });//阶梯次数
                        sendCmd.AddRange(new byte[] { 0x00, 0x00 });//阶梯间隔
                        break;
                    case BoardDriverEnum.PowerMethod.Step:
                        sendCmd.Add(0x01);//阶梯上电
                        sendCmd.AddRange(new byte[] { 0x00, (byte)Step });//阶梯次数
                        sendCmd.AddRange(new byte[] { 0x00, 0x0A });//阶梯间隔
                        break;
                    default:
                        break;
                }
                byte[] response = this.QueryProduct(sendCmd.ToArray(), espectLength);
                if ((response.Length == espectLength) && Tool.CheckFeedback(response[5]))
                {
                    return;
                }
                retry++;
                System.Threading.Thread.Sleep(20);
            } while (retry < retryOut);
            throw new Exception("Set Board Value Exception");
        }

        protected virtual byte[] WrapCmd(byte[] cmd)
        {
            List<byte> sendCmd = new List<byte>();
            sendCmd.AddRange(new byte[] { BoardAddress });
            sendCmd.AddRange(cmd);
            byte[] Byte = Tool.StringToHexByte(Tool.ToModbusCRC16(sendCmd.ToArray()));
            sendCmd.AddRange(Byte);
            return sendCmd.ToArray();
        }
        protected virtual byte[] QueryProduct(byte[] Send, int ReadLength)
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
                    for (int i = 0; i < 100; i++)
                    {
                        if (BoardSerialPort.BytesToRead == ReadLength)
                        {
                            break;
                        }
                        System.Threading.Thread.Sleep(5);
                    }
                    if (BoardSerialPort.BytesToRead > 0)
                    {
                        byte[] readBuffer = new byte[BoardSerialPort.BytesToRead];
                        BoardSerialPort.Read(readBuffer, 0, readBuffer.Length);
                        System.Threading.Thread.Sleep(20);
                        return readBuffer;
                    }
                    return new byte[1];
                }
                catch
                {
                    if (BoardSerialPort.IsOpen)
                    {
                        BoardSerialPort.Close();
                    }
                    return new byte[1];
                }
            }
        }

        public void CloseRelay(int channel)
        {
            throw new NotImplementedException();
        }

        public void OpenRelay(int channel)
        {
            throw new NotImplementedException();
        }

        public virtual void SetBoardAdjustDriveVoltage()
        {
            
        }
    }
}
