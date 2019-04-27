using RFKBackend.Accessors;
using RFKBackend.Engines.Validators;
using RFKBackend.Shared;
using RFKBackend.Shared.Contracts;
using RFKBackend.Shared.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Managers
{
    public class VolunteerManager : IVolunteerManager
    {
        IVolunteerAccessor volAcc = new VolunteerAccessor();
        IValidationEngine<Volunteer> validationEng = new VolunteerValidationEngine();

        public Volunteer[] FindAllVolunteers()
        {
            return volAcc.FindAllVolunteers();
        }

        public VolunteerSnapshot FindVolunteer(int volunteerId)
        {
            var result = volAcc.FindVolunteer(volunteerId);
            return result;
        }

        public Volunteer CreateNewVolunteer()
        {
            var result = new Volunteer()
            {
            };

            return result;
        }

        public void ToggleVerbal(int id, bool shouldBeOn, int year)
        {
            volAcc.ToggleBool(VolunteerToggleType.HasVerbalCommit, id, shouldBeOn, year);
        }

        public VolunteerResult SaveVolunteer(Volunteer volToSave)
        {
            var result = new VolunteerResult();
            try
            {
                var valResult = validationEng.Validate(volToSave);
                if (valResult.IsSuccess)
                {
                    volAcc.UpdateVolunteer(valResult.ValidatedData);
                    result.SetSuccess(valResult.ValidatedData);
                }
                else
                {
                    result.SetError(Messages.ValidateVolunteer_Fail(valResult.ValidationErrors?.FirstOrDefault()));
                }
            }
            catch (Exception e)
            {
                result.SetError(Messages.SaveVolunteer_Fail(volToSave?.VolunteerId.ToString()), e);
            }
            return result;
        }

        public void ToggleApplication(int id, bool shouldBeOn, int year)
        {
            volAcc.ToggleBool(VolunteerToggleType.HasApplication, id, shouldBeOn, year);
        }

        public RoleCountModel[] GetRoleCounts(int thisYear)
        {
            var result = volAcc.GetRoleCounts(thisYear);
            return result;
        }

        public void AdjustRoleCount(int year, int roleId, int delta)
        {
            volAcc.AdjustRoleCount(year, roleId, delta);
        }

        public RoleModel[] GetAllRoles()
        {
            return volAcc.GetAllRoles();
        }

        public void AddRoleToUser(int volunteerId, int roleId, int year)
        {
            volAcc.AddRoleToUser(volunteerId, roleId, year);
            AdjustRoleCount(year, roleId, -1);
        }
    }
}
