using System;

using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.IO;
using System.Diagnostics;

using System.IO.Compression;


using System.Net;
using System.Net.Sockets;
using System.Threading;
using Windows_TTS_App.Properties;

using System.Windows.Forms;


// 引用FubarDev.FtpServer和FubarDev.FtpServer.FileSystem.DotNet两个NuGet包
using FubarDev.FtpServer;
using FubarDev.FtpServer.FileSystem.DotNet;

// 引用Microsoft.Extensions.DependencyInjection和Microsoft.Extensions.Hosting两个包
using Microsoft.Extensions.DependencyInjection;


// 引用System.IO和System.Threading.Tasks两个包

using FubarDev.FtpServer.AccountManagement;

//using Microsoft.AspNet.Identity;


// 在Form类的构造函数中，初始化FTP服务端的主机对象



namespace Windows_TTS_App
{
    public partial class Form1 : Form
    {
        //private IHost _ftpHost;
        // HTTP服务
        //private HttpListener listener = new HttpListener();
        //private Thread ThreadListener = null;

        // web根目录
        //private string WebPath = AppDomain.CurrentDomain.BaseDirectory + "web\\";

        // 自定义POST处理
        public delegate string EventDo(object sender, HttpListenerRequest request);
        //public event EventDo OnEventDo;

        // 消息事件
        //public delegate void EventInfo(object sender, string info);
        //public event EventInfo OnEventInfo;

        // 最后错误信息
        //public string LastErrorInfo = "";

        // 是否调试
        //public bool IsDebug = false;

        // 触发消息
        //private void showInfo(string str)
        //{
        //    OnEventInfo(this, str);
        //}
        //主机地址
        private string hostAddress;
        //起始端口
        private int start;
        //终止端口
        private int end;
        //端口号
        private int port;
        //定义线程对象
        private Thread scanThread;
        //定义端口状态数据（开放则为true，否则为false）
        private bool[] done = new bool[65526];
        private bool OK;
        // 定义一个HttpListener对象

        // 定义一个按钮对象
        //private Button startButton;
        //private HttpListener listener;
        //private string rootDir;

        //private FtpServer _server; // FTP服务器对象


        // FTP服务器主机对象
        //private IFtpServerHost _ftpServerHost;

        // FTP服务器是否启动的标志
        //private bool _isRunning;
        private HttpListener listener;
        private string rootDirectory;
        private bool isRunning;
        private FtpServerHost _ftpServerHost;
        public Form1()
        {
            //boottime myabForm = new boottime();
            //myabForm.ShowDialog();
            InitializeComponent();
            if (Directory.Exists(@"C:\Users\Public\Documents\data"))
            {
                if (File.Exists(@"C:\Users\Public\Documents\data\api"))
                {
                    button12.Visible = true;
                    CheckForIllegalCrossThreadCalls = false;
                }
                else
                {
                    this.notifyIcon1.ShowBalloonTip(60, "应用初始化", "应用程序包无效!应用的功能受限!", ToolTipIcon.Error);
                    DirectoryInfo di = new DirectoryInfo(@"C:\Users\Public\Documents\data");
                    di.Delete(true);
                }
            }
            else
            {
                this.notifyIcon1.ShowBalloonTip(60, "应用初始化", "你没有加载应用程序包!应用的功能受限!", ToolTipIcon.Error);
            }

            listener = new HttpListener();
            isRunning = false;


            //InitializeFtpServer();
            // 设置按钮的文本
            //button34.Text = "启动";
            // 设置FTP服务器未启动
            //_isRunning = false;
        }
        SpeechSynthesizer sy = new SpeechSynthesizer();
        ServiceCollection services = new ServiceCollection();
        

        //private void InitializeFtpServer()
        //{
        //    // 设置依赖注入
        //    var services = new ServiceCollection();
        //     // 把指定的目录设为根目录，这里用当前目录作为示例
        //     services.Configure<DotNetFileSystemOptions>(opt => opt.RootPath = Directory.GetCurrentDirectory());
        //     // 添加FTP服务器服务
        //     // DotNetFileSystemProvider = 使用.NET文件系统功能
        //     // AnonymousMembershipProvider = 只允许匿名登录
        //     services.AddFtpServer(builder => builder
        //         .UseDotNetFileSystem() // 使用.NET文件系统功能
        //         .EnableAnonymousAuthentication()); // 允许匿名登录
        //
        //     // 配置FTP服务器选项
        //     services.Configure<FtpServerOptions>(opt => opt.ServerAddress = "127.0.0.1");
        //     // 构建服务提供者
        //    var serviceProvider = services.BuildServiceProvider();
        //     // 获取FTP服务器主机对象
        //     _ftpServerHost = serviceProvider.GetRequiredService<IFtpServerHost>();
        // }

        private void button33_Click(object sender, EventArgs e)
        {

            this.button33.Text = "arfaf";

        }


        

