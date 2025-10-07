using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientApp
{
    public partial class MainForm : Form
    {
        private UdpClient? udpClient;
        private CancellationTokenSource? cts;

        // Multicast groups
        private readonly IPAddress[] multicastAddresses = new[]
        {
            IPAddress.Parse("239.0.0.1"),
            IPAddress.Parse("239.0.0.2"),
            IPAddress.Parse("239.0.0.3")
        };

        private const int ListenPort = 5000;

        public MainForm()
        {
            InitializeComponent();
            lblStatus.Text = "Не підключено";
            lblStatus.ForeColor = System.Drawing.Color.Black;
            lblEmergency.Visible = false;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (cts != null && !cts.IsCancellationRequested)
                return; // all connected

            try
            {
                // Preparing sokcets
                udpClient = new UdpClient();
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpClient.ExclusiveAddressUse = false;
                udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, ListenPort));

                // Joining multicast
                foreach (var maddr in multicastAddresses)
                {
                    try
                    {
                        udpClient.JoinMulticastGroup(maddr);
                    }
                    catch (Exception ex)
                    {
                       
                        AppendMessage($"[System] Не вдалося приєднатися до {maddr}: {ex.Message}", false);
                    }
                }

                cts = new CancellationTokenSource();
                Task.Run(() => ReceiveLoop(cts.Token), cts.Token);

                lblStatus.Text = "Підключено, очікування повідомлень...";
                lblStatus.ForeColor = System.Drawing.Color.Green;
                btnConnect.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не вдалося відкрити сокет: " + ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReceiveLoop(CancellationToken token)
        {
            var remoteEP = new IPEndPoint(IPAddress.Any, 0);

            while (!token.IsCancellationRequested)
            {
                try
                {
                    byte[] data = udpClient!.Receive(ref remoteEP);
                    string message = Encoding.UTF8.GetString(data);

                    bool isEmergency = message.ToUpperInvariant().Contains("ЕКСТРЕНЕ") ||
                                       message.ToUpperInvariant().Contains("EMERGENCY");

                    bool show = false;

                    if (isEmergency)
                    {
                        show = true;
                    }
                    else
                    {
                        
                        bool news = false, announcements = false, technical = false;
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() =>
                            {
                                news = chkNews.Checked;
                                announcements = chkAnnouncements.Checked;
                                technical = chkTechnical.Checked;
                            }));
                        }
                        else
                        {
                            news = chkNews.Checked;
                            announcements = chkAnnouncements.Checked;
                            technical = chkTechnical.Checked;
                        }

                        if (news && message.Contains("Новини")) show = true;
                        if (announcements && message.Contains("Оголошення")) show = true;
                        if (technical && message.Contains("Технічні")) show = true;
                    }

                    if (show)
                        AppendMessage(message, isEmergency);
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (SocketException se)
                {
                    if (token.IsCancellationRequested) break;
                    AppendMessage("[System] Socket error: " + se.Message, false);
                }
                catch (Exception ex)
                {
                    AppendMessage("[System] Receive error: " + ex.Message, false);
                }
            }
        }

        private void AppendMessage(string msg, bool isEmergency)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AppendMessage(msg, isEmergency)));
                return;
            }

            if (isEmergency)
            {
                lblEmergency.Visible = true;
                lblEmergency.Text = " !!EMERGENCY MESSAGE!!";
                lblEmergency.ForeColor = System.Drawing.Color.Red;
            }

            txtMessages.SelectionStart = txtMessages.TextLength;
            txtMessages.SelectionColor = isEmergency ? System.Drawing.Color.Red : System.Drawing.Color.Black;
            txtMessages.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}{Environment.NewLine}");
            txtMessages.SelectionColor = txtMessages.ForeColor;
            txtMessages.ScrollToCaret();
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            try
            {
                cts?.Cancel();
                udpClient?.Close();
                udpClient?.Dispose();
            }
            catch { }
        }
    }
}
