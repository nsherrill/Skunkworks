using DigitalBoardGamer.Shared.SettlersShared;
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
        public BoardDefinition GetBoardDefinition(long boardId)
        {
            BoardDefinition result = null;
            string sql = string.Format("execute settlers.GetBoardDefinition {0}", boardId);

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
                            hexDef.TypeId = (long)rdr["HexTypeId"];

                            if (!string.IsNullOrEmpty(hexDef.BackupColor)
                                && hexDef.BackupColor[0] != '#')
                                hexDef.BackupColor = "#" + hexDef.BackupColor;

                            hexes.Add(hexDef);
                        }
                        result.HexBoardDefinition = hexes.ToArray();

                        rdr.NextResult();
                        List<ValueBoardDefinition> vals = new List<ValueBoardDefinition>();
                        while (rdr.Read())
                        {// value things
                            var valDef = new ValueBoardDefinition();
                            valDef.ValCount = (int)rdr["MaxValueCount"];
                            valDef.DiceValue = rdr["DiceValue"].ToString();
                            valDef.HexValueId = (long)rdr["HexValueId"];
                            vals.Add(valDef);
                        }
                        result.ValueBoardDefinition = vals.ToArray();

                        rdr.NextResult();
                        List<HexDefinition> staticHexes = new List<HexDefinition>();
                        while (rdr.Read())
                        {// static things
                            var hexDef = new HexDefinition();
                            hexDef.RowIndex = (int)rdr["RowIndex"];
                            hexDef.ColumnIndex = (int)rdr["ColIndex"];

                            var newHex = new HexType()
                            {
                                TypeId = (long)rdr["HexTypeId"],
                                Name = (string)rdr["Name"],
                                ImageUrl = rdr["ImageUrl"] == DBNull.Value ? null : (string)rdr["ImageUrl"],
                                BackupColor = rdr["BackupColor"] == DBNull.Value ? null : (string)rdr["BackupColor"],
                            };

                            if (!string.IsNullOrEmpty(newHex.BackupColor)
                                && newHex.BackupColor[0] != '#')
                                newHex.BackupColor = "#" + newHex.BackupColor;

                            hexDef.MyHexType = newHex;

                            var newVal = new HexValue()
                            {
                                DiceValue = rdr["DiceValue"].ToString(),
                                HexValueId = (long)rdr["HexValueId"],
                            };
                            hexDef.MyHexValue = newVal;

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
