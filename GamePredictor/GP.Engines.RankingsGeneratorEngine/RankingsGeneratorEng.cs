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
        Queue<long> lastPlayerList = new Queue<long>();
        long maxInQueue = 100;
        long MAX_DRAFT_COUNT_PER_QUEUE { get { return 3; } }
        Random random;
        public RankingsGeneratorEng()
        {
            random = new Random(DateTime.Now.Millisecond);
        }

        private void AddPlayerToQueue(long newId)
        {
            lastPlayerList.Enqueue(newId);
            if (lastPlayerList.Count > maxInQueue)
                lastPlayerList.Dequeue();
        }

        public FantasyRoster GenerateRoster(FantasyLeagueEntry interestedLeague, FantasyPlayerRanking[] playerOptions, ConfigType configType)
        {
            try
            {
                List<FantasyPlayerRanking> players = new List<FantasyPlayerRanking>();

                List<string> tooManyTeamNames = new List<string>();

                var options = playerOptions.ToList();
                int initial1B = interestedLeague.Starting1B;
                int initial2B = interestedLeague.Starting2B;
                int initial3B = interestedLeague.Starting3B;
                int initialC = interestedLeague.StartingC;
                int initialOF = interestedLeague.StartingOF;
                int initialP = interestedLeague.StartingP;
                int initialSS = interestedLeague.StartingSS;
                int playerCount = interestedLeague.Starting1B
                    + interestedLeague.Starting2B
                    + interestedLeague.Starting3B
                    + interestedLeague.StartingC
                    + interestedLeague.StartingOF
                    + interestedLeague.StartingP
                    + interestedLeague.StartingSS;
                double initialSalary = interestedLeague.SalaryCap;

                double retryCount = 0;
                List<FantasyPlayerRanking> poppedPlayers = new List<FantasyPlayerRanking>();

                for (int i = 0; i < playerCount; i++)
                {
                    FantasyPlayerRanking nextPlayer;

                    string methodName = string.Format("DetermineNextPlayer_{0}", configType);
                    var methodToCall = typeof(RankingsGeneratorEng).GetMethod(methodName);
                    if (methodToCall != null)
                    {
                        var methodResult = methodToCall.Invoke(this, new object[] { interestedLeague, options.ToArray() });
                        nextPlayer = (FantasyPlayerRanking)methodResult;
                    }
                    else
                    {
                        throw new NotSupportedException("ConfigType not yet supported for calculations: " + configType);
                    }
                    /*
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
                     */

                    if (nextPlayer == null)
                    {
                        retryCount++;
                        i--;
                        for (int retries = 0; retries < Math.Ceiling(retryCount / 3.0) && (players.Count > 0); retries++)
                        {
                            var playerToPop = players[players.Count - 1];
                            interestedLeague.SalaryCap += playerToPop.Value;
                            AdjustLeaguesPositionCount(interestedLeague, playerToPop.Position, 1);
                            players.RemoveAt(players.Count - 1);
                            i--;
                        }

                        if (options.Count == 0)
                            return null;
                        continue;
                    }

                    AdjustLeaguesPositionCount(interestedLeague, nextPlayer.Position, -1);

                    players.Add(nextPlayer);

                    interestedLeague.SalaryCap -= nextPlayer.Value;
                    var index = options.Select(o => o.ForeignId).ToList().IndexOf(nextPlayer.ForeignId);
                    poppedPlayers.Add(options[index]);
                    options.RemoveAt(index);
                    index = options.Select(o => o.ForeignId).ToList().IndexOf(nextPlayer.ForeignId);
                    while (index >= 0)
                    {
                        poppedPlayers.Add(options[index]);
                        options.RemoveAt(index);
                        index = options.Select(o => o.ForeignId).ToList().IndexOf(nextPlayer.ForeignId);
                    }

                    if (players.Count(p => p.TeamName == nextPlayer.TeamName) > 3)
                        tooManyTeamNames.Add(nextPlayer.TeamName);

                    poppedPlayers.AddRange(options.Where(o => tooManyTeamNames.Contains(o.TeamName)));
                    options.RemoveAll(o => tooManyTeamNames.Contains(o.TeamName));
                }

                interestedLeague.SalaryCap = initialSalary;
                interestedLeague.Starting1B = initial1B;
                interestedLeague.Starting2B = initial2B;
                interestedLeague.Starting3B = initial3B;
                interestedLeague.StartingC = initialC;
                interestedLeague.StartingP = initialP;
                interestedLeague.StartingOF = initialOF;
                interestedLeague.StartingSS = initialSS;

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
        public FantasyPlayerRanking DetermineNextPlayer_TopAvailablePPG(FantasyLeagueEntry interestedLeague, FantasyPlayerRanking[] playerOptions)
        {
            //ConfigType.TopAvailablePPG;

            FantasyPlayerRanking selected = null;

            List<BaseballPosition> remainingSpots = interestedLeague.RemainingRosterSpots();
            var bestRemaining = playerOptions
                .Where(p => remainingSpots
                    .Contains(p.Position)
                    && p.ABLast7 > 10
                    && p.Value < interestedLeague.SalaryCap
                    && lastPlayerList.Count(l => l == p.Id) < MAX_DRAFT_COUNT_PER_QUEUE);
            if (bestRemaining.Count() == 0)
                bestRemaining = playerOptions
                    .Where(p => remainingSpots.Contains(p.Position)
                    && p.Value < interestedLeague.SalaryCap
                    && lastPlayerList.Count(l => l == p.Id) < MAX_DRAFT_COUNT_PER_QUEUE);

            var orderedList = bestRemaining.OrderByDescending(p => p.PPG);

            var desiredIndex = random.Next(Math.Min(3, orderedList.Count()));
            selected = orderedList.ElementAtOrDefault(desiredIndex);

            return selected;
        }

        public FantasyPlayerRanking DetermineNextPlayer_TopAvailablePPG_PitcherFirst(FantasyLeagueEntry interestedLeague, FantasyPlayerRanking[] playerOptions)
        {
            //ConfigType.TopAvailablePPG_PitcherFirst

            FantasyPlayerRanking selected = null;

            if (interestedLeague.StartingP > 0)
            {
                var allPitchers = playerOptions.Where(p => p.Position == BaseballPosition.pos_P
                    && p.ABLast7 > 10
                    && lastPlayerList.Count(l => l == p.Id) < MAX_DRAFT_COUNT_PER_QUEUE);
                if (allPitchers.Count() == 0)
                    allPitchers = playerOptions.Where(p => p.Position == BaseballPosition.pos_P
                        && lastPlayerList.Count(l => l == p.Id) < MAX_DRAFT_COUNT_PER_QUEUE);
                var orderedList = allPitchers.OrderByDescending(p => p.PPG);

                var desiredIndex = random.Next(Math.Min(3, orderedList.Count()));
                selected = orderedList.ElementAtOrDefault(desiredIndex);

                interestedLeague.StartingP--;
            }
            else
            {
                List<BaseballPosition> remainingSpots = interestedLeague.RemainingRosterSpots();
                var bestRemaining = playerOptions
                    .Where(p => remainingSpots
                        .Contains(p.Position)
                        && p.ABLast7 > 10
                        && p.Value < interestedLeague.SalaryCap
                        && lastPlayerList.Count(l => l == p.Id) < MAX_DRAFT_COUNT_PER_QUEUE);
                if (bestRemaining.Count() == 0)
                    bestRemaining = playerOptions
                        .Where(p => remainingSpots.Contains(p.Position)
                        && p.Value < interestedLeague.SalaryCap
                        && lastPlayerList.Count(l => l == p.Id) < MAX_DRAFT_COUNT_PER_QUEUE);

                var orderedList = bestRemaining.OrderByDescending(p => p.PPG);

                var desiredIndex = random.Next(Math.Min(3, orderedList.Count()));
                selected = orderedList.ElementAtOrDefault(desiredIndex);

            }

            return selected;
        }

        public FantasyPlayerRanking DetermineNextPlayer_TopAvailablePPGPerValue(FantasyLeagueEntry interestedLeague, FantasyPlayerRanking[] playerOptions)
        {
            //ConfigType.TopAvailablePPGPerValue

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            FantasyPlayerRanking selected = null;

            List<BaseballPosition> remainingSpots = interestedLeague.RemainingRosterSpots();
            var bestRemaining = playerOptions
                .Where(p => remainingSpots
                    .Contains(p.Position)
                    && p.ABLast7 > 10
                    && p.Value < interestedLeague.SalaryCap
                    && p.Value > (interestedLeague.SalaryCap / ((double)remainingSpots.Count) / 2.0)
                    && lastPlayerList.Count(l => l == p.Id) < MAX_DRAFT_COUNT_PER_QUEUE);
            if (bestRemaining.Count() == 0)
            {
                bestRemaining = playerOptions
                    .Where(p => remainingSpots.Contains(p.Position)
                   && p.Value < interestedLeague.SalaryCap
                   && lastPlayerList.Count(l => l == p.Id) < MAX_DRAFT_COUNT_PER_QUEUE);
            }
            else
            {

            }

            if (bestRemaining.Count() == 0)
            {
            }
            var orderedList = bestRemaining.OrderByDescending(p => p.PPG / p.Value);

            var desiredIndex = random.Next(Math.Min(3, orderedList.Count()));
            selected = orderedList.ElementAtOrDefault(desiredIndex);
            sw.Stop();

            return selected;
        }

        public FantasyPlayerRanking DetermineNextPlayer_TopAvailableValue(FantasyLeagueEntry interestedLeague, FantasyPlayerRanking[] playerOptions)
        {
            //ConfigType.TopAvailableValue

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            FantasyPlayerRanking selected = null;

            List<BaseballPosition> remainingSpots = interestedLeague.RemainingRosterSpots();
            var bestRemaining = playerOptions
                .Where(p => remainingSpots
                    .Contains(p.Position)
                    && p.ABLast7 > 10
                    && p.Value < interestedLeague.SalaryCap
                    && lastPlayerList.Count(l => l == p.Id) < MAX_DRAFT_COUNT_PER_QUEUE);
            if (bestRemaining.Count() == 0)
            {
                bestRemaining = playerOptions
                    .Where(p => remainingSpots.Contains(p.Position)
                        && p.Value < interestedLeague.SalaryCap
                        && lastPlayerList.Count(l => l == p.Id) < MAX_DRAFT_COUNT_PER_QUEUE);
            }
            else
            {

            }

            if (bestRemaining.Count() == 0)
            {
            }
            var orderedList = bestRemaining.OrderByDescending(p => p.Value);

            var desiredIndex = random.Next(Math.Min(3, orderedList.Count()));
            selected = orderedList.ElementAtOrDefault(desiredIndex);

            sw.Stop();

            return selected;
        }

        public FantasyPlayerRanking DetermineNextPlayer_TopAvailableValue_HomeOnly(FantasyLeagueEntry interestedLeague, FantasyPlayerRanking[] playerOptions)
        {
            //ConfigType.TopAvailableValue_HomeOnly

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            FantasyPlayerRanking selected = null;

            List<BaseballPosition> remainingSpots = interestedLeague.RemainingRosterSpots();
            var bestRemaining = playerOptions
                .Where(p => remainingSpots
                    .Contains(p.Position)
                    && p.ABLast7 > 10
                    && p.Value < interestedLeague.SalaryCap
                    && true //p.IsHome 
                    && lastPlayerList.Count(l => l == p.Id) < MAX_DRAFT_COUNT_PER_QUEUE);
            if (bestRemaining.Count() == 0)
            {
                bestRemaining = playerOptions
                    .Where(p => remainingSpots.Contains(p.Position)
                        && p.Value < interestedLeague.SalaryCap
                        && lastPlayerList.Count(l => l == p.Id) < MAX_DRAFT_COUNT_PER_QUEUE);
            }
            else
            {

            }

            if (bestRemaining.Count() == 0)
            {
            }
            var orderedList = bestRemaining.OrderByDescending(p => p.Value);

            var desiredIndex = random.Next(Math.Min(3, orderedList.Count()));
            selected = orderedList.ElementAtOrDefault(desiredIndex);
            sw.Stop();

            return selected;
        }
        #endregion
    }
}
