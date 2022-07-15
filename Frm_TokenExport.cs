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
using System.Threading;

namespace RO_TOKEN
{
    public partial class Frm_TokenExport : Form
    {
        private String LDCONSOLE = String.Empty;
        private String ADBAPP = String.Empty;
        public Frm_TokenExport()
        {
            InitializeComponent();
            init();
        }


        public void init()
        {
            TextBox.CheckForIllegalCrossThreadCalls = false;

            SetupHelper.RegisterAssembly(typeof(ExtFileSystem).Assembly);
            SetupHelper.RegisterAssembly(typeof(Disk).Assembly);
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
                ADBAPP = Path.Join(txt_vms_folder.Text, "adb.exe");
                txt_log.AppendText($"\r\n[模拟器目录]{LDCONSOLE}");

            }
            else if (File.Exists(Path.Join(txt_vms_folder.Text, "ldconsole.exe")))
            {

                LDCONSOLE = Path.Join(txt_vms_folder.Text, "ldconsole.exe");
                ADBAPP = Path.Join(txt_vms_folder.Text, "adb.exe");
                txt_log.AppendText($"\r\n[模拟器目录]{LDCONSOLE}");
            }
            else
            {
                txt_log.AppendText("\r\n[模拟器目录]选择不正确");
                return false;

            }
            return true;
        }

        private void btn_box_folder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dilog = new FolderBrowserDialog();

            dilog.Description = "选择模拟器安装目录";
            var result = dilog.ShowDialog();

            if (result == DialogResult.OK || result == DialogResult.Yes)
            {

                txt_vms_folder.Text = dilog.SelectedPath;
            }
        }

        private void btn_select_export_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dilog = new FolderBrowserDialog();
            dilog.Description = "选择导出目录";
            var result = dilog.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Yes)
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

        private async void btn_export_token_Click(object sender, EventArgs e)
        {



            if (!Directory.Exists(txt_export_path.Text))
            {
                txt_log.AppendText($"\r\n [导出目录:{txt_export_path.Text}] 不存在");
                return;
            }



            bool init_ld = Init_Ldconsole();

            if (!init_ld)
            {
                txt_log.AppendText($"\r\n 初始化目录模拟器");
                return;
            }

            if (MessageBox.Show("导出时需要关闭所有模拟器,是否确认关闭", "警告", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                await closeAllVMBOX();
            }

            HashSet<String> allVmdkFiles = getVmdkFiles();
            for (int i = cb_start.SelectedIndex; i <= cb_end.SelectedIndex; i++)
            {
                String vmdk = Path.Join(txt_vms_folder.Text, "vms", "leidian" + i, "data.vmdk");
                if (!allVmdkFiles.Contains(vmdk))
                {
                    txt_log.AppendText($"\r\n\r\n {vmdk} 不存在 \r\n\r\n");

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
                    await ExportToken(vmdk, exportFileName);
                }
                catch (Exception ex)
                {

                    txt_log.AppendText("\r\n" + "错误：" + exportFileName + " " + ex.Message);
                }
            }
        }



        public async Task ExportToken(String vmdkPath, String DestinationFile)
        {



            String tokenFile = @"\\data\com.xd.ro.roapk\shared_prefs\XDUserToken.xml";
            VolumeManager volMgr = new VolumeManager();
            using VirtualDisk disk = VirtualDisk.OpenDisk(vmdkPath, FileAccess.Read);
            volMgr.AddDisk(disk);
            var volumes = volMgr.GetLogicalVolumes();
            var tokenVolume = volumes[1].PhysicalVolume;
            using var stream = tokenVolume.Partition.Open();
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
                    totalRead += await bootStream.ReadAsync(file, totalRead, file.Length - totalRead);
                }
                using FileStream fileStream = File.Create(DestinationFile, (int)bootStream.Length);
                await bootStream.CopyToAsync(fileStream);
                await fileStream.WriteAsync(file, 0, (int)bootStream.Length);

            }
            else
            {
                txt_log.AppendText($"\r\n  {vmdkPath}  未安装或未登陆过RO");
            }

        }


        public async Task<List<String>> ExecCmd(String processPath, String arguments)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = processPath;
            psi.Arguments = arguments;

            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;//不显示程序窗口
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;   //接受来自调用程序的输入信息
            psi.RedirectStandardError = true;   //重定向标准错误输出
            using var process = Process.Start(psi);
