using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BoardDriver
{
  public  class BoardDriver_FW0089D00_Net:BoardDriver_FW0089A00
    {
        public override List<ClampConfig> BoardClampConfig { get; set; } = new List<ClampConfig>()
        {
            new ClampConfig("CSPVoltage",true,"01.CSPVoltage(mV)","CSPVoltageRange 3500-8000mV，Recommend 3600mV",new double[] { 5000, 5000,5000 })
        };
        protected override byte[] QueryProduct(byte[] Send, int ReadLength)
        {
            lock (BoardClient)
            {
                try
                {
                    Send = WrapCmd(Send);
                    NetworkStream RelayNet = null;
                    if (!BoardClient.Connected)
                    {
                        var IPAdress = BoardClientConStr.Split(':')[0];
                        var PortAdress = BoardClientConStr.Split(':')[1];
                        BoardClient.Connect(IPAdress, Convert.ToInt32(PortAdress));
                        RelayNet = BoardClient.GetStream();
                        RelayNet.ReadTimeout = 1500;
                        RelayNet.WriteTimeout = 1500;
                    }
                    else
                    {
                        RelayNet = BoardClient.GetStream();
                    }
                    RelayNet.Write(Send, 0, Send.Length);

                    for (int i = 0; i < 1000; i++)
                    {
                        if (BoardClient.Available == ReadLength)
                        {
                            break;
                        }
                        Thread.Sleep(5);
                    }
                    if (BoardClient.Available > 0)
                    {
                        byte[] readBuffer = new byte[BoardClient.Available];
                        RelayNet.Read(readBuffer, 0, readBuffer.Length);
                        return readBuffer;
                    }
                    return new byte[1];
                }
                catch
                {
                    return new byte[1];
                }
            }
        }


        public override void SetBoardClamp(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.Direction Direction)
        {
            SetBoardOnSingleChannel(Channel);
           
        }

        public override void SetBoardAdjustDriveVoltage()
        {
            object obj = BoardClampConfig[0].Value;
            if (obj is double[] doubleArray)
            {
                // 直接使用 doubleArray
                for (int i = 0; i < doubleArray.Length; i++)
                {
                    SetBoardSupplyVoltage(Supply.Positive, i, doubleArray[i]);
                }
            }

        }


        public override void CloseBoardSerialPort()
        {
            
        }
    }
}
