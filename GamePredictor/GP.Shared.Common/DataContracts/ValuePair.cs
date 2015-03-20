using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Shared.Common
{
    public class ValuePair
    {
        public ValuePair(string key, object value)
        {
            this.Key = key;
            this.Value = value;
        }
        public string Key { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            if (Key != null && Value != null)
                return Key + "," + Value;
            return base.ToString();
        }
    }
}
