using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Shared.Common
{
    public class FantasyLeagueEntry : FantasyRosterDefinition
    {
        public long Id { get; set; }
        public string ForeignTitle { get; set; }
        public string ForeignSite { get; set; }
        public string Url { get; set; }
        public double BuyIn { get; set; }
        public int ParticipantCount { get; set; }
        public DateTime StartDate { get; set; }
        public LeagueType LeagueType { get; set; }
        public SportType Sport { get; set; }
        public bool IsActive { get; set; }

        public bool IsRegistered { get; set; }
        public bool IsInterested { get; set; }
        public string HistoricalForeignId { get; set; }
        public string HistoricalUrl { get; set; }
        public int FinalRank { get; set; }

        public override string ToString()
        {
            string title = string.Format("{0}: {1} ({2})", ForeignSite, ForeignTitle, BuyIn);
            if (!string.IsNullOrWhiteSpace(title))
                return title;
            return base.ToString();
        }

        public List<BaseballPosition> RemainingRosterSpots()
        {
            List<BaseballPosition> remainingSpots = new List<BaseballPosition>();
            for (int i = 0; i < this.StartingP; i++)
                remainingSpots.Add(BaseballPosition.pos_P);
            for (int i = 0; i < this.StartingOF; i++)
                remainingSpots.Add(BaseballPosition.pos_OF);
            for (int i = 0; i < this.StartingC; i++)
                remainingSpots.Add(BaseballPosition.pos_C);
            for (int i = 0; i < this.StartingSS; i++)
                remainingSpots.Add(BaseballPosition.pos_SS);
            for (int i = 0; i < this.Starting1B; i++)
                remainingSpots.Add(BaseballPosition.pos_1B);
            for (int i = 0; i < this.Starting2B; i++)
                remainingSpots.Add(BaseballPosition.pos_2B);
            for (int i = 0; i < this.Starting3B; i++)
                remainingSpots.Add(BaseballPosition.pos_3B);

            return remainingSpots;
        }
    }
}
