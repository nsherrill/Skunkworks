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
        MassData EnsureLocalDataForDate(DateTime dateTime);

        FutureGameEvent[] GetFutureGamesData(DateTime dateTime);

        void PullAvailableLeagues(GPChromeDriver chromeDriver);

        FantasyPlayer[] GetPlayersForLeague(GPChromeDriver cachedDriver, string leagueForeignId, string url);

        void GetandWriteLeagueRoster(GPChromeDriver cachedDriver, FantasyLeagueEntry interestedLeague);

        DateTime GetMaxAvailableDataDate();

        bool RegisterForLeague(GPChromeDriver cachedDriver, FantasyLeagueEntry interestedLeague, FantasyRoster roster);
    }

    public class BaseballDataRetrieverEng : IBaseballDataRetrieverEng
    {
        ILocalBaseballDataAcc localBaseballAcc = new LocalBaseballDataAcc();
        IRemoteBaseballDataAcc remoteBaseballAcc = new RemoteBaseballDataAcc();
        IRemoteFanDuelAcc fanDuelAcc = new RemoteFanDuelAcc();

        public MassData EnsureLocalDataForDate(DateTime desiredDate)
        {
            MassData result = localBaseballAcc.FindDataForDate(desiredDate);
            if (result == null
                || result.Games == null)
            {
                Console.WriteLine("Getting remote baseball data for " + desiredDate.ToShortDateString());
                result = remoteBaseballAcc.FindAllDataForDate(desiredDate);
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

        public FutureGameEvent[] GetFutureGamesData(DateTime desiredDate)
        {
            FutureGameEvent[] results = localBaseballAcc.FindFutureGamesForDate(desiredDate);

            if (results == null
                || results.Length == 0)
            {
                Console.WriteLine("Getting remote future baseball data for " + desiredDate.ToShortDateString());
                results = remoteBaseballAcc.FindFutureGamesForDate(desiredDate);

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
                        localBaseballAcc.WriteFantasyLeagues(temp.ToArray());
                        temp.Clear();
                    }
                }
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

        public void GetandWriteLeagueRoster(GPChromeDriver cachedDriver, FantasyLeagueEntry interestedLeague)
        {
            Console.WriteLine("Getting remote roster definition for " + interestedLeague.ForeignId);
            FantasyRosterDefinition result = fanDuelAcc.GetRoster(cachedDriver, interestedLeague.ForeignId, interestedLeague.Url);

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
        }

        public DateTime GetMaxAvailableDataDate()
        {
            DateTime result = localBaseballAcc.GetMaxAvailableDataDate();

            return result;
        }

        public bool RegisterForLeague(GPChromeDriver cachedDriver, FantasyLeagueEntry interestedLeague, FantasyRoster roster)
        {
            try
            {
                fanDuelAcc.NavigateToLeague(cachedDriver, interestedLeague);

                for (int i = 0; i < roster.PlayersToSelect.Length; i++)
                {
                    fanDuelAcc.AddPlayerToRoster(cachedDriver, interestedLeague.ForeignId, roster.PlayersToSelect[i]);
                }
                //fanDuelAcc.RegisterForLeague(interestedLeague);

                fanDuelAcc.ConfirmEntry(cachedDriver, interestedLeague.ForeignId);
                
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
    }
}
