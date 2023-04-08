using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Windows_TTS_App
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            //if (Directory.Exists(@"C:\Users\Public\Documents\data"))
            //{
                //DirectoryInfo di = new DirectoryInfo(@"C:\Users\Public\Documents\data");
                //di.Delete(true);
            //}
        }
    }
}
