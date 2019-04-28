using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Shared.DataContracts.Account
{
    public class UserModel
    {
        public UserModel() { }
        public UserModel(string email) : this()
        {
            this.Email = email;
        }

        public int UserId { get; set; }
        public string Email { get; set; }
        public bool CanLogin { get; set; } = false;
        public bool CanWrite { get; set; } = false;
        public bool CanRead { get; set; } = false;
    }
}
