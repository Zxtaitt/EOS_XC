using System;
using System.IO;
using System.Threading;
using ElectricalOverStressData;
using OscilloscopeDriver;

namespace ElectricalOverStressProcess
{
    /// <summary>
    /// 连续采图：不连驱动板，仅使用示波器。
    /// 按计划中通道1的配置设置示波器（含触发方式），让示波器连续运行(:RUN)后，
    /// 每隔“通道1的示波器基础时间刻度 × 10”秒截取一张当前屏幕，共 96 张，命名 Channel_通道号_时间戳。
    /// 与 EOS 检测流程相互独立，不受 Process 中的调试开关影响。
    /// </summary>
    public class OscilloscopeCapture
    {
        public event EventHandler LogHandler;
        public event EventHandler CompletedHandler;

        private const int ImageCount = 96;
        private const int FirstChannelKey = 1;
        private const double IntervalScaleFactor = 10.0;

        private readonly CommunicationInfo Communication;
        private readonly DataPlan Plan;

        public bool Stop
        { get; set; }

        public OscilloscopeCapture(CommunicationInfo communication, DataPlan plan)
        {
            Communication = communication;
            Plan = plan;
        }

        public void Run()
        {
            VisaComInstrument Oscilloscope = null;
            try
            {
                ChannelItem Item = Plan.ChannelItem[FirstChannelKey][0];
                double IntervalSeconds = Item.OscilloscopeTimeScale * IntervalScaleFactor;
                if (IntervalSeconds <= 0)
                {
                    SetLog("错误:采图间隔计算为 " + IntervalSeconds + " 秒(<=0)，请检查通道1的“示波器基础时间刻度”!");
                    return;
                }
                int IntervalMs = (int)Math.Round(IntervalSeconds * 1000);

                string SaveDir = CreateSaveDirectory();
                SetLog("连续采图准备就绪: 共 " + ImageCount + " 张, 间隔 " + IntervalSeconds + " 秒, 保存目录: " + SaveDir);

                Oscilloscope = new VisaComInstrument(Communication.OtherItem.OscilloscopeAaddress);
                Oscilloscope.SetTimeoutSeconds(10);
                SetLog("示波器信息: " + Oscilloscope.DoQueryString("*IDN?"));
                Oscilloscope.DoCommand(":HARDcopy:INKSaver OFF");

                ApplyOscilloscopeSetting(Oscilloscope, Item);
                Oscilloscope.DoCommand(":RUN");
                System.Threading.Thread.Sleep(1000);

                for (int Index = 1; Index <= ImageCount; Index++)
                {
                    if (Stop)
                    {
                        SetLog("人工停止!");
                        break;
                    }
                    CaptureOne(Oscilloscope, SaveDir, Index);
                    if (Index < ImageCount && !WaitInterval(IntervalMs))
                    {
                        SetLog("人工停止!");
                        break;
                    }
                }
                SetLog("连续采图结束!");
            }
            catch (Exception ex)
            {
                SetLog("错误:连续采图异常! " + ex.ToString());
            }
            finally
            {
                if (Oscilloscope != null)
                {
                    Oscilloscope.Close();
                }
                Completed();
            }
        }

        private void CaptureOne(VisaComInstrument Oscilloscope, string SaveDir, int Index)
        {
            try
            {
                byte[] ResultsArray = Oscilloscope.DoQueryIEEEBlock(":DISPlay:DATA? PNG, COLor");
                string FileName = BuildCaptureFileName(Index, DateTime.Now);
                string SavePath = Path.Combine(SaveDir, FileName);
                using (FileStream fStream = File.Open(SavePath, FileMode.Create))
                {
                    fStream.Write(ResultsArray, 0, ResultsArray.Length);
                }
                SetLog("已保存 " + FileName + " (" + Index + "/" + ImageCount + ")");
            }
            catch (Exception ex)
            {
                SetLog("错误:Channel_" + Index + " 采图失败! " + ex.Message);
            }
        }

        internal static string BuildCaptureFileName(int Channel, DateTime CaptureTime)
        {
            return "Channel_" + Channel + "_" + CaptureTime.ToString("yyyy_MM_dd_HH_mm_ss") + ".PNG";
        }

        private string CreateSaveDirectory()
        {
            string BaseDir = Communication.OtherItem.IamgePath;
            if (string.IsNullOrWhiteSpace(BaseDir))
            {
                BaseDir = "D:\\ElectricalOverStressPicture";
            }
            string SaveDir = Path.Combine(BaseDir, "连拍_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            if (!Directory.Exists(SaveDir))
            {
                Directory.CreateDirectory(SaveDir);
            }
            return SaveDir;
        }

        // 等待采图间隔，期间轮询停止标志；返回 false 表示被人工停止。
        private bool WaitInterval(int IntervalMs)
        {
            int Waited = 0;
            while (Waited < IntervalMs)
            {
                if (Stop)
                {
                    return false;
                }
                System.Threading.Thread.Sleep(100);
                Waited += 100;
            }
            return true;
        }

        // 按通道1配置设置示波器（触发源/斜率/电平/单位/量程/时基）。
        // 与检测流程 OscilloscopeSet 的区别：结尾使用连续运行 :RUN（由调用方发送），
        // 且通道显示遍历的是触发源通道枚举(CHANnel1~4)，确保只显示目标通道。
        private void ApplyOscilloscopeSetting(VisaComInstrument Oscilloscope, ChannelItem Item)
        {
            string TriggerSource = Item.OscilloscopeTriggerSource.ToString();
            foreach (string Source in Enum.GetNames(typeof(DataEnum.TriggerSource)))
            {
                Oscilloscope.DoCommand(":" + Source + ":DISPlay " + (Source == TriggerSource ? "1" : "0"));
            }
            Oscilloscope.DoCommand(":" + TriggerSource + ":UNITs " + Item.OscilloscopeChannelSource.ToString());
            Oscilloscope.DoCommand(":" + TriggerSource + ":SCALe " + Item.OscilloscopeChannelRange.ToString());
            Oscilloscope.DoCommand(":TRIGger:MODE EDGE");
            Oscilloscope.DoCommand(":TRIGger:EDGE:SOURce " + TriggerSource);
            Oscilloscope.DoCommand(":TRIGGER:EDGE:SLOPE " + Item.OscilloscopeTriggerSlope.ToString());
            Oscilloscope.DoCommand(":TRIGger:EDGE:LEVel " + Item.OscilloscopeTriggerLevel.ToString());
            Oscilloscope.DoCommand(":TIMebase:DELay " + Item.OscilloscopeBaseDelay.ToString());
            Oscilloscope.DoCommand(":TIMebase:SCALe " + Item.OscilloscopeTimeScale.ToString());
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
