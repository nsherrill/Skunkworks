using GP.Accessors.DatabaseAccessor;
using GP.Accessors.RemoteBaseballDataAccessor;
using GP.Accessors.RemoteLeagueAccessor;
using GP.Shared.Common;
using GP.Shared.Common.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Engines.DataRetrievalEngine
{
    public interface IBaseballDataRetrieverEng
    {
        MassData deprecated_EnsureLocalDataForDate(DateTime dateTime);

        FutureGameEvent[] deprecated_GetFutureGamesData(DateTime dateTime);

        void PullAvailableLeagues(GPChromeDriver chromeDriver);

        FantasyPlayer[] GetPlayersForLeague(GPChromeDriver cachedDriver, string leagueForeignId, string url);

        bool GetandWriteLeagueRoster(GPChromeDriver cachedDriver, FantasyLeagueEntry interestedLeague);

        DateTime GetMaxAvailableDataDate(SourceType sourceType);

        bool RegisterForLeague(GPChromeDriver cachedDriver, FantasyLeagueEntry interestedLeague, FantasyRoster roster, Action<long> onPlayerFail);

        CurrentPlayerStats[] GetRecentStats();
    }

    public class BaseballDataRetrieverEng : IBaseballDataRetrieverEng
    {
        ILocalBaseballDataAcc localBaseballAcc = new LocalBaseballDataAcc();
        IRemoteBaseballDataAcc remoteBaseballAcc = new SportingCharts_RemoteBaseballDataAcc();
        IRemoteFanDuelAcc fanDuelAcc = new RemoteFanDuelAcc();

        #region deprecated
        public MassData deprecated_EnsureLocalDataForDate(DateTime desiredDate)
        {
            MassData result = localBaseballAcc.FindDataForDate(desiredDate);
            if (result == null
                || result.Games == null)
            {
                Console.WriteLine("Getting remote baseball data for " + desiredDate.ToShortDateString());
                result = remoteBaseballAcc.deprecated_FindAllDataForDate(desiredDate);
                foreach (var game in result.Games)
                {
                    if (game.PlayerStats != null
                        && game.PlayerStats.Length > 0)
                    {
                        localBaseballAcc.WriteGameData(game);
                    }
                }
            }

            return result;
        }

        public FutureGameEvent[] deprecated_GetFutureGamesData(DateTime desiredDate)
        {
            FutureGameEvent[] results = localBaseballAcc.FindFutureGamesForDate(desiredDate);

            if (results == null
                || results.Length == 0)
            {
                Console.WriteLine("Getting remote future baseball data for " + desiredDate.ToShortDateString());
                results = remoteBaseballAcc.deprecated_FindFutureGamesForDate(desiredDate);

                foreach (var game in results)
                {
                    if (game.StartingPitchers != null
                        && game.StartingPitchers.Length > 0)
                    {
                        localBaseballAcc.WriteFutureGameData(game);
                    }
                }
            }

            return results;
        }
        #endregion

        public CurrentPlayerStats[] GetRecentStats()
        {
            string sessionId = Guid.NewGuid().ToString();

            var hittingStats = remoteBaseballAcc.GetCurrentPlayerHittingStats(sessionId);
            var pitchingStats = remoteBaseballAcc.GetCurrentPlayerPitchingStats(sessionId);
            List<CurrentPlayerStats> results = new List<CurrentPlayerStats>();
            if (hittingStats != null)
                results.AddRange(hittingStats);
            if (pitchingStats != null)
                results.AddRange(pitchingStats);
            return results.ToArray();
        }

        private DateTime lastLeagueRetrievalDate = DateTime.MinValue;
        public void PullAvailableLeagues(GPChromeDriver chromeDriver)
        {
            Console.WriteLine("Getting remote fantasy leagues on " + DateTime.Now.ToString());
            //get remotely
            FantasyLeagueEntry[] remoteResults = null;

            int attemptCount = 0;
            while ((remoteResults == null
                || remoteResults.Length == 0)
                && attemptCount < 5)
            {
                remoteResults = fanDuelAcc.GetAllLeagues(chromeDriver);
                attemptCount++;
            }

            if (remoteResults != null
                && remoteResults.Length > 0)
            {
                List<FantasyLeagueEntry> temp = new List<FantasyLeagueEntry>();
                for (int i = 0; i < remoteResults.Length; i++)
                {
                    temp.Add(remoteResults[i]);
                    if (i > 5 && i % 50 == 0)
                    {
                        if (temp.Count > 0)
                            localBaseballAcc.WriteFantasyLeagues(temp.ToArray());
                        temp.Clear();
                    }
                }
                if (temp.Count > 0)
                    localBaseballAcc.WriteFantasyLeagues(temp.ToArray());
            }
        }

        public FantasyPlayer[] GetPlayersForLeague(GPChromeDriver cachedDriver, string leagueForeignId, string url)
        {
            FantasyPlayer[] players = localBaseballAcc.FindAllPlayersForLeague(leagueForeignId);

            if (players == null
                || players.Length == 0)
            {
                players = fanDuelAcc.GetAllPlayers(cachedDriver, leagueForeignId, url);


                List<FantasyPlayer> temp = new List<FantasyPlayer>();
                for (int i = 0; i < players.Length; i++)
                {
                    temp.Add(players[i]);
                    if (i > 5 && i % 50 == 0)
                    {
                        localBaseballAcc.WritePlayers(temp.ToArray());
                        temp.Clear();
                    }
                }
                localBaseballAcc.WritePlayers(temp.ToArray());
            }

            return players;
        }

        public bool GetandWriteLeagueRoster(GPChromeDriver cachedDriver, FantasyLeagueEntry interestedLeague)
        {
            Console.WriteLine("Getting remote roster definition for " + interestedLeague.ForeignId);
            FantasyRosterDefinition result = fanDuelAcc.GetRoster(cachedDriver, interestedLeague.ForeignId, interestedLeague.Url);

            var isValid = fanDuelAcc.ValidateLeague(cachedDriver, interestedLeague.Url);
            if (!isValid)
                return false;

            if (result != null)
            {
                localBaseballAcc.WriteLeagueRoster(result);

                interestedLeague.StartingC = result.StartingC;
                interestedLeague.StartingP = result.StartingP;
                interestedLeague.StartingOF = result.StartingOF;
                interestedLeague.StartingSS = result.StartingSS;
                interestedLeague.Starting1B = result.Starting1B;
                interestedLeague.Starting2B = result.Starting2B;
                interestedLeague.Starting3B = result.Starting3B;
            }

            return true;
        }

        public DateTime GetMaxAvailableDataDate(SourceType source)
        {
            DateTime result = localBaseballAcc.GetMaxAvailableDataDate(source);

            return result;
        }

        public bool RegisterForLeague(GPChromeDriver cachedDriver, FantasyLeagueEntry interestedLeague, FantasyRoster roster
            , Action<long> onPlayerFail)
        {
            long? currPlayerId = null;
            try
            {
                var shouldContinue = fanDuelAcc.NavigateToLeague(cachedDriver, interestedLeague);
                if (!shouldContinue)
                    return false;

                for (int i = 0; i < roster.PlayersToSelect.Length; i++)
                {
                    currPlayerId = roster.PlayersToSelect[i].Id;
                    fanDuelAcc.AddPlayerToRoster(cachedDriver, interestedLeague.ForeignId, roster.PlayersToSelect[i]);
                }
                currPlayerId = null;
                //fanDuelAcc.RegisterForLeague(interestedLeague);

                fanDuelAcc.ConfirmEntry(cachedDriver, interestedLeague.ForeignId);

                return true;
            }
            catch (Exception e)
            {
                if (currPlayerId != null
                    && onPlayerFail != null)
                    onPlayerFail(currPlayerId.Value);

                return false;
            }
        }
    }
}
