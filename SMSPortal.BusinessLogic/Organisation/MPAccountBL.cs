using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SMSPortal.Models;
using SMSPortal.Repository.Organisation;
using SMSPortal.DapperDAL.Organisation;

using SMSPortal.DapperDAL;
using SMSPortal.Repository;

namespace SMSPortal.BusinessLogic.Organisation
{
    public class MPAccountBL
    {
        IMPAccountRepository iRepository = new MPAccountDAL();
        
        #region Common Functions

        public bool IsUsernameUnique(string strUsername)
        {
            bool bResult = iRepository.IsUsernameUnique(strUsername);
            return bResult;
        }
        
        public bool IsUsernameUniqueOnUpdate(string strUsername, int iAccountID)
        {
            bool bResult = iRepository.IsUsernameUniqueOnUpdate(strUsername,iAccountID);
            return bResult;
        }

        public BillingMethodDTO GetOrgBillingMethods(int iOrgID)
        {
            BillingMethodDTO objBillingMethodDTO = iRepository.GetOrgBillingMethods(iOrgID);
            return objBillingMethodDTO;
        }

        #endregion

        #region AdminPortal Micro Payment Account
        public List<MPAccountDTO> GetMPAccountsByOrganisationID(int iOrgID)
        {
            List<MPAccountDTO> lstMPAccount = iRepository.GetMPAccountsByOrganisationID(iOrgID);
            return lstMPAccount;
        }

        public bool AddMPAccount(bool bIsEnable, string sAccLogin, string sDesc, string sPassword, int iOrgID, 
                                 bool bSendLowBalWarn, int iBalWarnLmt, string sBalWarnEmail, 
                                 bool bUsageRestriction, int iServiceCode, 
                                 string sWorkingDayStart, string sWorkingDayFinish, string sNonWorkingDayStart, string sNonWorkingDayFinish,
                                 bool bPayPal, bool bInvoice, string sCreatedBy, string sUpdatedBy)
        {
            string sEncryptedPassword = CommonFunctions.Encrypt(sPassword);
            bool bIsAdded = iRepository.AddMPAccount(bIsEnable, sAccLogin, sDesc, sEncryptedPassword, iOrgID, 
                                                     bSendLowBalWarn, iBalWarnLmt, sBalWarnEmail, 
                                                     bUsageRestriction, iServiceCode, sWorkingDayStart, 
                                                     sWorkingDayFinish, sNonWorkingDayStart, sNonWorkingDayFinish, 
                                                     bPayPal, bInvoice, sCreatedBy,sUpdatedBy);
            return bIsAdded;
        }

        public MPAccountDTO GetMPAccountByID(int iAccountID)
        {
            MPAccountDTO objMPAccountDTO = iRepository.GetMPAccountByID(iAccountID);
            objMPAccountDTO.Password = CommonFunctions.Decrypt(objMPAccountDTO.Password);
            return objMPAccountDTO;
        }

        public bool UpdateMPAccount(int iAccountID,bool bIsEnabled, string sAccLogin, string sDesc, string sPassword, 
                                    bool bSendLowBalWarn, int iBalWarnLmt, string sBalWarnEmail, 
                                    bool bUsageRestriction, int iServiceCode, 
                                    string sWorkingDayStart, string sWorkingDayFinish, string sNonWorkingDayStart, string sNonWorkingDayFinish,
                                    bool bPayPal, bool bInvoice, string sUpdatedBy)
        {
            string sEncryptedPassword = CommonFunctions.Encrypt(sPassword);

            bool bIsUpdated=iRepository.UpdateMPAccount(iAccountID, bIsEnabled, sAccLogin, sDesc, sEncryptedPassword, 
                                               bSendLowBalWarn, iBalWarnLmt, sBalWarnEmail, 
                                               bUsageRestriction, iServiceCode, 
                                               sWorkingDayStart, sWorkingDayFinish, sNonWorkingDayStart, sNonWorkingDayFinish,
                                               bPayPal, bInvoice, sUpdatedBy);
            return bIsUpdated;
        }
        #endregion
        
        #region UserPortal Micro Payment Account
        public List<MPAccountDTO> GetMPAccountsByOrganisationIDForUserPortal(int iOrgID)
        {
            List<MPAccountDTO> lstMPAccount = iRepository.GetMPAccountsByOrganisationIDForUserPortal(iOrgID);
            return lstMPAccount;
        }

