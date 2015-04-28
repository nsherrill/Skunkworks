using GP.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Accessors.RemoteBaseballDataAccessor
{
    class RotoGrinders_RemoteBasebalDataParser : IRemoteBaseballDataParser
    {
        #region not implemented
        public Shared.Common.PlayerEventStats ParsePlayerEventStats(string gameId, string teamId, string[] data, string[] headers)
        {
            throw new NotImplementedException();
        }
        #endregion

        public CurrentPlayerStats ParseCurrentPlayerHittingStats(string content)
        {
            /*
             <tr>
			<td style="border:1px solid #cccccc; text-align:center;">	Mark Teixeira	</td>
			<td style="border:1px solid #cccccc; text-align:center;">	<span class="caps">NYY</span>	</td>
			<td style="border:1px solid #cccccc; text-align:center;">	1B	</td>
			<td style="border:1px solid #cccccc; text-align:center;">	Karns	</td>
			<td style="border:1px solid #cccccc; text-align:center;">	24	</td>
			<td style="border:1px solid #cccccc; text-align:center;">	5	</td>
			<td style="border:1px solid #cccccc; text-align:center;">	0.333	</td>
			<td style="border:1px solid #cccccc; text-align:center;">	0.414	</td>
			<td style="border:1px solid #cccccc; text-align:center;" class="green">	0.565	</td>
			<td style="border:1px solid #cccccc; text-align:center;">	1.414	</td>
			<td style="border:1px solid #cccccc; text-align:center;">	1.740	</td>
			<td style="border:1px solid #cccccc; text-align:center;">	43	</td>
			<td style="border:1px solid #cccccc; text-align:center;">	0.279	</td>
			<td style="border:1px solid #cccccc; text-align:center;">	0.453	</td>
			<td style="border:1px solid #cccccc; text-align:center;">	1.126	</td>
			<td style="border:1px solid #cccccc; text-align:center;" class="green">	0.188	</td>
			<td style="border:1px solid #cccccc; text-align:center;">	0.226	</td>
		</tr>
             */

            var allCols = RegexHelper.GetAllRegex(@">(.+?)</td>", content, 1);
            List<string> newList = new List<string>();
            foreach (var p in allCols)
                newList.Add(p.Trim().Replace("\t", "").Replace("\\t", ""));
            allCols = newList.ToArray();

            string name = allCols[0];
            string team = RegexHelper.GetRegex(@"caps"">(.*?)</span", allCols[1], 1);
            var position = (BaseballPosition)Enum.Parse(typeof(BaseballPosition), "pos_" + allCols[2]);
            string oppPitcher = allCols[3];
            int last7AB = int.Parse(allCols[4]);
            int last7HR = int.Parse(allCols[5]);
            double last7AVG = double.Parse(allCols[6]);
            double last7OBP = double.Parse(allCols[7]);
            double last7WOBA = double.Parse(allCols[8]);
            double last7OPS = double.Parse(allCols[9]);
            double fdPerAB = double.Parse(allCols[10]);

            List<ValuePair> data = new List<ValuePair>();
            data.Add(new ValuePair("Name", name));
            data.Add(new ValuePair("Team", team));
            data.Add(new ValuePair("Position", position.ToString()));
            data.Add(new ValuePair("NextOpposingPitcher", oppPitcher));
            data.Add(new ValuePair("ABLast7Days", last7AB));
            data.Add(new ValuePair("HRLast7Days", last7HR));
            data.Add(new ValuePair("AVGLast7Days", last7AVG));
            data.Add(new ValuePair("OBPLast7Days", last7OBP));
            data.Add(new ValuePair("WOBALast7Days", last7WOBA));
            data.Add(new ValuePair("OPSLast7Days", last7OPS));
            data.Add(new ValuePair("PointsPerABLast7Days", fdPerAB));


            CurrentPlayerStats result = new CurrentPlayerStats()
            {
                Data = data.ToArray(),
                ForeignPlayerName = name,
                Sport = SportType.Baseball,
                DataType = PlayerDataType.HotHitting,
                ForeignTeamId = team,
            };

            return result;
        }

        public Shared.Common.CurrentPlayerStats ParseCurrentPlayerPitchingStats(string content)
        {
            throw new NotImplementedException();
        }
    }
}