#pragma warning disable CS8602 // 解引用可能出现空引用。
            await process.WaitForExitAsync();
#pragma warning restore CS8602 // 解引用可能出现空引用。
            using StreamReader reader = process.StandardOutput;
            string data = await reader.ReadToEndAsync();
            using StreamReader errorReader = process.StandardError;
            String error = await errorReader.ReadToEndAsync();
            String command = $"{psi.FileName} {psi.Arguments}";
            List<String> list = new List<string> { command, data, error };
            return list;
        }

        public async Task killAllAdbs()
        {

            List<String> result = await this.ExecCmd("taskkill", "/f /im  adb.exe");
            txt_log.AppendText($"\r\n {result[0]}, {result[1]},{result[2]}");


        }



        public async Task<Dictionary<String, String[]>> getRunning()
        {
            Dictionary<String, String[]> devicesMap = new Dictionary<string, string[]>();
            List<String> result = await this.ExecCmd(LDCONSOLE, " list2");
            if (!String.IsNullOrEmpty(result[2]))
            {
                txt_export_path.AppendText($" {result[0]}, {result[1]},{result[2]}");
                return devicesMap;
            }
            List<String[]> list = result[1].Split("\r\n").Select(p => p.Split(",")).ToList();
            foreach (var item in list)
            {
                if (!String.IsNullOrEmpty(item[0]))
                {
                    devicesMap.Add(item[0], item);
                }


            }
            return devicesMap;
        }

        public async Task<String> adbPushToken(String index, String tokenPath)
        {
            var result = await ExecCmd(LDCONSOLE, $" adb --index {index} --command \"push {tokenPath} /data/data/com.xd.ro.roapk/shared_prefs/XDUserToken.xml\"");
            String msg = $"{result[0]}--> {result[1]} {result[2]}";
            return msg;
        }



        private async void btn_import_token_Click(object sender, EventArgs e)
        {



            bool init_ld = Init_Ldconsole();

            if (!init_ld)
            {
                txt_log.AppendText($"\r\n 初始化模拟器失败");
                return;
            }

            HashSet<String> allVmdkFiles = getVmdkFiles();
            List<String> allExist = new List<string>();
            for (int i = cb_start.SelectedIndex; i <= cb_end.SelectedIndex; i++)
            {
                String vmdk = Path.Join(txt_vms_folder.Text, "vms", "leidian" + i, "data.vmdk");
                if (!allVmdkFiles.Contains(vmdk))
                {
                    txt_log.AppendText("\r\n失败：" + vmdk + "模拟器不存在");
                    continue;
                }
                allExist.Add(i + "");
            }
            if (allExist.Count == 0)
            {
                txt_log.AppendText("\r\n选择的范围内模拟器不存在！");
                return;
            }



            //            var execInfo = await ExecCmd(LDCONSOLE, $" launch --index {currentMonitor[0]}");




            foreach (var i in allExist)
            {

                String exportFolder = Path.Join(txt_export_path.Text, "模拟器" + i);
                String importFilename = Path.Join(exportFolder, "XDUserToken.xml");
                if (!File.Exists(importFilename))
                {
                    txt_log.AppendText("\r\n 失败：" + exportFolder + " 无效目录");
                    continue;
                }

                String msg = await adbPushToken(i, importFilename);
                txt_log.AppendText("\r\n" + msg);

            }
            txt_log.AppendText("\r\n" + "导入完成");
        }




        public async Task closeAllVMBOX()
        {


            Process.Start(LDCONSOLE, "quitall");

            for (int i = 10; i > 0; i--)
            {
                txt_log.AppendText($"\r\n等待模拟器关闭 剩余{i}秒");
                await Task.Delay(1000);
            }

            var result = await this.ExecCmd("taskkill", " /f /im LsBoxHeadless.exe");
            txt_log.AppendText($"\r\n {result[0]}, {result[1]},{result[2]}");

            result = await this.ExecCmd("taskkill", " /f /im LsBoxSVC.exe");
            txt_log.AppendText($"\r\n {result[0]}, {result[1]},{result[2]}");
            result = await this.ExecCmd("taskkill", " /f /im lsplayer.exe");
            txt_log.AppendText($"\r\n {result[0]}, {result[1]},{result[2]}");
            result = await this.ExecCmd("taskkill", " /f /im LdVBoxHeadless.exe");
            txt_log.AppendText($"\r\n {result[0]}, {result[1]},{result[2]}");
            result = await this.ExecCmd("taskkill", " /f /im LdVBoxSVC.exe");

            txt_log.AppendText($"\r\n {result[0]}, {result[1]},{result[2]}");
            result = await this.ExecCmd("taskkill", " /f /im dnplayer.exe");
            txt_log.AppendText($"\r\n {result[0]}, {result[1]},{result[2]}");
            result = await this.ExecCmd("taskkill", " /f /im ldconsole.exe");
            txt_log.AppendText($"\r\n {result[0]}, {result[1]},{result[2]}");
            result = await this.ExecCmd("taskkill", " /f /im lsconsole.exe");
            txt_log.AppendText($"\r\n {result[0]}, {result[1]},{result[2]}");

        }

        private async void btn_kill_all_Click(object sender, EventArgs e)
        {
            bool init_ld = Init_Ldconsole();

            if (!init_ld)
            {
                return;
            }
            await closeAllVMBOX();


        }

        private async void btn_restart_adb_Click(object sender, EventArgs e)
        {
            bool init_ld = Init_Ldconsole();

            if (!init_ld)
            {
                return;
            }
            await killAllAdbs();
        }

        private void btn_open_adb_Click(object sender, EventArgs e)
        {
            bool init_ld = Init_Ldconsole();

            if (!init_ld)
            {
                return;
            }
            txt_log.AppendText("\r\n 正在 检查adb状态！");
            Process.Start(ADBAPP, "devices");
            //var  result = await this.ExecCmd(ADBAPP, "devices");
            //txt_log.AppendText($"\r\n {result[0]}, {result[1]},{result[2]}");
            txt_log.AppendText("\r\n  adb状态检查完成！");
        }

        private async void btn_start_monitor_Click(object sender, EventArgs e)
        {
            bool init_ld = Init_Ldconsole();

            if (!init_ld)
            {
                txt_log.AppendText($"\r\n 初始化模拟器失败");
                return;
            }
            HashSet<String> allVmdkFiles = getVmdkFiles();
            List<String> allExist = new List<string>();
            for (int i = cb_start.SelectedIndex; i <= cb_end.SelectedIndex; i++)
            {
                String vmdk = Path.Join(txt_vms_folder.Text, "vms", "leidian" + i, "data.vmdk");
                if (!allVmdkFiles.Contains(vmdk))
                {
                    txt_log.AppendText("\r\n失败：" + vmdk + "模拟器不存在");
                    continue;
                }
                allExist.Add(i + "");
            }
            if (allExist.Count == 0)
            {
                txt_log.AppendText("\r\n选择的范围内模拟器不存在！");
                return;
            }
            //this.button_disable(true);

            foreach (var i in allExist)
            {

                String exportFolder = Path.Join(txt_export_path.Text, "模拟器" + i);
                String importFilename = Path.Join(exportFolder, "XDUserToken.xml");
                if (!File.Exists(importFilename))
                {
                    txt_log.AppendText("\r\n 失败：" + exportFolder + " 无效目录");
                    continue;
                }
                Process.Start(LDCONSOLE, $" launch --index {i}"); ;
                txt_log.AppendText($"\r\n启动模拟器{i}");
                await Task.Delay(8000);
            }
            await Task.Delay(10000);

            txt_log.AppendText($"\r\n启动模拟器完毕");
            //this.button_disable(false);
        }

        private void btn_next_batch_Click(object sender, EventArgs e)
        {
            int end = cb_end.SelectedIndex - cb_start.SelectedIndex;
            cb_start.SelectedIndex = cb_end.SelectedIndex + 1;
            cb_end.SelectedIndex = cb_start.SelectedIndex + end;
        }

        HashSet<String> excludeBtn = new HashSet<string>() { "btn_restart_adb", "btn_open_adb" };

        public void button_disable(bool isEnable)
        {

            foreach (var item in this.Controls)
            {
                if (item is Button)
                {
                    Button btn = item as Button;
                    if (!excludeBtn.Contains(btn.Name))
                    {
                        btn.Enabled = isEnable;
                    }
                }
            }
        }
    }
}
