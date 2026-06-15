using System;
using System.IO;
using System.IO.Ports;
using System.Net.Sockets;
using System.Threading;
using BoardDriver;
using ElectricalOverStressData;
using OscilloscopeDriver;

namespace ElectricalOverStressProcess
{
    /// <summary>
    /// 两两采图模式：仅支持 3744 驱动板，固定 48 通道、两两相邻分组（共 24 组）。
    /// 每组（驱动板通道 A、B）：先上 A 再上 B，两路上电波形采 1 张图；先下 A 再下 B，下电波形采 1 张图。
    /// 共 24 组 × 2 张 = 48 张，命名 chA-B_ON / chA-B_OFF。只采图存档，不做 Pass/Fail 判定。
    ///
    /// 实现约定（如与实际不符可调整）：
    /// 1) 组内上/下电使用各自通道的配置（设定值、上电方式、步进数）。
    /// 2) 组钳位按组内第一个通道（A）的源类型/方向统一设置。
    /// 3) 示波器固定显示 CH1+CH2、触发源固定 CH1（对应组内 A 先上电），其余参数取通道 A 的配置。
    /// 4) 组内先上 A 后上 B 的间隔为 OnOffGapMs，需小于示波器一屏时间，保证两路波形落在同一张图内。
    /// </summary>
    public class GroupCapture
    {
        public event EventHandler LogHandler;
        public event EventHandler CompletedHandler;

        private const int TotalChannels = 48;
        private const int GroupSize = 2;
        private const int OnOffGapMs = 300;

        private readonly CommunicationInfo Communication;
        private readonly DataPlan Plan;

        public bool Stop
        { get; set; }

        public GroupCapture(CommunicationInfo communication, DataPlan plan)
        {
            Communication = communication;
            Plan = plan;
        }

        public void Run()
        {
            VisaComInstrument Oscilloscope = null;
            BoardDriver_FW03744A00 Board = null;
            try
            {
                string SaveDir = CreateSaveDirectory();
                int GroupCount = TotalChannels / GroupSize;
                SetLog("两两采图准备就绪: " + GroupCount + " 组, 共 " + (GroupCount * 2) + " 张, 保存目录: " + SaveDir);

                Oscilloscope = new VisaComInstrument(Communication.OtherItem.OscilloscopeAaddress);
                Oscilloscope.SetTimeoutSeconds(10);
                SetLog("示波器信息: " + Oscilloscope.DoQueryString("*IDN?"));
                Oscilloscope.DoCommand(":HARDcopy:INKSaver OFF");

                Board = CreateBoard();
                Board.SetBoardAdjustDriveVoltage();

                for (int g = 0; g < GroupCount; g++)
                {
                    if (Stop)
                    {
                        SetLog("人工停止!");
                        break;
                    }
                    int ChannelA = g * GroupSize;
                    int ChannelB = g * GroupSize + 1;
                    ProcessGroup(Oscilloscope, Board, SaveDir, ChannelA, ChannelB);
                }
                SetLog("两两采图结束!");
            }
            catch (Exception ex)
            {
                SetLog("错误:两两采图异常! " + ex.ToString());
            }
            finally
            {
                if (Board != null)
                {
                    try { Board.RunEndOfTestInitialization(); } catch { }
                }
                if (Oscilloscope != null)
                {
                    Oscilloscope.Close();
                }
                Completed();
            }
        }

