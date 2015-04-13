using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace GP.Clients.DataRetrieverSvc
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args != null
                && args.Length > 0
                && args[0].Equals("console", StringComparison.InvariantCultureIgnoreCase))
            {
                var dataRetriverSvc = new DataRetrieverSvc();
                var list = args.ToList();
                list.RemoveAt(0);
                dataRetriverSvc.ConsoleStart(list.ToArray());
                Console.WriteLine("Press enter to end service");
                Console.ReadLine();
                dataRetriverSvc.ConsoleStop();
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
                { 
                    new DataRetrieverSvc() 
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
