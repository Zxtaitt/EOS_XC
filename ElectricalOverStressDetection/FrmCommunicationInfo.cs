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
using System.Xml;
using ElectricalOverStressData;

namespace ElectricalOverStressDetection
{
    public partial class FrmCommunicationInfo : Form
    {
        private CommunicationInfo Communication;
        public FrmCommunicationInfo()
        {
            InitializeComponent();
        }

        private void FrmCommunication_Load(object sender, EventArgs e)
        {
            try
            {
                Global.PlanName = GetPlanList();
                if (File.Exists(Global.CommunicationPathName))
                {
                    StreamReader sr = File.OpenText(Global.CommunicationPathName);
                    Communication = DataTool.GetClassDeserializationXML(sr.ReadToEnd(), typeof(CommunicationInfo)) as CommunicationInfo;
                    sr.Close();
                    propertyGrid_OtherItem.SelectedObject = Communication.OtherItem;
                    dataGridView_BoardItem.DataSource = Communication.BoardItem;
                }
                if (Communication == null)
                {
                    CreateCommunicationInfo();
                }
            }
            catch
            {
                CreateCommunicationInfo();
            }
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            try
            {
                if(string.IsNullOrEmpty(Communication.OtherItem.BoardSerialPort) || string.IsNullOrEmpty(Communication.OtherItem.PlanName)|| string.IsNullOrEmpty(Communication.OtherItem.OscilloscopeAaddress))
                {
                    MessageBox.Show("保存通讯配置失败:" + "请将参数填写完整!");
                    return;
                }
                FileInfo fi = new FileInfo(Global.CommunicationPathName);
                StreamWriter wr = fi.CreateText();
                XmlDocument xml = new XmlDocument();
                UTF8Encoding encoding = new UTF8Encoding();
                wr.Write(encoding.GetString(DataTool.GetXMLSerializationClass(typeof(CommunicationInfo), Communication)));
                wr.Close();          
                MessageBox.Show("保存通讯配置成功");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存通讯配置失败" + "Error:" + ex.Message);
            }
        }

        private void propertyGrid_HardwareSetting_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.PropertyDescriptor.Name == "BoardNumber")
            {
                if (!(Communication.OtherItem.BoardNumber < 1 || Communication.OtherItem.BoardNumber > 4))
                {
                    dataGridView_BoardItem.DataSource = null;
                    Communication.BoardItem.Clear();
                    for (int i = 0; i < Enum.GetNames(typeof(DataEnum.Location)).Length; i++)
                    {
                        for (int j = 0; j < Communication.OtherItem.BoardNumber; j++)
                        {
                            CommunicationBoardItem PBI = new CommunicationBoardItem();
                            PBI.BoardLocation = (DataEnum.Location)Enum.ToObject(typeof(DataEnum.Location), (i + 1));
                            PBI.BoardIndex = j + 1;
                            if (Communication.OtherItem.BoardNumber == 1)
                            {
                                PBI.BoardAddress = i + Communication.OtherItem.BoardNumber;
                            }
                            else
                            {
                                PBI.BoardAddress = i * Communication.OtherItem.BoardNumber + j;
                            }
                            Communication.BoardItem.Add(PBI);
                        }
                    }
                    dataGridView_BoardItem.DataSource = Communication.BoardItem;
                }
                else
                {
                    MessageBox.Show("Errror: BoardNumber Range is One to Four");
                }

            }
        }
        private void CreateCommunicationInfo()
        {
            Communication = new CommunicationInfo();
            for (int i = 0; i < Enum.GetNames(typeof(DataEnum.Location)).Length; i++)
            {
                CommunicationBoardItem PBI = new CommunicationBoardItem();
                PBI.BoardLocation = (DataEnum.Location)Enum.ToObject(typeof(DataEnum.Location), (i + 1));
                PBI.BoardAddress = i + 1;
                Communication.BoardItem.Add(PBI);
            }
            propertyGrid_OtherItem.SelectedObject = Communication.OtherItem;
            dataGridView_BoardItem.DataSource = Communication.BoardItem;
        }
        private List<string> GetPlanList()
        {
            List<string> Result = new List<string>();
            try
            {
                //Global.PlanPath = @"C:\\EOSPlatformPlan\\" + "\\EOSPlan\\";
                //string Path = Application.StartupPath + "\\Plan\\";
                if (!Directory.Exists(Global.PlanPath))
                {
                    Directory.CreateDirectory(Global.PlanPath);
                }
                DirectoryInfo thefolder = new DirectoryInfo(Global.PlanPath);
                FileInfo[] files = thefolder.GetFiles();
                foreach (FileInfo fi in files)
                {
                    Result.Add(fi.Name.Replace(".xml", ""));
                }
                return Result;
            }
            catch
            {
                return Result;
            }
        }
    }
}