        // 处理按钮点击的方法
        
        
        private void button1_Click(object sender, EventArgs e)
        {
            sy.SpeakAsyncCancelAll();
            this.notifyIcon1.ShowBalloonTip(1, "TTS应用操作", "朗读本文中...", ToolTipIcon.Info);
            sy.SetOutputToDefaultAudioDevice();
            sy.Speak(textBox1.Text);
            this.notifyIcon1.ShowBalloonTip(20, "TTS应用操作", "朗读完毕", ToolTipIcon.Info);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            sy.SpeakAsyncCancelAll();
            string ad = textBox2.Text;
            if (ad == "")
            {
                errorProvider1.SetError(textBox2, "路径不能为空!");
                this.notifyIcon1.ShowBalloonTip(20, "Error", "路径不正确!", ToolTipIcon.Error);
            }
            else
            {
                errorProvider1.Clear();
                sy.SetOutputToWaveFile(@ad);
                sy.Speak(textBox1.Text);
                sy.SetOutputToDefaultAudioDevice();
                this.notifyIcon1.ShowBalloonTip(20, "应用操作", "成功地保存朗读音频文件", ToolTipIcon.Info);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            sy.SpeakAsyncCancelAll();
            errorProvider1.Clear();
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "音乐文件(*.mp3)|*.mp3|音乐文件(*.wav)|*.wav|所有文件|*.*";//设置文件类型
            sfd.FileName = "TTS保存音乐文件";//设置默认文件名
            sfd.DefaultExt = "mp3";//设置默认格式（可以不设）
            sfd.AddExtension = true;//设置自动在文件名中添加扩展名
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = sfd.FileName;
                //+ "\r\n";

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            sy.SpeakAsyncCancelAll();
            sy.Volume = trackBar1.Value;
            sy.Rate = trackBar2.Value;
            sy.SetOutputToDefaultAudioDevice();
            sy.SpeakAsync(textBox1.Text);
            this.notifyIcon1.ShowBalloonTip(1, "TTS应用操作", "自定义朗读本文中...", ToolTipIcon.Info);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (button6.Text == "暂停")
            {
                sy.Pause();
                this.notifyIcon1.ShowBalloonTip(1, "TTS应用操作", "朗读本文己暂停", ToolTipIcon.Info);
                button6.Text = "恢复";

            }
            else
            {
                sy.Resume();
                this.notifyIcon1.ShowBalloonTip(1, "TTS应用操作", "朗读本文己继续", ToolTipIcon.Info);
                button6.Text = "暂停";

            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            sy.SpeakAsyncCancelAll();
            if (textBox3.Text == "")
            {
                errorProvider1.SetError(textBox2, "次数不能为空!");
                this.notifyIcon1.ShowBalloonTip(20, "Error", "次数不正确!", ToolTipIcon.Error);
            }
            else
            {
                errorProvider1.Clear();
                this.notifyIcon1.ShowBalloonTip(1, "TTS应用操作", "自定义循环朗读本文中...", ToolTipIcon.Info);
                sy.Volume = trackBar1.Value;
                sy.Rate = trackBar2.Value;
                sy.SetOutputToDefaultAudioDevice();
                string s = textBox3.Text;
                int j2;
                int.TryParse(s, out j2);
                int j = Convert.ToInt32(s);
                for (int c = 0; c < j; c++)
                {
                    sy.SpeakAsync(textBox1.Text);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            sy.SpeakAsyncCancelAll();
            string ad = textBox2.Text;
            if (ad == "")
            {
                errorProvider1.SetError(textBox2, "路径不能为空!");
                this.notifyIcon1.ShowBalloonTip(20, "Error", "路径不正确!", ToolTipIcon.Error);
            }
            else
            {
                errorProvider1.Clear();
                sy.Volume = trackBar1.Value;
                sy.Rate = trackBar2.Value;
                sy.SetOutputToWaveFile(@ad);
                sy.Speak(textBox1.Text);
                sy.SetOutputToDefaultAudioDevice();
                this.notifyIcon1.ShowBalloonTip(20, "应用操作", "成功地保存朗读音频文件", ToolTipIcon.Info);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            sy.SpeakAsyncCancelAll();
            sy.Volume = trackBar1.Value;
            sy.Rate = trackBar2.Value;
            //sy.SetOutputToDefaultAudioDevice();
            sy.SpeakAsync(textBox1.Text);
            this.notifyIcon1.ShowBalloonTip(1, "应用操作", "朗读本文已重新开始", ToolTipIcon.Info);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Form2 myabForm = new Form2();
            myabForm.ShowDialog();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            sy.SpeakAsyncCancelAll();
            this.notifyIcon1.ShowBalloonTip(1, "应用操作", "朗读本文已终止", ToolTipIcon.Info);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            trackBar1.Value = 100;
            trackBar2.Value = 1;
            label4.Text = "100";
            label5.Text = "1";
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int kt = trackBar1.Value;
            string ki = kt.ToString();
            label4.Text = Convert.ToString(kt);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            int kt = trackBar2.Value;
            string ki = kt.ToString();
            label5.Text = Convert.ToString(kt);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            button13.Visible = true;
            panel1.Visible = true;
            button12.Visible = false;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            button13.Visible = false;
            panel1.Visible = false;
            button12.Visible = true;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            Form3 myabForm = new Form3();
            myabForm.ShowDialog();

        }

        private void button16_Click(object sender, EventArgs e)
        {
            //运行Minecraft
            panel2.Visible = true;
            label6.Visible = true;
            label7.Visible = true;
            label8.Visible = true;
            label9.Visible = true;
            label10.Visible = true;
            button19.Visible = true;
            button20.Visible = true;
            textBox4.Visible = true;
            textBox5.Visible = true;
            textBox6.Visible = true;
            textBox7.Visible = true;
            button22.Visible = true;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            //下载
            panel2.Visible = true;
            textBox11.Visible = true;
            textBox12.Visible = true;
            button21.Visible = true;
            webBrowser1.Visible = true;
        }


        private void button19_Click(object sender, EventArgs e)
        {
            //自定义运行Minecraft 1.19
            if (textBox4.Text == "")
            {
                errorProvider1.SetError(textBox4, "最小内存不能为空!");
                this.notifyIcon1.ShowBalloonTip(20, "Error", "最小内存不能为空!", ToolTipIcon.Error);
            }
            else
            {
                errorProvider1.Clear();
            }
            if (textBox5.Text == "")
            {
                errorProvider1.SetError(textBox5, "最大内存不能为空!");
                this.notifyIcon1.ShowBalloonTip(20, "Error", "最大内存不能为空!", ToolTipIcon.Error);
            }
            else
            {
                errorProvider1.Clear();
            }
            if (textBox6.Text == "")
            {
                errorProvider1.SetError(textBox6, "用户名不能为空!");
                this.notifyIcon1.ShowBalloonTip(20, "Error", "用户名不能为空!", ToolTipIcon.Error);
            }
            else
            {
                errorProvider1.Clear();
            }
            if (textBox7.Text == "")
            {
                errorProvider1.SetError(textBox7, "用户ID不能为空!");
                this.notifyIcon1.ShowBalloonTip(20, "Error", "用户ID不能为空!", ToolTipIcon.Error);
            }
            else
            {
                errorProvider1.Clear();
            }
            string rm = @"/c C:\Users\Public\Documents\data\run.bat C:\Users\Public\Documents\ 1.19.2 1.19 C:\Users\Public\Documents\.minecraft\java " + textBox6.Text + textBox7.Text + textBox4.Text + textBox5.Text;
            if (Directory.Exists(@"C:\Users\Public\Documents\.minecraft"))
            {
                this.notifyIcon1.ShowBalloonTip(20, "应用操作", "请不要退出!正在运行Minecraft中...", ToolTipIcon.Warning);
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = rm;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                p.StandardOutput.ReadToEnd();
                this.notifyIcon1.ShowBalloonTip(20, "应用操作", "己退出Minecraft!", ToolTipIcon.Info);
            }
            else
            {
                errorProvider1.Clear();
                this.notifyIcon1.ShowBalloonTip(20, "安装", "你没有安装Minecraft!正在为你安装Minecraft。请不要退出!", ToolTipIcon.Warning);
                //-----------------------------------------------------------------
                ZipFile.ExtractToDirectory(@"C:\Users\Public\Documents\data\.minecraft.zip", @"C:\Users\Public\Documents\");
                this.notifyIcon1.ShowBalloonTip(20, "应用操作", "己安装Minecraft!正在启动Minecraft!请不要退出!", ToolTipIcon.Warning);
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = rm;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                p.StandardOutput.ReadToEnd();
                this.notifyIcon1.ShowBalloonTip(20, "应用操作", "己退出Minecraft!", ToolTipIcon.Info);

            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            //还原
            textBox4.Text = "125";
            textBox5.Text = "2000";
            textBox6.Text = "officemc";
            textBox7.Text = "0000000000003004998F501A96EE8C3B";


        }

        private void button17_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel2.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
            label10.Visible = false;
            button19.Visible = false;
            button20.Visible = false;
            textBox4.Visible = false;
            textBox5.Visible = false;
            textBox6.Visible = false;
            textBox7.Visible = false;
            textBox11.Visible = false;
            textBox12.Visible = false;
            button21.Visible = false;
            webBrowser1.Visible = false;
            button22.Visible = false;
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button21_Click(object sender, EventArgs e)
        {
            string songLyrics = textBox12.Text;
            bool u = songLyrics.Contains("http://");
            bool p = songLyrics.Contains("https://");
            if (u == false)
            {
                if (p == false)
                {
                    textBox12.Text = "http://" + textBox12.Text;
                }
                else
                {
                    webBrowser1.Url = new Uri(textBox12.Text);
                }
            }
            else
            {
                webBrowser1.Url = new Uri(textBox12.Text);
            }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            textBox11.Text = webBrowser1.Url.ToString();

        }

        private void button22_Click(object sender, EventArgs e)
        {
            //删除Minecraft 1.19
            if (Directory.Exists(@"C:\Users\Public\Documents\.minecraft"))
            {
                this.notifyIcon1.ShowBalloonTip(1, "应用操作", "Minecraft正在删除", ToolTipIcon.Warning);
                File.Delete(@"C:\Users\Public\Documents\.minecraft");
                this.notifyIcon1.ShowBalloonTip(1, "应用操作", "Minecraft已删除", ToolTipIcon.Info);
            }
            else
            {
                this.notifyIcon1.ShowBalloonTip(1, "应用操作", "Minecraft没有安装", ToolTipIcon.Warning);
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == System.Windows.Forms.FormWindowState.Minimized)
                this.WindowState = System.Windows.Forms.FormWindowState.Normal;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 打开主界面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.WindowState == System.Windows.Forms.FormWindowState.Minimized)
                this.WindowState = System.Windows.Forms.FormWindowState.Normal;
        }

        private void 开始下载ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string songLyrics = toolStripTextBox1.Text;
            bool u = songLyrics.Contains("http://");
            bool p = songLyrics.Contains("https://");
            if (u == false)
            {
                if (p == false)
                {
                    toolStripTextBox1.Text = "http://" + toolStripTextBox1.Text;
                }
                else
                {
                    webBrowser1.Url = new Uri(toolStripTextBox1.Text);
                }
            }
            else
            {
                webBrowser1.Url = new Uri(toolStripTextBox1.Text);
            }
        }

        private void 开始朗读ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sy.SpeakAsyncCancelAll();
            sy.Volume = trackBar1.Value;
            sy.Rate = trackBar2.Value;
            sy.SetOutputToDefaultAudioDevice();
            sy.SpeakAsync(toolStripTextBox2.Text);
            this.notifyIcon1.ShowBalloonTip(1, "TTS应用操作", "自定义朗读本文中...", ToolTipIcon.Info);
        }

        private void 终止ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sy.SpeakAsyncCancelAll();
            this.notifyIcon1.ShowBalloonTip(1, "应用操作", "朗读本文已终止", ToolTipIcon.Info);
        }

        private void 停止ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sy.Pause();
            this.notifyIcon1.ShowBalloonTip(1, "TTS应用操作", "朗读本文己暂停", ToolTipIcon.Info);
        }

        private void 继续ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sy.Resume();
            this.notifyIcon1.ShowBalloonTip(1, "TTS应用操作", "朗读本文己继续", ToolTipIcon.Info);
        }

        private void 运行Minecraft119ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(@"C:\Users\Public\Documents\.minecraft"))
            {
                this.notifyIcon1.ShowBalloonTip(20, "应用操作", "请不要退出!正在运行Minecraft中...", ToolTipIcon.Info);
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = @"/c C:\Users\Public\Documents\data\run.bat C:\Users\Public\Documents\ 1.19.2 1.19 C:\Users\Public\Documents\.minecraft\java " + textBox6.Text + textBox7.Text + textBox4.Text + textBox5.Text;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                p.StandardOutput.ReadToEnd();
                this.notifyIcon1.ShowBalloonTip(20, "应用操作", "己退出Minecraft!", ToolTipIcon.Info);
            }
            else
            {
                errorProvider1.Clear();
                this.notifyIcon1.ShowBalloonTip(20, "安装", "你没有安装Minecraft!正在为你安装Minecraft。请不要退出!", ToolTipIcon.Warning);
                //------------------------------------------------------------------------
                ZipFile.ExtractToDirectory(@"C:\Users\Public\Documents\data\.minecraft.zip", @"C:\Users\Public\Documents\");
                this.notifyIcon1.ShowBalloonTip(20, "应用操作", "己安装Minecraft!正在启动Minecraft!请不要退出!", ToolTipIcon.Info);
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = @"/c C:\Users\Public\Documents\data\run.bat C:\Users\Public\Documents\ 1.19.2 1.19 C:\Users\Public\Documents\.minecraft\java " + textBox6.Text + textBox7.Text + textBox4.Text + textBox5.Text;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                p.StandardOutput.ReadToEnd();
                this.notifyIcon1.ShowBalloonTip(20, "应用操作", "己退出Minecraft!", ToolTipIcon.Info);

            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            sy.Resume();
        }

        private void button25_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.SaveFileDialog objSave = new System.Windows.Forms.SaveFileDialog();
            objSave.Filter = "文本文件(*.txt)|*.txt|" + "所有文件(*.*)|*.*";

            objSave.FileName = "文本" + DateTime.Now.ToString("yyyyMMddHHmm") + ".txt";

            if (objSave.ShowDialog() == DialogResult.OK)
            {
                StreamWriter FileWriter = new StreamWriter(objSave.FileName, true); //写文件

                FileWriter.Write(this.textBox1.Text);//将字符串写入
                FileWriter.Close(); //关闭StreamWriter对象
            }
            this.notifyIcon1.ShowBalloonTip(20, "应用操作", "己导出文本文件!", ToolTipIcon.Info);
        }

