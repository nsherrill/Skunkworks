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
            throw new NotImplementedException();
        }

        public CurrentPlayerStats ParseCurrentPlayerHittingStats(string content)
        {
            try
            {
                //<tr>
                //		<th style="text-align: ''"><span>Rank</span></th><th style="text-align: ''"><span>Player</span></th><th style="text-align: ''"><span>Pos</span></th><th style="text-align: ''"><span>Team</span></th><th style="text-align: ''"><span>AB</span></th><th style="text-align: ''"><span title="Singles">1B</span></th><th style="text-align: ''"><span title="Doubles">2B</span></th><th style="text-align: ''"><span title="Triples">3B</span></th><th style="text-align: ''"><span title="Home Runs">HR</span></th><th style="text-align: ''"><span title="Runs Batted In">RBI</span></th><th style="text-align: ''"><span title="Runs Scored">R</span></th><th style="text-align: ''"><span title="Walks">BB</span></th><th style="text-align: ''"><span title="Stolen Bases">SB</span></th><th style="text-align: ''"><span title="Hit By Pitch">HBP</span></th><th style="text-align: ''"><span title="Fan Duel Points">FD</span></th><th style="text-align: ''"><span title="Fan Duel Points Per Game">FD/G</span></th>
                //	</tr>
                //<tr>
                //		<td style="text-align: center">1</td><td style="text-align: left"><a href='/mlb/players/184104/adrian-gonzalez/'>Adrian Gonzalez</a></td><td style="text-align: center">1B</td><td style="text-align: center"><a href='/mlb/teams/243/los-angeles-dodgers/'>LAD</a></td><td style="text-align: center">44</td><td style="text-align: center">10</td><td style="text-align: center">8</td><td style="text-align: center">0</td><td style="text-align: center">5</td><td style="text-align: center">14</td><td style="text-align: center">13</td><td style="text-align: center">6</td><td style="text-align: center">0</td><td style="text-align: center">0</td><td style="text-align: center">84.25</td><td style="text-align: center">7.66</td>
                //	</tr>


                var matches = RegexHelper.GetAllRegex(
    @"<td.*?>(\d+)</td><td.*?><a href='(.*?)'>(.*?)</a></td><td.*?>(.*?)</td><td.*?><a href='(.*?)'>(.*?)</a></td><td.*?>(\d+)</td><td.*?>(\d+)</td><td.*?>(\d+)</td><td.*?>(\d+)</td><td.*?>(\d+)</td><td.*?>(\d+)</td><td.*?>(\d+)</td><td.*?>(\d+)</td><td.*?>(\d+)</td><td.*?>(\d+)</td><td.*?>([\d,\.]+)</td><td.*?>([\d,\.]+)</td>", content, -1);
                
                
                string rankStr = matches[1];
                string nameUrlStr = matches[2];
                string nameStr = matches[3];
                string positionStr = matches[4];
                positionStr = positionStr.Replace("CF", "OF").Replace("LF", "OF").Replace("RF", "OF");
                var position = (BaseballPosition)Enum.Parse(typeof(BaseballPosition), "pos_" + positionStr);
                string teamUrlStr = matches[5];
                string teamStr = matches[6];
                string abStr = matches[7];
                int ab = int.Parse(abStr);
                string singlesStr = matches[8];
                int singles = int.Parse(singlesStr);
                string doublesStr = matches[9];
                int doubles = int.Parse(doublesStr);
                string triplesStr = matches[10];
                int triples = int.Parse(triplesStr);
                string hrsStr = matches[11];
                int hr = int.Parse(hrsStr);
                string rbisStr = matches[12];
                int rbi = int.Parse(rbisStr);
                string rsStr = matches[13];
                int r = int.Parse(rsStr);
                string bbsStr = matches[14];
                int bb = int.Parse(bbsStr);
                string sbsStr = matches[15];
                int sb = int.Parse(sbsStr);
                string hbpsStr = matches[16];
                int hbp = int.Parse(hbpsStr);
                string totalPointsStr = matches[17];
                double points = double.Parse(totalPointsStr);
                string ppgStr = matches[18];
                double ppg = double.Parse(ppgStr);

                List<ValuePair> valPairs = new List<ValuePair>();
                valPairs.Add(new ValuePair("Position", position));
                valPairs.Add(new ValuePair("Team", teamStr));
                valPairs.Add(new ValuePair("AB", ab));
                valPairs.Add(new ValuePair("Singles", singles));
                valPairs.Add(new ValuePair("Doubles", doubles));
                valPairs.Add(new ValuePair("Triples", triples));
                valPairs.Add(new ValuePair("HR", hr));
                valPairs.Add(new ValuePair("RBI", rbi));
                valPairs.Add(new ValuePair("R", r));
                valPairs.Add(new ValuePair("BB", bb));
                valPairs.Add(new ValuePair("SB", sb));
                valPairs.Add(new ValuePair("HBP", hbp));
                valPairs.Add(new ValuePair("TotalPoints", points));
                valPairs.Add(new ValuePair("PPG", ppg));

                CurrentPlayerStats result = new CurrentPlayerStats()
                {
                    Data = valPairs.ToArray(),
                    DataType = PlayerDataType.Hitting,
                    ForeignPlayerName = nameStr,
                    Sport = SportType.Baseball,
                    ForeignTeamId = teamStr,
                };

                return result;
            }
            catch
            {
            }
            return null;
        }

        public CurrentPlayerStats ParseCurrentPlayerPitchingStats(string content)
        {
            try
            {
                //<th style="text-align: ''"><span>Rank</span></th>
                //<th style="text-align: ''"><span>Player</span></th>
                //<th style="text-align: ''"><span>Team</span></th>
                //<th style="text-align: ''"><span title="Games Started">GS</span></th>
                //<th style="text-align: ''"><span>Wins</span></th>
                //<th style="text-align: ''"><span title="Earned Runs Allowed">ER</span></th>
                //<th style="text-align: ''"><span title="Strike Outs Thrown">SO</span></th>
                //<th style="text-align: ''"><span title="Innings Pitched">IP</span></th>
                //<th style="text-align: ''"><span title="Fan Duel Points">FD</span></th>
                //<th style="text-align: ''"><span title="Fan Duel Points Per Start">FD/S</span></th>


                var matches = RegexHelper.GetAllRegex(
    @"<td.*?>(\d+)</td><td.*?><a href='(.*?)'>(.*?)</a></td><td.*?><a href='(.*?)'>(.*?)</a></td><td.*?>(\d+)</td><td.*?>(\d+)</td><td.*?>(\d+)</td><td.*?>(\d+)</td><td.*?>(\d+)</td><td.*?>([\d,\.]+)</td><td.*?>([\d,\.]+)</td>", content, -1);


                string rankStr = matches[1];
                string nameUrlStr = matches[2];
                string nameStr = matches[3];
                string teamUrlStr = matches[4];
                string teamStr = matches[5];
                string gsStr = matches[6];
                int gs = int.Parse(gsStr);
                string wStr = matches[7];
                int w = int.Parse(wStr);
                string eraStr = matches[8];
                int era = int.Parse(eraStr);
                string soStr= matches[9];
                int so = int.Parse(soStr);
                string ipStr= matches[10];
                int ip = int.Parse(ipStr);
                string totalPointsStr = matches[11];
                double points = double.Parse(totalPointsStr);
                string ppgStr = matches[12];
                double ppg = double.Parse(ppgStr);

                List<ValuePair> valPairs = new List<ValuePair>();
                valPairs.Add(new ValuePair("Team", teamStr));
                valPairs.Add(new ValuePair("GS", gs));
                valPairs.Add(new ValuePair("W", w));
                valPairs.Add(new ValuePair("ERA", era));
                valPairs.Add(new ValuePair("SO", so));
                valPairs.Add(new ValuePair("IP", ip));
                valPairs.Add(new ValuePair("TotalPoints", points));
                valPairs.Add(new ValuePair("PPG", ppg));

                CurrentPlayerStats result = new CurrentPlayerStats()
                {
                    Data = valPairs.ToArray(),
                    DataType = PlayerDataType.Pitching,
                    ForeignPlayerName = nameStr,
                    Sport = SportType.Baseball,
                    ForeignTeamId = teamStr,
                };

                return result;
            }
            catch
            {
            }
            return null;
        }
    }
}
