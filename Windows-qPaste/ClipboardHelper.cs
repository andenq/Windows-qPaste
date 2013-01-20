using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;

namespace Windows_qPaste
{
    class ClipboardHelper
    {
        private static readonly object _locker = new object();
        private static string type = "";
        private static StringCollection files = null;
        private static Image image = null;
        private static string text = null;

        /// <summary>
        /// Saves whatever is in the clipboard for later restoring.
        /// </summary>
        public static void MakeRestorePoint()
        {            
            if (Clipboard.ContainsFileDropList())
            {
                type = "files";
                files = Clipboard.GetFileDropList();
            }
            else if (Clipboard.ContainsImage())
            {
                type = "image";
                image = Clipboard.GetImage();
            }
            else if (Clipboard.ContainsText())
            {
                type = "text";
                text = Clipboard.GetText();
            }
        }

        /// <summary>
        /// Roles the clipboard back to the latest Restore Point.
        /// </summary>
        public static void Restore()
        {
            switch (type)
            {
                case "files":
                    Clipboard.SetFileDropList(files);
                    break;
                case "image":
                    Clipboard.SetImage(image);
                    break;
                case "text":
                    Clipboard.SetText(text);
                    break;
            }
        }

        /// <summary>
        /// Populates the clipboard with appropiate link, pastes it and then restores the clipboard.
        /// </summary>
        /// <param name="link">String to paste.</param>
        public static void Paste(string link)
        {
            lock (_locker)
            {
                //InputSimulator.SimulateTextEntry(link + " "); //"Pastes" without using clipboard
                ThreadStart action = new ThreadStart(() =>
                {
                    ClipboardHelper.MakeRestorePoint();
                    Debug.WriteLine("Pasting: " + link);
                    string toPaste = link + " ";
                    Clipboard.SetText(toPaste);
                    //The clipboard is slow, we have to wait for it!
                    Thread.Sleep(250);
                    InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
                    Thread.Sleep(250);
                    ClipboardHelper.Restore();
                });
                Thread thread = new Thread(action);
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
        }
    }
}
