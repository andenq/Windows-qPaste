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
        /// Populates the clipboard with appropiate link and pastes it.
        /// </summary>
        /// <param name="link">Link to paste.</param>
        public static void Paste(string link)
        {
            /*if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
            {
                Thread t = new Thread(new ThreadStart(() => { Paste(link); }));
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }
            else
            {*/
                ClipboardHelper.MakeRestorePoint();
                Debug.WriteLine("Pasting: " + link);

                string toPaste = link + " ";
                Clipboard.SetText(toPaste);
                
                //The clipboard is slow, we have to wait for it!
                while (!Clipboard.GetText().Equals(toPaste))
                    Thread.Sleep(500);
                
                InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
                ClipboardHelper.Restore();
            //}
        }
    }
}
