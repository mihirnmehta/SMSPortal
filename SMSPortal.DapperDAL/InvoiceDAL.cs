using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Dapper;

using SMSPortal.Models;
using SMSPortal.Repository;

namespace SMSPortal.DapperDAL
{
    public class InvoiceDAL : IInvoiceRepository
    {

        public List<InvoiceDTO> GetOrganisationsToInvoice()
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                string sqlquery = string.Format(@"SELECT org.OrgOpenAccountID, org.CompanyID, com.ProjectCode, org.BaseCompanyID, org.VATCode, org.InvoiceCurrency
						                        FROM tblMicropaymentTransactions mpt
						                        JOIN tblMPAccount mpact on mpt.MicropaymentAccountID = mpact.MPAccountCode
						                        JOIN tblOrganisation org on mpact.OrganisationID = org.OrganisationID
                                                JOIN OACompany com on org.CompanyID = com.ID	
						                        WHERE mpt.InvoiceStatus = '{0}'
						                        GROUP BY org.OrgOpenAccountID, org.CompanyID, org.BaseCompanyID, org.VATCode, org.InvoiceCurrency, com.ProjectCode", InvoiceStatus.InvoicePending);

                List<InvoiceDTO> lstOrgToInvoice = (List<InvoiceDTO>)conn.Query<InvoiceDTO>(sqlquery);

                return lstOrgToInvoice;
            }
        }

