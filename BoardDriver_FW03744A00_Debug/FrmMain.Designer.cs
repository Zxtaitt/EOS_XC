namespace BoardDriver_FW03744A00_Debug
{
    partial class FrmMain
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.grpConnect = new System.Windows.Forms.GroupBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.numAddress = new System.Windows.Forms.NumericUpDown();
            this.lblAddress = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtIp = new System.Windows.Forms.TextBox();
            this.lblIp = new System.Windows.Forms.Label();
            this.grpTest = new System.Windows.Forms.GroupBox();
            this.lblCountdown = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.lblDurationUnit = new System.Windows.Forms.Label();
            this.txtDuration = new System.Windows.Forms.TextBox();
            this.lblDuration = new System.Windows.Forms.Label();
            this.lblValueUnit = new System.Windows.Forms.Label();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.lblValue = new System.Windows.Forms.Label();
            this.rdoNegative = new System.Windows.Forms.RadioButton();
            this.rdoPositive = new System.Windows.Forms.RadioButton();
            this.lblDirection = new System.Windows.Forms.Label();
            this.rdoVoltage = new System.Windows.Forms.RadioButton();
            this.rdoCurrent = new System.Windows.Forms.RadioButton();
            this.lblSource = new System.Windows.Forms.Label();
            this.cmbChannel = new System.Windows.Forms.ComboBox();
            this.lblChannel = new System.Windows.Forms.Label();
            this.grpLog = new System.Windows.Forms.GroupBox();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.pnlLogButtons = new System.Windows.Forms.Panel();
            this.chkAutoScroll = new System.Windows.Forms.CheckBox();
            this.btnSaveLog = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.grpConnect.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAddress)).BeginInit();
            this.grpTest.SuspendLayout();
            this.grpLog.SuspendLayout();
            this.pnlLogButtons.SuspendLayout();
            this.SuspendLayout();
            //
            // grpConnect
            //
            this.grpConnect.Controls.Add(this.lblStatus);
            this.grpConnect.Controls.Add(this.btnDisconnect);
            this.grpConnect.Controls.Add(this.btnConnect);
            this.grpConnect.Controls.Add(this.numAddress);
            this.grpConnect.Controls.Add(this.lblAddress);
            this.grpConnect.Controls.Add(this.txtPort);
            this.grpConnect.Controls.Add(this.lblPort);
            this.grpConnect.Controls.Add(this.txtIp);
            this.grpConnect.Controls.Add(this.lblIp);
            this.grpConnect.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpConnect.Location = new System.Drawing.Point(0, 0);
            this.grpConnect.Name = "grpConnect";
            this.grpConnect.Size = new System.Drawing.Size(920, 70);
            this.grpConnect.TabIndex = 0;
            this.grpConnect.TabStop = false;
            this.grpConnect.Text = "连接";
            //
            // lblIp
            //
            this.lblIp.AutoSize = true;
            this.lblIp.Location = new System.Drawing.Point(15, 30);
            this.lblIp.Name = "lblIp";
            this.lblIp.Size = new System.Drawing.Size(23, 14);
            this.lblIp.TabIndex = 0;
            this.lblIp.Text = "IP:";
            //
            // txtIp
            //
            this.txtIp.Location = new System.Drawing.Point(45, 27);
            this.txtIp.Name = "txtIp";
            this.txtIp.Size = new System.Drawing.Size(130, 23);
            this.txtIp.TabIndex = 1;
            this.txtIp.Text = "192.168.1.100";
            //
            // lblPort
            //
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(190, 30);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(37, 14);
            this.lblPort.TabIndex = 2;
            this.lblPort.Text = "Port:";
            //
            // txtPort
            //
            this.txtPort.Location = new System.Drawing.Point(230, 27);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(65, 23);
            this.txtPort.TabIndex = 3;
            this.txtPort.Text = "8000";
            //
            // lblAddress
            //
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(310, 30);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(38, 14);
            this.lblAddress.TabIndex = 4;
            this.lblAddress.Text = "Addr:";
            //
            // numAddress
            //
            this.numAddress.Location = new System.Drawing.Point(355, 27);
            this.numAddress.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            this.numAddress.Name = "numAddress";
            this.numAddress.Size = new System.Drawing.Size(55, 23);
            this.numAddress.TabIndex = 5;
            this.numAddress.Value = new decimal(new int[] { 1, 0, 0, 0 });
            //
            // btnConnect
            //
            this.btnConnect.Location = new System.Drawing.Point(430, 25);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 27);
            this.btnConnect.TabIndex = 6;
            this.btnConnect.Text = "连接";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            //
            // btnDisconnect
            //
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Location = new System.Drawing.Point(515, 25);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(75, 27);
            this.btnDisconnect.TabIndex = 7;
            this.btnDisconnect.Text = "断开";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.lblStatus.Location = new System.Drawing.Point(620, 30);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(90, 14);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "状态: ● 未连接";
            //
            // grpTest
            //
            this.grpTest.Controls.Add(this.lblCountdown);
            this.grpTest.Controls.Add(this.progressBar);
            this.grpTest.Controls.Add(this.btnStop);
            this.grpTest.Controls.Add(this.btnStart);
            this.grpTest.Controls.Add(this.lblDurationUnit);
            this.grpTest.Controls.Add(this.txtDuration);
            this.grpTest.Controls.Add(this.lblDuration);
            this.grpTest.Controls.Add(this.lblValueUnit);
            this.grpTest.Controls.Add(this.txtValue);
            this.grpTest.Controls.Add(this.lblValue);
            this.grpTest.Controls.Add(this.rdoNegative);
            this.grpTest.Controls.Add(this.rdoPositive);
            this.grpTest.Controls.Add(this.lblDirection);
            this.grpTest.Controls.Add(this.rdoVoltage);
            this.grpTest.Controls.Add(this.rdoCurrent);
            this.grpTest.Controls.Add(this.lblSource);
            this.grpTest.Controls.Add(this.cmbChannel);
            this.grpTest.Controls.Add(this.lblChannel);
            this.grpTest.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpTest.Location = new System.Drawing.Point(0, 70);
            this.grpTest.Name = "grpTest";
            this.grpTest.Size = new System.Drawing.Size(920, 150);
            this.grpTest.TabIndex = 1;
            this.grpTest.TabStop = false;
            this.grpTest.Text = "通道测试";
            //
            // lblChannel
            //
            this.lblChannel.AutoSize = true;
            this.lblChannel.Location = new System.Drawing.Point(15, 30);
            this.lblChannel.Name = "lblChannel";
            this.lblChannel.Size = new System.Drawing.Size(52, 14);
            this.lblChannel.TabIndex = 0;
            this.lblChannel.Text = "通道号:";
            //
            // cmbChannel
            //
            this.cmbChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChannel.Location = new System.Drawing.Point(75, 27);
            this.cmbChannel.Name = "cmbChannel";
            this.cmbChannel.Size = new System.Drawing.Size(70, 22);
            this.cmbChannel.TabIndex = 1;
            //
            // lblSource
            //
            this.lblSource.AutoSize = true;
            this.lblSource.Location = new System.Drawing.Point(170, 30);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(52, 14);
            this.lblSource.TabIndex = 2;
            this.lblSource.Text = "源类型:";
            //
            // rdoCurrent
            //
            this.rdoCurrent.AutoSize = true;
            this.rdoCurrent.Checked = true;
            this.rdoCurrent.Location = new System.Drawing.Point(230, 28);
            this.rdoCurrent.Name = "rdoCurrent";
            this.rdoCurrent.Size = new System.Drawing.Size(84, 18);
            this.rdoCurrent.TabIndex = 3;
            this.rdoCurrent.TabStop = true;
            this.rdoCurrent.Text = "电流源(mA)";
            this.rdoCurrent.UseVisualStyleBackColor = true;
            this.rdoCurrent.CheckedChanged += new System.EventHandler(this.SourceChanged);
            //
            // rdoVoltage
            //
            this.rdoVoltage.AutoSize = true;
            this.rdoVoltage.Location = new System.Drawing.Point(320, 28);
            this.rdoVoltage.Name = "rdoVoltage";
            this.rdoVoltage.Size = new System.Drawing.Size(84, 18);
            this.rdoVoltage.TabIndex = 4;
            this.rdoVoltage.Text = "电压源(mV)";
            this.rdoVoltage.UseVisualStyleBackColor = true;
            this.rdoVoltage.CheckedChanged += new System.EventHandler(this.SourceChanged);
            //
            // lblDirection
            //
            this.lblDirection.AutoSize = true;
            this.lblDirection.Location = new System.Drawing.Point(420, 30);
            this.lblDirection.Name = "lblDirection";
            this.lblDirection.Size = new System.Drawing.Size(38, 14);
            this.lblDirection.TabIndex = 5;
            this.lblDirection.Text = "方向:";
            //
            // rdoPositive
            //
            this.rdoPositive.AutoSize = true;
            this.rdoPositive.Checked = true;
            this.rdoPositive.Location = new System.Drawing.Point(465, 28);
            this.rdoPositive.Name = "rdoPositive";
            this.rdoPositive.Size = new System.Drawing.Size(48, 18);
            this.rdoPositive.TabIndex = 6;
            this.rdoPositive.TabStop = true;
            this.rdoPositive.Text = "正向";
            this.rdoPositive.UseVisualStyleBackColor = true;
            //
            // rdoNegative
            //
            this.rdoNegative.AutoSize = true;
            this.rdoNegative.Location = new System.Drawing.Point(520, 28);
            this.rdoNegative.Name = "rdoNegative";
            this.rdoNegative.Size = new System.Drawing.Size(48, 18);
            this.rdoNegative.TabIndex = 7;
            this.rdoNegative.Text = "负向";
            this.rdoNegative.UseVisualStyleBackColor = true;
            //
            // lblValue
            //
            this.lblValue.AutoSize = true;
            this.lblValue.Location = new System.Drawing.Point(15, 70);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(52, 14);
            this.lblValue.TabIndex = 8;
            this.lblValue.Text = "设置值:";
            //
            // txtValue
            //
            this.txtValue.Location = new System.Drawing.Point(75, 67);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(100, 23);
            this.txtValue.TabIndex = 9;
            this.txtValue.Text = "10";
            //
            // lblValueUnit
            //
            this.lblValueUnit.AutoSize = true;
            this.lblValueUnit.Location = new System.Drawing.Point(180, 70);
            this.lblValueUnit.Name = "lblValueUnit";
            this.lblValueUnit.Size = new System.Drawing.Size(25, 14);
            this.lblValueUnit.TabIndex = 10;
            this.lblValueUnit.Text = "mA";
            //
            // lblDuration
            //
            this.lblDuration.AutoSize = true;
            this.lblDuration.Location = new System.Drawing.Point(230, 70);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(66, 14);
            this.lblDuration.TabIndex = 11;
            this.lblDuration.Text = "持续时间:";
            //
            // txtDuration
            //
            this.txtDuration.Location = new System.Drawing.Point(300, 67);
            this.txtDuration.Name = "txtDuration";
            this.txtDuration.Size = new System.Drawing.Size(80, 23);
            this.txtDuration.TabIndex = 12;
            this.txtDuration.Text = "5";
            //
            // lblDurationUnit
            //
            this.lblDurationUnit.AutoSize = true;
            this.lblDurationUnit.Location = new System.Drawing.Point(385, 70);
            this.lblDurationUnit.Name = "lblDurationUnit";
            this.lblDurationUnit.Size = new System.Drawing.Size(14, 14);
            this.lblDurationUnit.TabIndex = 13;
            this.lblDurationUnit.Text = "秒";
            //
            // btnStart
            //
            this.btnStart.Enabled = false;
            this.btnStart.Location = new System.Drawing.Point(15, 105);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(120, 30);
            this.btnStart.TabIndex = 14;
            this.btnStart.Text = "▶ 开始测试";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            //
            // btnStop
            //
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(145, 105);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(120, 30);
            this.btnStop.TabIndex = 15;
            this.btnStop.Text = "■ 紧急停止";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            //
            // progressBar
            //
            this.progressBar.Location = new System.Drawing.Point(275, 109);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(450, 22);
            this.progressBar.TabIndex = 16;
            //
            // lblCountdown
            //
            this.lblCountdown.AutoSize = true;
            this.lblCountdown.ForeColor = System.Drawing.Color.DimGray;
            this.lblCountdown.Location = new System.Drawing.Point(735, 113);
            this.lblCountdown.Name = "lblCountdown";
            this.lblCountdown.Size = new System.Drawing.Size(30, 14);
            this.lblCountdown.TabIndex = 17;
            this.lblCountdown.Text = "空闲";
            //
            // grpLog
            //
            this.grpLog.Controls.Add(this.rtbLog);
            this.grpLog.Controls.Add(this.pnlLogButtons);
            this.grpLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpLog.Location = new System.Drawing.Point(0, 220);
            this.grpLog.Name = "grpLog";
            this.grpLog.Size = new System.Drawing.Size(920, 430);
            this.grpLog.TabIndex = 2;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "通讯日志";
            //
            // pnlLogButtons
            //
            this.pnlLogButtons.Controls.Add(this.chkAutoScroll);
            this.pnlLogButtons.Controls.Add(this.btnSaveLog);
            this.pnlLogButtons.Controls.Add(this.btnClearLog);
            this.pnlLogButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLogButtons.Location = new System.Drawing.Point(3, 19);
            this.pnlLogButtons.Name = "pnlLogButtons";
            this.pnlLogButtons.Size = new System.Drawing.Size(914, 35);
            this.pnlLogButtons.TabIndex = 0;
            //
            // btnClearLog
            //
            this.btnClearLog.Location = new System.Drawing.Point(10, 5);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(75, 25);
            this.btnClearLog.TabIndex = 0;
            this.btnClearLog.Text = "清空";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            //
            // btnSaveLog
            //
            this.btnSaveLog.Location = new System.Drawing.Point(95, 5);
            this.btnSaveLog.Name = "btnSaveLog";
            this.btnSaveLog.Size = new System.Drawing.Size(100, 25);
            this.btnSaveLog.TabIndex = 1;
            this.btnSaveLog.Text = "保存到文件";
            this.btnSaveLog.UseVisualStyleBackColor = true;
            this.btnSaveLog.Click += new System.EventHandler(this.btnSaveLog_Click);
            //
            // chkAutoScroll
            //
            this.chkAutoScroll.AutoSize = true;
            this.chkAutoScroll.Checked = true;
            this.chkAutoScroll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoScroll.Location = new System.Drawing.Point(210, 9);
            this.chkAutoScroll.Name = "chkAutoScroll";
            this.chkAutoScroll.Size = new System.Drawing.Size(74, 18);
            this.chkAutoScroll.TabIndex = 2;
            this.chkAutoScroll.Text = "自动滚动";
            this.chkAutoScroll.UseVisualStyleBackColor = true;
            //
            // rtbLog
            //
            this.rtbLog.BackColor = System.Drawing.Color.White;
            this.rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.rtbLog.Location = new System.Drawing.Point(3, 54);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.Size = new System.Drawing.Size(914, 373);
            this.rtbLog.TabIndex = 1;
            this.rtbLog.Text = "";
            this.rtbLog.WordWrap = false;
            //
            // FrmMain
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(920, 650);
            this.Controls.Add(this.grpLog);
            this.Controls.Add(this.grpTest);
            this.Controls.Add(this.grpConnect);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.MinimumSize = new System.Drawing.Size(820, 500);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BoardDriver_FW03744A00 调试工具";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.grpConnect.ResumeLayout(false);
            this.grpConnect.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAddress)).EndInit();
            this.grpTest.ResumeLayout(false);
            this.grpTest.PerformLayout();
            this.grpLog.ResumeLayout(false);
            this.pnlLogButtons.ResumeLayout(false);
            this.pnlLogButtons.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.GroupBox grpConnect;
        private System.Windows.Forms.Label lblIp;
        private System.Windows.Forms.TextBox txtIp;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.NumericUpDown numAddress;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.GroupBox grpTest;
        private System.Windows.Forms.Label lblChannel;
        private System.Windows.Forms.ComboBox cmbChannel;
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.RadioButton rdoCurrent;
        private System.Windows.Forms.RadioButton rdoVoltage;
        private System.Windows.Forms.Label lblDirection;
        private System.Windows.Forms.RadioButton rdoPositive;
        private System.Windows.Forms.RadioButton rdoNegative;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Label lblValueUnit;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.TextBox txtDuration;
        private System.Windows.Forms.Label lblDurationUnit;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblCountdown;
        private System.Windows.Forms.GroupBox grpLog;
        private System.Windows.Forms.Panel pnlLogButtons;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Button btnSaveLog;
        private System.Windows.Forms.CheckBox chkAutoScroll;
        private System.Windows.Forms.RichTextBox rtbLog;
    }
}
