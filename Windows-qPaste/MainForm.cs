using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Windows_qPaste
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            new HotkeyHandler();

            if (AutostartHelper.isAutostartEnabled())
            {
                if (!AutostartHelper.isAutostartPathThis())
                {
                    AutostartHelper.SetAutostart();
                }
                AutostartCheckbox.Checked = true;
            }
            CombineZIPCheckbox.Checked = (bool) Properties.Settings.Default["combinezip"];
            PutnameCheckbox.Checked = (bool) Properties.Settings.Default["putname"];
            MenuItem[] items = new MenuItem[1];
            items[0] = new MenuItem("Exit", new EventHandler((object o, EventArgs args) => {
                notifyIcon.Visible = false;
                Environment.Exit(0);
            }));
            notifyIcon.ContextMenu = new ContextMenu(items);
            
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void CombineZIPCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["combinezip"] = CombineZIPCheckbox.Checked;
            Properties.Settings.Default.Save();
        }

        private void AutostartCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (AutostartCheckbox.Checked)
            {
                AutostartHelper.SetAutostart();
            }
            else
            {
                AutostartHelper.UnSetAutostart();
            }
        }

        private void PutnameCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["putname"] = PutnameCheckbox.Checked;
            Properties.Settings.Default.Save();
        }
    }
}
