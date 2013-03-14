using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Windows_qPaste
{
    class ProgressForm : TopForm
    {
        private static ProgressForm instance;
        private static int value = 0;
        public static int MaxValue = 100;

        private ProgressForm() : base()
        {
            Load += ToastForm_Load;
            FormBorderStyle = FormBorderStyle.None;
            Width = 0;
            BackColor = Color.FromArgb(6, 176, 37);
            Opacity = 0.5;
            TopMost = false;
            ShowInTaskbar = false;
        }

        public new static void Show()
        {
            ThreadStart action = new ThreadStart(() =>
            {
                instance = null;
                instance = new ProgressForm();
                instance.Width = 0;
                value = 0;
                instance.ShowDialog();
            });
            Thread thread = new Thread(action);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public new static void Hide()
        {
            if (instance != null)
            {
                try
                {
                    instance.Invoke(new Action(() =>
                    {
                        instance.Close();
                        instance.Dispose();
                    }));
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine("Exception: " + e.ToString());
                }
            }
        }

        /// <summary>
        /// Progress value. Should be between 0 and MaxValue 
        /// </summary>
        public static int Value
        {
            get { return value; }
            set 
            {
                instance.Invoke(new Action(() =>
                {
                    ProgressForm.value = value;
                    instance.Width = (int) (((decimal)value / (decimal)MaxValue) * Screen.PrimaryScreen.WorkingArea.Width);

                    Console.WriteLine("Width: " + (int)(((decimal)value / (decimal)MaxValue) * Screen.PrimaryScreen.WorkingArea.Width) );
                }));
            }
        }

        private void ToastForm_Load(object sender, EventArgs e)
        {
            Height = 5;
            Location = new Point(0, Screen.PrimaryScreen.WorkingArea.Height - Height);   
        }
    }
}
