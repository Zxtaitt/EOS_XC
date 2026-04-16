using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using ElectricalOverStressData;

namespace ElectricalOverStressDetection
{
    public partial class FrmBindInfo : Form
    {
        public BindInfo BindingInfo
        { get; set; }
        private int ChannelCount;       
        public FrmBindInfo(int Count)
        {
            InitializeComponent();
            ChannelCount = Count;
        }

        private void FrmBinding_Load(object sender, EventArgs e)
        {          
            Global.Channel = GetChannelList();
            BindingInfo = new BindInfo();
            BindingInfo.LayerName = DataEnum.Layer.L1;
            BindingInfo.LocationName = DataEnum.Location._1;
            BindingInfo.StartChannel = Global.Channel[0];
            BindingInfo.EndChannel = Global.Channel[Global.Channel.Count - 1];
            propertyGrid_Binding.SelectedObject = BindingInfo;
        }

        private void button_Confirm_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }
        private List<string> GetChannelList()
        {
            List<string> Result = new List<string>();
            try
            {
                for(int i = 0; i < ChannelCount; i++)
                {
                    string result = "CH-" + (i + 1).ToString().PadLeft(2, '0');
                    Result.Add(result);
                }
                return Result;
            }
            catch
            {
                Result.Add("CH-01");
                return Result;
            }
        }
    }
}
