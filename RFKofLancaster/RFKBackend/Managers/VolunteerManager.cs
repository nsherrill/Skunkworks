using RFKBackend.Accessors;
using RFKBackend.Shared;
using RFKBackend.Shared.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Managers
{
    public class VolunteerManager
    {
        IVolunteerAccessor volAcc = new VolunteerAccessor();

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

        public void ToggleApplication(int id, bool shouldBeOn, int year)
        {
            volAcc.ToggleBool(VolunteerToggleType.HasApplication, id, shouldBeOn, year);
        }
    }
}
