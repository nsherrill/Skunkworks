using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterSearchBackend
{
    public class TweetModel
    {
        public string UserName { get; set; }
        public string Text { get; set; }

        public TweetModel()
        { }

        public TweetModel(string text, string userName)
            : base()
        {
            this.Text = text;
            this.UserName = userName;
        }
    }
}
