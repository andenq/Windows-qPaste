﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Windows_qPaste
{
    public partial class ToastForm : TopForm
    {
        private const int SW_SHOWNA = 8;

        public ToastForm()
        {
            InitializeComponent();
            TopMost = false;
        }

        private static ToastForm instance;

        public static void View()
        {
            ThreadStart action = new ThreadStart(() =>
            {
                instance = null; 
                instance = new ToastForm(); 
                instance.ShowDialog();
            });
            Thread thread = new Thread(action);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public static void DontView()
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
                    Debug.WriteLine("Exception: " + e.ToString());
                }
            }
        }

        private void ToastForm_Load(object sender, EventArgs e)
        {
            Width = 135;
            Height = 30;
            Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - Width, Screen.PrimaryScreen.WorkingArea.Height - Height);
        }
    }
}
