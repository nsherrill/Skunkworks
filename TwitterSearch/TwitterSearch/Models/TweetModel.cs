using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwitterSearchBackend;

namespace TwitterSearch.Models
{
    public class TweetModel
    {
        public string UserName { get; set; }
        public string Text { get; set; }
        public DateTime CreateDate { get; set; }

        public TweetModel()
        {

        }

        public TweetModel(TweetContract source)
            : base()
        {
            this.UserName = source.UserName;
            this.Text = source.Text;
            this.CreateDate = source.CreateDate;
        }
    }
}