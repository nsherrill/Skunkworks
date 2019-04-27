using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Shared.DataContracts
{
    public class Camper
    {
        [Required]
        [Key]
        [Display(Name = "Id")]
        public int CamperId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "BirthYear")]
        public int BirthYear { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; }
    }
}
