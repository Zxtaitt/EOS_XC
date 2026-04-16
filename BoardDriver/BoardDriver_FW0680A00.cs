using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace BoardDriver
{
    public class BoardDriver_FW0680A00 : BoardDriver_FW0325D00
    {
        private int[] CoefficientCurrentAddress = new int[6] { 0, 4, 8, 12, 16, 20 };
        protected Dictionary<int, Dictionary<BoardDriverEnum.SourceType, Coefficient>> ChannelCoefficient;
        public override void SetBoardClamp(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.Direction Direction)
        {
            switch (SourceType)
            {
                case BoardDriverEnum.SourceType.CurrentSource:
                    SetBoardOffChannel();
                    SetBoardOnSingleChannel(Channel);
                    break;
                case BoardDriverEnum.SourceType.VoltageSource:
                    break;
                default:
                    break;
            }
        }
        public override void SetBoardOnPower(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.PowerMethod PowerMethod, int Step, double SetValue)
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

        public override void SetBoardOFFPower(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.PowerMethod PowerMethod, int Step, double SetValue)
        {
            double SetZero = 0;
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
        protected override void SetBoardOnSingleChannel(int Channel)
        {
            int espectLength = 8;
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x00, 0x0A, 0x10, 0x05, 0x00, (byte)(Channel + 1), 0x01 };
                byte[] response = this.QueryProduct(cmd, espectLength);
                if ((response.Length == espectLength) && Tool.CheckFeedback(response[5]))
                {
                    return;
                }
                retry++;
                System.Threading.Thread.Sleep(20);
            } while (retry < retryOut);
            throw new Exception("Set Board On Single Exception");
        }
        protected override void SetBoardOffSingleChannel(int Channel)
        {
            int espectLength = 8;
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x00, 0x0A, 0x10, 0x05, 0x00, (byte)(Channel + 1), 0x00 };
                byte[] response = this.QueryProduct(cmd, espectLength);
                if ((response.Length == espectLength) && Tool.CheckFeedback(response[5]))
                {
                    return;
                }
                retry++;
                System.Threading.Thread.Sleep(20);
            } while (retry < retryOut);
            throw new Exception("Set Board Off Single Exception");
        }
        protected override void SetBoardOffChannel()
        {
            int espectLength = 8;
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x00, 0x0A, 0x10, 0x05, 0x00, 0xFF, 0x00 };
                byte[] response = this.QueryProduct(cmd, espectLength);
                if ((response.Length == espectLength) && Tool.CheckFeedback(response[5]))
                {
                    return;
                }
                retry++;
                System.Threading.Thread.Sleep(20);
            } while (retry < retryOut);
            throw new Exception("Set Board Off All Single Exception");
        }
    
        protected virtual void SetBoardValue(int Channel, double Value, BoardDriverEnum.SourceType SourceType)
        {
            ReadKB(Channel, SourceType);
            double SetValue = double.Parse(ChannelCoefficient[Channel][SourceType].SetK) * Value + double.Parse(ChannelCoefficient[Channel][SourceType].SetB);
            int espectLength = 8;
            int retry = 0;
            do
            {
                byte[] Byte = BitConverter.GetBytes(Convert.ToInt32(SetValue)).Skip(0).Take(2).ToArray();
                Array.Reverse(Byte);
                byte[] cmd = new byte[] { 0x00, 0x0C, 0x10, 0x04, 0x00, (byte)(Channel + 1) };
                List<byte> sendCmd = new List<byte>();
                sendCmd.AddRange(cmd);
                sendCmd.AddRange(Byte);
                sendCmd.Add(0x00);
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
        protected virtual void ReadKB(int Channel, BoardDriverEnum.SourceType SourceType)
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
                Address += Channel * 24;
                byte[] ReadAddress = BitConverter.GetBytes(Address).Skip(0).Take(2).ToArray();
                Array.Reverse(ReadAddress);
                byte[] Byte = Read(ReadAddress);
                Array.Reverse(Byte);
                result = Tool.ToFloat(Byte).ToString();
                return result;
            }
            catch
            {
                return result;
            }
        }
        protected virtual byte[] Read(byte[] Address)
        {
            int espectLength = 16;
            int retryOut = 1;
            int retry = 0;
            byte[] result = new byte[4];
            do
            {
                byte[] cmd = new byte[] { 0x00, 0x10, 0x03, 0xA0, 0x00 };
                List<byte> sendCmd = new List<byte>();
                sendCmd.AddRange(cmd);
                sendCmd.AddRange(Address);
                sendCmd.AddRange(new byte[] { 0x00, 0x04 });
                byte[] response = this.QueryProduct(sendCmd.ToArray(), espectLength);
                if ((response.Length == espectLength) & Tool.CheckFeedback(response[5]))
                {
                    return response.Skip(10).Take(4).ToArray();
                }
                retry++;
                System.Threading.Thread.Sleep(20);
            } while (retry < retryOut);
            return result;
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


    }
}
