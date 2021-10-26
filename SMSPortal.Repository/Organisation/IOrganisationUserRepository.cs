using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SMSPortal.Models;

namespace SMSPortal.Repository.Organisation
{
    public interface IOrganisationUserRepository
    {
        #region Common Functions

        List<DropDownDTO> GetUserPortalAccessLevels();

        bool IsEmailIDUnique(string strEmail);

        bool IsEmailIDUniqueOnUpdate(int iOrganisationUserID, string strEmail);

        bool UpdateOrgUserPassword(string strEmail, string strPassword, string strUpdatedBy);

        #endregion

        #region AdminPortal

        List<OrganisationUserDTO> GetOrganisationUsers(int iOrganisationID);

        bool AddOrganisationUser(int iOrganisationID, string strForename, string strSurname, string strEmail, string strPassword, int iAccessLevelID, string strUpdatedBy);

        OrganisationUserDTO GetOrganisationUserByID(int iOrganisationUserID);

        bool UpdateOrganisationUser(int iOrganisationUserID, string strEmail, int iAccessLevelID, string sUpdatedBy);

        bool DeleteOrganisationUser(int iOrganisationUserID);
       
        #endregion

        #region UserPortal 

        List<OrganisationUserDTO> GetOrganisationUsersForUserPortal(int iOrganisationID);

        bool AddOrganisationUserForUserPortal(int iOrganisationID, string strForename, string strSurname, string strEmail, string strPassword, int iAccessLevelID, string strUpdatedBy);

        OrganisationUserDTO GetOrganisationUserByIDForUserPortal(int iOrganisationUserID);

        bool UpdateOrganisationUserForUserPortal(int iOrganisationUserID, string strEmail, int iAccessLevelID, string sUpdatedBy);

        bool DeleteOrganisationUserForUserPortal(int iOrganisationUserID);

        bool DoChangePassword(string sLoggedInUserEmail, string sPassword);

        bool VerifyCurrentPassword(string sLoggedInUserEmail, string sEncryptedPassword);

        bool CheckEmailExists(string strEmailID);
        #endregion
    }
}

