using Krystalware.UploadHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Windows_qPaste
{
    class UploadHelper
    {
        //public static readonly string HOST = "http://127.0.0.1:1337";
        public static readonly string HOST = "http://qpaste.eu01.aws.af.cm";

        /// <summary>
        /// Upload file to server.
        /// </summary>
        /// <param name="filepath">Path to the file to upload.</param>
        /// <param name="token">Upload token.</param>
        /// <param name="callback">Delegate to run after upload is complete.</param>
        public static void UploadAsync(string filepath, Token token, Action callback)
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Debug.WriteLine("Uploading Async");
                Upload(filepath, token);
                Debug.WriteLine("Upload done, running callback");
                callback();
            }));
            thread.Start();
        }

        /// <summary>
        /// Upload file to server.
        /// </summary>
        /// <param name="filepath">Path to the file to upload.</param>
        /// <param name="token">Upload token.</param>
        public static void Upload(string filepath, Token token)
        {
            //Library from http://aspnetupload.com/Upload-File-POST-HttpWebRequest-WebClient-RFC-1867.aspx
            string url = HOST + "/upload";
            UploadFile[] files = new UploadFile[] 
            { 
                new UploadFile(filepath, "upload", "plain/text")
            };
            NameValueCollection form = new NameValueCollection();
            form["token"] = token.token;
            string response = Krystalware.UploadHelper.HttpUploadHelper.Upload(url, files, form);
            
            /*var yourUrl = HOST + "/upload";
            var httpForm = new HttpForm(yourUrl);
            httpForm.AttachFile("upload", filepath);
            httpForm.SetValue("token", token.token);
            //ExecuteSecure(() => { StatusLabel.Text = "Uploading file: " + Path.GetFileName(filepath); });
            //response = httpForm.Submit();
            httpForm.Submit();*/
        }

        /// <summary>
        /// Reserve a link for file upload.
        /// </summary>
        /// <returns>Token and link</returns>
        public static Token getToken()
        {
            var request = WebRequest.Create(HOST + "/upload-token");
            string text;
            var response = (HttpWebResponse)request.GetResponse();
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }
            Token json = JsonConvert.DeserializeObject<Token>(text);
            return json;
        }

    }
}
