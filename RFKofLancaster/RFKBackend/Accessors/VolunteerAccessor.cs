using RFKBackend.Shared.DataContracts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Accessors
{
    public class VolunteerAccessor : BaseSqlAccessor
    {
        public Volunteer[] FindAllVolunteers()
        {
            string sqlString = @"
    select * from dbo.volunteers order by name";
            var result = base.ExecuteReader<Volunteer>(sqlString, VolunteerReader);

            return result;
        }

        public VolunteerSnapshot FindVolunteer(int volunteerId)
        {
            string sqlString = @"select * from dbo.volunteers where volunteerid = @id";
            var vol = base.ExecuteReader<Volunteer>(sqlString, VolunteerReader
                , new SqlParameter[] { new SqlParameter("@id", volunteerId) }).FirstOrDefault();

            sqlString = @"select dbo.CalculateYearsVolunteered(@id, @year) as YearCount";
            var yearCount = base.ExecuteReader<int>(sqlString, base.IntReader
                , new SqlParameter[] { new SqlParameter("@id", volunteerId), new SqlParameter("@year", DateTime.Now.Year) }).FirstOrDefault();


            var result = new VolunteerSnapshot()
            {
                Volunteer = vol,
                YearsVolunteered = yearCount
            };

            return result;
        }

        #region privates
        private Volunteer VolunteerReader(SqlDataReader rdr)
        {
            Volunteer result = new Volunteer()
            {
                VolunteerId = Convert.ToInt32(rdr["VolunteerId"]),
                Name = rdr["Name"].ToString(),
                NickName = rdr["NickName"].ToString(),
                Gender = rdr["Gender"].ToString(),
                Notes = rdr["Notes"] as string,
            };
            return result;
        }
        #endregion
    }
}
