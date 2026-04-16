using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using ElectricalOverStressData;
using BoardDriver;

namespace ElectricalOverStressData
{
    public partial class FrmPlanSetting : Form
    {
        private DataPlan Plan;
        private ContextMenuStrip PlanContextMenuStrip;
        private FrmPlanInfo fpi;
        public FrmPlanSetting()
        {
            InitializeComponent();
        }

        private void FrmCalibrationSetting_Load(object sender, EventArgs e)  //加载界面
        {
            Initialize();
            listBox_Plan.ContextMenuStrip = PlanContextMenuStrip;
            RefreshCalibrationPlan();
            textBox_CommunicationType.Text = "";
        }
        private void listBox_CalibrationPlan_DoubleClick(object sender, EventArgs e)  //双击listbox事件
        {
            if (listBox_Plan.SelectedIndex > -1)
            {
                StreamReader sr = File.OpenText(Global.PlanPath + listBox_Plan.Text + ".xml");
                Plan = DataTool.GetClassDeserializationXML(sr.ReadToEnd(), typeof(DataPlan)) as DataPlan;
                sr.Close();
                CreateFrmCalibrationItem();
                fpi.CalibrationItem = Plan.ChannelItem;
            }
        }
        private void NewPlanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmChooseChannelCount fccc = new FrmChooseChannelCount();
            if (fccc.ShowDialog() != DialogResult.Yes)
            {
                return;
            }
            Plan = new DataPlan();
            Plan.ChannelCount = fccc.ChannelCount;
            Plan.PlanName = "New Plan";
            Plan.BoardType = fccc.BoardType;
            CreateFrmCalibrationItem();
        }
        private void DeletePlanToolStripMenuItem_Click(object sender, EventArgs e) //先把xml文件删了，再更新界面
        {
            if (listBox_Plan.SelectedIndex > -1)
            {
                DialogResult MessageBoxResult = MessageBox.Show("确认删除计划:" + listBox_Plan.Text + " ?", "提示", MessageBoxButtons.OKCancel);
                if (MessageBoxResult == DialogResult.OK)
                {
                    string PathName = Global.PlanPath + listBox_Plan.Text + ".xml";
                    File.Delete(PathName);
                    RefreshCalibrationPlan();
                }
            }
            else
            {
                MessageBox.Show("请选择计划");
            }
        }      

        private void button_Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (Plan != null)
                {
                    Plan.PlanName = textBox_PlanName.Text;
                    Plan.BoardType = (BoardDriverEnum.BoardType)Enum.Parse(typeof(BoardDriverEnum.BoardType), textBox_CommunicationType.Text);
                    Plan.ChannelItem = fpi.CalibrationItem;
                    bool Save = true;
                    string PathName = Global.PlanPath + Plan.PlanName + ".xml";
                    if (File.Exists(PathName))
                    {
                        DialogResult dr = MessageBox.Show("计划已存在,是否覆盖?", "判定", MessageBoxButtons.OKCancel);
                        if (dr == DialogResult.Cancel)
                        {
                            Save = false;
                        }
                    }
                    if (Save)
                    {
                        FileInfo fi = new FileInfo(PathName);
                        StreamWriter wr = fi.CreateText();
                        XmlDocument xml = new XmlDocument();
                        UTF8Encoding encoding = new UTF8Encoding();
                        wr.Write(encoding.GetString(DataTool.GetXMLSerializationClass(typeof(DataPlan), Plan)));
                        wr.Close();
                        MessageBox.Show("保存计划:" + Plan.PlanName + "完成");
                    }
                    RefreshCalibrationPlan();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存计划失败:" + ex.ToString());
            }
        }

        private void Initialize()
        {
            PlanContextMenuStrip = new ContextMenuStrip();
            PlanContextMenuStrip.Name = "PlanContextMenuStrip";
            ToolStripMenuItem NewPlanToolStripMenuItem = new ToolStripMenuItem();
            NewPlanToolStripMenuItem.Name = "NewPlanToolStripMenuItem";
            NewPlanToolStripMenuItem.Text = "新建";
            NewPlanToolStripMenuItem.Click += new EventHandler(NewPlanToolStripMenuItem_Click);
            ToolStripMenuItem DeletePlanToolStripMenuItem = new ToolStripMenuItem();
            DeletePlanToolStripMenuItem.Name = "DeletePlanToolStripMenuItem";
            DeletePlanToolStripMenuItem.Text = "删除";
            DeletePlanToolStripMenuItem.Click += new EventHandler(DeletePlanToolStripMenuItem_Click);
            PlanContextMenuStrip.Items.AddRange(new ToolStripItem[] { NewPlanToolStripMenuItem, DeletePlanToolStripMenuItem});
        }
        private void RefreshCalibrationPlan()
        {
            listBox_Plan.Items.Clear();
            listBox_Plan.Items.AddRange(DataTool.GetPlanNameList());
        }
        private void CreateFrmCalibrationItem()
        {
            if (fpi != null)
            {
                fpi.Close();
            }
            fpi = new FrmPlanInfo(Plan.ChannelCount);
            fpi.TopLevel = false; //是否将窗口显示为顶层窗口
            fpi.FormBorderStyle = FormBorderStyle.None;  //设置窗体的边框样式
            fpi.Dock = DockStyle.Fill;  //控件的各个边缘分别停靠在其包含控件的各个边缘，并且适当调整大小。
            fpi.Parent = tableLayoutPanel_CalibrationItem;  
            fpi.Show();
            textBox_PlanName.Text = Plan.PlanName;
            textBox_CommunicationType.Text = Plan.BoardType.ToString();
        }

    }
}
