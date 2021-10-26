using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Transactions;

using Dapper;

using SMSPortal.Repository;
using SMSPortal.Models;

namespace SMSPortal.DapperDAL
{
    public class AccountsDAL : IAccountManagementRepository
    {
        public ManagementUserDTO Login(string strUsername, string strPassword)
        {
            //bool bAuthentic = false;
            ManagementUserDTO objUser = null;

            using (SqlConnection conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                conn.Open();
                                
                //string strQuery = @"select  mu.AccessLevelID from tblManagementUser mu where mu.Email='mehta.m@advanced-india.com' and mu.Password='mihir'";
                //int iAccessLevelID = conn.Query<int>(strQuery).First();
                try
                {
                    string sLoginQuery = "SELECT mu.ManagementUserID, mu.Forename, mu.Surname, mu.Email, mu.AccessLevelID FROM tblManagementUser mu WHERE mu.Email=@strUsername AND mu.Password=@strPassword";
                    objUser = conn.Query<ManagementUserDTO>(sLoginQuery, new { strUsername, strPassword }).Single();
                }
                catch (System.InvalidOperationException ex)
                {
                    if (ex.Message == "Sequence contains no elements")
                        objUser = null;
                }   
                
                conn.Close();
            }

            return objUser;
        }

        public bool DoChangePassword(string sLoggedInUserEmail, string sEncryptedPassword)
        {            
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                    
                connection.Open();
                string DoChangeQuery = "UPDATE tblManagementUser SET Password=@sEncryptedPassword WHERE Email=@sLoggedInUserEmail";
                connection.Execute(DoChangeQuery, new { sEncryptedPassword, sLoggedInUserEmail });
                connection.Close();
            }

            return true;            
        }

        public bool VerifyCurrentPassword(string sLoggedInUserEmail, string sEncryptedPassword)
        {
            bool bIsCurrentPassword = false;
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {                
                connection.Open();
                string sqlquery = "SELECT COUNT(*) AS count from tblManagementUser where Email = @sLoggedInUserEmail and Password=@sEncryptedPassword";
 
                int count = connection.Query<int>(sqlquery, new{ sLoggedInUserEmail, sEncryptedPassword }).Single();
                connection.Close();
                if (count > 0)
                    bIsCurrentPassword= true;
            }
            return bIsCurrentPassword;
        }

        public bool CheckEmailExists(string strEmailID)
        {
            bool bExist = false;

            using (SqlConnection conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                conn.Open();
                string strQuery = "SELECT COUNT(*) FROM tblManagementUser WHERE Email=@strEmailID";

                int count = conn.Query<int>(strQuery, new { strEmailID }).SingleOrDefault();

                if (count > 0)
                    bExist = true;
            }

            return bExist;
        }


        /// <summary>
        /// Verify UserPortal Login
        /// </summary>
        /// <param name="strUsername">Email ID of Organisation User</param>
        /// <param name="strPassword">Password</param>
        /// <returns>Returns true if the user id valid else false</returns>
        public OrganisationUserDTO LoginForUserPortal(string strUsername, string strPassword)
        {
            OrganisationUserDTO objUser = new OrganisationUserDTO();

            try
            {
                using (SqlConnection conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
                {
                    conn.Open();
                    string sLoginQuery = "SELECT orgUser.OrganisationUserID, orgUser.Forename, orgUser.Surname, orgUser.Email, orgUser.AccessLevelID, orgUser.OrganisationID FROM tblOrganisationUser orgUser where orgUser.Email=@strUsername and orgUser.Password=@strPassword";
                    objUser = conn.Query<OrganisationUserDTO>(sLoginQuery, new { strUsername, strPassword }).SingleOrDefault();
                    conn.Close();
                }
            }
            catch (System.InvalidOperationException ex)
            {
                if (ex.Message == "Sequence contains no elements")
                    objUser = null;
            }
            return objUser;
        }

        /// <summary>
        /// Gets Organisation Name branding based on OrganisationID
        /// </summary>
        /// <param name="iOrganisationID">organisationID</param>
        /// <returns>Returns corresponding Organisation Name</returns>
        public string GetOrgCompanyNameByID(int iOrganisationID)
        {
            string strOrgName = "";
            using (SqlConnection conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                conn.Open();
                string sQuery = (@"SELECT
	                                    cmp.CompanyName
                                    FROM 
	                                    tblOrganisation org
                                    JOIN
	                                    OACompany cmp
                                    ON
	                                    org.CompanyID=cmp.ID
                                    WHERE org.OrganisationID=@iOrganisationID");
                strOrgName = conn.Query<string>(sQuery, new { iOrganisationID }).SingleOrDefault();
                conn.Close();
            }
            return strOrgName;
        }
    }
}
