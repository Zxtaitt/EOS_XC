using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using ElectricalOverStressData;

namespace ElectricalOverStressData
{
    public partial class FrmChannelSetting : Form
    {
        private Dictionary<string, bool> PropertyGridClick;
        private int Channel;
        private ContextMenuStrip CalibrationContextMenuStrip;
        private Dictionary<int, ChannelItem> ChannelItem = new Dictionary<int, ChannelItem>();
        public List<ChannelItem> CalibrationItem;
        public event EventHandler SaveCalibrationItem;
        public bool ClickSave;
        public FrmChannelSetting()
        {
            InitializeComponent();
        }

        private void FrmChannelSetting_Load(object sender, EventArgs e)
        {
            Initialize();
            PropertyGridClick = new Dictionary<string, bool>();
            if (CalibrationItem == null)
            {
                CalibrationItem = new List<ChannelItem>();
                AddTabPage();
            }
            else
            {
                ShowItemInfo();
            }
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            CalibrationItem = ChannelItem.Values.ToList();
            if (SaveCalibrationItem != null)
            {
                SaveCalibrationItem.Invoke(CalibrationItem, null);
            }
        }
        private void tabControl_ChannelSetting_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < tabControl_ChannelSetting.TabPages.Count; i++)
                {
                    TabPage tp = tabControl_ChannelSetting.TabPages[i];
                    if (tabControl_ChannelSetting.GetTabRect(i).Contains(new Point(e.X, e.Y)))
                    {
                        tabControl_ChannelSetting.SelectedTab = tp;
                        break;
                    }
                }
                this.tabControl_ChannelSetting.ContextMenuStrip = this.CalibrationContextMenuStrip;
            }

        }
        private void tabControl_ChannelSetting_MouseLeave(object sender, EventArgs e)
        {
            this.tabControl_ChannelSetting.ContextMenuStrip = null;
        }
        private void AddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                AddTabPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void RemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                RemoveTabPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void PropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            try
            {
                if (e.ChangedItem.PropertyDescriptor.Name != null)
                {
                    if (PropertyGridClick[e.ChangedItem.PropertyDescriptor.Name])
                    {
                        PropertyGrid pg = s as PropertyGrid;
                        SetPropertyReadOnly(pg.SelectedObject, e.ChangedItem.PropertyDescriptor.Name, true);
                    }
                }
            }
            catch
            {

            }
        }
        private void PropertyGrid_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            try
            {
                if (e.NewSelection.PropertyDescriptor.Name != null)
                {
                    if (!PropertyGridClick.Keys.Contains(e.NewSelection.PropertyDescriptor.Name))
                    {
                        PropertyGridClick.Add(e.NewSelection.PropertyDescriptor.Name, e.NewSelection.PropertyDescriptor.IsReadOnly);
                    }
                    if (PropertyGridClick[e.NewSelection.PropertyDescriptor.Name])
                    {
                        PropertyGrid pg = sender as PropertyGrid;
                        SetPropertyReadOnly(pg.SelectedObject, e.NewSelection.PropertyDescriptor.Name, false);
                    }
                }
            }
            catch
            {

            }
        }
        public void ShowChannel(int ChooseChannel)
        {
            Channel = ChooseChannel;
            textBox_Channel.Text = Channel.ToString();
        }
        private void Initialize()
        {
            CalibrationContextMenuStrip = new ContextMenuStrip();
            CalibrationContextMenuStrip.Name = "CalibrationContextMenuStrip";
            ToolStripMenuItem AddToolStripMenuItem = new ToolStripMenuItem();
            AddToolStripMenuItem.Name = "AddToolStripMenuItem";
            AddToolStripMenuItem.Text = "增加";
            AddToolStripMenuItem.Click += new EventHandler(AddToolStripMenuItem_Click);
            ToolStripMenuItem RemoveToolStripMenuItem = new ToolStripMenuItem();
            RemoveToolStripMenuItem.Name = "RemoveToolStripMenuItem";
            RemoveToolStripMenuItem.Text = "删除";
            RemoveToolStripMenuItem.Click += new EventHandler(RemoveToolStripMenuItem_Click);
            CalibrationContextMenuStrip.Items.AddRange(new ToolStripItem[] { AddToolStripMenuItem, RemoveToolStripMenuItem });
        }
        private void AddTabPage()
        {
            int Count = tabControl_ChannelSetting.TabPages.Count;
            if (Count != 0)
            {
                Count = Convert.ToInt32(tabControl_ChannelSetting.TabPages[Count - 1].Tag);
            }
            string page = "tabPage_";
            string name = "Calibration" + (Count + 1).ToString();
            TabPage tp = new TabPage(page + name);
            tp.Name = page + name;
            tp.Text = name;
            tp.Tag = Count + 1;
            tp.BackColor = Color.White;
            PropertyGrid pg = new PropertyGrid();
            pg.PropertyValueChanged += PropertyGrid_PropertyValueChanged;
            pg.SelectedGridItemChanged += PropertyGrid_SelectedGridItemChanged;
            pg.Name = "PropertyGrid_" + name;
            pg.Dock = DockStyle.Fill;
            pg.ToolbarVisible = false;
            pg.Parent = tp;
            ChannelItem cis = new ChannelItem();
            pg.SelectedObject = cis;
            ChannelItem.Add(Count + 1, cis);
            tabControl_ChannelSetting.TabPages.Add(tp);
        }


        private void RemoveTabPage()
        {
            int Count = tabControl_ChannelSetting.SelectedIndex;
            //if (tabControl_ChannelSetting.TabPages.Count > 1) 
            {
                ChannelItem.Remove(Count + 1);
                tabControl_ChannelSetting.TabPages.Remove(tabControl_ChannelSetting.SelectedTab);
                this.Refresh();
            }
            //else
            //{
            //    ChannelItem.Remove(ChannelItem[Count]);
            //    tabControl_ChannelSetting.TabPages[Count].Controls.Clear();
            //}
        }
        private void ShowItemInfo()
        {
            for (int i = 0; i < CalibrationItem.Count; i++)
            {
                string page = "tabPage_";
                string name = "Calibration" + (i + 1).ToString();
                TabPage tp = new TabPage(page + name);
                tp.Name = page + name;
                tp.Text = name;
                tp.Tag = i + 1;
                tp.BackColor = Color.White;
                PropertyGrid pg = new PropertyGrid();
                pg.PropertyValueChanged += PropertyGrid_PropertyValueChanged;
                pg.SelectedGridItemChanged += PropertyGrid_SelectedGridItemChanged;
                pg.Name = "PropertyGrid_" + name;
                pg.Dock = DockStyle.Fill;
                pg.ToolbarVisible = false;
                pg.Parent = tp;
                pg.SelectedObject = CalibrationItem[i];
                //SetPropertyReadOnly(pg.SelectedObject, "CalibrationPoint", true);
                pg.Refresh();
                ChannelItem.Add(i + 1, CalibrationItem[i]);
                tabControl_ChannelSetting.TabPages.Add(tp);
            }
        }


        private void SetPropertyReadOnly(object obj, string propertyName, bool readOnly)
        {
            Type type = typeof(ReadOnlyAttribute);
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(obj);
            FieldInfo fld = type.GetField("isReadOnly", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance);
            AttributeCollection attrs = props[propertyName].Attributes;
            fld.SetValue(attrs[type], readOnly);
        }
    }
}
