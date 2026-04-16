using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BoardDriver
{
    public class BoardDriver_HW : IBoardDriver
    {
        public SerialPort BoardSerialPort { get; set; }
        // public TcpClient BoardClient;
        private NetworkStream testNet;
        private object LockObj = new object();
        public byte BoardAddress { get; set; }
        public string BoardClientConStr { get; set; }
        public TcpClient BoardClient { get; set; }

        public BoardDriver_HW()
        {
            if (BoardClient == null)
            {
                BoardClient = new TcpClient();
            }
        }
        public void SetBoardAdjustDriveVoltage()
        {

        }
        public void CloseBoardSerialPort()
        {

        }

        public void SetBoardClamp(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.Direction Direction)
        {
            List<byte> cmd = new List<byte>();

            cmd.Add(0xAA);
            cmd.Add(0x55);

            cmd.Add(00);
            cmd.Add(00);

            cmd.Add(0x01);
            cmd.Add(0x96);


            cmd.Add(0);
            cmd.Add(0);


            //Address
            cmd.Add((byte)BoardAddress);

            cmd.Add(0);

            cmd.Add((byte)Channel);

            //output  true 1开通道, false  0关通道
            cmd.Add((byte)1);


            cmd.Add(0x55);
            cmd.Add(0xAA);


            cmd[2] = GetHighByte(cmd.Count);
            cmd[3] = GetLowByte(cmd.Count);

            byte[] r = WriteCMD(cmd.ToArray(), 14, "SetChannelOutput");

            if ((r[6] + r[7]) != 0)
            {
                throw new Exception("SetChannelOutput");
            }
        }

        private byte GetLowByte(int data)
        {
            return Convert.ToByte(data & 0xFF);

        }

        public byte GetHighByte(int data)
        {
            return Convert.ToByte((data >> 8) & 0xFF);
        }

        public void SetBoardOFFPower(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.PowerMethod PowerMethod, int Step, double SetValue)
        {
            List<byte> cmd = new List<byte>();

            cmd.Add(0xAA);
            cmd.Add(0x55);

            cmd.Add(00);
            cmd.Add(00);

            cmd.Add(0x01);
            cmd.Add(0xA1);


            cmd.Add(0);
            cmd.Add(0);

            //Address
            cmd.Add((byte)BoardAddress);
            cmd.Add(0);
            cmd.Add((byte)Channel);
            cmd.Add((byte)0);//Mode

            cmd.Add(GetHighByte(0));//Steps
            cmd.Add(GetLowByte(0));//Steps

            cmd.Add(GetHighByte(0));//StepDelay
            cmd.Add(GetLowByte(0));//StepDelay


            cmd.Add(0);

            cmd.Add(0);
            cmd.Add(0);
            cmd.Add(GetHighByte(0));//DAC
            cmd.Add(GetLowByte(0));//DAC


            cmd.Add(0x55);
            cmd.Add(0xAA);

            cmd[2] = GetHighByte(cmd.Count);
            cmd[3] = GetLowByte(cmd.Count);


            byte[] r = WriteCMD(cmd.ToArray(), 23, "SetChannelFloatOutput");

            if ((r[6] + r[7]) != 0)
            {
                throw new Exception("SetChannelFloatOutput");
            }
        }

        public void SetBoardOnPower(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.PowerMethod PowerMethod, int Step, double SetValue)
        {
            List<byte> cmd = new List<byte>();

            cmd.Add(0xAA);
            cmd.Add(0x55);

            cmd.Add(00);
            cmd.Add(00);

            cmd.Add(0x01);
            cmd.Add(0xA1);


            cmd.Add(0);
            cmd.Add(0);

            //Address
            cmd.Add((byte)BoardAddress);
            cmd.Add(0);
            cmd.Add((byte)Channel);
            //Mode 
            cmd.Add((byte)1);

            cmd.Add(GetHighByte(0));//Steps
            cmd.Add(GetLowByte(0x05));//Steps

            cmd.Add(GetHighByte(0));//StepDelay
            cmd.Add(GetLowByte(0x10));//StepDelay


            cmd.Add(1);

            float f = (float)SetValue;//Data
            cmd.AddRange(BitConverter.GetBytes(f));

            cmd.Add(0x55);
            cmd.Add(0xAA);

            cmd[2] = GetHighByte(cmd.Count);
            cmd[3] = GetLowByte(cmd.Count);


            byte[] r = WriteCMD(cmd.ToArray(), 23, "SetChannelFloatOutput");

            if ((r[6] + r[7]) != 0)
            {
                throw new Exception("SetChannelFloatOutput");
            }
            ChannelOut(Channel);
        }

        private byte[] WriteCMD(byte[] cmd, int returnLength, string CmdName)
        {
            try
            {
                lock (LockObj)
                {
                    NetworkStream BoardNet = null;
                    if (!BoardClient.Connected)
                    {
                        var IPAdress = BoardClientConStr.Split(':')[0];
                        var PortAdress = BoardClientConStr.Split(':')[1];
                        BoardClient.Connect(IPAdress, Convert.ToInt32(PortAdress));
                        BoardNet = BoardClient.GetStream();
                        BoardNet.ReadTimeout = 1500;
                        BoardNet.WriteTimeout = 1500;
                    }
                    else
                    {
                        BoardNet = BoardClient.GetStream();
                    }
                    //for (int i = 0; i < 100; i++)
                    //{
                    //    if (BoardNet.DataAvailable)
                    //    {
                    //        byte[] clearBuffer = new byte[1024];
                    //        BoardNet.Read(clearBuffer, 0, clearBuffer.Length);
                    //        break;
                    //    }
                    //    System.Threading.Thread.Sleep(10);
                    //}

                    byte[] readbuffer = new byte[returnLength];
                    BoardNet.Write(cmd, 0, cmd.Length);


                    for (int i = 0; i < 100; i++)
                    {
                        if (BoardNet.DataAvailable)
                        {
                            int r = BoardNet.Read(readbuffer, 0, readbuffer.Length);
                            List<byte> mem = new List<byte>();

                            for (int j = 0; j < r; j++)
                            {
                                mem.Add(readbuffer[j]);
                            }
                            return mem.ToArray();
                        }
                        System.Threading.Thread.Sleep(100);

                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public void CloseRelay(int channel)
        {
            List<byte> cmd = new List<byte>();

            //Address
            cmd.Add(0x01);
            cmd.Add(05);
            cmd.Add(0);
            cmd.Add((byte)channel);
            cmd.Add(0xff);
            cmd.Add(0);
            byte[] crc = Tool.CRC16(cmd.ToArray());

            cmd.Add(crc[1]);
            cmd.Add(crc[0]);

            byte[] r = Send485Cmd(cmd.ToArray());

            for (int i = 0; i < r.Length; i++)
            {
                if (r[i] != cmd[i])
                {
                    throw new Exception("关闭继电器失败");
                }
            }
            SetConfig(0x91);
            SetConfig(0x92);
        }

        public void OpenRelay(int channel)
        {
            List<byte> cmd = new List<byte>();

            //Address
            cmd.Add(0x01);
            cmd.Add(05);
            cmd.Add(0);
            cmd.Add((byte)channel);
            cmd.Add(0);
            cmd.Add(0);
            byte[] crc = Tool.CRC16(cmd.ToArray());

            cmd.Add(crc[1]);
            cmd.Add(crc[0]);

            byte[] r = Send485Cmd(cmd.ToArray());
            for (int i = 0; i < r.Length; i++)
            {
                if (r[i] != cmd[i])
                {
                    throw new Exception("打开继电器失败");
                }
            }
           
        }


        private void SetConfig(byte cmdType)
        {
            List<byte> cmd = new List<byte>();
            cmd.Add(0xAA);
            cmd.Add(0x55);
            cmd.Add(00);
            cmd.Add(00);
            cmd.Add(0x01);
            cmd.Add(cmdType);//
            cmd.Add(00);
            cmd.Add(00);
            cmd.Add(BoardAddress);
            cmd.Add(0);
            cmd.Add(1);
            cmd.Add(0x55);
            cmd.Add(0xAA);
            cmd[2] = GetHighByte(cmd.Count);
            cmd[3] = GetLowByte(cmd.Count);
            WriteCMD(cmd.ToArray(), 13, "SetConfig");
        }

        private void ChannelOut(int channel)
        {
            List<byte> cmd = new List<byte>();
            cmd.Add(0xAA);
            cmd.Add(0x55);
            cmd.Add(00);
            cmd.Add(00);
            cmd.Add(0x01);
            cmd.Add(0x96);//
            cmd.Add(00);
            cmd.Add(00);
            cmd.Add(BoardAddress);
            cmd.Add(0);
            cmd.Add((byte)channel);
            cmd.Add(1);
            cmd.Add(0x55);
            cmd.Add(0xAA);
            cmd[2] = GetHighByte(cmd.Count);
            cmd[3] = GetLowByte(cmd.Count);
            WriteCMD(cmd.ToArray(), 14, "ChannelOut");
        }


        public byte[] Send485Cmd(byte[] strcmd)
        {
            List<byte> cmd = new List<byte>();
            cmd.Add(0xAA);
            cmd.Add(0x55);

            cmd.Add(00);
            cmd.Add(0x00);

            cmd.Add(0x00);
            cmd.Add(0xA0);


            cmd.Add(0);
            cmd.Add(0);
            cmd.Add(GetHighByte(9600));
            cmd.Add(GetLowByte(9600));
            cmd.Add(1);



            cmd.AddRange(strcmd);
            cmd.Add(0x55);
            cmd.Add(0xAA);
            cmd[2] = GetHighByte(cmd.Count);
            cmd[3] = GetLowByte(cmd.Count);


            //byte[] r = WriteCMD(cmd.ToArray(), 512, "485CMD", true);

            byte[] r = null;
            for (int i = 0; i < 3; i++)
            {
                r = WriteCMD(cmd.ToArray(), 512, "485CMD");

                if (r != null)
                {
                    if ((r[6] + r[7]) == 0)
                    {
                        break;
                    }



                }
                System.Threading.Thread.Sleep(100);
            }

            if (r == null)
            {
                throw new Exception("485CMD发送不成功,无返回值");
            }
            if ((r[6] + r[7]) != 0)
            {
                throw new Exception("485CMD发送不成功" + (r[6] << 8 | r[7]).ToString("X4"));
            }
            List<byte> rturn = new List<byte>();

            for (int i = 11; i < r.Length - 2; i++)
            {
                rturn.Add(r[i]);
            }
            return rturn.ToArray();

        }
    }
}
