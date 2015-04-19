using GP.Shared.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Accessors.DatabaseAccessor
{
    public interface ILocalBaseballDataAcc
    {
        MassData FindDataForDate(DateTime desiredDate);

        GameEvent WriteGameData(GameEvent gameData);

        FutureGameEvent[] FindFutureGamesForDate(DateTime desiredDate);

        FutureGameEvent WriteFutureGameData(Shared.Common.FutureGameEvent game);

        FantasyLeagueEntry[] WriteFantasyLeagues(FantasyLeagueEntry[] source);

        FantasyLeagueEntry[] UpdateFutureFantasyLeagueInterest(double minDollar, double maxDollar);

        FantasyPlayer[] WritePlayers(FantasyPlayer[] players);

        void WriteLeagueRoster(FantasyRosterDefinition def);

        FantasyPlayer[] FindAllPlayersForLeague(string leagueForeignId);

        FantasyPlayerRanking[] GetPlayerRankings(string leagueForeignId);

        RankingsConfiguration GetRankingsConfiguration();

        DateTime GetMaxAvailableDataDate(SourceType source);
    }

    public class LocalBaseballDataAcc : ILocalBaseballDataAcc
    {
        IDatabaseAcc dbAcc = new DatabaseAcc();

        #region publics
        public MassData FindDataForDate(DateTime desiredDate)
        {
            string sql = "select * from dbo.gameevents where date > @startDateTime and date< @endDateTime";
            List<ValuePair> paramList = new List<ValuePair>();
            paramList.Add(new ValuePair("@startDateTime", desiredDate.Date));
            paramList.Add(new ValuePair("@endDateTime", desiredDate.Date.AddDays(1).AddSeconds(-1)));

            List<List<object>> resultReader = dbAcc.ExecuteQuery(sql, paramList.ToArray(), (rdr, index) =>
            {
                return DbParser.GetGameEvent(rdr);
            });

            List<GameEvent> games = new List<GameEvent>();

            if (resultReader != null
                && resultReader.Count > 0)
            {
                if (resultReader[0] != null
                    && resultReader[0].Count > 0)
                {
                    foreach (var res in resultReader[0])
                        if (res is GameEvent)
                            games.Add(res as GameEvent);
                }
            }

            if (games.Count == 0)
                return null;

            var result = new MassData()
            {
                Games = games.ToArray(),
            };
            return result;
        }

        public GameEvent WriteGameData(GameEvent gameData)
        {
            string sql = @"

merge into dbo.gameevents as target
    using (select @game_foreignid, @game_sport, @game_date, @game_stadium, @game_attendence, @game_weather_degrees, @game_weather_type, @game_windspeed, @game_notes) 
    as source (ForeignId, Sport, [Date], Stadium, Attendence, Weather_Degrees, Weather_Type, Windspeed, Notes) 
    on (source.foreignid = target.foreignid and source.sport = target.sport)
    when matched then
	    update set 
		    sport = source.sport,
		    [Date] = source.[Date],
		    Stadium = source.Stadium,
		    Attendence = source.Attendence,
		    Weather_Degrees = source.Weather_Degrees,
		    Weather_Type = source.Weather_Type,
		    Windspeed = source.Windspeed,
		    Notes = source.Notes
    when not matched then
	    insert (foreignid, sport, [Date], stadium, attendence, weather_degrees, weather_type, windspeed, notes)
		    values (source.foreignid, source.sport, source.[Date], source.stadium, source.attendence, 
			    source.weather_degrees, source.weather_type, source.windspeed, source.notes);
				

declare @players table 
	(Name nvarchar(200), foreignid nvarchar(200), sport nvarchar(100))
insert into @players (Name, foreignid, sport) values
	--(@players_name_1, @players_foreignid_1, @players_sport_1)
    {BASEBALL_PLAYERS}

merge into dbo.players as target
    using (select * from @players group by name, foreignid, sport)
    as source (Name, foreignid, sport)
    on (source.foreignid = target.foreignid and source.sport = target.sport)
    when matched then
	    update set 
		    Name = source.Name
    when not matched then
	    insert (Name, foreignid, sport)
		    values (source.name, source.foreignid, source.sport);
				

declare @teams table 
	(Name nvarchar(200), foreignid nvarchar(200), sport nvarchar(100))
insert into @teams (Name, foreignid, sport) values
	--(@teams_name_1, @teams_foreignid_1, @teams_sport_1)
    {BASEBALL_TEAMS}

merge into dbo.teams as target
    using (select * from @teams)
    as source (Name, foreignid, sport)
    on (source.foreignid = target.foreignid and source.sport = target.sport)
    when matched then
	    update set 
		    Name = source.Name
    when not matched then
	    insert (Name, foreignid, sport)
		    values (source.name, source.foreignid, source.sport);


declare @baseball_playerhittingstats table 
	(foreignplayerid nvarchar(200), foreignteamid nvarchar(200), foreigngameid nvarchar(200), 
		ab int, r int, h int, rbi int, bb int, so int, np int, [avg] decimal (10,5), obp decimal(10,5), slg decimal(10,5))
insert into @baseball_playerhittingstats (foreignplayerid, foreignteamid, foreigngameid, ab, r, h, rbi, bb, so, np, [avg], obp, slg) values
	--(@baseball_playerhittingstats_foreignplayerid_1, @baseball_playerhittingstats_foreignteamid_1, @baseball_playerhittingstats_foreigngameid_1, 
	--@baseball_playerhittingstats_ab_1, @baseball_playerhittingstats_r_1, @baseball_playerhittingstats_h_1, @baseball_playerhittingstats_rbi_1, 
	--@baseball_playerhittingstats_bb_1, @baseball_playerhittingstats_so_1, @baseball_playerhittingstats_np_1, @baseball_playerhittingstats_avg_1, 
	--@baseball_playerhittingstats_obp_1, @baseball_playerhittingstats_slg_1)
    {BASEBALL_PLAYERHITTINGSTATS}

    merge into baseball.playerhittingstats as target
    using (select phs.*, p.id, ge.id, t.id
		from @baseball_playerhittingstats phs 
			join dbo.players p on phs.foreignplayerid = p.foreignid and p.sport = 'Baseball'
			join dbo.gameevents ge on ge.foreignid = phs.foreigngameid and ge.sport = 'Baseball'
			join dbo.teams t on t.foreignid = phs.foreignteamid and t.sport = 'Baseball') 
    as source (foreignplayerid, foreignteamid, foreigngameid, ab, r, h, rbi, bb, so, np, [avg], obp, slg, playerid, gameid, teamid) 
    on (source.playerid = target.playerid and source.teamid = target.teamid and source.gameid = target.gameid)
    when matched then
	    update set 
		    ab = source.ab,
		    r = source.r,
		    h = source.h,
		    rbi = source.rbi,
		    bb = source.bb,
		    so = source.so,
		    np = source.np,
		    [avg] = source.[avg],
		    obp = source.obp,
		    slg = source.slg
    when not matched then
	    insert (playerid, teamid, gameid, ab, r, h, rbi, bb, so, np, [avg], obp, slg)
		    values (source.playerid, source.teamid, source.gameid, source.ab, source.r, 
				source.h, source.rbi, source.bb, source.so, source.np, source.[avg], 
				source.obp, source.slg);


declare @baseball_playerpitchingstats table 
	(foreignplayerid nvarchar(200), foreignteamid nvarchar(200), foreigngameid nvarchar(200), 
		IP decimal(10,2), H int, R int, ER int, BB int, SO int, HR int, ERA decimal(10,5), NP int, ST int)
insert into @baseball_playerpitchingstats (foreignplayerid, foreignteamid, foreigngameid, IP, H, R, ER, BB, SO, HR, ERA, NP, ST) values
	--(@baseball_playerpitchingstats_foreignplayerid_1, @baseball_playerpitchingstats_foreignteamid_1, @baseball_playerpitchingstats_foreigngameid_1, 
	--@baseball_playerpitchingstats_IP_1, @baseball_playerpitchingstats_H_1, @baseball_playerpitchingstats_R_1, @baseball_playerpitchingstats_ER_1, 
	--@baseball_playerpitchingstats_BB_1, @baseball_playerpitchingstats_SO_1, @baseball_playerpitchingstats_HR_1, @baseball_playerpitchingstats_ERA_1, 
	--@baseball_playerpitchingstats_NP_1, @baseball_playerpitchingstats_ST_1)
    {BASEBALL_PLAYERPITCHINGSTATS}

    merge into baseball.playerpitchingstats as target
    using (select pps.*, p.id, ge.id, t.id
		from @baseball_playerpitchingstats pps 
			join dbo.players p on pps.foreignplayerid = p.foreignid and p.sport = 'Baseball'
			join dbo.gameevents ge on ge.foreignid = pps.foreigngameid and ge.sport = 'Baseball'
			join dbo.teams t on t.foreignid = pps.foreignteamid and t.sport = 'Baseball') 
    as source (foreignplayerid, foreignteamid, foreigngameid, IP, H, R, ER, BB, SO, HR, ERA, NP, ST, playerid, gameid, teamid) 
    on (source.playerid = target.playerid and source.teamid = target.teamid and source.gameid = target.gameid)
    when matched then
	    update set 
		    IP = source.IP,
		    H = source.H,
		    R = source.R,
		    ER = source.ER,
		    BB = source.BB,
		    SO = source.SO,
		    HR = source.HR,
		    ERA = source.ERA,
		    NP = source.NP,
		    ST = source.ST
    when not matched then
	    insert (playerid, teamid, gameid, IP, H, R, ER, BB, SO, HR, ERA, NP, ST)
		    values (source.playerid, source.teamid, source.gameid, source.IP, source.H, 
				source.R, source.ER, source.BB, source.SO, source.HR, source.ERA, 
				source.NP, source.ST);



    select * from dbo.gameevents where foreignid = @game_foreignid

    select phs.*
            , ge.foreignid as ForeignGameId
            , p.foreignid as ForeignPlayerId
            , t.foreignid as ForeignTeamId
        from baseball.playerhittingstats phs
            join dbo.gameevents ge on phs.gameid = ge.id
            join dbo.teams t on phs.teamid = t.id
            join dbo.players p on phs.playerid = p.id
        where ge.foreignid = @game_foreignid

    select pps.*
            , ge.foreignid as ForeignGameId
            , p.foreignid as ForeignPlayerId
            , t.foreignid as ForeignTeamId
        from baseball.playerpitchingstats pps
            join dbo.gameevents ge on pps.gameid = ge.id
            join dbo.teams t on pps.teamid = t.id
            join dbo.players p on pps.playerid = p.id
        where ge.foreignid = @game_foreignid
    ";

            List<ValuePair> paramList = new List<ValuePair>();
            sql = UpdateSqlForGame(sql, gameData, paramList);
            sql = UpdateSqlForPlayers(sql, gameData, paramList);
            sql = UpdateSqlForTeams(sql, gameData, paramList);
            sql = UpdateSqlForPlayerHittingStats(sql, gameData, paramList);
            sql = UpdateSqlForPlayerPitchingStats(sql, gameData, paramList);

            List<List<object>> resultReader = dbAcc.ExecuteQuery(sql, paramList.ToArray(), (rdr, index) =>
             {
                 switch (index)
                 {
                     case 0:
                         return DbParser.GetGameEvent(rdr);
                         break;
                     case 1:
                         return DbParser.GetPlayerHittingStats(rdr);
                         break;
                     case 2:
                         return DbParser.GetPlayerPitchingStats(rdr);
                         break;
                 }
                 throw new Exception("Index not handled!");
             });

            GameEvent result = null;
            if (resultReader != null
                && resultReader.Count > 0)
            {
                if (resultReader[0] != null
                    && resultReader[0].Count > 0
                    && resultReader[0][0] is GameEvent)
                {
                    result = resultReader[0][0] as GameEvent;
                }

                List<PlayerEventStats> stats = new List<PlayerEventStats>();
                if (resultReader.Count > 1
                    && resultReader[1] != null
                    && resultReader[1].Count > 0)
                {
                    foreach (var stat in resultReader[1])
                        stats.Add(stat as PlayerEventStats);
                }

                if (resultReader.Count > 2
                    && resultReader[2] != null
                    && resultReader[2].Count > 0)
                {
                    foreach (var stat in resultReader[2])
                        stats.Add(stat as PlayerEventStats);
                }

                result.PlayerStats = stats.ToArray();

                return result;
            }

            return null;
        }

        public FutureGameEvent[] FindFutureGamesForDate(DateTime desiredDate)
        {
            string sql = @"
    
    select fge.*, 
		    t1.foreignid as ForeignIdTeam1, t2.foreignid as ForeignIdTeam2
        from dbo.futuregameevents fge
            join (select max(playerid) as playerid, gameid from baseball.StartingPitchers group by gameid) p1 on p1.gameid = fge.id
            join (select min(playerid) as playerid, gameid from baseball.StartingPitchers group by gameid) p2 on p2.gameid = fge.id and p2.playerid <> p1.playerid
            join baseball.startingpitchers sp1 on sp1.playerid = p1.playerid and sp1.gameid = p1.gameid
            join baseball.startingpitchers sp2 on sp2.playerid = p2.playerid and sp2.gameid = p2.gameid
            join dbo.teams t1 on sp1.teamid = t1.id
            join dbo.teams t2 on sp2.teamid = t2.id and t1.id <> t2.id
        where date > @startDateTime and date< @endDateTime
        order by fge.id
    
    select t.* 
	    from dbo.teams t
		    join baseball.startingpitchers sp on sp.teamid = t.id
		    join dbo.futuregameevents fge on fge.id = sp.gameid
        where fge.date > @startDateTime and fge.date< @endDateTime
	    order by t.id
    
    select p.*
            , ge.foreignid as ForeignGameId
            , p.foreignid as ForeignPlayerId
            , t.foreignid as ForeignTeamId
            , ge.id as GameId
            , p.id as PlayerId
            , t.id as TeamId
            , ge.sport as Sport
        from baseball.startingpitchers sp
            join dbo.futuregameevents ge on ge.id = sp.gameid
            join dbo.teams t on sp.teamid = t.id
            join dbo.players p on sp.playerid = p.id
        where ge.date > @startDateTime and ge.date< @endDateTime
	    order by p.id";
            List<ValuePair> paramList = new List<ValuePair>();
            paramList.Add(new ValuePair("@startDateTime", desiredDate.Date));
            paramList.Add(new ValuePair("@endDateTime", desiredDate.Date.AddDays(1).AddSeconds(-1)));

            List<List<object>> resultReader = dbAcc.ExecuteQuery(sql, paramList.ToArray(),
                (rdr, index) =>
                {
                    if (index == 0)
                        return DbParser.GetFutureGameEvent(rdr);
                    else if (index == 1)
                        return DbParser.GetTeam(rdr);
                    else if (index == 2)
                        return DbParser.GetStartingPitcher(rdr);
                    return null;
                });

            List<FutureGameEvent> games = new List<FutureGameEvent>();
            Dictionary<string, PlayerEventStats> pitchers = new Dictionary<string, PlayerEventStats>();
            List<Team> teams = new List<Team>();

            if (resultReader != null
                && resultReader.Count > 0)
            {
                if (resultReader[0] != null
                    && resultReader[0].Count > 0)
                {
                    foreach (var res in resultReader[0])
                        if (res is FutureGameEvent)
                            games.Add(res as FutureGameEvent);
                }
                if (resultReader.Count > 1
                    && resultReader[1] != null
                    && resultReader[1].Count > 0)
                {
                    foreach (var res in resultReader[1])
                        if (res is Team)
                        {
                            var tempTeam = res as Team;
                            teams.Add(tempTeam);
                        }
                }
                if (resultReader.Count > 2
                    && resultReader[2] != null
                    && resultReader[2].Count > 0)
                {
                    foreach (var res in resultReader[2])
                        if (res is PlayerEventStats)
                        {
                            var tempPitch = res as PlayerEventStats;
                            var key = tempPitch.ForeignGameEventId.ToString() + "__" + tempPitch.ForeignTeamId;
                            pitchers.Add(key, tempPitch);
                        }
                }
            }

            foreach (var game in games)
            {
                string foreignGameEventId;
                string foreignTeamId1;
                string foreignTeamId2;

                foreignGameEventId = game.ForeignId;

                if (game.Teams != null
                    && game.Teams.Length == 2)
                {
                    foreignTeamId1 = game.Teams[0].ForeignId;
                    foreignTeamId2 = game.Teams[1].ForeignId;
                }
                else continue;

                if (string.IsNullOrEmpty(foreignTeamId1)
                    || string.IsNullOrEmpty(foreignTeamId2)
                    || string.IsNullOrEmpty(foreignGameEventId))
                    continue;

                var key1 = foreignGameEventId + "__" + foreignTeamId1;
                var key2 = foreignGameEventId + "__" + foreignTeamId2;
                game.StartingPitchers = new PlayerEventStats[]
                {
                    pitchers[key1], 
                    pitchers[key2]
                };

                var team1 = teams.Where(t => t.ForeignId == foreignTeamId1).FirstOrDefault();
                var team2 = teams.Where(t => t.ForeignId == foreignTeamId2).FirstOrDefault();

                game.Teams[0] = team1;
                game.Teams[1] = team2;
            }

            if (games.Count == 0)
                return null;

            return games.ToArray();
        }

        public FutureGameEvent WriteFutureGameData(FutureGameEvent gameData)
        {
            string sql = @"

merge into dbo.futuregameevents as target
    using (select @game_foreignid, @game_sport, @game_date, @game_stadium, @game_weather_highdegrees, @game_weather_lowdegrees, @game_precipitation) 
    as source (ForeignId, Sport, [Date], Stadium, Weather_LowDegrees, Weather_HighDegrees, Precipitation) 
    on (source.foreignid = target.foreignid and source.sport = target.sport)
    when matched then
	    update set 
		    sport = source.sport,
		    [Date] = source.[Date],
		    Stadium = source.Stadium,
		    Weather_HighDegrees = source.Weather_HighDegrees,
		    Weather_LowDegrees = source.Weather_LowDegrees,
		    Precipitation = source.Precipitation
    when not matched then
	    insert (foreignid, sport, [Date], stadium, Weather_HighDegrees, Weather_LowDegrees, Precipitation)
		    values (source.foreignid, source.sport, source.[Date], source.stadium, source.Weather_HighDegrees, 
			    source.Weather_LowDegrees, source.Precipitation);
				

declare @players table 
	(Name nvarchar(200), foreignid nvarchar(200), sport nvarchar(100))
insert into @players (Name, foreignid, sport) values
	--(@players_name_1, @players_foreignid_1, @players_sport_1)
    {BASEBALL_PLAYERS}

merge into dbo.players as target
    using (select * from @players group by name, foreignid, sport)
    as source (Name, foreignid, sport)
    on (source.foreignid = target.foreignid and source.sport = target.sport)
    when matched then
	    update set 
		    Name = source.Name
    when not matched then
	    insert (Name, foreignid, sport)
		    values (source.name, source.foreignid, source.sport);
				

declare @teams table 
	(Name nvarchar(200), foreignid nvarchar(200), sport nvarchar(100))
insert into @teams (Name, foreignid, sport) values
	--(@teams_name_1, @teams_foreignid_1, @teams_sport_1)
    {BASEBALL_TEAMS}

merge into dbo.teams as target
    using (select * from @teams)
    as source (Name, foreignid, sport)
    on (source.foreignid = target.foreignid and source.sport = target.sport)
    when matched then
	    update set 
		    Name = source.Name
    when not matched then
	    insert (Name, foreignid, sport)
		    values (source.name, source.foreignid, source.sport);
				

declare @baseball_startingpitchers table 
	(playerid nvarchar(100), gameid nvarchar(100), teamid nvarchar(100))
insert into @baseball_startingpitchers (playerid, gameid, teamid) values
	--(@baseball_startingpitchers_playerid_1, @baseball_startingpitchers_gameid_1, @baseball_startingpitchers_teamid_1)
    {BASEBALL_STARTINGPITCHERS}

merge into baseball.startingpitchers as target
    using (select p.id, f.id, t.id 
        from @baseball_startingpitchers sp
            join dbo.players p on sp.playerid = p.foreignid
            join dbo.futuregameevents f on sp.gameid = f.foreignid
            join dbo.teams t on sp.teamid = t.foreignid)
    as source (playerid, gameid, teamid)
    on (source.playerid = target.playerid and source.gameid = target.gameid and source.teamid = target.teamid)
    when not matched then
	    insert (playerid, gameid, teamid)
		    values (source.playerid, source.gameid, source.teamid);


    select * from dbo.futuregameevents where foreignid = @game_foreignid

    select p.*
            , ge.foreignid as ForeignGameId
            , p.foreignid as ForeignPlayerId
            , t.foreignid as ForeignTeamId
            , ge.id as GameId
            , p.id as PlayerId
            , t.id as TeamId
            , ge.sport as Sport
        from baseball.startingpitchers sp
            join dbo.futuregameevents ge on ge.id = sp.gameid
            join dbo.teams t on sp.teamid = t.id
            join dbo.players p on sp.playerid = p.id
        where ge.foreignid = @game_foreignid

    ";

            List<ValuePair> paramList = new List<ValuePair>();
            sql = UpdateSqlForFutureGame(sql, gameData, paramList);
            sql = UpdateSqlForFutureGamePlayers(sql, gameData, paramList);
            sql = UpdateSqlForFutureGameStartingPitchers(sql, gameData, paramList);
            sql = UpdateSqlForFutureGameTeams(sql, gameData, paramList);

            List<List<object>> resultReader = dbAcc.ExecuteQuery(sql, paramList.ToArray(), (rdr, index) =>
            {
                switch (index)
                {
                    case 0:
                        return DbParser.GetFutureGameEvent(rdr);
                        break;
                    case 1:
                        return DbParser.GetStartingPitcher(rdr);
                        break;
                }
                throw new Exception("Index not handled!");
            });

            FutureGameEvent result = null;
            if (resultReader != null
                && resultReader.Count > 0)
            {
                if (resultReader[0] != null
                    && resultReader[0].Count > 0
                    && resultReader[0][0] is FutureGameEvent)
                {
                    result = resultReader[0][0] as FutureGameEvent;
                }

                List<PlayerEventStats> players = new List<PlayerEventStats>();
                if (resultReader.Count > 1
                    && resultReader[1] != null
                    && resultReader[1].Count > 0)
                {
                    foreach (var stat in resultReader[1])
                        players.Add(stat as PlayerEventStats);
                }

                result.StartingPitchers = players.ToArray();

                return result;
            }

            return null;
        }

        public FantasyLeagueEntry[] WriteFantasyLeagues(FantasyLeagueEntry[] source)
        {
            string sql = @"

declare @fantasyLeagues table (ForeignId nvarchar(100), ForeignTitle nvarchar(500), ForeignSite nvarchar(500), Url nvarchar(500), BuyIn decimal(12,6), 
        PartCount int, StartDate datetime, LeagueType nvarchar(100), Sport nvarchar(100), IsActive bit, SalaryCap decimal(12,2),
        Starting1B int, Starting2B int, Starting3B int, StartingP int, StartingOF int, StartingC int, StartingSS int)
    --(@baseball_fantasyLeague_foreignid_1, @baseball_fantasyLeague_foreigntitle_1, @baseball_fantasyLeague_foreignsite_1, 
    --@baseball_fantasyLeague_url_1, @baseball_fantasyLeague_buyin_1, @baseball_fantasyLeague_partcount_1, @baseball_fantasyLeague_startdate_1, 
    --@baseball_fantasyLeague_leaguetype_1, @baseball_fantasyLeague_sport_1, @baseball_fantasyLeague_isactive_1)
    {FANTASY_LEAGUES}

merge into dbo.FantasyLeagues as target
    using (select * from @fantasyLeagues) 
    as source (ForeignId, ForeignTitle, ForeignSite, Url, BuyIn, PartCount, StartDate, LeagueType, Sport, IsActive, SalaryCap,
        Starting1B , Starting2B , Starting3B , StartingP , StartingOF , StartingC , StartingSS ) 
    on (source.ForeignId = target.ForeignId and source.sport = target.sport)
    when not matched then
	    insert (ForeignId, ForeignTitle, ForeignSite, Url, BuyIn, ParticipationCount, StartDate, LeagueType, Sport, IsActive, SalaryCap,
                Starting1B , Starting2B , Starting3B , StartingP , StartingOF , StartingC , StartingSS )
		    values (source.ForeignId, source.ForeignTitle, source.ForeignSite, source.Url, source.BuyIn, source.PartCount, 
                source.StartDate, source.LeagueType, source.Sport, source.IsActive, source.SalaryCap,
                source.Starting1B , source.Starting2B , source.Starting3B , source.StartingP , source.StartingOF , source.StartingC , source.StartingSS );
				
select * from dbo.fantasyleagues where sport = 'Baseball' and IsActive = 1;
    ";

            List<ValuePair> paramList = new List<ValuePair>();
            sql = UpdateSqlForFantasyLeagues(sql, source, paramList);

            List<List<object>> resultReader = dbAcc.ExecuteQuery(sql, paramList.ToArray(), (rdr, index) =>
            {
                return DbParser.GetFantasyLeague(rdr);
            });

            List<FantasyLeagueEntry> result = new List<FantasyLeagueEntry>();
            if (resultReader != null
                && resultReader.Count > 0)
            {
                if (resultReader[0] != null
                    && resultReader[0].Count > 0
                    && resultReader[0][0] is FantasyLeagueEntry)
                {
                    foreach (var league in resultReader[0])
                        result.Add((FantasyLeagueEntry)league);
                }

                return result.ToArray();
            }

            return null;
        }

        public FantasyLeagueEntry[] UpdateFutureFantasyLeagueInterest(double minDollar, double maxDollar)
        {
            string sql = @"

    update dbo.fantasyleagues set isactive = 0 where startdate < @startDate;
    update dbo.fantasyleagues set isinterested = 0 where startdate > @startDate;
    update dbo.fantasyleagues 
        set isinterested = 1 
        where startdate > @startDate 
            and leaguetype = 'auction'
            and buyin >= @minDollar 
            and buyin <= @maxdollar;

    select * from dbo.fantasyleagues where sport = 'Baseball' and IsActive = 1 and isinterested = 1 and startdate > @startDate;
    ";

            List<ValuePair> paramList = new List<ValuePair>();
            paramList.Add(new Shared.Common.ValuePair("@startDate", DateTime.Now));
            paramList.Add(new Shared.Common.ValuePair("@minDollar", minDollar));
            paramList.Add(new Shared.Common.ValuePair("@maxDollar", maxDollar));

            List<List<object>> resultReader = dbAcc.ExecuteQuery(sql, paramList.ToArray(), (rdr, index) =>
            {
                return DbParser.GetFantasyLeague(rdr);
            });

            List<FantasyLeagueEntry> result = new List<FantasyLeagueEntry>();
            if (resultReader != null
                && resultReader.Count > 0)
            {
                if (resultReader[0] != null
                    && resultReader[0].Count > 0
                    && resultReader[0][0] is FantasyLeagueEntry)
                {
                    foreach (var league in resultReader[0])
                        result.Add((FantasyLeagueEntry)league);
                }

                return result.ToArray();
            }

            return null;
        }

        public void WriteLeagueRoster(FantasyRosterDefinition def)
        {
            string sql = @"

        update dbo.FantasyLeagues set
                StartingP = @startingP,
                StartingOF = @startingOF,
                StartingC = @startingC,
                StartingSS = @startingSS,
                Starting1B = @starting1B,
                Starting2B = @starting2B,
                Starting3B = @starting3B
            where ForeignId = @foreignLeagueId
    ";

            List<ValuePair> paramList = new List<ValuePair>();
            paramList.Add(new ValuePair("@startingP", def.StartingP));
            paramList.Add(new ValuePair("@startingOF", def.StartingOF));
            paramList.Add(new ValuePair("@startingC", def.StartingC));
            paramList.Add(new ValuePair("@startingSS", def.StartingSS));
            paramList.Add(new ValuePair("@starting1B", def.Starting1B));
            paramList.Add(new ValuePair("@starting2B", def.Starting2B));
            paramList.Add(new ValuePair("@starting3B", def.Starting3B));
            paramList.Add(new ValuePair("@foreignLeagueId", def.ForeignId));

            dbAcc.ExecuteNonQuery(sql, paramList.ToArray());
        }

        public FantasyPlayer[] WritePlayers(FantasyPlayer[] source)
        {
            string sql = @"

declare @fantasyPlayers table (ForeignId int, ForeignLeagueId nvarchar(100), Name nvarchar(100), 
        Value decimal(12,6), Position nvarchar(100), PPG decimal(12,6), GamesPlayed int, TeamAbr nvarchar(100))
    --(@baseball_fantasyPlayer_foreignid_1, @baseball_fantasyPlayer_foreignleagueid_1, @baseball_fantasyPlayer_name_1, 
    --@baseball_fantasyPlayer_value_1, @baseball_fantasyPlayer_position_1, @baseball_fantasyPlayer_ppg_1, 
    --@baseball_fantasyPlayer_gamesplayed_1, @baseball_fantasyPlayer_teamabr_1)
    {FANTASY_PLAYERS}

merge into dbo.FantasyPlayers as target
    using (select * from @fantasyPlayers) 
    as source (ForeignId, ForeignLeagueId, Name, Value, Position, PPG, GamesPlayed, TeamAbr) 
    on (source.ForeignId = target.ForeignId and source.ForeignLeagueId = target.ForeignLeagueId)
    when not matched then
	    insert (ForeignId, ForeignLeagueId, Name, Value, Position, PPG, GamesPlayed, TeamAbr)
		    values (source.ForeignId, source.ForeignLeagueId, source.Name, source.Value, source.Position, source.PPG, source.GamesPlayed, source.TeamAbr);
				
select * from dbo.FantasyPlayers where foreignleagueid in ({FANTASY_LEAGUEIDS});
    ";

            List<ValuePair> paramList = new List<ValuePair>();
            sql = UpdateSqlForFantasyPlayers(sql, source, paramList);
            sql = UpdateSqlForFantasyPlayers_LEAGUEIDS(sql, source);

            List<List<object>> resultReader = dbAcc.ExecuteQuery(sql, paramList.ToArray(), (rdr, index) =>
            {
                return DbParser.GetFantasyPlayer(rdr);
            });

            List<FantasyPlayer> result = new List<FantasyPlayer>();
            if (resultReader != null
                && resultReader.Count > 0)
            {
                if (resultReader[0] != null
                    && resultReader[0].Count > 0
                    && resultReader[0][0] is FantasyPlayer)
                {
                    foreach (var player in resultReader[0])
                        result.Add((FantasyPlayer)player);
                }

                return result.ToArray();
            }

            return null;
        }

        public FantasyPlayer[] FindAllPlayersForLeague(string leagueForeignId)
        {
            string sql = "select * from dbo.FantasyPlayers where foreignleagueid = @leagueForeignId";

            List<ValuePair> paramList = new List<ValuePair>();
            paramList.Add(new ValuePair("leagueForeignId", leagueForeignId));

            List<List<object>> resultReader = dbAcc.ExecuteQuery(sql, paramList.ToArray(), (rdr, index) =>
            {
                return DbParser.GetFantasyPlayer(rdr);
            });

            List<FantasyPlayer> result = new List<FantasyPlayer>();
            if (resultReader != null
                && resultReader.Count > 0)
            {
                if (resultReader[0] != null
                    && resultReader[0].Count > 0
                    && resultReader[0][0] is FantasyPlayer)
                {
                    foreach (var player in resultReader[0])
                        result.Add((FantasyPlayer)player);
                }

                return result.ToArray();
            }

            return null;
        }

        public FantasyPlayerRanking[] GetPlayerRankings(string leagueForeignId)
        {
            string sql = "exec dbo.GetHittingRankings '{0}'";
            sql = string.Format(sql, leagueForeignId);

            List<ValuePair> paramList = new List<ValuePair>();

            List<List<object>> resultReader = dbAcc.ExecuteQuery(sql, paramList.ToArray(), (rdr, index) =>
            {
                return DbParser.GetFantasyPlayerRanking(rdr);
            });

            List<FantasyPlayerRanking> result = new List<FantasyPlayerRanking>();
            if (resultReader != null
                && resultReader.Count > 0)
            {
                if (resultReader[0] != null
                    && resultReader[0].Count > 0
                    && resultReader[0][0] is FantasyPlayerRanking)
                {
                    foreach (var player in resultReader[0])
                        result.Add((FantasyPlayerRanking)player);
                }

                return result.ToArray();
            }

            return null;

        }

        public RankingsConfiguration GetRankingsConfiguration()
        {
            string sql = "select * from dbo.configurations";

            List<ValuePair> paramList = new List<ValuePair>();

            List<List<object>> resultReader = dbAcc.ExecuteQuery(sql, paramList.ToArray(), (rdr, index) =>
            {
                return DbParser.GetValuePair(rdr);
            });

            RankingsConfiguration result = new RankingsConfiguration();
            List<ValuePair> tempRes = new List<ValuePair>();
            if (resultReader != null
                && resultReader.Count > 0)
            {
                if (resultReader[0] != null
                    && resultReader[0].Count > 0
                    && resultReader[0][0] is ValuePair)
                {
                    foreach (var pair in resultReader[0])
                        tempRes.Add((ValuePair)pair);
                }
            }

            result.ConservativePercent = tempRes.GetValue<double>("ConservativePercent", 50.0);
            result.AggressivePercent = tempRes.GetValue<double>("AggressivePercent", 50.0);

            return result;
        }

        public DateTime GetMaxAvailableDataDate(SourceType source)
        {
            string sql = @"
    SELECT top 1 [date] 
      FROM [GamePredictor].[dbo].[GameEvents]
      order by [date] desc";

            if (source == SourceType.SportingCharts)
            {
                sql = @"
    select top 1 * from
    (SELECT top 1 [dateretrieved] 
      FROM [GamePredictor].[baseball].[currentHittingStats]
      order by [dateretrieved] desc
    union
    select top 1 [dateretrieved]
      from [gamepredictor].[baseball].[currentPitchingStats]
      order by [dateretrieved] desc
    union 
    select '1/1/2010')
    as q
    order by [dateretrieved] desc";
            }

            List<ValuePair> paramList = new List<ValuePair>();

            List<List<object>> resultReader = dbAcc.ExecuteQuery(sql, paramList.ToArray(), (rdr, index) =>
            {
                return (DateTime)rdr[0];
            });

            return (DateTime)resultReader[0][0];
        }
        #endregion

        #region fantasy player writer privates
        private string UpdateSqlForFantasyPlayers_LEAGUEIDS(string sql, FantasyPlayer[] source)
        {
            string idList = string.Join(",", source.Select(p => p.ForeignLeagueId).Distinct());
            sql = sql.Replace("{FANTASY_LEAGUEIDS}", idList);
            return sql;
        }

        private string UpdateSqlForFantasyPlayers(string sql, FantasyPlayer[] source, List<ValuePair> paramList)
        {
            if (source != null
                && source.Length > 0)
            {
                StringBuilder fps = new StringBuilder();
                fps.AppendLine(@"insert into @fantasyPlayers (ForeignId, ForeignLeagueId, Name, Value, Position, PPG, GamesPlayed, TeamAbr)
	VALUES 
        ");
                // fantasy players
                //(@baseball_fantasyPlayer_foreignid_1, @baseball_fantasyPlayer_foreignleagueid_1, @baseball_fantasyPlayer_name_1, 
                //@baseball_fantasyPlayer_value_1, @baseball_fantasyPlayer_position_1, @baseball_fantasyPlayer_ppg_1, 
                //@baseball_fantasyPlayer_gamesplayed_1, @baseball_fantasyPlayer_teamabr_1
                for (int i = 0; i < source.Length; i++)
                {
                    fps.Append("(@baseball_fantasyPlayer_foreignid_");
                    fps.Append(i);
                    fps.Append(", @baseball_fantasyPlayer_foreignleagueid_");
                    fps.Append(i);
                    fps.Append(", @baseball_fantasyPlayer_name_");
                    fps.Append(i);
                    fps.Append(", @baseball_fantasyPlayer_value_");
                    fps.Append(i);
                    fps.Append(", @baseball_fantasyPlayer_position_");
                    fps.Append(i);
                    fps.Append(", @baseball_fantasyPlayer_ppg_");
                    fps.Append(i);
                    fps.Append(", @baseball_fantasyPlayer_gamesplayed_");
                    fps.Append(i);
                    fps.Append(", @baseball_fantasyPlayer_teamabr_");
                    fps.Append(i);
                    fps.Append(')');
                    if (source.Length > i + 1)
                        fps.AppendLine(",");

                    paramList.Add(new ValuePair("@baseball_fantasyPlayer_foreignid_" + i, source[i].ForeignId));
                    paramList.Add(new ValuePair("@baseball_fantasyPlayer_foreignleagueid_" + i, source[i].ForeignLeagueId));
                    paramList.Add(new ValuePair("@baseball_fantasyPlayer_name_" + i, source[i].Name));
                    paramList.Add(new ValuePair("@baseball_fantasyPlayer_value_" + i, source[i].Value));
                    paramList.Add(new ValuePair("@baseball_fantasyPlayer_position_" + i, source[i].Position.ToString()));
                    paramList.Add(new ValuePair("@baseball_fantasyPlayer_ppg_" + i, source[i].PPG));
                    paramList.Add(new ValuePair("@baseball_fantasyPlayer_gamesplayed_" + i, source[i].GamesPlayed));
                    paramList.Add(new ValuePair("@baseball_fantasyPlayer_teamabr_" + i, source[i].TeamAbr));
                }

                sql = sql.Replace("{FANTASY_PLAYERS}", fps.ToString());

            }
            return sql;
        }
        #endregion

        #region game writer privates
        private string UpdateSqlForPlayerPitchingStats(string sql, Shared.Common.GameEvent gameData, List<Shared.Common.ValuePair> paramList)
        {

            // playerpitchingstats
            //(@baseball_playerpitchingstats_foreignplayerid_1, @baseball_playerpitchingstats_foreignteamid_1, @baseball_playerpitchingstats_foreigngameid_1, 
            //@baseball_playerpitchingstats_IP_1, @baseball_playerpitchingstats_H_1, @baseball_playerpitchingstats_R_1, @baseball_playerpitchingstats_ER_1, 
            //@baseball_playerpitchingstats_BB_1, @baseball_playerpitchingstats_SO_1, @baseball_playerpitchingstats_HR_1, @baseball_playerpitchingstats_ERA_1, 
            //@baseball_playerpitchingstats_NP_1, @baseball_playerpitchingstats_ST_1)
            StringBuilder sb_pps = new StringBuilder();
            var pitchingStats = gameData.PlayerStats.Where(p => p.DataType == Shared.Common.PlayerDataType.Pitching).ToArray();
            for (int i = 0; i < pitchingStats.Length; i++)
            {
                sb_pps.Append("(@baseball_playerpitchingstats_foreignplayerid_");
                sb_pps.Append(i);
                sb_pps.Append(", @baseball_playerpitchingstats_foreignteamid_");
                sb_pps.Append(i);
                sb_pps.Append(", @baseball_playerpitchingstats_foreigngameid_");
                sb_pps.Append(i);
                sb_pps.Append(", @baseball_playerpitchingstats_IP_");
                sb_pps.Append(i);
                sb_pps.Append(", @baseball_playerpitchingstats_H_");
                sb_pps.Append(i);
                sb_pps.Append(", @baseball_playerpitchingstats_R_");
                sb_pps.Append(i);
                sb_pps.Append(", @baseball_playerpitchingstats_ER_");
                sb_pps.Append(i);
                sb_pps.Append(", @baseball_playerpitchingstats_BB_");
                sb_pps.Append(i);
                sb_pps.Append(", @baseball_playerpitchingstats_SO_");
                sb_pps.Append(i);
                sb_pps.Append(", @baseball_playerpitchingstats_HR_");
                sb_pps.Append(i);
                sb_pps.Append(", @baseball_playerpitchingstats_ERA_");
                sb_pps.Append(i);
                sb_pps.Append(", @baseball_playerpitchingstats_NP_");
                sb_pps.Append(i);
                sb_pps.Append(", @baseball_playerpitchingstats_ST_");
                sb_pps.Append(i);
                sb_pps.Append(')');
                if (pitchingStats.Length > i + 1)
                    sb_pps.AppendLine(",");

                paramList.Add(new ValuePair("@baseball_playerpitchingstats_foreignplayerid_" + i, pitchingStats[i].ForeignPlayerId));
                paramList.Add(new ValuePair("@baseball_playerpitchingstats_foreignteamid_" + i, pitchingStats[i].ForeignTeamId));
                paramList.Add(new ValuePair("@baseball_playerpitchingstats_foreigngameid_" + i, pitchingStats[i].ForeignGameEventId));
                paramList.Add(new ValuePair("@baseball_playerpitchingstats_IP_" + i, pitchingStats[i].GetData("IP")));
                paramList.Add(new ValuePair("@baseball_playerpitchingstats_H_" + i, pitchingStats[i].GetData("H")));
                paramList.Add(new ValuePair("@baseball_playerpitchingstats_R_" + i, pitchingStats[i].GetData("R")));
                paramList.Add(new ValuePair("@baseball_playerpitchingstats_ER_" + i, pitchingStats[i].GetData("ER")));
                paramList.Add(new ValuePair("@baseball_playerpitchingstats_BB_" + i, pitchingStats[i].GetData("BB")));
                paramList.Add(new ValuePair("@baseball_playerpitchingstats_SO_" + i, pitchingStats[i].GetData("SO")));
                paramList.Add(new ValuePair("@baseball_playerpitchingstats_HR_" + i, pitchingStats[i].GetData("HR")));
                paramList.Add(new ValuePair("@baseball_playerpitchingstats_ERA_" + i, pitchingStats[i].GetData("ERA")));
                paramList.Add(new ValuePair("@baseball_playerpitchingstats_NP_" + i, pitchingStats[i].GetData("NP")));
                paramList.Add(new ValuePair("@baseball_playerpitchingstats_ST_" + i, pitchingStats[i].GetData("ST")));
            }

            sql = sql.Replace("{BASEBALL_PLAYERPITCHINGSTATS}", sb_pps.ToString());
            //string tempData = "('ricky-nolasco', 'Minnesota Twins', '340615106', 5.1, 9, 3, 3, 2, 5, 0, 5.66, 103, 62)";
            //sql = sql.Replace("{BASEBALL_PLAYERPITCHINGSTATS}", tempData);

            return sql;
        }

        private string UpdateSqlForPlayerHittingStats(string sql, Shared.Common.GameEvent gameData, List<Shared.Common.ValuePair> paramList)
        {
            // playerhittingstats
            //(@baseball_playerhittingstats_foreignplayerid_1, @baseball_playerhittingstats_foreignteamid_1, @baseball_playerhittingstats_foreigngameid_1, 
            //@baseball_playerhittingstats_ab_1, @baseball_playerhittingstats_r_1, @baseball_playerhittingstats_h_1, @baseball_playerhittingstats_rbi_1, 
            //@baseball_playerhittingstats_bb_1, @baseball_playerhittingstats_so_1, @baseball_playerhittingstats_np_1, @baseball_playerhittingstats_avg_1, 
            //@baseball_playerhittingstats_obp_1, @baseball_playerhittingstats_slg_1)
            StringBuilder sb_phs = new StringBuilder();
            var hittingStats = gameData.PlayerStats.Where(p => p.DataType == Shared.Common.PlayerDataType.Hitting).ToArray();
            for (int i = 0; i < hittingStats.Length; i++)
            {
                sb_phs.Append("\t\t(@baseball_playerhittingstats_foreignplayerid_");
                sb_phs.Append(i);
                sb_phs.Append(", @baseball_playerhittingstats_foreignteamid_");
                sb_phs.Append(i);
                sb_phs.Append(", @baseball_playerhittingstats_foreigngameid_");
                sb_phs.Append(i);
                sb_phs.Append(", @baseball_playerhittingstats_ab_");
                sb_phs.Append(i);
                sb_phs.Append(", @baseball_playerhittingstats_r_");
                sb_phs.Append(i);
                sb_phs.Append(", @baseball_playerhittingstats_h_");
                sb_phs.Append(i);
                sb_phs.Append(", @baseball_playerhittingstats_rbi_");
                sb_phs.Append(i);
                sb_phs.Append(", @baseball_playerhittingstats_bb_");
                sb_phs.Append(i);
                sb_phs.Append(", @baseball_playerhittingstats_so_");
                sb_phs.Append(i);
                sb_phs.Append(", @baseball_playerhittingstats_np_");
                sb_phs.Append(i);
                sb_phs.Append(", @baseball_playerhittingstats_avg_");
                sb_phs.Append(i);
                sb_phs.Append(", @baseball_playerhittingstats_obp_");
                sb_phs.Append(i);
                sb_phs.Append(", @baseball_playerhittingstats_slg_");
                sb_phs.Append(i);
                sb_phs.Append(')');
                if (hittingStats.Length > i + 1)
                    sb_phs.AppendLine(",");

                paramList.Add(new ValuePair("@baseball_playerhittingstats_foreignplayerid_" + i, hittingStats[i].ForeignPlayerId));
                paramList.Add(new ValuePair("@baseball_playerhittingstats_foreignteamid_" + i, hittingStats[i].ForeignTeamId));
                paramList.Add(new ValuePair("@baseball_playerhittingstats_foreigngameid_" + i, hittingStats[i].ForeignGameEventId));
                paramList.Add(new ValuePair("@baseball_playerhittingstats_ab_" + i, hittingStats[i].GetData("AB")));
                paramList.Add(new ValuePair("@baseball_playerhittingstats_r_" + i, hittingStats[i].GetData("R")));
                paramList.Add(new ValuePair("@baseball_playerhittingstats_h_" + i, hittingStats[i].GetData("H")));
                paramList.Add(new ValuePair("@baseball_playerhittingstats_rbi_" + i, hittingStats[i].GetData("RBI")));
                paramList.Add(new ValuePair("@baseball_playerhittingstats_bb_" + i, hittingStats[i].GetData("BB")));
                paramList.Add(new ValuePair("@baseball_playerhittingstats_so_" + i, hittingStats[i].GetData("SO")));
                paramList.Add(new ValuePair("@baseball_playerhittingstats_np_" + i, hittingStats[i].GetData("NP")));
                paramList.Add(new ValuePair("@baseball_playerhittingstats_avg_" + i, hittingStats[i].GetData("AVG")));
                paramList.Add(new ValuePair("@baseball_playerhittingstats_obp_" + i, hittingStats[i].GetData("OBP")));
                paramList.Add(new ValuePair("@baseball_playerhittingstats_slg_" + i, hittingStats[i].GetData("SLG")));
            }

            sql = sql.Replace("{BASEBALL_PLAYERHITTINGSTATS}", sb_phs.ToString());

            return sql;
        }

        private string UpdateSqlForTeams(string sql, Shared.Common.GameEvent gameData, List<Shared.Common.ValuePair> paramList)
        {
            // teams
            //(@teams_name_1, @teams_foreignid_1, @teams_sport_1)
            StringBuilder sb_teams = new StringBuilder();
            for (int i = 0; i < gameData.Teams.Length; i++)
            {
                sb_teams.Append("\t\t(@teams_name_");
                sb_teams.Append(i);
                sb_teams.Append(", @teams_foreignid_");
                sb_teams.Append(i);
                sb_teams.Append(", @teams_sport_");
                sb_teams.Append(i);
                sb_teams.Append(')');
                if (gameData.Teams.Length > i + 1)
                    sb_teams.AppendLine(",");

                paramList.Add(new ValuePair("@teams_name_" + i, gameData.Teams[i].Name));
                paramList.Add(new ValuePair("@teams_foreignid_" + i, gameData.Teams[i].ForeignId));
                paramList.Add(new ValuePair("@teams_sport_" + i, gameData.Teams[i].Sport.ToString()));
            }

            sql = sql.Replace("{BASEBALL_TEAMS}", sb_teams.ToString());

            return sql;
        }

        private string UpdateSqlForPlayers(string sql, Shared.Common.GameEvent gameData, List<Shared.Common.ValuePair> paramList)
        {
            // players
            //(@players_name_1, @players_foreignid_1, @players_sport_1)
            StringBuilder sb_players = new StringBuilder();
            for (int i = 0; i < gameData.PlayerStats.Length; i++)
            {
                sb_players.Append("\t\t(@players_name_");
                sb_players.Append(i);
                sb_players.Append(", @players_foreignid_");
                sb_players.Append(i);
                sb_players.Append(", @players_sport_");
                sb_players.Append(i);
                sb_players.Append(')');
                if (gameData.PlayerStats.Length > i + 1)
                    sb_players.AppendLine(",");

                paramList.Add(new ValuePair("@players_name_" + i, gameData.PlayerStats[i].ForeignPlayerName));
                paramList.Add(new ValuePair("@players_foreignid_" + i, gameData.PlayerStats[i].ForeignPlayerId));
                paramList.Add(new ValuePair("@players_sport_" + i, gameData.PlayerStats[i].Sport.ToString()));
            }

            sql = sql.Replace("{BASEBALL_PLAYERS}", sb_players.ToString());

            return sql;
        }

        private string UpdateSqlForGame(string sql, GameEvent gameData, List<ValuePair> paramList)
        {
            // game
            paramList.Add(new ValuePair("@game_foreignid", gameData.ForeignId));
            paramList.Add(new ValuePair("@game_sport", gameData.Sport.ToString()));
            paramList.Add(new ValuePair("@game_date", gameData.Date.ToString()));
            paramList.Add(new ValuePair("@game_attendence", gameData.Attendence.ToString()));
            paramList.Add(new ValuePair("@game_weather_type", gameData.Weather_Type.ToString()));
            paramList.Add(new ValuePair("@game_windspeed", gameData.WindSpeed.ToString()));

            if (gameData.Weather_Degrees == null
                    || !gameData.Weather_Degrees.HasValue)
                sql = sql.Replace("@game_weather_degrees", "null");
            else
                paramList.Add(new ValuePair("@game_weather_degrees", gameData.Weather_Degrees.Value));

            if (string.IsNullOrWhiteSpace(gameData.Stadium))
                sql = sql.Replace("@game_stadium", "null");
            else
                paramList.Add(new ValuePair("@game_stadium", gameData.Stadium));

            if (string.IsNullOrWhiteSpace(gameData.GameNotes))
                sql = sql.Replace("@game_notes", "null");
            else
                paramList.Add(new ValuePair("@game_notes", gameData.GameNotes));

            return sql;
        }
        #endregion

        #region future game writer privates
        private string UpdateSqlForFutureGameTeams(string sql, FutureGameEvent gameData, List<ValuePair> paramList)
        {
            // teams
            //(@teams_name_1, @teams_foreignid_1, @teams_sport_1)
            StringBuilder sb_teams = new StringBuilder();
            for (int i = 0; i < gameData.Teams.Length; i++)
            {
                sb_teams.Append("\t\t(@teams_name_");
                sb_teams.Append(i);
                sb_teams.Append(", @teams_foreignid_");
                sb_teams.Append(i);
                sb_teams.Append(", @teams_sport_");
                sb_teams.Append(i);
                sb_teams.Append(')');
                if (gameData.Teams.Length > i + 1)
                    sb_teams.AppendLine(",");

                paramList.Add(new ValuePair("@teams_name_" + i, gameData.Teams[i].Name));
                paramList.Add(new ValuePair("@teams_foreignid_" + i, gameData.Teams[i].ForeignId));
                paramList.Add(new ValuePair("@teams_sport_" + i, gameData.Teams[i].Sport.ToString()));
            }

            sql = sql.Replace("{BASEBALL_TEAMS}", sb_teams.ToString());

            return sql;
        }

        private string UpdateSqlForFutureGamePlayers(string sql, FutureGameEvent gameData, List<ValuePair> paramList)
        {
            // players
            //(@players_name_1, @players_foreignid_1, @players_sport_1)
            StringBuilder sb_players = new StringBuilder();
            for (int i = 0; i < gameData.StartingPitchers.Length; i++)
            {
                sb_players.Append("\t\t(@players_name_");
                sb_players.Append(i);
                sb_players.Append(", @players_foreignid_");
                sb_players.Append(i);
                sb_players.Append(", @players_sport_");
                sb_players.Append(i);
                sb_players.Append(')');
                if (gameData.StartingPitchers.Length > i + 1)
                    sb_players.AppendLine(",");

                paramList.Add(new ValuePair("@players_name_" + i, gameData.StartingPitchers[i].ForeignPlayerId));
                paramList.Add(new ValuePair("@players_foreignid_" + i, gameData.StartingPitchers[i].ForeignPlayerId));
                paramList.Add(new ValuePair("@players_sport_" + i, gameData.StartingPitchers[i].Sport.ToString()));
            }

            sql = sql.Replace("{BASEBALL_PLAYERS}", sb_players.ToString());

            return sql;
        }

        private string UpdateSqlForFutureGameStartingPitchers(string sql, FutureGameEvent gameData, List<ValuePair> paramList)
        {
            // startingpitchers
            //(@baseball_startingpitchers_playerid_1, @baseball_startingpitchers_gameid_1, @baseball_startingpitchers_teamid_1)
            StringBuilder sb_players = new StringBuilder();
            for (int i = 0; i < gameData.StartingPitchers.Length; i++)
            {
                sb_players.Append("\t\t(@baseball_startingpitchers_playerid_");
                sb_players.Append(i);
                sb_players.Append(", @baseball_startingpitchers_gameid_");
                sb_players.Append(i);
                sb_players.Append(", @baseball_startingpitchers_teamid_");
                sb_players.Append(i);
                sb_players.Append(')');
                if (gameData.StartingPitchers.Length > i + 1)
                    sb_players.AppendLine(",");

                paramList.Add(new ValuePair("@baseball_startingpitchers_playerid_" + i, gameData.StartingPitchers[i].ForeignPlayerId));
                paramList.Add(new ValuePair("@baseball_startingpitchers_gameid_" + i, gameData.StartingPitchers[i].ForeignGameEventId));
                paramList.Add(new ValuePair("@baseball_startingpitchers_teamid_" + i, gameData.StartingPitchers[i].ForeignTeamId));
            }

            sql = sql.Replace("{BASEBALL_STARTINGPITCHERS}", sb_players.ToString());

            return sql;
        }

        private string UpdateSqlForFutureGame(string sql, FutureGameEvent gameData, List<ValuePair> paramList)
        {
            // game
            paramList.Add(new ValuePair("@game_foreignid", gameData.ForeignId));
            paramList.Add(new ValuePair("@game_sport", gameData.Sport.ToString()));
            paramList.Add(new ValuePair("@game_date", gameData.Date.ToString()));

            if (gameData.PrecipitationChance == null
                    || !gameData.PrecipitationChance.HasValue)
                sql = sql.Replace("@game_precipitation", "null");
            else
                paramList.Add(new ValuePair("@game_precipitation", gameData.PrecipitationChance.Value));

            if (gameData.Weather_HighDegrees == null
                    || !gameData.Weather_HighDegrees.HasValue)
                sql = sql.Replace("@game_weather_highdegrees", "null");
            else
                paramList.Add(new ValuePair("@game_weather_highdegrees", gameData.Weather_HighDegrees.Value));

            if (gameData.Weather_LowDegrees == null
                    || !gameData.Weather_LowDegrees.HasValue)
                sql = sql.Replace("@game_weather_lowdegrees", "null");
            else
                paramList.Add(new ValuePair("@game_weather_lowdegrees", gameData.Weather_LowDegrees.Value));

            if (string.IsNullOrWhiteSpace(gameData.Stadium))
                sql = sql.Replace("@game_stadium", "null");
            else
                paramList.Add(new ValuePair("@game_stadium", gameData.Stadium));

            return sql;
        }
        #endregion

        #region fantasy league writer privates
        private string UpdateSqlForFantasyLeagues(string sql, FantasyLeagueEntry[] source, List<ValuePair> paramList)
        {
            if (source != null
                && source.Length > 0)
            {
                StringBuilder ffl = new StringBuilder();
                ffl.AppendLine(@"insert into @fantasyLeagues (ForeignId, ForeignTitle, ForeignSite, Url, BuyIn, PartCount, StartDate, LeagueType, Sport, IsActive, SalaryCap,
                    Starting1B, Starting2B, Starting3B, StartingSS, StartingP, StartingOF, StartingC)
	VALUES 
        ");
                // fantasy leagues
                //(@baseball_fantasyLeague_foreignid_1, @baseball_fantasyLeague_foreigntitle_1, @baseball_fantasyLeague_foreignsite_1, 
                //@baseball_fantasyLeague_url_1, @baseball_fantasyLeague_buyin_1, @baseball_fantasyLeague_partcount_1, @baseball_fantasyLeague_startdate_1, 
                //@baseball_fantasyLeague_leaguetype_1, @baseball_fantasyLeague_sport_1, @baseball_fantasyLeague_isactive_1)
                for (int i = 0; i < source.Length; i++)
                {
                    ffl.Append("(@baseball_fantasyLeague_foreignid_");
                    ffl.Append(i);
                    ffl.Append(", @baseball_fantasyLeague_foreigntitle_");
                    ffl.Append(i);
                    ffl.Append(", @baseball_fantasyLeague_foreignsite_");
                    ffl.Append(i);
                    ffl.Append(", @baseball_fantasyLeague_url_");
                    ffl.Append(i);
                    ffl.Append(", @baseball_fantasyLeague_buyin_");
                    ffl.Append(i);
                    ffl.Append(", @baseball_fantasyLeague_partcount_");
                    ffl.Append(i);
                    ffl.Append(", @baseball_fantasyLeague_startdate_");
                    ffl.Append(i);
                    ffl.Append(", @baseball_fantasyLeague_leaguetype_");
                    ffl.Append(i);
                    ffl.Append(", @baseball_fantasyLeague_sport_");
                    ffl.Append(i);
                    ffl.Append(", @baseball_fantasyLeague_isactive_");
                    ffl.Append(i);
                    ffl.Append(", @baseball_fantasyLeague_SalaryCap_");
                    ffl.Append(i);

                    ffl.Append(", @baseball_fantasyLeague_1B_");
                    ffl.Append(i);
                    ffl.Append(", @baseball_fantasyLeague_2B_");
                    ffl.Append(i);
                    ffl.Append(", @baseball_fantasyLeague_3B_");
                    ffl.Append(i);
                    ffl.Append(", @baseball_fantasyLeague_SS_");
                    ffl.Append(i);
                    ffl.Append(", @baseball_fantasyLeague_P_");
                    ffl.Append(i);
                    ffl.Append(", @baseball_fantasyLeague_OF_");
                    ffl.Append(i);
                    ffl.Append(", @baseball_fantasyLeague_C_");
                    ffl.Append(i);
                    ffl.Append(')');
                    if (source.Length > i + 1)
                        ffl.AppendLine(",");

                    paramList.Add(new ValuePair("@baseball_fantasyLeague_foreignid_" + i, source[i].ForeignId));
                    paramList.Add(new ValuePair("@baseball_fantasyLeague_foreigntitle_" + i, source[i].ForeignTitle));
                    paramList.Add(new ValuePair("@baseball_fantasyLeague_foreignsite_" + i, source[i].ForeignSite));
                    paramList.Add(new ValuePair("@baseball_fantasyLeague_url_" + i, source[i].Url));
                    paramList.Add(new ValuePair("@baseball_fantasyLeague_buyin_" + i, source[i].BuyIn));
                    paramList.Add(new ValuePair("@baseball_fantasyLeague_partcount_" + i, source[i].ParticipantCount));
                    paramList.Add(new ValuePair("@baseball_fantasyLeague_startdate_" + i, source[i].StartDate));
                    paramList.Add(new ValuePair("@baseball_fantasyLeague_leaguetype_" + i, source[i].LeagueType.ToString()));
                    paramList.Add(new ValuePair("@baseball_fantasyLeague_sport_" + i, source[i].Sport.ToString()));
                    paramList.Add(new ValuePair("@baseball_fantasyLeague_isactive_" + i, source[i].IsActive));
                    paramList.Add(new ValuePair("@baseball_fantasyLeague_SalaryCap_" + i, source[i].SalaryCap));

                    paramList.Add(new ValuePair("@baseball_fantasyLeague_1B_" + i, source[i].Starting1B));
                    paramList.Add(new ValuePair("@baseball_fantasyLeague_2B_" + i, source[i].Starting2B));
                    paramList.Add(new ValuePair("@baseball_fantasyLeague_3B_" + i, source[i].Starting3B));
                    paramList.Add(new ValuePair("@baseball_fantasyLeague_SS_" + i, source[i].StartingSS));
                    paramList.Add(new ValuePair("@baseball_fantasyLeague_P_" + i, source[i].StartingP));
                    paramList.Add(new ValuePair("@baseball_fantasyLeague_OF_" + i, source[i].StartingOF));
                    paramList.Add(new ValuePair("@baseball_fantasyLeague_C_" + i, source[i].StartingC));
                }

                sql = sql.Replace("{FANTASY_LEAGUES}", ffl.ToString());
                //string tempData = "('ricky-nolasco', 'Minnesota Twins', '340615106', 5.1, 9, 3, 3, 2, 5, 0, 5.66, 103, 62)";
                //sql = sql.Replace("{FANTASY_LEAGUES}", tempData);

            }
            return sql;
        }
        #endregion
    }
}
