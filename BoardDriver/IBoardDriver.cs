using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Net.Sockets;

namespace BoardDriver
{
    public interface IBoardDriver
    {
        string BoardClientConStr { get; set; }
        SerialPort BoardSerialPort { get; set; }
        TcpClient BoardClient { get; set; }
        byte BoardAddress { get; set; }
        void SetBoardClamp(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.Direction Direction);
        void SetBoardOnPower(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.PowerMethod PowerMethod, int Step, double SetValue);
        void SetBoardOFFPower(int Channel, BoardDriverEnum.SourceType SourceType, BoardDriverEnum.PowerMethod PowerMethod, int Step, double SetValue);
        void CloseBoardSerialPort();
        void SetBoardAdjustDriveVoltage();
        void CloseRelay(int channel);
        void OpenRelay(int channel);
    }
}
