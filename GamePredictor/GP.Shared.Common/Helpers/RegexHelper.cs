using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GP.Shared.Common
{
    public static class RegexHelper
    {
        public static string GetRegex(string regex, string source, int groupIndex = 0)
        {
            Match match = Regex.Match(source, regex,
                RegexOptions.IgnoreCase);

            // Here we check the Match instance.
            if (match.Success)
            {
                // Finally, we get the Group value and display it.
                string key = match.Groups[groupIndex].Value;
                return key;
            }
            return null;
        }

        public static string[] GetAllRegex(string regex, string source, int groupIndex = 0, RegexOptions options = RegexOptions.IgnoreCase)
        {
            var matches = Regex.Matches(source, regex,
                options);

            List<string> result = new List<string>();
            if (matches != null
                && matches.Count > 0)
            {
                for (int m = 0; m < matches.Count; m++)
                    if (groupIndex >= 0)
                    {
                        result.Add(matches[m].Groups[groupIndex].Value);
                    }
                    else
                    {
                        for (int c = 0; c < matches[m].Groups.Count; c++)
                        {
                            if (!string.IsNullOrEmpty(matches[m].Groups[c].Value))
                                result.Add(matches[m].Groups[c].Value);
                        }
                    }

            }

            return result.ToArray();
        }
    }
}
