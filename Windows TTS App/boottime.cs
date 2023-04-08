using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

namespace Windows_TTS_App
{
    public partial class boottime : Form
    {
        public boottime()
        {
            InitializeComponent();
        }

        private void boottime_Shown(object sender, EventArgs e)
        {
            //Thread.Sleep(10000);
            label3.Text = "正在启动 文本转语音 ...";
            if (Directory.Exists(@"C:\Users\Public\Documents\data"))
            {
               
                if (File.Exists(@"C:\Users\Public\Documents\data\api"))
                {
                    MessageBox.Show("已加载!点击确定以进入", "应用启动", MessageBoxButtons.OK, MessageBoxIcon.None);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("应用程序包受损,应用功能将重新加载!", "应用初始化", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DirectoryInfo di = new DirectoryInfo(@"C:\Users\Public\Documents\data");
                    di.Delete(true);
                }
            }
            else
            {
                if (File.Exists(Application.StartupPath + @"\data.ini"))
                {
                    label3.Text = "正在初始化 文本转语音 ...";
                    MessageBox.Show("正在加载应用程序包!請不要退出!", "应用初始化", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    File.Copy(Application.StartupPath + @"\data.ini", @"C:\Users\Public\Documents\data.zip");
                    ZipFile.ExtractToDirectory(@"C:\Users\Public\Documents\data.zip", @"C:\Users\Public\Documents\data");
                    File.Delete(@"C:\Users\Public\Documents\data.zip");
                    if (File.Exists(@"C:\Users\Public\Documents\data\api"))
                    {
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("应用程序包受损!应用功能将受限!", "应用初始化", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("没有检测到应用程序包!请选择他所在的位置!", "应用初始化", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.InitialDirectory = "c:\\";//注意这里写路径时要用c:\\而不是c:\
                    openFileDialog.Filter = "应用程序包|*.ini|所有文件|*.*";
                    openFileDialog.RestoreDirectory = true;
                    openFileDialog.FilterIndex = 1;
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string fName = openFileDialog.FileName;
                        //MessageBox.Show("没有检测到应用程序包!请选择他所在的位置!"+fName, "应用初始化", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        File.Copy(@fName, @"C:\Users\Public\Documents\data.zip");
                        ZipFile.ExtractToDirectory(@"C:\Users\Public\Documents\data.zip", @"C:\Users\Public\Documents\data");
                        File.Delete(@"C:\Users\Public\Documents\data.zip");
                        if (File.Exists(@"C:\Users\Public\Documents\data\api"))
                        {
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("应用程序包受损!应用功能将受限!", "应用初始化", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("应用程序包无效!应用功能将受限!", "应用初始化", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                    }
                }
            }
            if(File.Exists(@"C:\Users\Public\Documents\data.zip"))
            {
                File.Delete(@"C:\Users\Public\Documents\data.zip");
            }
            
        }
    }
}
