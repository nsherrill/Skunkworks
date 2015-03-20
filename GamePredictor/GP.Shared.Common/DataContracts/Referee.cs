using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Shared.Common
{
    public class Referee
    {
        public long Id { get; set; }
        public string ForeignId { get; set; }
        public string Name { get; set; }
        public RefereeType Type { get; set; }
        public SportType SportType { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(Name))
                return Type.ToString() + ": " + Name;
            return base.ToString();
        }
    }
}
