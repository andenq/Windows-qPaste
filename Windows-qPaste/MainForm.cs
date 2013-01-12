using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Windows_qPaste
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            new HotkeyHandler();

            Debug.WriteLine(Clipboard.ContainsFileDropList().ToString());
            Debug.WriteLine(Clipboard.ContainsData(DataFormats.FileDrop).ToString());
            Debug.WriteLine(Clipboard.ContainsData(DataFormats.Text).ToString());
            Debug.WriteLine(Clipboard.GetData(DataFormats.Text));

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
    }
}
