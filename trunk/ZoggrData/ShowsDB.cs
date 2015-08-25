using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZoggrData
{
    public class ShowsDB
    {
        ZoggrDataContext db = new ZoggrDataContext();

        public System.Linq.IOrderedQueryable<Show> ListShows()
        {
            var shows = from s in db.Shows
                        orderby s.ShowTitle, s.EpisodeTitle
                        select s;
            return shows;
        }

        public void ClearShows(string replaySerialNumber)
        {
            var oldShows = from s in db.Shows
                           where s.ReplaySerialNumber == replaySerialNumber
                           select s;
            db.Shows.DeleteAllOnSubmit(oldShows);
        }

        public void AddShow(Show show)
        {
            db.Shows.InsertOnSubmit(show);
        }

        public void SubmitChanges()
        {
            db.SubmitChanges();
        }
    }
}
