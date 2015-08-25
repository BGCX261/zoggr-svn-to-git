namespace ReplayTv
{
    using System;
    using System.IO;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Xml;

    public class ReplayClient
    {
        private string erRespId;
        private string erRespType;
        private string guideFile;
        private static string[] httpfsHeaders = new string[] { "Authorization: Basic Uk5TQmFzaWM6QTd4KjgtUXQ=", "User-Agent: Replay-HTTPFS/1", "Accept-Encoding: text/plain" };
        private static string rddnsServer = "rddns-production.replaytv.net";
        private ReplayDevice recipientDevice;
        private ReplayDevice remoteDevice;
        private ReplayDevice theDevice;

        public ReplayClient(ReplayDevice re)
        {
            this.theDevice = re;
            this.guideFile = ReplayHelper.MakePathAndFile(this.theDevice.serialNumber + ".guide");
        }

        public long GetFileSize(string file)
        {
            string text2 = ReplayHelper.ByteArrayToHexString(ReplayCrypt.HTTPFSEncrypt("name=" + file)).ToLower();
            byte[] bytes = HTTPClient.Get(string.Format("http://{0}/httpfs-fstat?__Q_={1}", this.theDevice.ip, text2), httpfsHeaders);
            if (bytes == null)
            {
                throw new ApplicationException(string.Format("Send failed!  No response from {0} ReplayTV.  Is it connected to the network?", this.theDevice.friendlyName));
            }
            if (bytes.GetLength(0) == 0)
            {
                throw new ApplicationException("Send failed!  Show size -1MB error caught. Unable to determine a valid clock offset. Check your computer's clock.");
            }
            string text4 = Encoding.Default.GetString(bytes);
            char[] separator = new char[] { '\n' };
            string[] textArray = text4.Split(separator);
            if (text4.StartsWith("80820005"))
            {
                throw new ApplicationException(string.Format("Send failed!  Show not found on {0} ReplayTV.  Refresh your listings.", this.theDevice.friendlyName));
            }
            long num = -1;
            string[] textArray2 = textArray;
            int length = textArray2.Length;
            for (int i = 0; i < length; i++)
            {
                string text5 = textArray2[i];
                if (text5.StartsWith("size="))
                {
                    num = Convert.ToInt64(text5.Substring(5).Trim());
                }
            }
            if (num == -1)
            {
                throw new ApplicationException("Send failed!  Show size -1MB error caught.  Check your computer's clock.");
            }
            return num;
        }

        public string GetRemoteIVSName(string remoteISN, string rddnsip)
        {
            string ivsNickname;
            try
            {
                this.remoteDevice = this.RDDNSLookup(remoteISN, false, rddnsip);
            }
            catch
            {
                throw new ApplicationException("Unable to contact RDDNS server.  IVS may be down.");
            }
            if (this.erRespType.Equals("SERVER") && this.erRespId.Equals("-1"))
            {
                throw new ApplicationException("RDDNS lookup failed.  Check your computer's clock.");
            }
            if (this.erRespType.Equals("WORKER") && this.erRespId.Equals("5000"))
            {
                throw new ApplicationException(string.Format("{0} not found on IVS server.", remoteISN));
            }
            if (this.remoteDevice == null)
            {
                throw new ApplicationException(string.Format("{0} not found on IVS server.", remoteISN));
            }
            if (this.remoteDevice.port.Equals("0"))
            {
                throw new ApplicationException("Error: Unable to contact remote ReplayTV using a port of 0.");
            }
            this.remoteDevice.ivsNickname = "n/a";
            try
            {
                string xml = null;
                xml = HTTPClient.GetAsString("http://" + this.remoteDevice.ip + ":" + this.remoteDevice.port + "/ivs-IVSGetUnitInfo");
                XmlDocument document = new XmlDocument();
                document.LoadXml(xml);
                foreach (XmlNode node in document.SelectNodes("/UnitInfo"))
                {
                    XmlElement element = node as XmlElement;
                    if (element != null)
                    {
                        this.remoteDevice.ivsNickname = element.GetAttribute("nickname");
                    }
                }
                ivsNickname = this.remoteDevice.ivsNickname;
            }
            catch
            {
                throw new ApplicationException("Error: Unable to contact remote ReplayTV.");
            }
            return ivsNickname;
        }

        public int IVSSend(string recipientISN, string showID, string rddnsip)
        {
            int num;
            if (recipientISN.StartsWith("00055"))
            {
                num = this.IVSSend4000(recipientISN, showID, rddnsip);
            }
            else
            {
                num = this.IVSSend5000(recipientISN, showID, rddnsip);
            }
            this.LogSend(showID);
            return num;
        }

        public int IVSSend4000(string recipientISN, string showID, string rddnsip)
        {
            try
            {
                this.recipientDevice = this.RDDNSLookup(recipientISN, false, rddnsip);
            }
            catch
            {
                throw new ApplicationException("Unable to contact RDDNS server.  IVS may be down.");
            }
            if (this.erRespType.Equals("SERVER") && this.erRespId.Equals("-1"))
            {
                throw new ApplicationException("RDDNS lookup failed.  Check your computer's clock.");
            }
            if (this.erRespType.Equals("WORKER") && this.erRespId.Equals("5000"))
            {
                throw new ApplicationException(string.Format("{0} not found on IVS server.", recipientISN));
            }
            if (this.recipientDevice == null)
            {
                throw new ApplicationException(string.Format("{0} not found on IVS server.", recipientISN));
            }
            IVSInvite4000 structure = new IVSInvite4000();
            structure.unknown1 = IPAddress.HostToNetworkOrder(1);
            structure.unknown2 = IPAddress.HostToNetworkOrder(0);
            structure.unknown3 = IPAddress.HostToNetworkOrder(0);
            structure.unknown4 = IPAddress.HostToNetworkOrder(1);
            structure.isn_field_length = IPAddress.HostToNetworkOrder(0x12);
            structure.recipient_isn = recipientISN;
            structure.magic1 = IPAddress.HostToNetworkOrder(0);
            structure.magic2 = IPAddress.HostToNetworkOrder(3);
            structure.sender_abe.name = this.theDevice.ivsNickname;
            byte[] bytes = BitConverter.GetBytes(long.Parse(this.theDevice.isn.Replace("-", "")));
            structure.sender_abe.serial_number = new byte[6];
            if (BitConverter.IsLittleEndian)
            {
                structure.sender_abe.serial_number[0] = bytes[5];
                structure.sender_abe.serial_number[1] = bytes[4];
                structure.sender_abe.serial_number[2] = bytes[3];
                structure.sender_abe.serial_number[3] = bytes[2];
                structure.sender_abe.serial_number[4] = bytes[1];
                structure.sender_abe.serial_number[5] = bytes[0];
            }
            else
            {
                structure.sender_abe.serial_number[0] = bytes[2];
                structure.sender_abe.serial_number[1] = bytes[3];
                structure.sender_abe.serial_number[2] = bytes[4];
                structure.sender_abe.serial_number[3] = bytes[5];
                structure.sender_abe.serial_number[4] = bytes[6];
                structure.sender_abe.serial_number[5] = bytes[7];
            }
            structure.sender_abe.unknown = 0;
            structure.sender_abe.flags = IPAddress.HostToNetworkOrder(0);
            structure.replay_show = new byte[0x1ac];
            ReplayGuide guide = new ReplayGuide(this.theDevice);
            Array.Copy(guide.GetReplayShow(Convert.ToInt32(showID)), 0, structure.replay_show, 0, structure.replay_show.Length);
            long host = this.GetFileSize("/Video/" + showID + ".mpg");
            structure.mpeg_size = IPAddress.HostToNetworkOrder(host);
            host = this.GetFileSize("/Video/" + showID + ".ndx");
            structure.index_size = IPAddress.HostToNetworkOrder(host);
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(structure));
            Marshal.StructureToPtr(structure, ptr, true);
            if (ptr == IntPtr.Zero)
            {
                return 1;
            }
            byte[] destination = new byte[Marshal.SizeOf(structure)];
            Marshal.Copy(ptr, destination, 0, destination.Length);
            string text = ReplayHelper.ByteArrayToHexString(destination).ToUpper();
            string asString = HTTPClient.GetAsString(string.Concat(new object[] { "http://", this.recipientDevice.ip, ":", this.recipientDevice.port, "/ivs-IVSSendNotification?isn=", recipientISN, "&size=", text.Length, "&data=", text }));
            if (asString == "0x9478001e")
            {
                throw new ApplicationException(string.Format("Remote ReplayTV is blocking sends from {0}.  Unable to send show!", this.theDevice.ivsNickname));
            }
            if (asString != "0x00000000")
            {
                throw new ApplicationException("Unable to contact remote ReplayTV");
            }
            return 0;
        }

        public int IVSSend5000(string recipientISN, string showID, string rddnsip)
        {
            try
            {
                this.recipientDevice = this.RDDNSLookup(recipientISN, false, rddnsip);
            }
            catch
            {
                throw new ApplicationException("Unable to contact RDDNS server.  IVS may be down.");
            }
            if (this.erRespType.Equals("SERVER") && this.erRespId.Equals("-1"))
            {
                throw new ApplicationException("RDDNS lookup failed.  Check your computer's clock.");
            }
            if (this.erRespType.Equals("WORKER") && this.erRespId.Equals("5000"))
            {
                throw new ApplicationException(string.Format("{0} not found on IVS server.", recipientISN));
            }
            if (this.recipientDevice == null)
            {
                throw new ApplicationException(string.Format("{0} not found on IVS server.", recipientISN));
            }
            IVSInvite5000 structure = new IVSInvite5000();
            structure.unknown1 = IPAddress.HostToNetworkOrder(1);
            structure.unknown2 = IPAddress.HostToNetworkOrder(0);
            structure.unknown3 = IPAddress.HostToNetworkOrder(0);
            structure.unknown4 = IPAddress.HostToNetworkOrder(1);
            structure.isn_field_length = IPAddress.HostToNetworkOrder(0x12);
            structure.recipient_isn = recipientISN;
            structure.magic1 = IPAddress.HostToNetworkOrder(0);
            structure.magic2 = IPAddress.HostToNetworkOrder(5);
            structure.sender_abe.name = this.theDevice.ivsNickname;
            byte[] bytes = BitConverter.GetBytes(long.Parse(this.theDevice.isn.Replace("-", "")));
            structure.sender_abe.serial_number = new byte[6];
            if (BitConverter.IsLittleEndian)
            {
                structure.sender_abe.serial_number[0] = bytes[5];
                structure.sender_abe.serial_number[1] = bytes[4];
                structure.sender_abe.serial_number[2] = bytes[3];
                structure.sender_abe.serial_number[3] = bytes[2];
                structure.sender_abe.serial_number[4] = bytes[1];
                structure.sender_abe.serial_number[5] = bytes[0];
            }
            else
            {
                structure.sender_abe.serial_number[0] = bytes[2];
                structure.sender_abe.serial_number[1] = bytes[3];
                structure.sender_abe.serial_number[2] = bytes[4];
                structure.sender_abe.serial_number[3] = bytes[5];
                structure.sender_abe.serial_number[4] = bytes[6];
                structure.sender_abe.serial_number[5] = bytes[7];
            }
            structure.sender_abe.unknown = 0;
            structure.sender_abe.flags = IPAddress.HostToNetworkOrder(0);
            structure.replay_show = new byte[0x1ac];
            ReplayGuide guide = new ReplayGuide(this.theDevice);
            Array.Copy(guide.GetReplayShow(Convert.ToInt32(showID)), 0, structure.replay_show, 0, structure.replay_show.Length);
            long host = this.GetFileSize("/Video/" + showID + ".mpg");
            structure.mpeg_size = IPAddress.HostToNetworkOrder(host);
            host = this.GetFileSize("/Video/" + showID + ".ndx");
            structure.index_size = IPAddress.HostToNetworkOrder(host);
            host = this.GetFileSize("/Video/" + showID + ".evt");
            structure.evt_size = IPAddress.HostToNetworkOrder(host);
            structure.reserved1 = new byte[0x2c];
            structure.reserved2 = new byte[8];
            structure.reserved3 = new byte[8];
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(structure));
            Marshal.StructureToPtr(structure, ptr, true);
            if (ptr == IntPtr.Zero)
            {
                return 1;
            }
            byte[] destination = new byte[Marshal.SizeOf(structure)];
            Marshal.Copy(ptr, destination, 0, destination.Length);
            string text = ReplayHelper.ByteArrayToHexString(destination).ToUpper();
            string asString = HTTPClient.GetAsString(string.Concat(new object[] { "http://", this.recipientDevice.ip, ":", this.recipientDevice.port, "/ivs-IVSSendNotification?isn=", recipientISN, "&size=", text.Length, "&data=", text }));
            if (asString == "0x9478001e")
            {
                throw new ApplicationException(string.Format("Remote ReplayTV is blocking sends from {0}.  Unable to send show!", this.theDevice.ivsNickname));
            }
            if (asString != "0x00000000")
            {
                throw new ApplicationException("Unable to contact remote ReplayTV");
            }
            return 0;
        }

        private void LogSend(string showID)
        {
            this.recipientDevice.ivsNickname = "n/a";
            try
            {
                string xml = null;
                xml = HTTPClient.GetAsString("http://" + this.recipientDevice.ip + ":" + this.recipientDevice.port + "/ivs-IVSGetUnitInfo");
                XmlDocument document = new XmlDocument();
                document.LoadXml(xml);
                foreach (XmlNode node in document.SelectNodes("/UnitInfo"))
                {
                    XmlElement element = node as XmlElement;
                    if (element != null)
                    {
                        this.recipientDevice.ivsNickname = element.GetAttribute("nickname");
                    }
                }
            }
            catch
            {
                ReplayLogger.Log("Send logging: Unable to query remote replay for ivsNickname");
            }
            string text2 = "n/a";
            string text3 = "n/a";
            string text4 = "n/a";
            XmlDocument document2 = new XmlDocument();
            string path = ReplayHelper.MakePathAndFile(this.theDevice.serialNumber + ".xml");
            if (System.IO.File.Exists(path))
            {
                document2.Load(path);
                XmlElement element2 = document2.SelectSingleNode(string.Format("/result/channel-list/channel/show[@showID=\"{0}\"]", showID)) as XmlElement;
                if (element2 != null)
                {
                    text2 = element2.GetAttribute("title");
                    text3 = element2.GetAttribute("episodeTitle");
                    text4 = element2.GetAttribute("startTimeGMT");
                }
                else
                {
                    ReplayLogger.Log("showID not found in guide xml file: " + showID);
                }
            }
            else
            {
                ReplayLogger.Log("guide xml file not found.");
            }
            string filename = ReplayHelper.MakePathAndFile("sendlog.xml");
            XmlDocument document3 = new XmlDocument();
            try
            {
                document3.Load(filename);
            }
            catch (FileNotFoundException)
            {
                XmlTextWriter writer = new XmlTextWriter(filename, Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteStartElement("sendlog");
                writer.Close();
                document3.Load(filename);
            }
            try
            {
                XmlElement newChild = document3.CreateElement("send");
                XmlAttribute newAttr = document3.CreateAttribute("show");
                newAttr.Value = text2;
                XmlAttribute attribute2 = document3.CreateAttribute("episode");
                attribute2.Value = text3;
                XmlAttribute attribute3 = document3.CreateAttribute("showdate");
                attribute3.Value = text4;
                XmlAttribute attribute4 = document3.CreateAttribute("isn");
                attribute4.Value = this.recipientDevice.isn;
                XmlAttribute attribute5 = document3.CreateAttribute("ivsname");
                attribute5.Value = this.recipientDevice.ivsNickname;
                XmlAttribute attribute6 = document3.CreateAttribute("senddate");
                attribute6.Value = DateTime.Now.ToString();
                XmlAttribute attribute7 = document3.CreateAttribute("unit");
                attribute7.Value = this.theDevice.friendlyName;
                newChild.SetAttributeNode(newAttr);
                newChild.SetAttributeNode(attribute2);
                newChild.SetAttributeNode(attribute3);
                newChild.SetAttributeNode(attribute4);
                newChild.SetAttributeNode(attribute5);
                newChild.SetAttributeNode(attribute6);
                newChild.SetAttributeNode(attribute7);
                document3.DocumentElement.InsertBefore(newChild, document3.DocumentElement.FirstChild);
                document3.Save(filename);
            }
            catch (Exception exception)
            {
                ReplayLogger.Log("Send Log write exception: " + exception.ToString());
            }
        }

        private ReplayDevice ParseRDDNSResponse(byte[] responseContent)
        {
            byte[] bytes = ReplayCrypt.RDDNSDecrypt(responseContent);
            string text = Encoding.Default.GetString(bytes);
            if (text.IndexOf("ReplayError") >= 0)
            {
                char[] separator = new char[] { ' ' };
                string[] textArray = text.Split(separator);
                char[] trimChars = new char[] { '"' };
                this.erRespType = textArray[2].Substring(5).Trim(trimChars);
                char[] chArray3 = new char[] { '"' };
                this.erRespId = textArray[3].Substring(3).Trim(chArray3);
            }
            else
            {
                this.erRespType = "";
                this.erRespId = "";
            }
            if (text.IndexOf("UnitTicket") >= 0)
            {
                ReplayDevice device = new ReplayDevice();
                char[] chArray4 = new char[] { ' ' };
                string[] textArray2 = text.Split(chArray4);
                char[] chArray5 = new char[] { '"' };
                device.isn = textArray2[1].Substring(4).Trim(chArray5);
                char[] chArray6 = new char[] { '"' };
                device.ip = textArray2[2].Substring(3).Trim(chArray6);
                char[] chArray7 = new char[] { '"' };
                device.port = textArray2[3].Substring(5).Trim(chArray7);
                return device;
            }
            return null;
        }

        public ReplayDevice RDDNSLookup(string isn, bool final, string rddnsip)
        {
            string requestUri;
            string text2 = ReplayHelper.ByteArrayToHexString(ReplayCrypt.RDDNSEncrypt("isn=" + isn)).ToLower();
            if (rddnsip.Length > 0)
            {
                requestUri = string.Format("http://{0}/rd/servlet/gu?q={1}", rddnsip, text2);
            }
            else
            {
                requestUri = string.Format("http://{0}/rd/servlet/gu?q={1}", rddnsServer, text2);
            }
            byte[] responseContent = HTTPClient.Get(requestUri);
            ReplayDevice device = this.ParseRDDNSResponse(responseContent);
            if ((!final && this.erRespType.Equals("SERVER")) && this.erRespId.Equals("-1"))
            {
                device = this.RDDNSLookup(isn, true, rddnsip);
            }
            return device;
        }

        [StructLayout(LayoutKind.Sequential, Size=0x20, Pack=1)]
        private struct AddressBookEntry
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x15)]
            public string name;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=6)]
            public byte[] serial_number;
            public byte unknown;
            public int flags;
        }

        [StructLayout(LayoutKind.Sequential, Size=0x20a, Pack=1)]
        private struct IVSInvite4000
        {
            public int unknown1;
            public int unknown2;
            public int unknown3;
            public int unknown4;
            public int isn_field_length;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x12)]
            public string recipient_isn;
            public int magic1;
            public int magic2;
            public ReplayClient.AddressBookEntry sender_abe;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=0x1ac)]
            public byte[] replay_show;
            public long index_size;
            public long mpeg_size;
        }

        [StructLayout(LayoutKind.Sequential, Size=590, Pack=1)]
        private struct IVSInvite5000
        {
            public int unknown1;
            public int unknown2;
            public int unknown3;
            public int unknown4;
            public int isn_field_length;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x12)]
            public string recipient_isn;
            public int magic1;
            public int magic2;
            public ReplayClient.AddressBookEntry sender_abe;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=0x1ac)]
            public byte[] replay_show;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=0x2c)]
            public byte[] reserved1;
            public long index_size;
            public long mpeg_size;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=8)]
            public byte[] reserved2;
            public long evt_size;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=8)]
            public byte[] reserved3;
        }
    }
}
