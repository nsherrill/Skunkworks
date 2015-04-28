using GP.Accessors.RemoteAccessor;
using GP.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Accessors.RemoteBaseballDataAccessor
{
    public class RotoGrinders_RemoteBaseballDataAcc : IRemoteBaseballDataAcc
    {
        const string BASE_PAGE = "https://rotogrinders.com/offers/fanduel/mlb";
        const string HOSTURL = "https://rotogrinders.com";
        IRemoteAcc remoteAcc = new RemoteAcc();
        IRemoteBaseballDataParser parser = new RotoGrinders_RemoteBasebalDataParser();

        #region deprecated
        public Shared.Common.MassData deprecated_FindAllDataForDate(DateTime desiredDate)
        {
            throw new NotImplementedException();
        }

        public Shared.Common.GameEvent deprecated_FindGameDataFromContents(string gameId, string pageContents)
        {
            throw new NotImplementedException();
        }

        public Shared.Common.MassData deprecated_FindAllGamesDataFromContents(string pageContents)
        {
            throw new NotImplementedException();
        }

        public Shared.Common.FutureGameEvent[] deprecated_FindFutureGamesForDate(DateTime desiredDate)
        {
            throw new NotImplementedException();
        }

        public Shared.Common.FutureGameEvent deprecated_FindFutureGameDataFromContents(string gameId, string pageContents)
        {
            throw new NotImplementedException();
        }

        public Shared.Common.FutureGameEvent[] deprecated_FindAllFutureGamesDataFromContents(string pageContents)
        {
            throw new NotImplementedException();
        }
        #endregion

        public CurrentPlayerStats[] GetCurrentPlayerHittingStats(string sessionId)
        {
            string basePageContent = remoteAcc.GetPageContent(BASE_PAGE);
            string hitterUrl = RegexHelper.GetRegex(@"<a href=""(/pages/hot-streak-hitters-\d+)"">Daily Batter Hub</a>",
                basePageContent, 1);
            hitterUrl = HOSTURL + hitterUrl;
            string hitterPageContent = remoteAcc.GetPageContent(hitterUrl);
            hitterPageContent = hitterPageContent.Substring(hitterPageContent.IndexOf("tbl data-table no-wrap"));
            hitterPageContent = hitterPageContent.Substring(hitterPageContent.IndexOf("</thead>"));

            List<CurrentPlayerStats> results = new List<CurrentPlayerStats>();

            var hitterRows = RegexHelper.GetAllRegex(@"<tr>(.*?)</tr>", hitterPageContent, 1,
                System.Text.RegularExpressions.RegexOptions.Singleline);

            if (hitterRows != null)
            {
                foreach (var row in hitterRows)
                {
                    var hitterStat = parser.ParseCurrentPlayerHittingStats(row);
                    hitterStat.SessionId = sessionId;
                    results.Add(hitterStat);
                }
            }
            return results.ToArray();
        }

        public CurrentPlayerStats[] GetCurrentPlayerPitchingStats(string sessionId)
        {
            throw new NotImplementedException();
        }
    }
}
