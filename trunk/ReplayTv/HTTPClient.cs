namespace ReplayTv
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    public class HTTPClient
    {
        public static byte[] Get(string requestUri)
        {
            string[] requestHeaders = new string[0];
            return Get(requestUri, requestHeaders, -1, -1, -1);
        }

        public static byte[] Get(string requestUri, int Timeout)
        {
            string[] requestHeaders = new string[0];
            return Get(requestUri, requestHeaders, -1, -1, Timeout);
        }

        public static byte[] Get(string requestUri, string[] requestHeaders)
        {
            return Get(requestUri, requestHeaders, -1, -1, -1);
        }

        public static byte[] Get(string requestUri, string[] requestHeaders, int rangeFrom, int rangeTo)
        {
            return Get(requestUri, requestHeaders, rangeFrom, rangeTo, -1);
        }

        public static byte[] Get(string requestUri, string[] requestHeaders, int rangeFrom, int rangeTo, int Timeout)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(requestUri);
                request.KeepAlive = false;
                if (Timeout > 0)
                {
                    request.Timeout = Timeout;
                }
                //WebProxy defaultProxy = (WebProxy) request.Proxy;
                //defaultProxy.BypassProxyOnLocal = true;
                //request.Proxy = defaultProxy;
                if ((rangeFrom >= 0) && (rangeTo >= 0))
                {
                    request.AddRange(rangeFrom, rangeTo);
                }
                string[] textArray = requestHeaders;
                int length = textArray.Length;
                for (int i = 0; i < length; i++)
                {
                    string header = textArray[i];
                    int index = header.IndexOf(":");
                    if (index > 0)
                    {
                        string text2 = header.Substring(0, index).Trim();
                        string s = header.Substring(index + 1).Trim();
                        switch (text2)
                        {
                            case "Accept":
                                request.Accept = s;
                                break;

                            case "Connection":
                                request.Connection = s;
                                break;

                            case "Content-Length":
                                request.ContentLength = long.Parse(s);
                                break;

                            case "Content-Type":
                                request.ContentType = s;
                                break;

                            case "Expect":
                                request.Expect = s;
                                break;

                            case "If-Modified-Since":
                                request.IfModifiedSince = Convert.ToDateTime(s);
                                break;

                            case "Referer":
                                request.Referer = s;
                                break;

                            case "Transfer-Encoding":
                                request.TransferEncoding = s;
                                break;

                            case "User-Agent":
                                request.UserAgent = s;
                                break;

                            default:
                                if ((text2 != "Host") && (text2 != "Range"))
                                {
                                    request.Headers.Add(header);
                                }
                                break;
                        }
                    }
                }
                HttpWebResponse response = (HttpWebResponse) request.GetResponse();
                Stream input = response.GetResponseStream();
                BinaryReader reader = new BinaryReader(input);
                byte[] array = new byte[0];
                int size = 0;
                int destinationIndex = 0;
                byte[] buffer = new byte[0x2000];
                while ((size = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    array = ReplayHelper.GrowArray(array, size);
                    Array.Copy(buffer, 0, array, destinationIndex, size);
                    destinationIndex += size;
                }
                reader.Close();
                input.Close();
                response.Close();
                return array;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetAsString(string requestUri)
        {
            return GetAsString(requestUri, -1);
        }

        public static string GetAsString(string requestUri, string[] requestHeaders)
        {
            byte[] bytes = Get(requestUri, requestHeaders);
            if (bytes != null)
            {
                return Encoding.Default.GetString(bytes);
            }
            return null;
        }

        public static string GetAsString(string requestUri, int Timeout)
        {
            byte[] bytes = Get(requestUri, Timeout);
            if (bytes != null)
            {
                return Encoding.Default.GetString(bytes);
            }
            return null;
        }

        public static string GetAsString(string requestUri, string[] requestHeaders, int rangeFrom, int rangeTo)
        {
            byte[] bytes = Get(requestUri, requestHeaders, rangeFrom, rangeTo);
            if (bytes != null)
            {
                return Encoding.Default.GetString(bytes);
            }
            return null;
        }
    }
}
