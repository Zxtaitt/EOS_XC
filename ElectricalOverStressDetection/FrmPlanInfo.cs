using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Xml.Serialization;
using ElectricalOverStressData;

namespace ElectricalOverStressData
{
    public partial class FrmPlanInfo : Form
    {
        public SerializableDictionary<int, List<ChannelItem>> CalibrationItem
        { get; set; }
        public int ChannelCount
        { get; set; }
        private int BasicChannel = 16;
        private Dictionary<int, Button> ButtonChannel;
        private ContextMenuStrip ChannelContextMenuStrip;
        private FrmChannelSetting fcs;
        private int RightClickChannel;
        private int ChooseChannel;
        private List<ChannelItem> ChannelItem;
        public FrmPlanInfo(int Count)
        {
            InitializeComponent();
            this.ChannelCount = Count;
            if (Count < BasicChannel)
                BasicChannel = Count;
            CalibrationItem = new SerializableDictionary<int, List<ChannelItem>>();
        }

        private void FrmCalibrationItem_Load(object sender, EventArgs e)
        {
            Initialize();
            Thread LoadingThread = new Thread(new ThreadStart(LoadingItem));
            LoadingThread.Start();
        }
        private void Button_Channel_Click(object sender, EventArgs e)
        {
            try
            {
                Button bt = sender as Button;
                ChooseChannel = Convert.ToInt32(bt.Tag);
                if (fcs != null)
                {
                    fcs.Close();
                    fcs = null;
                }
                fcs = new FrmChannelSetting();
                fcs.TopLevel = false;
                fcs.FormBorderStyle = FormBorderStyle.None;
                fcs.Dock = DockStyle.Fill;
                fcs.Parent = tableLayoutPanel_ChannelSetting;
                fcs.SaveCalibrationItem += SaveCalibrationItem_Hander;
                fcs.ShowChannel(ChooseChannel);
                if (CalibrationItem.Keys.Contains(ChooseChannel))
                {
                    fcs.CalibrationItem = CalibrationItem[ChooseChannel];
                }
                fcs.Show();
                if (ButtonChannel[ChooseChannel].BackColor == Color.White)
                {
                    SetButtonChannelRed(ChooseChannel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void ChannelContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            RightClickChannel = Convert.ToInt32((sender as ContextMenuStrip).SourceControl.Tag);
        }
        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (CalibrationItem.Keys.Contains(this.RightClickChannel))
                {
                    ChannelItem = CalibrationItem[RightClickChannel];
                }
                else
                {
                    ChannelItem = null;
                    MessageBox.Show("通道" + RightClickChannel.ToString() + "未配置条件!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (ChannelItem != null)
                {
                    PasteToChannel(RightClickChannel);
                }
                else
                {
                    MessageBox.Show("请先复制条件!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void BatchPasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (ChannelItem == null)
                {
                    MessageBox.Show("请先复制条件!");
                    return;
                }
                string input;
                if (!ShowChannelRangeInput(out input))
                {
                    return;
                }
                List<int> targets = ParseChannelNumbers(input);
                if (targets.Count == 0)
                {
                    MessageBox.Show("未输入有效的通道!");
                    return;
                }
                foreach (int channel in targets)
                {
                    PasteToChannel(channel);
                }
                MessageBox.Show("已批量粘贴到 " + targets.Count + " 个通道!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void PasteToChannel(int channel)
        {
            if (CalibrationItem.Keys.Contains(channel))
            {
                CalibrationItem.Remove(channel);
            }
            CalibrationItem.Add(channel, CloneChannelItems(ChannelItem));
            SetButtonChannelYellow(channel);
        }
        private static List<ChannelItem> CloneChannelItems(List<ChannelItem> source)
        {
            if (source == null)
            {
                return null;
            }
            XmlSerializer serializer = new XmlSerializer(typeof(List<ChannelItem>));
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.Serialize(ms, source);
                ms.Position = 0;
                return (List<ChannelItem>)serializer.Deserialize(ms);
            }
        }
        private List<int> ParseChannelNumbers(string input)
        {
            List<int> result = new List<int>();
            if (string.IsNullOrWhiteSpace(input))
            {
                return result;
            }
            string normalized = input.Replace("，", ",").Replace("－", "-").Replace(" ", "").Trim();
            if (normalized == "全部" || normalized.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                for (int i = 1; i <= ChannelCount; i++)
                {
                    result.Add(i);
                }
                return result;
            }
            string[] tokens = normalized.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string token in tokens)
            {
                if (token.Contains("-"))
                {
                    string[] range = token.Split('-');
                    if (range.Length != 2)
                    {
                        throw new Exception("通道格式错误: " + token);
                    }
                    int start = ParseChannelNumber(range[0]);
                    int end = ParseChannelNumber(range[1]);
                    if (start > end)
                    {
                        int temp = start;
                        start = end;
                        end = temp;
                    }
                    for (int i = start; i <= end; i++)
                    {
                        if (!result.Contains(i))
                        {
                            result.Add(i);
                        }
                    }
                }
                else
                {
                    int channel = ParseChannelNumber(token);
                    if (!result.Contains(channel))
                    {
                        result.Add(channel);
                    }
                }
            }
            return result;
        }
        private int ParseChannelNumber(string text)
        {
            int channel;
            if (!int.TryParse(text, out channel))
            {
                throw new Exception("无效的通道: " + text);
            }
            if (channel < 1 || channel > ChannelCount)
            {
                throw new Exception("通道超出范围(1~" + ChannelCount + "): " + channel);
            }
            return channel;
        }
        private bool ShowChannelRangeInput(out string result)
        {
            result = "";
            using (Form inputForm = new Form())
            {
                inputForm.Text = "批量粘贴";
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.StartPosition = FormStartPosition.CenterParent;
                inputForm.MinimizeBox = false;
                inputForm.MaximizeBox = false;
                inputForm.ShowIcon = false;
                inputForm.ShowInTaskbar = false;
                inputForm.ClientSize = new Size(330, 140);

                Label label = new Label();
                label.AutoSize = false;
                label.SetBounds(12, 12, 306, 50);
                label.Text = "复制源: 通道" + RightClickChannel + "  (可粘贴范围 1~" + ChannelCount + ")\r\n"
                    + "粘贴到通道, 例: 1-16  或  2,4,6  或  全部";

                TextBox textBox = new TextBox();
                textBox.SetBounds(12, 66, 306, 25);
                textBox.Text = "1-" + ChannelCount;

                Button okButton = new Button();
                okButton.Text = "确定";
                okButton.DialogResult = DialogResult.OK;
                okButton.SetBounds(152, 100, 78, 30);

                Button cancelButton = new Button();
                cancelButton.Text = "取消";
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.SetBounds(240, 100, 78, 30);

                inputForm.Controls.Add(label);
                inputForm.Controls.Add(textBox);
                inputForm.Controls.Add(okButton);
                inputForm.Controls.Add(cancelButton);
                inputForm.AcceptButton = okButton;
                inputForm.CancelButton = cancelButton;

                if (inputForm.ShowDialog(this) == DialogResult.OK)
                {
                    result = textBox.Text;
                    return true;
                }
                return false;
            }
        }
        private void LoadingItem()
        {
            this.Invoke(new MethodInvoker(() =>
            {
                tableLayoutPanel_Loading.BringToFront();
                tableLayoutPanel_CalibrationChannelSetting.Controls.Clear();
            }));
            ButtonChannel = new Dictionary<int, Button>();
            tableLayoutPanel_CalibrationChannelSetting.ColumnCount = 1;
            tableLayoutPanel_CalibrationChannelSetting.RowCount = 1;
            tableLayoutPanel_CalibrationChannelSetting.RowStyles.Clear();
            tableLayoutPanel_CalibrationChannelSetting.ColumnStyles.Clear();
            for (int row = 0; row < BasicChannel; row++)
            {
                tableLayoutPanel_CalibrationChannelSetting.RowCount++;
                tableLayoutPanel_CalibrationChannelSetting.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
                TableLayoutPanel tp = new TableLayoutPanel();
                this.Invoke(new MethodInvoker(() =>
                {
                    tableLayoutPanel_CalibrationChannelSetting.Controls.Add(tp, 0, row);
                    tp.Dock = DockStyle.Fill;
                    tp.Margin = new Padding(0, 0, 0, 0);
                    tp.Padding = new Padding(0, 0, 0, 0);
                }));
                tp.BackColor = Color.Silver;
                tp.CellBorderStyle = TableLayoutPanelCellBorderStyle.OutsetDouble;
                tp.ColumnStyles.Clear();
                tp.RowStyles.Clear();
                tp.RowCount++;
                tp.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
                int Columns = this.ChannelCount / BasicChannel;
                for (int column = 0; column < Columns; column++)
                {
                    Thread.Sleep(10);
                    Button Button_Channel = new Button();
                    Button_Channel.Name = "Button_Channel" + (row + column * BasicChannel + 1).ToString();
                    Button_Channel.Text = "CH: " + (row + column * BasicChannel + 1).ToString().PadLeft(2, '0');   //?
                    Button_Channel.Tag = row + column * BasicChannel + 1;
                    Button_Channel.TextAlign = ContentAlignment.MiddleCenter;
                    Button_Channel.Margin = new Padding(0, 0, 0, 0);
                    Button_Channel.FlatStyle = FlatStyle.Popup;
                    Button_Channel.BackColor = Color.White;
                    Button_Channel.Click += Button_Channel_Click;
                    Button_Channel.Dock = DockStyle.Fill;
                    Button_Channel.ContextMenuStrip = ChannelContextMenuStrip;
                    ButtonChannel.Add(row + column * BasicChannel + 1, Button_Channel);
                    this.Invoke(new MethodInvoker(() =>
                    {
                        tp.ColumnCount++;
                        tp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50 / Columns));
                        tp.Controls.Add(Button_Channel, column, 0);
                    }));
                }
            }
            this.Invoke(new MethodInvoker(() =>
            {
                tableLayoutPanel_CalibrationItem.BringToFront();
                this.Refresh();
            }));
        }
        private void Initialize()
        {
            ChannelContextMenuStrip = new ContextMenuStrip();
            ChannelContextMenuStrip.Name = "ChannelContextMenuStrip";
            ChannelContextMenuStrip.Opening += new CancelEventHandler(ChannelContextMenuStrip_Opening);
            ToolStripMenuItem CopyToolStripMenuItem = new ToolStripMenuItem();
            CopyToolStripMenuItem.Name = "CopyToolStripMenuItem";
            CopyToolStripMenuItem.Text = "复制";
            CopyToolStripMenuItem.Click += new EventHandler(CopyToolStripMenuItem_Click);
            ToolStripMenuItem PasteToolStripMenuItem = new ToolStripMenuItem();
            PasteToolStripMenuItem.Name = "PasteToolStripMenuItem";
            PasteToolStripMenuItem.Text = "粘贴";
            PasteToolStripMenuItem.Click += new EventHandler(PasteToolStripMenuItem_Click);
            ToolStripMenuItem BatchPasteToolStripMenuItem = new ToolStripMenuItem();
            BatchPasteToolStripMenuItem.Name = "BatchPasteToolStripMenuItem";
            BatchPasteToolStripMenuItem.Text = "批量粘贴";
            BatchPasteToolStripMenuItem.Click += new EventHandler(BatchPasteToolStripMenuItem_Click);
            ChannelContextMenuStrip.Items.AddRange(new ToolStripItem[] { CopyToolStripMenuItem, PasteToolStripMenuItem, BatchPasteToolStripMenuItem });
        }
        private void SaveCalibrationItem_Hander(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                List<ChannelItem> cci = sender as List<ChannelItem>;
                if (cci != null)
                {
                    if (CalibrationItem.Keys.Contains(ChooseChannel))
                    {
                        CalibrationItem.Remove(ChooseChannel);
                    }
                    CalibrationItem.Add(ChooseChannel, cci);
                    SetButtonChannelGreen(ChooseChannel);
                }
            }));
        }
        public void SetButtonChannelWhite()
        {
            for (int i = 0; i < this.ChannelCount; i++)
            {
                ButtonChannel[i + 1].BackColor = Color.White;
            }
        }
        public void SetButtonChannelYellow(int Channel)
        {
            ButtonChannel[Channel].BackColor = Color.Yellow;
        }
        public void SetButtonChannelGreen(int Channel)
        {
            ButtonChannel[Channel].BackColor = Color.Green;
        }
        public void SetButtonChannelRed(int Channel)
        {
            ButtonChannel[Channel].BackColor = Color.Red;
        }
    }
}