        public bool AddMPAccountForUserPortal(bool bIsEnable, string sAccLogin, string sDesc, string sPassword, int iOrgID,
                                 bool bSendLowBalWarn, int iBalWarnLmt, string sBalWarnEmail,
                                 bool bUsageRestriction, int iServiceCode,
                                 string sWorkingDayStart, string sWorkingDayFinish, string sNonWorkingDayStart, string sNonWorkingDayFinish,
                                 bool bPayPal, bool bInvoice, string sCreatedBy, string sUpdatedBy)
        {
            string sEncryptedPassword = CommonFunctions.Encrypt(sPassword);
            bool bIsAdded = iRepository.AddMPAccountForUserPortal(bIsEnable, sAccLogin, sDesc, sEncryptedPassword, iOrgID,
                                                     bSendLowBalWarn, iBalWarnLmt, sBalWarnEmail,
                                                     bUsageRestriction, iServiceCode, sWorkingDayStart,
                                                     sWorkingDayFinish, sNonWorkingDayStart, sNonWorkingDayFinish,
                                                     bPayPal, bInvoice, sCreatedBy, sUpdatedBy);
            return bIsAdded;
        }

        public MPAccountDTO GetMPAccountByIDForUserPortal(int iAccountID)
        {
            MPAccountDTO objMPAccountDTO = iRepository.GetMPAccountByIDForUserPortal(iAccountID);
            objMPAccountDTO.Password = CommonFunctions.Decrypt(objMPAccountDTO.Password);
            return objMPAccountDTO;
        }

        public bool UpdateMPAccountForUserPortal(int iAccountID, bool bIsEnabled, string sAccLogin, string sDesc, string sPassword,
                                    bool bSendLowBalWarn, int iBalWarnLmt, string sBalWarnEmail,
                                    bool bUsageRestriction, int iServiceCode,
                                    string sWorkingDayStart, string sWorkingDayFinish, string sNonWorkingDayStart, string sNonWorkingDayFinish,
                                    bool bPayPal, bool bInvoice, string sUpdatedBy)
        {
            string sEncryptedPassword = CommonFunctions.Encrypt(sPassword);

            bool bIsUpdated = iRepository.UpdateMPAccountForUserPortal(iAccountID, bIsEnabled, sAccLogin, sDesc, sEncryptedPassword,
                                               bSendLowBalWarn, iBalWarnLmt, sBalWarnEmail,
                                               bUsageRestriction, iServiceCode,
                                               sWorkingDayStart, sWorkingDayFinish, sNonWorkingDayStart, sNonWorkingDayFinish,
                                               bPayPal, bInvoice, sUpdatedBy);
            return bIsUpdated;
        }
        #endregion

        //--------------------------------------------Testing Code--------------------------------------
        #region Function to test Invoice Generation Locally

        IInvoiceRepository iInvoiceRepository = new InvoiceDAL();

