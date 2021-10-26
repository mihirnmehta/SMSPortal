using SMSPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSPortal.Repository
{
    public interface IManagementUserRepository
    {
        List<ManagementUserDTO> GetManagementUsers(int iAccessLevelID);

        List<DropDownDTO> GetAdminPortalAccessLevels(int iAccessLevelID);

        bool DeleteManagementUser(int iManagementUserID);

        bool AddManagementUser(string strForename, string strSurname, string strContactEmailAddress, int iAccessLevelID, string strContactPhonenumber, string strPassword,string strUpdatedBy);

        bool UpdateManagementUser(int iMgmtUserID, string strContactEmailAddress, int iAccessLevelID, string strContactPhonenumber, string strUpdatedBy);

        ManagementUserDTO GetUserByID(int iManagementUserID);

        bool UpdateMgmtUserPassword(string strEmail, string strPassword, string strUpdatedBy);

        bool IsEmailIDUnique(string strContactEmailAddress);

        bool IsEmailIDUniqueOnUpdate(int iManagementUserID, string strContactEmailAddress);
    }
}
