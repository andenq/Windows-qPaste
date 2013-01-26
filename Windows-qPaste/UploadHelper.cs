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

namespace Windows_qPaste
{
    class UploadHelper
    {
        //public static readonly string HOST = "http://127.0.0.1:1337";
        public static readonly string HOST = "http://qpaste.rs.af.cm";
        //public static readonly string HOST = "http://qpaste-dev.rs.af.cm";

        /// <summary>
        /// Upload file to server.
        /// </summary>
        /// <param name="filepath">Path to the file to upload.</param>
        /// <param name="token">Upload token.</param>
        /// <param name="callback">Delegate to run after upload is complete.</param>
        public static void UploadAsync(string filepath, Token token, Action callback)
        {
            // Old
            /*Thread thread = new Thread(new ThreadStart(() =>
            {
                Debug.WriteLine("Uploading Async");
                Upload(filepath, token);
                Debug.WriteLine("Upload done, running callback");
                callback();
            }));
            thread.Start();*/
        }

        /// <summary>
        /// Upload file to server.
        /// </summary>
        /// <param name="filepath">Path to the file to upload.</param>
        /// <param name="token">Upload token.</param>
        public static void Upload(string filepath, Token token)
        {
            //Library from http://aspnetupload.com/Upload-File-POST-HttpWebRequest-WebClient-RFC-1867.aspx
            //string url = HOST + "/upload";
            string url = "http://qpaste.s3.amazonaws.com/";
            UploadFile[] files = new UploadFile[] 
            { 
                new UploadFile(filepath, "file", token.storage.s3Policy.conditions.mime)
            };
            NameValueCollection form = new NameValueCollection();
            form["key"] = token.storage.s3Policy.conditions.key;
            form["AWSAccessKeyId"] = token.storage.s3Key;
            form["Policy"] = token.storage.s3PolicyBase64;
            form["Signature"] = token.storage.s3Signature;
            form["Bucket"] = token.storage.s3Policy.conditions.bucket;
            form["acl"] = token.storage.s3Policy.conditions.acl;
            form["Content-Type"] = token.storage.s3Policy.conditions.mime;
            form["Content-Disposition"] = token.storage.s3Policy.conditions.disposition;

            try
            {
                string response = Krystalware.UploadHelper.HttpUploadHelper.Upload(url, files, form);
                UploadDone(token.token);
            }
            catch (System.Net.WebException ex)
            {
                string output = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                Console.WriteLine(output);
                Debug.WriteLine(output);
            }
        }

        /// <summary>
        /// Callback to main server after upload to S3 is done.
        /// </summary>
        /// <param name="token">Token for the file that was just uploaded</param>
        private static void UploadDone(string token)
        {
            NameValueCollection values = new NameValueCollection();
            values.Add("token", token);

            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                byte[] result = client.UploadValues(HOST + "/upload-done", "POST", values);
            }
        }

        /// <summary>
        /// Reserve a link for file upload.
        /// </summary>
        /// <param name="filepath">Link to make token for</param>
        /// <returns>Token and link</returns>
        public static Token getToken(string filepath)
        {
            string filename = Path.GetFileName(filepath);
            string mime = GetContentType(filepath);

            NameValueCollection values = new NameValueCollection();
            values.Add("filename", filename);
            values.Add("mime", mime);

            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                byte[] result = client.UploadValues(HOST + "/upload-token", "POST", values);
                string text = Encoding.UTF8.GetString(result);
                Token json = JsonConvert.DeserializeObject<Token>(text);
                return json;
            }
        }

        /// <summary>
        /// Attempts to query registry for content-type of supplied file name.
        /// </summary>
        /// <param name="fileName">File to analyze</param>
        /// <returns>mime-type, application/octet-stream if it fails in finding it.</returns>
        public static string GetContentType(string fileName)
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
                                    return keyName;
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
            return "application/octet-stream";
        }
    }
}
