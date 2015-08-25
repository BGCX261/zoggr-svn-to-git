namespace ReplayTv
{
    using System;
    using System.Collections;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    public class ReplayUPnP
    {
        public static SortedList DiscoverRTVs(bool allowWiRNS)
        {
            Socket socket = null;
            SortedList list2;
            try
            {
                SortedList list = new SortedList();
                string s = "M-SEARCH * HTTP/1.1\r\nDate: Fri, 02 Jan 1970 13:53:05 GMT\r\nServer: Unknown/0.0 UPnP/1.0 Virata-EmWeb/R6_0_1\r\nHOST: 239.255.255.250:1900\r\nMAN: ssdp:discover\r\nMX: 10\r\nST: urn:replaytv-com:device:ReplayDevice:1\r\n\r\n";
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                IPEndPoint localEP = new IPEndPoint(IPAddress.Any, 0);
                socket.Bind(localEP);
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 0x76c);
                byte[] bytes = Encoding.ASCII.GetBytes(s);
                socket.SendTo(bytes, remoteEP);
                for (int i = 0; i < 20; i++)
                {
                    Thread.Sleep(500);
                }
                IPEndPoint point3 = new IPEndPoint(IPAddress.Any, 0);
                EndPoint point4 = point3;
                byte[] buffer = new byte[0x800];
                while (socket.Poll(0x7a120, SelectMode.SelectRead))
                {
                    socket.ReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref point4);
                    string text2 = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
                    if (text2 != null)
                    {
                        int startIndex = text2.ToUpper().IndexOf("LOCATION:");
                        if (startIndex >= 0)
                        {
                            startIndex += 9;
                            int index = text2.IndexOf("\r\n", startIndex);
                            Uri uri = new Uri(text2.Substring(startIndex, index - startIndex).Trim());
                            string ip = uri.Host;
                            try
                            {
                                ReplayDevice device = new ReplayDevice(ip, allowWiRNS);
                                list.Add(ip, device);
                                continue;
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
                list2 = list;
            }
            catch (Exception exception)
            {
                string message = "Auto-discovery failed!";
                throw new Exception(message, exception);
            }
            finally
            {
                if (socket != null)
                {
                    socket.Close();
                }
            }
            return list2;
        }
    }
}
