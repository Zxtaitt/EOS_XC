using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BoardDriver;

namespace BoardDriver_FW03744A00_Debug
{
    public partial class FrmMain : Form
    {
        private const int ChannelCount = 48;

        private BoardDriver_FW03744A00 _driver;
        private CancellationTokenSource _cts;
        private volatile bool _busy;

        public FrmMain()
        {
            InitializeComponent();
            for (int i = 0; i < ChannelCount; i++) cmbChannel.Items.Add(i);
            cmbChannel.SelectedIndex = 0;
            UpdateValueUnit();
        }

        #region 连接 / 断开

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string ip = txtIp.Text.Trim();
            if (ip.Length == 0) { MessageBox.Show(this, "请填写 IP", "提示"); return; }
            if (!int.TryParse(txtPort.Text.Trim(), out int port) || port <= 0 || port > 65535)
            {
                MessageBox.Show(this, "Port 无效", "提示"); return;
            }
            byte addr = (byte)numAddress.Value;

            try
            {
                var client = new TcpClient();
                AppendLog("INFO", null, $"尝试 TCP 连接 {ip}:{port} ...");
                var connectTask = client.ConnectAsync(ip, port);
                if (!connectTask.Wait(3000))
                {
                    try { client.Close(); } catch { }
                    throw new Exception("TCP 连接超时 (3s)");
                }
                var ns = client.GetStream();
                ns.ReadTimeout = 3000;
                ns.WriteTimeout = 3000;

                _driver = new BoardDriver_FW03744A00
                {
                    BoardClientConStr = $"{ip}:{port}",
                    BoardClient = client,
                    BoardAddress = addr
                };
                _driver.OnFrame += OnDriverFrame;

                SetConnected(true);
                AppendLog("INFO", null, $"TCP 已连接 {ip}:{port}  (BoardAddress={addr})");
            }
            catch (Exception ex)
            {
                AppendLog("ERR", null, "连接失败: " + ex.Message);
                MessageBox.Show(this, "连接失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            CloseDriver();
            SetConnected(false);
            AppendLog("INFO", null, "已断开连接");
        }

        private void CloseDriver()
        {
            if (_driver != null)
            {
                _driver.OnFrame -= OnDriverFrame;
                try { if (_driver.BoardClient != null) _driver.BoardClient.Close(); } catch { }
                _driver = null;
            }
        }

        private void SetConnected(bool on)
        {
            btnConnect.Enabled = !on;
            btnDisconnect.Enabled = on;
            btnStart.Enabled = on;
            txtIp.Enabled = !on;
            txtPort.Enabled = !on;
            numAddress.Enabled = !on;
            lblStatus.Text = on ? "状态: ● 已连接" : "状态: ● 未连接";
            lblStatus.ForeColor = on ? Color.Green : Color.DimGray;
        }

        #endregion

        #region 一键测试

        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (_busy || _driver == null) return;

            int channel = (int)cmbChannel.SelectedItem;
            var source = rdoVoltage.Checked
                ? BoardDriverEnum.SourceType.VoltageSource
                : BoardDriverEnum.SourceType.CurrentSource;
            var direction = rdoNegative.Checked
                ? BoardDriverEnum.Direction.Negative
                : BoardDriverEnum.Direction.Positive;

            if (!double.TryParse(txtValue.Text.Trim(), out double setValue) || setValue < 0)
            {
                MessageBox.Show(this, "设置值无效", "提示"); return;
            }
            if (!double.TryParse(txtDuration.Text.Trim(), out double durationSec) || durationSec < 0)
            {
                MessageBox.Show(this, "持续时间无效", "提示"); return;
            }

            _busy = true;
            _cts = new CancellationTokenSource();
            SetTestUi(true);

            string unit = source == BoardDriverEnum.SourceType.VoltageSource ? "mV" : "mA";
            string srcText = source == BoardDriverEnum.SourceType.VoltageSource ? "电压源" : "电流源";
            string dirText = direction == BoardDriverEnum.Direction.Negative ? "负向" : "正向";
            AppendLog("INFO", null, $"========== 开始测试 CH{channel} {srcText} {dirText} {setValue}{unit} 持续 {durationSec}s ==========");

            var token = _cts.Token;
            try
            {
                await Task.Run(() => RunTestSequence(channel, source, direction, setValue, durationSec, token), token);
                AppendLog("INFO", null, "========== 测试完成 ==========");
            }
            catch (OperationCanceledException)
            {
                AppendLog("INFO", null, "========== 测试被用户中止 ==========");
            }
            catch (Exception ex)
            {
                AppendLog("ERR", null, "测试异常: " + ex.Message);
            }
            finally
            {
                _busy = false;
                SetTestUi(false);
            }
        }

        private void RunTestSequence(int channel, BoardDriverEnum.SourceType source, BoardDriverEnum.Direction direction,
                                     double setValue, double durationSec, CancellationToken token)
        {
            AppendLog("INFO", null, "[步骤1] 初始化 + CSP/CSN + 全通道输出准备");
            _driver.SetBoardAdjustDriveVoltage();
            token.ThrowIfCancellationRequested();

            AppendLog("INFO", null, "[步骤2] 设置输出模式 + 钳位 + 打开通道输出");
            _driver.SetBoardClamp(channel, source, direction);
            token.ThrowIfCancellationRequested();

            AppendLog("INFO", null, $"[步骤3] 阶梯上电到目标值 {setValue}");
            _driver.SetBoardOnPower(channel, source, BoardDriverEnum.PowerMethod.Single, 1, setValue);
            token.ThrowIfCancellationRequested();

            AppendLog("INFO", null, $"[步骤4] 保持 {durationSec}s （请在此期间观察示波器波形）");
            RunHold(durationSec, token);

            AppendLog("INFO", null, "[步骤5] 阶梯下电");
            _driver.SetBoardOFFPower(channel, source, BoardDriverEnum.PowerMethod.Single, 1, setValue);

            AppendLog("INFO", null, "[步骤6] 关闭通道输出");
            _driver.OpenRelay(channel);
        }

