using DiscUtils;
using DiscUtils.Ext;
using DiscUtils.Setup;
using DiscUtils.Vmdk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace RO_TOKEN
{
    public partial class Form1 : Form
    {
        private String LDCONSOLE = String.Empty;
        public Form1()
        {
            InitializeComponent();
            init();
        }


        public void init()
        {
            SetupHelper.RegisterAssembly(typeof(ExtFileSystem).Assembly);
            SetupHelper.RegisterAssembly(typeof(DiscUtils.Vmdk.Disk).Assembly);
            SetupHelper.RegisterAssembly(typeof(VirtualDiskManager).Assembly);
            SetupHelper.RegisterAssembly(typeof(VirtualDisk).Assembly);


            cb_start.DataSource = Enumerable.Range(0, 500).ToList();
            cb_end.DataSource = Enumerable.Range(0, 500).ToList();
            cb_end.SelectedIndex = 33;
            cb_start.SelectedIndex = 33;

        }

        public bool Init_Ldconsole()
        {

      

            if (File.Exists(Path.Join(txt_vms_folder.Text, "ls2console.exe")))
            {
                LDCONSOLE = Path.Join(txt_vms_folder.Text, "ls2console.exe");

            }
            else if (File.Exists(Path.Join(txt_vms_folder.Text, "ldconsole.exe")))
            {

                LDCONSOLE = Path.Join(txt_vms_folder.Text, "ldconsole.exe");
            }
            else
            {
                MessageBox.Show("模拟器路径选择错误");
                return false;

            }
            return true;
        }

        private void btn_box_folder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dilog = new FolderBrowserDialog();

            dilog.Description = "选择模拟器安装目录";

            if (dilog.ShowDialog() == DialogResult.OK || dilog.ShowDialog() == DialogResult.Yes)
            {

                txt_vms_folder.Text = dilog.SelectedPath;
            }
        }

        private void btn_select_export_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dilog = new FolderBrowserDialog();

            dilog.Description = "选择导出目录";

            if (dilog.ShowDialog() == DialogResult.OK || dilog.ShowDialog() == DialogResult.Yes)

            {

                txt_export_path.Text = dilog.SelectedPath;

            }
        }







        private HashSet<String> getVmdkFiles()
        {

            HashSet<String> filenames = new HashSet<string>();
            var folders = Directory.GetDirectories(Path.Join(txt_vms_folder.Text, "vms"));
            foreach (var folder in folders)
            {
                if (!folder.Contains("leidian"))
                {
                    continue;
                }
                filenames.Add(Path.Join(folder, "data.vmdk"));

            }
            return filenames;
        }

        private void btn_export_token_Click(object sender, EventArgs e)
        {

            HashSet<String> allVmdkFiles = getVmdkFiles();
            for (int i = cb_start.SelectedIndex; i <= cb_end.SelectedIndex; i++)
            {
                String vmdk = Path.Join(txt_vms_folder.Text, "vms", "leidian" + i, "data.vmdk");
                if (!allVmdkFiles.Contains(vmdk))
                {
                    continue;
                }

                String exportFolder = Path.Join(txt_export_path.Text, "模拟器" + i);
                if (!Directory.Exists(exportFolder))
                {
                    Directory.CreateDirectory(exportFolder);
                }

                String exportFileName = Path.Join(exportFolder, "XDUserToken.xml");
                txt_log.AppendText("\r\n" + "导出：" + exportFileName);
                try
                {
                    ExportToken(vmdk, exportFileName);
                }
                catch (Exception ex)
                {

                    txt_log.AppendText("\r\n" + "错误：" + exportFileName+" "+ex.Message);
                } 
            }
        }

 

        public void ExportToken(String vmdkPath, String DestinationFile)
        {
            String tokenFile = @"\\data\com.xd.ro.roapk\shared_prefs\XDUserToken.xml";
            VolumeManager volMgr = new VolumeManager();
            using VirtualDisk disk = VirtualDisk.OpenDisk(vmdkPath, FileAccess.Read);
            volMgr.AddDisk(disk);
            var volumes = volMgr.GetLogicalVolumes();
            var tokenVolume = volumes[1].PhysicalVolume;
            using  var stream = tokenVolume.Partition.Open();
            using ExtFileSystem extFs = new ExtFileSystem(stream);
            bool isExist = extFs.FileExists(tokenFile);
            if (isExist)
            {
                long fileLength = extFs.GetFileLength(tokenFile);

                using Stream bootStream = extFs.OpenFile(tokenFile, FileMode.Open, FileAccess.Read);

                byte[] file = new byte[bootStream.Length];
                int totalRead = 0;
                while (totalRead < file.Length)
                {
                    totalRead += bootStream.Read(file, totalRead, file.Length - totalRead);

                }
                using FileStream fileStream = File.Create(DestinationFile, (int)bootStream.Length);
                bootStream.CopyTo(fileStream);
                fileStream.Write(file, 0, (int)bootStream.Length);

            }
            else
            {
                Console.WriteLine("\r\n [!] File {0} can not be found", tokenFile);
            }

        }





        public String adbPushToken(int index, String tokenPath)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = LDCONSOLE;
            psi.Arguments = $" adb --index {index} --command \"push {tokenPath} /data/data/com.xd.ro.roapk/shared_prefs/XDUserToken.xml\"";
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;//不显示程序窗口
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;   //接受来自调用程序的输入信息
            psi.RedirectStandardError = true;   //重定向标准错误输出
            using var process = Process.Start(psi);
#pragma warning disable CS8602 // 解引用可能出现空引用。
            process.WaitForExit();
#pragma warning restore CS8602 // 解引用可能出现空引用。
            using StreamReader reader = process.StandardOutput;
            string data = reader.ReadToEnd();
        
            using StreamReader errorReader    = process.StandardError;
            String error = errorReader.ReadToEnd();
            String command = $"{LDCONSOLE} {psi.Arguments}";
            String msg = $"{command}--> {data} {error}";
            return msg;
        }



        private void btn_import_token_Click(object sender, EventArgs e)
        {
            bool init_ld = Init_Ldconsole();
            if (!init_ld)
            {
                return;
            }
            HashSet<String> allVmdkFiles = getVmdkFiles();
            for (int i = cb_start.SelectedIndex; i <= cb_end.SelectedIndex; i++)
            {
                String vmdk = Path.Join(txt_vms_folder.Text, "vms", "leidian" + i, "data.vmdk");
                if (!allVmdkFiles.Contains(vmdk))
                {
                    txt_log.AppendText("\r\n失败：" + vmdk + "模拟器不存在");
                    continue;
                }
                String exportFolder = Path.Join(txt_export_path.Text, "模拟器" + i);
                String importFilename = Path.Join(exportFolder, "XDUserToken.xml");
                if (!File.Exists(importFilename))
                {
                    txt_log.AppendText("\r\n 失败：" + exportFolder + " 无效目录");
                    continue;
                }

                String msg = adbPushToken(i, importFilename);
                txt_log.AppendText("\r\n" + msg);

            }
        }

    }
}
