namespace Zoggr
{
    using System;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Text;

    public class ZoggrWorker
    {
        private static int UploadData(string uri, byte[] data, string serialNumber)
        {
            string text = "application/octet-stream";
            string text2 = "------------" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            HttpWebResponse response = null;
            Stream requestStream = null;
            request.ContentType = string.Format("multipart/form-data; boundary={0}", text2);
            request.Method = "POST";
            AssemblyName name = Assembly.GetExecutingAssembly().GetName();
            request.UserAgent = name.Name + " v" + name.Version.ToString();
            try
            {
                requestStream = request.GetRequestStream();
                byte[] bytes = Encoding.ASCII.GetBytes("--" + text2 + "\r\n");
                requestStream.Write(bytes, 0, bytes.Length);
                string s = string.Format("Content-Disposition: form-data; name=\"file\"; filename=\"{0}\"\r\nContent-Type: {1}\r\n\r\n", serialNumber + ".XML", text);
                byte[] buffer = Encoding.UTF8.GetBytes(s);
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Write(data, 0, data.Length);
                requestStream.WriteByte(13);
                requestStream.WriteByte(10);
                bytes = Encoding.ASCII.GetBytes("--" + text2 + "--\r\n");
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Flush();
            }
            catch (WebException)
            {
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
            finally
            {
                if (requestStream != null)
                {
                    requestStream.Close();
                }
            }
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException)
            {
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
            int num = 0;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                num = 1;
            }
            if (response != null)
            {
                response.Close();
            }
            return num;
        }

        public static int UploadToZoggr(string serialNumber, string filename)
        {
            byte[] data = ZoggrHelper.ReadFile(ZoggrHelper.MakePathAndFile(filename));
            string uri = "http://www.zoggr.com/Upload.aspx?sn=" + serialNumber;
            ZoggrLogger.Log("UploadToZoggr URI: " + uri);
            return UploadData(uri, data, serialNumber);
        }

        public static void DeleteFile(string filename)
        {
            ZoggrHelper.DeleteFile(ZoggrHelper.MakePathAndFile(filename));
        }
    }
}
