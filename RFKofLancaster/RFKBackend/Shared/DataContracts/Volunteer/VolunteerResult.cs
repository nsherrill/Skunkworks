using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Shared.DataContracts
{
    public class VolunteerResult : DataResult<Volunteer>
    {
    }
    public class VolunteersResult : DataResult<Volunteer[]>
    {
    }
}
