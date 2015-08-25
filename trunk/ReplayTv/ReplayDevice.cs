namespace ReplayTv
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    public class ReplayDevice
    {
        [XmlAttribute("friendlyName")]
        public string friendlyName;
        [XmlAttribute("ip")]
        public string ip;
        [XmlAttribute("isn")]
        public string isn;
        [XmlAttribute("ivsNickname")]
        public string ivsNickname;
        [XmlAttribute("modelNumber")]
        public string modelNumber;
        [XmlAttribute("port")]
        public string port;
        [XmlAttribute("serialNumber")]
        public string serialNumber;

        public ReplayDevice()
        {
            this.ip = "N/A";
            this.friendlyName = "N/A";
            this.serialNumber = "N/A";
            this.modelNumber = "N/A";
            this.isn = "N/A";
            this.port = "N/A";
            this.ivsNickname = "N/A";
        }

        public ReplayDevice(string ip, bool allowWiRNS)
        {
            this.ip = "N/A";
            this.friendlyName = "N/A";
            this.serialNumber = "N/A";
            this.modelNumber = "N/A";
            this.isn = "N/A";
            this.port = "N/A";
            this.ivsNickname = "N/A";
            this.ip = ip;
            this.GetDeviceDescription();
            if (this.modelNumber.Equals("4999"))
            {
                throw new ApplicationException(string.Format("Ignoring DVArchive found at {0}.", this.ip));
            }
            if (this.modelNumber.Equals("4998") && !allowWiRNS)
            {
                throw new ApplicationException(string.Format("Ignoring WiRNS found at {0}.", this.ip));
            }
            this.GetIVSInfo();
        }

        private void GetDeviceDescription()
        {
            string xml = null;
            try
            {
                xml = HTTPClient.GetAsString("http://" + this.ip + "/Device_Descr.xml");
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("Error fetching device description from {0}!", this.ip), exception);
            }
            if (xml == null)
            {
                throw new Exception(string.Format("No ReplayTV found at {0}!", this.ip));
            }
            try
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(xml);
                foreach (XmlNode node in document.GetElementsByTagName("device"))
                {
                    foreach (XmlNode node2 in node.ChildNodes)
                    {
                        if (node2.Name == "serialNumber")
                        {
                            this.serialNumber = node2.InnerText;
                            continue;
                        }
                        if (node2.Name == "friendlyName")
                        {
                            this.friendlyName = node2.InnerText;
                            continue;
                        }
                        if (node2.Name == "modelNumber")
                        {
                            this.modelNumber = node2.InnerText;
                        }
                    }
                }
            }
            catch (Exception exception2)
            {
                throw new Exception(string.Format("Error parsing device description from {0}!", this.ip), exception2);
            }
        }

        private void GetIVSInfo()
        {
            string xml = null;
            try
            {
                xml = HTTPClient.GetAsString("http://" + this.ip + "/ivs-IVSGetUnitInfo");
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("Error fetching IVS information from {0}!", this.ip), exception);
            }
            try
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(xml);
                foreach (XmlNode node in document.SelectNodes("/UnitInfo"))
                {
                    XmlElement element = node as XmlElement;
                    if (element != null)
                    {
                        this.ivsNickname = element.GetAttribute("nickname");
                        this.isn = element.GetAttribute("isn");
                    }
                }
            }
            catch (Exception exception2)
            {
                throw new Exception(string.Format("Error parsing IVS information from {0}!", this.ip), exception2);
            }
        }
    }
}
