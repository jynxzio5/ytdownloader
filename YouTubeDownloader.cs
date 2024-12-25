using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace YouTubeDownloader
{
    public partial class MainForm : Form
    {
        private readonly string downloadPath;
        private bool isDownloading = false;

        public MainForm()
        {
            // Set download path
            downloadPath = Path.Combine(Path.GetPathRoot(Environment.SystemDirectory), "Downloads", "ytdownloads");
            Directory.CreateDirectory(downloadPath);
            InitializeComponents();
            CheckYtDlp();
        }

        private void CheckYtDlp()
        {
            if (!File.Exists("yt-dlp.exe"))
            {
                MessageBox.Show(
                    "يرجى تحميل yt-dlp.exe ووضعه في نفس مجلد البرنامج\nيمكنك تحميله من:\nhttps://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe",
                    "ملف مطلوب",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void InitializeComponents()
        {
            // Form settings
            this.Text = "تحميل فيديوهات يوتيوب";
            this.Size = new Size(800, 600);
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 11F);
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Title Label
            var titleLabel = new Label
            {
                Text = "تحميل فيديوهات يوتيوب",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            titleLabel.Location = new Point((this.ClientSize.Width - titleLabel.PreferredWidth) / 2, 30);

            // URL Input
            var urlPanel = new Panel
            {
                Width = 700,
                Height = 50,
                Location = new Point(50, 120),
                BackColor = Color.FromArgb(45, 45, 45),
                Padding = new Padding(10)
            };

            var urlTextBox = new TextBox
            {
                Width = 680,
                Height = 30,
                Location = new Point(10, 10),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 12F)
            };
            urlPanel.Controls.Add(urlTextBox);

            // Download Type
            var typeComboBox = new ComboBox
            {
                Width = 200,
                Height = 40,
                Location = new Point(550, 200),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            typeComboBox.Items.AddRange(new string[] { "فيديو", "صوت فقط" });
            typeComboBox.SelectedIndex = 0;

            // Quality Selection
            var qualityComboBox = new ComboBox
            {
                Width = 200,
                Height = 40,
                Location = new Point(50, 200),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            qualityComboBox.Items.AddRange(new string[] { "1080p", "720p", "480p" });
            qualityComboBox.SelectedIndex = 0;

            // Progress Bar
            var progressBar = new ProgressBar
            {
                Width = 700,
                Height = 30,
                Location = new Point(50, 300),
                Style = ProgressBarStyle.Continuous,
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.FromArgb(220, 53, 69)
            };

            // Status Label
            var statusLabel = new Label
            {
                AutoSize = true,
                Location = new Point(50, 340),
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10F)
            };

            // Download Button
            var downloadButton = new Button
            {
                Text = "تحميل",
                Width = 200,
                Height = 50,
                Location = new Point(300, 400),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            downloadButton.FlatAppearance.BorderSize = 0;
            downloadButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 33, 49);

            // Event handlers
            downloadButton.Click += async (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(urlTextBox.Text))
                {
                    MessageBox.Show("الرجاء إدخال رابط صحيح", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!File.Exists("yt-dlp.exe"))
                {
                    MessageBox.Show("الرجاء تحميل yt-dlp.exe أولاً", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (isDownloading) return;

                isDownloading = true;
                downloadButton.Enabled = false;
                progressBar.Style = ProgressBarStyle.Marquee;
                statusLabel.Text = "جاري التحميل...";

                try
                {
                    string format;
                    string outputTemplate;
                    string arguments;

                    if (typeComboBox.SelectedItem.ToString() == "صوت فقط")
                    {
                        format = "bestaudio/best";
                        outputTemplate = Path.Combine(downloadPath, "%(title)s.%(ext)s");
                        arguments = $"-f {format} --extract-audio --audio-format mp3 --audio-quality 192k --no-keep-video -o \"{outputTemplate}\" \"{urlTextBox.Text}\"";
                    }
                    else
                    {
                        format = qualityComboBox.SelectedIndex switch
                        {
                            0 => "best[height<=1080][ext=mp4]/bestvideo[height<=1080][ext=mp4]+bestaudio[ext=m4a]",
                            1 => "best[height<=720][ext=mp4]/bestvideo[height<=720][ext=mp4]+bestaudio[ext=m4a]",
                            _ => "best[height<=480][ext=mp4]/bestvideo[height<=480][ext=mp4]+bestaudio[ext=m4a]"
                        };
                        outputTemplate = Path.Combine(downloadPath, "%(title)s.mp4");
                        arguments = $"-f {format} -o \"{outputTemplate}\" \"{urlTextBox.Text}\"";
                    }

                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "yt-dlp.exe",
                            Arguments = arguments,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true
                        }
                    };

                    var tcs = new TaskCompletionSource<bool>();

                    process.OutputDataReceived += (sender, args) =>
                    {
                        if (args.Data != null)
                            this.BeginInvoke(() => statusLabel.Text = args.Data);
                    };

                    process.ErrorDataReceived += (sender, args) =>
                    {
                        if (args.Data != null)
                            this.BeginInvoke(() => statusLabel.Text = args.Data);
                    };

                    process.Exited += (sender, args) => tcs.SetResult(true);
                    process.EnableRaisingEvents = true;

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromMinutes(5)));

                    if (process.HasExited)
                    {
                        if (process.ExitCode == 0)
                        {
                            statusLabel.Text = "تم التحميل بنجاح!";
                            var result = MessageBox.Show($"تم التحميل بنجاح!\nهل تريد فتح مجلد التنزيلات؟", "نجاح", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if (result == DialogResult.Yes)
                            {
                                Process.Start("explorer.exe", downloadPath);
                            }
                        }
                        else
                        {
                            throw new Exception("فشل التحميل");
                        }
                    }
                    else
                    {
                        process.Kill();
                        throw new Exception("انتهت مهلة التحميل");
                    }
                }
                catch (Exception ex)
                {
                    statusLabel.Text = "فشل التحميل!";
                    MessageBox.Show($"حدث خطأ أثناء التحميل: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    isDownloading = false;
                    downloadButton.Enabled = true;
                    progressBar.Style = ProgressBarStyle.Continuous;
                }
            };

            // Add controls to form
            this.Controls.AddRange(new Control[] {
                titleLabel,
                urlPanel,
                typeComboBox,
                qualityComboBox,
                progressBar,
                statusLabel,
                downloadButton
            });
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
} 