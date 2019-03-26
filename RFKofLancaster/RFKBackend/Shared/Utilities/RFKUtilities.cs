using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Shared
{
    public static class RFKUtilities
    {
        public static int DetermineCurrentYear()
        {
            return DateTime.Now.Month < 8 ? DateTime.Now.Year : DateTime.Now.Year + 1;
        }
    }
}
