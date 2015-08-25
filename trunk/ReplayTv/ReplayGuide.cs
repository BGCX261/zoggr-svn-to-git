namespace ReplayTv
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml;

    public class ReplayGuide
    {
        private Encoding encoding;
        private string guideFile;
        private string guideXMLFile;
        private ReplayDevice theDevice;

        public ReplayGuide(ReplayDevice rd)
        {
            this.theDevice = rd;
            this.guideFile = ReplayHelper.MakePathAndFile(this.theDevice.serialNumber + ".guide");
            this.guideXMLFile = ReplayHelper.MakePathAndFile(this.theDevice.serialNumber + ".xml");
            OperatingSystem oSVersion = Environment.OSVersion;
            if (((oSVersion.Platform == PlatformID.Win32NT) || (oSVersion.Platform == PlatformID.Win32S)) || (oSVersion.Platform == PlatformID.Win32Windows))
            {
                ReplayLogger.DebugLog("Using Default Encoding for Windows - " + oSVersion.Platform);
                this.encoding = Encoding.Default;
            }
            else
            {
                ReplayLogger.DebugLog("Request iso-8859-1 Encoding for " + oSVersion.Platform);
                this.encoding = Encoding.GetEncoding("iso-8859-1");
            }
        }

        private SortedList Build4000Categories(byte[] guideData)
        {
            SortedList list = new SortedList();
            for (int i = 0; i < ReplayHelper.ReplayByteToInt(guideData, 0x24, 4); i++)
            {
                int key = ((int) 1) << ReplayHelper.ReplayByteToInt(guideData, 40 + (i * 4), 4);
                int num3 = ReplayHelper.ReplayByteToInt(guideData, 0xa8 + (i * 4), 4);
                byte[] destinationArray = new byte[0x10];
                Array.Copy(guideData, 0x128 + num3, destinationArray, 0, 0x10);
                string text = ReplayHelper.ReplaceUTF8InString(this.encoding.GetString(destinationArray));
                int length = text.IndexOf('\0');
                string text2 = text.Substring(0, length);
                list.Add(key, text2);
            }
            return list;
        }

        public SortedList Build4000Channels(byte[] guideData)
        {
            SortedList categoryList = this.Build4000Categories(guideData);
            return this.Build4000Channels(guideData, categoryList);
        }

        private SortedList Build4000Channels(byte[] guideData, SortedList categoryList)
        {
            SortedList list = new SortedList();
            for (int i = 0; i < ReplayHelper.ReplayByteToInt(guideData, 8, 4); i++)
            {
                byte[] destinationArray = new byte[0x270];
                byte[] buffer2 = new byte[0x30];
                int num3 = 0;
                int num4 = 0;
                Array.Copy(guideData, ReplayHelper.ReplayByteToInt(guideData, 20, 4) + (i * 0x270), destinationArray, 0, 0x270);
                int key = ReplayHelper.ReplayByteToInt(destinationArray, 0x20, 4);
                num3 = ReplayHelper.ReplayByteToInt(destinationArray, 0, 4);
                num4 = ReplayHelper.ReplayByteToInt(destinationArray, 0x1c, 4);
                try
                {
                    Array.Copy(destinationArray, 0x44, buffer2, 0, 0x30);
                    string text = ReplayHelper.ReplaceUTF8InString(this.encoding.GetString(buffer2));
                    int length = text.IndexOf('\0');
                    string text2 = text.Substring(0, length);
                    ReplayChannelGuide guide = new ReplayChannelGuide();
                    guide.channelTitle = text2;
                    guide.channelType = num3;
                    guide.channelCategory = num4;
                    guide.channelCatName = (string) categoryList[num4];
                    list.Add(key, guide);
                }
                catch
                {
                }
            }
            return list;
        }

        private SortedList Build5000Categories(byte[] guideData)
        {
            SortedList list = new SortedList();
            for (int i = 0; i < ReplayHelper.ReplayByteToInt(guideData, 0x44, 4); i++)
            {
                int key = ((int) 1) << ReplayHelper.ReplayByteToInt(guideData, 0x48 + (i * 4), 4);
                int num3 = ReplayHelper.ReplayByteToInt(guideData, 200 + (i * 4), 4);
                byte[] destinationArray = new byte[0x10];
                Array.Copy(guideData, 0x148 + num3, destinationArray, 0, 0x10);
                string text = ReplayHelper.ReplaceUTF8InString(this.encoding.GetString(destinationArray));
                int length = text.IndexOf('\0');
                string text2 = text.Substring(0, length);
                list.Add(key, text2);
            }
            return list;
        }

        public SortedList Build5000Channels(byte[] guideData)
        {
            SortedList categoryList = this.Build5000Categories(guideData);
            return this.Build5000Channels(guideData, categoryList);
        }

        private SortedList Build5000Channels(byte[] guideData, SortedList categoryList)
        {
            SortedList list = new SortedList();
            for (int i = 0; i < ReplayHelper.ReplayByteToInt(guideData, 0x10, 4); i++)
            {
                byte[] destinationArray = new byte[0x2c8];
                byte[] buffer2 = new byte[0x30];
                Array.Copy(guideData, ReplayHelper.ReplayByteToInt(guideData, 0x20, 4) + (i * 0x2c8), destinationArray, 0, 0x2c8);
                int key = ReplayHelper.ReplayByteToInt(destinationArray, 0x23c, 4);
                int num4 = ReplayHelper.ReplayByteToInt(destinationArray, 0x240, 4);
                int num3 = ReplayHelper.ReplayByteToInt(destinationArray, 580, 4);
                try
                {
                    Array.Copy(destinationArray, 0x260, buffer2, 0, 0x30);
                    string text = ReplayHelper.ReplaceUTF8InString(this.encoding.GetString(buffer2));
                    int length = text.IndexOf('\0');
                    string text2 = text.Substring(0, length);
                    ReplayChannelGuide guide = new ReplayChannelGuide();
                    guide.channelTitle = text2;
                    guide.channelType = num3;
                    guide.channelCategory = num4;
                    guide.channelCatName = (string) categoryList[num4];
                    list.Add(key, guide);
                }
                catch
                {
                }
            }
            return list;
        }

        public SortedList BuildChannels()
        {
            if (File.Exists(this.guideFile))
            {
                FileStream stream = File.OpenRead(this.guideFile);
                byte[] array = new byte[(int) stream.Length];
                stream.Read(array, 0, array.Length);
                stream.Close();
                switch (this.CheckVersion())
                {
                    case 1:
                    case 2:
                        return this.Build4000Channels(array);

                    case 3:
                        return this.Build5000Channels(array);
                }
            }
            return null;
        }

        public int CheckVersion()
        {
            FileStream stream = File.OpenRead(this.guideFile);
            byte[] array = new byte[4];
            stream.Read(array, 0, 4);
            stream.Close();
            switch (ReplayHelper.ReplayByteToInt(array, 0, 4))
            {
                case 2:
                    return 3;

                case 0x30001:
                    return 1;

                case 0x50001:
                    return 2;
            }
            return -1;
        }

        public int GetGuide()
        {
            int count;
            DateTime time = new DateTime(0x7b2, 1, 1, 0, 0, 0, 0);
            string requestUri = string.Format("http://{0}/http_replay_guide-get_snapshot?guide_file_name=0&serial_no=RTV4080K0000000000", this.theDevice.ip, (long) (((ulong) (DateTime.UtcNow.Ticks - time.Ticks)) / 0x989680));
            try
            {
                byte[] haystack = HTTPClient.Get(requestUri);
                int offset = ReplayHelper.FindInByteArray(haystack, this.encoding.GetBytes("#####ATTACHED_FILE_START#####")) + 0x1d;
                count = ReplayHelper.FindInByteArray(haystack, this.encoding.GetBytes("#####ATTACHED_FILE_END#####")) - offset;
                try
                {
                    if (count > 0)
                    {
                        FileStream stream = File.Create(this.guideFile);
                        stream.Write(haystack, offset, count);
                        stream.Close();
                    }
                }
                catch (Exception)
                {
                    ReplayLogger.DebugLog(string.Concat(new object[] { "Unable to create guide file: ", this.guideFile, " of ", count, " bytes." }));
                }
            }
            catch (Exception)
            {
                ReplayLogger.DebugLog(string.Format("Unable to contact ReplayTV {0} at {1}", this.theDevice.friendlyName, this.theDevice.ip));
                throw new ApplicationException(string.Format("Unable to contact ReplayTV {0} at {1}", this.theDevice.friendlyName, this.theDevice.ip));
            }
            return count;
        }

        public string GetQuality(int qualityCode)
        {
            switch (qualityCode)
            {
                case 0:
                    return "HIGH";

                case 1:
                    return "MEDIUM";
            }
            return "STANDARD";
        }

        public byte[] GetReplayShow(int showID)
        {
            byte[] destinationArray = null;
            if (File.Exists(this.guideFile))
            {
                FileStream stream = File.OpenRead(this.guideFile);
                byte[] array = new byte[(int) stream.Length];
                int count = array.Length;
                stream.Read(array, 0, count);
                stream.Close();
                int offset = -1;
                switch (this.CheckVersion())
                {
                    case 1:
                    case 2:
                        destinationArray = new byte[0x1bc];
                        offset = 0x18;
                        break;

                    case 3:
                        destinationArray = new byte[0x200];
                        offset = 0x24;
                        break;
                }
                if (offset > 0)
                {
                    int num3 = ReplayHelper.ReplayByteToInt(array, offset, 4);
                    int sourceIndex = -1;
                    while ((sourceIndex < 0) && (num3 < count))
                    {
                        int num5 = ReplayHelper.ReplayByteToInt(array, num3 + 4, 4);
                        if (showID == num5)
                        {
                            sourceIndex = num3;
                        }
                        else
                        {
                            num3 += destinationArray.Length;
                        }
                    }
                    if (sourceIndex < 0)
                    {
                        throw new ApplicationException(string.Format("Show not found on {0} at {1}.", this.theDevice.friendlyName, this.theDevice.ip));
                    }
                    Array.Copy(array, sourceIndex, destinationArray, 0, destinationArray.Length);
                }
            }
            return destinationArray;
        }

        private int Save4000GuideAsXML(bool shareReceived, bool showHDTV)
        {
            XmlTextWriter writer = new XmlTextWriter(this.guideXMLFile, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartElement("result");
            int[] numArray = new int[] { 0, 0 };
            bool flag = true;
            FileStream stream = File.OpenRead(this.guideFile);
            byte[] array = new byte[(int) stream.Length];
            stream.Read(array, 0, (int) stream.Length);
            stream.Close();
            SortedList categoryList = new SortedList();
            categoryList = this.Build4000Categories(array);
            writer.WriteStartElement("category-list");
            for (int i = 0; i < categoryList.Count; i++)
            {
                writer.WriteStartElement("category");
                writer.WriteAttributeString("categoryID", null, categoryList.GetKey(i).ToString());
                writer.WriteAttributeString("categoryName", null, categoryList.GetByIndex(i).ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            SortedList list2 = new SortedList();
            list2 = this.Build4000Channels(array, categoryList);
            writer.WriteStartElement("channel-list");
            try
            {
                int num2 = 0;
                int length = array.Length;
                int num4 = length - (0x328 + (0x270 * ReplayHelper.ReplayByteToInt(array, 8, 4)));
                byte[] destinationArray = new byte[num4];
                byte[] buffer3 = new byte[0x1bc];
                Array.Copy(array, 0x328 + (0x270 * ReplayHelper.ReplayByteToInt(array, 8, 4)), destinationArray, 0, num4);
                for (int j = 0; (num4 - (0x1bc * j)) >= 0x1bc; j++)
                {
                    string text5;
                    Array.Copy(destinationArray, 0x1bc * j, buffer3, 0, 0x1bc);
                    byte[] buffer4 = null;
                    byte[] buffer5 = null;
                    byte[] buffer6 = null;
                    DateTime time = new DateTime(0x7b2, 1, 1, 0, 0, 0, 0);
                    long num6 = ReplayHelper.ReplayByteToLong(buffer3, 120, 4);
                    int num7 = 0;
                    if ((num6 & 0x40) > 0)
                    {
                        num7 += 4;
                    }
                    if ((num6 & 0x20) > 0)
                    {
                        num7 += 8;
                    }
                    int num8 = ReplayHelper.ReplayByteToInt(buffer3, 0, 4);
                    int num9 = ReplayHelper.ReplayByteToInt(buffer3, 4, 4);
                    buffer4 = new byte[ReplayHelper.ReplayByteToInt(buffer3, 140, 1) - 1];
                    buffer5 = new byte[ReplayHelper.ReplayByteToInt(buffer3, 0x8d, 1) - 1];
                    buffer6 = new byte[ReplayHelper.ReplayByteToInt(buffer3, 0x8e, 1) - 1];
                    int qualityCode = ReplayHelper.ReplayByteToInt(buffer3, 12, 4);
                    string quality = this.GetQuality(qualityCode);
                    int num11 = ReplayHelper.ReplayByteToInt(buffer3, 0x188, 4);
                    int num12 = num11 / 60;
                    if ((num11 % 60) >= 30)
                    {
                        num12++;
                    }
                    string text2 = num12.ToString();
                    int num13 = ReplayHelper.ReplayByteToInt(buffer3, 0x7c, 4);
                    int num14 = ReplayHelper.ReplayByteToInt(buffer3, 380, 4);
                    int num15 = ReplayHelper.ReplayByteToInt(buffer3, 0x184, 4);
                    string text3 = time.AddSeconds((double) num13).ToString("yyyy-MM-dd HH:mm:ss.000", DateTimeFormatInfo.InvariantInfo);
                    Array.Copy(buffer3, 0x94 + num7, buffer4, 0, buffer4.Length);
                    Array.Copy(buffer3, ((0x94 + num7) + buffer4.Length) + 1, buffer5, 0, buffer5.Length);
                    Array.Copy(buffer3, ((((0x94 + num7) + buffer4.Length) + 1) + buffer5.Length) + 1, buffer6, 0, buffer6.Length);
                    ReplayChannelGuide guide = (ReplayChannelGuide) list2[num8];
                    string channelTitle = guide.channelTitle;
                    switch (guide.channelType)
                    {
                        case 1:
                            text5 = "Recurring";
                            break;

                        case 2:
                            text5 = "Theme";
                            break;

                        case 3:
                            text5 = "Single";
                            break;

                        case 4:
                            text5 = "Zone";
                            break;

                        default:
                            text5 = "";
                            break;
                    }
                    int channelCategory = guide.channelCategory;
                    string channelCatName = guide.channelCatName;
                    string text7 = ReplayHelper.ReplaceUTF8InString(this.encoding.GetString(buffer4));
                    string text8 = ReplayHelper.ReplaceUTF8InString(this.encoding.GetString(buffer6));
                    string text9 = ReplayHelper.ReplaceUTF8InString(this.encoding.GetString(buffer5));
                    if (shareReceived || ((num14 == 0) && (num15 < 15)))
                    {
                        if (num8 != num2)
                        {
                            if (flag)
                            {
                                flag = false;
                            }
                            else
                            {
                                writer.WriteEndElement();
                            }
                            writer.WriteStartElement("channel");
                            writer.WriteAttributeString("title", null, channelTitle);
                            writer.WriteAttributeString("channelID", null, num8.ToString());
                            writer.WriteAttributeString("channelType", null, text5);
                            writer.WriteAttributeString("categoryID", null, channelCategory.ToString());
                            writer.WriteAttributeString("categoryName", null, channelCatName);
                            guide.channelDone = true;
                        }
                        writer.WriteStartElement("show");
                        if (shareReceived && (num14 != 0))
                        {
                            writer.WriteAttributeString("title", null, text7);
                        }
                        else if (text5.Equals("Single") || text5.Equals("Recurring"))
                        {
                            writer.WriteAttributeString("title", null, channelTitle);
                        }
                        else
                        {
                            writer.WriteAttributeString("title", null, text7);
                        }
                        writer.WriteAttributeString("description", null, text8);
                        if (!showHDTV && text9.StartsWith("(HDTV) "))
                        {
                            text9 = text9.Remove(0, 7);
                        }
                        if (text9.Equals(""))
                        {
                            if (text8.Equals(""))
                            {
                                writer.WriteAttributeString("episodeTitle", null, text7);
                            }
                            else
                            {
                                if (text8.Length > 0x23)
                                {
                                    text8 = text8.Substring(0, 0x20);
                                    if (text8.LastIndexOf(" ") >= 0)
                                    {
                                        text8 = text8.Substring(0, text8.LastIndexOf(" ")) + "...";
                                    }
                                }
                                writer.WriteAttributeString("episodeTitle", null, text8);
                            }
                        }
                        else
                        {
                            writer.WriteAttributeString("episodeTitle", null, text9);
                        }
                        writer.WriteAttributeString("startTimeGMT", null, text3);
                        writer.WriteAttributeString("durationInMinutes", null, text2);
                        writer.WriteAttributeString("quality", null, quality);
                        writer.WriteAttributeString("showID", null, num9.ToString());
                        writer.WriteEndElement();
                    }
                    num2 = num8;
                }
                if (!flag)
                {
                    writer.WriteEndElement();
                }
                IList keyList = list2.GetKeyList();
                for (int k = 0; k < list2.Count; k++)
                {
                    string text11;
                    ReplayChannelGuide guide2 = (ReplayChannelGuide) list2[keyList[k]];
                    string text10 = guide2.channelTitle;
                    switch (guide2.channelType)
                    {
                        case 1:
                            text11 = "Recurring";
                            break;

                        case 2:
                            text11 = "Theme";
                            break;

                        case 3:
                            text11 = "Single";
                            break;

                        case 4:
                            text11 = "Zone";
                            break;

                        default:
                            text11 = "";
                            break;
                    }
                    int num18 = guide2.channelCategory;
                    string text12 = guide2.channelCatName;
                    if (!guide2.channelDone)
                    {
                        writer.WriteStartElement("channel");
                        writer.WriteAttributeString("title", null, text10);
                        writer.WriteAttributeString("channelID", null, keyList[k].ToString());
                        writer.WriteAttributeString("channelType", null, text11);
                        writer.WriteAttributeString("categoryID", null, num18.ToString());
                        writer.WriteAttributeString("categoryName", null, text12);
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            catch (Exception)
            {
                if (!flag)
                {
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.Flush();
                writer.Close();
                return -1;
            }
            writer.Flush();
            writer.Close();
            return 1;
        }

        private int Save5000GuideAsXML(bool shareReceived, bool showHDTV)
        {
            XmlTextWriter writer = new XmlTextWriter(this.guideXMLFile, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartElement("result");
            int[] numArray = new int[] { 0, 0 };
            FileStream stream = File.OpenRead(this.guideFile);
            byte[] array = new byte[(int) stream.Length];
            stream.Read(array, 0, (int) stream.Length);
            stream.Close();
            bool flag = true;
            SortedList categoryList = new SortedList();
            categoryList = this.Build5000Categories(array);
            writer.WriteStartElement("category-list");
            for (int i = 0; i < categoryList.Count; i++)
            {
                writer.WriteStartElement("category");
                writer.WriteAttributeString("categoryID", null, categoryList.GetKey(i).ToString());
                writer.WriteAttributeString("categoryName", null, categoryList.GetByIndex(i).ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            SortedList list2 = new SortedList();
            list2 = this.Build5000Channels(array, categoryList);
            writer.WriteStartElement("channel-list");
            try
            {
                int num2 = 0;
                int length = array.Length;
                int num4 = length - (840 + (0x2c8 * ReplayHelper.ReplayByteToInt(array, 0x10, 4)));
                byte[] destinationArray = new byte[num4];
                byte[] buffer3 = new byte[0x200];
                Array.Copy(array, 840 + (0x2c8 * ReplayHelper.ReplayByteToInt(array, 0x10, 4)), destinationArray, 0, num4);
                byte[] buffer4 = null;
                byte[] buffer5 = null;
                byte[] buffer6 = null;
                for (int j = 0; (num4 - (0x200 * j)) >= 0x200; j++)
                {
                    string text5;
                    Array.Copy(destinationArray, 0x200 * j, buffer3, 0, 0x200);
                    DateTime time = new DateTime(0x7b2, 1, 1, 0, 0, 0, 0);
                    long num6 = ReplayHelper.ReplayByteToLong(buffer3, 120, 4);
                    int num7 = 0;
                    if ((num6 & 0x40) > 0)
                    {
                        num7 += 4;
                    }
                    if ((num6 & 0x20) > 0)
                    {
                        num7 += 8;
                    }
                    int num8 = ReplayHelper.ReplayByteToInt(buffer3, 0, 4);
                    int num9 = ReplayHelper.ReplayByteToInt(buffer3, 4, 4);
                    buffer4 = new byte[ReplayHelper.ReplayByteToInt(buffer3, 140, 1) - 1];
                    buffer5 = new byte[ReplayHelper.ReplayByteToInt(buffer3, 0x8d, 1) - 1];
                    buffer6 = new byte[ReplayHelper.ReplayByteToInt(buffer3, 0x8e, 1) - 1];
                    int qualityCode = ReplayHelper.ReplayByteToInt(buffer3, 12, 4);
                    string quality = this.GetQuality(qualityCode);
                    int num11 = ReplayHelper.ReplayByteToInt(buffer3, 0x188, 4);
                    int num12 = num11 / 60;
                    if ((num11 % 60) >= 30)
                    {
                        num12++;
                    }
                    string text2 = num12.ToString();
                    int num13 = ReplayHelper.ReplayByteToInt(buffer3, 0x7c, 4);
                    int num14 = ReplayHelper.ReplayByteToInt(buffer3, 380, 4);
                    int num15 = ReplayHelper.ReplayByteToInt(buffer3, 0x184, 4);
                    string text3 = time.AddSeconds((double) num13).ToString("yyyy-MM-dd HH:mm:ss.000", DateTimeFormatInfo.InvariantInfo);
                    Array.Copy(buffer3, 0x94 + num7, buffer4, 0, buffer4.Length);
                    Array.Copy(buffer3, ((0x94 + num7) + buffer4.Length) + 1, buffer5, 0, buffer5.Length);
                    Array.Copy(buffer3, ((((0x94 + num7) + buffer4.Length) + 1) + buffer5.Length) + 1, buffer6, 0, buffer6.Length);
                    ReplayChannelGuide guide = (ReplayChannelGuide) list2[num8];
                    string channelTitle = guide.channelTitle;
                    switch (guide.channelType)
                    {
                        case 1:
                            text5 = "Recurring";
                            break;

                        case 2:
                            text5 = "Theme";
                            break;

                        case 3:
                            text5 = "Single";
                            break;

                        case 4:
                            text5 = "Zone";
                            break;

                        default:
                            text5 = "";
                            break;
                    }
                    int channelCategory = guide.channelCategory;
                    string channelCatName = guide.channelCatName;
                    string text7 = ReplayHelper.ReplaceUTF8InString(this.encoding.GetString(buffer4));
                    string text8 = ReplayHelper.ReplaceUTF8InString(this.encoding.GetString(buffer6));
                    string text9 = ReplayHelper.ReplaceUTF8InString(this.encoding.GetString(buffer5));
                    if (shareReceived || ((num14 == 0) && (num15 < 15)))
                    {
                        if (num8 != num2)
                        {
                            if (flag)
                            {
                                flag = false;
                            }
                            else
                            {
                                writer.WriteEndElement();
                            }
                            writer.WriteStartElement("channel");
                            writer.WriteAttributeString("title", null, channelTitle);
                            writer.WriteAttributeString("channelID", null, num8.ToString());
                            writer.WriteAttributeString("channelType", null, text5);
                            writer.WriteAttributeString("categoryID", null, channelCategory.ToString());
                            writer.WriteAttributeString("categoryName", null, channelCatName);
                            guide.channelDone = true;
                        }
                        writer.WriteStartElement("show");
                        if (shareReceived && (num14 != 0))
                        {
                            writer.WriteAttributeString("title", null, text7);
                        }
                        else if (text5.Equals("Single") || text5.Equals("Recurring"))
                        {
                            writer.WriteAttributeString("title", null, channelTitle);
                        }
                        else
                        {
                            writer.WriteAttributeString("title", null, text7);
                        }
                        writer.WriteAttributeString("description", null, text8);
                        if (!showHDTV && text9.StartsWith("(HDTV) "))
                        {
                            text9 = text9.Remove(0, 7);
                        }
                        if (text9.Equals(""))
                        {
                            if (text8.Equals(""))
                            {
                                writer.WriteAttributeString("episodeTitle", null, text7);
                            }
                            else
                            {
                                if (text8.Length > 0x23)
                                {
                                    text8 = text8.Substring(0, 0x20);
                                    if (text8.LastIndexOf(" ") >= 0)
                                    {
                                        text8 = text8.Substring(0, text8.LastIndexOf(" ")) + "...";
                                    }
                                }
                                writer.WriteAttributeString("episodeTitle", null, text8);
                            }
                        }
                        else
                        {
                            writer.WriteAttributeString("episodeTitle", null, text9);
                        }
                        writer.WriteAttributeString("startTimeGMT", null, text3);
                        writer.WriteAttributeString("durationInMinutes", null, text2);
                        writer.WriteAttributeString("quality", null, quality);
                        writer.WriteAttributeString("showID", null, num9.ToString());
                        writer.WriteEndElement();
                    }
                    num2 = num8;
                }
                if (!flag)
                {
                    writer.WriteEndElement();
                }
                IList keyList = list2.GetKeyList();
                for (int k = 0; k < list2.Count; k++)
                {
                    string text11;
                    ReplayChannelGuide guide2 = (ReplayChannelGuide) list2[keyList[k]];
                    string text10 = guide2.channelTitle;
                    switch (guide2.channelType)
                    {
                        case 1:
                            text11 = "Recurring";
                            break;

                        case 2:
                            text11 = "Theme";
                            break;

                        case 3:
                            text11 = "Single";
                            break;

                        case 4:
                            text11 = "Zone";
                            break;

                        default:
                            text11 = "";
                            break;
                    }
                    int num18 = guide2.channelCategory;
                    string text12 = guide2.channelCatName;
                    if (!guide2.channelDone)
                    {
                        writer.WriteStartElement("channel");
                        writer.WriteAttributeString("title", null, text10);
                        writer.WriteAttributeString("channelID", null, keyList[k].ToString());
                        writer.WriteAttributeString("channelType", null, text11);
                        writer.WriteAttributeString("categoryID", null, num18.ToString());
                        writer.WriteAttributeString("categoryName", null, text12);
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            catch (Exception exception)
            {
                ReplayLogger.Log("Save5000GuideAsXML Exception" + exception.ToString());
                if (!flag)
                {
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.Flush();
                writer.Close();
                return -1;
            }
            writer.Flush();
            writer.Close();
            return 1;
        }

        public int SaveGuideAsXML(bool shareReceived, bool showHDTV)
        {
            if (File.Exists(this.guideFile))
            {
                switch (this.CheckVersion())
                {
                    case 1:
                    case 2:
                        return this.Save4000GuideAsXML(shareReceived, showHDTV);

                    case 3:
                        return this.Save5000GuideAsXML(shareReceived, showHDTV);
                }
            }
            return -1;
        }
    }
}
