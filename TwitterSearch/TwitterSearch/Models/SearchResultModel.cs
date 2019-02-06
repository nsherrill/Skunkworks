using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwitterSearch.Models
{
    public class SearchResultModel
    {
        public TweetModel[] Items { get; set; }
        public string Error { get; set; }
        public string SearchText { get; set; }
    }
}