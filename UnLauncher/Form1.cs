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
using CmlLib.Core;
using CmlLib.Core.ProcessBuilder;
using CmlLib.Core.Installers;
using CmlLib.Core.Auth;

namespace UnLauncher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            getVersions();
        }

        private async void getVersions()
        {
            var launcher = new MinecraftLauncher();
            var versions = await launcher.GetAllVersionsAsync();
            foreach (var version in versions)
            {
                if (version.Type=="release")
                {
                    versionsBox.Items.Add(version.Name);
                }
            }
            versionsBox.SelectedIndex = 0;
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e){ Application.Exit(); }

        private async void playButton_Click(object sender, EventArgs e)
        {
            if (usernameBox.Text!="")
            {
                disableBottomBar();
                var launcher = new MinecraftLauncher();
                launcher.FileProgressChanged += progressChanged;
                var process = await launcher.InstallAndBuildProcessAsync(versionsBox.SelectedItem.ToString(), new MLaunchOption
                {
                    Session = MSession.CreateOfflineSession(usernameBox.Text)
                });
                process.EnableRaisingEvents = true;
                process.Exited += Process_Exited;
                process.Start();
                Hide();
            }
            else
            {
                usernameMessageDialog.Show();
            }
        }

        private void disableBottomBar()
        {
            playButton.Enabled = false;
            versionsBox.Enabled = false;
            usernameBox.Enabled = false;
            playButton.Text = "Yükleniyor...";
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                // Ana iş parçacığı üzerinde çalıştırmak için Invoke kullanın
                Invoke(new Action(() =>
                {
                    Show();
                    enableBottomBar();
                }));
            }
            else
            {
                Show();
                enableBottomBar();
            }
        }

        private void enableBottomBar()
        {
            progressBar.Text = "";
            playButton.Enabled = true;
            versionsBox.Enabled = true;
            usernameBox.Enabled = true;
            playButton.Text = "Oyna";
        }

        private void progressChanged(object sender, InstallerProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressedTasks * 100 / e.TotalTasks;
            progressBar.Text = string.Format("{0} {1} - {2}/{3}", e.EventType, e.Name, e.ProgressedTasks, e.TotalTasks);
        }

        private void guna2CircleButton2_Click(object sender, EventArgs e) { WindowState = FormWindowState.Minimized; }
    }
}
