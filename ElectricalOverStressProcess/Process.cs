using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO.Ports;
using System.IO;
using BoardDriver;
using OscilloscopeDriver;
using ElectricalOverStressData;
using System.Net.Sockets;

namespace ElectricalOverStressProcess
{

    public class Process
    {

        public event EventHandler ControlEnableHandler;
        public event EventHandler LogHandler;
        public event EventHandler ShowColorHandler;
        // 临时调试开关：true 时屏蔽示波器相关调用，仅验证驱动板上/下电流程。
        private const bool DisableOscilloscopeForDebug = false;
        private VisaComInstrument Oscilloscope;
        private List<IBoardDriver> BoardDirver;
        private int StartChannel;
        private int EndChannel;
        private DataPlan Plan;
        private CommunicationInfo Communication;
        private DataEnum.Location Location;
        private string DrawerNumbered;
        public bool Stop
        { get; set; }
        public Process(int startChannel, int endChannel, DataPlan plan, CommunicationInfo communication, DataEnum.Location location,string drawerNumbered)
        {
            StartChannel = startChannel;
            EndChannel = endChannel;
            Plan = plan;
            Communication = communication;
            Location = location;
            DrawerNumbered = drawerNumbered;
        }
        public bool Initialize(out string Message)
        {
            try
            {
                Message = "";
                if (!DisableOscilloscopeForDebug && Oscilloscope == null)
                {
                    Oscilloscope = new VisaComInstrument(Communication.OtherItem.OscilloscopeAaddress);
                    Oscilloscope.SetTimeoutSeconds(10);
                    Message = "Oscilloscope information is: " + Oscilloscope.DoQueryString("*IDN?");
                    Oscilloscope.DoCommand(":HARDcopy:INKSaver OFF");
                }
                if (BoardDirver == null)
                {
                    BoardDirver = new List<IBoardDriver>();
                    SerialPort DriveBoardSerialPort = null;
                    TcpClient tcpClient = new TcpClient();
                    string ipaddr = "";
                    if (Communication.OtherItem.BoardSerialPort.Contains(":"))
                    {
                        ipaddr = Communication.OtherItem.BoardSerialPort;
                    }
                    else
                    {
                        DriveBoardSerialPort = new SerialPort(Communication.OtherItem.BoardSerialPort, 115200);
                    }                 
                    foreach (CommunicationBoardItem BoardItem in Communication.BoardItem)
                    {
                        if (BoardItem.BoardLocation == Location)
                        {
                            IBoardDriver iBoardDirver = Activator.CreateInstance(GetModel("BoardDriver_" + Plan.BoardType.ToString())) as IBoardDriver;
                            iBoardDirver.BoardSerialPort = DriveBoardSerialPort;
                            iBoardDirver.BoardClientConStr = ipaddr;
                            iBoardDirver.BoardClient = tcpClient;
                            iBoardDirver.BoardAddress = (byte)BoardItem.BoardAddress;
                            BoardDirver.Add(iBoardDirver);
                            iBoardDirver.SetBoardAdjustDriveVoltage();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return false;
            }
        }
        public void DetectionProcess()
        {
            for (int i = StartChannel; i <= EndChannel; i++)
            {
                ShowColor(BaseProcess(i));
                if (Stop)
                {
                    break;
                }
            }
            if (BoardDirver != null)
            {
                foreach (IBoardDriver driver in BoardDirver)
                {
                    try
                    {
                        if (IsDeferredCloseDriver(driver))
                        {
                            ((BoardDriver_FW03744A00)driver).RunEndOfTestInitialization();
                        }
                    }
                    catch
                    {

                    }
                }
            }
            BoardDirver = null;
            ControlEnable();
        }
        private DetectionResult BaseProcess(int Channel)
        {
            DetectionResult Result = new DetectionResult();
            Result.Channel = Channel;
            try
            {
                int BaseChannel = Channel;
                int BoardDirverIndex = GetBoardDirverIndex(out BaseChannel, Channel + 1);
                if (Plan.ChannelItem.ContainsKey(BaseChannel + 1))  //确认是否包含指定键
                {
                    SetLog("通道: " + (Channel + 1).ToString() + "开始验证!");
                    foreach(ChannelItem Item in Plan.ChannelItem[BaseChannel + 1])
                    {
                        if (!DisableOscilloscopeForDebug)
                        {
                            OscilloscopeSet(Item);
                            System.Threading.Thread.Sleep(1000);
                        }
                        BoardDriverEnum.Direction Direction = BoardDriverEnum.Direction.Positive;
                        if (Item.SourceValue < 0)
                        {
                            Direction = BoardDriverEnum.Direction.Negative;
                        }
                        SetLog("通道: " + (Channel + 1).ToString() + "开始钳制!");
                        BoardDirver[BoardDirverIndex].SetBoardClamp(BaseChannel, Item.SourceType, Direction);
                        SetLog("通道: " + (Channel + 1).ToString() + "完成钳制!");
                        System.Threading.Thread.Sleep(1000);
                        BoardDirver[BoardDirverIndex].SetBoardOnPower(BaseChannel, Item.SourceType, Item.SetMethod, Item.StepCount, Item.SourceValue);
                        SetLog("通道: " + (Channel + 1).ToString() + "完成加电!");
                        System.Threading.Thread.Sleep(1000);
                        BoardDirver[BoardDirverIndex].SetBoardOFFPower(BaseChannel, Item.SourceType, Item.SetMethod, Item.StepCount, Item.SourceValue);
                        SetLog("通道: " + (Channel + 1).ToString() + "完成下电!");
                        System.Threading.Thread.Sleep(3000);
                        if (DisableOscilloscopeForDebug)
                        {
                            SetLog("通道: " + (Channel + 1).ToString() + "已屏蔽示波器流程（调试模式）!");
                            Result.Result = DataEnum.Result.Pass;
                        }
                        else
                        {
                            string Read = Oscilloscope.DoQueryString(":TER?");
                            if (!Read.Contains("+1"))
                            {
                                SetLog("通道: " + (Channel + 1).ToString() + "未触发示波器!");
                                BoardDirver[BoardDirverIndex].CloseBoardSerialPort();
                                Result.Result = DataEnum.Result.Fail;
                                return Result;
                            }
                            byte[] ResultsArray = Oscilloscope.DoQueryIEEEBlock(":DISPlay:DATA? PNG, COLor");
                            string SaveName = "Channel_" + (Channel + 1).ToString() + "-" + Item.SourceType.ToString() + "_" + Item.SourceValue.ToString();
                            SaveIamge(ResultsArray, SaveName);
                            SetLog("保存图片: 《" + SaveName + "》完成!");
                            Result.Result = DataEnum.Result.Pass;
                        }
                        if (Stop)
                        {
                            SetLog("人工停止!");
                            break;
                        }
                    }
                        BoardDirver[BoardDirverIndex].CloseBoardSerialPort();
                    return Result;
                }
                else
                {
                    Result.Result = DataEnum.Result.Skip;
                    SetLog("通道: " + (Channel + 1).ToString() + "未搜索到校准条件!");
                    return Result;
                }
            }
            catch (Exception ex)
            {
                Result.Result = DataEnum.Result.Fail;
                SetLog("错误:通道: " + (Channel + 1).ToString() + "EOS异常!" + ex.ToString());
                return Result;
            }
        }
        private bool IsDeferredCloseDriver(IBoardDriver driver)
        {
            return driver is BoardDriver_FW03744A00;
        }
        private void ShowColor(DetectionResult Result)
        {
            if (ShowColorHandler != null)
            {
                ShowColorHandler.Invoke(Result, null);
            }
        }
        private void ControlEnable()
        {
            if (ControlEnableHandler != null)
            {
                ControlEnableHandler.Invoke(null, null);
            }
        }
        private void SetLog(string log)
        {
            if (LogHandler != null)
            {
                LogHandler.Invoke(log, null);
            }
        }
        private int GetBoardDirverIndex(out int BoardChannel, int Channel)
        {
            for (int i = 0; i < BoardDirver.Count; i++)
            {
                if (Channel <= (i + 1) * Plan.ChannelCount)
                {
                    BoardChannel = Channel - (i * Plan.ChannelCount) - 1;
                    return i;
                }
            }
            BoardChannel = Channel - 1;
            return 0;
        }
        private Type GetModel(string Model)
        {
            Assembly asby = Assembly.LoadFile(Application.StartupPath + "\\BoardDriver.dll");
            Type[] tps = asby.GetTypes();
            Type tp = null;
            foreach (var item in tps)
            {
                if (item.Name == Model)
                {
                    tp = item;
                    break;
                }
            }
            return tp;
        }
        private void SaveIamge(byte[] ResultsArray, string name)
        {
            ChannelItem Item = new ChannelItem();
            if (!Directory.Exists(Communication.OtherItem.IamgePath))
            {
                Directory.CreateDirectory(Communication.OtherItem.IamgePath);
            }
            string SavePath = Communication.OtherItem.IamgePath + "\\" + DrawerNumbered+ Item.SourceType.ToString() + "_" + Item.SourceValue.ToString();
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }
            name = name + DateTime.Now.ToString("-yyyy_MM_dd_HH_mm_ss");
            SavePath = SavePath + "\\" + name + ".PNG";
            FileStream fStream = File.Open(SavePath, FileMode.Create);
            fStream.Write(ResultsArray, 0, ResultsArray.Length);
            fStream.Close();
        }
        private void OscilloscopeSet(ChannelItem Item)  //示波器
        {
            string[] ChannelSource = Enum.GetNames(typeof(DataEnum.ChannelSource));
            foreach (string Source in ChannelSource)
            {
                if (Source == Item.OscilloscopeTriggerSource.ToString())
                {
                    Oscilloscope.DoCommand(":" + Source + ":DISPlay 1");
                }
                else
                {
                    Oscilloscope.DoCommand(":" + Source + ":DISPlay 0");
                }
            }
            Oscilloscope.DoCommand(":" + Item.OscilloscopeTriggerSource.ToString() + ":UNITs " + Item.OscilloscopeChannelSource.ToString());
            Oscilloscope.DoCommand(":" + Item.OscilloscopeTriggerSource.ToString() + ":SCALe " + Item.OscilloscopeChannelRange.ToString());
            Oscilloscope.DoCommand(":TRIGger:MODE EDGE");
            Oscilloscope.DoCommand(":TRIGger:EDGE:SOURce " + Item.OscilloscopeTriggerSource.ToString());
            Oscilloscope.DoCommand(":TRIGGER:EDGE:SLOPE " + Item.OscilloscopeTriggerSlope.ToString());
            Oscilloscope.DoCommand(":TRIGger:EDGE:LEVel " + Item.OscilloscopeTriggerLevel.ToString());
            Oscilloscope.DoCommand(":TIMebase:DELay " + Item.OscilloscopeBaseDelay.ToString());
            Oscilloscope.DoCommand(":TIMebase:SCALe " + Item.OscilloscopeTimeScale.ToString());
            Oscilloscope.DoCommand(":SINGle");
        }
    }
}
