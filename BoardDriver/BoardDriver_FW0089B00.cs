using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BoardDriver
{
    public class BoardDriver_FW0089B00 : BoardDriver_FW0061A00
    {
        protected Dictionary<int, Dictionary<BoardDriverEnum.SourceType, Coefficient>> ChannelCoefficient;

        public override void SetBoardClamp(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.Direction Direction)
        {

            SetBoardOnSingleChannel(Channel);
        }


        protected override void SetBoardOnSingleChannel(int Channel)
        {
            if (Channel >= 32)
                return;
            int espectLength = 8;
            int retryOut = 1;
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x00, 0x0A, 0x10, 0x85, 0x00, (byte)(Channel), 0x01 };
                byte[] response = this.QueryProduct(cmd, espectLength);
                if ((response.Length == espectLength) && Tool.CheckFeedback(response[5]))
                {
                    return;
                }
                retry++;
            } while (retry < retryOut);
            throw new Exception("Set DVB Board On Single Exception");
        }
        protected override void SetBoardOffSingleChannel(int Channel)
        {
            if (Channel >= 32)
                return;
            int espectLength = 8;
            int retryOut = 1;
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x00, 0x0A, 0x10, 0x85, 0x00, (byte)(Channel), 0x00 };
                byte[] response = this.QueryProduct(cmd, espectLength);
                if ((response.Length == espectLength) && Tool.CheckFeedback(response[5]))
                {
                    return;
                }
                retry++;
            } while (retry < retryOut);
            throw new Exception("Set DVB Board Off Single Exception");
        }
        protected override void SetBoardSourceType(int Channel, BoardDriverEnum.SourceType SourceType)
        {
            return;
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

        protected virtual Coefficient GetCoefficient(int Channel, BoardDriverEnum.SourceType SourceType)
        {
         
            Coefficient CoefficientValue = GetCoefficientValue(Channel, SourceType);
            return CoefficientValue;
        }
        protected Coefficient GetCoefficientValue(int Channel, BoardDriverEnum.SourceType SourceType)
        {
            Coefficient CoefficientValue = new Coefficient();
        

            if (CoefficientValue == null)
            {
                CoefficientValue = new Coefficient();
            }
            int espectLength = 33;
            byte ReadCMD = 0xA4;//电压源
            if (SourceType == BoardDriverEnum.SourceType.CurrentSource)
            {
                ReadCMD = 0xA5;//电流源
            }
            byte[] cmd = new byte[] { 0x00, 0x09, 0x03, ReadCMD, 0x00, (byte)(Channel) };
            byte[] response = this.QueryProduct(cmd, espectLength);
            if ((response.Length == espectLength) && (Tool.CheckFeedback(response[5])))
            {
                CoefficientValue.SetK = BitConverter.ToSingle(new byte[] { response[7], response[8], response[9], response[10] }, 0).ToString();
                CoefficientValue.SetB = BitConverter.ToSingle(new byte[] { response[11], response[12], response[13], response[14] }, 0).ToString();
                CoefficientValue.ReadCurrentK = BitConverter.ToSingle(new byte[] { response[15], response[16], response[17], response[18] }, 0).ToString();
                CoefficientValue.ReadCurrentB = BitConverter.ToSingle(new byte[] { response[19], response[20], response[21], response[22] }, 0).ToString();
                CoefficientValue.ReadVoltageK = BitConverter.ToSingle(new byte[] { response[23], response[24], response[25], response[26] }, 0).ToString();
                CoefficientValue.ReadVoltageB = BitConverter.ToSingle(new byte[] { response[27], response[28], response[29], response[30] }, 0).ToString();
                return CoefficientValue;
            }
            throw new Exception("获取KB值异常");

           
        }

        public override void SetBoardOnPower(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.PowerMethod PowerMethod, int Step, double SetValue)
        {
           
                GetChannelCoefficient(Channel, SourceType);
                double RealValue = double.Parse(ChannelCoefficient[Channel][SourceType].SetK) * SetValue + double.Parse(ChannelCoefficient[Channel][SourceType].SetB);
                SetBoardValue(Channel, PowerMethod, RealValue, Step);

          
        }

        public override void SetBoardOFFPower(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.PowerMethod PowerMethod, int Step, double SetValue)
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
            SetBoardValue(Channel, PowerMethod, ElectricityValue, Step);
            SetBoardOffSingleChannel(Channel);
        }

        protected override void SetBoardValue(int Channel, BoardDriverEnum.PowerMethod PowerMethod, double Value, double Step)
        {
            int espectLength = 8;
            int retry = 0;
            do
            {

                byte[] Byte = BitConverter.GetBytes(Convert.ToInt32(Value));
                Array.Reverse(Byte);
                if (Byte[2] > 0x7f)
                {
                    Byte = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                }
                byte[] cmd = new byte[] { 0x00, 0x13, 0x10, 0x93, 0x00, (byte)(Channel), 0x00 };
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
                if (response.Length == espectLength && Tool.CheckFeedback(response[5]))
                {
                    return;
                }
                retry++;
                Thread.Sleep(20);
            } while (retry < retryOut);
            throw new Exception("Set Board Value Exception");
        }
    }
}
