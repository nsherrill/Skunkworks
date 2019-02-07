using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterSearchBackend
{
    public class TweetContract
    {
        public string UserName { get; set; }
        public string Text { get; set; }
        public DateTime CreateDate { get; set; }

        public TweetContract()
        { }

        public TweetContract(string text, string userName, DateTime createDate)
            : base()
        {
            this.Text = text;
            this.UserName = userName;
            this.CreateDate = createDate;
        }
    }
}
