using RFKBackend.Shared.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Shared.Contracts
{
    public interface IVolunteerManager
    {
        Volunteer[] FindAllVolunteers();
        VolunteerSnapshot FindVolunteer(int volunteerId);

        Volunteer CreateNewVolunteer();
        VolunteerResult SaveVolunteer(Volunteer volToSave);

        void ToggleVerbal(int id, bool shouldBeOn, int year);
        void ToggleApplication(int id, bool shouldBeOn, int year);

        RoleCountModel[] GetRoleCounts(int thisYear);
        void AdjustRoleCount(int year, int roleId, int delta);
        RoleModel[] GetAllRoles();
        void AddRoleToUser(int volunteerId, int roleId, int year);
    }
}
