using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zoggr
{
    public class ZoggrImporter
    {
        static public bool ClearShows(string replaySerialNumber)
        {
            return true;
        }

        static public bool AddShow(ImportDetails details)
        {
            return true;
        }

        public struct ImportDetails
        {
            public string ReplaySerialNumber;
            public string TabId;
            public string TabName;
            public string ChannelId;
            public string ChannelType;
            public string ChannelTitle;
            public string ShowTitle;
            public string EpisodeId;
            public string EpisodeDescription;
            public string EpisodeTitle;
            public DateTime EpisodeStartTimeGMT;
            public int EpisodeDuration;
            public string EpisodeQuality;
        }
    }
}
