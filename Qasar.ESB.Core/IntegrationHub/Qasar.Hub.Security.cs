using System;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;

namespace Qasar.ESB
{
    /// <summary>
    /// Provides security checks
    /// </summary>
    public class Security
    {
        public enum SecurityCheck
        {
            InvalidPassword,
            InvalidSource,
            InvalidLogin,
            Success,
        }
        public Security()
        {
        }

        public SecurityCheck ConfirmCredentials(string loginID, string password, string source)
        {
            SecurityCheck res = SecurityCheck.Success;
            Database db = DatabaseFactory.CreateDatabase("Hub");
            DbCommand dbCommand = db.GetStoredProcCommand("uspConfirmLogin");
            db.AddInParameter(dbCommand, "LoginID", DbType.String, loginID);
            db.AddOutParameter(dbCommand, "Password", DbType.String, 20);
            db.ExecuteNonQuery(dbCommand);
            string pwd = Convert.ToString(db.GetParameterValue(dbCommand, "@Password"));
            if (pwd == string.Empty)
            {
                //No record exists for this login ID
                res = SecurityCheck.InvalidLogin;
            }
            else if (pwd != password)
            {
                //The password provided was not the same as in the database
                res = SecurityCheck.InvalidPassword;
            }
            else if (pwd == password)
            {
                //password's ok so now check that the login details are associated with source system from whence
                //they came
                res = SecurityCheck.Success;
                DbCommand dbCommand2 = db.GetStoredProcCommand("uspConfirmSource");
                db.AddInParameter(dbCommand2, "LoginID", DbType.String, loginID);
                db.AddOutParameter(dbCommand2, "Source", DbType.String, 50);
                db.ExecuteNonQuery(dbCommand2);
                string s = Convert.ToString(db.GetParameterValue(dbCommand2, "@Source"));
                if (s != Convert.ToString(source))
                {
                    res = SecurityCheck.InvalidSource;
                }
            }
            return res;
        }
    }
}
