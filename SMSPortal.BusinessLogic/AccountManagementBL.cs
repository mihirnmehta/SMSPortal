using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SMSPortal.Models;
using SMSPortal.Repository;
using SMSPortal.DapperDAL;


namespace SMSPortal.BusinessLogic
{
    public class AccountManagementBL
    {
       
        IAccountManagementRepository iRepository = new AccountsDAL();

        public ManagementUserDTO DoLogin(string strUsername, string strPassword)
        {
            //bool bAuthentic = false;
            ManagementUserDTO objUser = iRepository.Login(strUsername, strPassword);
            return objUser;            
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
            bool bSelect = iRepository.VerifyCurrentPassword(sLoggedInUserEmail, sEncryptedPassword);
            return bSelect;
        }

        public bool CheckEmailExists(string strEmailID)
        {
            bool bExist = false;
            bExist = iRepository.CheckEmailExists(strEmailID);

            return bExist;
        }


        public OrganisationUserDTO DoLoginForUserPortal(string strUsername, string strPassword)
        {
            OrganisationUserDTO objUser = iRepository.LoginForUserPortal(strUsername, strPassword);
            return objUser;
        }

        public string GetOrgCompanyNameByID(int iOrganisationID)
        {
            string orgName = iRepository.GetOrgCompanyNameByID(iOrganisationID);
            return orgName;
        }
    }
}
