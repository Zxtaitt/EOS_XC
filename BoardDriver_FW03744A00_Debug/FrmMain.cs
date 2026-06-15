using System;
using System.Drawing;
using System.IO;
using System.IO.Ports;
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
        private const int DefaultIoTimeoutMs = 3000;

        private BoardDriver_FW03744A00 _driver;
        private CancellationTokenSource _cts;
        private volatile bool _busy;
        private TcpClient _tcpClient;
        private SerialPort _serialPort;

        private ComboBox cmbTransport;
        private Label lblTransport;
        private Label lblCom;
        private Label lblBaud;
        private ComboBox cmbCom;
        private TextBox txtBaud;
        private Button btnRefreshCom;
        private TextBox txtHexCmd;
        private Button btnSendHex;

        private enum TransportKind
        {
            Tcp,
            Serial
        }

        public FrmMain()
        {
            InitializeComponent();
            InitializeTransportControls();
            InitializeRawCommandControls();
            for (int i = 0; i < ChannelCount; i++) cmbChannel.Items.Add(i);
            cmbChannel.SelectedIndex = 0;
            UpdateValueUnit();
            RefreshComPorts();
        }

        #region 连接 / 断开

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (GetTransportKind() == TransportKind.Serial)
            {
                ConnectSerial();
                return;
            }

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
                ns.ReadTimeout = DefaultIoTimeoutMs;
                ns.WriteTimeout = DefaultIoTimeoutMs;
                _tcpClient = client;

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

        private void ConnectSerial()
        {
            string com = cmbCom == null ? string.Empty : cmbCom.Text.Trim();
            if (com.Length == 0)
            {
                MessageBox.Show(this, "请选择串口号", "提示");
                return;
            }
            if (!int.TryParse(txtBaud.Text.Trim(), out int baud) || baud <= 0)
            {
                MessageBox.Show(this, "波特率无效", "提示");
                return;
            }

            byte addr = (byte)numAddress.Value;
            try
            {
                AppendLog("INFO", null, $"尝试串口连接 {com}@{baud} ...");
                var sp = new SerialPort(com, baud, Parity.None, 8, StopBits.One);
                sp.ReadTimeout = DefaultIoTimeoutMs;
                sp.WriteTimeout = DefaultIoTimeoutMs;
                _serialPort = sp;

                var serialDriver = new BoardDriver_FW03744A00_Serial
                {
                    BoardSerialPort = sp,
                    BoardAddress = addr,
                    BaudRate = baud,
                    Parity = Parity.None,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    Handshake = Handshake.None,
                    ReadTimeoutMs = DefaultIoTimeoutMs,
                    WriteTimeoutMs = DefaultIoTimeoutMs
                };
                serialDriver.ConnectSerialClient();
                serialDriver.OnFrame += OnDriverFrame;
                _driver = serialDriver;

                SetConnected(true);
                AppendLog("INFO", null, $"串口已连接 {com}@{baud} (BoardAddress={addr})");
            }
            catch (Exception ex)
            {
                AppendLog("ERR", null, "串口连接失败: " + ex.Message);
                MessageBox.Show(this, "串口连接失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            CloseConnection();
            SetConnected(false);
            AppendLog("INFO", null, "已断开连接");
        }

        private void CloseConnection()
        {
            if (_driver != null)
            {
                _driver.OnFrame -= OnDriverFrame;
                _driver = null;
            }
            try
            {
                if (_tcpClient != null) _tcpClient.Close();
            }
            catch { }
            _tcpClient = null;
            try
            {
                if (_serialPort != null && _serialPort.IsOpen) _serialPort.Close();
                if (_serialPort != null) _serialPort.Dispose();
            }
            catch { }
            _serialPort = null;
        }

        private void SetConnected(bool on)
        {
            btnConnect.Enabled = !on;
            btnDisconnect.Enabled = on;
            btnStart.Enabled = on && _driver != null;
            txtIp.Enabled = !on;
            txtPort.Enabled = !on;
            numAddress.Enabled = !on;
            if (cmbTransport != null) cmbTransport.Enabled = !on;
            if (cmbCom != null) cmbCom.Enabled = !on;
            if (txtBaud != null) txtBaud.Enabled = !on;
            if (btnRefreshCom != null) btnRefreshCom.Enabled = !on;
            if (btnSendHex != null) btnSendHex.Enabled = on;
            lblStatus.Text = on ? $"状态: ● 已连接 ({(GetTransportKind() == TransportKind.Tcp ? "TCP" : "Serial")})" : "状态: ● 未连接";
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
            int stepCount = (int)numStep.Value;

            _busy = true;
            _cts = new CancellationTokenSource();
            SetTestUi(true);

            string unit = source == BoardDriverEnum.SourceType.VoltageSource ? "mV" : "mA";
            string srcText = source == BoardDriverEnum.SourceType.VoltageSource ? "电压源" : "电流源";
            string dirText = direction == BoardDriverEnum.Direction.Negative ? "负向" : "正向";
            AppendLog("INFO", null, $"========== 开始测试 CH{channel} {srcText} {dirText} {setValue}{unit} 持续 {durationSec}s 上下电步数={stepCount} ==========");

            var token = _cts.Token;
            try
            {
                await Task.Run(() => RunTestSequence(channel, source, direction, setValue, durationSec, stepCount, token), token);
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
                                     double setValue, double durationSec, int stepCount, CancellationToken token)
        {
            var method = stepCount <= 1 ? BoardDriverEnum.PowerMethod.Single : BoardDriverEnum.PowerMethod.Step;

            AppendLog("INFO", null, "[步骤1] 初始化 + CSP/CSN + 全通道输出准备");
            _driver.SetBoardAdjustDriveVoltage();
            token.ThrowIfCancellationRequested();

            AppendLog("INFO", null, "[步骤2] 设置输出模式 + 钳位 + 打开通道输出");
            _driver.SetBoardClamp(channel, source, direction);
            token.ThrowIfCancellationRequested();

            AppendLog("INFO", null, $"[步骤3] 阶梯上电到目标值 {setValue}（{stepCount} 步）");
            _driver.SetBoardOnPower(channel, source, method, stepCount, setValue);
            token.ThrowIfCancellationRequested();

            AppendLog("INFO", null, $"[步骤4] 保持 {durationSec}s （请在此期间观察示波器波形）");
            RunHold(durationSec, token);

            AppendLog("INFO", null, $"[步骤5] 阶梯下电（{stepCount} 步）");
            _driver.SetBoardOFFPower(channel, source, method, stepCount, setValue);

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

            int stepCount = (int)numStep.Value;
            Task.Run(() =>
            {
                try
                {
                    var source = rdoVoltage.Checked
                        ? BoardDriverEnum.SourceType.VoltageSource
                        : BoardDriverEnum.SourceType.CurrentSource;
                    double.TryParse(txtValue.Text.Trim(), out double v);
                    var method = stepCount <= 1 ? BoardDriverEnum.PowerMethod.Single : BoardDriverEnum.PowerMethod.Step;
                    AppendLog("INFO", null, $"[紧急下电] 阶梯下电（{stepCount} 步）+ 关通道");
                    _driver.SetBoardOFFPower(channel, source, method, stepCount, v);
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
            numStep.Enabled = !testing;
            if (!testing)
            {
                progressBar.Value = 0;
                lblCountdown.Text = "空闲";
            }
        }

        #endregion

        #region 通用收发

        private void InitializeTransportControls()
        {
            lblTransport = new Label
            {
                AutoSize = true,
                Left = 600,
                Top = 30,
                Text = "方式:"
            };
            cmbTransport = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 90,
                Left = 645,
                Top = 26
            };
            cmbTransport.Items.AddRange(new object[] { "TCP", "Serial" });
            cmbTransport.SelectedIndex = 0;
            cmbTransport.SelectedIndexChanged += (s, e) => UpdateConnectUiByTransport();

            lblCom = new Label
            {
                AutoSize = true,
                Left = 15,
                Top = 30,
                Text = "COM:",
                Visible = false
            };
            cmbCom = new ComboBox
            {
                Width = 130,
                Left = 45,
                Top = 27,
                Visible = false
            };
            lblBaud = new Label
            {
                AutoSize = true,
                Left = 190,
                Top = 30,
                Text = "Baud:",
                Visible = false
            };
            txtBaud = new TextBox
            {
                Width = 65,
                Left = 230,
                Top = 27,
                Text = "115200",
                Visible = false
            };
            btnRefreshCom = new Button
            {
                Width = 60,
                Left = 305,
                Top = 25,
                Text = "刷新",
                Visible = false
            };
            btnRefreshCom.Click += (s, e) => RefreshComPorts();

            grpConnect.Controls.Add(lblTransport);
            grpConnect.Controls.Add(cmbTransport);
            grpConnect.Controls.Add(lblCom);
            grpConnect.Controls.Add(cmbCom);
            grpConnect.Controls.Add(lblBaud);
            grpConnect.Controls.Add(txtBaud);
            grpConnect.Controls.Add(btnRefreshCom);
            lblStatus.Left = 745;
        }

        private void InitializeRawCommandControls()
        {
            txtHexCmd = new TextBox
            {
                Width = 520,
                Left = 300,
                Top = 6,
                Text = "AA 55"
            };
            btnSendHex = new Button
            {
                Width = 80,
                Left = 830,
                Top = 5,
                Text = "发送HEX",
                Enabled = false
            };
            btnSendHex.Click += btnSendHex_Click;
            pnlLogButtons.Controls.Add(txtHexCmd);
            pnlLogButtons.Controls.Add(btnSendHex);
        }

        private void RefreshComPorts()
        {
            if (cmbCom == null) return;
            string current = cmbCom.Text;
            cmbCom.Items.Clear();
            foreach (string name in SerialPort.GetPortNames())
            {
                cmbCom.Items.Add(name);
            }
            if (cmbCom.Items.Count > 0)
            {
                int idx = current.Length > 0 ? cmbCom.Items.IndexOf(current) : -1;
                cmbCom.SelectedIndex = idx >= 0 ? idx : 0;
            }
        }

        private void UpdateConnectUiByTransport()
        {
            bool tcp = GetTransportKind() == TransportKind.Tcp;
            txtIp.Visible = tcp;
            txtPort.Visible = tcp;
            lblIp.Visible = tcp;
            lblPort.Visible = tcp;
            numAddress.Visible = tcp;
            lblAddress.Visible = tcp;

            lblCom.Visible = !tcp;
            cmbCom.Visible = !tcp;
            lblBaud.Visible = !tcp;
            txtBaud.Visible = !tcp;
            btnRefreshCom.Visible = !tcp;
        }

        private TransportKind GetTransportKind()
        {
            if (cmbTransport == null || cmbTransport.SelectedIndex <= 0) return TransportKind.Tcp;
            return TransportKind.Serial;
        }

        private async void btnSendHex_Click(object sender, EventArgs e)
        {
            if (_busy)
            {
                MessageBox.Show(this, "测试进行中，暂不支持手动发包。", "提示");
                return;
            }
            if (!TryParseHex(txtHexCmd.Text, out var frame, out var err))
            {
                MessageBox.Show(this, "HEX 格式错误: " + err, "提示");
                return;
            }
            try
            {
                AppendLog("TX", frame, "手动发送");
                byte[] rx = await Task.Run(() => SendAndReceiveRaw(frame));
                AppendLog("RX", rx, "手动接收");
            }
            catch (Exception ex)
            {
                AppendLog("ERR", null, "手动发送失败: " + ex.Message);
            }
        }

        private byte[] SendAndReceiveRaw(byte[] frame)
        {
            if (GetTransportKind() == TransportKind.Tcp)
            {
                if (_tcpClient == null || !_tcpClient.Connected) throw new Exception("TCP 未连接");
                NetworkStream stream = _tcpClient.GetStream();
                if (stream.DataAvailable)
                {
                    byte[] discard = new byte[_tcpClient.Available];
                    stream.Read(discard, 0, discard.Length);
                }
                stream.Write(frame, 0, frame.Length);
                return ReadProtocolFrame(() => _tcpClient.Available, () => stream.DataAvailable, count =>
                {
                    byte[] buf = new byte[count];
                    int r = stream.Read(buf, 0, buf.Length);
                    if (r == buf.Length) return buf;
                    byte[] cut = new byte[r];
                    Array.Copy(buf, cut, r);
                    return cut;
                });
            }

            if (_serialPort == null || !_serialPort.IsOpen) throw new Exception("串口未连接");
            _serialPort.DiscardInBuffer();
            _serialPort.Write(frame, 0, frame.Length);
            return ReadProtocolFrame(() => _serialPort.BytesToRead, () => _serialPort.BytesToRead > 0, count =>
            {
                byte[] buf = new byte[count];
                int r = _serialPort.Read(buf, 0, count);
                if (r == buf.Length) return buf;
                byte[] cut = new byte[r];
                Array.Copy(buf, cut, r);
                return cut;
            });
        }

        private byte[] ReadProtocolFrame(Func<int> availableGetter, Func<bool> hasData, Func<int, byte[]> readFn)
        {
            int timeoutMs = DefaultIoTimeoutMs;
            var mem = new System.Collections.Generic.List<byte>();
            int targetLen = -1;
            int elapsed = 0;
            while (elapsed < timeoutMs)
            {
                if (hasData())
                {
                    int avail = availableGetter();
                    if (avail > 0)
                    {
                        byte[] part = readFn(avail);
                        mem.AddRange(part);
                        if (targetLen < 0 && mem.Count >= 4 && mem[0] == 0xAA && mem[1] == 0x55)
                        {
                            targetLen = mem[2] | (mem[3] << 8);
                            targetLen += 2; // 0A 0D
                        }
                        if (targetLen > 0 && mem.Count >= targetLen)
                        {
                            return mem.GetRange(0, targetLen).ToArray();
                        }
                    }
                }
                Thread.Sleep(2);
                elapsed += 2;
            }
            if (mem.Count == 0) throw new TimeoutException("接收超时，无响应");
            return mem.ToArray();
        }

        private static bool TryParseHex(string text, out byte[] bytes, out string error)
        {
            bytes = null;
            error = null;
            string[] parts = (text ?? string.Empty).Replace(",", " ").Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                error = "为空";
                return false;
            }
            var list = new System.Collections.Generic.List<byte>(parts.Length);
            foreach (string p in parts)
            {
                string s = p.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? p.Substring(2) : p;
                if (!byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out byte b))
                {
                    error = p;
                    return false;
                }
                list.Add(b);
            }
            bytes = list.ToArray();
            return true;
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
            CloseConnection();
        }

        #endregion
    }
}
