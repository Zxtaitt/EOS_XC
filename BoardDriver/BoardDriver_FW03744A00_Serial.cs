using System;
using System.IO.Ports;

namespace BoardDriver
{
    /// <summary>
    /// PB3744A 串口驱动入口类。
    /// 复用 FW03744A00 的业务时序，通信层自动走 SerialPort。
    /// </summary>
    public class BoardDriver_FW03744A00_Serial : BoardDriver_FW03744A00
    {
        public int BaudRate { get; set; } = 115200;
        public Parity Parity { get; set; } = Parity.None;
        public int DataBits { get; set; } = 8;
        public StopBits StopBits { get; set; } = StopBits.One;
        public Handshake Handshake { get; set; } = Handshake.None;
        public int ReadTimeoutMs { get; set; } = 3000;
        public int WriteTimeoutMs { get; set; } = 3000;

        public BoardDriver_FW03744A00_Serial()
        {
            PreferredTransport = TransportMode.Serial;
        }

        /// <summary>
        /// 显式串口客户端连接，避免隐式自动切换。
        /// </summary>
        public void ConnectSerialClient()
        {
            if (BoardSerialPort == null)
            {
                throw new Exception("BoardSerialPort is null");
            }

            BoardSerialPort.BaudRate = BaudRate;
            BoardSerialPort.Parity = Parity;
            BoardSerialPort.DataBits = DataBits;
            BoardSerialPort.StopBits = StopBits;
            BoardSerialPort.Handshake = Handshake;
            BoardSerialPort.ReadTimeout = ReadTimeoutMs;
            BoardSerialPort.WriteTimeout = WriteTimeoutMs;

            if (!BoardSerialPort.IsOpen)
            {
                BoardSerialPort.Open();
            }
        }
    }
}
