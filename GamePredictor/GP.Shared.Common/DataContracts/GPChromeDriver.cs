using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Shared.Common.DataContracts
{
    public class GPChromeDriver : ChromeDriver
    {
        public GPChromeDriver(string directory)
            : base(directory)
        {

        }

        public bool HasLoggedIn { get; set; }
    }
}
