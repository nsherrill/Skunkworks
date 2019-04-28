using RFKBackend.Managers;
using RFKBackend.Shared.Contracts;
using RFKBackend.Shared.DataContracts.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class RFKController : Controller
    {
        public UserModel MyUser
        {
            get { return (UserModel)base.TempData["MyUser"]; }
            set { base.TempData["MyUser"] = value; }
        }

        private IAccountManager accountMgr { get; set; }

        public RFKController()
        {
            this.accountMgr = new AccountManager();
            TempData.Keep();
        }

        internal void SetMyUser()
        {
            if (base.User != null && base.User.Identity != null && !string.IsNullOrEmpty(base.User.Identity.Name))
            {
                if (this.MyUser == null || string.IsNullOrEmpty(this.MyUser.Email))
                    this.MyUser = accountMgr.GetUser(base.User.Identity.Name);
            }

            if (this.MyUser == null)
                this.MyUser = new UserModel(); // default to no permissions
            TempData.Keep();
        }
    }
}