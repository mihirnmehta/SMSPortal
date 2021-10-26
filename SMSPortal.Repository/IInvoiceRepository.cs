using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SMSPortal.Models;

namespace SMSPortal.Repository
{
    public interface IInvoiceRepository
    {
        List<InvoiceDTO> GetOrganisationsToInvoice();

        /// <summary>
        /// Gets Transactions to Invoice, based on CustomerID in ascending order of Transaction date, whose InvoiceStatus='InvoicePending'
        /// </summary>
        /// <param name="iOrgOpenAccountID">CustomerID of Organisation in OA</param>
        /// <returns></returns>
        List<TransactionDTO> GetTransactionsToInvoice(int iOrgOpenAccountID);

        List<string> GetListOfCompanies();

        /// <summary>
        /// Gets Base Company details: Cost Code, ExpenseCode, ProjectCode, ActivityCode
        /// </summary>
        /// <param name="sBaseCompanyID"></param>
        /// <returns></returns>
        BaseCompanyDTO GetBaseCompanyDetails(string sBaseCompanyID);

        string GetActivityCode(int iCompanyID, string sBaseCompanyID);
        
        void GetInvoicePostingInfo(int iCompanyID, out int PostingYear, out int PostingMonth);

        float GetGPBToCustCurrConversionRate(int iCompanyID, string strCustomerCurrencyCode);

        bool UpdateBatchSentInvoiceRequest(List<int> lstBatch, out string strBatchNumber);

        bool UpdateBatchInvoiceAcknowledged(string BatchNumber);
        

    }
}
