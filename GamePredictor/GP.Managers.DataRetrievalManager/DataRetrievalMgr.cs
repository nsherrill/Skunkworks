﻿using GP.Accessors.DatabaseAccessor;
using GP.Engines.DataRetrievalEngine;
using GP.Engines.RankingsGeneratorEngine;
using GP.Engines.RemoteDataEngine;
using GP.Shared.Common;
using GP.Shared.Common.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Managers.DataRetrievalManager
{
    public interface IDataRetrievalMgr
    {
        void GetBaseballData();

        void RetrieveFutureGames();

        void RetrieveCurrentLeagues();

        void SignUpForLeagues();
    }

    public class DataRetrievalMgr : IDataRetrievalMgr
    {
        IBaseballDataRetrieverEng dataEng = new BaseballDataRetrieverEng();
        IRemoteDataEng remoteDataEng = new RemoteDataEng();
        ILocalBaseballDataAcc localBaseballDataAcc = new LocalBaseballDataAcc();
        IRankingsGeneratorEng rankingsGeneratorEng = new RankingsGeneratorEng();
        ILocalBaseballDataAcc localBaseballAcc = new LocalBaseballDataAcc();

        private GPChromeDriver cachedDriver { get; set; }

        public void GetBaseballData()
        {
            DateTime date = dataEng.GetMaxAvailableDataDate();
            //DateTime date = DateTime.Parse("3/17/2015");
            while (date < DateTime.Now)//for (int i = 0; i < 2; i++)
            {
                MassData data = dataEng.EnsureLocalDataForDate(date);

                date = date.AddDays(1);
            }
        }

        public void RetrieveFutureGames()
        {
            List<FutureGameEvent> games = new List<FutureGameEvent>();
            FutureGameEvent[] tempGames = dataEng.GetFutureGamesData(DateTime.Now.Date);
            if (tempGames != null)
                games.AddRange(tempGames);
            var tempGames2 = dataEng.GetFutureGamesData(DateTime.Now.AddDays(1).Date);
            if (tempGames2 != null)
                games.AddRange(tempGames2);
        }

        public void RetrieveCurrentLeagues()
        {
            if (cachedDriver == null
                || !cachedDriver.HasLoggedIn)
            {
                cachedDriver = remoteDataEng.GenerateDriver();
            }
            dataEng.PullAvailableLeagues(cachedDriver);
        }

        public void SignUpForLeagues()
        {
            try
            {
                if (cachedDriver == null
                    || !cachedDriver.HasLoggedIn)
                {
                    cachedDriver = remoteDataEng.GenerateDriver();
                }

                double minDollar = ConfigHelper.MinimumBuyIn;
                double maxDollar = ConfigHelper.MaximumBuyIn;
                var allInterestedLeagues = localBaseballAcc.UpdateFutureFantasyLeagueInterest(minDollar, maxDollar);

                RankingsConfiguration config = localBaseballDataAcc.GetRankingsConfiguration();

                //foreach (var interestedLeague in allInterestedLeagues)
                for (int i = 0; i < allInterestedLeagues.Length
                    && i < 1
                    ; i++)
                {
                    try
                    {
                        var interestedLeague = allInterestedLeagues[i];

                        FantasyPlayer[] players = dataEng.GetPlayersForLeague(cachedDriver, interestedLeague.ForeignId, interestedLeague.Url);

                        dataEng.GetandWriteLeagueRoster(cachedDriver, interestedLeague);

                        FantasyPlayerRanking[] playerOptions = localBaseballDataAcc.GetPlayerRankings(interestedLeague.ForeignId);

                        var configType = config.GetConfigType(i, allInterestedLeagues.Length);
                        var roster = rankingsGeneratorEng.GenerateRoster(interestedLeague, playerOptions, configType);

                        bool result = dataEng.RegisterForLeague(cachedDriver, interestedLeague, roster);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            catch
            {

            }


            if (cachedDriver != null
                && cachedDriver.HasLoggedIn)
            {
                cachedDriver.Quit();
                cachedDriver.Dispose();
            }
        }

        public void TEST_DESERIALIZE()
        {
            new GP.Accessors.RemoteLeagueAccessor.RemoteFanDuelAcc().TEST_DESERIALIZE();
        }
    }
}
