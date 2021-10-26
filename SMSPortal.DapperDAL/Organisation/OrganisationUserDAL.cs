using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using System.Data.SqlClient;
using Dapper;

using SMSPortal.Repository;
using SMSPortal.Models;
using SMSPortal.Repository.Organisation;

namespace SMSPortal.DapperDAL.Organisation
{
    public class OrganisationUserDAL : IOrganisationUserRepository
    {
        #region Common Functions

        /// <summary>
        /// Get list of AccessLevel for UserPortal
        /// </summary>
        /// <returns>List of AccessLevel for UserPortal</returns>
        public List<DropDownDTO> GetUserPortalAccessLevels()
        {
            List<DropDownDTO> lstOrgUserAccessLevels = new List<DropDownDTO>();
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                connection.Open();
                string sqlquery = @"SELECT AccessLevelID as Value, AccessLevel as Text FROM tblUserPortalAccessLevel";
                lstOrgUserAccessLevels = (List<DropDownDTO>)connection.Query<DropDownDTO>(sqlquery);
                connection.Close();
            }
            return lstOrgUserAccessLevels;
        }

        /// <summary>
        /// Checks whether EmailId already exists 
        /// </summary>
        /// <param name="strEmail">EmailId</param>
        /// <returns>Returns true if email is unique else returns false</returns>
        public bool IsEmailIDUnique(string strEmail)
        {
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                connection.Open();
                string sqlquery = "SELECT COUNT(*) AS count from tblOrganisationUser where Email = @strEmail";

                int count = connection.Query<int>(sqlquery, new { strEmail }).Single();
                connection.Close();
                if (count == 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Checks whether email already exists other than the Updating Organisation User
        /// </summary>
        /// <param name="iOrganisationUserID">Updating OrganisationUserID</param>
        /// <param name="strEmail">EmailID</param>
        /// <returns>Returns true if unique after ignoring the selected organisationUser otherwise false</returns>
        public bool IsEmailIDUniqueOnUpdate(int iOrganisationUserID, string strEmail)
        {
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                connection.Open();
                string sqlquery = "SELECT COUNT(*) AS COUNT FROM tblOrganisationUser WHERE Email = @strEmail AND OrganisationUserID!=@iOrganisationUserID";

                int count = connection.Query<int>(sqlquery, new { strEmail, iOrganisationUserID }).Single();
                connection.Close();
                if (count == 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Update Organisation User Password on Reset
        /// </summary>
        /// <param name="strEmail">EmailId  of whose Password is Reset</param>
        /// <param name="sEncryptedPassword">New Password in encrypted format</param>
        /// <returns>Return true if OrganisationUser updated otherwise false</returns>
        public bool UpdateOrgUserPassword(string strEmail, string sEncryptedPassword, string strUpdatedBy)
        {
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                connection.Open();

                string sqlquery = "UPDATE tblOrganisationUser SET Password=@sEncryptedPassword, UpdatedBy=@strUpdatedBy, UpdatedDate=GETDATE() WHERE  Email=@strEmail";
                connection.Execute(sqlquery, new { sEncryptedPassword, strUpdatedBy, strEmail });
                connection.Close();
            }
            return true;
        }

        #endregion

        #region AdminPortal

        /// <summary>
        /// Get list of Organisation Users based on Organisation
        /// </summary>
        /// <param name="iOrganisationID">Selected OrganisationID</param>
        /// <returns>List of Organisaton users of selected Organisation</returns>
        public List<OrganisationUserDTO> GetOrganisationUsers(int iOrganisationID)
        {
            List<OrganisationUserDTO> lstOrganisationUser = new List<OrganisationUserDTO>();
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                connection.Open();
                string sqlquery = string.Format(@"SELECT 
                                                        orgUser.OrganisationUserID, 
                                                        orgUser.ForeName + ' ' + orgUser.Surname as Forename, 
                                                        orgUser.Surname, 
                                                        orgUser.Email,
                                                        uAccessLvl.AccessLevel
                                                 FROM tblOrganisationUser orgUser
                                                 INNER JOIN 
                                                        tblUserPortalAccessLevel uAccessLvl 
                                                 ON 
                                                        orgUser.AccessLevelID= uAccessLvl.AccessLevelID 
                                                 WHERE 
                                                        orgUser.OrganisationID=@iOrganisationID",

                                                        iOrganisationID
                                               );
                lstOrganisationUser = (List<OrganisationUserDTO>)connection.Query<OrganisationUserDTO>(sqlquery, new { iOrganisationID });
            }
            return lstOrganisationUser;
        }

        /// <summary>
        /// Add OrganisationUser for the User Portal in selected Organisation
        /// </summary>
        /// <param name="iOrganisationID"></param>
        /// <param name="strForename"></param>
        /// <param name="strSurname"></param>
        /// <param name="strEmail"></param>
        /// <param name="sEncryptedPassword"></param>
        /// <param name="iAccessLevelID"></param>
        /// <param name="strUpdatedBy"></param>
        /// <returns>Returns true if User added otherwise false</returns>
        public bool AddOrganisationUser(int iOrganisationID, string strForename, string strSurname, string strEmail, string sEncryptedPassword, int iAccessLevelID, string strUpdatedBy)
        {
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                connection.Open();

                string sqlquery = "INSERT INTO tblOrganisationUser (OrganisationID, Forename,Surname,Email,Password,AccessLevelID,UpdatedBy)VALUES(@iOrganisationID,@strForename,@strSurname,@strEmail,@sEncryptedPassword,@iAccessLevelID,@strUpdatedBy)";
                connection.Execute(sqlquery, new { iOrganisationID, strForename, strSurname, strEmail, sEncryptedPassword, iAccessLevelID, strUpdatedBy });

                connection.Close();
            }

            return true;
        }

        /// <summary>
        /// Gets Details of selected OrganisationUser for updation
        /// </summary>
        /// <param name="iOrganisationUserID">selected OrganisationUserId</param>
        /// <returns>Return details of OrganisationUser</returns>
        public OrganisationUserDTO GetOrganisationUserByID(int iOrganisationUserID)
        {
            OrganisationUserDTO objManageOrgUsers = null;
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                connection.Open();
                string sqlquery = "SELECT OrganisationUserID, OrganisationID, Forename, Surname, Email, AccessLevelID FROM tblOrganisationUser WHERE OrganisationUserID=@iOrganisationUserID";
                objManageOrgUsers = connection.Query<OrganisationUserDTO>(sqlquery, new { iOrganisationUserID }).Single();

                connection.Close();
            }
            return objManageOrgUsers;
        }

