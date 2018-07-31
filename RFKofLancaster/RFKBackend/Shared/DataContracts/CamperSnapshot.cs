using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Shared.DataContracts
{
    public class CamperSnapshot
    {
        public Camper Camper { get; set; }

        public int YearsAtCamp { get; set; }

        public string Mentor { get; set; }

        public KeyValuePair<int, string> RecentCounselors { get; set; }

        public KeyValuePair<int, string> RecentBuddyCounselors { get; set; }
    }
}
