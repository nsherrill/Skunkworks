using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterSearchBackend
{
    public interface ITwitterSearchServiceBase
    {
        string TestMe(string source);
    }
    public abstract class TwitterSearchServiceBase
    {
        public virtual string TestMe(string source = null)
        {
            if (string.IsNullOrEmpty(source))
                return this.GetType().ToString();

            return source + ", " + this.GetType();
        }
    }
}
