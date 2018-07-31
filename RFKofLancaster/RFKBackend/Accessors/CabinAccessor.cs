using RFKBackend.Shared.DataContracts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Accessors
{
    public class CabinAccessor : BaseSqlAccessor
    {
        public Cabin[] FindAllCabins()
        {
            string sqlString = @"
    select * from dbo.Cabins order by name";
            var result = base.ExecuteReader<Cabin>(sqlString, CabinReader);

            return result;
        }

        public Cabin FindCabin(int cabinId)
        {
            string sqlString = @"select * from dbo.Cabins where Cabinid = @id";
            var result = base.ExecuteReader<Cabin>(sqlString, CabinReader
                , new SqlParameter[] { new SqlParameter("@id", cabinId) }).FirstOrDefault();

            return result;
        }

        #region privates
        private Cabin CabinReader(SqlDataReader rdr)
        {
            Cabin result = new Cabin()
            {
                CabinId = Convert.ToInt32(rdr["CabinId"]),
                Name = rdr["Name"].ToString(),
                NickName = rdr["NickName"].ToString(),
                Notes = rdr["Notes"] as string,
                CamperCapacity = Convert.ToInt32(rdr["CamperCapacity"]),
                AdultCapacity = Convert.ToInt32(rdr["AdultCapacity"]),
            };
            return result;
        }
        #endregion
    }
}
