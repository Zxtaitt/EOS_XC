using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ElectricalOverStressData;
using ElectricalOverStressProcess;

namespace ElectricalOverStressDetection
{
    public partial class ElectricalOverStressDetection : Form
    {
        private FrmShowInfo fsi;
        private Process DetectionProcess;
        private CommunicationInfo Communication;
        private DataPlan Plan;
        private Task ProcessTask;
        private OscilloscopeCapture CaptureProcess;
        private Task CaptureTask;
        private GroupCapture PairCaptureProcess;
        private Task PairCaptureTask;
        public ElectricalOverStressDetection()
        {
            InitializeComponent();
        }
        private void ElectricalOverStressDetection_Load(object sender, EventArgs e)
        {
            Initialize();
            ButtonControl(true);
        }
        private void button_Start_Click(object sender, EventArgs e)
        {
             try
            {
                StreamReader sr = null;
                if (File.Exists(Global.CommunicationPathName))
                {
                    sr = File.OpenText(Global.CommunicationPathName);
                    Communication = DataTool.GetClassDeserializationXML(sr.ReadToEnd(), typeof(CommunicationInfo)) as CommunicationInfo;
                    sr.Close();
                }
                else
                {
                    MessageBox.Show("错误:未获取到通讯配置信息!");
                    return;
                }
                sr = File.OpenText(Global.PlanPath + Communication.OtherItem.PlanName + ".xml");
                Plan = DataTool.GetClassDeserializationXML(sr.ReadToEnd(), typeof(DataPlan)) as DataPlan;
                sr.Close();
                int Count = Communication.OtherItem.BoardNumber * Plan.ChannelCount;
                FrmBindInfo fb = new FrmBindInfo(Count);
                if (fb.ShowDialog() != DialogResult.Yes)
                {
                    return;
                }
                string DrawerNumber = (fb.BindingInfo.LayerName.ToString() + fb.BindingInfo.LocationName.ToString()).Replace('_', '-');
                DetectionProcess = new Process(Global.Channel.IndexOf(fb.BindingInfo.StartChannel), Global.Channel.IndexOf(fb.BindingInfo.EndChannel), Plan, Communication, fb.BindingInfo.LocationName, DrawerNumber);
                DetectionProcess.LogHandler += Log_Hander;
                DetectionProcess.ControlEnableHandler += ControlEnable_Hander;
                DetectionProcess.ShowColorHandler += ShowColor_Hander;
                string message = "";
                if (!DetectionProcess.Initialize(out message))
                {
                    MessageBox.Show("错误:初始化失败!" + message);
                    return;
                }
                if (fsi == null)
                {
                    fsi = new FrmShowInfo(Count);
                    fsi.TopLevel = false;
                    fsi.FormBorderStyle = FormBorderStyle.None;
                    fsi.Dock = DockStyle.Fill;
                    fsi.Parent = tableLayoutPanel_ShowInfo;
                    fsi.Show();
                    //Thread.Sleep(5000);
                }
                ButtonControl(false);
                //ShowInfo(fb.BindingInfo);
                ProcessTask = new Task(DetectionProcess.DetectionProcess);
                ProcessTask.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button_Stop_Click(object sender, EventArgs e)
        {
            if (DetectionProcess != null)
            {
                DetectionProcess.Stop = true;
            }
            if (CaptureProcess != null)
            {
                CaptureProcess.Stop = true;
            }
            if (PairCaptureProcess != null)
            {
                PairCaptureProcess.Stop = true;
            }
            ButtonControl(true);
            if (fsi != null)
            {
                fsi.Close();
                fsi = null;
            }
            ClearLog();
            propertyGrid_ShowInfo.SelectedObject = null;
        }

        private void button_Capture_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(Global.CommunicationPathName))
                {
                    MessageBox.Show("错误:未获取到通讯配置信息!");
                    return;
                }
                StreamReader sr = File.OpenText(Global.CommunicationPathName);
                Communication = DataTool.GetClassDeserializationXML(sr.ReadToEnd(), typeof(CommunicationInfo)) as CommunicationInfo;
                sr.Close();

                string PlanFile = Global.PlanPath + Communication.OtherItem.PlanName + ".xml";
                if (!File.Exists(PlanFile))
                {
                    MessageBox.Show("错误:未找到测试计划文件! " + PlanFile);
                    return;
                }
                sr = File.OpenText(PlanFile);
                Plan = DataTool.GetClassDeserializationXML(sr.ReadToEnd(), typeof(DataPlan)) as DataPlan;
                sr.Close();

                if (Plan == null || Plan.ChannelItem == null || !Plan.ChannelItem.ContainsKey(1)
                    || Plan.ChannelItem[1] == null || Plan.ChannelItem[1].Count == 0)
                {
                    MessageBox.Show("错误:计划中通道1没有配置，无法确定采图参数!");
                    return;
                }

                CaptureProcess = new OscilloscopeCapture(Communication, Plan);
                CaptureProcess.LogHandler += Log_Hander;
                CaptureProcess.CompletedHandler += ControlEnable_Hander;
                ButtonControl(false);
                CaptureTask = new Task(CaptureProcess.Run);
                CaptureTask.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button_PairCapture_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(Global.CommunicationPathName))
                {
                    MessageBox.Show("错误:未获取到通讯配置信息!");
                    return;
                }
                StreamReader sr = File.OpenText(Global.CommunicationPathName);
                Communication = DataTool.GetClassDeserializationXML(sr.ReadToEnd(), typeof(CommunicationInfo)) as CommunicationInfo;
                sr.Close();

