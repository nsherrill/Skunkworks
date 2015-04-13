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
            try
            {
                List<FantasyPlayerRanking> players = new List<FantasyPlayerRanking>();

                var options = playerOptions.ToList();
                int playerCount = options.Count;
                double initialSalary = interestedLeague.SalaryCap;

                for (int i = 0; i < playerCount; i++)
                {
                    FantasyPlayerRanking nextPlayer;
                    switch (configType)
                    {
                        case ConfigType.Conservative:
                            nextPlayer = DetermineNextPlayer_Conservative(interestedLeague, options.ToArray());
                            break;
                        case ConfigType.Aggressive:
                            nextPlayer = DetermineNextPlayer_Aggressive(interestedLeague, options.ToArray());
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    switch (nextPlayer.Position)
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

                    interestedLeague.SalaryCap -= nextPlayer.Value;
                    var index = options.Select(o => o.ForeignId).ToList().IndexOf(nextPlayer.ForeignId);
                    options.RemoveAt(index);
                    index = options.Select(o => o.ForeignId).ToList().IndexOf(nextPlayer.ForeignId);
                    while (index >= 0)
                    {
                        options.RemoveAt(index);
                        index = options.Select(o => o.ForeignId).ToList().IndexOf(nextPlayer.ForeignId);
                    }
                }

                interestedLeague.SalaryCap = initialSalary;

                FantasyRoster result = new FantasyRoster()
                {
                    ForeignLeagueId = long.Parse(interestedLeague.ForeignId),
                    PlayersToSelect = players.ToArray()
                };
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private FantasyPlayerRanking DetermineNextPlayer_Aggressive(FantasyLeagueEntry interestedLeague, FantasyPlayerRanking[] playerOptions)
        {

            FantasyPlayerRanking selected = null;

            if (interestedLeague.StartingP > 0)
            {
                var allPitchers = playerOptions.Where(p => p.Position == BaseballPosition.pos_P).Where(p => p.Hits > 100);
                if (allPitchers.Count() == 0)
                    allPitchers = playerOptions.Where(p => p.Position == BaseballPosition.pos_P);
                var maxPitchCalc = allPitchers.Select(p => p.AVG / p.ERA).Max();
                var selectedPitcher = allPitchers.Where(p => p.AVG / p.ERA == maxPitchCalc).FirstOrDefault();
                selected = selectedPitcher;
                interestedLeague.StartingP--;
            }
            else
            {
                List<BaseballPosition> remainingSpots = interestedLeague.RemainingRosterSpots();
                var bestRemaining = playerOptions
                    .Where(p => remainingSpots
                        .Contains(p.Position)
                        && p.Hits > 100
                        && p.Value < interestedLeague.SalaryCap);
                if (bestRemaining.Count() == 0)
                    bestRemaining = playerOptions
                        .Where(p => remainingSpots.Contains(p.Position)
                        && p.Value < interestedLeague.SalaryCap);

                var maxCalc = bestRemaining.Select(p => p.AVG / p.ERA).Max();
                selected = bestRemaining
                    .Where(p => remainingSpots.Contains(p.Position)
                        && p.AVG / p.ERA == maxCalc
                        && p.Value < interestedLeague.SalaryCap)
                    .FirstOrDefault();
            }

            return selected;
        }

        private FantasyPlayerRanking DetermineNextPlayer_Conservative(FantasyLeagueEntry interestedLeague, FantasyPlayerRanking[] playerOptions)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            FantasyPlayerRanking selected = null;

            List<BaseballPosition> remainingSpots = interestedLeague.RemainingRosterSpots();
            var bestRemaining = playerOptions
                .Where(p => remainingSpots
                    .Contains(p.Position)
                    && p.Hits > 100
                    && p.Value < interestedLeague.SalaryCap);
            if (bestRemaining.Count() == 0)
                bestRemaining = playerOptions
                    .Where(p => remainingSpots.Contains(p.Position)
                        && p.Value < interestedLeague.SalaryCap);
            if (bestRemaining.Count() == 0)
            {
            }
            selected = bestRemaining.OrderByDescending(p => p.AVG / p.ERA).First();
            sw.Stop();

            return selected;
        }
    }
}
