using GP.Shared.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Accessors.DatabaseAccessor
{
    public static class DbParser
    {
        public static Team GetTeam(SqlDataReader rdr)
        {
            return new Team()
            {
                Id = rdr.GetLong("Id"),
                Name = rdr.GetString("Name"),
                ForeignId = rdr.GetString("ForeignId"),
                Sport = rdr.GetEnum<SportType>("Sport", SportType.Baseball),
            };
        }

        public static GameEvent GetGameEvent(SqlDataReader rdr)
        {
            return new GameEvent()
            {
                Attendence = rdr.GetInt("Attendence"),
                Date = rdr.GetDateTime("Date", DateTime.MinValue),
                ForeignId = rdr.GetString("ForeignId"),
                GameNotes = rdr.GetString("Notes"),
                Sport = rdr.GetEnum<SportType>("Sport", SportType.Baseball),
                Stadium = rdr.GetString("Stadium"),
                Weather_Degrees = rdr.GetDouble("Weather_Degrees"),
                Weather_Type = rdr.GetEnum<WeatherType>("Weather_Type", WeatherType.Unknown),
                WindSpeed = rdr.GetDouble("WindSpeed"),
            };
        }

        public static PlayerEventStats GetPlayerHittingStats(SqlDataReader rdr)
        {
            List<ValuePair> data = new List<ValuePair>();

            data.Add(new Shared.Common.ValuePair("AB", rdr.GetInt("AB")));
            data.Add(new Shared.Common.ValuePair("R", rdr.GetInt("R")));
            data.Add(new Shared.Common.ValuePair("H", rdr.GetInt("H")));
            data.Add(new Shared.Common.ValuePair("RBI", rdr.GetInt("RBI")));
            data.Add(new Shared.Common.ValuePair("BB", rdr.GetInt("BB")));
            data.Add(new Shared.Common.ValuePair("SO", rdr.GetInt("SO")));
            data.Add(new Shared.Common.ValuePair("NP", rdr.GetInt("NP")));
            data.Add(new Shared.Common.ValuePair("AVG", rdr.GetDouble("AVG")));
            data.Add(new Shared.Common.ValuePair("OBP", rdr.GetDouble("OBP")));
            data.Add(new Shared.Common.ValuePair("SLG", rdr.GetDouble("SLG")));

            return new PlayerEventStats()
            {
                DataType = PlayerDataType.Hitting,
                ForeignGameEventId = rdr.GetInt("ForeignGameId"),
                ForeignPlayerName = rdr.GetString("ForeignPlayerName"),
                ForeignPlayerId = rdr.GetInt("ForeignPlayerId"),
                ForeignTeamId = rdr.GetString("ForeignTeamId"),
                Sport = rdr.GetEnum<SportType>("Sport", SportType.Baseball),
                Data = data.ToArray(),
            };
        }

        public static PlayerEventStats GetPlayerPitchingStats(SqlDataReader rdr)
        {
            List<ValuePair> data = new List<ValuePair>();

            data.Add(new Shared.Common.ValuePair("IP", rdr.GetDouble("IP")));
            data.Add(new Shared.Common.ValuePair("H", rdr.GetInt("H")));
            data.Add(new Shared.Common.ValuePair("R", rdr.GetInt("R")));
            data.Add(new Shared.Common.ValuePair("ER", rdr.GetInt("ER")));
            data.Add(new Shared.Common.ValuePair("BB", rdr.GetInt("BB")));
            data.Add(new Shared.Common.ValuePair("SO", rdr.GetInt("SO")));
            data.Add(new Shared.Common.ValuePair("HR", rdr.GetInt("HR")));
            data.Add(new Shared.Common.ValuePair("ERA", rdr.GetDouble("ERA")));
            data.Add(new Shared.Common.ValuePair("NP", rdr.GetInt("NP")));
            data.Add(new Shared.Common.ValuePair("ST", rdr.GetInt("ST")));

            return new PlayerEventStats()
            {
                DataType = PlayerDataType.Pitching,
                ForeignGameEventId = rdr.GetInt("ForeignGameId"),
                ForeignPlayerId = rdr.GetInt("ForeignPlayerId"),
                ForeignPlayerName = rdr.GetString("ForeignPlayerName"),
                ForeignTeamId = rdr.GetString("ForeignTeamId"),
                Sport = rdr.GetEnum<SportType>("Sport", SportType.Baseball),
                Data = data.ToArray(),
            };
        }

        internal static object GetFantasyLeague(SqlDataReader rdr)
        {
            var result = new FantasyLeagueEntry()
            {
                ForeignId = rdr.GetString("ForeignId"),
                Sport = rdr.GetEnum<SportType>("Sport", SportType.Baseball),
                BuyIn = rdr.GetDouble("BuyIn"),
                ForeignSite = rdr.GetString("ForeignSite"),
                ForeignTitle = rdr.GetString("ForeignTitle"),
                Id = rdr.GetLong("Id"),
                IsActive = rdr.GetBool("IsActive"),
                LeagueType = rdr.GetEnum<LeagueType>("LeagueType", LeagueType.Auction),
                ParticipantCount = rdr.GetInt("ParticipantCount"),
                StartDate = rdr.GetDateTime("StartDate", DateTime.MinValue),
                Url = rdr.GetString("Url"),
                SalaryCap = rdr.GetDouble("SalaryCap"),

                Starting1B = rdr.GetInt("Starting1B"),
                Starting2B = rdr.GetInt("Starting2B"),
                Starting3B = rdr.GetInt("Starting3B"),
                StartingC = rdr.GetInt("StartingC"),
                StartingOF = rdr.GetInt("StartingOF"),
                StartingP = rdr.GetInt("StartingP"),
                StartingSS = rdr.GetInt("StartingSS"),
            };

            return result;
        }

        internal static object GetFantasyPlayer(SqlDataReader rdr)
        {
            var result = new FantasyPlayer()
            {
                ForeignId = rdr.GetInt("ForeignId").ToString(),
                ForeignLeagueId = rdr.GetString("ForeignLeagueId"),
                Id = rdr.GetLong("Id"),
                GamesPlayed = rdr.GetInt("GamesPlayed"),
                Name = rdr.GetString("Name"),
                Position = rdr.GetEnum<BaseballPosition>("Position", BaseballPosition.pos_P),
                PPG = rdr.GetDouble("PPG"),
                TeamAbr = rdr.GetString("TeamAbr"),
                Value = rdr.GetDouble("Value"),
            };

            return result;
        }

        internal static object GetFantasyPlayerRanking(SqlDataReader rdr)
        {
            var result = new FantasyPlayerRanking()
            {
                ForeignId = rdr.GetInt("ForeignId").ToString(),
                ForeignLeagueId = rdr.GetString("ForeignLeagueId"),
                GamesPlayed = rdr.GetInt("GamesPlayed"),
                Name = rdr.GetString("Name"),
                Position = rdr.GetEnum<BaseballPosition>("Position", BaseballPosition.pos_P),
                PPG = rdr.GetDouble("PPG"),
                Value = rdr.GetDouble("Value"),
                NormCalc = rdr.GetDouble("NormCalc"),
                Hits = rdr.GetInt("Hits"),
                TeamName = rdr.GetString("TeamName"),
                HRs = rdr.GetInt("HRs"),
                AVG = rdr.GetDouble("AVG"),
                ERA = rdr.GetDouble("ERA"),
                IsHome = rdr.GetBool("IsHome"),
                TeamAbr = rdr.GetString("TeamAbr"),
            };

            return result;
        }

        internal static object GetValuePair(SqlDataReader rdr)
        {
            var result = new ValuePair(rdr.GetString("Key"), rdr["Value"]);

            return result;
        }

        internal static object GetFutureGameEvent(SqlDataReader rdr)
        {
            string foreignIdTeam1 = rdr.GetString("ForeignIdTeam1", string.Empty);
            string foreignIdTeam2 = rdr.GetString("ForeignIdTeam2", string.Empty);

            var result = new FutureGameEvent()
            {
                Date = rdr.GetDateTime("Date", DateTime.MinValue),
                ForeignId = rdr.GetString("ForeignId"),
                PrecipitationChance = rdr.GetDouble("Precipitation"),
                Sport = rdr.GetEnum<SportType>("Sport", SportType.Baseball),
                Stadium = rdr.GetString("Stadium"),
                Weather_HighDegrees = rdr.GetDouble("Weather_HighDegrees"),
                Weather_LowDegrees = rdr.GetDouble("Weather_LowDegrees"),
            };

            if (!string.IsNullOrEmpty(foreignIdTeam1)
                && !string.IsNullOrEmpty(foreignIdTeam2))
            {
                result.Teams = new Team[]{
                    new Team(){
                        ForeignId = foreignIdTeam1
                    },
                    new Team(){
                        ForeignId = foreignIdTeam2
                    }
                };
            }

            return result;
        }

        internal static object GetStartingPitcher(SqlDataReader rdr)
        {
            var result = new PlayerEventStats()
            {
                ForeignTeamId = rdr.GetString("ForeignTeamId"),
                ForeignGameEventId = rdr.GetInt("ForeignGameId"),
                ForeignPlayerId = rdr.GetInt("ForeignPlayerId"),
                ForeignPlayerName = rdr.GetString("ForeignPlayerName"),
                DataType = PlayerDataType.StartingPitcher,
                PlayerId = rdr.GetInt("PlayerId"),
                GameEventId = rdr.GetInt("GameId"),
                TeamId = rdr.GetInt("TeamId"),
                Sport = rdr.GetEnum<SportType>("Sport", SportType.Baseball),
            };
            return result;
        }
    }
}
