using RFKBackend.Accessors;
using RFKBackend.Shared.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Managers
{
    public class CabinManager
    {
        CabinAccessor cabAcc = new CabinAccessor();

        public Cabin[] FindAllCabins()
        {
            return cabAcc.FindAllCabins();
        }

        public Cabin FindCabin(int CabinId)
        {
            var result = cabAcc.FindCabin(CabinId);
            return result;
        }

        public Cabin CreateNewCabin()
        {
            var result = new Cabin()
            {
            };

            return result;
        }
    }
}
