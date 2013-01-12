using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        public UploadForm(string filepath)
        {
            InitializeComponent();
            new Thread(new ThreadStart(delegate {
                var request = WebRequest.Create("http://localhost:1337/upload-token");
                string text;
                ExecuteSecure(() => { StatusLabel.Text = "Getting link..."; });
                var response = (HttpWebResponse)request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    text = sr.ReadToEnd();
                }

                Token json = JsonConvert.DeserializeObject<Token>(text);

                string link = json.link;
                string token = json.token;

                ExecuteSecure(() => {  
                    Clipboard.SetText(link + " ");
                    InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
                });

                var yourUrl = "http://localhost:1337/upload";
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

        public void Done()
        {
            Close();
            Dispose();
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