        private void ProcessGroup(VisaComInstrument Oscilloscope, BoardDriver_FW03744A00 Board, string SaveDir, int ChannelA, int ChannelB)
        {
            int DisplayA = ChannelA + 1;
            int DisplayB = ChannelB + 1;
            string GroupName = "ch" + DisplayA + "-" + DisplayB;

            ChannelItem ItemA = GetChannelItem(ChannelA);
            ChannelItem ItemB = GetChannelItem(ChannelB);
            if (ItemA == null || ItemB == null)
            {
                SetLog("组 " + GroupName + " 跳过: 通道未在计划中配置!");
                return;
            }

            BoardDriverEnum.Direction DirectionA = ItemA.SourceValue < 0 ? BoardDriverEnum.Direction.Negative : BoardDriverEnum.Direction.Positive;

            int CaptureWaitMs = (int)(ItemA.OscilloscopeTimeScale * 10 * 1000) + 1000;
            if (CaptureWaitMs < 2000)
            {
                CaptureWaitMs = 2000;
            }

            SetLog("组 " + GroupName + " 开始: 钳制通道 " + DisplayA + "," + DisplayB);
            Board.SetGroupClamp(new int[] { ChannelA, ChannelB }, ItemA.SourceType, DirectionA);
            if (!WaitOrStop(500)) return;

            // 上电图：武装示波器 -> 先上 A 再上 B -> 抓一张图
            ApplyOscilloscopeSetting(Oscilloscope, ItemA);
            if (!WaitOrStop(1000)) return;
            SetLog("组 " + GroupName + " 上电: 先 " + DisplayA + " 后 " + DisplayB);
            Board.SetBoardOnPower(ChannelA, ItemA.SourceType, ItemA.SetMethod, ItemA.StepCount, ItemA.SourceValue);
            if (!WaitOrStop(OnOffGapMs)) return;
            Board.SetBoardOnPower(ChannelB, ItemB.SourceType, ItemB.SetMethod, ItemB.StepCount, ItemB.SourceValue);
            if (!WaitOrStop(CaptureWaitMs)) return;
            CaptureImage(Oscilloscope, SaveDir, GroupName + "_ON");

            // 下电图：重新武装 -> 先下 A 再下 B -> 抓一张图
            ApplyOscilloscopeSetting(Oscilloscope, ItemA);
            if (!WaitOrStop(1000)) return;
            SetLog("组 " + GroupName + " 下电: 先 " + DisplayA + " 后 " + DisplayB);
            Board.OffPowerChannelKeepOutput(ChannelA, ItemA.SourceType, ItemA.SetMethod, ItemA.StepCount, ItemA.SourceValue);
            if (!WaitOrStop(OnOffGapMs)) return;
            Board.OffPowerChannelKeepOutput(ChannelB, ItemB.SourceType, ItemB.SetMethod, ItemB.StepCount, ItemB.SourceValue);
            if (!WaitOrStop(CaptureWaitMs)) return;
            CaptureImage(Oscilloscope, SaveDir, GroupName + "_OFF");

            Board.CloseGroupOutput(new int[] { ChannelA, ChannelB });
            SetLog("组 " + GroupName + " 完成!");
        }

        private ChannelItem GetChannelItem(int channelZeroBased)
        {
            int key = channelZeroBased + 1;
            if (Plan.ChannelItem != null && Plan.ChannelItem.ContainsKey(key)
                && Plan.ChannelItem[key] != null && Plan.ChannelItem[key].Count > 0)
            {
                return Plan.ChannelItem[key][0];
            }
            return null;
        }

        // 两两采图固定使用示波器 CH1、CH2（组内通道分别接 CH1、CH2），触发源固定 CH1。
        private void ApplyOscilloscopeSetting(VisaComInstrument Oscilloscope, ChannelItem Item)
        {
            Oscilloscope.DoCommand(":CHANnel1:DISPlay 1");
            Oscilloscope.DoCommand(":CHANnel2:DISPlay 1");
            Oscilloscope.DoCommand(":CHANnel3:DISPlay 0");
            Oscilloscope.DoCommand(":CHANnel4:DISPlay 0");
            Oscilloscope.DoCommand(":CHANnel1:UNITs " + Item.OscilloscopeChannelSource.ToString());
            Oscilloscope.DoCommand(":CHANnel2:UNITs " + Item.OscilloscopeChannelSource.ToString());
            Oscilloscope.DoCommand(":CHANnel1:SCALe " + Item.OscilloscopeChannelRange.ToString());
            Oscilloscope.DoCommand(":CHANnel2:SCALe " + Item.OscilloscopeChannelRange.ToString());
            Oscilloscope.DoCommand(":TRIGger:MODE EDGE");
            Oscilloscope.DoCommand(":TRIGger:EDGE:SOURce CHANnel1");
            Oscilloscope.DoCommand(":TRIGGER:EDGE:SLOPE " + Item.OscilloscopeTriggerSlope.ToString());
            Oscilloscope.DoCommand(":TRIGger:EDGE:LEVel " + Item.OscilloscopeTriggerLevel.ToString());
            Oscilloscope.DoCommand(":TIMebase:DELay " + Item.OscilloscopeBaseDelay.ToString());
            Oscilloscope.DoCommand(":TIMebase:SCALe " + Item.OscilloscopeTimeScale.ToString());
            Oscilloscope.DoCommand(":SINGle");
        }

        private void CaptureImage(VisaComInstrument Oscilloscope, string SaveDir, string Name)
        {
            try
            {
                byte[] ResultsArray = Oscilloscope.DoQueryIEEEBlock(":DISPlay:DATA? PNG, COLor");
                string SavePath = Path.Combine(SaveDir, Name + ".PNG");
                using (FileStream fStream = File.Open(SavePath, FileMode.Create))
                {
                    fStream.Write(ResultsArray, 0, ResultsArray.Length);
                }
                SetLog("已保存 " + Name);
            }
            catch (Exception ex)
            {
                SetLog("错误:" + Name + " 采图失败! " + ex.Message);
            }
        }

