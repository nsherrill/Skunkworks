﻿using DigitalBoardGamer.Shared.SettlersShared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBoardGamer.ResourceAccessor.SettlersAccessor
{
    public class BoardDefinitionAccessor : IBoardDefinitionAccessor
    {
        public BoardDefinition GetBoardDefinition(long boardId, int playerCount)
        {
            BoardDefinition result = null;
            string sql = string.Format("execute settlers.GetBoardDefinition {0}, {1}", boardId, playerCount);

            using (SqlConnection conn = new SqlConnection(@"Server=.\SqlExpress;Database=DigitalBoardGamer;Trusted_Connection=true"))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    var rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {// board things
                        result = new BoardDefinition();
                        result.RowCount = (int)rdr["RowCount"];
                        result.ColumnCount = (int)rdr["ColumnCount"];

                        rdr.NextResult();
                        List<HexBoardDefinition> hexes = new List<HexBoardDefinition>();
                        while (rdr.Read())
                        {// hex things
                            var hexDef = new HexBoardDefinition();
                            hexDef.MaxHexCount = (int)rdr["MaxHexCount"];
                            hexDef.Name = (string)rdr["Name"];
                            hexDef.ImageUrl = rdr["ImageUrl"] == DBNull.Value ? null : (string)rdr["ImageUrl"];
                            hexDef.BackupColor = rdr["BackupColor"] == DBNull.Value ? null : (string)rdr["BackupColor"];
                            hexDef.HexTypeId = (long)rdr["HexTypeId"];
                            hexes.Add(hexDef);
                        }
                        result.HexDefinition = hexes.ToArray();

                        rdr.NextResult();
                        List<ValueBoardDefinition> vals = new List<ValueBoardDefinition>();
                        while (rdr.Read())
                        {// value things
                            var valDef = new ValueBoardDefinition();
                            valDef.ValCount = (int)rdr["MaxValueCount"];
                            valDef.DiceValue = (int)rdr["DiceValue"];
                            valDef.HexValueId = (long)rdr["HexValueId"];
                            vals.Add(valDef);
                        }
                        result.ValueDefinition = vals.ToArray();

                        rdr.NextResult();
                        List<HexDefinition> staticHexes = new List<HexDefinition>();
                        while (rdr.Read())
                        {// static things
                            var hexDef = new HexDefinition();
                            hexDef.RowIndex = (int)rdr["RowIndex"];
                            hexDef.ColumnIndex = (int)rdr["ColumnIndex"];
                            hexDef.HexTypeId = (long)rdr["HexTypeId"];
                            hexDef.HexValueId = (long)rdr["ValueId"];
                            staticHexes.Add(hexDef);
                        }
                        result.StaticHexes = staticHexes.ToArray();
                    }
                }
            }

            return result;
        }
    }
}
