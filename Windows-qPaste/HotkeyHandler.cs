using Ionic.Utils.Zip;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Windows_qPaste
{
    /// <summary>
    /// Handles all kind of uploading that is initiated through the hotkey.
    /// </summary>
    class HotkeyHandler
    {
        private static readonly object _locker = new object();
        KeyboardHook hook = new KeyboardHook();
        System.Windows.Forms.Timer hookTimer = new System.Windows.Forms.Timer();
        public HotkeyHandler()
        {
            //Hook to global keyboard for qPaste Hotkey
            hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);

            //Application seems to lose the binding after a while
            hookTimer.Interval = 1000 * 60;
            hookTimer.Tick += (object sender, EventArgs e) => { Hook(); };
            hookTimer.Start();

            Hook();
        }

        private void Hook()
        {
            try
            {
                Debug.WriteLine("Binding hotkey");
                hook.RegisterHotKey(Windows_qPaste.ModifierKeys.Control | Windows_qPaste.ModifierKeys.Shift, Keys.V);
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine("Key already bound");
            }
        }

        private void UploadFiles(StringCollection files)
        {
            if (files.Count > 1 && (bool)Properties.Settings.Default["combinezip"])
            {
                string tempfile = Path.GetTempPath() + "\\qpaste.zip";
                File.Delete(tempfile);
                using (ZipFile zip = new ZipFile(tempfile))
                {
                    foreach (string file in files)
                    {
                        FileAttributes attr = File.GetAttributes(file);
                        if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            Debug.WriteLine(Path.GetFileName(file));
                            zip.AddDirectory(file, Path.GetFileName(file));
                        }
                        else
                        {
                            zip.AddFile(file, "");
                        }
                    }
                    zip.Save();
                }
                Token token = UploadHelper.getToken(tempfile);
                ClipboardHelper.PasteWithName("Multiple files", token.link);
                UploadHelper.Upload(tempfile, token);
                File.Delete(tempfile);
            }
            else
            {
                foreach (string file in files)
                {
                    Token token = null;

                    FileAttributes attr = File.GetAttributes(file);
                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        token = UploadHelper.getToken(file);
                        ClipboardHelper.PasteWithName(Path.GetFileName(file), token.link);

                        string tempfile = Path.GetTempPath() + "\\" + Path.GetFileNameWithoutExtension(file) + ".zip";
                        File.Delete(tempfile);
                        using (ZipFile zip = new ZipFile(tempfile))
                        {
                            zip.AddDirectory(file, "");
                            zip.Save();
                        }

                        UploadHelper.Upload(tempfile, token);
                        File.Delete(tempfile);
                    }
                    else
                    {
                        token = UploadHelper.getToken(file);
                        ClipboardHelper.PasteWithName(Path.GetFileName(file), token.link);

                        UploadHelper.Upload(file, token);
                    }
                }
            }
        }

        private void UploadImage(Image image)
        {
            string file = Path.GetTempPath() + "\\qpaste_image.png";
            File.Delete(file);
            image.Save(file, ImageFormat.Png);
            Token token = UploadHelper.getToken(file);
            ClipboardHelper.PasteWithName("Image", token.link);
            UploadHelper.Upload(file, token);
            File.Delete(file);
        }

        private void UploadText(string text)
        {
            string file = Path.GetTempPath() + "\\qpaste_text.txt";
            File.Delete(file);
            File.WriteAllText(file, text);
            Token token = UploadHelper.getToken(file);
            ClipboardHelper.PasteWithName("Text snippet", token.link);
            UploadHelper.Upload(file, token);
            File.Delete(file);
        }
        
        /// <summary>
        /// Handle upload of data. Should be run in different thread to avoid UI-blocking.
        /// </summary>
        private void HandleUpload(string ctype, object content)
        {
            lock (_locker)
            {
                ToastForm.View();
                if (ctype.Equals("files"))
                {
                    UploadFiles((StringCollection)content);
                }
                else if (ctype.Equals("image"))
                {
                    UploadImage((Image)content);
                }
                else if (ctype.Equals("text"))
                {
                    UploadText((string)content);
                }
                ToastForm.DontView();
            }
        }

        private void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            string ctype = "";
            object content = null;
            if (Clipboard.ContainsFileDropList()) 
            {
                ctype = "files";
                content = Clipboard.GetFileDropList();
            }
            else if (Clipboard.ContainsImage())
            {
                ctype = "image";
                content = Clipboard.GetImage();
            }
            else if (Clipboard.ContainsText())
            {
                ctype = "text";
                content = Clipboard.GetText();
            }
            Thread t = new Thread(new ThreadStart(() => { HandleUpload(ctype, content); }));
            t.Start();
        }
    }
}
