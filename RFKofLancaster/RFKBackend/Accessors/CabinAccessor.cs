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
        internal override string PRIMARY_KEY_NAME => "CabinId";
        internal override string TABLE_NAME => "[dbo].Cabins";

        public Cabin[] FindAllCabins()
        {
            string sqlString = base.GetBasicFindAllSql("Name");
            var result = base.ExecuteReader<Cabin>(sqlString, CabinReader);

            return result;
        }

        public Cabin FindCabin(int cabinId)
        {
            string sqlString = base.GetBasicFindSql("@id");
            var result = base.ExecuteReader<Cabin>(sqlString, CabinReader
                , new SqlParameter[] { new SqlParameter("@id", cabinId) }).FirstOrDefault();

            return result;
        }

        internal Cabin SaveCabin(Cabin source)
        {
            string sqlString = @"
                merge into dbo.Cabins as target
                    using (select @id as id, @name as [name], @nickname as nickname, @camperCapacity as campercapacity
                        , @adultCapacity as adultCapacity, @note as notes) as source
                    on (target.cabinid = source.id and source.id > 0)
                    when matched then update set 
                        [name] = source.[name],
                        nickname = source.nickname,
                        campercapacity = source.campercapacity,
                        adultcapacity = source.adultcapacity,
                        notes = source.notes
                    when not matched then insert ([name], nickname, campercapacity, adultcapacity, notes)
                        values (source.[name], source.nickname, source.campercapacity, source.adultcapacity, source.notes);
                select top 1 @id = cabinid from dbo.cabins order by cabinid desc
        
                select * from dbo.Cabins where Cabinid = @id";

            SqlParameter[] paramsList = new SqlParameter[]
            {
                    new SqlParameter("@id", source.CabinId)
                    , new SqlParameter("@name", source.Name)
                    , new SqlParameter("@nickname", source.NickName)
                    , new SqlParameter("@camperCapacity", source.CamperCapacity)
                    , new SqlParameter("@adultCapacity", source.AdultCapacity)
                    , new SqlParameter("@note", source.Notes ?? "")
            };

            var result = base.ExecuteReader<Cabin>(sqlString, CabinReader
                , paramsList).FirstOrDefault();

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
