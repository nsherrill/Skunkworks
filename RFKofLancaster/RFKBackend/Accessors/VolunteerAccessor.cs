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
        void UpdateVolunteer(Volunteer volunteerToSave);

        RoleCountModel[] GetRoleCounts(int year);
        void AdjustRoleCount(int year, int roleId, int delta);
        RoleModel[] GetAllRoles();
        void AddRoleToUser(int volunteerId, int roleId, int year);
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

        public void UpdateVolunteer(Volunteer volunteerToSave)
        {
            var paramList = new SqlParameter[]
            {
                new SqlParameter("@VolunteerId", volunteerToSave.VolunteerId)
                , new SqlParameter("@Name", volunteerToSave.Name)
                , new SqlParameter("@NickName", volunteerToSave.NickName)
                , new SqlParameter("@Gender", volunteerToSave.Gender)
                , new SqlParameter("@Notes", volunteerToSave.Notes?? "")
            };

            string mergeText = base.GetBasicMergeText(paramList);

            base.ExecuteScalar(mergeText, paramList);
        }

        public RoleCountModel[] GetRoleCounts(int year)
        {
            string sqlString = $@"
                select r.RoleId, r.name as DisplayName, count(vry.roleid) as [Count]
                    from dbo.Roles r
                        left join dbo.VolunteerRoleYear vry on vry.RoleId = r.RoleId and vry.year = {year}
                    group by r.roleid, r.name, r.[index]
                    order by r.[index]";

            var result = base.ExecuteReader<RoleCountModel>(sqlString, (rdr) =>
            {
                return new RoleCountModel((int)rdr["RoleId"], (string)rdr["DisplayName"], (int)rdr["Count"]);
            });

            return result.ToArray();
        }

        public void AdjustRoleCount(int year, int roleId, int delta)
        {
            if (delta > 0)
            {
                string sqlText = $@"
                    insert into dbo.VolunteerRoleYear(VolunteerId, RoleId, [Year])
                        values(null, {roleId}, {year})";

                for (int i = 0; i < delta; i++)
                {
                    base.ExecuteScalar(sqlText);
                }
            }
            else
            {
                string sqlText = $@" select count(*) from dbo.VolunteerRoleYear where RoleId = {roleId} and [Year] = {year} and VolunteerId is null ";
                int originalCount = (int)base.ExecuteScalar(sqlText);

                if (originalCount > 0)
                {
                    sqlText = $@"delete from dbo.volunteerroleyear where roleid = {roleId} and [year] = {year} and volunteerid is null";
                    base.ExecuteScalar(sqlText);

                    if (originalCount > 1)
                        AdjustRoleCount(year, roleId, originalCount - 1);
                }
            }
        }

        public RoleModel[] GetAllRoles()
        {
            string sqlString = $@"
                select r.RoleId, r.name as DisplayName
                    from dbo.Roles r
                    order by r.[name]";

            var result = base.ExecuteReader<RoleModel>(sqlString, (rdr) =>
            {
                return new RoleModel((int)rdr["RoleId"], (string)rdr["DisplayName"]);
            });

            return result.ToArray();
        }

        public void AddRoleToUser(int volunteerId, int roleId, int year)
        {
            string sqlString = $@"if not exists 
                (select * 
                    from [dbo].[VolunteerRoleYear] 
                    where volunteerId = {volunteerId}
                        and roleId = {roleId}
                        and year = {year})
                begin
                    insert into [dbo].[VolunteerRoleYear] (volunteerid, roleid, year)
                        select {volunteerId}, {roleId}, {year}
                end";

            base.ExecuteScalar(sqlString);
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
