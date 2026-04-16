using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BoardDriver
{
    public class BoardDriver_Macom : BoardDriver_FW0422400
    {
        public override void SetBoardClamp(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.Direction Direction)
        {
            SetBoardOffAllChannel();
            Thread.Sleep(1000);
            //SetRangeCurr();
            SetBoardClampBeforeOpenChannel();
            OpenChannel(Channel, SourceType, Direction);
            SetBoardClampAfterOpenChannel(SourceType, Direction);

            //SetBoardOnSingleChannel(Channel);

        }

        public override void OpenChannel(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.Direction Direction)
        {
            //var IChannelList = BoardSetting.ChannelSourceIsCurrentDictionary.Where(a => a.Value).Select(a => a.Key).ToList();

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
                sendCmd.AddRange(new byte[] { (byte)Channel });

                //sendCmd.Add(DataTypeDic[DataType.Bool]);
                //sendCmd.Add(0x01);
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
        protected override void SetChannelOffValue(int Channel, double Value, bool PowerMethodIsSingle, int StepCount)
        {


            //int retryOut = 3;
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

                //sendCmd.AddRange(new byte[] { 0x00, (byte)Channel });
                //sendCmd.Add(DataTypeDic[DataType.Bool]);
                //sendCmd.Add(0x01);
                sendCmd.AddRange(CommandIDDic[CommandID.上电模式]);
                sendCmd.Add(DataTypeDic[DataType.U8]);

                //sendCmd.Add(0x02);
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
                sendCmd.AddRange(new byte[] { 0x64, 0x00, });//为什么这里32

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
        public override void SetBoardOFFPower(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.PowerMethod PowerMethod, int Step, double SetValue)
        {



            //SetChannelValue(Channel, ElectricityValue, PowerMethod, Step);
            SetChannelOffValue(Channel, SetValue, false, Step);

            SetBoardOffSingleChannel(Channel);

        }

        public override void SetBoardOffSingleChannel(int Channel)
        {


            //int retryOut = 3;
            int retry = 0;
            do
            {
                List<byte> sendCmd = new List<byte>();
                sendCmd.Add(FunctionCodeDic[FunctionCode.写入配置寄存器]);
                sendCmd.Add(0x02);//动作数
                sendCmd.AddRange(CommandIDDic[CommandID.通道ID]);
                sendCmd.Add(DataTypeDic[DataType.U8Array]);//0x82
                sendCmd.Add(0x00);
                sendCmd.Add(0x01);
                sendCmd.AddRange(new byte[] { (byte)Channel });
                //sendCmd.AddRange(CommandIDDic[CommandID.通道类型]);
                //sendCmd.Add(DataTypeDic[DataType.U8]);
                //sendCmd.Add(0x00);

                //sendCmd.Add(DataTypeDic[DataType.Bool]);
                //sendCmd.Add(0x01);
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
        /// <summary>
        /// 设置电流档位
        /// </summary>
        /// <param name="currentRange"></param>
        private void SetRangeCurr()
        {
            int retryOut = 9;
            int retry = 0;

            do
            {
                int range = 2;

                List<byte> sendCmd = new List<byte>();
                sendCmd.Add(FunctionCodeDic[FunctionCode.写入配置寄存器]);
                sendCmd.Add(0x01);//动作数
                sendCmd.AddRange(CommandIDDic[CommandID.电流采样档位]);
                sendCmd.Add(DataTypeDic[DataType.U8]);
                sendCmd.Add((byte)range);
                byte[] response = this.QueryProduct(sendCmd.ToArray(), 0);
                if (response.Length > 1 && CheckFeedback(response[1]))
                {
                    return;
                }
                retry++;
            } while (retry < retryOut);
            throw new Exception("Set DVB Board On Single Exception");
        }
    }
}

