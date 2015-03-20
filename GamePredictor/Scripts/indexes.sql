
CREATE INDEX StartingPitchers_playerid_idx
ON baseball.StartingPitchers ([PlayerId])
CREATE INDEX StartingPitchers_teamid_idx
ON baseball.StartingPitchers ([TeamId])
CREATE INDEX StartingPitchers_gameid_idx
ON baseball.StartingPitchers ([GameId])
GO

CREATE UNIQUE INDEX StartingPitchers_idx
ON baseball.StartingPitchers ([PlayerId], [TeamId], [GameId])
GO

CREATE INDEX PlayerPitchingStats_playerid_idx
ON baseball.PlayerPitchingStats ([PlayerId])
CREATE INDEX PlayerPitchingStats_teamid_idx
ON baseball.PlayerPitchingStats ([TeamId])
CREATE INDEX PlayerPitchingStats_gameid_idx
ON baseball.PlayerPitchingStats ([GameId])
GO

CREATE UNIQUE INDEX PlayerPitchingStats_idx
ON baseball.PlayerPitchingStats ([PlayerId], [TeamId], [GameId])
GO

CREATE INDEX PlayerHittingStats_playerid_idx
ON baseball.PlayerHittingStats ([PlayerId])
CREATE INDEX PlayerHittingStats_teamid_idx
ON baseball.PlayerHittingStats ([TeamId])
CREATE INDEX PlayerHittingStats_gameid_idx
ON baseball.PlayerHittingStats ([GameId])

CREATE UNIQUE INDEX PlayerHittingStats_idx
ON baseball.PlayerHittingStats ([PlayerId], [TeamId], [GameId])
GO

CREATE INDEX fantasyplayers_spacedname_idx
ON FantasyPlayers (spacedname)
CREATE UNIQUE INDEX fantasyplayers_idx
ON FantasyPlayers (spacedname, ForeignLeagueId)

CREATE UNIQUE INDEX fantasyleagues_foreignid_idx
ON fantasyleagues (foreignid)

CREATE UNIQUE INDEX players_id_idx
ON Players (id)

CREATE INDEX players_name_idx
ON Players (name)

CREATE UNIQUE INDEX teams_id_idx
ON teams (id)

CREATE UNIQUE INDEX gameevents_foreignid_idx
ON gameevents (foreignid)

CREATE UNIQUE INDEX gameevents_id_idx
ON gameevents (id)



