using RFKBackend.Accessors;
using RFKBackend.Engines;
using RFKBackend.Engines.Validators;
using RFKBackend.Shared;
using RFKBackend.Shared.Contracts;
using RFKBackend.Shared.DataContracts;
using RFKBackend.Shared.DataContracts.Account;
using RFKBackend.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Managers
{
    public class AccountManager : IAccountManager
    {
        private IAccountAccessor accountAcc { get; set; }
        private IPermissionsEngine permissionEng { get; set; }

        public AccountManager()
        {
            this.accountAcc = new AccountAccessor();
            this.permissionEng = new PermissionsEngine();
        }

        public bool ValidateUser(string email, AccessType accessType)
        {
            var user = accountAcc.GetUserByEmail(email);

            var doesUserHavePermission = permissionEng.ValidateUser(user, accessType);
            return doesUserHavePermission;
        }

        public void CreateUser(UserModel newUser)
        {
            accountAcc.CreateUser(newUser);
        }

        public UserModel GetUser(string email)
        {
            return accountAcc.GetUserByEmail(email);
        }
    }
}
