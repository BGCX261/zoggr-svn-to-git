namespace Zoggr
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Xml.Serialization;
    using ReplayTv;

    public class ZoggrOptions
    {
        private string configFile;
        public OptionsContainer Options;

        public ZoggrOptions()
        {
            this.Options.rtvList = new ReplayDeviceList();
            this.Options.updateFrequency = 0xdbba00;
            this.Options.shareReceived = false;
            this.Options.rddnsip = "";
            this.Options.allowWiRNS = false;
            this.configFile = ZoggrHelper.MakePathAndFile("config.xml");
        }

        public void LoadConfiguration()
        {
            if (File.Exists(this.configFile))
            {
                XmlSerializer serializer = null;
                TextReader textReader = null;
                try
                {
                    serializer = new XmlSerializer(typeof(OptionsContainer));
                    textReader = new StreamReader(this.configFile);
                    this.Options = (OptionsContainer)serializer.Deserialize(textReader);
                }
                catch (Exception exception)
                {
                    throw new Exception(string.Format("Error reading configuration from {0}!", this.configFile), exception);
                }
                finally
                {
                    if (textReader != null)
                    {
                        textReader.Close();
                    }
                }
            }
            if (this.Options.rddnsip == null)
            {
                this.Options.rddnsip = "";
            }
        }

        public void SaveConfiguration()
        {
            XmlSerializer serializer = null;
            TextWriter textWriter = null;
            try
            {
                serializer = new XmlSerializer(typeof(OptionsContainer));
                textWriter = new StreamWriter(this.configFile);
                serializer.Serialize(textWriter, this.Options);
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("Error writing configuration to {0}!", this.configFile), exception);
            }
            finally
            {
                if (textWriter != null)
                {
                    textWriter.Close();
                }
            }
        }

        [StructLayout(LayoutKind.Sequential), XmlRoot("zoggrService")]
        public struct OptionsContainer
        {
            [XmlElement("devices")]
            public ReplayTv.ReplayDeviceList rtvList;
            [XmlElement("updateFrequency")]
            public int updateFrequency;
            [XmlElement("shareReceived")]
            public bool shareReceived;
            [XmlElement("rddnsip")]
            public string rddnsip;
            [XmlElement("allowWiRNS")]
            public bool allowWiRNS;
        }
    }
}
