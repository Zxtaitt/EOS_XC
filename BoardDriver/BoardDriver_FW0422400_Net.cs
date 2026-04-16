using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BoardDriver
{
  public  class BoardDriver_FW0422400_Net: BoardDriver_FW0422400
    {

        protected override byte[] QueryProduct(byte[] Send, int Sleep)
        {
            lock (BoardClient)
            {
                try
                {
                    Thread.Sleep(50);
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
                    if (RelayNet.DataAvailable)
                    {
                        byte[] clearBuffer = new byte[1024];
                        RelayNet.Read(clearBuffer, 0, clearBuffer.Length);
                    }
                    RelayNet.Write(Send, 0, Send.Length);
                    System.Threading.Thread.Sleep(200);
                    var Length = 0;
                    for (int i = 0; i < 1000; i++)
                    {
                        if (BoardClient.Available >= 4)
                        {
                            byte[] readBuffer = new byte[4];
                            RelayNet.Read(readBuffer, 0, readBuffer.Length);
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
                        if (BoardClient.Available == Length)
                        {
                            byte[] readBuffer = new byte[Length];
                            RelayNet.Read(readBuffer, 0, readBuffer.Length);
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
                    if (!BoardClient.Connected)
                    {
                        BoardClient.Close();
                    }
                    return new byte[1];
                }
            }
        }
    }
}