        public List<TransactionDTO> GetTransactionsToInvoice(int iOrgOpenAccountID)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                string strQuery = string.Format(@"SELECT mpt.ID, mpt.MicropaymentAccountID as MPAccountCode, mpt.Amount as TopupAmount, mpt.TransactionDate
                                                    FROM tblMicropaymentTransactions mpt
                                                    JOIN tblMPAccount mp 
	                                                    ON mpt.MicropaymentAccountID = mp.MPAccountCode
                                                    JOIN tblOrganisation org
	                                                    ON mp.OrganisationID = org.OrganisationID
                                                    WHERE org.OrgOpenAccountID = {0}
                                                        AND mpt.PaymentMethodID = 2
                                                        AND mpt.InvoiceStatus = '{1}'
                                                    ORDER BY TransactionDate DESC", iOrgOpenAccountID.ToString(), InvoiceStatus.InvoicePending);

                List<TransactionDTO> lstTrans = (List<TransactionDTO>)conn.Query<TransactionDTO>(strQuery);
                return lstTrans;
            }
        }

        public List<string> GetListOfCompanies()
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                string sqlquery = "select distinct ID  from OACompany";

                List<string> lstCompanies = (List<string>)conn.Query<string>(sqlquery);

                return lstCompanies;
            }

        }

        public BaseCompanyDTO GetBaseCompanyDetails(string sBaseCompanyID)
        {
            BaseCompanyDTO objBaseCompany = new BaseCompanyDTO();

            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                string sqlquery = string.Format(@"SELECT BaseCompanyID, CostCode, ExpenseCode
                                                  FROM OABaseCompany
                                                  WHERE BaseCompanyID = '{0}'", sBaseCompanyID);                                                  

                objBaseCompany = (BaseCompanyDTO)connection.Query<BaseCompanyDTO>(sqlquery).Single();

            }

            return objBaseCompany;
        }

        public string GetActivityCode(int iCompanyID, string sBaseCompanyID)
        {
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                string sqlquery = string.Format(@"SELECT ActivityCode
                                                    FROM OAActivityCode
                                                    WHERE CompanyID = {0} and BaseCompanyID = '{1}'", iCompanyID, sBaseCompanyID);


                string strActivityCode = connection.Query<string>(sqlquery).SingleOrDefault();
                return strActivityCode;
            }
        }

        public void GetInvoicePostingInfo(int iCompanyID, out int PostingYear, out int PostingMonth)
         {
             //Because the Invoice creation time is to be used for 'Posting Year' & 'Posting Period' 
             int iCurrentDay        = DateTime.Now.Day;
             int iCurrentMonth      = DateTime.Now.Month; 
             int iCurrentYear       = DateTime.Now.Year;
             
             string DateToSearch        = string.Empty; // => MM/dd/yyyy format

             #region Construct the date to search
           
            if (iCurrentMonth == 1 || iCurrentMonth == 3 || iCurrentMonth == 5 || iCurrentMonth == 7 || iCurrentMonth == 8 || iCurrentMonth == 10 || iCurrentMonth == 12)
                DateToSearch = iCurrentMonth.ToString("D2") + "/31/" + iCurrentYear.ToString();
                        
            else if( iCurrentMonth == 4 || iCurrentMonth == 6 || iCurrentMonth == 9 || iCurrentMonth == 11)
                DateToSearch = iCurrentMonth.ToString("D2") + "/30/" + iCurrentYear.ToString();

            else if(iCurrentMonth == 2)
                {
                    if(iCurrentYear % 4 == 0)
                        DateToSearch = "02/29/" + iCurrentYear.ToString();
                    else
                        DateToSearch = "02/28/" + iCurrentYear.ToString();
                }

             #endregion
             
             using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
                {                
                    string sqlquery = string.Format(@"SELECT PYear, PDates
                                                      FROM OAPeriod
                                                      WHERE IsDeletedFromOA = 0 
                                                            AND CompanyID = {0}
                                                            AND PDates like '%{1}%'", iCompanyID, DateToSearch);

                   // select PYear, PDates from OAPeriod where CompanyID = 350 and PDates like '%12/31/2013%'

                    OAPeriodDTO objPeriodDTO = (OAPeriodDTO)connection.Query<OAPeriodDTO>(sqlquery).Single();

                    string[] arrMonths = objPeriodDTO.PDates.Split(';');
                    int x = 0;
                    for (x = 0; x < arrMonths.Length; x++)
                    {
                        if (arrMonths[x] == DateToSearch)
                        {
                            PostingMonth = x + 1;
                            break;
                        }
                    }

                    PostingMonth = x + 1;
                    PostingYear = objPeriodDTO.PYear;
                }

         }

        public float GetGPBToCustCurrConversionRate(int iCompanyID, string strCustomerCurrencyCode)
        {           
            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                /* This query will also work
                string sqlquery = string.Format(@"SELECT  TOP(1) CompanyID, Currency, EffectiveDate, ConversionRate, IsDeletedFromOA  
                                                  FROM OACurrency
                                                  WHERE CompanyID = {0} 
                                                      AND Currency = 'GBP' A
                                                      AND IsDeletedFromOA = 0
                                                  ORDER BY EffectiveDate DESC", iCompanyID);*/

                #region First get conversion rate of Pound to Company Currency

                string sqlquery = string.Format(@"SELECT CompanyID, Currency, EffectiveDate, ConversionRate, IsDeletedFromOA 
                                                    FROM OACurrency
                                                    WHERE CompanyID={0} 
                                                         AND Currency='GBP' 
                                                         AND IsDeletedFromOA = 0
                                                         AND EffectiveDate =(SELECT MAX(EffectiveDate) 
                                                    FROM OACurrency 
                                                    WHERE CompanyID={0} 
                                                        AND Currency='GBP'
                                                        AND IsDeletedFromOA = 0 
                                                    GROUP BY CompanyID, Currency)", iCompanyID);

                OACurrencyDTO objCurrencyDTO = (OACurrencyDTO)connection.Query<OACurrencyDTO>(sqlquery).Single();
                float fConversionRateA = objCurrencyDTO.ConversionRate;

                #endregion

                #region Then get conversion rate of CompanyCurrency to Customer Currency

                sqlquery = string.Format(@"SELECT CompanyID, Currency, EffectiveDate, ConversionRate, IsDeletedFromOA 
                                                    FROM OACurrency
                                                    WHERE CompanyID={0} 
                                                         AND Currency='{1}' 
                                                         AND IsDeletedFromOA = 0
                                                         AND EffectiveDate =(SELECT MAX(EffectiveDate) 
                                                    FROM OACurrency 
                                                    WHERE CompanyID={0} 
                                                        AND Currency='{1}'
                                                        AND IsDeletedFromOA = 0 
                                                    GROUP BY CompanyID, Currency)", iCompanyID, strCustomerCurrencyCode);

                objCurrencyDTO = (OACurrencyDTO)connection.Query<OACurrencyDTO>(sqlquery).Single();
                float fConversionRateB = objCurrencyDTO.ConversionRate;

                #endregion

                float fConversionRate = fConversionRateA / fConversionRateB;
                return fConversionRate;
            }
           
        }

        public bool UpdateBatchSentInvoiceRequest(List<int> lstBatch, out string strBatchNumber)
        {
            SqlConnection conn = new SqlConnection();
            strBatchNumber = string.Empty;

            try
            {
                DateTime dt = DateTime.Now;
                string sBatchDate = dt.ToString("yyyy-MM-dd HH:mm:ss");
                strBatchNumber = dt.ToString("ddMMyy") + "-" + dt.ToString("HHmmss");

                conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING);
                conn.Open();
                SqlTransaction sqlTran = conn.BeginTransaction();

                SqlCommand cmd = conn.CreateCommand();
                cmd.Transaction = sqlTran;

                foreach (int ID in lstBatch)
                {
                    cmd.CommandText = string.Format(@"UPDATE tblMicropaymentTransactions 
                                                    SET InvoiceStatus = '{0}', 
                                                        BatchNumber = '{1}'
                                                    WHERE ID = {2}", InvoiceStatus.InvoiceRequestSent, strBatchNumber, ID);
                    cmd.ExecuteNonQuery();
                }

                sqlTran.Commit();
                return true;
            }
            catch (Exception ex)
            {                
                return false;
            }
            finally
            {
                if(conn.State == ConnectionState.Open)
                    conn.Close();
            }

        }

        public bool UpdateBatchInvoiceAcknowledged(string strBatchNumber)
        {
            SqlConnection conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING);
            try
            {                
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = string.Format(@"UPDATE tblMicropaymentTransactions 
                                                SET InvoiceStatus = '{0}',
                                                    InvoiceStatusDate = GETDATE() 
                                                WHERE BatchNumber = '{1}'", InvoiceStatus.InvoiceCreated, strBatchNumber);
                cmd.ExecuteNonQuery();
                conn.Close();
                return true;                                 
            }
            catch (Exception ex)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                return false;
            }            
        }
        
    }
}
