﻿using Ionic.Utils.Zip;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Windows_qPaste
{
    /// <summary>
    /// Handles all kind of uploading that is initiated through the hotkey.
    /// </summary>
    class HotkeyHandler
    {
        KeyboardHook hook = new KeyboardHook();
        System.Windows.Forms.Timer hookTimer = new System.Windows.Forms.Timer();
        bool isUploading = false;
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
        
        /// <summary>
        /// Handle upload of data. Should be run in different thread to avoid UI-blocking.
        /// </summary>
        private void HandleUpload(string ctype, object content)
        {
            if (isUploading)
                return;
            ToastForm.View();
            isUploading = true;
            if (ctype.Equals("files"))
            {
                StringCollection files = (StringCollection)content;
                if (files.Count > 1 && (bool)Properties.Settings.Default["combinezip"])
                {
                    Token token = UploadHelper.getToken();
                    ClipboardHelper.Paste(token.link);

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

                    UploadHelper.Upload(tempfile, token);
                    File.Delete(tempfile);
                    AfterUpload();

                    /*UploadHelper.UploadAsync(tempfile, token, new Action(() => 
                    {
                        File.Delete(tempfile);
                        AfterUpload();
                    }));*/
                }
                else
                {
                    foreach (string file in files)
                    {
                        Token token = UploadHelper.getToken();
                        ClipboardHelper.Paste(token.link);

                        FileAttributes attr = File.GetAttributes(file);
                        if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            string tempfile = Path.GetTempPath() + "\\"+ Path.GetFileNameWithoutExtension(file) +".zip";
                            File.Delete(tempfile);
                            using (ZipFile zip = new ZipFile(tempfile))
                            {
                                zip.AddDirectory(file, "");
                                zip.Save();
                            }

                            UploadHelper.Upload(tempfile, token);
                            File.Delete(tempfile);
                            AfterUpload();

                            /*UploadHelper.UploadAsync(tempfile, token, new Action(() =>
                            {
                                File.Delete(tempfile);
                                AfterUpload();
                            }));*/
                        }
                        else
                        {
                            UploadHelper.Upload(file, token);
                            AfterUpload();

                            /*UploadHelper.UploadAsync(file, token, new Action(() =>
                            {
                                AfterUpload();
                            }));*/
                        }
                    }
                }
            }
            else if (ctype.Equals("image"))
            {
                string file = Path.GetTempPath() + "\\qpaste_temp.png";
                File.Delete(file);
                Image image = (Image)content;
                image.Save(file, ImageFormat.Png);
                Token token = UploadHelper.getToken();
                ClipboardHelper.Paste(token.link);

                UploadHelper.Upload(file, token);
                File.Delete(file);
                AfterUpload();

                /*UploadHelper.UploadAsync(file, token, new Action(() =>
                {
                    File.Delete(file);
                    AfterUpload();
                }));*/
            }
            else if (ctype.Equals("text"))
            {
                string file = Path.GetTempPath() + "\\qpaste_temp.txt";
                File.Delete(file);
                File.WriteAllText(file, (string)content);
                Token token = UploadHelper.getToken();
                ClipboardHelper.Paste(token.link);

                UploadHelper.Upload(file, token);
                File.Delete(file);
                AfterUpload();

                /*UploadHelper.UploadAsync(file, token, new Action(() =>
                {
                    File.Delete(file);
                    AfterUpload();
                }));*/
            }
            else
            {
                AfterUpload();
            }
            //toast.Close();
        }

        private void AfterUpload()
        {
            ToastForm.DontView();
            isUploading = false;
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