        /// <summary>
        /// Update OrganisationUser
        /// </summary>
        /// <param name="iOrganisationUserID">Id of OrganisationUser to be updated</param>
        /// <param name="strEmail">EmailID</param>
        /// <param name="iAccessLevelID">AccessLevelID</param>
        /// <returns>Returns true if OrganisationUser updated otherwise false</returns>
        public bool UpdateOrganisationUser(int iOrganisationUserID, string strEmail, int iAccessLevelID, string strUpdatedBy)
        {
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                connection.Open();
                string sqlquery = ("UPDATE tblOrganisationUser SET Email=@strEmail, AccessLevelID=@iAccessLevelID, UpdatedBy=@strUpdatedBy, UpdatedDate=GETDATE() WHERE OrganisationUserID=@iOrganisationUserID");
                connection.Execute(sqlquery, new { strEmail, iAccessLevelID, strUpdatedBy, iOrganisationUserID });

                connection.Close();
            }

            return true;
        }

        /// <summary>
        /// Delete selected Organisation User
        /// </summary>
        /// <param name="iOrganisationUserID">Id of selected OrganisationUser</param>
        /// <returns>returns true if User is deleted otherwise false</returns>
        public bool DeleteOrganisationUser(int iOrganisationUserID)
        {
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                connection.Open();
                string sqlquery = "DELETE FROM tblOrganisationUser WHERE OrganisationUserID=@iOrganisationUserID";
                connection.Execute(sqlquery, new { iOrganisationUserID });
            }

            return true;
        }

        #endregion

        /*------------------------------------------------------*/
        /*------------------------------------------------------*/

        #region UserPortal

        /// <summary>
        /// Get list of Organisation Users based on Organisation
        /// </summary>
        /// <param name="iOrganisationID">Selected OrganisationID</param>
        /// <returns>List of Organisaton users of selected Organisation</returns>
        public List<OrganisationUserDTO> GetOrganisationUsersForUserPortal(int iOrganisationID)
        {
            List<OrganisationUserDTO> lstOrganisationUser = new List<OrganisationUserDTO>();
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                connection.Open();
                string sqlquery = string.Format(@"SELECT 
                                                        orgUser.OrganisationUserID, 
                                                        orgUser.ForeName + ' ' + orgUser.Surname as Forename, 
                                                        orgUser.Surname, 
                                                        orgUser.Email,
                                                        uAccessLvl.AccessLevel
                                                 FROM tblOrganisationUser orgUser
                                                 INNER JOIN 
                                                        tblUserPortalAccessLevel uAccessLvl 
                                                 ON 
                                                        orgUser.AccessLevelID= uAccessLvl.AccessLevelID 
                                                 WHERE 
                                                        orgUser.OrganisationID=@iOrganisationID",

                                                        iOrganisationID
                                               );
                lstOrganisationUser = (List<OrganisationUserDTO>)connection.Query<OrganisationUserDTO>(sqlquery, new { iOrganisationID });
            }
            return lstOrganisationUser;
        }

        /// <summary>
        /// Add OrganisationUser for the User Portal in selected Organisation
        /// </summary>
        /// <param name="iOrganisationID"></param>
        /// <param name="strForename"></param>
        /// <param name="strSurname"></param>
        /// <param name="strEmail"></param>
        /// <param name="sEncryptedPassword"></param>
        /// <param name="iAccessLevelID"></param>
        /// <param name="strUpdatedBy"></param>
        /// <returns>Returns true if User added otherwise false</returns>
        public bool AddOrganisationUserForUserPortal(int iOrganisationID, string strForename, string strSurname, string strEmail, string sEncryptedPassword, int iAccessLevelID, string strUpdatedBy)
        {
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                connection.Open();

                string sqlquery = "INSERT INTO tblOrganisationUser (OrganisationID, Forename,Surname,Email,Password,AccessLevelID,UpdatedBy)VALUES(@iOrganisationID,@strForename,@strSurname,@strEmail,@sEncryptedPassword,@iAccessLevelID,@strUpdatedBy)";
                connection.Execute(sqlquery, new { iOrganisationID, strForename, strSurname, strEmail, sEncryptedPassword, iAccessLevelID, strUpdatedBy });

                connection.Close();
            }

            return true;
        }

        /// <summary>
        /// Gets Details of selected OrganisationUser for updation
        /// </summary>
        /// <param name="iOrganisationUserID">selected OrganisationUserId</param>
        /// <returns>Return details of OrganisationUser</returns>
        public OrganisationUserDTO GetOrganisationUserByIDForUserPortal(int iOrganisationUserID)
        {
            OrganisationUserDTO objManageOrgUsers = null;
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                connection.Open();
                string sqlquery = "SELECT OrganisationUserID, OrganisationID, Forename, Surname, Email, AccessLevelID FROM tblOrganisationUser WHERE OrganisationUserID=@iOrganisationUserID";
                objManageOrgUsers = connection.Query<OrganisationUserDTO>(sqlquery, new { iOrganisationUserID }).Single();

                connection.Close();
            }
            return objManageOrgUsers;
        }

        /// <summary>
        /// Update OrganisationUser
        /// </summary>
        /// <param name="iOrganisationUserID">Id of OrganisationUser to be updated</param>
        /// <param name="strEmail">EmailID</param>
        /// <param name="iAccessLevelID">AccessLevelID</param>
        /// <returns>Returns true if OrganisationUser updated otherwise false</returns>
        public bool UpdateOrganisationUserForUserPortal(int iOrganisationUserID, string strEmail, int iAccessLevelID, string strUpdatedBy)
        {
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                connection.Open();
                string sqlquery = ("UPDATE tblOrganisationUser SET Email=@strEmail, AccessLevelID=@iAccessLevelID, UpdatedBy=@strUpdatedBy, UpdatedDate=GETDATE() WHERE OrganisationUserID=@iOrganisationUserID");
                connection.Execute(sqlquery, new { strEmail, iAccessLevelID, strUpdatedBy, iOrganisationUserID });

                connection.Close();
            }

            return true;
        }


        /// <summary>
        /// Delete selected Organisation User
        /// </summary>
        /// <param name="iOrganisationUserID">Id of selected OrganisationUser</param>
        /// <returns>returns true if User is deleted otherwise false</returns>
        public bool DeleteOrganisationUserForUserPortal(int iOrganisationUserID)
        {
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                connection.Open();
                string sqlquery = "DELETE FROM tblOrganisationUser WHERE OrganisationUserID=@iOrganisationUserID";
                connection.Execute(sqlquery, new { iOrganisationUserID });
            }

            return true;
        }

        public bool DoChangePassword(string sLoggedInUserEmail, string sEncryptedPassword)
        {
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {

                connection.Open();
                string DoChangeQuery = ("UPDATE tblOrganisationUser SET Password=@sEncryptedPassword WHERE Email=@sLoggedInUserEmail");
                connection.Execute(DoChangeQuery, new { sEncryptedPassword, sLoggedInUserEmail });
                connection.Close();
            }

            return true;
        }

        /// <summary>
        /// Validate Password wheater entered and current password are same
        /// </summary>
        /// <param name="sEncryptedPassword">Entered Password</param>
        /// <returns>Returns true if password is current password otherwise false</returns>
        public bool VerifyCurrentPassword(string sLoggedInUserEmail, string sEncryptedPassword)
        {
            bool bIsCurrentPassword = false;
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                connection.Open();
                string sqlquery = ("SELECT COUNT(*) AS COUNT FROM tblOrganisationUser WHERE Email= @sLoggedInUserEmail AND Password = @sEncryptedPassword");

                int count = connection.Query<int>(sqlquery, new { sLoggedInUserEmail, sEncryptedPassword }).Single();
                connection.Close();
                if (count > 0)
                    bIsCurrentPassword = true;
            }
            return bIsCurrentPassword;
        }

        public bool CheckEmailExists(string strEmailID)
        {
            bool bExist = false;

            using (SqlConnection conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                conn.Open();
                string strQuery = string.Format("select count(*) from tblOrganisationUser where Email=@strEmailID");

                int count = conn.Query<int>(strQuery, new { strEmailID }).SingleOrDefault();

                if (count > 0)
                    bExist = true;
            }

            return bExist;
        }

        #endregion
    }
}
