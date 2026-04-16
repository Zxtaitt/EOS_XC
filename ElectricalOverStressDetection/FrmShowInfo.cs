using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace ElectricalOverStressDetection
{
    public partial class FrmShowInfo : Form
    {
        private int ChannelNumber;
        private int BasicChannel = 16;
        private Dictionary<int, Label> LabelChannel;
        public FrmShowInfo(int Channel)
        {
            InitializeComponent();
            this.ChannelNumber = Channel;
        }

        private void FrmCalibrationInfo_Load(object sender, EventArgs e)
        {
            Thread LoadingThread = new Thread(new ThreadStart(LoadingInfo));
            LoadingThread.Start();
        }
        private void LoadingInfo()
        {
            LabelChannel = new Dictionary<int, Label>();
            this.Invoke(new MethodInvoker(() =>
            {
                tableLayoutPanel_LoadingInfo.BringToFront();
                tableLayoutPanel_ShowInfo.Controls.Clear();
            }));
            tableLayoutPanel_ShowInfo.ColumnCount = 1;
            tableLayoutPanel_ShowInfo.RowCount = 1;
            tableLayoutPanel_ShowInfo.RowStyles.Clear();
            tableLayoutPanel_ShowInfo.ColumnStyles.Clear();
            for (int row = 0; row < BasicChannel; row++)
            {
                tableLayoutPanel_ShowInfo.RowCount++;
                tableLayoutPanel_ShowInfo.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
                TableLayoutPanel tp = new TableLayoutPanel();
                this.Invoke(new MethodInvoker(() =>
                {
                    tableLayoutPanel_ShowInfo.Controls.Add(tp, 0, row);
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
                if (ChannelNumber < BasicChannel)
                    BasicChannel = ChannelNumber;
                int Columns = this.ChannelNumber / BasicChannel;
                for (int column = 0; column < Columns; column++)
                {
                    Thread.Sleep(10);
                    Label Label_Channel = new Label();
                    Label_Channel.Name = "Label_Channel" + (row + column * BasicChannel + 1).ToString();
                    Label_Channel.Text = "CH: " + (row + column * BasicChannel + 1).ToString().PadLeft(2, '0');
                    Label_Channel.Tag = row + column * BasicChannel + 1;
                    Label_Channel.TextAlign = ContentAlignment.MiddleCenter;
                    Label_Channel.Margin = new Padding(0, 0, 0, 0);
                    Label_Channel.BackColor = Color.White;
                    Label_Channel.Dock = DockStyle.Fill;
                    LabelChannel.Add(row + column * BasicChannel + 1, Label_Channel);
                    this.Invoke(new MethodInvoker(() =>
                    {
                        tp.ColumnCount++;
                        tp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50 / Columns));
                        tp.Controls.Add(Label_Channel, column, 0);
                    }));
                }               
            }
            this.Invoke(new MethodInvoker(() =>
            {
                tableLayoutPanel_ShowInfo.BringToFront();
                this.Refresh();
            }));

          
        }
        public void SetLabelChannelWhite()
        {
            for (int i = 0; i < this.ChannelNumber; i++)
            {
                LabelChannel[i + 1].BackColor = Color.White;
            }
        }
        public void SetLabelChannelGreen(int Channel)
        {
            LabelChannel[Channel + 1].BackColor = Color.Green;
        }
        public void SetLabelChannelRed(int Channel)
        {
            LabelChannel[Channel + 1].BackColor = Color.Red;
        }        
    }
}
