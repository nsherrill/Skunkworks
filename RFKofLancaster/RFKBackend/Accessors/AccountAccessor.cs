using RFKBackend.Shared;
using RFKBackend.Shared.DataContracts;
using RFKBackend.Shared.DataContracts.Account;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Accessors
{
    public interface IAccountAccessor
    {
        UserModel GetUserByEmail(string email);
        UserModel GetUserById(int userId);
        void CreateUser(UserModel newUser);
    }

    public class AccountAccessor : BaseSqlAccessor, IAccountAccessor
    {
        internal override string PRIMARY_KEY_NAME => "[UserId]";
        internal override string TABLE_NAME => "[dbo].[Users]";

        public void CreateUser(UserModel newUser)
        {
            if (newUser == null)
                return;

            var existingUser = GetUserByEmail(newUser.Email);
            if (existingUser != null)
                return;

            existingUser = GetUserById(newUser.UserId);
            if (existingUser != null)
                return;

            var paramList = new SqlParameter[]
            {
                new SqlParameter("@UserId", newUser.UserId),
                new SqlParameter("@Email", newUser.Email),
                new SqlParameter("@CanLogin", newUser.CanLogin),
                new SqlParameter("@CanRead", newUser.CanRead),
                new SqlParameter("@CanWrite", newUser.CanWrite),
            };

            var sql = GetBasicMergeText(paramList);

            base.ExecuteScalar(sql, paramList);
        }

        public UserModel GetUserById(int userId)
        {
            var sql = base.GetBasicFindSql("@id");
            var result = base.ExecuteReader<UserModel>(sql, UserModelReader, new SqlParameter[] { new SqlParameter("@id", userId) });
            return result.FirstOrDefault();
        }

        public UserModel GetUserByEmail(string email)
        {
            var sql = base.GetBasicFindSql("@email", this.TABLE_NAME, "[Email]");
            var result = base.ExecuteReader<UserModel>(sql, UserModelReader, new SqlParameter[] { new SqlParameter("@email", email) });
            return result.FirstOrDefault();
        }

        #region privates
        private UserModel UserModelReader(SqlDataReader rdr)
        {
            UserModel result = new UserModel()
            {
                UserId = Convert.ToInt32(rdr["UserId"]),
                Email = rdr["Email"].ToString(),
                CanLogin = (bool)rdr["CanLogin"],
                CanWrite = (bool)rdr["CanWrite"],
                CanRead = (bool)rdr["CanRead"],
            };
            return result;
        }
        #endregion
    }
}
