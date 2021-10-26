using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Data.SqlClient;
using Dapper;

using SMSPortal.Repository;
using SMSPortal.Models;


namespace SMSPortal.DapperDAL
{
    public class ManagementUserDAL : IManagementUserRepository
    {
        /// <summary>
        /// Gets Management User other than Logged user and Users with Less AccessLevel
        /// </summary>
        /// <param name="sLoggedInUserEmail"></param>
        /// <param name="iAccessLevelID"></param>
        /// <returns></returns>
        public List<ManagementUserDTO> GetManagementUsers(int iAccessLevelID)
        {
            List<ManagementUserDTO> lstManagementUser = new List<ManagementUserDTO>();
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {

                string sqlquery = @"SELECT ManagementUserID, 
                                           ForeName + ' '+Surname as Forename, 
                                           Surname, 
                                           Email, 
                                           PhoneNumber,
                                           AccessLevel
                                    FROM tblManagementUser 
                                    INNER JOIN 
                                         tblAdminPortalAccessLevel 
                                    on 
                                         tblManagementUser.AccessLevelID= tblAdminPortalAccessLevel.AccessLevelID
                                    WHERE tblManagementUser.AccessLevelID>=@iAccessLevelID";

                lstManagementUser = (List<ManagementUserDTO>)connection.Query<ManagementUserDTO>(sqlquery, new { iAccessLevelID });
            }
            return lstManagementUser;
        }

        public List<DropDownDTO> GetAdminPortalAccessLevels(int iAccessLevelID)
        {
            List<DropDownDTO> lstAccessLevel = new List<DropDownDTO>();

            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                string sqlquery = "SELECT AccessLevelID as Value, AccessLevel AS Text from tblAdminPortalAccessLevel WHERE AccessLevelID >= @iAccessLevelID";

                lstAccessLevel = (List<DropDownDTO>)connection.Query<DropDownDTO>(sqlquery, new { iAccessLevelID });                
            }
            return lstAccessLevel;
        }

        public bool DeleteManagementUser(int iManagementUserID)
        {         
            int iCount = 0;
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                string sqlquery = "DELETE FROM tblManagementUser where ManagementUserID=@iManagementUserID";
                iCount = connection.Execute(sqlquery, new { iManagementUserID });
            }

            if (iCount == 1)
                return true;
            else
                return false;           
        }

        public bool IsEmailIDUnique(string strContactEmailAddress)
        {
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                string sqlquery = "SELECT COUNT(*) AS count from tblManagementUser where Email = @strContactEmailAddress";
                int count = connection.Query<int>(sqlquery, new { strContactEmailAddress }).Single();
               
                if (count == 0)
                    return true;
                else
                    return false;
            }
        }

        public bool IsEmailIDUniqueOnUpdate(int iManagementUserID, string strContactEmailAddress)
        {
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {                
                string sqlquery = string.Format(@"SELECT COUNT(*) AS count from tblManagementUser where Email = @strContactEmailAddress
                                                   AND ManagementUserID != @iManagementUserID");

                int count = connection.Query<int>(sqlquery, new { strContactEmailAddress, iManagementUserID }).Single();
                
                if (count == 0)
                    return true;
                else
                    return false;
            }
        }

        public bool AddManagementUser(string strForename, string strSurname, string strContactEmailAddress, int iAccessLevelID, string strContactPhonenumber, string sEncryptedPassword, string strUpdatedBy)
        {
            int iCount = 0;
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {                 
                string sqlquery = @"INSERT INTO tblManagementUser (Forename, Surname, Email, PhoneNumber, AccessLevelID, Password, UpdatedBy) 
                                    VALUES (@strForename, @strSurname, @strContactEmailAddress, @strContactPhonenumber, @iAccessLevelID, @sEncryptedPassword, @strUpdatedBy)";
                iCount = connection.Execute(sqlquery, new { strForename, strSurname, strContactEmailAddress, strContactPhonenumber, iAccessLevelID, sEncryptedPassword, strUpdatedBy});

                if (iCount == 1)
                    return true;
                else
                    return false;
            }            
           
        }

        public bool UpdateManagementUser(int iMgmtUserID, string strContactEmailAddress, int iAccessLevelID, string strContactPhonenumber, string strUpdatedBy)
        {
            int iCount = 0;
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {                
                string sqlquery = @"UPDATE tblManagementUser SET Email=@strContactEmailAddress, AccessLevelID=@iAccessLevelID, PhoneNumber=@strContactPhonenumber, UpdatedBy=@strUpdatedBy, UpdatedDate=GETDATE() WHERE ManagementUserID=@iMgmtUserID";
                iCount = connection.Execute(sqlquery, new { strContactEmailAddress, iAccessLevelID, strContactPhonenumber, strUpdatedBy, iMgmtUserID  });

                if(iCount == 1)
                    return true;
                else
                    return false;               
            }       
        }

        public bool UpdateMgmtUserPassword(string strEmail, string sEncryptedPassword, string strUpdatedBy)
        {
            int iCount = 0;
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {                
                string sqlquery = string.Format(@"UPDATE tblManagementUser 
                                                    SET Password=@sEncryptedPassword, UpdatedBy=@strUpdatedBy, UpdatedDate=GETDATE() 
                                                    WHERE  Email=@strEmail");

                iCount = connection.Execute(sqlquery, new { sEncryptedPassword, strUpdatedBy, strEmail });
                connection.Close();

                if (iCount == 1)
                    return true;
                else
                    return false;
            }                    
        }

        public ManagementUserDTO GetUserByID(int iManagementUserID)
        {
            ManagementUserDTO objManagementUser = null;
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                string sqlquery = "SELECT ManagementUserID, Forename, Surname, Email, PhoneNumber, Password, AccessLevelID from tblManagementUser WHERE ManagementUserID='" + iManagementUserID + "'";
                objManagementUser = connection.Query<ManagementUserDTO>(sqlquery).FirstOrDefault(m => m.ManagementUserID == iManagementUserID);
            }
           
            return objManagementUser;
        }
    }
}
