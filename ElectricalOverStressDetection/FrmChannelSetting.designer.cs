namespace ElectricalOverStressData
{
    partial class FrmChannelSetting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel_ChannelSetting = new System.Windows.Forms.TableLayoutPanel();
            this.label_Channel = new System.Windows.Forms.Label();
            this.textBox_Channel = new System.Windows.Forms.TextBox();
            this.button_Save = new System.Windows.Forms.Button();
            this.tableLayoutPanel_Setting = new System.Windows.Forms.TableLayoutPanel();
            this.tabControl_ChannelSetting = new System.Windows.Forms.TabControl();
            this.tableLayoutPanel_ChannelSetting.SuspendLayout();
            this.tableLayoutPanel_Setting.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel_ChannelSetting
            // 
            this.tableLayoutPanel_ChannelSetting.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel_ChannelSetting.ColumnCount = 4;
            this.tableLayoutPanel_ChannelSetting.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel_ChannelSetting.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel_ChannelSetting.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.tableLayoutPanel_ChannelSetting.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel_ChannelSetting.Controls.Add(this.label_Channel, 0, 0);
            this.tableLayoutPanel_ChannelSetting.Controls.Add(this.textBox_Channel, 1, 0);
            this.tableLayoutPanel_ChannelSetting.Controls.Add(this.button_Save, 3, 0);
            this.tableLayoutPanel_ChannelSetting.Controls.Add(this.tableLayoutPanel_Setting, 0, 1);
            this.tableLayoutPanel_ChannelSetting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_ChannelSetting.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_ChannelSetting.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel_ChannelSetting.Name = "tableLayoutPanel_ChannelSetting";
            this.tableLayoutPanel_ChannelSetting.RowCount = 2;
            this.tableLayoutPanel_ChannelSetting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel_ChannelSetting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 95F));
            this.tableLayoutPanel_ChannelSetting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_ChannelSetting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_ChannelSetting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_ChannelSetting.Size = new System.Drawing.Size(961, 737);
            this.tableLayoutPanel_ChannelSetting.TabIndex = 0;
            // 
            // label_Channel
            // 
            this.label_Channel.AutoSize = true;
            this.label_Channel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_Channel.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_Channel.Location = new System.Drawing.Point(4, 1);
            this.label_Channel.Name = "label_Channel";
            this.label_Channel.Size = new System.Drawing.Size(89, 36);
            this.label_Channel.TabIndex = 1;
            this.label_Channel.Text = "通道";
            this.label_Channel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox_Channel
            // 
            this.textBox_Channel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Channel.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_Channel.Location = new System.Drawing.Point(100, 4);
            this.textBox_Channel.Name = "textBox_Channel";
            this.textBox_Channel.ReadOnly = true;
            this.textBox_Channel.Size = new System.Drawing.Size(89, 31);
            this.textBox_Channel.TabIndex = 2;
            this.textBox_Channel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button_Save
            // 
            this.button_Save.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Save.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Save.Location = new System.Drawing.Point(818, 4);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(139, 30);
            this.button_Save.TabIndex = 3;
            this.button_Save.Text = "保存";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // tableLayoutPanel_Setting
            // 
            this.tableLayoutPanel_Setting.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel_Setting.ColumnCount = 1;
            this.tableLayoutPanel_ChannelSetting.SetColumnSpan(this.tableLayoutPanel_Setting, 4);
            this.tableLayoutPanel_Setting.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_Setting.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_Setting.Controls.Add(this.tabControl_ChannelSetting, 0, 0);
            this.tableLayoutPanel_Setting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_Setting.Location = new System.Drawing.Point(1, 38);
            this.tableLayoutPanel_Setting.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel_Setting.Name = "tableLayoutPanel_Setting";
            this.tableLayoutPanel_Setting.RowCount = 1;
            this.tableLayoutPanel_Setting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_Setting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 697F));
            this.tableLayoutPanel_Setting.Size = new System.Drawing.Size(959, 698);
            this.tableLayoutPanel_Setting.TabIndex = 4;
            // 
            // tabControl_ChannelSetting
            // 
            this.tabControl_ChannelSetting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_ChannelSetting.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabControl_ChannelSetting.Location = new System.Drawing.Point(1, 1);
            this.tabControl_ChannelSetting.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl_ChannelSetting.Name = "tabControl_ChannelSetting";
            this.tabControl_ChannelSetting.SelectedIndex = 0;
            this.tabControl_ChannelSetting.Size = new System.Drawing.Size(957, 696);
            this.tabControl_ChannelSetting.TabIndex = 0;
            this.tabControl_ChannelSetting.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tabControl_ChannelSetting_MouseDown);
            this.tabControl_ChannelSetting.MouseLeave += new System.EventHandler(this.tabControl_ChannelSetting_MouseLeave);
            // 
            // FrmChannelSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(961, 737);
            this.Controls.Add(this.tableLayoutPanel_ChannelSetting);
            this.Name = "FrmChannelSetting";
            this.Text = "FrmChannelSetting";
            this.Load += new System.EventHandler(this.FrmChannelSetting_Load);
            this.tableLayoutPanel_ChannelSetting.ResumeLayout(false);
            this.tableLayoutPanel_ChannelSetting.PerformLayout();
            this.tableLayoutPanel_Setting.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_ChannelSetting;
        private System.Windows.Forms.TabControl tabControl_ChannelSetting;
        private System.Windows.Forms.Label label_Channel;
        private System.Windows.Forms.TextBox textBox_Channel;
        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Setting;
    }
}