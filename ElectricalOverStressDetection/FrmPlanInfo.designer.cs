namespace ElectricalOverStressData
{
    partial class FrmPlanInfo
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
            this.tableLayoutPanel_CalibrationItem = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel_CalibrationChannelSetting = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel_Loading = new System.Windows.Forms.TableLayoutPanel();
            this.progressBar_loading = new System.Windows.Forms.ProgressBar();
            this.tableLayoutPanel_ChannelSetting = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel_CalibrationItem.SuspendLayout();
            this.tableLayoutPanel_Loading.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel_CalibrationItem
            // 
            this.tableLayoutPanel_CalibrationItem.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel_CalibrationItem.ColumnCount = 2;
            this.tableLayoutPanel_CalibrationItem.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel_CalibrationItem.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel_CalibrationItem.Controls.Add(this.tableLayoutPanel_CalibrationChannelSetting, 0, 0);
            this.tableLayoutPanel_CalibrationItem.Controls.Add(this.tableLayoutPanel_ChannelSetting, 1, 0);
            this.tableLayoutPanel_CalibrationItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_CalibrationItem.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_CalibrationItem.Name = "tableLayoutPanel_CalibrationItem";
            this.tableLayoutPanel_CalibrationItem.RowCount = 1;
            this.tableLayoutPanel_CalibrationItem.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_CalibrationItem.Size = new System.Drawing.Size(1534, 861);
            this.tableLayoutPanel_CalibrationItem.TabIndex = 0;
            // 
            // tableLayoutPanel_CalibrationChannelSetting
            // 
            this.tableLayoutPanel_CalibrationChannelSetting.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel_CalibrationChannelSetting.ColumnCount = 1;
            this.tableLayoutPanel_CalibrationChannelSetting.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_CalibrationChannelSetting.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_CalibrationChannelSetting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_CalibrationChannelSetting.Location = new System.Drawing.Point(1, 1);
            this.tableLayoutPanel_CalibrationChannelSetting.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel_CalibrationChannelSetting.Name = "tableLayoutPanel_CalibrationChannelSetting";
            this.tableLayoutPanel_CalibrationChannelSetting.RowCount = 1;
            this.tableLayoutPanel_CalibrationChannelSetting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_CalibrationChannelSetting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 858F));
            this.tableLayoutPanel_CalibrationChannelSetting.Size = new System.Drawing.Size(382, 859);
            this.tableLayoutPanel_CalibrationChannelSetting.TabIndex = 0;
            // 
            // tableLayoutPanel_Loading
            // 
            this.tableLayoutPanel_Loading.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel_Loading.ColumnCount = 1;
            this.tableLayoutPanel_Loading.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_Loading.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_Loading.Controls.Add(this.progressBar_loading, 0, 1);
            this.tableLayoutPanel_Loading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_Loading.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_Loading.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel_Loading.Name = "tableLayoutPanel_Loading";
            this.tableLayoutPanel_Loading.RowCount = 3;
            this.tableLayoutPanel_Loading.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47F));
            this.tableLayoutPanel_Loading.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.tableLayoutPanel_Loading.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47F));
            this.tableLayoutPanel_Loading.Size = new System.Drawing.Size(1534, 861);
            this.tableLayoutPanel_Loading.TabIndex = 1;
            // 
            // progressBar_loading
            // 
            this.progressBar_loading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar_loading.Location = new System.Drawing.Point(4, 407);
            this.progressBar_loading.MarqueeAnimationSpeed = 1;
            this.progressBar_loading.Name = "progressBar_loading";
            this.progressBar_loading.Size = new System.Drawing.Size(1526, 45);
            this.progressBar_loading.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar_loading.TabIndex = 0;
            // 
            // tableLayoutPanel_ChannelSetting
            // 
            this.tableLayoutPanel_ChannelSetting.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel_ChannelSetting.ColumnCount = 1;
            this.tableLayoutPanel_ChannelSetting.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_ChannelSetting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_ChannelSetting.Location = new System.Drawing.Point(384, 1);
            this.tableLayoutPanel_ChannelSetting.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel_ChannelSetting.Name = "tableLayoutPanel_ChannelSetting";
            this.tableLayoutPanel_ChannelSetting.RowCount = 1;
            this.tableLayoutPanel_ChannelSetting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_ChannelSetting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_ChannelSetting.Size = new System.Drawing.Size(1149, 859);
            this.tableLayoutPanel_ChannelSetting.TabIndex = 1;
            // 
            // FrmCalibrationItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1534, 861);
            this.Controls.Add(this.tableLayoutPanel_CalibrationItem);
            this.Controls.Add(this.tableLayoutPanel_Loading);
            this.Name = "FrmCalibrationItem";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FrmCalibrationItem";
            this.Load += new System.EventHandler(this.FrmCalibrationItem_Load);
            this.tableLayoutPanel_CalibrationItem.ResumeLayout(false);
            this.tableLayoutPanel_Loading.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_CalibrationItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_CalibrationChannelSetting;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Loading;
        private System.Windows.Forms.ProgressBar progressBar_loading;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_ChannelSetting;
    }
}