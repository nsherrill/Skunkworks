using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwitterSearchBackend;

namespace TwitterSearch.Models
{
    public class SearchResultModel
    {
        public TweetContract[] Items { get; set; }
        public string Error { get; set; }
        public string SearchText { get; set; }
    }
}