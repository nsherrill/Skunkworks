using GP.Managers.DataRetrievalManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace GP.Clients.DataRetrieverSvc
{
    public partial class DataRetrieverSvc : ServiceBase
    {
        IDataRetrievalMgr dataRetrieverMgr = new DataRetrievalMgr();
        public DataRetrieverSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ConsoleStart(args);
        }

        protected override void OnStop()
        {
            ConsoleStop();
        }

        internal void ConsoleStart(string[] args)
        {
            bool pullHistoricalData = true;
            bool pullFutureGames = true;
            bool pullCurrentLeagues = true;
            bool signUpForLeagues = true;
            bool getBaseballStats = true;
            long leagueCap = -1;

            if (args != null
                && args.Length > 0)
            {
                //foreach (var arg in args)
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];

                    if (!string.IsNullOrEmpty(arg))
                        switch (arg.ToLower())
                        {
                            case "-nopullhistory":
                                pullHistoricalData = false;
                                getBaseballStats = false;
                                break;

                            case "-nopullgames":
                                pullFutureGames = false;
                                break;

                            case "-nopullleagues":
                                pullCurrentLeagues = false;
                                break;

                            case "-nosignup":
                                signUpForLeagues = false;
                                break;

                            case "-capleagues":
                                leagueCap = long.Parse(args[i + 1]);
                                i++;
                                break;

                            default:
                                throw new NotImplementedException("Argument unknown: " + arg);
                        }
                }
            }

            if (pullHistoricalData
                && false)
                dataRetrieverMgr.GetBaseballData();

            if (getBaseballStats)
                dataRetrieverMgr.GetCurrentBaseballStats();

            if (pullFutureGames
                && false)
                dataRetrieverMgr.RetrieveFutureGames();

            if (pullCurrentLeagues)
                dataRetrieverMgr.RetrieveCurrentLeagues();

            if (signUpForLeagues)
                dataRetrieverMgr.SignUpForLeagues(leagueCap);

        }

        internal void ConsoleStop()
        {
        }
    }
}
