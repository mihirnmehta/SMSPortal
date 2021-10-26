using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SMSPortal.Models;

namespace SMSPortal.Repository
{
    public interface ITopupRepository
    {
        #region Common Functions
        
        bool RecordInvoiceTopupTransaction(int iMPActID, decimal dAmount, string strTopupCurrency, string strLoggedInUserEmail, bool bFromAdminPortal);

        bool RecordPayPalTopupTransaction(int iMPActID, decimal dAmount, decimal dFees, string strTopupCurrency, string strPayerEmail, string strPayPalTxnID, string strPayPalTxnDate, string strLoggedInUserEmail);

        bool IsIPNReceived(string strTransactionID);

        #endregion

        #region Admin Portal
        List<DropDownDTO> GetOrgList(int CompanyID);

        List<DropDownDTO> GetMPActListByOrg(int iOrganisationID);
        #endregion

        #region User Portal
        List<DropDownDTO> GetAllMPAccountsByOrg(int iOrganisationID);

        List<DropDownDTO> GetPaymentTypesForMPAct(int iMPActID);
        #endregion

    }
}
