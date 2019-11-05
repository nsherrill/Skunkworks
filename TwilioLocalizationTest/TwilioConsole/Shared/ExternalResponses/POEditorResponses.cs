using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwilioConsole.Shared.ExternalResponses
{
    public class Response
    {
        public string status { get; set; }
        public string code { get; set; }
        public string message { get; set; }
    }

    public class Translation
    {
        public string content { get; set; }
        public int fuzzy { get; set; }
        public object updated { get; set; }
    }

    public class Term
    {
        public string term { get; set; }
        public string context { get; set; }
        public string plural { get; set; }
        public DateTime created { get; set; }
        public object updated { get; set; }
        public Translation translation { get; set; }
        public string reference { get; set; }
        public List<object> tags { get; set; }
        public string comment { get; set; }
    }

    public class Result
    {
        public List<Term> terms { get; set; }
    }

    public class RootObject
    {
        public Response response { get; set; }
        public Result result { get; set; }
    }
}
