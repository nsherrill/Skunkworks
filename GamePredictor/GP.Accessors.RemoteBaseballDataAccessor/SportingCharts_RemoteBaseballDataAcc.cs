﻿using GP.Accessors.RemoteAccessor;
using GP.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Accessors.RemoteBaseballDataAccessor
{
    public class SportingCharts_RemoteBaseballDataAcc : IRemoteBaseballDataAcc
    {
        IRemoteAcc remoteAcc = new RemoteAcc();
        IRemoteBaseballDataParser dataParser = new SportingCharts_RemoteBaseballDataParser();

        string SPORTINGCHARTS_HITTINGSTATSURL_FORMATTER { get { return "http://www.sportingcharts.com/mlb/stats/mlb-fanduel-hitter-statistics/{0}/"; } }

        #region deprecated
        private string FORMATTER_DAYSGAMESURL
        {
            get
            {
                //http://scores.espn.go.com/mlb/scoreboard?date=20140610
                return "http://scores.espn.go.com/mlb/scoreboard?date={0}";
            }
        }
        private string FORMATTER_INDIVIDUALGAMEURL
        {
            get
            {
                //http://scores.espn.go.com/mlb/boxscore?gameId=340610101
                return "http://scores.espn.go.com/mlb/boxscore?gameId={0}";
            }
        }
        private string FORMATTER_INDIVIDUALFUTUREGAMEURL
        {
            get
            {
                //http://scores.espn.go.com/mlb/preview?gameId=340618106
                return "http://scores.espn.go.com/mlb/preview?gameId={0}";
            }
        }

        public MassData deprecated_FindAllDataForDate(DateTime desiredDate)
        {
            throw new NotImplementedException();
        }

        public GameEvent deprecated_FindGameDataFromContents(string gameId, string pageContents)
        {
            throw new NotImplementedException();
        }

        public MassData deprecated_FindAllGamesDataFromContents(string pageContents)
        {
            throw new NotImplementedException();
        }

        public FutureGameEvent[] deprecated_FindFutureGamesForDate(DateTime desiredDate)
        {
            throw new NotImplementedException();
        }

        public FutureGameEvent deprecated_FindFutureGameDataFromContents(string gameId, string pageContents)
        {
            throw new NotImplementedException();
        }

        public FutureGameEvent[] deprecated_FindAllFutureGamesDataFromContents(string pageContents)
        {
            throw new NotImplementedException();
        }
        #endregion

        public CurrentPlayerStats[] GetCurrentPlayerHittingStats()
        {
            var pageUrl = string.Format(SPORTINGCHARTS_HITTINGSTATSURL_FORMATTER, DateTime.Now.Year);
            var pageContents = remoteAcc.GetPageContent(pageUrl);

            List<CurrentPlayerStats> result = new List<CurrentPlayerStats>();
            pageContents = pageContents.Substring(pageContents.IndexOf("MLB Fan Duel Hitter Statistics"));
            pageContents = pageContents.Substring(0, pageContents.IndexOf(@"[0].Grid = {""ColumnInfo"":"));

            var matches = RegexHelper.GetAllRegex(@"<tr>
					<td style=""text-align: center"">\d+</td><td style=""text-align: left""><a.*?</a></td><td style=""text-align: center"">.*?</td><td style=""text-align: center""><a.*?>.*?</a></td><td style=""text-align: center"">\d+</td><td style=""text-align: center"">\d+</td><td style=""text-align: center"">\d+</td><td style=""text-align: center"">\d+</td><td style=""text-align: center"">\d+</td><td style=""text-align: center"">\d+</td><td style""=""text-align: center"">\d+</td><td style=""text-align: center"">\d+</td><td style=""text-align: center"">\d+</td><td style=""text-align: center"">\d+</td><td style=""text-align: center"">[\d,\.]+</td><td style=""text-align: center"">[\d,\.]+</td>
				</tr>", pageContents);

            foreach (var match in matches)
            {
                CurrentPlayerStats stat = dataParser.ParseCurrentPlayerStats(match);
                if (stat != null)
                    result.Add(stat);
            }

            return result.ToArray();
        }

        public CurrentPlayerStats[] GetCurrentPlayerPitchingStats()
        {
            throw new NotImplementedException();
        }
    }
}
