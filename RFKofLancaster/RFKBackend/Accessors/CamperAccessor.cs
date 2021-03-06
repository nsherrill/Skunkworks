﻿using RFKBackend.Shared.DataContracts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Accessors
{
    public class CamperAccessor : BaseSqlAccessor
    {
        internal override string PRIMARY_KEY_NAME => "CamperId";
        internal override string TABLE_NAME => "[dbo].Campers";

        public Camper[] FindAllCampers()
        {
            string sqlString = base.GetBasicFindAllSql("name");
            var result = base.ExecuteReader<Camper>(sqlString, CamperReader);

            return result;
        }

        public CamperSnapshot FindCamper(int camperId)
        {
            string sqlString = base.GetBasicFindSql("@id");
            var result = base.ExecuteReader<Camper>(sqlString, CamperReader
                , new SqlParameter[] { new SqlParameter("@id", camperId) }).FirstOrDefault();

            var camperSnap = new CamperSnapshot()
            {
                Camper = result,
                YearsAtCamp = 0,
            };

            return camperSnap;
        }

        #region privates
        private Camper CamperReader(SqlDataReader rdr)
        {
            Camper result = new Camper()
            {
                CamperId = Convert.ToInt32(rdr["CamperId"]),
                Name = rdr["Name"].ToString(),
                Gender = rdr["Gender"].ToString(),
            };
            return result;
        }
        #endregion
    }
}
