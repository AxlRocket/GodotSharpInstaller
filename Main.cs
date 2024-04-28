using GogotSharp.Properties;
using Ionic.Zip;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using Octokit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace GogotSharp
{
    public partial class Main : Form
    {

        private string INSTALL_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "GodotSharpApp");

        private bool closeFromMenuStrip = false;

        private IReadOnlyCollection<Release> releases;

        public Main(IReadOnlyCollection<Release> serviceReleases)
        {
            releases = serviceReleases;

            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            try
            {
                Font defaultFont = InitializeResourceFont(Resources.OpenSans);
                this.Font = defaultFont;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }

            

            //Window position
            Left = Screen.PrimaryScreen.WorkingArea.Right - 290 - 10;
            Top = Screen.PrimaryScreen.WorkingArea.Height - 370 - 10;

            formStrip.Renderer = new MySR();

            startWithWindowsToolStripMenuItem.Checked = Settings.Default.AtStartup;

            ignoreGodot4ToolStripMenuItem.Checked = Settings.Default.IgnoreGodotFour;

            refreshList();

            updateInfoLabel(releases.Count + " releases found");
        }

        private async void checkUpadtes()
        {
            disableButtons(); 

            var client = new GitHubClient(new ProductHeaderValue("godotengine"));

            updateInfoLabel("Checking releases");

            releases = await client.Repository.Release.GetAll("godotengine", "godot");

            updateInfoLabel(releases.Count + " releases found");

            refreshList();

            activateButtons();
        }

        private void refreshList()
        {
            Invoke(new Action(() =>
            {
                godotVersionSelect.Items.Clear();
            }));

            for (int i = 0; i < releases.Count; i++)
            {
                if (Settings.Default.IgnoreGodotFour)
                {
                    if (!releases.ElementAt(i).TagName[0].Equals('4'))
                    {
                        Invoke(new Action(() =>
                        {
                            godotVersionSelect.Items.Add(releases.ElementAt(i).TagName);
                        }));
                    }
                }
                else
                {
                    Invoke(new Action(() =>
                    {
                        godotVersionSelect.Items.Add(releases.ElementAt(i).TagName);
                    }));
                }
            }

            Invoke(new Action(() =>
            {
                godotVersionSelect.SelectedIndex = 0;
            }));
        }

        public void activateButtons()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    godotVersionSelect.Enabled = true;

                    if (!Settings.Default.InstalledVersion[0].Equals("-"))
                    {
                        godotInstall.Enabled = false;
                        godotUninstall.Enabled = true;
                    }
                    else
                    {
                        godotInstall.Enabled = true;
                        godotUninstall.Enabled = false;
                    }
                }));
            }
            else
            {
                godotVersionSelect.Enabled = true;

                if (!Settings.Default.InstalledVersion[0].Equals("-"))
                {
                    godotInstall.Enabled = false;
                    godotUninstall.Enabled = true;
                }
                else
                {
                    godotInstall.Enabled = true;
                    godotUninstall.Enabled = false;
                }
            }
        }

        public void disableButtons()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    godotVersionSelect.Enabled = false;
                    godotInstall.Enabled = false;
                    godotUninstall.Enabled = false;
                }));
            }
            else
            {
                godotVersionSelect.Enabled = false;
                godotInstall.Enabled = false;
                godotUninstall.Enabled = false;
            }
        }

        public void updateInfoLabel(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    info.Text = text;
                }));
            }
            else
            {
                info.Text = text;
            }
        }

        //*******
        //BUTTONS
        //*******
        private void godotBtn_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://godotengine.org/");
        }

        private void godotVersionSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Settings.Default.InstalledVersion.Contains(godotVersionSelect.SelectedItem.ToString()))
            {
                Console.WriteLine(Settings.Default.InstalledVersion.Contains(godotVersionSelect.SelectedItem.ToString()));
                godotInstall.Enabled = false;
                godotUninstall.Enabled = true;
            }
            else
            {
                godotInstall.Enabled = true;
                godotUninstall.Enabled = false;
            }
        }

        private async void godotInstall_Click(object sender, EventArgs e)
        {
            disableButtons();

            string version = godotVersionSelect.SelectedItem.ToString();

            try
            {
                using (WebClient _webClient = new WebClient())
                {
                    _webClient.DownloadProgressChanged += (s, ev) =>
                    {
                        updateInfoLabel("Download in progress : " + ev.ProgressPercentage.ToString() + " %");
                    };

                    Uri DosyaAdresi = new Uri(@"https://github.com/godotengine/godot/releases/download/" + version + "/Godot_v" + version + "_mono_win64.zip");
                    string zipPath = Path.Combine(INSTALL_PATH, version + ".zip");
                    await _webClient.DownloadFileTaskAsync(DosyaAdresi, zipPath);

                    updateInfoLabel("Installing godot...");

                    using (ZipFile zip = ZipFile.Read(zipPath))
                    {
                        zip.ExtractProgress += new EventHandler<ExtractProgressEventArgs>(zip_ExtractProgress);
                        zip.ExtractAll(INSTALL_PATH, ExtractExistingFileAction.OverwriteSilently);
                    }

                    System.IO.File.Delete(zipPath);

                    updateInfoLabel("Creating shortcut");

                    WshShell shell = new WshShell();
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Godot " + version + ".lnk"));
                    shortcut.TargetPath = Path.Combine(INSTALL_PATH, "Godot_v" + version + "_mono_win64", "Godot_v" + version + "_mono_win64.exe");
                    shortcut.IconLocation = Path.Combine(INSTALL_PATH, "Godot_v" + version + "_mono_win64", "Godot_v" + version + "_mono_win64.exe");
                    shortcut.Description = "Shortcut to Godot " + version;
                    shortcut.Save();

                    updateInfoLabel("Installation finished");
                }

                Settings.Default.InstalledVersion.Add(version);
                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            activateButtons();
        }

        private void godotUninstall_Click(object sender, EventArgs e)
        {
            string version = godotVersionSelect.SelectedItem.ToString();

            Directory.Delete(Path.Combine(INSTALL_PATH, "Godot_v" + version + "_mono_win64"), true);

            updateInfoLabel("Godot " + version + " uninstalled");
        }

        void zip_ExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            if (e.TotalBytesToTransfer > 0)
            {
                updateInfoLabel("Installation in progress : " + Convert.ToInt32(100 * e.BytesTransferred / e.TotalBytesToTransfer).ToString() + " %");
            }
        }

        //*********
        //MenuStrip
        //*********
        private void ignoreGodot4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Settings.Default.IgnoreGodotFour)
                Settings.Default.IgnoreGodotFour = false;
            else
                Settings.Default.IgnoreGodotFour = true;

            Settings.Default.Save();

            ignoreGodot4ToolStripMenuItem.Checked = Settings.Default.IgnoreGodotFour;

            Task.Run(() =>
            {
                checkUpadtes();
            });
        }

        private void startWithWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Settings.Default.AtStartup)
                Settings.Default.AtStartup = false;
            else
                Settings.Default.AtStartup = true;

            Settings.Default.Save();

            startWithWindowsToolStripMenuItem.Checked = Settings.Default.AtStartup;

            changeRegistryKey();
        }

        private void changeRegistryKey()
        {
            var path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);

            if (Settings.Default.AtStartup)
                key.SetValue("GodotSharpInstaller", AppDomain.CurrentDomain.BaseDirectory + "GodotSharpInstaller.exe");
            else
                key.DeleteValue("GodotSharpInstaller", false);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            closeFromMenuStrip = true;
            System.Windows.Forms.Application.Exit();
        }

        //*****
        //FONTS
        //*****
        private PrivateFontCollection fonts = new PrivateFontCollection();

        private Font InitializeResourceFont(byte[] resourceFont)
        {
            byte[] fontData = resourceFont;
            fonts.AddFontFile(@"Resources\OpenSans.ttf");
            IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
            Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            Marshal.FreeCoTaskMem(fontPtr);
            return new Font(fonts.Families[0], 9.00F);
        }

        //*********
        //Overrides
        //*********
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.CloseReason == CloseReason.WindowsShutDown) return;

            if (closeFromMenuStrip) return;

            Dispose();

            e.Cancel = true;
        }

        //Prevent window from being moved
        protected override void WndProc(ref Message message)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;

            switch (message.Msg)
            {
                case WM_SYSCOMMAND:
                    int command = message.WParam.ToInt32() & 0xfff0;
                    if (command == SC_MOVE)
                        return;
                    break;
            }

            base.WndProc(ref message);
        }
    }
}
