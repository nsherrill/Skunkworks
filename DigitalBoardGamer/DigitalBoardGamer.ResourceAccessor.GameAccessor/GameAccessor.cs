using DigitalBoardGamer.Shared.SharedContracts;
using DigitalBoardGamer.Shared.SharedContracts.Contracts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBoardGamer.ResourceAccessor.GameAccessor
{
    public class GameAccessor : IGameAccessor, IBoardAccessor
    {
        public Game[] GetAllGames()
        {
            List<Game> result = new List<Game>();
            string sql = string.Format("select * from dbo.games");

            using (SqlConnection conn = new SqlConnection(@"Server=.\SqlExpress;Database=DigitalBoardGamer;Trusted_Connection=true"))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    var rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {// board things
                        var gameId = (long)rdr["GameId"];
                        var gameName = (string)rdr["Name"];
                        var gameDllName = (string)rdr["ManagerDllName"];
                        result.Add(new Game(gameId, gameName, gameDllName));
                    }
                }
            }
            return result.ToArray();
        }
        public BoardOption[] GetAllBoardOptions(long gameId)
        {
            List<BoardOption> result = new List<BoardOption>();
            string sql = string.Format("select * from dbo.boards where gameid = {0}", gameId);

            using (SqlConnection conn = new SqlConnection(@"Server=.\SqlExpress;Database=DigitalBoardGamer;Trusted_Connection=true"))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    var rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {// board things
                        var boardId = (long)rdr["BoardId"];
                        var boardOptionName = (string)rdr["Name"];
                        result.Add(new BoardOption(boardId, boardOptionName));
                    }
                }
            }
            return result.ToArray();
        }
    }
}
