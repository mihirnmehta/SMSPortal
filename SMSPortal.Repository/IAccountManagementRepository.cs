using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SMSPortal.Models;
namespace SMSPortal.Repository
{
    public interface IAccountManagementRepository
    {
      
        ManagementUserDTO Login(string strUsername, string strPassword);
        
        bool DoChangePassword(string sLoggedInUserEmail, string strPassword);

        bool VerifyCurrentPassword(string sLoggedInUserEmail, string sEncryptedPassword);

        bool CheckEmailExists(string strEmailID);

        OrganisationUserDTO LoginForUserPortal(string strUsername, string strPassword);

        string GetOrgCompanyNameByID(int iOrganisationID);
    }
}
