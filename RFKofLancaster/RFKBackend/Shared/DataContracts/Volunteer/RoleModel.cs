using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Shared.DataContracts
{
    public class RoleModel
    {
        public string DisplayName { get; set; }
        public int RoleId { get; set; }

        public RoleModel(int roleId, string displayName)
        {
            this.DisplayName = displayName;
            this.RoleId = roleId;
        }
    }

    public class RoleCountModel : RoleModel
    {
        public int Count { get; set; }
        public RoleCountModel(int roleId, string displayName, int count)
            : base(roleId, displayName)
        {
            this.Count = count;
        }
    }
}
