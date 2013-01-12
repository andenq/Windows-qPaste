﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
        public static void Upload(string filepath, Token token)
        {
            var yourUrl = HOST + "/upload";
            var httpForm = new HttpForm(yourUrl);
            httpForm.AttachFile("upload", filepath);
            httpForm.SetValue("token", token.token);
            //ExecuteSecure(() => { StatusLabel.Text = "Uploading file: " + Path.GetFileName(filepath); });
            //response = httpForm.Submit();
            httpForm.Submit();
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
