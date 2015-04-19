using GP.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Accessors.RemoteBaseballDataAccessor
{
    public interface IRemoteBaseballDataAcc
    {
        #region deprecated
        MassData deprecated_FindAllDataForDate(DateTime desiredDate);

        GameEvent deprecated_FindGameDataFromContents(string gameId, string pageContents);

        MassData deprecated_FindAllGamesDataFromContents(string pageContents);

        FutureGameEvent[] deprecated_FindFutureGamesForDate(DateTime desiredDate);

        FutureGameEvent deprecated_FindFutureGameDataFromContents(string gameId, string pageContents);

        FutureGameEvent[] deprecated_FindAllFutureGamesDataFromContents(string pageContents);
        #endregion

        CurrentPlayerStats[] GetCurrentPlayerHittingStats();

        CurrentPlayerStats[] GetCurrentPlayerPitchingStats();

    }

    public interface IRemoteBaseballDataParser
    {
        PlayerEventStats ParsePlayerEventStats(string gameId, string teamId, string[] data, string[] headers);

        CurrentPlayerStats ParseCurrentPlayerStats(string content);

    }
}
