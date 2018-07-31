using RFKBackend.Accessors;
using RFKBackend.Shared.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Managers
{
    public class CamperManager
    {
        CamperAccessor cabAcc = new CamperAccessor();

        public Camper[] FindAllCampers()
        {
            return cabAcc.FindAllCampers();
        }

        public CamperSnapshot FindCamper(int camperId)
        {
            var result = cabAcc.FindCamper(camperId);
            return result;
        }

        public Camper CreateNewCamper()
        {
            var result = new Camper()
            {
            };

            return result;
        }
    }
}