        private BoardDriver_FW03744A00 CreateBoard()
        {
            if (Communication.BoardItem == null || Communication.BoardItem.Count == 0)
            {
                throw new Exception("未配置驱动板信息!");
            }
            if (Plan.BoardType != BoardDriverEnum.BoardType.FW03744A00
                && Plan.BoardType != BoardDriverEnum.BoardType.FW03744A00_Serial)
            {
                throw new Exception("两两采图模式目前仅支持 3744 驱动板 (当前: " + Plan.BoardType + ")");
            }

            CommunicationBoardItem BoardItem = Communication.BoardItem[0];
            string SerialOrIp = Communication.OtherItem.BoardSerialPort;

            SerialPort DriveSerialPort = null;
            string Ipaddr = "";
            if (SerialOrIp != null && SerialOrIp.Contains(":"))
            {
                Ipaddr = SerialOrIp;
            }
            else
            {
                DriveSerialPort = new SerialPort(SerialOrIp, 115200);
            }

            BoardDriver_FW03744A00 Board = (Plan.BoardType == BoardDriverEnum.BoardType.FW03744A00_Serial)
                ? new BoardDriver_FW03744A00_Serial()
                : new BoardDriver_FW03744A00();

            Board.BoardSerialPort = DriveSerialPort;
            Board.BoardClientConStr = Ipaddr;
            Board.BoardClient = new TcpClient();
            Board.BoardAddress = (byte)BoardItem.BoardAddress;

            if (Board is BoardDriver_FW03744A00_Serial SerialBoard)
            {
                ConfigureSerial(SerialBoard, SerialOrIp);
                SerialBoard.ConnectSerialClient();
            }
            return Board;
        }

        private void ConfigureSerial(BoardDriver_FW03744A00_Serial serialDriver, string serialConfig)
        {
            string[] parts = (serialConfig ?? "").Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                throw new Exception("驱动板串口配置为空!");
            }

            string portName = parts[0].Trim();
            SerialPort sp = serialDriver.BoardSerialPort;
            if (sp == null || !string.Equals(sp.PortName, portName, StringComparison.OrdinalIgnoreCase))
            {
                sp = new SerialPort(portName, 115200);
                serialDriver.BoardSerialPort = sp;
            }

            if (parts.Length > 1 && int.TryParse(parts[1].Trim(), out int baud)) serialDriver.BaudRate = baud;
            if (parts.Length > 2 && TryParseParity(parts[2].Trim(), out Parity parity)) serialDriver.Parity = parity;
            if (parts.Length > 3 && int.TryParse(parts[3].Trim(), out int dataBits)) serialDriver.DataBits = dataBits;
            if (parts.Length > 4 && TryParseStopBits(parts[4].Trim(), out StopBits stopBits)) serialDriver.StopBits = stopBits;
            if (parts.Length > 5 && int.TryParse(parts[5].Trim(), out int readTimeout)) serialDriver.ReadTimeoutMs = readTimeout;
            if (parts.Length > 6 && int.TryParse(parts[6].Trim(), out int writeTimeout)) serialDriver.WriteTimeoutMs = writeTimeout;
        }

        private bool TryParseParity(string text, out Parity parity)
        {
            parity = Parity.None;
            switch ((text ?? "").Trim().ToUpperInvariant())
            {
                case "N":
                case "NONE":
                    parity = Parity.None;
                    return true;
                case "O":
                case "ODD":
                    parity = Parity.Odd;
                    return true;
                case "E":
                case "EVEN":
                    parity = Parity.Even;
                    return true;
                case "M":
                case "MARK":
                    parity = Parity.Mark;
                    return true;
                case "S":
                case "SPACE":
                    parity = Parity.Space;
                    return true;
                default:
                    return false;
            }
        }

        private bool TryParseStopBits(string text, out StopBits stopBits)
        {
            stopBits = StopBits.One;
            switch ((text ?? "").Trim())
            {
                case "1":
                    stopBits = StopBits.One;
                    return true;
                case "1.5":
                    stopBits = StopBits.OnePointFive;
                    return true;
                case "2":
                    stopBits = StopBits.Two;
                    return true;
                default:
                    return false;
            }
        }

        private string CreateSaveDirectory()
        {
            string BaseDir = Communication.OtherItem.IamgePath;
            if (string.IsNullOrWhiteSpace(BaseDir))
            {
                BaseDir = "D:\\ElectricalOverStressPicture";
            }
            string SaveDir = Path.Combine(BaseDir, "两两采图_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            if (!Directory.Exists(SaveDir))
            {
                Directory.CreateDirectory(SaveDir);
            }
            return SaveDir;
        }

        // 分片等待，期间轮询停止标志；返回 false 表示被人工停止。
        private bool WaitOrStop(int ms)
        {
            int waited = 0;
            while (waited < ms)
            {
                if (Stop)
                {
                    return false;
                }
                System.Threading.Thread.Sleep(100);
                waited += 100;
            }
            return true;
        }

        private void SetLog(string log)
        {
            if (LogHandler != null)
            {
                LogHandler.Invoke(log, null);
            }
        }

        private void Completed()
        {
            if (CompletedHandler != null)
            {
                CompletedHandler.Invoke(null, null);
            }
        }
    }
}
