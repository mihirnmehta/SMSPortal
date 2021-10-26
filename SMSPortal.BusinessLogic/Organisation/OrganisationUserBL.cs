using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SMSPortal.Models;
using SMSPortal.DapperDAL.Organisation;
using SMSPortal.Repository.Organisation;

namespace SMSPortal.BusinessLogic.Organisation
{
    public class OrganisationUserBL
    {
        IOrganisationUserRepository iRepository = new OrganisationUserDAL();

        #region Common Functions

        public List<DropDownDTO> GetUserPortalAccessLevels()
        {
            List<DropDownDTO> lstuseraccesslevel = iRepository.GetUserPortalAccessLevels();
            return lstuseraccesslevel;
        }

        public bool IsEmailIDUnique(string strEmail)
        {
            bool bSelect = iRepository.IsEmailIDUnique(strEmail);
            return bSelect;
        }        
        
        public bool IsEmailIDUniqueOnUpdate(int iOrganisationUserID, string strEmail)
        {
            bool bSelect = iRepository.IsEmailIDUniqueOnUpdate(iOrganisationUserID, strEmail);
            return bSelect;
        }

        public bool UpdateOrgUserPassword(string strEmail, string strPassword, string strUpdatedBy)
        {
            string sEncryptedPassword = CommonFunctions.Encrypt(strPassword);
            bool bUpdate = iRepository.UpdateOrgUserPassword(strEmail, sEncryptedPassword, strUpdatedBy);
            return bUpdate;
        }

        #endregion

        # region AdminPortal

        public List<OrganisationUserDTO> GetOrganisationUsers(int iOrganisationID)
        {
            List<OrganisationUserDTO> lstOrganisationUser = iRepository.GetOrganisationUsers(iOrganisationID);
            return lstOrganisationUser;
        }
        
        public bool AddOrganisationUser(int iOrganisationID,  string strForename, string strSurname, string strEmail, string sPassword, int iAccessLevelID, string strUpdatedBy)
        {
            
            string sEncryptedPassword = CommonFunctions.Encrypt(sPassword);
            bool bInsert=iRepository.AddOrganisationUser(iOrganisationID, strForename, strSurname, strEmail, sEncryptedPassword, iAccessLevelID, strUpdatedBy);
            return bInsert;

        }
       
        public OrganisationUserDTO GetOrganisationUserByID(int iOrganisationUserID)
        {
            OrganisationUserDTO objOrgUserDTO=iRepository.GetOrganisationUserByID(iOrganisationUserID);
            return objOrgUserDTO;
        }

        public bool UpdateOrganisationUser(int iOrganisationUserID, string strEmail, int iAccessLevelID, string sUpdatedBy)
        {
            bool iUpdate = iRepository.UpdateOrganisationUser(iOrganisationUserID, strEmail, iAccessLevelID, sUpdatedBy);
            return iUpdate;
        }
        
        public bool DeleteOrganisationUser(int iOrganisationUserID)
        {
            bool bDelete=iRepository.DeleteOrganisationUser(iOrganisationUserID);
            return bDelete;
        }

        #endregion

        # region UserPortal

        public List<OrganisationUserDTO> GetOrganisationUsersForUserPortal(int iOrganisationID)
        {
            List<OrganisationUserDTO> lstOrganisationUser = iRepository.GetOrganisationUsersForUserPortal(iOrganisationID);
            return lstOrganisationUser;
        }

        public bool AddOrganisationUserForUserPortal(int iOrganisationID, string strForename, string strSurname, string strEmail, string sPassword, int iAccessLevelID, string strUpdatedBy)
        {

            string sEncryptedPassword = CommonFunctions.Encrypt(sPassword);
            bool bInsert = iRepository.AddOrganisationUserForUserPortal(iOrganisationID, strForename, strSurname, strEmail, sEncryptedPassword, iAccessLevelID, strUpdatedBy);
            return bInsert;

        }

        public OrganisationUserDTO GetOrganisationUserByIDForUserPortal(int iOrganisationUserID)
        {
            OrganisationUserDTO objOrgUserDTO = iRepository.GetOrganisationUserByIDForUserPortal(iOrganisationUserID);
            return objOrgUserDTO;
        }

        public bool UpdateOrganisationUserForUserPortal(int iOrganisationUserID, string strEmail, int iAccessLevelID, string sUpdatedBy)
        {
            bool iUpdate = iRepository.UpdateOrganisationUserForUserPortal(iOrganisationUserID, strEmail, iAccessLevelID, sUpdatedBy);
            return iUpdate;
        }

        public bool DeleteOrganisationUserForUserPortal(int iOrganisationUserID)
        {
            bool bDelete = iRepository.DeleteOrganisationUserForUserPortal(iOrganisationUserID);
            return bDelete;
        }

        public bool DoChangePassword(string sLoggedInUserEmail, string sPassword)
        {
            string sEncryptedPassword = CommonFunctions.Encrypt(sPassword);
            bool bChngpwd = iRepository.DoChangePassword(sLoggedInUserEmail, sEncryptedPassword);
            return bChngpwd;
        }

        public bool VerifyCurrentPassword(string sLoggedInUserEmail, string sPassword)
        {
            string sEncryptedPassword = CommonFunctions.Encrypt(sPassword);
            bool bIsCurrentPassword = iRepository.VerifyCurrentPassword(sLoggedInUserEmail, sEncryptedPassword);
            return bIsCurrentPassword;
        }

        public bool CheckEmailExists(string strEmailID)
        {
            bool bExist = false;
            bExist = iRepository.CheckEmailExists(strEmailID);

            return bExist;
        }

        #endregion

    }
}
