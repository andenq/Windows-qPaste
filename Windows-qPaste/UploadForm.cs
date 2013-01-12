using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;

namespace Windows_qPaste
{
    public partial class UploadForm : Form
    {
        Action callback;
        //readonly string HOST = "http://qpaste.eu01.aws.af.cm";
        readonly string HOST = "http://127.0.0.1:1337";
        public UploadForm(string filepath, Action callback)
        {
            InitializeComponent();
            this.callback = callback;
            new Thread(new ThreadStart(delegate {
                //var request = WebRequest.Create("http://localhost:1337/upload-token");
                var request = WebRequest.Create(HOST + "/upload-token");
                string text;
                //ExecuteSecure(() => { StatusLabel.Text = "Getting link..."; });
                var response = (HttpWebResponse)request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    text = sr.ReadToEnd();
                }

                Token json = JsonConvert.DeserializeObject<Token>(text);

                string link = json.link;
                string token = json.token;

                ExecuteSecure(() => {
                    Paste(link);
                });

                var yourUrl = HOST + "/upload";
                var httpForm = new HttpForm(yourUrl);
                httpForm.AttachFile("upload", filepath);
                httpForm.SetValue("token", token);
                ExecuteSecure(() => { StatusLabel.Text = "Uploading file: " + Path.GetFileName(filepath); });
                response = httpForm.Submit();

                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    //MessageBox.Show(sr.ReadToEnd());
                }
                ExecuteSecure(() => { StatusLabel.Text = "Done!"; });
                ExecuteSecure(Done);
            })).Start();
        }

        /// <summary>
        /// Closes and disposes of this window.
        /// </summary>
        public void Done()
        {
            Close();
            Dispose();
            callback();
        }

        /// <summary>
        /// Populates the clipboard with appropiate link, pastes it, and then repopulates it with the original data.
        /// </summary>
        /// <param name="link">Link to paste.</param>
        public void Paste(string link)
        {
            string type = "";

            StringCollection files = null;
            Image image = null;
            string text = null;
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

            Clipboard.SetText(link + " ");
            InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);

            switch (type)
            {
                case "files":
                    Clipboard.SetFileDropList(files);
                    Debug.Write("Clipboard contained files: ");
                    foreach (string file in files) 
                    {
                        Debug.Write(file + "\n");
                    }
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
        /// Helper method to Invoke methods on this Form.
        /// </summary>
        /// <param name="a">Method to invoke</param>
        private void ExecuteSecure(Action a)
        {
            if (InvokeRequired)
                BeginInvoke(a);
            else
                a();
        }
    }
}