        public List<string> ImportInvoices()
        {
            List<string> lstInvoiceFile = new List<string>();
            string HEADERLINE = string.Empty;
            string GLLINE = string.Empty;
            string EXTRALINE = string.Empty;
            string FOOTERLINE = string.Empty;

            string CurrentDate = DateTime.Now.Day.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString();

            List<InvoiceDTO> lstOrgToInvoice = iInvoiceRepository.GetOrganisationsToInvoice();
            List<int> lstBatch = new List<int>();

            #region Create Invoices

            foreach (InvoiceDTO objOrgToInvoice in lstOrgToInvoice)
            {
                #region Commented Query
                /*select distinct OrgOpenAccountID
                    from tblOrganisation 
                    where OrgOpenAccountID in (
                    select OrgOpenAccountID 
                    from tblMicropaymentTransactions mpt
                    join tblMPAccount mpact on mpt.MicropaymentAccountID = mpact.MPAccountCode
                    join tblOrganisation org on mpact.OrganisationID = org.OrganisationID
                    where mpt.InvoiceCreated = 0)
                 */
                #endregion
                //1. Create the Footer Line. Add it to CSV
                //2. Get all pending Invoice transactions.
                //3. Update Total Amount. Create a pair of GL & Extra Lines for each Topup. Add it to CSV. Add MP Act ID to batch. 
                //4. Create Header Line - (Done after adding GL & Extra Lines to calculate sum after conversion to Customer's currency)
                //5. Reverse the list 
                //6. Update Batch Status & Add the Batch Number to the FRONT

                #region Step 1: Create the Footer Line. Add it to CSV

                FOOTERLINE = string.Format("X|HSINV|||||||||||||||||||||YES|NO");
                lstInvoiceFile.Add(FOOTERLINE);

                #endregion

                //Step 2
                List<TransactionDTO> lstTransactions = iInvoiceRepository.GetTransactionsToInvoice(objOrgToInvoice.OrgOpenAccountID);
                decimal dTotalAmount = 0;

                #region Step 3: Create a pair of GL & Extra Lines for each Topup. Add it to CSV. Add MP Act ID to batch.

                BaseCompanyDTO objBaseCompany = iInvoiceRepository.GetBaseCompanyDetails(objOrgToInvoice.BaseCompanyID);
                string strActivityCode = iInvoiceRepository.GetActivityCode(objOrgToInvoice.CompanyID, objOrgToInvoice.BaseCompanyID);

                float fConversionRate = 1;
                if (objOrgToInvoice.InvoiceCurrency != "GBP")
                {
                    fConversionRate = iInvoiceRepository.GetGPBToCustCurrConversionRate(objOrgToInvoice.CompanyID, objOrgToInvoice.InvoiceCurrency);
                }

                foreach (TransactionDTO objTran in lstTransactions)
                {
                    decimal dTopupAmtInCustomerCurrency = objTran.TopupAmount * Convert.ToDecimal(fConversionRate);
                    dTotalAmount += dTopupAmtInCustomerCurrency;

                    #region GL Line
                    /*----------------------------------------------
                 
                     Argument Number    Field No        Field Value
                     ---------------    --------        -----------
                      0                 2               CostCode(Ex: 155-33)
                      1                 3               ExpenseCode(Ex: 1-060-04)
                      2,3               4               ProjectCode(Ex: Customer ID-ProjectCode: 8509-50001
                      4                 5               ActivityCode(Ex: 1-501-5150)
                      5                 6               Topup Amount after Currency conversion
                      6                 7               VAT Code
                    ----------------------------------------------*/
                    string CustomerID = objOrgToInvoice.OrgOpenAccountID.ToString();
                    if (CustomerID.Length > 4)
                        CustomerID = CustomerID.Substring(CustomerID.Length - 4);

                    GLLINE = string.Format("N|{0}|{1}|{2}-{3}|{4}|{5}|{6}|SMS Topup Support||||||||||||||||",
                        objBaseCompany.CostCode,
                        objBaseCompany.ExpenseCode,
                        CustomerID, objOrgToInvoice.ProjectCode,
                        strActivityCode,
                        dTopupAmtInCustomerCurrency.ToString("F2"),
                        objOrgToInvoice.VATCode);

                    #endregion

                    #region Extra Line

                    /*----------------------------------------------
                 
                     Argument Number    Field No        Field Value
                     ---------------    --------        -----------
                      0                 4               VAT Code
                      1                 5               Topup Amount(In GBP, ie Without conversion)
                      2                 20              Topup Amount
                      3                 22              Topup Amount
                    ----------------------------------------------*/

                    EXTRALINE = string.Format("X|DSINV||{0}|Top-Up {1}GBP|||||||||||||||{2}|1|{2}||",
                        objOrgToInvoice.VATCode,
                        objTran.TopupAmount.ToString("F2"),
                        dTopupAmtInCustomerCurrency.ToString("F2"));

                    #endregion

                    lstInvoiceFile.Add(EXTRALINE);
                    lstInvoiceFile.Add(GLLINE);

                    lstBatch.Add(objTran.ID);
                }

                #endregion

                #region Step 4: Create the Header Line and Add it to CSV

                int PostingYear = 0, PostingMonth = 0;

                iInvoiceRepository.GetInvoicePostingInfo(objOrgToInvoice.CompanyID, out PostingYear, out PostingMonth);

                /*----------------------------------------------
                 
                 Argument Number    Field No        Field Value
                 ---------------    --------        -----------
                  0                 2               CompanyID
                  1                 5               InvoiceCurrency
                  2                 7               Summation
                  3                 8               OrgOpenAccountID
                  4                 9               Posting Period - Month
                  5                 10              Posting Year - Year
                  6                 11              Date the interface file to OA is created - dd/MM/yyyy format
                 
                ----------------------------------------------*/

                HEADERLINE = string.Format("H|{0}|ARPAYG|PAYG Support|{1}||{2}|{3}|{4}|{5}|{6}|S||||||||||||",
                                            objOrgToInvoice.CompanyID, objOrgToInvoice.InvoiceCurrency, dTotalAmount.ToString("F2"),
                                            objOrgToInvoice.OrgOpenAccountID.ToString("D6"), PostingMonth, PostingYear, CurrentDate);

                lstInvoiceFile.Add(HEADERLINE);


                #endregion

            }

            #endregion

            //Step 5
            lstInvoiceFile.Reverse();

            #region Step 6: Update Batch Status & Add the Batch Number to the FRONT

            if (lstBatch.Count > 0)
            {

                string strBatchNumber;
                bool bBatchCreated = iInvoiceRepository.UpdateBatchSentInvoiceRequest(lstBatch, out strBatchNumber);

                if (bBatchCreated)
                {
                    lstInvoiceFile.Insert(0, strBatchNumber);
                    return lstInvoiceFile;
                }
            }

            #endregion

            return new List<string>();

        }

        public bool UpdateBatchInvoiceAcknowledged(string strBatchNumber)
        {
            return iInvoiceRepository.UpdateBatchInvoiceAcknowledged(strBatchNumber);
        }

        #endregion
    }
}
