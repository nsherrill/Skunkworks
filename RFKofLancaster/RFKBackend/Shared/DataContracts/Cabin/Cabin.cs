using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Shared.DataContracts
{
    [Table("Cabins")]
    public class Cabin
    {
        [Required]
        [Key]
        [Display(Name = "Id")]
        public int CabinId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "NickName")]
        public string NickName { get; set; }

        [Required]
        [Display(Name = "CamperCapacity")]
        public int CamperCapacity { get; set; }

        [Required]
        [Display(Name = "AdultCapacity")]
        public int AdultCapacity { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }
    }
}
