using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Net.Sockets;

namespace BoardDriver
{
    public class BoardDriver_FW0144E00 : IBoardDriver
    {
        public SerialPort BoardSerialPort
        { get; set; }
        public byte BoardAddress
        { get; set; }
        public string BoardClientConStr { get ; set; }
        public TcpClient BoardClient { get; set; }
        protected int[] CoefficientCurrentAddress = new int[6] { 0, 3, 1, 4, 2, 5 };
        protected Dictionary<int, Dictionary<BoardDriverEnum.SourceType, Coefficient>> ChannelCoefficient;
        protected const int espectLength = 9;
        protected const int retryOut = 9;
        public virtual void SetBoardClamp(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.Direction Direction)
        {
            SetBoardOffChannel();
            double BeforeValue = 0;
            switch (SourceType)
            {
                case BoardDriverEnum.SourceType.CurrentSource:
                    switch (Direction)
                    {
                        case BoardDriverEnum.Direction.Positive:
                            BeforeValue = -5;
                            break;
                        case BoardDriverEnum.Direction.Negative:
                            BeforeValue = 5;
                            break;
                    }
                    break;
                default: break;
            }
            SetBoardValue(Channel, BeforeValue, SourceType);
            SetBoardVoltageClamp_Neg(0);
            Thread.Sleep(300);
            SetBoardVoltageClamp_Pos(0);
            Thread.Sleep(500);
            SetBoardOnSingleChannel(Channel);
            switch (Direction)
            {
                case BoardDriverEnum.Direction.Positive:
                    for (int i = 0; i < 10; i++)
                    {
                        SetBoardVoltageClamp_Pos(100 * (i + 1));
                    }
                    SetBoardVoltageClamp_Pos(3000);
                    break;
                case BoardDriverEnum.Direction.Negative:
                    for (int i = 0; i < 10; i++)
                    {
                        SetBoardVoltageClamp_Neg(-100 * (i + 1));
                    }
                    SetBoardVoltageClamp_Neg(-3000);
                    break;
            }

        }
        public void SetBoardAdjustDriveVoltage()
        {

        }
        public virtual void SetBoardOnPower(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.PowerMethod PowerMethod, int Step, double SetValue)
        {
            switch (PowerMethod)
            {
                case BoardDriverEnum.PowerMethod.Single:
                    SetBoardValue(Channel, SetValue, SourceType);
                    break;
                case BoardDriverEnum.PowerMethod.Step:
                    double stepValue = SetValue / Step;
                    for (int i = 0; i < Step - 1; i++)
                    {
                        SetBoardValue(Channel, stepValue * (i + 1), SourceType);
                        Thread.Sleep(10);
                    }
                    SetBoardValue(Channel, SetValue, SourceType);
                    break;
                default:
                    break;
            }
        }

