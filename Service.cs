using GogotSharp.Properties;
using Microsoft.Toolkit.Uwp.Notifications;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GogotSharp
{
    internal class Service : ApplicationContext
    {
        private bool canBeOpened = false;

        public Service() 
        {
            createNotifyIcon();

            Task.Run(() =>
            {
                checkUpdates();
            });
        }

        private void createNotifyIcon()
        {
            ContextMenuStrip CMS = new ContextMenuStrip
            {
                Items =
                {
                    "Open",
                    "Check updates",
                    "Close"
                },
                ShowImageMargin = false,
                RenderMode = ToolStripRenderMode.System
            };

            CMS.Items[0].Click += Open_Click;
            CMS.Items[1].Click += CheckUpdates_Click; ;
            CMS.Items[2].Click += Close_Click; ;

            NotifyIcon NI = new NotifyIcon
            {
                Text = "GodotSharpInstaller",
                Icon = Resources.Godot_icon,
                ContextMenuStrip = CMS
            };

            NI.MouseDoubleClick += NI_MouseDoubleClick;
            NI.Visible = true;
        }

        private void Close_Click(object sender, EventArgs e)
        {
            this.ExitThreadCore();
        }

        private void CheckUpdates_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                checkUpdates();
            });
        }

        private void Open_Click(object sender, EventArgs e)
        {
            openAppGUI();
        }

        private void NI_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            openAppGUI();
        }

        private void openAppGUI()
        {
            System.Threading.SpinWait.SpinUntil(() => canBeOpened);

            if (canBeOpened)
            {
                if (mainApp == null)
                {
                    mainApp = new Main(releases);
                    mainApp.ShowDialog();
                    mainApp = null;
                }
                else
                {
                    mainApp.Activate();
                }
            }
        }

        private Main mainApp = null;

        private IReadOnlyCollection<Release> releases;

        private async void checkUpdates()
        {
            canBeOpened = false;

            try
            {
                var client = new GitHubClient(new ProductHeaderValue("godotengine"));

                mainApp?.updateInfoLabel("Checking releases");

                releases = await client.Repository.Release.GetAll("godotengine", "godot");

                mainApp?.updateInfoLabel("Releases found");

                mainApp?.enableButtons();

                mainApp?.refreshList();

                if (Settings.Default.IgnoreGodotFour)
                {
                    if (!Settings.Default.InstalledVersion.Contains(releases.Select(m => m).Where(j => j.Name.StartsWith("3.")).FirstOrDefault().Name))
                    {
                        new ToastContentBuilder()
                        .AddText("New version available : " + releases.Select(m => m).Where(j => j.Name.StartsWith("3.")).FirstOrDefault().Name)
                        .Show();
                    }
                    else
                    {
                        new ToastContentBuilder()
                        .AddText("You got the last version")
                        .Show();
                    }
                }
                else
                {
                    if (!Settings.Default.InstalledVersion.Contains(releases.ElementAt(0).Name))
                    {
                        new ToastContentBuilder()
                        .AddText("New version available : " + releases.ElementAt(0).Name)
                        .Show();
                    }
                    else
                    {
                        new ToastContentBuilder()
                        .AddText("You are up to date")
                        .Show();
                    }
                }
            }
            catch
            {
                mainApp?.updateInfoLabel("Network error");

                new ToastContentBuilder()
                .AddText("Cannot check for updates. Verify your internet connection")
                .Show();
            }
            
            canBeOpened = true;
        }
    }
}
