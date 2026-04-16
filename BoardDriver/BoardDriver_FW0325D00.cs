using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Net.Sockets;

namespace BoardDriver
{

    public class BoardDriver_FW0325D00 : IBoardDriver
    {
        public SerialPort BoardSerialPort
        { get; set; }
        public byte BoardAddress
        { get; set; }
        public TcpClient BoardClient { get; set; }
        public string BoardClientConStr { get; set ; }

        protected const int retryOut = 9;
        public virtual void SetBoardClamp(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.Direction Direction)
        {
            SetBoardOffChannel();
            double BeforeValue = 0;
            double BoforePositive = 0;
            double BeforeNegative = 0;
            double AfterPositive = 0;
            double AfterNegative = 0;
            switch (SourceType)
            {
                case BoardDriverEnum.SourceType.CurrentSource:
                    switch (Direction)
                    {
                        case BoardDriverEnum.Direction.Positive:
                            BeforeValue = -1;
                            BoforePositive = 1;
                            BeforeNegative = 0;
                            AfterPositive = 3000;
                            AfterNegative = 0;
                            break;
                        case BoardDriverEnum.Direction.Negative:
                            BeforeValue = 1;
                            BoforePositive = 0;
                            BeforeNegative = -1;
                            AfterPositive = 0;
                            AfterNegative = -3000;
                            break;
                        default: break;
                    }
                    break;
                case BoardDriverEnum.SourceType.VoltageSource:
                    switch (Direction)
                    {
                        case BoardDriverEnum.Direction.Positive:
                            BeforeValue = 0;
                            BoforePositive = 1;
                            BeforeNegative = 0;
                            AfterPositive = 300;
                            AfterNegative = 0;
                            break;
                        case BoardDriverEnum.Direction.Negative:
                            BeforeValue = 0;
                            BoforePositive = 0;
                            BeforeNegative = -1;
                            AfterPositive = 0;
                            AfterNegative = -300;
                            break;
                        default: break;
                    }
                    break;
                default: break;
            }
            SetBoardSourceType(Channel, SourceType);
            SetBoardMuzzle(Channel, SourceType, BoardDriverEnum.Direction.Positive, BoforePositive);
            SetBoardMuzzle(Channel, SourceType, BoardDriverEnum.Direction.Negative, BeforeNegative);
            SetBoardValue(Channel, BoardDriverEnum.PowerMethod.Single, BeforeValue);
            System.Threading.Thread.Sleep(500);
            SetBoardOnSingleChannel(Channel);
            SetBoardMuzzle(Channel, SourceType, BoardDriverEnum.Direction.Positive, AfterPositive);
            SetBoardMuzzle(Channel, SourceType, BoardDriverEnum.Direction.Negative, AfterNegative);
        }
        public void SetBoardAdjustDriveVoltage()
        {

        }
        public virtual void SetBoardOnPower(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.PowerMethod PowerMethod, int Step, double SetValue)
        {
            switch (PowerMethod)
            {
                case BoardDriverEnum.PowerMethod.Single:
                    SetBoardValue(Channel, BoardDriverEnum.PowerMethod.Single, SetValue);
                    break;
                case BoardDriverEnum.PowerMethod.Step:
                    double stepValue = SetValue / Step;
                    for (int i = 0; i < Step - 1; i++)
                    {
                        SetBoardValue(Channel, BoardDriverEnum.PowerMethod.Single, stepValue * (i + 1));
                        System.Threading.Thread.Sleep(100);
                    }
                    SetBoardValue(Channel, BoardDriverEnum.PowerMethod.Single, SetValue);
                    System.Threading.Thread.Sleep(500);
                    break;
                default:
                    break;
            }
        }

