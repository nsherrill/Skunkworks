using GP.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Accessors.RemoteBaseballDataAccessor
{
    public class SportingCharts_RemoteBaseballDataParser : IRemoteBaseballDataParser
    {
        public PlayerEventStats ParsePlayerEventStats(string gameIdStr, string teamIdStr, string[] data, string[] headers)
        {
            List<ValuePair> resultData = new List<ValuePair>();
            string name = string.Empty;
            string foreignPlayerIdStr = string.Empty;
            PlayerDataType dataType = PlayerDataType.Unknown;

            for (int i = 0; i < data.Length && i < headers.Length; i++)
            {
                if (headers[i].Equals("hitters", StringComparison.InvariantCultureIgnoreCase))
                {
                    //<a href="http://espn.go.com/mlb/player/_/id/30841/brock-holt">B Holt</a> LF
                    foreignPlayerIdStr = RegexHelper.GetRegex(@"/(\d+)/.+?""", data[i], 1);
                    name = RegexHelper.GetRegex(@"/\d+/(.+?)""", data[i], 1);
                    var position = RegexHelper.GetRegex("</a> (.+)", data[i], 1);

                    if (string.IsNullOrEmpty(foreignPlayerIdStr))
                    {
                        //<div style="padding-left: 10px;">P Baez P
                        foreignPlayerIdStr = RegexHelper.GetRegex(@";"">(.+?) [a-zA-Z0-9-]+?$", data[i], 1);
                        if (string.IsNullOrEmpty(foreignPlayerIdStr))
                            foreignPlayerIdStr = "-1";

                        name = RegexHelper.GetRegex(@";"">(.+?) [a-zA-Z0-9-]+?$", data[i], 1);
                        if (string.IsNullOrEmpty(name))
                            name = RegexHelper.GetRegex(@"^(.+?) [a-zA-Z0-9-]+?$", data[i], 1);
                        position = RegexHelper.GetRegex(" ([a-zA-Z0-9-]+?)$", data[i], 1);
                    }

                    dataType = PlayerDataType.Hitting;
                    resultData.Add(new ValuePair("Position", position));
                }
                else if (headers[i].Equals("pitchers", StringComparison.InvariantCultureIgnoreCase))
                {
                    //<a href="http://espn.go.com/mlb/player/_/id/30841/brock-holt">B Holt</a> LF
                    foreignPlayerIdStr = RegexHelper.GetRegex(@"/(\d+)/.+?""", data[i], 1);
                    name = RegexHelper.GetRegex(@"/\d+/(.+?)""", data[i], 1);
                    var position = "P";

                    if (string.IsNullOrEmpty(foreignPlayerIdStr))
                    {
                        //P Baez
                        foreignPlayerIdStr = RegexHelper.GetRegex(@";"">(.+?) [a-zA-Z0-9]+?$", data[i], 1);
                        if (string.IsNullOrEmpty(foreignPlayerIdStr))
                            foreignPlayerIdStr = RegexHelper.GetRegex(@"^(.+)$", data[i], 1);

                        name = RegexHelper.GetRegex(@";"">(.+?) [a-zA-Z0-9]+?$", data[i], 1);
                        if (string.IsNullOrEmpty(name))
                            name = RegexHelper.GetRegex(@"^(.+)$", data[i], 1);
                    }

                    dataType = PlayerDataType.Pitching;
                    resultData.Add(new ValuePair("Position", "P"));
                }
                else
                {
                    var newKey = headers[i].Replace('#', 'N');
                    newKey = newKey.Replace("&nbsp;", "");

                    if (newKey.Equals("pc-st", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var datas = data[i].Split('-');
                        if (datas != null && datas.Length > 1)
                        {
                            resultData.Add(new ValuePair("NP", datas[0]));
                            resultData.Add(new ValuePair("ST", datas[1]));
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        var newData = data[i].Replace("---", "0");
                        newData = newData.ToLower().Replace("inf", "0");
                        resultData.Add(new ValuePair(newKey, newData));
                    }
                }
            }

            // need to get foreign player id
            int foreignPlayerId = int.Parse(foreignPlayerIdStr);
            int gameId = int.Parse(gameIdStr);
            PlayerEventStats result = new PlayerEventStats()
            {
                ForeignGameEventId = gameId,
                ForeignPlayerId = foreignPlayerId,
                ForeignPlayerName = name,
                ForeignTeamId = teamIdStr,
                DataType = dataType,
                Data = resultData.ToArray(),
                Sport = SportType.Baseball,
            };
            return result;
        }

        public CurrentPlayerStats ParseCurrentPlayerStats(string content)
        {
            //<tr>
			//		<th style="text-align: ''"><span>Rank</span></th><th style="text-align: ''"><span>Player</span></th><th style="text-align: ''"><span>Pos</span></th><th style="text-align: ''"><span>Team</span></th><th style="text-align: ''"><span>AB</span></th><th style="text-align: ''"><span title="Singles">1B</span></th><th style="text-align: ''"><span title="Doubles">2B</span></th><th style="text-align: ''"><span title="Triples">3B</span></th><th style="text-align: ''"><span title="Home Runs">HR</span></th><th style="text-align: ''"><span title="Runs Batted In">RBI</span></th><th style="text-align: ''"><span title="Runs Scored">R</span></th><th style="text-align: ''"><span title="Walks">BB</span></th><th style="text-align: ''"><span title="Stolen Bases">SB</span></th><th style="text-align: ''"><span title="Hit By Pitch">HBP</span></th><th style="text-align: ''"><span title="Fan Duel Points">FD</span></th><th style="text-align: ''"><span title="Fan Duel Points Per Game">FD/G</span></th>
			//	</tr>
            //<tr>
			//		<td style="text-align: center">1</td><td style="text-align: left"><a href='/mlb/players/184104/adrian-gonzalez/'>Adrian Gonzalez</a></td><td style="text-align: center">1B</td><td style="text-align: center"><a href='/mlb/teams/243/los-angeles-dodgers/'>LAD</a></td><td style="text-align: center">44</td><td style="text-align: center">10</td><td style="text-align: center">8</td><td style="text-align: center">0</td><td style="text-align: center">5</td><td style="text-align: center">14</td><td style="text-align: center">13</td><td style="text-align: center">6</td><td style="text-align: center">0</td><td style="text-align: center">0</td><td style="text-align: center">84.25</td><td style="text-align: center">7.66</td>
			//	</tr>


            var matches = RegexHelper.GetAllRegex(@"<tr>
					<td style=""text-align: center"">(\d+)</td><td style=""text-align: left""><a(.*?)</a></td><td style=""text-align: center"">(.*?)</td><td style=""text-align: center""><a.*?>(.*?)</a></td><td style=""text-align: center"">(\d+)</td><td style=""text-align: center"">(\d+)</td><td style=""text-align: center"">(\d+)</td><td style=""text-align: center"">(\d+)</td><td style=""text-align: center"">(\d+)</td><td style=""text-align: center"">(\d+)</td><td style""=""text-align: center"">(\d+)</td><td style=""text-align: center"">(\d+)</td><td style=""text-align: center"">(\d+)</td><td style=""text-align: center"">(\d+)</td><td style=""text-align: center"">([\d,\.]+)</td><td style=""text-align: center"">([\d,\.]+)</td>
				</tr>", content);
            string rankStr = matches[0];
            string nameUrlStr = matches[1];
            string nameStr = matches[2];
            string positionStr = matches[3];
            string teamStr = matches[4];
            string abStr = matches[5];
            string singlesStr = matches[6];
            string doublesStr = matches[7];
            string triplesStr = matches[8];
            string hrsStr = matches[9];
            string rbisStr = matches[10];
            string rsStr = matches[11];
            string bbsStr = matches[12];
            string sbsStr = matches[13];
            string hbpsStr = matches[14];
            string totalPointsStr = matches[15];
            string ppgStr = matches[16];


            return null;
        }
    }
}
