using RFKBackend.Shared.DataContracts;
using RFKBackend.Shared.DataContracts.Account;
using RFKBackend.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Shared.Contracts
{
    public interface IAccountManager
    {
        bool ValidateUser(string email, AccessType accessType);
        void CreateUser(UserModel userModel);
        UserModel GetUser(string email);
    }
}