        public virtual void SetBoardOFFPower(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.PowerMethod PowerMethod, int Step, double SetValue)
        {
            double ElectricityValue = 0;
            if (SetValue > 0)
            {
                ElectricityValue =0;
            }
            switch (PowerMethod)
            {
                case BoardDriverEnum.PowerMethod.Single:
                    SetBoardValue(Channel, BoardDriverEnum.PowerMethod.Single, SetValue);
                    break;
                case BoardDriverEnum.PowerMethod.Step:
                    double stepValue = SetValue / Step;
                    for (int i = 0; i < Step - 1; i++)
                    {
                        SetBoardValue(Channel, BoardDriverEnum.PowerMethod.Single, SetValue - stepValue * (i + 1));
                        System.Threading.Thread.Sleep(100);
                    }
                    SetBoardValue(Channel, BoardDriverEnum.PowerMethod.Single, ElectricityValue);
                    System.Threading.Thread.Sleep(500);
                    break;
                default:
                    break;
            }
            SetBoardOffSingleChannel(Channel);
            SetBoardSourceType(Channel, BoardDriverEnum.SourceType.VoltageSource);
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
                var cmd = new byte[] { 0x00, 0x0A, 0x10, 0x25, 0x00, (byte)(Channel + 1), 0x01 };
                byte[] response = this.QueryProduct(cmd, espectLength);
                if ((response.Length == espectLength) && Tool.CheckFeedback(response[5]))
                {
                    return;
                }
                retry++;
                System.Threading.Thread.Sleep(20);
            } while (retry < retryOut);
            throw new Exception("Set DVB Board On Single Exception");
        }
        protected virtual void SetBoardOffSingleChannel(int Channel)
        {
            int espectLength = 8;
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x00, 0x0A, 0x10, 0x25, 0x00, (byte)(Channel + 1), 0x00 };
                byte[] response = this.QueryProduct(cmd, espectLength);
                if ((response.Length == espectLength) && Tool.CheckFeedback(response[5]))
                {
                    return;
                }
                retry++;
                System.Threading.Thread.Sleep(20);
            } while (retry < retryOut);
            throw new Exception("Set DVB Board Off Single Exception");
        }
        protected virtual void SetBoardOffChannel()
        {
            int espectLength = 8;
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x00, 0x0A, 0x10, 0x25, 0x00, 0xFF, 0x00 };
                byte[] response = this.QueryProduct(cmd, espectLength);
                if ((response.Length == espectLength) && Tool.CheckFeedback(response[5]))
                {
                    return;
                }
                retry++;
                System.Threading.Thread.Sleep(20);
            } while (retry < retryOut);
            throw new Exception("Set DVB Board Off All Single Exception");
        }
        protected virtual void SetBoardSourceType(int Channel, BoardDriverEnum.SourceType SourceType)
        {
            int espectLength = 8;
            int retry = 0;
            do
            {
                int Model = (int)SourceType;
                var cmd = new byte[] { 0x00, 0x10, 0x10, 0x23, 0x00, (byte)(Channel + 1), 0x00, (byte)Model, 0x00, 0x00, 0x00, 0x00, 0x00 };
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
        protected virtual void SetBoardMuzzle(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.Direction Direction, double Muzzle)
        {
            int espectLength = 8;
            int retry = 0;
            do
            {
                int Model = (int)SourceType;
                int Orientation = (int)Direction;
                int Ratio = 100;
                if (SourceType == BoardDriverEnum.SourceType.VoltageSource)
                {
                    Ratio = 10;
                }
                int Value = Math.Abs(Convert.ToInt32(Muzzle * Ratio));
                var cmd = new byte[] { 0x00, 0x10, 0x10, 0x23, 0x00, (byte)(byte)(Channel + 1), 0x01, (byte)Model, 0x01, (byte)Orientation, 0x01 };
                byte[] Byte = BitConverter.GetBytes(Value).Skip(0).Take(2).ToArray();
                Array.Reverse(Byte);
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

        protected virtual void SetBoardValue(int Channel, BoardDriverEnum.PowerMethod PowerMethod, double Value)
        {
            int espectLength = 8;
            int retry = 0;
            do
            {
                int Ladder = (int)PowerMethod;
                byte[] Byte = BitConverter.GetBytes(Convert.ToInt32(Value * 1000));
                Array.Reverse(Byte);
                byte[] cmd = new byte[] { 0x00, 0xD0, 0x10, 0x24, 0x00, (byte)(Channel + 1), 0x01, (byte)Ladder, 0x00, 0x00, 0x00, 0x64};
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
        protected byte[] QueryProduct(byte[] Send, int ReadLength)
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
    }
}
