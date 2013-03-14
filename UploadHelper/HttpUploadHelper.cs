// http://aspnetupload.com
// Copyright � 2009 Krystalware, Inc.
//
// This work is licensed under a Creative Commons Attribution-Share Alike 3.0 United States License
// http://creativecommons.org/licenses/by-sa/3.0/us/

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.IO;

namespace Krystalware.UploadHelper
{
    public class HttpUploadHelper
    {
        private HttpUploadHelper()
        { }

        public static string Upload(string url, UploadFile[] files, NameValueCollection form, Action<int> progress)
        {
            try
            {
                HttpWebResponse resp = Upload((HttpWebRequest)WebRequest.Create(url), files, form, progress);

                using (Stream s = resp.GetResponseStream())
                using (StreamReader sr = new StreamReader(s))
                {
                    return sr.ReadToEnd();
                }
            } catch {
                throw;
            }
        }

        public static HttpWebResponse Upload(HttpWebRequest req, UploadFile[] files, NameValueCollection form, Action<int> progress)
        {
            req.Timeout = int.MaxValue; //Maximum timeout
            req.ReadWriteTimeout = int.MaxValue;
            req.AllowWriteStreamBuffering = false;

            List<MimePart> mimeParts = new List<MimePart>();

            try
            {
                foreach (string key in form.AllKeys)
                {
                    StringMimePart part = new StringMimePart();

                    part.Headers["Content-Disposition"] = "form-data; name=\"" + key + "\"";
                    part.StringData = form[key];

                    mimeParts.Add(part);
                }

                int nameIndex = 0;

                foreach (UploadFile file in files)
                {
                    StreamMimePart part = new StreamMimePart();

                    if (string.IsNullOrEmpty(file.FieldName))
                        file.FieldName = "file" + nameIndex++;

                    part.Headers["Content-Disposition"] = "form-data; name=\"" + file.FieldName + "\"; filename=\"" + file.FileName + "\"";
                    part.Headers["Content-Type"] = file.ContentType;

                    part.SetStream(file.Data);

                    mimeParts.Add(part);
                }

                string boundary = "----------" + DateTime.Now.Ticks.ToString("x");

                req.ContentType = "multipart/form-data; boundary=" + boundary;
                req.Method = "POST";

                long contentLength = 0;

                byte[] _footer = Encoding.UTF8.GetBytes("--" + boundary + "--\r\n");

                foreach (MimePart part in mimeParts)
                {
                    contentLength += part.GenerateHeaderFooterData(boundary);
                }

                req.ContentLength = contentLength + _footer.Length;

                byte[] buffer = new byte[8192];
                byte[] afterFile = Encoding.UTF8.GetBytes("\r\n");
                int read;

                decimal sent = 0;
                using (Stream s = req.GetRequestStream())
                {
                    foreach (MimePart part in mimeParts)
                    {
                        s.Write(part.Header, 0, part.Header.Length);

                        while ((read = part.Data.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            s.Write(buffer, 0, read);
                            sent += buffer.Length;
                            progress((int) Math.Round((sent / req.ContentLength) * 100));
                            //TODO Something here is causing miscalculation in total data to be sent!
                            //Console.WriteLine("MB sent: " + Math.Round(sent / (1024 * 1024) , 2) + " - Percent: " + Math.Round((sent / req.ContentLength) * 100, 2) + "%");
                        }

                        part.Data.Dispose();

                        s.Write(afterFile, 0, afterFile.Length);
                    }

                    s.Write(_footer, 0, _footer.Length);
                }
                
                req.KeepAlive = false; //Request gets canceled otherwise
                return (HttpWebResponse)req.GetResponse();
            }
            catch 
            {
                foreach (MimePart part in mimeParts)
                    if (part.Data != null)
                        part.Data.Dispose();

                throw;
            }
        }
    }
}