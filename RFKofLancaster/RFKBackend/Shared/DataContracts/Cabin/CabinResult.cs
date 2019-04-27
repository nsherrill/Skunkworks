using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Shared.DataContracts
{
    public class CabinResult : DataResult<Cabin>
    {
    }
    public class CabinsResult : DataResult<Cabin[]>
    {
    }
}
