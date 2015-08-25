using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.Xml.Linq;
using System.Transactions;

namespace Zoggr.Web
{
    public partial class Upload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Files.Count > 0)
            {
                SaveFile();
            }
        }

        private void SaveFile()
        {
            HttpPostedFile file;
            int showCount = 0;
            for (int i = 0; i <= Request.Files.Count - 1; i++)
            {
                file = Request.Files[i];
                if (file != null && file.FileName != String.Empty && file.ContentLength > 0)
                {
                    string fullPath = Server.MapPath("/upload/") + file.FileName;
                    file.SaveAs(fullPath);
                    showCount += ProcessFile(fullPath);
                }
            }
            MessageLabel.Text = String.Format("{0} shows uploaded.", showCount);
        }

        private int ProcessFile(string path)
        {
            string replaySerialNumber = System.IO.Path.GetFileNameWithoutExtension(path);
            ZoggrData.ShowsDB db = new ZoggrData.ShowsDB();

            int ShowCount = 0;
            using (TransactionScope transactions = new TransactionScope())
            {
                // Delete Existing Shows
                db.ClearShows(replaySerialNumber);

                // Add New Shows
                XmlDocument document = new XmlDocument();
                document.Load(path);

                foreach (XmlNode node in document.SelectNodes("/result/channel-list/channel/show"))
                {
                    XmlElement ShowNode = node as XmlElement;
                    if ((ShowNode != null))
                    {
                        XmlElement ChannelNode = ShowNode.ParentNode as XmlElement;

                        var show = new ZoggrData.Show();
                        show.ReplaySerialNumber = replaySerialNumber;
                        show.TabId = ChannelNode.GetAttribute("categoryID");
                        show.TabName = ChannelNode.GetAttribute("categoryName");
                        show.ChannelId = ChannelNode.GetAttribute("channelID");
                        show.ChannelType = ChannelNode.GetAttribute("channelType");
                        show.ChannelTitle = ChannelNode.GetAttribute("title");
                        show.ShowTitle = ShowNode.GetAttribute("title");
                        show.EpisodeId = ShowNode.GetAttribute("showID");
                        show.EpisodeDescription = ShowNode.GetAttribute("description");
                        show.EpisodeTitle = ShowNode.GetAttribute("episodeTitle");
                        show.EpisodeStartTimeGMT = Convert.ToDateTime(ShowNode.GetAttribute("startTimeGMT"));
                        show.EpisodeDuration = Convert.ToInt32(ShowNode.GetAttribute("durationInMinutes"));
                        show.EpisodeQuality = ShowNode.GetAttribute("quality");

                        db.AddShow(show);
                        ShowCount += 1;
                    }
                }
                db.SubmitChanges();
                transactions.Complete();
            }
            return ShowCount;
        }
    }
}
