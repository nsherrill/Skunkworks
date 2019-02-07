using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterSearchBackend
{
    public class TweetArrayResult
    {
        public TweetContract[] Items { get; set; }
        public string Error { get; set; }
    }
}
