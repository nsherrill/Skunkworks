using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RFKBackend.Shared.DataContracts.Account;
using RFKBackend.Shared.Enums;

namespace RFKBackend.Engines
{
    public interface IPermissionsEngine
    {
        bool ValidateUser(UserModel user, AccessType accessType);
    }

    public class PermissionsEngine : IPermissionsEngine
    {
        public bool ValidateUser(UserModel user, AccessType accessType)
        {
            if (user == null)
                return false;

            switch (accessType)
            {
                case AccessType.Login:
                    return user.CanLogin;
                case AccessType.Read:
                    return user.CanRead;
                case AccessType.Write:
                    return user.CanWrite;
                default:
                    break;
            }

            return false;
        }
    }
}
