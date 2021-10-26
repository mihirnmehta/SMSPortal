using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SMSPortal.Models;

namespace SMSPortal.Repository.Organisation
{
    public interface IMPAccountRepository
    {
        #region Common Function

        bool IsUsernameUnique(string strUsername);
        
        bool IsUsernameUniqueOnUpdate(string strUsername, int iAccountID);

        BillingMethodDTO GetOrgBillingMethods(int iOrgID);

        #endregion

        #region AdminPortal Micro Payment Account
        List<MPAccountDTO> GetMPAccountsByOrganisationID(int iOrgID);

        bool AddMPAccount(bool bIsEnable, string sAccLogin, string sDesc, string sPassword, int iOrgID,
                          bool bSendLowBalWarn, int iBalWarnLmt, string sBalWarnEmail,
                          bool iUsageRestriction, int sServiceCode,
                          string sWorkingDayStart, string sWorkingDayFinish, string sNonWorkingDayStart, string sNonWorkingDayFinish,
                          bool bPayPal, bool bInvoice, string sCreatedBy, string sUpdatedBy);

        MPAccountDTO GetMPAccountByID(int iAccountID);

        bool UpdateMPAccount(int iAccountID, bool bIsEnabled, string sAccLogin, string sDesc, string sPassword,
                             bool bSendLowBalWarn, int iBalWarnLmt, string sBalWarnEmail,
                             bool bUsageRestriction, int iServiceCode,
                             string sWorkingDayStart, string sWorkingDayFinish, string sNonWorkingDayStart, string sNonWorkingDayFinish,
                             bool bPayPal, bool bInvoice, string sUpdatedBy);
        
        #endregion

        #region UserPortal Micro Payment Account
        List<MPAccountDTO> GetMPAccountsByOrganisationIDForUserPortal(int iOrgID);

        bool AddMPAccountForUserPortal(bool bIsEnable, string sAccLogin, string sDesc, string sPassword, int iOrgID,
                          bool bSendLowBalWarn, int iBalWarnLmt, string sBalWarnEmail,
                          bool iUsageRestriction, int sServiceCode,
                          string sWorkingDayStart, string sWorkingDayFinish, string sNonWorkingDayStart, string sNonWorkingDayFinish,
                          bool bPayPal, bool bInvoice, string sCreatedBy, string sUpdatedBy);

        MPAccountDTO GetMPAccountByIDForUserPortal(int iAccountID);

        bool UpdateMPAccountForUserPortal(int iAccountID, bool bIsEnabled, string sAccLogin, string sDesc, string sPassword,
                             bool bSendLowBalWarn, int iBalWarnLmt, string sBalWarnEmail,
                             bool bUsageRestriction, int iServiceCode,
                             string sWorkingDayStart, string sWorkingDayFinish, string sNonWorkingDayStart, string sNonWorkingDayFinish,
                             bool bPayPal, bool bInvoice, string sUpdatedBy);
        #endregion
    }
}
