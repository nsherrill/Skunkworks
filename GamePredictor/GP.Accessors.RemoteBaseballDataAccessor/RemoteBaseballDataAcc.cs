using GP.Accessors.RemoteAccessor;
using GP.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Accessors.RemoteBaseballDataAccessor
{
    public interface IRemoteBaseballDataAcc
    {
        MassData FindAllDataForDate(DateTime desiredDate);

        GameEvent FindGameDataFromContents(string gameId, string pageContents);

        MassData FindAllGamesDataFromContents(string pageContents);

        FutureGameEvent[] FindFutureGamesForDate(DateTime desiredDate);

        FutureGameEvent FindFutureGameDataFromContents(string gameId, string pageContents);

        FutureGameEvent[] FindAllFutureGamesDataFromContents(string pageContents);
    }

    public class RemoteBaseballDataAcc : IRemoteBaseballDataAcc
    {
        IRemoteAcc remoteAcc = new RemoteAcc();
        IRemoteBaseballDataParser dataParser = new RemoteBaseballDataParser();
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

        public MassData FindAllDataForDate(DateTime desiredDate)
        {
            var dateString = desiredDate.ToString("yyyyMMdd");
            string gameListUrl = string.Format(FORMATTER_DAYSGAMESURL, dateString);

            string gamesPageContent = remoteAcc.GetPageContent(gameListUrl);

            return FindAllGamesDataFromContents(gamesPageContent);
        }

        public MassData FindAllGamesDataFromContents(string pageContents)
        {
            List<GameEvent> allGames = new List<GameEvent>();
            //string[] gameIds = RegexHelper.GetAllRegex(@"""(\d*)-gamebox""", pageContents, 1);
            int startIndex = pageContents.IndexOf("boxscore");
            if (startIndex > 0)
            {
                string shortenedContents = pageContents.Substring(startIndex);
                string[] gameIds = RegexHelper.GetAllRegex(@"http://espn\.go\.com/mlb/boxscore\?gameId=(\d*)""", shortenedContents, 1);


                foreach (var gameId in gameIds)
                {
                    string gameUrl = string.Format(FORMATTER_INDIVIDUALGAMEURL, gameId);
                    string gamePageContent = remoteAcc.GetPageContent(gameUrl);
                    GameEvent newGame = FindGameDataFromContents(gameId, gamePageContent);
                    if (newGame != null)
                        allGames.Add(newGame);
                    else
                        break;
                }
            }

            MassData result = new MassData()
            {
                Games = allGames.ToArray(),
            };
            return result;
        }

        public GameEvent FindGameDataFromContents(string gameId, string gamePageContent)
        {
            try
            {
                //var stubbedContent = gamePageContent.Substring(gamePageContent.IndexOf(@"id=""gameStatusBarWrap"""));
                string team1 = RegexHelper.GetRegex(@"<title>(.+?) vs\. (.+?) - Box Score - ", gamePageContent, 1);
                string team2 = RegexHelper.GetRegex(@"<title>(.+?) vs\. (.+?) - Box Score - ", gamePageContent, 2);

                string substringContent = gamePageContent.Substring(gamePageContent.IndexOf("<span class=\"title\">Conversation</span>"));
                string[] teamHittingTables = RegexHelper.GetAllRegex(@"(.*? *\d+ +\d+ +\d+ +\d+)", substringContent, 1);
                //string[] homeTeamHittingTables = RegexHelper.GetAllRegex(@"\d+ +\d+ +\d+ +\d+(.*? *?\d+ +\d+ +\d+ +\d+) *$", substringContent, 1);

                string[][] cleanTables = CleanTables(teamHittingTables);
                // string[][] cleanHomeTables = CleanTables(homeTeamHittingTables);

                List<PlayerEventStats> allPlayerStats = new List<PlayerEventStats>();
                string[] headers = { "hitters", "AB", "R", "H", "BI" };

                for (int i = 0; i < cleanTables.Length; i++)
                {
                    var teamToUse = i % 2 == 0 ? team1 : team2;
                    PlayerEventStats playerData = dataParser.ParsePlayerEventStats(gameId, teamToUse, cleanTables[i], headers);
                    allPlayerStats.Add(playerData);
                }



                /*
                string[] dataTables = RegexHelper.GetAllRegex(@"<table border=""0"" width=""100%"" class=""mod-data mlb-box"">(.+?)</table>", gamePageContent);
                List<PlayerEventStats> allPlayerStats = new List<PlayerEventStats>();

                for (int dt = 0; dt < dataTables.Length; dt++)
                {
                    var teamToUse = dt < 2 ? team1 : team2;
                    var rows = RegexHelper.GetAllRegex(@"<tr .+?>(.+?)</tr>", dataTables[dt], 1);
                    List<List<string>> rowData = new List<List<string>>();
                    for (int i = 0; i < rows.Length; i++)
                    {
                        var contents = RegexHelper.GetAllRegex("<t[hd].*?>(.+?)</t[hd]>", rows[i], 1);
                        if (contents.Length > 3)
                        {
                            if (rowData.Count == 0)
                            {// this is header

                            }
                            rowData.Add(contents.ToList());
                        }
                    }

                    for (int i = 1; i < rowData.Count - 1; i++) // ignore top and bottom row
                    {
                        PlayerEventStats playerData = dataParser.ParsePlayerEventStats(gameId, teamToUse, rowData[i], rowData[0]);
                        allPlayerStats.Add(playerData);
                    }
                }
                */

                List<Team> teams = new List<Team>();
                teams.Add(new Team()
                {
                    ForeignId = team1,
                    Name = team1,
                    Sport = SportType.Baseball,
                });
                teams.Add(new Team()
                {
                    ForeignId = team2,
                    Name = team2,
                    Sport = SportType.Baseball,
                });

                //<div class="game-time-location"><p>7:05 PM ET, June 10, 2014</p><p>Oriole Park at Camden Yards, Baltimore, Maryland&nbsp;</p></div>
                string gameTimeStr = RegexHelper.GetRegex(@"<div class=""game-time-location""><p>(.+?) ET, (.+?)</p>", gamePageContent, 1);
                string gameDateStr = RegexHelper.GetRegex(@"<div class=""game-time-location""><p>(.+?) ET, (.+?)</p>", gamePageContent, 2);
                DateTime gameDate;
                if (!DateTime.TryParse(gameTimeStr + ',' + gameDateStr, out gameDate))
                {
                    gameDateStr = RegexHelper.GetRegex(@"<div class=""game-time-location""><p>(.+?)</p>", gamePageContent, 1);
                    gameDate = DateTime.Parse(gameDateStr);
                }

                string stadium = RegexHelper.GetRegex(@"<div class=""game-time-location""><p>.+?</p><p>(.+?)</p></div>", gamePageContent.Replace("&nbsp;", ""), 1);

                //<div class="mod-header"><h4>Game Notes</h4></div><div class="mod-content"><table class="mod-data"><tbody><tr class="even"><td style="text-align:left;">THE GAME WAS DELAYED 1:33 DUE TO RAIN IN THE BOTTOM OF THE SECOND INNING.</td></tr></tbody></table></div>
                string gameNotes = RegexHelper.GetRegex(@"<div class=""mod-header""><h4>Game Notes</h4></div><div class=""mod-content""><table class=""mod-data""><tbody><tr class=""even""><td style=""text-align:left;"">(.+?)</td></tr></tbody></table></div>", gamePageContent, 1);

                int attendence = 0;
                double? weatherDegs = null;
                WeatherType weatherType = WeatherType.Unknown;
                double windSpeed = 0;
                List<Referee> referees = new List<Referee>();
                if (allPlayerStats.Count == 0
                    || (!string.IsNullOrWhiteSpace(gameNotes)
                        && gameNotes.ToLower().Contains("postponed")))
                {

                }
                else
                {
                    //<strong>Attendance</strong></td><td style="text-align:left; ">24,184 
                    string attendenceStr = RegexHelper.GetRegex(@"<strong>Attendance</strong></td><td style=""text-align:left; "">(.+?) ", gamePageContent, 1);
                    if (!string.IsNullOrWhiteSpace(attendenceStr))
                    {
                        if (attendenceStr.Contains("<"))
                            attendenceStr = attendenceStr.Substring(0, attendenceStr.IndexOf("<"));
                        attendence = int.Parse(attendenceStr.Replace(",", ""));
                    }

                    //<strong>Weather</strong></td><td style="text-align:left; ">82 degrees, cloudy</td>
                    string weatherStr = RegexHelper.GetRegex(@"<strong>Weather</strong></td><td style=""text-align:left; "">(.+?) degrees, (.+?)</td>", gamePageContent, 1);
                    if (string.IsNullOrWhiteSpace(weatherStr))
                    {
                        weatherStr = RegexHelper.GetRegex(@"<strong>Weather</strong></td><td style=""text-align:left; "">(.+?)</td>", gamePageContent, 1);
                        weatherType = (WeatherType)Enum.Parse(typeof(WeatherType), weatherStr.Replace(" ", ""), true);
                    }
                    else
                    {
                        weatherDegs = double.Parse(weatherStr);
                        weatherStr = RegexHelper.GetRegex(@"<strong>Weather</strong></td><td style=""text-align:left; "">(.+?) degrees, (.+?)</td>", gamePageContent, 2);
                        if (weatherStr.Equals("roof closed", StringComparison.InvariantCultureIgnoreCase))
                            weatherStr = WeatherType.Indoors.ToString();
                        weatherType = (WeatherType)Enum.Parse(typeof(WeatherType), weatherStr.Replace(" ", ""), true);
                    }

                    //<strong>Wind</strong></td><td style="text-align:left; ">1 mph</td>
                    string windSpeedStr = RegexHelper.GetRegex(@"<strong>Wind</strong></td><td style=""text-align:left; "">(\d*) mph</td>", gamePageContent, 1);
                    if (!string.IsNullOrWhiteSpace(windSpeedStr))
                        windSpeed = double.Parse(windSpeedStr);

                    //<strong>Umpires</strong></td><td style="text-align:left; ">Home Plate - Brian Gorman, First Base - Tony Randazzo, Second Base - David Rackley, Third Base - Will  Little</td>
                    string refStr = RegexHelper.GetRegex(@"<strong>Umpires</strong></td><td style=""text-align:left; "">(.+?)</td>", gamePageContent, 1);
                    if (!string.IsNullOrWhiteSpace(refStr))
                    {
                        string[] refsSplits = refStr.Split(',');
                        foreach (var referee in refsSplits)
                        {
                            var refSplits = referee.Split('-');
                            string typeStr = refSplits[0].Replace(" ", "");
                            if (typeStr.Equals("home", StringComparison.InvariantCultureIgnoreCase))
                                typeStr = RefereeType.HomePlate.ToString();
                            RefereeType type = (RefereeType)Enum.Parse(typeof(RefereeType), typeStr);
                            string refName = refSplits[1].Trim().Replace("  ", " ");
                            referees.Add(new Referee()
                            {
                                ForeignId = refName,
                                Name = refName,
                                Type = type,
                                SportType = SportType.Baseball,
                            });
                        }
                    }
                }

                GameEvent result = new GameEvent()
                {
                    Date = gameDate,
                    ForeignId = gameId,
                    Sport = SportType.Baseball,
                    Teams = teams.ToArray(),
                    PlayerStats = allPlayerStats.ToArray(),
                    Stadium = stadium,
                    Attendence = attendence,
                    Weather_Degrees = weatherDegs,
                    Weather_Type = weatherType,
                    WindSpeed = windSpeed,
                    Referees = referees.ToArray(),
                    GameNotes = gameNotes,
                };
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private string[][] CleanTables(string[] source)
        {
            List<string[]> result = new List<string[]>();
            for (int i = 0; i < source.Length - 1; i++) // ignore bottom row
            {
                string name = "";
                bool notName = false;
                List<string> data = new List<string>();
                foreach (var col in source[i].Split(' '))
                    if (!string.IsNullOrWhiteSpace(col))
                    {
                        long templong;
                        if (!long.TryParse(col, out templong)
                            && !notName)
                        {
                            if (name.Length > 0)
                                name += ' ';
                            name += col;
                        }
                        else
                        {
                            if (!notName)
                                data.Add(name);
                            notName = true;
                            data.Add(col);
                        }
                    }
                if (name.Equals("totals", StringComparison.InvariantCultureIgnoreCase))
                    break;
                result.Add(data.ToArray());
            }

            return result.ToArray();
        }

        public FutureGameEvent[] FindFutureGamesForDate(DateTime desiredDate)
        {
            var dateString = desiredDate.ToString("yyyyMMdd");
            string gameListUrl = string.Format(FORMATTER_DAYSGAMESURL, dateString);

            string gamesPageContent = remoteAcc.GetPageContent(gameListUrl);

            return FindAllFutureGamesDataFromContents(gamesPageContent);
        }

        public FutureGameEvent[] FindAllFutureGamesDataFromContents(string pageContents)
        {
            //<a href="/mlb/conversation?gameId=340618106&amp;teams=kansas-city-royals-vs-detroit-tigers">Conversation</a>
            string[] gameIds = RegexHelper.GetAllRegex(@"""(\d*)-gamebox""", pageContents, 1);
            List<FutureGameEvent> allGames = new List<FutureGameEvent>();

            for (int i = 0; i < gameIds.Length; i++)
            //foreach (var gameId in gameIds)
            {
                var gameId = gameIds[i];

                string gameUrl = string.Format(FORMATTER_INDIVIDUALFUTUREGAMEURL, gameId);
                string gamePageContent = remoteAcc.GetPageContent(gameUrl);
                FutureGameEvent newGame = FindFutureGameDataFromContents(gameId, gamePageContent);
                if (newGame != null)
                    allGames.Add(newGame);

                if (i > 3
                    && allGames.Count == 0)
                    break;
            }
            return allGames.ToArray();
        }

        public FutureGameEvent FindFutureGameDataFromContents(string gameIdStr, string pageContents)
        {
            try
            {
                string team1Str = RegexHelper.GetRegex(@"<title>(.+?) vs\. (.+?) - Preview - ", pageContents, 1);
                string team2Str = RegexHelper.GetRegex(@"<title>(.+?) vs\. (.+?) - Preview - ", pageContents, 2);

                List<Team> teams = new List<Team>();
                teams.Add(new Team()
                {
                    ForeignId = team1Str,
                    Name = team1Str,
                    Sport = SportType.Baseball,
                });
                teams.Add(new Team()
                {
                    ForeignId = team2Str,
                    Name = team2Str,
                    Sport = SportType.Baseball,
                });

                double tempDouble;
                //<span class="weatherText"><span title="High">87°F</span> / <span title="Low">65°F</span><br>Precip: 50%</span>
                string highTempStr = RegexHelper.GetRegex(@"<span title=""High"">(\d+)", pageContents, 1);
                double? highTemp = null;
                if (double.TryParse(highTempStr, out tempDouble))
                    highTemp = tempDouble;

                string lowTempStr = RegexHelper.GetRegex(@"<span title=""Low"">(\d+)", pageContents, 1);
                double? lowTemp = null;
                if (double.TryParse(lowTempStr, out tempDouble))
                    lowTemp = tempDouble;

                string precipStr = RegexHelper.GetRegex(@"Precip.*?(\d+)", pageContents, 1);
                double? precip = null;
                if (double.TryParse(precipStr, out tempDouble))
                    precip = tempDouble;

                //<div class="game-time-location"><p>1:08 PM ET, June 18, 2014</p><p>Comerica Park, Detroit, Michigan&nbsp;</p></div>

                //<div class="game-time-location"><p>7:05 PM ET, June 10, 2014</p><p>Oriole Park at Camden Yards, Baltimore, Maryland&nbsp;</p></div>
                string gameTimeStr = RegexHelper.GetRegex(@"<div class=""game-time-location""><p>(.+?) ET, (.+?)</p>", pageContents, 1);
                string gameDateStr = RegexHelper.GetRegex(@"<div class=""game-time-location""><p>(.+?) ET, (.+?)</p>", pageContents, 2);
                DateTime gameDate;
                if (!DateTime.TryParse(gameTimeStr + ',' + gameDateStr, out gameDate))
                {
                    gameDateStr = RegexHelper.GetRegex(@"<div class=""game-time-location""><p>(.+?)</p>", pageContents, 1);
                    gameDate = DateTime.Parse(gameDateStr);
                }

                string stadium = RegexHelper.GetRegex(@"<div class=""game-time-location""><p>.+?</p><p>(.+?)</p></div>", pageContents.Replace("&nbsp;", ""), 1);


                //<p class="heading"><span>Starting Pitchers<span></span></span></p>
                //    <p><span>Kansas City:</span> <a href="http://espn.go.com/mlb/player/_/id/5370/jeremy-guthrie">Guthrie</a> (3-6, 4.04 ERA) </p>
                //    <p><span>Detroit:</span> <a href="http://espn.go.com/mlb/player/_/id/31816/drew-smyly">Smyly</a> (3-5, 3.58 ERA) </p>
                //</div>


                //Starting Pitchers[.\r\n]+/mlb/player/_/id/\d+/(.+?)""
                string foreignPitcherId1Str = RegexHelper.GetRegex(@"Starting Pitchers(.|\n)+?/mlb/player/_/id/(\d+)/.+?""", pageContents, 2);
                string foreignPitcherName1Str = RegexHelper.GetRegex(@"Starting Pitchers(.|\n)+?/mlb/player/_/id/\d+/(.+?)""", pageContents, 2);
                string foreignPitcherId2Str = RegexHelper.GetRegex(@"Starting Pitchers(.|\n)+?/mlb/player/_/id/(.|\n)+?/mlb/player/_/id/(\d+)/.+?""", pageContents, 3);
                string foreignPitcherName2Str = RegexHelper.GetRegex(@"Starting Pitchers(.|\n)+?/mlb/player/_/id/(.|\n)+?/mlb/player/_/id/\d+/(.+?)""", pageContents, 3);

                int foreignPitcherId1 = int.Parse(foreignPitcherId1Str);
                int foreignPitcherId2 = int.Parse(foreignPitcherId2Str);

                int gameId = int.Parse(gameIdStr);

                List<PlayerEventStats> startingPitchers = new List<PlayerEventStats>();
                startingPitchers.Add(new PlayerEventStats()
                {
                    ForeignPlayerName = foreignPitcherName1Str,
                    ForeignPlayerId = foreignPitcherId1,
                    DataType = PlayerDataType.StartingPitcher,
                    ForeignGameEventId = gameId,
                    ForeignTeamId = team1Str,
                    Sport = SportType.Baseball,
                });
                startingPitchers.Add(new PlayerEventStats()
                {
                    ForeignPlayerName = foreignPitcherName2Str,
                    ForeignPlayerId = foreignPitcherId2,
                    DataType = PlayerDataType.StartingPitcher,
                    ForeignGameEventId = gameId,
                    ForeignTeamId = team2Str,
                    Sport = SportType.Baseball,
                });

                return new FutureGameEvent()
                {
                    Date = gameDate,
                    ForeignId = gameIdStr,
                    PrecipitationChance = precip,
                    Sport = SportType.Baseball,
                    Stadium = stadium,
                    Weather_HighDegrees = highTemp,
                    Weather_LowDegrees = lowTemp,
                    StartingPitchers = startingPitchers.ToArray(),
                    Teams = teams.ToArray(),
                    //Referees = referees.ToArray(),
                };
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
