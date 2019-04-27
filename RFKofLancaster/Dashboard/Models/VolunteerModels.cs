using RFKBackend.Shared.DataContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Dashboard.Models
{
    public class VolunteerModel : Volunteer
    {
        public VolunteerModel() { }
        public VolunteerModel(Volunteer source)
        {
            this.VolunteerId = source.VolunteerId;
            this.Name = source.Name;
            this.NickName = source.NickName;
            this.Gender = source.Gender;
            this.Notes = source.Notes;
        }
    }
    public class CounselorModel : VolunteerModel
    {
        public CounselorModel() { }
        public CounselorModel(Volunteer source)
            : base(source)
        {
        }
    }
}