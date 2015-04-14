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

            if (args != null
                && args.Length > 0)
            {
                foreach (var arg in args)
                    if (!string.IsNullOrEmpty(arg))
                        switch (arg.ToLower())
                        {
                            case "-nopullhistory":
                                pullHistoricalData = false;
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

                            default:
                                throw new NotImplementedException("Argument unknown: " + arg);
                        }
            }

            if (pullHistoricalData)
                dataRetrieverMgr.GetBaseballData();

            if (pullFutureGames)
                dataRetrieverMgr.RetrieveFutureGames();

            if (pullCurrentLeagues)
                dataRetrieverMgr.RetrieveCurrentLeagues();

            if (signUpForLeagues)
                dataRetrieverMgr.SignUpForLeagues();

        }

        internal void ConsoleStop()
        {
        }
    }
}
