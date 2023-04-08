using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Speech.AudioFormat;
using System.IO;
using System.Diagnostics;
using System.Resources;
using System.IO.Compression;
using System.Reflection;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Windows_TTS_App.Properties;
using System.Configuration;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using System.Security.AccessControl;
using System.Runtime.Remoting.Contexts;

namespace Windows_TTS_App
{
    public partial class Form1 : Form
    {
        // HTTP服务
        private HttpListener listener = new HttpListener();
        private Thread ThreadListener = null;

        // web根目录
        private string WebPath = AppDomain.CurrentDomain.BaseDirectory + "web\\";

        // 自定义POST处理
        public delegate string EventDo(object sender, HttpListenerRequest request);
        public event EventDo OnEventDo;

        // 消息事件
        public delegate void EventInfo(object sender, string info);
        public event EventInfo OnEventInfo;

        // 最后错误信息
        public string LastErrorInfo = "";

        // 是否调试
        //public bool IsDebug = false;

        // 触发消息
        private void showInfo(string str)
        {
            OnEventInfo(this, str);
        }
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
        public Form1()
        {
            boottime myabForm = new boottime();
            myabForm.ShowDialog();
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

        }
        SpeechSynthesizer sy = new SpeechSynthesizer();
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
    }
}