        private void button26_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "打开";
            openFileDialog.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != "")//判断文件名是否为空
            {
                //StreamReader streamReader = new StreamReader(openFileDialog.FileName, Encoding.Default);
                //this.textBox1.Text = streamReader.ReadToEnd();
                using (var sr = new StreamReader(openFileDialog.FileName))
                {
                    this.textBox1.Text = sr.ReadToEnd();
                }
            }
            this.notifyIcon1.ShowBalloonTip(20, "应用操作", "己导入文本文件!", ToolTipIcon.Info);
        }

        private void button24_Click(object sender, EventArgs e)
        {
            //setting
            //Form4 myabForm = new Form4();
            //myabForm.ShowDialog();
            panel2.Visible = true;
            panel3.Visible = true;
        }

        private void button23_Click_1(object sender, EventArgs e)
        {
            textBox1.Text = "";

        }

        private void button27_Click(object sender, EventArgs e)
        {

            if (Directory.Exists(@"C:\Users\Public\Documents\data"))
            {
                DirectoryInfo di = new DirectoryInfo(@"C:\Users\Public\Documents\data");
                di.Delete(true);
            }
            this.Close();
        }

        private void button28_Click(object sender, EventArgs e)
        {
            //web
            Form5 myabForm = new Form5();
            myabForm.ShowDialog();
        }

        private void button29_Click(object sender, EventArgs e)
        {
            panel3.Visible = false;
            panel2.Visible = false;
        }

        private void button30_Click(object sender, EventArgs e)
        {
            try
            {
                //初始化
                tbShow.Clear();
                lb.Text = "0%";

                //获取ip地址和始末端口号
                hostAddress = tbHost.Text;
                start = Int32.Parse(tbSPort.Text);
                end = Int32.Parse(tbEPort.Text);

                if (decideAddress()) // 端口合理
                {
                    if (tbHost.ReadOnly == false)
                    {
                        //让输入的textbox只读，无法改变
                        tbHost.ReadOnly = true;
                        tbSPort.ReadOnly = true;
                        tbEPort.ReadOnly = true;
                        errorProvider1.Clear();
                        //创建线程，并创建ThreadStart委托对象
                        Thread process = new Thread(new ThreadStart(PortScan));
                        process.Start();
                        //设置进度条的范围
                        pb.Minimum = start;
                        pb.Maximum = end;

                        //显示框显示
                        tbShow.AppendText("扫描 " + tbHost.Text + " 端口" + Environment.NewLine + Environment.NewLine);
                    }
                    else
                    {
                        MessageBox.Show("端口扫描已开始", "端口扫描", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    //若端口号不合理，弹窗报错
                    MessageBox.Show("端口输入错误，端口范围为[0-65536]!", "端口扫描", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    errorProvider1.SetError(tbSPort, "端口输入错误，端口范围为[0-65536]!");
                    errorProvider1.SetError(tbEPort, "端口输入错误，端口范围为[0-65536]!");
                    this.notifyIcon1.ShowBalloonTip(20, "端口扫描Error", "端口输入错误，端口范围为[0-65536]!", ToolTipIcon.Error);
                }

            }
            catch
            {
                //若输入的端口号为非整型，则弹窗报错
                MessageBox.Show("端口输入错误，端口号为非整型!", "端口扫描", MessageBoxButtons.OK, MessageBoxIcon.Error);
                errorProvider1.SetError(tbSPort, "端口输入错误，端口号为非整型!");
                errorProvider1.SetError(tbEPort, "端口输入错误，端口号为非整型!");
                this.notifyIcon1.ShowBalloonTip(20, "端口扫描Error", "端口输入错误，端口号为非整型！", ToolTipIcon.Error);
            }
        }

        /// <summary>
        /// 判断端口是否合理
        /// </summary>
        /// <returns></returns>
        private bool decideAddress()
        {
            //判断端口号是否合理
            if ((start >= 0 && start <= 65536) && (end >= 0 && end <= 65536) && (start <= end))
                return true;
            else
                return false;
        }


        private void PortScan()
        {
            double x;
            string xian;
            //显示扫描状态
            tbShow.AppendText("---------------开始扫描 " + tbHost.Text + " ---------------" + Environment.NewLine + Environment.NewLine);
            //循环抛出线程扫描端口
            for (int i = start; i <= end; i++)
            {
                x = (double)(i - start + 1) / (end - start + 1);
                xian = x.ToString("0%");
                port = i;
                //使用该端口的扫描线程
                scanThread = new Thread(new ThreadStart(Scan));
                scanThread.Start();
                //使线程睡眠
                System.Threading.Thread.Sleep(100);
                //进度条值改变
                lb.Text = xian;
                pb.Value = i;
            }
            while (!OK)
            {
                OK = true;
                for (int i = start; i <= end; i++)
                {
                    if (!done[i])
                    {
                        OK = false;
                        break;
                    }
                }
                System.Threading.Thread.Sleep(1000);
            }
            tbShow.AppendText(Environment.NewLine + "---------------扫描结束---------------" + Environment.NewLine);
            //输入框textbox只读属性取消
            tbHost.ReadOnly = false;
            tbSPort.ReadOnly = false;
            tbEPort.ReadOnly = false;
        }

        /// <summary>
        /// 扫描某个端口
        /// </summary>
        private void Scan()
        {
            int portnow = port;
            //创建线程变量
            Thread Threadnow = scanThread;
            //扫描端口，成功则写入信息
            done[portnow] = true;
            //创建TcpClient对象，TcpClient用于为TCP网络服务提供客户端连接
            TcpClient objTCP = null;
            try
            {
                //用于TcpClient对象扫描端口
                objTCP = new TcpClient(hostAddress, portnow);
                //扫描到则显示到显示框
                tbShow.AppendText(tbHost.Text + ":" + port + " 开放！" + Environment.NewLine);
            }
            catch
            {
                //未扫描到，则会抛出错误
            }
        }

        private void button31_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.SaveFileDialog objSave = new System.Windows.Forms.SaveFileDialog();
            objSave.Filter = "文本文件(*.txt)|*.txt|" + "所有文件(*.*)|*.*";

            objSave.FileName = "文本" + DateTime.Now.ToString("yyyyMMddHHmm") + ".txt";

            if (objSave.ShowDialog() == DialogResult.OK)
            {
                StreamWriter FileWriter = new StreamWriter(objSave.FileName, true); //写文件

                FileWriter.Write(this.tbShow.Text);//将字符串写入
                FileWriter.Close(); //关闭StreamWriter对象
            }
            this.notifyIcon1.ShowBalloonTip(20, "端口扫描应用操作", "己导出端口扫描输出结果!", ToolTipIcon.Info);
        }
        HttpListener httpListener = new HttpListener();
        //static HttpListener sSocket = null;
        //string prefix;
        //private static string _listenerUri = ConfigurationManager.AppSettings["ListenerUri"];
        //static HttpListener httpobj;
        private void button36_Click(object sender, EventArgs e)
        {
            //httpListener.Prefixes.Add(prefix);
            httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            
            httpListener.Prefixes.Add("http://localhost:8080/");
            
            httpListener.Start();
            httpListener.BeginGetContext(new AsyncCallback(GetContextCallBack), httpListener);
        }
        
        static void GetContextCallBack(IAsyncResult ar)
        {
            try
            {
                string Path = $"{System.Environment.CurrentDirectory}";
                HttpListener httpListener = ar.AsyncState as HttpListener;
                HttpListenerContext httpListenerContext = httpListener.EndGetContext(ar);
                httpListenerContext.Response.StatusCode = 200;
                //StreamWriter writer = new StreamWriter(httpListenerContext.Response.OutputStream);
                HttpListenerResponse response = httpListenerContext.Response;
                var htuel = httpListenerContext.Request.Url.AbsolutePath.Trim('/');

                var ph = Path + @"\" + htuel;
                var pz = System.IO.Path.GetExtension(@ph);


                //response.ContentLength64= System.Text.Encoding.UTF8.GetByteCount(htuel);
                response.ContentType = "text/type; charset=UTF-8";
                System.IO.Stream output = response.OutputStream;
                System.IO.StreamWriter writer = new System.IO.StreamWriter(output);
                //writer.WriteLineAsync("<h1>" + htuel + "</h1>");
                //writer.WriteAsync(htuel);
                
                
                if (!File.Exists(@ph))
                {writer.Write("<h1>404:"+ @ph+"</h1>");
                }
                    
                else
                {
                    
                    using (var sr = new StreamReader(@ph))
                    {
                        writer.Write(sr.ReadToEnd());
                    }
                    //writer.Write(pz);
                }
                //writer.WriteLineAsync("<h1>"+htuel+"</h1>");
                writer.Close();
                //notifyIcon1.ShowBalloonTip(20, "http用户请求", htuel, ToolTipIcon.Info);
                httpListener.BeginGetContext(new AsyncCallback(GetContextCallBack), httpListener);
            }
            catch { };
        }
        private void button37_Click_1(object sender, EventArgs e)
        {
            
            if (httpListener.IsListening)
            {
                httpListener.Stop();
            }
            //httpListener.Close();
            //httpListener.Stop();
            
        }
        //private CancellationTokenSource cts;
        private async void button18_Click(object sender, EventArgs e)
        {
            //cts = new CancellationTokenSource();
            //while (!cts.IsCancellationRequested)
            {
                //----------------------------------------------
                //{
                //    // Do something here
                //    //
                //    //1、创建一个用于监听连接的Socket对象
                //    Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //    //2、绑定IP地址和端口号
                //    IPAddress ip = IPAddress.Parse(textBox9.Text);
                //    string str = textBox10.Text;
                //    int port = int.Parse(str);
                //    IPEndPoint ipe = new IPEndPoint(ip, port);
                //    serverSocket.Bind(ipe);
                //    //3、设置最大连接数
                //    serverSocket.Listen(10);
                //    Console.WriteLine("等待客户端连接...");
                //    //4、接收客户端连接
                //    Socket clientSocket = serverSocket.Accept();
                //    Console.WriteLine("客户端已连接！");
                //    //5、接收客户端发送的文件
                //    //设置保存文件的目录
                //    //string currentDirectory = System.Environment.CurrentDirectory;
                //    string savePath = System.Environment.CurrentDirectory;
                //    //获取文件名
                //    string fileName = Path.GetFileName("tem");
                //    //合并路径
                //    string filePath = Path.Combine(savePath, fileName);
                //    //创建文件流
                //    FileStream fs = new FileStream(filePath, FileMode.Create);
                //    //接收数据
                //    int receivedBytesLen = 0;
                //    byte[] receivedBytes = new byte[1024];
                //    while ((receivedBytesLen = clientSocket.Receive(receivedBytes)) > 0)
                //    {
                //        fs.Write(receivedBytes, 0, receivedBytesLen);
                //    }
                //    fs.Close();
                //
                //    //File.Exists(Application.StartupPath + @"\data.ini"
                //    //byte[] buffer = new byte[1024 * 1024 * 5];
                //    //int count = clientSocket.Receive(buffer);
                //    //string fileName = Encoding.UTF8.GetString(buffer, 0, count);
                //    //Console.WriteLine("接收到的文件名为：" + @fileName);
                //    //6、保存文件到本地
                //    //using (FileStream fsWrite = new FileStream(@fileName, FileMode.Create))
                //    //{
                //    //fsWrite.Write(buffer, count + 1, buffer.Length - count - 1);
                //    //Console.WriteLine("文件保存成功！");
                //    //}
                //    //7、关闭套接字
                //    clientSocket.Close();
                //    serverSocket.Close();
                //    //await Task.Delay(1000);
                //}
                byte[] buffer = new byte[256];
                IPAddress ip = IPAddress.Any;
                string str = textBox10.Text;
                int port = int.Parse(str);
                IPEndPoint localEndPoint = new IPEndPoint(ip, port);
                Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                listener.Bind(localEndPoint);
                listener.Listen(10);

                Console.WriteLine("Waiting for connection...");

                Socket handler = listener.Accept();

                Console.WriteLine($"Connection from {((IPEndPoint)handler.RemoteEndPoint).Address}");

                int nameLength = handler.Receive(buffer);
                string fileName = Path.GetFileNameWithoutExtension(System.Text.Encoding.UTF8.GetString(buffer, 0, nameLength));

                using (FileStream fStream = File.Create($"D:\\{fileName}.txt"))
                {
                    int count;
                    long totalBytesReceived = 0;

                    while ((count = handler.Receive(buffer)) > 0)
                    {
                        fStream.Write(buffer, 0, count);
                        totalBytesReceived += count;
                    }

                    Console.WriteLine($"File received, saved as {fStream.Name}, {totalBytesReceived} bytes.");
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

                listener.Close();
            }
            
        }

        private void button28_Click_1(object sender, EventArgs e)
        {
            //string ad2 = textBox8.Text;
            //if (ad2 == "")
            //{
            //    errorProvider1.SetError(textBox8, "路径不能为空!");
            //    this.notifyIcon1.ShowBalloonTip(20, "Error", "路径不正确!", ToolTipIcon.Error);
            //}
            //else
            //{
            //    errorProvider1.Clear();
            //    //sy.SetOutputToWaveFile(@ad);
            //    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //    //socket.Connect(textBox9.Text, textBox10.Text);
            //    //string ip = textBox9.Text;
            //    string str = textBox10.Text;
            //    int port = int.Parse(str);
            //    //int port = 1234;
            //    //{
            //    //    IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(ip), port);
            //    //    socket.Connect(ipe);
            //    //    string filePath = @ad2;
            //    //    byte[] fileData = File.ReadAllBytes(filePath);
            //    //    socket.Send(fileData);
            //    //    socket.Close();
            //    //}
            //   
            //        //sy.Speak(textBox1.Text);
            //        //sy.SetOutputToDefaultAudioDevice();
            //        this.notifyIcon1.ShowBalloonTip(20, "应用操作", "成功地保存朗读音频文件", ToolTipIcon.Info);
            //}
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            //errorProvider1.Clear();
            //SaveFileDialog sfd = new SaveFileDialog();
            //sfd.Filter = "TXT文件(*.txt)|*.txt|所有文件|*.*";//设置文件类型
            //sfd.FileName = "保存的传输文件 ";//设置默认文件名
            //sfd.DefaultExt = "txt";//设置默认格式（可以不设）
            ////sfd.AddExtension = true;//设置自动在文件名中添加扩展名
            //if (sfd.ShowDialog() == DialogResult.OK)
            //{
            //    textBox8.Text = sfd.FileName;
            //    //+ "\r\n";

            //}
        }

        private void button32_Click(object sender, EventArgs e)
        {

            if (!isRunning)
            {
                rootDirectory = @textBox14.Text;
                listener.Prefixes.Add($"http://{textBox15.Text}:{textBox16.Text}/");
                listener.Start();
                isRunning = true;
                button32.Text = "Stop";
                listener.BeginGetContext(new AsyncCallback(OnRequest), null);
            }
            else
            {
                listener.Stop();
                isRunning = false;
                button32.Text = "Start";
            }
            //cts.Cancel();
        }
        private void OnRequest(IAsyncResult result)
        {
            if (isRunning)
            {
                var context = listener.EndGetContext(result);
                var request = context.Request;
                var response = context.Response;

                var requestedFile = request.Url.AbsolutePath.Substring(1);
                var filePath = Path.Combine(rootDirectory, requestedFile);

                if (File.Exists(filePath))
                {
                    var fileBytes = File.ReadAllBytes(filePath);
                    string aaaaa = Path.GetExtension(filePath) + ";";
                    string searchKeyword = aaaaa; // 您要查找的内容
                    string result3 = null; // 用于存储结果的变量

                    // 将TextBox中的文本按行分割
                    string[] lines = textBox1.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                    // 遍历每一行，查找指定内容
                    foreach (string line in lines)
                    {
                        if (line.Contains(searchKeyword))
                        {
                            result3 = line;
                            
                            break;
                        }
                        else
                        {
                            response.ContentType = @"application/octet-stream";
                            break;
                        }
                    }

                    // 检查结果是否为空
                    if (result3 != null)
                    {
                        // 在这里处理结果
                    }
                    else
                    {
                        // 没有找到指定内容
                    }
                    response.ContentType = result3;
                    //response.ContentType = GetContentType(filePath);
                    response.ContentLength64 = fileBytes.Length;
                    response.OutputStream.Write(fileBytes, 0, fileBytes.Length);
                    response.OutputStream.Close();
                }
                else
                {
                    response.StatusCode = 404;
                    response.OutputStream.Close();
                }

                listener.BeginGetContext(new AsyncCallback(OnRequest), null);
            }
        }
        
        //private string GetContentType(string filePath)
        //{

            //return "application/octet-stream";
            //switch (Path.GetExtension(filePath))
            //{
            //
            //    case ".html":
            //        return "text/html";
            //    case ".css":
            //        return "text/css";
            //    case ".js":
            //        return "application/javascript";
            //    case ".jpg":
            //        return "image/jpeg";
            //    case ".png":
            //        return "image/png";
            //    default:
            //        return "application/octet-stream";
            //}
        //}

        

        private async void button34_Click(object sender, EventArgs e)
        {
            //if (_isRunning)
            //{
            // 如果FTP服务器已经启动，就停止它
            //    await _ftpServerHost.StopAsync();
            // 设置按钮的文本
            //    this.button34.Text = "启动";
            //    // 设置FTP服务器未启动
            //    _isRunning = false;
            //}
            //else
            //{
            //   // 如果FTP服务器未启动，就开始它
            //    await _ftpServerHost.StartAsync();
            //    // 设置按钮的文本
            //    this.button34.Text = "停止";
            //    // 设置FTP服务器已启动
            //    _isRunning = true;
            //}
            // 设置依赖项注入
            

            // 使用%TEMP%/TestFtpServer作为根文件夹
            
            if(button34.Text == "停止") { button34.Text = "启动"; _ftpServerHost.StopAsync(CancellationToken.None).Wait(); } else { await _ftpServerHost.StartAsync(CancellationToken.None); button34.Text = "停止"; }
                // 启动FTP服务器
              

                //Console.WriteLine("按ENTER/RETURN键关闭测试应用程序。");
            
                //Console.ReadLine();

            // 停止FTP服务器
            //ftpServerHost.StopAsync(CancellationToken.None).Wait();

        }

        public class TestMembershipProvider : IMembershipProvider
        {
            public Task<MemberValidationResult> ValidateUserAsync(string username, string password)
            {
                if (username == "admin" && password == "admin")
                {
                    // 创建一个 TestUser 对象
                    var user = new TestUser(username, password);
                    // 返回验证结果和用户对象
                    return Task.FromResult(new MemberValidationResult(MemberValidationStatus.AuthenticatedUser, user));
                }
                return Task.FromResult(new MemberValidationResult(MemberValidationStatus.InvalidLogin));
            }
        }

        public class TestUser :  IFtpUser
        {
            // 用户名
            public string Name { get; }

            // 密码
            public string Password { get; }

            // 构造函数
            public TestUser(string name, string password)
            {
                Name = name;
                Password = password;
            }

            // 实现 IFtpUser 接口的属性和方法
            public string HomeDirectory => "/";

            public bool IsInGroup(string groupName) => false;

            // 判断用户是否属于某个角色
            public bool IsInRole(string roleName) => roleName == "Administrators";
        }

        private void button35_Click(object sender, EventArgs e)
        {
            services.Configure<DotNetFileSystemOptions>(opt => opt.RootPath = Directory.GetCurrentDirectory());

            // 添加FTP服务器服务
            // DotNetFileSystemProvider = 使用.NET文件系统功能
            // AnonymousMembershipProvider = 仅允许匿名登录
            services.AddFtpServer(builder =>
            {
                builder.UseDotNetFileSystem(); // 使用.NET文件系统功能
                builder.EnableAnonymousAuthentication(); // 允许匿名登录
                builder.Services.AddSingleton<IMembershipProvider, TestMembershipProvider>(); //用户登录
            });

            // 配置FTP服务器
            services.Configure<FtpServerOptions>(opt => opt.ServerAddress = "0.0.0.0");

            // 构建服务提供商
            var serviceProvider = services.BuildServiceProvider();

            // 初始化FTP服务器
            var _ftpServerHost = serviceProvider.GetRequiredService<IFtpServerHost>();
        }
        //private void Form1_Load(object sender, EventArgs e)
        //{
        // 设置依赖注入
        //var services = new ServiceCollection();

        // 把指定的目录设为根目录，这里用当前程序所在的目录作为示例
        //services.Configure<DotNetFileSystemOptions>(opt => opt.RootPath = Directory.GetCurrentDirectory());

        // 添加FTP服务器服务
        // 使用DotNetFileSystemProvider作为文件系统提供器
        // 使用AnonymousMembershipProvider允许匿名登录
        //services.AddFtpServer(builder => builder
        //    .UseDotNetFileSystem()
        //    .EnableAnonymousAuthentication());

        // 配置FTP服务器选项，这里设置服务器地址为本地回环地址
        //services.Configure<FtpServerOptions>(opt => opt.ServerAddress = "127.0.0.1");

        // 构建服务提供器
        //var serviceProvider = services.BuildServiceProvider();

        // 初始化FTP服务器主机对象
        //_ftpServerHost = serviceProvider.GetRequiredService<IFtpServerHost>();



        //}
    }
}
