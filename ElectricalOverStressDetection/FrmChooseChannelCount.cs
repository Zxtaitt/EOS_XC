using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BoardDriver;

namespace ElectricalOverStressData
{
    public partial class FrmChooseChannelCount : Form
    {
        public int ChannelCount
        { get; set; }
        public BoardDriverEnum.BoardType BoardType
        { get; set; }
        public FrmChooseChannelCount()
        {
            InitializeComponent();
        }

        private void FrmChooseChannelCount_Load(object sender, EventArgs e)
        {
            comboBox_ChooseChannelCount.DataSource = Enum.GetNames(typeof(BoardDriverEnum.BoardType));  //combobox数据源datasource
        }

        private void button_Confirm_Click(object sender, EventArgs e)
        {
            this.ChannelCount = (int)Enum.Parse(typeof(BoardDriverEnum.BoardChannel), comboBox_ChooseChannelCount.Text);
            this.BoardType = (BoardDriverEnum.BoardType)Enum.Parse(typeof(BoardDriverEnum.BoardType), comboBox_ChooseChannelCount.Text);
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }
    }
}
