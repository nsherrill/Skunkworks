using GP.Accessors.DatabaseAccessor;
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

        void SignUpForLeagues(long leagueCap = -1);

        void GetCurrentBaseballStats();
    }

    public class DataRetrievalMgr : IDataRetrievalMgr
    {
        IBaseballDataRetrieverEng dataEng = new BaseballDataRetrieverEng();
        IRemoteDataEng remoteDataEng = new RemoteDataEng();
        ILocalBaseballDataAcc localBaseballDataAcc = new LocalBaseballDataAcc();
        IRankingsGeneratorEng rankingsGeneratorEng = new RankingsGeneratorEng();

        private GPChromeDriver cachedDriver { get; set; }

        public void GetBaseballData()
        {
            return;
            DateTime date = dataEng.GetMaxAvailableDataDate(SourceType.ESPN);
            //DateTime date = DateTime.Parse("3/17/2015");
            while (date < DateTime.Now)//for (int i = 0; i < 2; i++)
            {
                MassData data = dataEng.deprecated_EnsureLocalDataForDate(date);

                date = date.AddDays(1);
            }
        }

        public void RetrieveFutureGames()
        {
            return;
            List<FutureGameEvent> games = new List<FutureGameEvent>();
            FutureGameEvent[] tempGames = dataEng.deprecated_GetFutureGamesData(DateTime.Now.Date);
            if (tempGames != null)
                games.AddRange(tempGames);
            var tempGames2 = dataEng.deprecated_GetFutureGamesData(DateTime.Now.AddDays(1).Date);
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

        public void SignUpForLeagues(long leagueCap = -1)
        {
            if (firstInvalidDate < DateTime.Now.Date)
            {
                InvalidPlayers.Clear();
                firstInvalidDate = DateTime.Now;
            }

            try
            {
                if (cachedDriver == null
                    || !cachedDriver.HasLoggedIn)
                {
                    cachedDriver = remoteDataEng.GenerateDriver();
                }

                double minDollar = ConfigHelper.MinimumBuyIn;
                double maxDollar = ConfigHelper.MaximumBuyIn;
                var allInterestedLeagues = localBaseballDataAcc.UpdateFutureFantasyLeagueInterest(minDollar, maxDollar);

                RankingsConfiguration config = localBaseballDataAcc.GetRankingsConfiguration();
                int registeredCount = 0;

                for (int i = 0;
                    i < allInterestedLeagues.Length
                        && (registeredCount < leagueCap || leagueCap < 0);
                    i++)
                {
                    try
                    {
                        var interestedLeague = allInterestedLeagues[i];

                        Console.WriteLine("Attempting to sign up for league: {0}", interestedLeague.ForeignId);
                        FantasyPlayer[] players = dataEng.GetPlayersForLeague(cachedDriver, interestedLeague.ForeignId, interestedLeague.Url);

                        var isValid = dataEng.GetandWriteLeagueRoster(cachedDriver, interestedLeague);
                        if (!isValid)
                        {
                            continue;
                        }
                        FantasyPlayerRanking[] playerOptions = localBaseballDataAcc.GetPlayerRankings(interestedLeague.ForeignId);
                        playerOptions = playerOptions.Where(p => !InvalidPlayers.Contains(p.Id)).ToArray();

                        int maxAttempts = 3;
                        bool result = false;
                        ConfigType configType = ConfigType.TopAvailablePPG;
                        for (int attempt = 0; attempt < maxAttempts && !result; attempt++)
                        {
                            configType = config.GetConfigType(i, allInterestedLeagues.Length);
                            var roster = rankingsGeneratorEng.GenerateRoster(interestedLeague, playerOptions, configType);

                            if (roster == null)
                                continue;

                            result = dataEng.RegisterForLeague(cachedDriver, interestedLeague, roster, InvalidatePlayerById);
                        }
                        if (result)
                        {
                            Console.WriteLine("Successfully registered for league {0} as {1}!", interestedLeague.ForeignId, configType);
                            localBaseballDataAcc.RecordSuccessfulLeagueSignup(interestedLeague.Id, playerOptions.Select(p => p.Id).ToArray(), configType);
                            registeredCount++;
                        }
                        else
                            Console.WriteLine("  Just couldn't make this league work... next!");
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

        public void GetCurrentBaseballStats()
        {
            DateTime date = dataEng.GetMaxAvailableDataDate(SourceType.SportingCharts);

            if (date < DateTime.Now.Date)
            {
                Console.WriteLine("Currently have stale stats data, pulling new stats.");
                var stats = dataEng.GetRecentStats();

                var allHittingStats = stats.Where(s => s.DataType == PlayerDataType.Hitting).ToArray();
                Console.WriteLine("Writing {0} hitting stats to db", allHittingStats.Length);
                List<CurrentPlayerStats> currentStats = new List<CurrentPlayerStats>();
                for (int i = 0; i < allHittingStats.Count(); i++)
                {
                    currentStats.Add(allHittingStats[i]);
                    if (i > 0 && i % 100 == 0)
                    {
                        localBaseballDataAcc.WriteStats(currentStats.ToArray(), SportType.Baseball, PlayerDataType.Hitting);
                        currentStats.Clear();
                    }
                }
                if (currentStats.Count > 0)
                    localBaseballDataAcc.WriteStats(currentStats.ToArray(), SportType.Baseball, PlayerDataType.Hitting);

                currentStats.Clear();

                var allPitchingStats = stats.Where(s => s.DataType == PlayerDataType.Pitching).ToArray();
                Console.WriteLine("Writing {0} pitching stats to db", allPitchingStats.Length);
                for (int i = 0; i < allPitchingStats.Count(); i++)
                {
                    currentStats.Add(allPitchingStats[i]);
                    if (i > 0 && i % 100 == 0)
                    {
                        localBaseballDataAcc.WriteStats(currentStats.ToArray(), SportType.Baseball, PlayerDataType.Pitching);
                        currentStats.Clear();
                    }
                }
                if (currentStats.Count > 0)
                    localBaseballDataAcc.WriteStats(currentStats.ToArray(), SportType.Baseball, PlayerDataType.Pitching);
            }
        }

        List<long> InvalidPlayers = new List<long>();
        DateTime firstInvalidDate = DateTime.MinValue;
        private void InvalidatePlayerById(long playerId)
        {
            InvalidPlayers.Add(playerId);
        }
    }
}
