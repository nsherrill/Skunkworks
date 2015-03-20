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
            ConsoleStart();
        }

        protected override void OnStop()
        {
            ConsoleStop();
        }

        internal void ConsoleStart()
        {
            dataRetrieverMgr.GetBaseballData();
            dataRetrieverMgr.RetrieveFutureGames();
            dataRetrieverMgr.SignUpForLeagues();


            //new DataRetrievalMgr().TEST_DESERIALIZE();
        }

        internal void ConsoleStop()
        {
        }
    }
}
