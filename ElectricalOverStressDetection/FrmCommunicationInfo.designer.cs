namespace ElectricalOverStressDetection
{
    partial class FrmCommunicationInfo
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
            this.button_Save = new System.Windows.Forms.Button();
            this.tableLayoutPanel_CommunicationInfo = new System.Windows.Forms.TableLayoutPanel();
            this.propertyGrid_OtherItem = new System.Windows.Forms.PropertyGrid();
            this.dataGridView_BoardItem = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel_CommunicationInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_BoardItem)).BeginInit();
            this.SuspendLayout();
            // 
            // button_Save
            // 
            this.button_Save.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Save.Location = new System.Drawing.Point(4, 627);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(1076, 64);
            this.button_Save.TabIndex = 4;
            this.button_Save.Text = "保存";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // tableLayoutPanel_CommunicationInfo
            // 
            this.tableLayoutPanel_CommunicationInfo.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel_CommunicationInfo.ColumnCount = 1;
            this.tableLayoutPanel_CommunicationInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_CommunicationInfo.Controls.Add(this.button_Save, 0, 2);
            this.tableLayoutPanel_CommunicationInfo.Controls.Add(this.propertyGrid_OtherItem, 0, 0);
            this.tableLayoutPanel_CommunicationInfo.Controls.Add(this.dataGridView_BoardItem, 0, 1);
            this.tableLayoutPanel_CommunicationInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_CommunicationInfo.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tableLayoutPanel_CommunicationInfo.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_CommunicationInfo.Name = "tableLayoutPanel_CommunicationInfo";
            this.tableLayoutPanel_CommunicationInfo.RowCount = 2;
            this.tableLayoutPanel_CommunicationInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel_CommunicationInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_CommunicationInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel_CommunicationInfo.Size = new System.Drawing.Size(1084, 695);
            this.tableLayoutPanel_CommunicationInfo.TabIndex = 0;
            // 
            // propertyGrid_OtherItem
            // 
            this.propertyGrid_OtherItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid_OtherItem.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.propertyGrid_OtherItem.Location = new System.Drawing.Point(4, 4);
            this.propertyGrid_OtherItem.Name = "propertyGrid_OtherItem";
            this.propertyGrid_OtherItem.Size = new System.Drawing.Size(1076, 270);
            this.propertyGrid_OtherItem.TabIndex = 5;
            this.propertyGrid_OtherItem.ToolbarVisible = false;
            this.propertyGrid_OtherItem.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_HardwareSetting_PropertyValueChanged);
            // 
            // dataGridView_BoardItem
            // 
            this.dataGridView_BoardItem.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView_BoardItem.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView_BoardItem.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_BoardItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_BoardItem.Location = new System.Drawing.Point(4, 281);
            this.dataGridView_BoardItem.Name = "dataGridView_BoardItem";
            this.dataGridView_BoardItem.RowTemplate.Height = 30;
            this.dataGridView_BoardItem.Size = new System.Drawing.Size(1076, 339);
            this.dataGridView_BoardItem.TabIndex = 6;
            // 
            // FrmCommunicationInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 695);
            this.Controls.Add(this.tableLayoutPanel_CommunicationInfo);
            this.Name = "FrmCommunicationInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FrmHardwareSetting";
            this.Load += new System.EventHandler(this.FrmCommunication_Load);
            this.tableLayoutPanel_CommunicationInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_BoardItem)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_CommunicationInfo;
        private System.Windows.Forms.PropertyGrid propertyGrid_OtherItem;
        private System.Windows.Forms.DataGridView dataGridView_BoardItem;
    }
}