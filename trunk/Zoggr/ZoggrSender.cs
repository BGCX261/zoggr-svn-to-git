namespace Zoggr
{
    using ReplayTv;
    using System;
    using System.Text;
    using System.Xml;

    public class ZoggrSender
    {
        public ZoggrOptions options;
        private string rddnsServer = "rddns-production.replaytv.net";

        public ZoggrSender()
        {
            ZoggrLogger.Log(AppInfo.GetTitle() + " v" + AppInfo.GetVersion() + " - (c) 2008 Zoggr");
            ZoggrLogger.LogThanks();
            this.options = new ZoggrOptions();
            this.options.LoadConfiguration();
            if (this.options.Options.rddnsip.Length > 0)
            {
                this.rddnsServer = this.options.Options.rddnsip;
            }
        }

        public string IVSTest(string isn, string responseContentText)
        {
            ReplayDevice device = new ReplayDevice();
            char[] separator = new char[] { ' ' };
            string[] textArray = responseContentText.Split(separator);
            char[] trimChars = new char[] { '"' };
            device.isn = textArray[1].Substring(4).Trim(trimChars);
            char[] chArray3 = new char[] { '"' };
            device.ip = textArray[2].Substring(3).Trim(chArray3);
            char[] chArray4 = new char[] { '"' };
            device.port = textArray[3].Substring(5).Trim(chArray4);
            try
            {
                string xml = HTTPClient.GetAsString("http://" + device.ip + ":" + device.port + "/ivs-IVSGetUnitInfo", 0x3a98);
                XmlDocument document = new XmlDocument();
                document.LoadXml(xml);
                foreach (XmlNode node in document.SelectNodes("/UnitInfo"))
                {
                    XmlElement element = node as XmlElement;
                    if (element != null)
                    {
                        return element.GetAttribute("nickname");
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        public string LookupISN(string isn)
        {
            byte[] bytes;
            string text4;
            string plainText = "isn=" + isn;
            string text2 = ReplayHelper.ByteArrayToHexString(ReplayCrypt.RDDNSEncrypt(plainText)).ToLower();
            string requestUri = string.Format("http://{0}/rd/servlet/gu?q={1}", this.rddnsServer, text2);
            ZoggrLogger.DebugLog("requestUri: " + requestUri);
            try
            {
                bytes = ReplayCrypt.RDDNSDecrypt(HTTPClient.Get(requestUri));
                text4 = Encoding.Default.GetString(bytes);
            }
            catch
            {
                Console.WriteLine("RDDNS Lookup Failed");
                return null;
            }
            if (text4.IndexOf("ReplayError") >= 0)
            {
                char[] separator = new char[] { ' ' };
                string[] textArray = text4.Split(separator);
                char[] trimChars = new char[] { '"' };
                string text5 = textArray[2].Substring(5).Trim(trimChars);
                char[] chArray3 = new char[] { '"' };
                string text6 = textArray[3].Substring(3).Trim(chArray3);
                if (!text5.Equals("SERVER") || !text6.Equals("-1"))
                {
                    return text4;
                }
                text2 = ReplayHelper.ByteArrayToHexString(ReplayCrypt.RDDNSEncrypt(plainText)).ToLower();
                requestUri = string.Format("http://{0}/rd/servlet/gu?q={1}", this.rddnsServer, text2);
                ZoggrLogger.DebugLog("requestUri: " + requestUri);
                try
                {
                    bytes = ReplayCrypt.RDDNSDecrypt(HTTPClient.Get(requestUri));
                    text4 = Encoding.Default.GetString(bytes);
                }
                catch
                {
                    Console.WriteLine("RDDNS Lookup Failed");
                    return null;
                }
            }
            return text4;
        }

        public void OverrideRDDNS(string rddns)
        {
            this.rddnsServer = rddns;
        }

        public int Send(string fromDevice, string showID, string recipientISN)
        {
            int num = 1;
            ZoggrLogger.Log(string.Format("Sending showID {0} from {1} to {2}...", showID, fromDevice, recipientISN));
            try
            {
                ReplayDevice[] replayDevices = this.options.Options.rtvList.ReplayDevices;
                int length = replayDevices.Length;
                for (int i = 0; i < length; i++)
                {
                    ReplayDevice re = replayDevices[i];
                    if (fromDevice.Equals(re.friendlyName))
                    {
                        ReplayClient client = new ReplayClient(re);
                        ZoggrLogger.Log(string.Format("Found {0}...", re.friendlyName));
                        num = client.IVSSend(recipientISN, showID, this.rddnsServer);
                        if (num == 1)
                        {
                            ZoggrLogger.Log("Error: Failed to send the show!");
                        }
                        else
                        {
                            ZoggrLogger.Log("Show successfully sent.");
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ZoggrLogger.Log(exception.Message);
            }
            return num;
        }
    }
}
