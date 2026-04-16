using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BoardDriver
{
  public  class BoardDriver_FW0089B00_Net: BoardDriver_FW0089B00
    {

        protected override byte[] QueryProduct(byte[] Send, int ReadLength)
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
                    System.Threading.Thread.Sleep(50);
                    for (int i = 0; i < 100; i++)
                    {
                        if (BoardClient.Available== ReadLength)
                        {
                            break;
                        }
                        System.Threading.Thread.Sleep(5);
                    }
                    if (BoardClient.Available > 0)
                    {
                        byte[] readBuffer = new byte[BoardClient.Available];
                        RelayNet.Read(readBuffer, 0, readBuffer.Length);
                        System.Threading.Thread.Sleep(20);
                        return readBuffer;
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
