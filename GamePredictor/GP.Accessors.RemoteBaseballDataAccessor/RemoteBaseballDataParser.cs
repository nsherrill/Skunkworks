using GP.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Accessors.RemoteBaseballDataAccessor
{
    public interface IRemoteBaseballDataParser
    {
        PlayerEventStats ParsePlayerEventStats(string gameId, string teamId, string[] data, string[] headers);
    }

    public class RemoteBaseballDataParser : IRemoteBaseballDataParser
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
    }
}
