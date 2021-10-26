using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SMSPortal.Models;
using SMSPortal.DapperDAL;
using SMSPortal.Repository;

namespace SMSPortal.BusinessLogic
{
    public class ManagementUserBL
    {
        IManagementUserRepository iRepository = new ManagementUserDAL();
       
        #region Management User

        public List<ManagementUserDTO> GetManagementUsers(int iAccessLevelID)
        {
            List<ManagementUserDTO> lstmanagementuser = iRepository.GetManagementUsers(iAccessLevelID);
            return lstmanagementuser;
        }

        public List<DropDownDTO> GetAdminPortalAccessLevels(int iAccessLevelID)
        {
            List<DropDownDTO> lstAccessLevel = iRepository.GetAdminPortalAccessLevels(iAccessLevelID);
            return lstAccessLevel;
        }
        
        public bool DeleteManagementUser(int iManagementUserID)
        {
            bool bDelete = iRepository.DeleteManagementUser(iManagementUserID);
            return bDelete;
        }
                
        public bool AddManagementUser(string strForename, string strSurname, string strContactEmailAddress, int iAccessLevelID, string strContactPhonenumber, string sPassword,string strUpdatedBy)
        {
            
            string sEncryptedPassword = CommonFunctions.Encrypt(sPassword);
            bool bInsert = iRepository.AddManagementUser(strForename, strSurname, strContactEmailAddress, iAccessLevelID,strContactPhonenumber, sEncryptedPassword,strUpdatedBy);
            return bInsert;
        }

        public bool UpdateManagementUser(int iMgmtUserID, string strContactEmailAddress, int iAccessLevelID, string strContactPhonenumber, string strUpdatedBy)
        {
            bool bUpdate = iRepository.UpdateManagementUser(iMgmtUserID, strContactEmailAddress, iAccessLevelID, strContactPhonenumber, strUpdatedBy);
            return bUpdate;
             
        }

        public ManagementUserDTO GetUserByID(int iManagementUserID)
        {
            ManagementUserDTO objUserDTO = iRepository.GetUserByID(iManagementUserID);
            objUserDTO.Password = CommonFunctions.Decrypt(objUserDTO.Password);
            return objUserDTO;
        }

        public bool UpdateMgmtUserPassword(string strEmail, string sPassword, string strUpdatedBy)
        {
            string sEncryptedPassword = CommonFunctions.Encrypt(sPassword);
            bool bUpdate = iRepository.UpdateMgmtUserPassword(strEmail, sEncryptedPassword, strUpdatedBy);
            return bUpdate;
        }

        public bool IsEmailIDUnique(string strContactEmailAddress)
        {
            bool bSelect = iRepository.IsEmailIDUnique(strContactEmailAddress);
            return bSelect;
        }

        public bool IsEmailIDUniqueOnUpdate(int iManagementUserID, string strContactEmailAddress)
        {
            bool bSelect = iRepository.IsEmailIDUniqueOnUpdate(iManagementUserID, strContactEmailAddress);
            return bSelect;
        }


        #endregion


       
       
    }
}
