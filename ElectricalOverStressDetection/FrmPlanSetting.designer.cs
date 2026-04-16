namespace ElectricalOverStressData
{
    partial class FrmPlanSetting
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
            this.tableLayoutPanel_Setting = new System.Windows.Forms.TableLayoutPanel();
            this.label_CalibrationPlan = new System.Windows.Forms.Label();
            this.listBox_Plan = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel_PlanInfo = new System.Windows.Forms.TableLayoutPanel();
            this.label_PlanName = new System.Windows.Forms.Label();
            this.textBox_PlanName = new System.Windows.Forms.TextBox();
            this.button_Save = new System.Windows.Forms.Button();
            this.label_CommunicationType = new System.Windows.Forms.Label();
            this.textBox_CommunicationType = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel_CalibrationItem = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel_Setting.SuspendLayout();
            this.tableLayoutPanel_PlanInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel_Setting
            // 
            this.tableLayoutPanel_Setting.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel_Setting.ColumnCount = 2;
            this.tableLayoutPanel_Setting.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel_Setting.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 85F));
            this.tableLayoutPanel_Setting.Controls.Add(this.label_CalibrationPlan, 0, 0);
            this.tableLayoutPanel_Setting.Controls.Add(this.listBox_Plan, 0, 1);
            this.tableLayoutPanel_Setting.Controls.Add(this.tableLayoutPanel_PlanInfo, 1, 0);
            this.tableLayoutPanel_Setting.Controls.Add(this.tableLayoutPanel_CalibrationItem, 1, 1);
            this.tableLayoutPanel_Setting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_Setting.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_Setting.Name = "tableLayoutPanel_Setting";
            this.tableLayoutPanel_Setting.RowCount = 2;
            this.tableLayoutPanel_Setting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel_Setting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 95F));
            this.tableLayoutPanel_Setting.Size = new System.Drawing.Size(1283, 771);
            this.tableLayoutPanel_Setting.TabIndex = 0;
            // 
            // label_CalibrationPlan
            // 
            this.label_CalibrationPlan.AutoSize = true;
            this.label_CalibrationPlan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_CalibrationPlan.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_CalibrationPlan.Location = new System.Drawing.Point(4, 1);
            this.label_CalibrationPlan.Name = "label_CalibrationPlan";
            this.label_CalibrationPlan.Size = new System.Drawing.Size(186, 38);
            this.label_CalibrationPlan.TabIndex = 3;
            this.label_CalibrationPlan.Text = "校准计划";
            this.label_CalibrationPlan.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // listBox_Plan
            // 
            this.listBox_Plan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox_Plan.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBox_Plan.FormattingEnabled = true;
            this.listBox_Plan.ItemHeight = 21;
            this.listBox_Plan.Location = new System.Drawing.Point(1, 40);
            this.listBox_Plan.Margin = new System.Windows.Forms.Padding(0);
            this.listBox_Plan.MultiColumn = true;
            this.listBox_Plan.Name = "listBox_Plan";
            this.listBox_Plan.Size = new System.Drawing.Size(192, 730);
            this.listBox_Plan.TabIndex = 4;
            this.listBox_Plan.DoubleClick += new System.EventHandler(this.listBox_CalibrationPlan_DoubleClick);
            // 
            // tableLayoutPanel_PlanInfo
            // 
            this.tableLayoutPanel_PlanInfo.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel_PlanInfo.ColumnCount = 7;
            this.tableLayoutPanel_PlanInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tableLayoutPanel_PlanInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.63636F));
            this.tableLayoutPanel_PlanInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.18182F));
            this.tableLayoutPanel_PlanInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tableLayoutPanel_PlanInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.63636F));
            this.tableLayoutPanel_PlanInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.18182F));
            this.tableLayoutPanel_PlanInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.18182F));
            this.tableLayoutPanel_PlanInfo.Controls.Add(this.label_PlanName, 0, 0);
            this.tableLayoutPanel_PlanInfo.Controls.Add(this.textBox_PlanName, 1, 0);
            this.tableLayoutPanel_PlanInfo.Controls.Add(this.button_Save, 6, 0);
            this.tableLayoutPanel_PlanInfo.Controls.Add(this.label_CommunicationType, 3, 0);
            this.tableLayoutPanel_PlanInfo.Controls.Add(this.textBox_CommunicationType, 4, 0);
            this.tableLayoutPanel_PlanInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_PlanInfo.Location = new System.Drawing.Point(194, 1);
            this.tableLayoutPanel_PlanInfo.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel_PlanInfo.Name = "tableLayoutPanel_PlanInfo";
            this.tableLayoutPanel_PlanInfo.RowCount = 1;
            this.tableLayoutPanel_PlanInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_PlanInfo.Size = new System.Drawing.Size(1088, 38);
            this.tableLayoutPanel_PlanInfo.TabIndex = 2;
            // 
            // label_PlanName
            // 
            this.label_PlanName.AutoSize = true;
            this.label_PlanName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_PlanName.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_PlanName.Location = new System.Drawing.Point(4, 1);
            this.label_PlanName.Name = "label_PlanName";
            this.label_PlanName.Size = new System.Drawing.Size(92, 36);
            this.label_PlanName.TabIndex = 0;
            this.label_PlanName.Text = "计划名称";
            this.label_PlanName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox_PlanName
            // 
            this.textBox_PlanName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_PlanName.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_PlanName.Location = new System.Drawing.Point(103, 4);
            this.textBox_PlanName.Name = "textBox_PlanName";
            this.textBox_PlanName.Size = new System.Drawing.Size(141, 31);
            this.textBox_PlanName.TabIndex = 2;
            // 
            // button_Save
            // 
            this.button_Save.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Save.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Save.Location = new System.Drawing.Point(892, 4);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(192, 30);
            this.button_Save.TabIndex = 4;
            this.button_Save.Text = "保存";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // label_CommunicationType
            // 
            this.label_CommunicationType.AutoSize = true;
            this.label_CommunicationType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_CommunicationType.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_CommunicationType.Location = new System.Drawing.Point(448, 1);
            this.label_CommunicationType.Name = "label_CommunicationType";
            this.label_CommunicationType.Size = new System.Drawing.Size(92, 36);
            this.label_CommunicationType.TabIndex = 7;
            this.label_CommunicationType.Text = "通讯类型";
            this.label_CommunicationType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox_CommunicationType
            // 
            this.textBox_CommunicationType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_CommunicationType.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_CommunicationType.Location = new System.Drawing.Point(547, 4);
            this.textBox_CommunicationType.Name = "textBox_CommunicationType";
            this.textBox_CommunicationType.ReadOnly = true;
            this.textBox_CommunicationType.Size = new System.Drawing.Size(141, 31);
            this.textBox_CommunicationType.TabIndex = 8;
            // 
            // tableLayoutPanel_CalibrationItem
            // 
            this.tableLayoutPanel_CalibrationItem.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel_CalibrationItem.ColumnCount = 1;
            this.tableLayoutPanel_CalibrationItem.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_CalibrationItem.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_CalibrationItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_CalibrationItem.Location = new System.Drawing.Point(194, 40);
            this.tableLayoutPanel_CalibrationItem.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel_CalibrationItem.Name = "tableLayoutPanel_CalibrationItem";
            this.tableLayoutPanel_CalibrationItem.RowCount = 1;
            this.tableLayoutPanel_CalibrationItem.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_CalibrationItem.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 729F));
            this.tableLayoutPanel_CalibrationItem.Size = new System.Drawing.Size(1088, 730);
            this.tableLayoutPanel_CalibrationItem.TabIndex = 5;
            // 
            // FrmPlanSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1283, 771);
            this.Controls.Add(this.tableLayoutPanel_Setting);
            this.Name = "FrmPlanSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CalibrationSetting";
            this.Load += new System.EventHandler(this.FrmCalibrationSetting_Load);
            this.tableLayoutPanel_Setting.ResumeLayout(false);
            this.tableLayoutPanel_Setting.PerformLayout();
            this.tableLayoutPanel_PlanInfo.ResumeLayout(false);
            this.tableLayoutPanel_PlanInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Setting;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_PlanInfo;
        private System.Windows.Forms.Label label_PlanName;
        private System.Windows.Forms.TextBox textBox_PlanName;
        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.Label label_CalibrationPlan;
        private System.Windows.Forms.ListBox listBox_Plan;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_CalibrationItem;
        private System.Windows.Forms.Label label_CommunicationType;
        private System.Windows.Forms.TextBox textBox_CommunicationType;
    }
}