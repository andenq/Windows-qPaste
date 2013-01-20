using Krystalware.UploadHelper;
using Microsoft.Win32;
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
            string ctype;
            UploadFile[] files = new UploadFile[] 
            { 
                new UploadFile(filepath, "upload", TryGetContentType(filepath, out ctype) ? ctype : "application/octet-stream")
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

        /// <summary>
        /// Attempts to query registry for content-type of supplied file name.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static bool TryGetContentType(string fileName, out string contentType)
        {
            try
            {
                RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type");

                if (key != null)
                {
                    foreach (string keyName in key.GetSubKeyNames())
                    {
                        RegistryKey subKey = key.OpenSubKey(keyName);
                        if (subKey != null)
                        {
                            string subKeyValue = (string)subKey.GetValue("Extension");

                            if (!string.IsNullOrEmpty(subKeyValue))
                            {
                                if (string.Compare(Path.GetExtension(fileName).ToUpperInvariant(),
                                                    subKeyValue.ToUpperInvariant(), StringComparison.OrdinalIgnoreCase) ==
                                    0)
                                {
                                    contentType = keyName;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            {
                // fail silently
                // TODO: rethrow registry access denied errors
            }
            // ReSharper restore EmptyGeneralCatchClause
            contentType = "";
            return false;
        }
    }
}
