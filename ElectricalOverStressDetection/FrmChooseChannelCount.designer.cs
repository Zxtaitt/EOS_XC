namespace ElectricalOverStressData
{
    partial class FrmChooseChannelCount
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
            this.tableLayoutPanel_ChooseChannelCount = new System.Windows.Forms.TableLayoutPanel();
            this.label_ChooseChannelCount = new System.Windows.Forms.Label();
            this.button_Confirm = new System.Windows.Forms.Button();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.comboBox_ChooseChannelCount = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel_ChooseChannelCount.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel_ChooseChannelCount
            // 
            this.tableLayoutPanel_ChooseChannelCount.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel_ChooseChannelCount.ColumnCount = 2;
            this.tableLayoutPanel_ChooseChannelCount.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_ChooseChannelCount.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_ChooseChannelCount.Controls.Add(this.label_ChooseChannelCount, 0, 0);
            this.tableLayoutPanel_ChooseChannelCount.Controls.Add(this.button_Confirm, 0, 1);
            this.tableLayoutPanel_ChooseChannelCount.Controls.Add(this.button_Cancel, 1, 1);
            this.tableLayoutPanel_ChooseChannelCount.Controls.Add(this.comboBox_ChooseChannelCount, 1, 0);
            this.tableLayoutPanel_ChooseChannelCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_ChooseChannelCount.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_ChooseChannelCount.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel_ChooseChannelCount.Name = "tableLayoutPanel_ChooseChannelCount";
            this.tableLayoutPanel_ChooseChannelCount.RowCount = 2;
            this.tableLayoutPanel_ChooseChannelCount.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_ChooseChannelCount.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_ChooseChannelCount.Size = new System.Drawing.Size(384, 76);
            this.tableLayoutPanel_ChooseChannelCount.TabIndex = 0;
            // 
            // label_ChooseChannelCount
            // 
            this.label_ChooseChannelCount.AutoSize = true;
            this.label_ChooseChannelCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_ChooseChannelCount.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_ChooseChannelCount.Location = new System.Drawing.Point(1, 1);
            this.label_ChooseChannelCount.Margin = new System.Windows.Forms.Padding(0);
            this.label_ChooseChannelCount.Name = "label_ChooseChannelCount";
            this.label_ChooseChannelCount.Size = new System.Drawing.Size(190, 36);
            this.label_ChooseChannelCount.TabIndex = 0;
            this.label_ChooseChannelCount.Text = "选择通道数";
            this.label_ChooseChannelCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button_Confirm
            // 
            this.button_Confirm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Confirm.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Confirm.Location = new System.Drawing.Point(4, 41);
            this.button_Confirm.Name = "button_Confirm";
            this.button_Confirm.Size = new System.Drawing.Size(184, 31);
            this.button_Confirm.TabIndex = 1;
            this.button_Confirm.Text = "确定";
            this.button_Confirm.UseVisualStyleBackColor = true;
            this.button_Confirm.Click += new System.EventHandler(this.button_Confirm_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Cancel.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Cancel.Location = new System.Drawing.Point(195, 41);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(185, 31);
            this.button_Cancel.TabIndex = 2;
            this.button_Cancel.Text = "取消";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // comboBox_ChooseChannelCount
            // 
            this.comboBox_ChooseChannelCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBox_ChooseChannelCount.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox_ChooseChannelCount.FormattingEnabled = true;
            this.comboBox_ChooseChannelCount.Location = new System.Drawing.Point(195, 4);
            this.comboBox_ChooseChannelCount.Name = "comboBox_ChooseChannelCount";
            this.comboBox_ChooseChannelCount.Size = new System.Drawing.Size(185, 29);
            this.comboBox_ChooseChannelCount.TabIndex = 3;
            // 
            // FrmChooseChannelCount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 76);
            this.Controls.Add(this.tableLayoutPanel_ChooseChannelCount);
            this.Name = "FrmChooseChannelCount";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FrmChooseChannelCount";
            this.Load += new System.EventHandler(this.FrmChooseChannelCount_Load);
            this.tableLayoutPanel_ChooseChannelCount.ResumeLayout(false);
            this.tableLayoutPanel_ChooseChannelCount.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_ChooseChannelCount;
        private System.Windows.Forms.Label label_ChooseChannelCount;
        private System.Windows.Forms.Button button_Confirm;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.ComboBox comboBox_ChooseChannelCount;
    }
}