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

                List<string> tooManyTeamNames = new List<string>();

                var options = playerOptions.ToList();
                int playerCount = interestedLeague.Starting1B
                    + interestedLeague.Starting2B
                    + interestedLeague.Starting3B
                    + interestedLeague.StartingC
                    + interestedLeague.StartingOF
                    + interestedLeague.StartingP
                    + interestedLeague.StartingSS;
                double initialSalary = interestedLeague.SalaryCap;

                double retryCount = 0;

                for (int i = 0; i < playerCount; i++)
                {
                    FantasyPlayerRanking nextPlayer;
                    switch (configType)
                    {
                        case ConfigType.Conservative:
                            nextPlayer = DetermineNextPlayer_Conservative(interestedLeague, options.ToArray());
                            break;
                        case ConfigType.Conservative_MostExpensive:
                            nextPlayer = DetermineNextPlayer_ConservativeMostExpensive(interestedLeague, options.ToArray());
                            break;
                        case ConfigType.Conservative_MostExpensiveHomePlayer:
                            nextPlayer = DetermineNextPlayer_ConservativeMostExpensiveHomePlayer(interestedLeague, options.ToArray());
                            break;
                        case ConfigType.Aggressive:
                            nextPlayer = DetermineNextPlayer_Aggressive(interestedLeague, options.ToArray());
                            break;
                        case ConfigType.Aggressive_PitcherFirst:
                            nextPlayer = DetermineNextPlayer_AggressivePitcherFirst(interestedLeague, options.ToArray());
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    if (nextPlayer == null)
                    {
                        retryCount++;
                        for (int retries = 0; retries < Math.Ceiling(retryCount / 3.0) && (players.Count > 0); retries++)
                        {
                            var playerToPop = players[players.Count - 1];
                            interestedLeague.SalaryCap += playerToPop.Value;
                            AdjustLeaguesPositionCount(interestedLeague, playerToPop.Position, 1);
                            players.RemoveAt(players.Count - 1);
                            i--;
                        }
                        continue;
                    }

                    AdjustLeaguesPositionCount(interestedLeague, nextPlayer.Position, -1);

                    players.Add(nextPlayer);

                    interestedLeague.SalaryCap -= nextPlayer.Value;
                    var index = options.Select(o => o.ForeignId).ToList().IndexOf(nextPlayer.ForeignId);
                    options.RemoveAt(index);
                    index = options.Select(o => o.ForeignId).ToList().IndexOf(nextPlayer.ForeignId);
                    while (index >= 0)
                    {
                        options.RemoveAt(index);
                        index = options.Select(o => o.ForeignId).ToList().IndexOf(nextPlayer.ForeignId);
                    }

                    if (players.Count(p => p.TeamName == nextPlayer.TeamName) > 3)
                        tooManyTeamNames.Add(nextPlayer.TeamName);

                    options.RemoveAll(o => tooManyTeamNames.Contains(o.TeamName));
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

        private void AdjustLeaguesPositionCount(FantasyLeagueEntry interestedLeague, BaseballPosition position, int changeCount)
        {
            switch (position)
            {
                case BaseballPosition.pos_1B:
                    interestedLeague.Starting1B += changeCount;
                    break;
                case BaseballPosition.pos_2B:
                    interestedLeague.Starting2B += changeCount;
                    break;
                case BaseballPosition.pos_3B:
                    interestedLeague.Starting3B += changeCount;
                    break;
                case BaseballPosition.pos_SS:
                    interestedLeague.StartingSS += changeCount;
                    break;
                case BaseballPosition.pos_OF:
                    interestedLeague.StartingOF += changeCount;
                    break;
                case BaseballPosition.pos_C:
                    interestedLeague.StartingC += changeCount;
                    break;
                case BaseballPosition.pos_P:
                    interestedLeague.StartingP += changeCount;
                    break;
                default:
                    throw new NotSupportedException("Position isn't yet supported");
            }
        }

        #region DeterminePlayers
        private FantasyPlayerRanking DetermineNextPlayer_Aggressive(FantasyLeagueEntry interestedLeague, FantasyPlayerRanking[] playerOptions)
        {

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

            selected = bestRemaining.OrderByDescending(p => p.PPG).FirstOrDefault();

            return selected;
        }

        private FantasyPlayerRanking DetermineNextPlayer_AggressivePitcherFirst(FantasyLeagueEntry interestedLeague, FantasyPlayerRanking[] playerOptions)
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

                selected = bestRemaining.OrderByDescending(p => p.AVG / p.ERA).FirstOrDefault();
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
                    && p.Value < interestedLeague.SalaryCap
                    && p.Value > (interestedLeague.SalaryCap / ((double)remainingSpots.Count) / 2.0));
            if (bestRemaining.Count() == 0)
            {
                bestRemaining = playerOptions
                    .Where(p => remainingSpots.Contains(p.Position)
                   && p.Value < interestedLeague.SalaryCap);
            }
            else
            {

            }

            if (bestRemaining.Count() == 0)
            {
            }
            selected = bestRemaining.OrderByDescending(p => p.PPG / p.Value).FirstOrDefault();
            //selected = bestRemaining.OrderByDescending(p => p.AVG / p.ERA).FirstOrDefault();
            sw.Stop();

            return selected;
        }
        
        private FantasyPlayerRanking DetermineNextPlayer_ConservativeMostExpensive(FantasyLeagueEntry interestedLeague, FantasyPlayerRanking[] playerOptions)
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
            {
                bestRemaining = playerOptions
                    .Where(p => remainingSpots.Contains(p.Position)
                   && p.Value < interestedLeague.SalaryCap);
            }
            else
            {

            }

            if (bestRemaining.Count() == 0)
            {
            }
            selected = bestRemaining.OrderByDescending(p => p.Value).FirstOrDefault();
            sw.Stop();

            return selected;
        }
        
        private FantasyPlayerRanking DetermineNextPlayer_ConservativeMostExpensiveHomePlayer(FantasyLeagueEntry interestedLeague, FantasyPlayerRanking[] playerOptions)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            FantasyPlayerRanking selected = null;

            List<BaseballPosition> remainingSpots = interestedLeague.RemainingRosterSpots();
            var bestRemaining = playerOptions
                .Where(p => remainingSpots
                    .Contains(p.Position)
                    && p.Hits > 100
                    && p.Value < interestedLeague.SalaryCap
                    && p.IsHome);
            if (bestRemaining.Count() == 0)
            {
                bestRemaining = playerOptions
                    .Where(p => remainingSpots.Contains(p.Position)
                   && p.Value < interestedLeague.SalaryCap);
            }
            else
            {

            }

            if (bestRemaining.Count() == 0)
            {
            }
            selected = bestRemaining.OrderByDescending(p => p.Value).FirstOrDefault();
            sw.Stop();

            return selected;
        }
        #endregion
    }
}
