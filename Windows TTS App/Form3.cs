using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Windows.Forms;

namespace Windows_TTS_App
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(@"C:\winpe"))
            {
                MessageBox.Show("你已经安装过Windows 10 PE!无需重复安装!", "Windows 10 PE 安装", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("请不要退出!", "Windows 10 PE 安装", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Directory.CreateDirectory(@"C:\winpe");

                byte[] bywDll = global::Windows_TTS_App.Properties.Resources.boot;//获取嵌入dll文件的字节数组  
                string strwPath = @"C:\winpe\boot.sdi";//设置释放路径   导出路径
                                                       //创建dll文件（覆盖模式）  
                using (FileStream fws = new FileStream(strwPath, FileMode.Create))
                {
                    fws.Write(bywDll, 0, bywDll.Length);
                }
                if (checkBox1.Checked == true)
                {
                    //byte[] byDll = global::Windows_TTS_App.Properties.Resources.boot10_wim;//获取嵌入dll文件的字节数组  
                    //string strPath = @"C:\winpe\boot.wim";//设置释放路径   导出路径
                    //创建dll文件（覆盖模式）  
                    //using (FileStream fs = new FileStream(strPath, FileMode.Create))
                    //{
                    //fs.Write(byDll, 0, byDll.Length);
                    //}
                    File.Copy(@"C:\Users\Public\Documents\data\boot10.wim", @"C:\winpe\boot.wim");
                }
                else
                {
                    File.Copy(@"C:\Users\Public\Documents\data\boot.wim", @"C:\winpe\boot.wim");
                }
                if (checkBox3.Checked == true)
                {
                    byte[] bybDll = global::Windows_TTS_App.Properties.Resources.peu;//获取嵌入dll文件的字节数组  
                    string strbPath = @"C:\Users\Public\Documents\pe.bat";//设置释放路径   导出路径
                                                          //创建dll文件（覆盖模式）  
                    using (FileStream fbs = new FileStream(strbPath, FileMode.Create))
                    {
                        fbs.Write(bybDll, 0, bybDll.Length);
                    }
                    Process p = new Process();
                    p.StartInfo.FileName = "cmd.exe";
                    p.StartInfo.Arguments = @"/c C:\Users\Public\Documents\pe.bat";
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardInput = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    p.StandardOutput.ReadToEnd();
                    File.Delete(@"C:\Users\Public\Documents\pe.bat");
                }
                else
                {
                    byte[] bybDll = global::Windows_TTS_App.Properties.Resources.pel;//获取嵌入dll文件的字节数组  
                    string strbPath = @"C:\Users\Public\Documents\pe.bat";//设置释放路径   导出路径
                                                           //创建dll文件（覆盖模式）  
                    using (FileStream fbs = new FileStream(strbPath, FileMode.Create))
                    {
                        fbs.Write(bybDll, 0, bybDll.Length);
                    }
                    Process p = new Process();
                    p.StartInfo.FileName = "cmd.exe";
                    p.StartInfo.Arguments = @"/c C:\Users\Public\Documents\pe.bat";
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardInput = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    p.StandardOutput.ReadToEnd();
                    File.Delete(@"C:\Users\Public\Documents\pe.bat");
                }
                MessageBox.Show("Windows 10 PE已成功地安装到本地磁盘!", "Windows 10 PE 安装", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(comboBox2.Text + @"\sources"))
            {
                MessageBox.Show("你已经在可移动磁盘安装过Windows 10 PE!无需重复安装!", "Windows 10 PE 安装", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("请不要退出!", "Windows 10 PE 安装", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (checkBox2.Checked == true)
                {
                    Process p = new Process();
                    p.StartInfo.FileName = "cmd.exe";
                    p.StartInfo.Arguments = "/c FORMAT " + comboBox2.Text + " /Y /FS:FAT32 /Q";
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardInput = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    p.StandardOutput.ReadToEnd();
                }
                Directory.CreateDirectory(comboBox2.Text + @"\sources");
                byte[] bywDll = global::Windows_TTS_App.Properties.Resources.winpe;//获取嵌入dll文件的字节数组  
                string strwPath = @"C:\Users\Public\Documents\winpe.zip";//设置释放路径   导出路径
                                                                         //创建dll文件（覆盖模式）  
                using (FileStream fws = new FileStream(strwPath, FileMode.Create))
                {
                    fws.Write(bywDll, 0, bywDll.Length);
                }
                ZipFile.ExtractToDirectory(@"C:\Users\Public\Documents\winpe.zip", comboBox2.Text);
                File.Delete(@"C:\Users\Public\Documents\winpe.zip");
                if (checkBox1.Checked == true)
                {
                    //byte[] byDll = global::Windows_TTS_App.Properties.Resources.boot10_wim;//获取嵌入dll文件的字节数组  
                    //string strPath = comboBox2.Text + @"\sources\boot.wim";//设置释放路径   导出路径
                    //创建dll文件（覆盖模式）  
                    //using (FileStream fs = new FileStream(strPath, FileMode.Create))
                    //{
                    //fs.Write(byDll, 0, byDll.Length);
                    //}
                    File.Copy(@"C:\Users\Public\Documents\data\boot10.wim",comboBox2.Text + @"\sources\boot.wim");
                }
                else
                {
                    File.Copy(@"C:\Users\Public\Documents\data\boot.wim", comboBox2.Text + @"\sources\boot.wim");
                }
                MessageBox.Show("Windows 10 PE已成功地安装到可移动磁盘!", "Windows 10 PE 安装", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
