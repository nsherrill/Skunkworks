using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwilioConsole.Accessors;
using TwilioConsole.Managers;

namespace TwilioConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //var poAcc = new POEditorAccessor();
            //Task.Run(async () =>
            //{
            //    var result = await poAcc.GetTerms();
            //}).Wait();


            var newMgr = new TwilioConsoleManager();
            newMgr.Start();
        }
    }
}
