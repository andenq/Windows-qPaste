using MouseKeyboardActivityMonitor;
using MouseKeyboardActivityMonitor.WinApi;
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
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;

namespace Windows_qPaste
{
    public partial class Form1 : Form
    {
        KeyboardHook hook = new KeyboardHook();
        Queue<string> uploadQueue = new Queue<string>();
        readonly int simultaneousUploads = 1;
        public Form1()
        {
            InitializeComponent();

            // register the event that is fired after the key press.
            hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
            // register the control + shift + v combination as hot key.
            hook.RegisterHotKey(Windows_qPaste.ModifierKeys.Control | Windows_qPaste.ModifierKeys.Shift, Keys.V);
            //hook.RegisterHotKey(ModifierKeys.Control | ModifierKeys.Alt, Keys.F12);
        }

        private void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (Clipboard.ContainsFileDropList())
            {
                StringCollection files = Clipboard.GetFileDropList();
                foreach (string file in files) 
                {
                    FileAttributes attr = File.GetAttributes(file);
                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        //Directory
                    }
                    else
                    {
                        uploadQueue.Enqueue(file);
                    }
                }
                DoQueue();
            }
        }

        private void QueueUpload(string filepath)
        {
            uploadQueue.Enqueue(filepath);
        }

        private void DoUpload(string filepath)
        {
            new UploadForm(filepath, UploadDone).Show();
        }

        int activeUploads = 0;
        private void DoQueue()
        {
            if (uploadQueue.Count > 0)
            {
                if (activeUploads == 0)
                {
                    activeUploads = 1;
                    DoUpload(uploadQueue.Dequeue());
                }
                else
                {
                    if (activeUploads < simultaneousUploads)
                    {
                        activeUploads++;
                        DoUpload(uploadQueue.Dequeue());
                    }
                }
            }
        }

        private void UploadDone()
        {
            activeUploads--;
            DoQueue();
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        string token;
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            FileBox.Text = openFileDialog1.FileName;

            var request = WebRequest.Create("http://localhost:1337/upload-token");
            string text;
            var response = (HttpWebResponse)request.GetResponse();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            Token json = JsonConvert.DeserializeObject<Token>(text);

            LinkBox.Text = json.link;
            token = json.token;
        }

        private void UploadButton_Click(object sender, EventArgs e)
        {
            //string filePath = Path.GetFullPath("TestFiles/TextFile1.txt");
            /*string filePath = FileBox.Text;
            NameValueCollection postData = new NameValueCollection();
            postData.Add("token", token);
            string responseText;
            WebResponse response = Upload.PostFile(new Uri("http://localhost:1337/upload"), postData, filePath, null, null, null, null);
            progressBar1.Value = 100;

            using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                MessageBox.Show(sr.ReadToEnd());
            }*/

            var file1 = FileBox.Text;

            var yourUrl = "http://localhost:1337/upload";
            var httpForm = new HttpForm(yourUrl);
            httpForm.AttachFile("upload", file1);
            httpForm.SetValue("token", token);
            HttpWebResponse response = httpForm.Submit();
            
            progressBar1.Value = 100;

            using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                MessageBox.Show(sr.ReadToEnd());
            }
        }

        private void MimeButton_Click(object sender, EventArgs e)
        {
            string contentType;
            bool b = Upload.TryGetContentType(FileBox.Text, out contentType);
            MessageBox.Show("MIME (" + b + "): " + contentType);
        }
    }
}