        public virtual void SetBoardOFFPower(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.PowerMethod PowerMethod, int Step, double SetValue)
        {
            double SetZero = 0;
            switch (SourceType)
            {
                case BoardDriverEnum.SourceType.CurrentSource:
                    if (SetValue < 0)
                    {
                        SetZero = -1;
                    }
                    else
                    {
                        SetZero = 1;
                    }
                    break;
                case BoardDriverEnum.SourceType.VoltageSource:
                    break;
                default:
                    break;
            }
            switch (PowerMethod)
            {
                case BoardDriverEnum.PowerMethod.Single:
                    SetBoardValue(Channel, SetZero, SourceType);
                    break;
                case BoardDriverEnum.PowerMethod.Step:
                    double stepValue = SetValue / Step;
                    for (int i = 0; i < Step - 1; i++)
                    {
                        SetBoardValue(Channel, SetValue - stepValue * (i + 1), SourceType);
                        Thread.Sleep(10);
                    }
                    SetBoardValue(Channel, SetZero, SourceType);
                    break;
                default:
                    break;
            }
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
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x81, (byte)Channel, 0x00, 0x00 };
                byte[] response = this.QueryProduct(cmd);
                if ((response.Length == espectLength) && Tool.CheckFeedback(response[3]))
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
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x80, (byte)Channel, 0x00, 0x00 };
                byte[] response = this.QueryProduct(cmd);
                if ((response.Length == espectLength) && Tool.CheckFeedback(response[3]))
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
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x80, 0xFF, 0x00, 0x00 };
                byte[] response = this.QueryProduct(cmd);
                if ((response.Length == espectLength) && Tool.CheckFeedback(response[3]))
                {
                    return;
                }
                retry++;
                System.Threading.Thread.Sleep(20);
            } while (retry < retryOut);
            throw new Exception("Set Board Off All Channel Exception");
        }
        public virtual bool SetBoardValue(int Channel, double Value, BoardDriverEnum.SourceType SourceType)
        {
            bool result = false;
            try
            {
                GetChannelCoefficient(Channel, SourceType);
                double SetValue = double.Parse(ChannelCoefficient[Channel][SourceType].SetK) * Value + double.Parse(ChannelCoefficient[Channel][SourceType].SetB);
                int retry = 0;
                do
                {
                    string hexOutput = string.Format("{0:X4}", Convert.ToInt32(SetValue));
                    string hex1 = hexOutput.Substring(0, hexOutput.Length - 2);
                    string hex2 = hexOutput.Substring(hexOutput.Length - 2, 2);
                    byte[] cmd = new byte[] { 0xD0, (byte)Channel, Convert.ToByte(hex1, 16), Convert.ToByte(hex2, 16) };
                    byte[] response = this.QueryProduct(cmd);
                    if (response.Length == espectLength)
                    {
                        return Tool.CheckFeedback(response[3]);
                    }
                    retry++;
                    System.Threading.Thread.Sleep(20);
                } while (retry < retryOut);
                return false;
                throw new Exception("Set Board Value Exception");
            }
            catch
            {
                return result;
            }
        }
        protected virtual void GetChannelCoefficient(int Channel, BoardDriverEnum.SourceType SourceType)
        {
            if (ChannelCoefficient == null)
            {
                ChannelCoefficient = new Dictionary<int, Dictionary<BoardDriverEnum.SourceType, Coefficient>>();
            }
            if (ChannelCoefficient.ContainsKey(Channel))
            {
                if (!ChannelCoefficient[Channel].ContainsKey(SourceType))
                {
                    Coefficient CoefficientValue = GetCoefficient(Channel, SourceType);
                    ChannelCoefficient[Channel].Add(SourceType, CoefficientValue);
                }
            }
            else
            {
                Dictionary<BoardDriverEnum.SourceType, Coefficient> SourceTypeCoefficient = new Dictionary<BoardDriverEnum.SourceType, Coefficient>();
                Coefficient CoefficientValue = GetCoefficient(Channel, SourceType);
                SourceTypeCoefficient.Add(SourceType, CoefficientValue);
                ChannelCoefficient.Add(Channel, SourceTypeCoefficient);
            }
        }

        protected virtual string ReadCoefficient(int Channel, int Address)
        {
            string result = "";
            try
            {
                byte[] Byte = new byte[4];
                for (int i = 0; i < Byte.Length; i++)
                {
                    Byte[i] = Read(Channel * Byte.Length + i, (byte)Address);
                }
                result = Tool.ToFloat(Byte).ToString();
                return result;
            }
            catch
            {
                return result;
            }
        }

        protected virtual void SetBoardVoltageClamp_Pos(double volthes)
        {
            int value = 0;
            double dacval = Math.Abs(volthes / 5000 * 4096);
            try
            {
                value = Convert.ToInt32(dacval);
                if (value >= 4095)
                {
                    value = 4095;
                }
            }
            catch
            { }
            int retry = 0;
            do
            {
                string hexOutput = string.Format("{0:X4}", Convert.ToInt32(value));
                string hex1 = hexOutput.Substring(0, hexOutput.Length - 2);
                string hex2 = hexOutput.Substring(hexOutput.Length - 2, 2);
                byte[] cmd = new byte[] { 0xD1, 0x00, Convert.ToByte(hex1, 16), Convert.ToByte(hex2, 16) };
                byte[] response = this.QueryProduct(cmd);
                if ((response.Length == espectLength) && Tool.CheckFeedback(response[3]))
                {
                    return;
                }
                retry++;
                System.Threading.Thread.Sleep(20);
            } while (retry < retryOut);
            throw new Exception("Set Board VolThesPos Exception");
        }
        protected virtual void SetBoardVoltageClamp_Neg(double volthes)
        {
            int value = 0;
            double dacval = Math.Abs(volthes / 5000 * 4096);
            try
            {
                value = Convert.ToInt32(dacval);
                if (value <= -4095)
                {
                    value = -4095;
                }
            }
            catch
            { }
            int retry = 0;
            do
            {
                string hexOutput = string.Format("{0:X4}", Convert.ToInt32(value));
                string hex1 = hexOutput.Substring(0, hexOutput.Length - 2);
                string hex2 = hexOutput.Substring(hexOutput.Length - 2, 2);
                byte[] cmd = new byte[] { 0xD2, 0x00, Convert.ToByte(hex1, 16), Convert.ToByte(hex2, 16) };
                byte[] response = this.QueryProduct(cmd);
                if ((response.Length == espectLength) && Tool.CheckFeedback(response[3]))
                {
                    return;
                }
                retry++;
                System.Threading.Thread.Sleep(20);
            } while (retry < retryOut);
            throw new Exception("Set Board VolThesNeg Exception");
        }
        protected virtual Coefficient GetCoefficient(int Channel, BoardDriverEnum.SourceType SourceType)
        {
            List<int> CoefficientAddress = CoefficientCurrentAddress.ToList();
            Coefficient CoefficientValue = GetCoefficientValue(Channel, CoefficientAddress);
            return CoefficientValue;
        }
        protected Coefficient GetCoefficientValue(int Channel, List<int> CoefficientAddress)
        {
            Coefficient CoefficientValue = new Coefficient();
            CoefficientValue.SetK = ReadCoefficient(Channel, CoefficientAddress[0]);
            CoefficientValue.SetB = ReadCoefficient(Channel, CoefficientAddress[1]);
            CoefficientValue.ReadCurrentK = ReadCoefficient(Channel, CoefficientAddress[2]);
            CoefficientValue.ReadCurrentB = ReadCoefficient(Channel, CoefficientAddress[3]);
            CoefficientValue.ReadVoltageK = ReadCoefficient(Channel, CoefficientAddress[4]);
            CoefficientValue.ReadVoltageB = ReadCoefficient(Channel, CoefficientAddress[5]);
            return CoefficientValue;
        }
        private byte Read(int Channel, byte Address)
        {
            int retry = 0;
            byte result = 0;
            do
            {
                byte[] cmd = new byte[] { 0xE1, Address, (byte)Channel, 0x00 };
                byte[] response = this.QueryProduct(cmd);
                if ((response.Length == espectLength) & Tool.CheckFeedback(response[3]))
                {
                    result = response[6];
                    return result;
                }
                System.Threading.Thread.Sleep(20);
                retry++;
            } while (retry < retryOut);
            return result;
        }

        private byte[] WrapCmd(byte[] cmd)
        {
            List<byte> sendCmd = new List<byte>();
            sendCmd.AddRange(new byte[] { 0xAA, 0x55, BoardAddress });
            sendCmd.AddRange(cmd);
            sendCmd.AddRange(new byte[] { 0x55, 0xAA });
            return sendCmd.ToArray();
        }

        protected byte[] QueryProduct(byte[] send)
        {
            lock (BoardSerialPort)
            {
                try
                {
                    send = WrapCmd(send);
                    if (!BoardSerialPort.IsOpen)
                    {
                        BoardSerialPort.ReadTimeout = 1500;
                        BoardSerialPort.WriteTimeout = 1500;
                        BoardSerialPort.Open();
                    }
                    BoardSerialPort.DiscardInBuffer();
                    BoardSerialPort.DiscardOutBuffer();
                    BoardSerialPort.Write(send, 0, send.Length);
                    for (int i = 0; i < 100; i++)
                    {
                        if (BoardSerialPort.BytesToRead == send.Length)
                        {
                            break;
                        }
                        Thread.Sleep(2);
                    }
                    if (BoardSerialPort.BytesToRead > 0)
                    {
                        byte[] readBuffer = new byte[BoardSerialPort.BytesToRead];
                        BoardSerialPort.Read(readBuffer, 0, readBuffer.Length);
                        return readBuffer;
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
