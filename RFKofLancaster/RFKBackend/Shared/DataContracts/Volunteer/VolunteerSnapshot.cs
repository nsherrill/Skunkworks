using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Shared.DataContracts
{
    public class VolunteerSnapshot
    {
        public Volunteer Volunteer { get; set; }

        public int YearsVolunteered { get; set; }

        public KeyValuePair<string, string> RoleHistory { get; set; }

        public KeyValuePair<int, string> RecentRoles { get; set; }
    }
}
