using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Dashboard.Models
{
    [Table("Volunteers")]
    public class VolunteerModel
    {
        [Required]
        [Key]
        [Display(Name = "Id")]
        public int VolunteerId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "NickName")]
        public string NickName { get; set; }
    }
    public class CounselorModel : VolunteerModel
    {
    }
}