using RFKBackend.Shared;
using RFKBackend.Shared.DataContracts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Accessors
{
    public interface IVolunteerAccessor
    {
        Volunteer[] FindAllVolunteers();
        VolunteerSnapshot FindVolunteer(int volunteerId);
        void ToggleBool(VolunteerToggleType hasApplication, int id, bool shouldBeOn, int year);
    }

    public class VolunteerAccessor : BaseSqlAccessor, IVolunteerAccessor
    {
        internal override string PRIMARY_KEY_NAME => "[VolunteerId]";
        internal override string TABLE_NAME => "[dbo].[Volunteers]";

        public Volunteer[] FindAllVolunteers()
        {
            string sqlString = base.GetBasicFindAllSql("Name");
            var result = base.ExecuteReader<Volunteer>(sqlString, VolunteerReader);

            return result;
        }

        public VolunteerSnapshot FindVolunteer(int volunteerId)
        {
            string sqlString = base.GetBasicFindSql("@id");
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

        public void ToggleBool(VolunteerToggleType toggleType, int id, bool shouldBeOn, int year)
        {
            if (toggleType == VolunteerToggleType.HasApplication || toggleType == VolunteerToggleType.HasVerbalCommit)
            {
                var paramList = new SqlParameter[]
                {
                    new SqlParameter("@VolunteerId", id)
                    , new SqlParameter($"@{toggleType.ToString()}", shouldBeOn)
                    , new SqlParameter("@Year", year)
                };

                string mergeText = base.GetBasicMergeText(paramList, "and target.year = source.year", "[dbo].[VolunteerYearStatus]");

                base.ExecuteScalar(mergeText, paramList);


                base.ToggleBool(toggleType.ToString(), id, shouldBeOn, $"and Year = {year}", "[dbo].[VolunteerYearStatus]");
            }
            else
            {
                throw new NotImplementedException();
            }
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
