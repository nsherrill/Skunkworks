using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Shared.Common
{
    public class Team
    {
        public long Id { get; set; }
        public string ForeignId { get; set; }
        public string Name { get; set; }
        public SportType Sport { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(Name))
                return Name;
            return base.ToString();
        }
    }
}
