using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace BoardDriver
{
    public class BoardDriver_FW0325C00 : BoardDriver_FW0144E00
    {
        protected int[] CoefficientVoltageAddress = new int[6] { 6, 9, 7, 10, 8, 11 };
        public override void SetBoardClamp(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.Direction Direction)
        {
            SetBoardOffChannel();
            double BeforeValue = 0;
            switch (SourceType)
            {
                case BoardDriverEnum.SourceType.CurrentSource:
                  
                    switch (Direction)
                    {
                        case BoardDriverEnum.Direction.Positive:
                            SetBoardVoltageClamp_Neg(0);
                            Thread.Sleep(100);
                            SetBoardVoltageClamp_Pos(1000);
                            Thread.Sleep(100);
                            BeforeValue = -1;
                            break;
                        case BoardDriverEnum.Direction.Negative:
                            SetBoardVoltageClamp_Neg(-1000);
                            Thread.Sleep(100);
                            SetBoardVoltageClamp_Pos(0);
                            Thread.Sleep(100);
                            BeforeValue = 1;
                            break;
                    }
                    break;
                case BoardDriverEnum.SourceType.VoltageSource:
                    SetBoardCurrentClamp_Pos(25);
                    Thread.Sleep(100);
                    SetBoardCurrentClamp_Neg(-25);
                    Thread.Sleep(100);
                    break;
                default:
                    break;
            }         
            SetBoardSourceType(Channel, SourceType);
            SetBoardValue(Channel, BeforeValue, SourceType);
            SetBoardOnSingleChannel(Channel);
            switch (SourceType)
            {
                case BoardDriverEnum.SourceType.CurrentSource:
                    switch (Direction)
                    {
                        case BoardDriverEnum.Direction.Positive:
                            SetBoardVoltageClamp_Pos(3000);
                            break;
                        case BoardDriverEnum.Direction.Negative:
                            SetBoardVoltageClamp_Neg(-3000);
                            break;
                    }
                    break;
                case BoardDriverEnum.SourceType.VoltageSource:
                    switch (Direction)
                    {
                        case BoardDriverEnum.Direction.Positive:
                            SetBoardCurrentClamp_Pos(300);
                            break;
                        case BoardDriverEnum.Direction.Negative:
                            SetBoardCurrentClamp_Neg(-300);
                            break;
                    }
                    break;
                default:
                    break;
            }

        }   
        protected virtual void SetBoardSourceType(int Channel, BoardDriverEnum.SourceType SourceType)
        {
            int retry = 0;
            do
            {
                byte Model = 0x83;
                switch (SourceType)
                {
                    case BoardDriverEnum.SourceType.CurrentSource:
                        Model = 0x82;
                        break;
                    case BoardDriverEnum.SourceType.VoltageSource:
                        Model = 0x83;
                        break;
                    default:
                        break;
                }
                var cmd = new byte[] { Model, (byte)Channel, 0x00, 0x00 };
                byte[] response = this.QueryProduct(cmd);
                if ((response.Length == espectLength) && (Tool.CheckFeedback(response[3])))
                {
                    return;
                }
                System.Threading.Thread.Sleep(20);
                retry++;
            } while (retry < retryOut);
            throw new Exception("Set Board Source Type Exception");
        }          
        protected override Coefficient GetCoefficient(int Channel, BoardDriverEnum.SourceType SourceType)
        {
            List<int> CoefficientAddress = new List<int>();
            switch (SourceType)
            {
                case BoardDriverEnum.SourceType.CurrentSource:
                    CoefficientAddress = CoefficientCurrentAddress.ToList();
                    break;
                case BoardDriverEnum.SourceType.VoltageSource:
                    CoefficientAddress = CoefficientVoltageAddress.ToList();
                    break;
                default:
                    break;
            }
            Coefficient CoefficientValue = GetCoefficientValue(Channel, CoefficientAddress);
            return CoefficientValue;
        }     
        protected override void SetBoardVoltageClamp_Pos(double volthes)
        {
            int value = 0;
            double dacval = Math.Abs(volthes / 10000 * 16383);
            try
            {
                value = Convert.ToInt32(dacval);
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
        protected override void SetBoardVoltageClamp_Neg(double volthes)
        {
            int value = 0;
            double dacval = Math.Abs(volthes / 10000 * 16383);
            try
            {
                value = Convert.ToInt32(dacval);
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
        private void SetBoardCurrentClamp_Pos(double volthes)
        {
            int value = 0;
            double dacval = volthes * 32.766;
            try
            {
                value = Convert.ToInt32(dacval);
            }
            catch
            { }
            if (value > 0x3FFF)
            {
                throw new Exception("Set Board CurThesPos Out of Range :" + value.ToString("X4"));
            }
            int retry = 0;
            do
            {
                string hexOutput = string.Format("{0:X4}", Convert.ToInt32(value));
                string hex1 = hexOutput.Substring(0, hexOutput.Length - 2);
                string hex2 = hexOutput.Substring(hexOutput.Length - 2, 2);
                byte[] cmd = new byte[] { 0xD3, 0x00, Convert.ToByte(hex1, 16), Convert.ToByte(hex2, 16) };
                byte[] response = this.QueryProduct(cmd);
                if ((response.Length == espectLength) && Tool.CheckFeedback(response[3]))
                {
                    return;
                }
                retry++;
                System.Threading.Thread.Sleep(20);
            } while (retry < retryOut);
            throw new Exception("Set Board CurThesPos Exception");
        }
        private void SetBoardCurrentClamp_Neg(double volthes)
        {
            int value = 0;
            double dacval = Math.Abs(volthes * 32.766);
            try
            {
                value = Convert.ToInt32(dacval);
            }
            catch
            { }
            if (value > 0x3FFF)
            {
                throw new Exception("Set Board CurThesNeg Out of Range :" + value.ToString("X4"));
            }
            int retry = 0;
            do
            {
                string hexOutput = string.Format("{0:X4}", Convert.ToInt32(value));
                string hex1 = hexOutput.Substring(0, hexOutput.Length - 2);
                string hex2 = hexOutput.Substring(hexOutput.Length - 2, 2);
                byte[] cmd = new byte[] { 0xD4, 0x00, Convert.ToByte(hex1, 16), Convert.ToByte(hex2, 16) };
                byte[] response = this.QueryProduct(cmd);
                if ((response.Length == espectLength) && Tool.CheckFeedback(response[3]))
                {
                    return;
                }
                retry++;
                System.Threading.Thread.Sleep(20);
            } while (retry < retryOut);
            throw new Exception("Set Board CurThesNeg Exception");
        }
    }
}
