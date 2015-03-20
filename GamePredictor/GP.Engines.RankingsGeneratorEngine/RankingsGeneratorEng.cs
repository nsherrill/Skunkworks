using GP.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Engines.RankingsGeneratorEngine
{
    public interface IRankingsGeneratorEng
    {
        FantasyRoster GenerateRoster(FantasyLeagueEntry interestedLeague, FantasyPlayerRanking[] playerOptions, ConfigType configType);
    }

    public class RankingsGeneratorEng : IRankingsGeneratorEng
    {
        public FantasyRoster GenerateRoster(FantasyLeagueEntry interestedLeague, FantasyPlayerRanking[] playerOptions, ConfigType configType)
        {
            List<FantasyPlayerRanking> players;// = new List<FantasyPlayerRanking>();
            switch (configType)
            {
                case ConfigType.Conservative:
                    players = GenerateRoster_Conservative(interestedLeague, playerOptions);
                    break;
                case ConfigType.Aggressive:
                    players = GenerateRoster_Aggressive(interestedLeague, playerOptions);
                    break;
                default:
                    throw new NotImplementedException();
            }

            FantasyRoster result = new FantasyRoster()
            {
                ForeignLeagueId = long.Parse(interestedLeague.ForeignId),
                PlayersToSelect = players.ToArray()
            };
            return result;
        }

        private List<FantasyPlayerRanking> GenerateRoster_Conservative(FantasyLeagueEntry interestedLeague, FantasyPlayerRanking[] playerOptions)
        {
            List<FantasyPlayerRanking> results = new List<FantasyPlayerRanking>();

            if (interestedLeague.StartingP > 0)
            {
                var allPitchers = playerOptions.Where(p => p.Position == BaseballPosition.pos_P).Where(p => p.Hits > 100);
                if (allPitchers.Count() == 0)
                    allPitchers = playerOptions.Where(p => p.Position == BaseballPosition.pos_P);
                var maxPitchCalc = allPitchers.Select(p => p.AVG / p.ERA).Max();
                var selectedPitcher = allPitchers.Where(p => p.AVG / p.ERA == maxPitchCalc).FirstOrDefault();
                results.Add(selectedPitcher);
                interestedLeague.StartingP--;
            }

            List<BaseballPosition> remainingSpots = interestedLeague.RemainingRosterSpots();
            while (results.Count < remainingSpots.Count)
            {
                var bestRemaining = playerOptions.Where(p => remainingSpots.Contains(p.Position)).Where(p => p.Hits > 100);
                if (bestRemaining.Count() == 0)
                    bestRemaining = playerOptions.Where(p => remainingSpots.Contains(p.Position));
                var maxCalc = bestRemaining.Select(p => p.AVG / p.ERA).Max();
                var selected = bestRemaining.Where(p => remainingSpots.Contains(p.Position) && p.AVG / p.ERA == maxCalc).FirstOrDefault();
                results.Add(selected);
                switch (selected.Position)
                {
                    case BaseballPosition.pos_1B:
                        interestedLeague.Starting1B--;
                        break;
                    case BaseballPosition.pos_2B:
                        interestedLeague.Starting2B--;
                        break;
                    case BaseballPosition.pos_3B:
                        interestedLeague.Starting3B--;
                        break;
                    case BaseballPosition.pos_SS:
                        interestedLeague.StartingSS--;
                        break;
                    case BaseballPosition.pos_OF:
                        interestedLeague.StartingOF--;
                        break;
                    case BaseballPosition.pos_C:
                        interestedLeague.StartingC--;
                        break;
                    case BaseballPosition.pos_P:
                        interestedLeague.StartingP--;
                        break;
                }

                remainingSpots = interestedLeague.RemainingRosterSpots();
            }

            return results;
        }

        private List<FantasyPlayerRanking> GenerateRoster_Aggressive(FantasyLeagueEntry interestedLeague, FantasyPlayerRanking[] playerOptions)
        {
            List<FantasyPlayerRanking> results = new List<FantasyPlayerRanking>();

            if (interestedLeague.StartingP > 0)
            {
                var allPitchers = playerOptions.Where(p => p.Position == BaseballPosition.pos_P).Where(p => p.Hits > 100);
                if (allPitchers.Count() == 0)
                    allPitchers = playerOptions.Where(p => p.Position == BaseballPosition.pos_P);
                var maxPitchCalc = allPitchers.Select(p => p.AVG / p.ERA).Max();
                var selectedPitcher = allPitchers.Where(p => p.AVG / p.ERA == maxPitchCalc).FirstOrDefault();
                results.Add(selectedPitcher);
                interestedLeague.StartingP--;
            }

            List<BaseballPosition> remainingSpots = interestedLeague.RemainingRosterSpots();
            while (results.Count < remainingSpots.Count)
            {
                var bestRemaining = playerOptions.Where(p => remainingSpots.Contains(p.Position) 
                    && (results.Sum(r=>r.Value) + p.Value < interestedLeague.SalaryCap)).Where(p => p.Hits > 100);
                if (bestRemaining.Count() == 0)
                    bestRemaining = playerOptions.Where(p => remainingSpots.Contains(p.Position));
                var maxCalc = bestRemaining.Select(p => p.AVG / p.ERA).Max();
                var selected = bestRemaining.Where(p => remainingSpots.Contains(p.Position) && p.AVG / p.ERA == maxCalc).FirstOrDefault();
                results.Add(selected);
                switch (selected.Position)
                {
                    case BaseballPosition.pos_1B:
                        interestedLeague.Starting1B--;
                        break;
                    case BaseballPosition.pos_2B:
                        interestedLeague.Starting2B--;
                        break;
                    case BaseballPosition.pos_3B:
                        interestedLeague.Starting3B--;
                        break;
                    case BaseballPosition.pos_SS:
                        interestedLeague.StartingSS--;
                        break;
                    case BaseballPosition.pos_OF:
                        interestedLeague.StartingOF--;
                        break;
                    case BaseballPosition.pos_C:
                        interestedLeague.StartingC--;
                        break;
                    case BaseballPosition.pos_P:
                        interestedLeague.StartingP--;
                        break;
                }

                remainingSpots = interestedLeague.RemainingRosterSpots();
            }

            return results;
        }
    }
}
