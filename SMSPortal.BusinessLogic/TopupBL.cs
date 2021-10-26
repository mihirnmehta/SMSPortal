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
    public class TopupBL
    {
        ITopupRepository iRepository = new TopupDAL();

        #region Common Functions

        public bool IsIPNReceived(string strTransactionID)
        {
            bool bResult = iRepository.IsIPNReceived(strTransactionID);
            return bResult;
        }

        public bool RecordInvoiceTopupTransaction(int iMPActID, decimal dAmount, string strTopupCurrency, string strLoggedInUserEmail, bool bFromAdminPortal)
        {
            bool bResult = iRepository.RecordInvoiceTopupTransaction(iMPActID, dAmount, strTopupCurrency, strLoggedInUserEmail, bFromAdminPortal);

            return bResult;
        }

        public bool RecordPayPalTopupTransaction(int iMPActID, decimal dAmount, decimal dFees, string strCurrency, string strPayerEmail, string strPayPalTxnID, string strTxnDate, string strLoggedInUserEmail)
        {
            bool bResult = iRepository.RecordPayPalTopupTransaction(iMPActID, dAmount, dFees, strCurrency, strPayerEmail, strPayPalTxnID, strTxnDate, strLoggedInUserEmail);

            return bResult;
        }
       
        #endregion

        #region Admin Portal
        public List<DropDownDTO> GetOrganisationList(int CompanyID)
        {
            List<DropDownDTO> lstOrganisation = iRepository.GetOrgList(CompanyID);
            return lstOrganisation;
        }

        public List<DropDownDTO> GetMPActListByOrg(int iOrganisationID)
        {
            List<DropDownDTO> lstMPAccount = iRepository.GetMPActListByOrg(iOrganisationID);
            return lstMPAccount;
        }
        #endregion

        #region User Portal
        public List<DropDownDTO> GetPaymentTypesForMPAct(int iMPActID)
        {
            List<DropDownDTO> lstPaymentTypes = iRepository.GetPaymentTypesForMPAct(iMPActID);
            return lstPaymentTypes;
        }

        public List<DropDownDTO> GetAllMPAccountsByOrg(int iOrganisationID)
        {
            return iRepository.GetAllMPAccountsByOrg(iOrganisationID);
        }
        #endregion
    }
}