        private void RunHold(double durationSec, CancellationToken token)
        {
            int totalMs = (int)Math.Round(durationSec * 1000);
            if (totalMs <= 0) return;

            BeginInvoke((Action)(() =>
            {
                progressBar.Minimum = 0;
                progressBar.Maximum = totalMs;
                progressBar.Value = 0;
            }));

            int elapsed = 0;
            const int tick = 100;
            while (elapsed < totalMs)
            {
                if (token.IsCancellationRequested) throw new OperationCanceledException(token);
                int sleep = Math.Min(tick, totalMs - elapsed);
                Thread.Sleep(sleep);
                elapsed += sleep;
                int e = elapsed;
                int remainSec = (totalMs - e + 999) / 1000;
                BeginInvoke((Action)(() =>
                {
                    if (progressBar.Maximum >= e) progressBar.Value = e;
                    lblCountdown.Text = $"剩余 {remainSec}s";
                }));
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (_cts != null)
            {
                AppendLog("INFO", null, "用户请求紧急停止，将跳转到下电+关通道");
                _cts.Cancel();
            }
            TryEmergencyShutdown();
        }

        private void TryEmergencyShutdown()
        {
            if (_driver == null) return;
            int channel;
            try { channel = (int)cmbChannel.SelectedItem; } catch { return; }

            Task.Run(() =>
            {
                try
                {
                    var source = rdoVoltage.Checked
                        ? BoardDriverEnum.SourceType.VoltageSource
                        : BoardDriverEnum.SourceType.CurrentSource;
                    double.TryParse(txtValue.Text.Trim(), out double v);
                    AppendLog("INFO", null, "[紧急下电] 阶梯下电 + 关通道");
                    _driver.SetBoardOFFPower(channel, source, BoardDriverEnum.PowerMethod.Single, 1, v);
                    _driver.OpenRelay(channel);
                }
                catch (Exception ex)
                {
                    AppendLog("ERR", null, "紧急下电失败: " + ex.Message);
                }
            });
        }

        private void SetTestUi(bool testing)
        {
            btnStart.Enabled = !testing && _driver != null;
            btnStop.Enabled = testing;
            btnDisconnect.Enabled = !testing && _driver != null;
            cmbChannel.Enabled = !testing;
            rdoCurrent.Enabled = !testing;
            rdoVoltage.Enabled = !testing;
            rdoPositive.Enabled = !testing;
            rdoNegative.Enabled = !testing;
            txtValue.Enabled = !testing;
            txtDuration.Enabled = !testing;
            if (!testing)
            {
                progressBar.Value = 0;
                lblCountdown.Text = "空闲";
            }
        }

        #endregion

        #region 日志

        private void OnDriverFrame(string direction, byte[] bytes, string note)
        {
            AppendLog(direction, bytes, note);
        }

        private void AppendLog(string tag, byte[] bytes, string note)
        {
            if (InvokeRequired) { BeginInvoke((Action)(() => AppendLog(tag, bytes, note))); return; }

            Color color;
            switch (tag)
            {
                case "TX": color = Color.FromArgb(0, 80, 180); break;
                case "RX": color = Color.FromArgb(20, 130, 40); break;
                case "ERR": color = Color.Red; break;
                default: color = Color.Black; break;
            }
            string ts = DateTime.Now.ToString("HH:mm:ss.fff");
            string head = $"{ts} [{tag,-3}] {note ?? ""}\r\n";

            int start = rtbLog.TextLength;
            rtbLog.AppendText(head);
            rtbLog.Select(start, head.Length);
            rtbLog.SelectionColor = color;

            if (bytes != null && bytes.Length > 0)
            {
                string hex = "              " + ToHex(bytes) + "\r\n";
                int s2 = rtbLog.TextLength;
                rtbLog.AppendText(hex);
                rtbLog.Select(s2, hex.Length);
                rtbLog.SelectionColor = Color.Gray;
            }

            rtbLog.Select(rtbLog.TextLength, 0);
            rtbLog.SelectionColor = Color.Black;

            if (chkAutoScroll.Checked)
            {
                rtbLog.SelectionStart = rtbLog.TextLength;
                rtbLog.ScrollToCaret();
            }
        }

        private static string ToHex(byte[] b)
        {
            var sb = new System.Text.StringBuilder(b.Length * 3);
            for (int i = 0; i < b.Length; i++)
            {
                if (i > 0) sb.Append(' ');
                sb.Append(b[i].ToString("X2"));
            }
            return sb.ToString();
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            rtbLog.Clear();
        }

        private void btnSaveLog_Click(object sender, EventArgs e)
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "日志文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";
                dlg.FileName = $"FW03744A00_debug_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(dlg.FileName, rtbLog.Text);
                        AppendLog("INFO", null, "日志已保存: " + dlg.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "保存失败: " + ex.Message, "错误");
                    }
                }
            }
        }

        #endregion

        #region 其它

        private void SourceChanged(object sender, EventArgs e)
        {
            UpdateValueUnit();
        }

        private void UpdateValueUnit()
        {
            lblValueUnit.Text = rdoVoltage.Checked ? "mV" : "mA";
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_busy)
            {
                if (MessageBox.Show(this, "测试正在进行，确定关闭？会尝试紧急下电。", "确认",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                {
                    e.Cancel = true;
                    return;
                }
                if (_cts != null) _cts.Cancel();
                TryEmergencyShutdown();
                Thread.Sleep(500);
            }
            CloseDriver();
        }

        #endregion
    }
}
