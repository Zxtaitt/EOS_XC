namespace ElectricalOverStressDetection
{
    partial class FrmShowInfo
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
            this.tableLayoutPanel_LoadingInfo = new System.Windows.Forms.TableLayoutPanel();
            this.progressBar_Loading = new System.Windows.Forms.ProgressBar();
            this.tableLayoutPanel_ShowInfo = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel_LoadingInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel_LoadingInfo
            // 
            this.tableLayoutPanel_LoadingInfo.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel_LoadingInfo.ColumnCount = 1;
            this.tableLayoutPanel_LoadingInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_LoadingInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_LoadingInfo.Controls.Add(this.progressBar_Loading, 0, 1);
            this.tableLayoutPanel_LoadingInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_LoadingInfo.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_LoadingInfo.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel_LoadingInfo.Name = "tableLayoutPanel_LoadingInfo";
            this.tableLayoutPanel_LoadingInfo.RowCount = 3;
            this.tableLayoutPanel_LoadingInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47F));
            this.tableLayoutPanel_LoadingInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.tableLayoutPanel_LoadingInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47F));
            this.tableLayoutPanel_LoadingInfo.Size = new System.Drawing.Size(1335, 811);
            this.tableLayoutPanel_LoadingInfo.TabIndex = 1;
            // 
            // progressBar_Loading
            // 
            this.progressBar_Loading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar_Loading.Location = new System.Drawing.Point(4, 384);
            this.progressBar_Loading.MarqueeAnimationSpeed = 1;
            this.progressBar_Loading.Name = "progressBar_Loading";
            this.progressBar_Loading.Size = new System.Drawing.Size(1327, 42);
            this.progressBar_Loading.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar_Loading.TabIndex = 0;
            // 
            // tableLayoutPanel_ShowInfo
            // 
            this.tableLayoutPanel_ShowInfo.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel_ShowInfo.ColumnCount = 1;
            this.tableLayoutPanel_ShowInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_ShowInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_ShowInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_ShowInfo.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_ShowInfo.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel_ShowInfo.Name = "tableLayoutPanel_ShowInfo";
            this.tableLayoutPanel_ShowInfo.RowCount = 1;
            this.tableLayoutPanel_ShowInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_ShowInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 810F));
            this.tableLayoutPanel_ShowInfo.Size = new System.Drawing.Size(1335, 811);
            this.tableLayoutPanel_ShowInfo.TabIndex = 0;
            // 
            // FrmShowInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1335, 811);
            this.Controls.Add(this.tableLayoutPanel_ShowInfo);
            this.Controls.Add(this.tableLayoutPanel_LoadingInfo);
            this.Name = "FrmShowInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FrmCalibrationInfo";
            this.Load += new System.EventHandler(this.FrmCalibrationInfo_Load);
            this.tableLayoutPanel_LoadingInfo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ProgressBar progressBar_Loading;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_ShowInfo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_LoadingInfo;
    }
}