                string PlanFile = Global.PlanPath + Communication.OtherItem.PlanName + ".xml";
                if (!File.Exists(PlanFile))
                {
                    MessageBox.Show("错误:未找到测试计划文件! " + PlanFile);
                    return;
                }
                sr = File.OpenText(PlanFile);
                Plan = DataTool.GetClassDeserializationXML(sr.ReadToEnd(), typeof(DataPlan)) as DataPlan;
                sr.Close();

                PairCaptureProcess = new GroupCapture(Communication, Plan);
                PairCaptureProcess.LogHandler += Log_Hander;
                PairCaptureProcess.CompletedHandler += ControlEnable_Hander;
                ButtonControl(false);
                PairCaptureTask = new Task(PairCaptureProcess.Run);
                PairCaptureTask.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button_Plan_Click(object sender, EventArgs e)
        {
            FrmPlanSetting fp = new FrmPlanSetting();
            fp.ShowDialog();
        }
        private void button_Configuration_Click(object sender, EventArgs e)
        {
            FrmCommunicationInfo fc = new FrmCommunicationInfo();
            if (fc.ShowDialog() != DialogResult.Yes)
            {
                return;
            }
        }
        private void Initialize()
        {
            List<string> Name = SerialPort.GetPortNames().ToList();
            if (Name.Count == 0)
            {
                Name.Add("COM1");
            }
            //因为Global全是静态方法和属性，可以直接用类名调用
            Global.SerialPortName = Name;        
            Global.CommunicationPath = @"C:\EOSPlatformPlan\" + "\\Communication\\";
            Global.CommunicationPathName = Global.CommunicationPath + "Communication.xml";
            if (!Directory.Exists(Global.CommunicationPath))
            {
                Directory.CreateDirectory(Global.CommunicationPath);
            }
            Global.PlanPath = @"C:\\EOSPlatformPlan\\" + "\\EOSPlan\\";
            if (!Directory.Exists(Global.PlanPath))
            {
                Directory.CreateDirectory(Global.PlanPath);
            }
        }
        private void ButtonControl(bool Enable)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                button_Start.Enabled = Enable;
                button_Capture.Enabled = Enable;
                button_PairCapture.Enabled = Enable;
                button_Stop.Enabled = !Enable;
            }));
        }
        private void ControlEnable_Hander(object sender, EventArgs e)
        {
            ButtonControl(true);
        }
        private void ShowInfo(BindInfo bi)
        {
            ShowInfo si = new ShowInfo();
            si.DrawerNumber = (bi.LayerName.ToString() + bi.LocationName.ToString()).Replace('_', '-');
            si.PlanName = "";
            propertyGrid_ShowInfo.SelectedObject = si;
        }
        private void Log_Hander(object sender, EventArgs e)
        {
            SetLog(sender.ToString());
        }
        private void ClearLog()
        {
            this.Invoke(new MethodInvoker(() =>
            {
                richTextBox_Log.Clear();
            }));
        }
        private void SetLog(string Message)
        {
            try
            {
                string path = "D:\\ElectricalOverStressDetectionLog\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                this.Invoke(new MethodInvoker(() =>
                {
                    Message += "------" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n";
                    richTextBox_Log.AppendText(Message);
                    richTextBox_Log.Select(richTextBox_Log.TextLength - Message.Length > 0 ? richTextBox_Log.TextLength - Message.Length + 1 : 0, Message.Length + 1);
                    richTextBox_Log.ScrollToCaret();
                    using (StreamWriter sw = new StreamWriter(path + DateTime.Now.ToString("yyyy_MM_dd") + ".txt", true))
                    {
                        sw.Write(Message);
                    }
                }));
            }
            catch
            {

            }
        }
        private void ShowColor_Hander(object sender, EventArgs e)
        {
            if (fsi != null)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    if (sender != null)
                    {
                        DetectionResult Result = sender as DetectionResult;
                        switch (Result.Result)
                        {
                            case DataEnum.Result.Pass:
                                fsi.SetLabelChannelGreen(Result.Channel);
                                break;
                            case DataEnum.Result.Fail:
                                fsi.SetLabelChannelRed(Result.Channel);
                                break;
                            case DataEnum.Result.Skip:
                                break;
                            default:
                                break;
                        }
                    }
                }));
            }
        }
    }
}
