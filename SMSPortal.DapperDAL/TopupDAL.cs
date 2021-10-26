using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;

using Dapper;

using SMSPortal.Models;
using SMSPortal.Repository;

namespace SMSPortal.DapperDAL
{
    public class TopupDAL : ITopupRepository
    {

        //All methods are common for Admin as well as UserPortal

        #region Common Functions

        public bool RecordInvoiceTopupTransaction(int iMPActID, decimal dAmount, string strTopupCurrency, string strLoggedInUserEmail, bool bFromAdminPortal)
        {

            using (TransactionScope ts = new TransactionScope())
            {
                using (SqlConnection conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
                {
                    //DateTime dtNow = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

                    string strTxnQuery = "insert into tblMicropaymentTransactions (PaymentMethodID, MicropaymentAccountID, Amount, "
                                            + "LoggedInUserEmail, FromAdminPortal, Currency, InvoiceStatus) "
                                            + "values (@iPaymentID, @iMPActID, @dAmount, @strLoggedInUserEmail, @bFromAdminPortal, @strTopupCurrency, @sInvoiceStatus)";

                    conn.Execute(strTxnQuery, new { iPaymentID = (int)PaymentTypes.Invoice, iMPActID, dAmount, strLoggedInUserEmail, bFromAdminPortal, strTopupCurrency, sInvoiceStatus = InvoiceStatus.InvoicePending.ToString()});

                    string strUpdateBalanceQuery = @"UPDATE tblMPAccount 
                                                    SET Balance=Balance + @dAmount, UpdatedBy=@strLoggedInUserEmail 
                                                    WHERE MPAccountCode=@iMPActID";

                    int iCount = conn.Execute(strUpdateBalanceQuery, new { dAmount, strLoggedInUserEmail, iMPActID });
                    ts.Complete();

                    if (iCount == 1)
                        return true;
                    else
                        return false;
                }
            }

        }

        public bool RecordPayPalTopupTransaction(int iMPActID, decimal dAmount, decimal dFees, string strTopupCurrency, string strPayerEmail, string strPayPalTxnID, string strPayPalTxnDate, string strLoggedInUserEmail)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                using (SqlConnection conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
                {

                    if(IsIPNReceived(strPayPalTxnID))
                        return false;
                    
                    //string strIsTransactionIDUnique = "SELECT COUNT(*) AS count from tblMicropaymentTransactions where PayPalTransactionID = @strPayPalTxnID";
                    //int count = conn.Query<int>(strIsTransactionIDUnique, new { strPayPalTxnID }).Single();

                    //if (count > 0)
                    //    return false;

                    dAmount = dAmount - dFees;

                    string strTxnQuery = "INSERT INTO tblMicropaymentTransactions (PaymentMethodID, MicropaymentAccountID, Amount, "
                                            + "LoggedInUserEmail, FromAdminPortal, PayPalTransactionID, PayerEmail, Currency, TransactionDate) "
                                            + "VALUES (@iPaymentID, @iMPActID, @dAmount, @strLoggedInUserEmail, @bFromAdminPortal, @strPayPalTxnID, @strPayerEmail, @strTopupCurrency, @strPayPalTxnDate)";
                    conn.Execute(strTxnQuery, new { iPaymentID = (int)PaymentTypes.PayPal, iMPActID, dAmount, strLoggedInUserEmail, bFromAdminPortal = false, strPayPalTxnID, strPayerEmail, strTopupCurrency, strPayPalTxnDate });

                    string strUpdateBalanceQuery = @"UPDATE tblMPAccount 
                                                    SET Balance=Balance + @dAmount, UpdatedBy=@strLoggedInUserEmail 
                                                    WHERE MPAccountCode=@iMPActID";

                    conn.Execute(strUpdateBalanceQuery, new { dAmount, strLoggedInUserEmail, iMPActID });
                    //conn.Execute(strUpdateBalanceQuery);

                    ts.Complete();
                    return true;
                }
            }// ends ts

        }

        public bool IsIPNReceived(string strTransactionID)
        {
            
            string sqlTransactionIDCount = "SELECT COUNT(*) AS count from tblMicropaymentTransactions where PayPalTransactionID = @strTransactionID";

            SqlConnection conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING);
            int count = conn.Query<int>(sqlTransactionIDCount, new { strTransactionID }).Single();

            if (count > 0)
                return true;
            else
                return false;
        }

        #endregion

        #region Admin Portal

        public List<DropDownDTO> GetOrgList(int CompanyID)
        {
            List<DropDownDTO> lstOrganisation = new List<DropDownDTO>();

            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                string sqlquery = string.Format(@"SELECT org.OrganisationID as Value, OrganisationName as Text
                                                    FROM tblOrganisation org
                                                    JOIN tblOrgBillingMethod bm 
	                                                    ON org.OrganisationID = bm.OrganisationID
                                                    WHERE org.CompanyID=@CompanyID AND
                                                          org.IsSetup=1 AND 
                                                          bm.BillingMethodID = 2
                                                    ORDER BY OrganisationName");

                lstOrganisation = (List<DropDownDTO>)connection.Query<DropDownDTO>(sqlquery, new { CompanyID });
            }
            return lstOrganisation;
        }

        public List<DropDownDTO> GetMPActListByOrg(int iOrganisationID)
        {
            List<DropDownDTO> lstMPAccount = new List<DropDownDTO>();

            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                string sqlquery = string.Format(@"SELECT mpacc.MPAccountCode as Value, mpacc.Description as Text,  convert(numeric(18,2),mpacc.Balance) AS  Attribute 
                                                  FROM tblMPAccount mpacc
												  join tblMPAccountBillingMethod mpbm on mpacc.MPAccountCode = mpbm.MPAccountCode
                                                  WHERE 
                                                        mpacc.IsEnabled = 1 
                                                        AND 
                                                        mpacc.OrganisationID=@iOrganisationID
														AND
														mpbm.BillingMethodID = 2
                                                  ORDER BY 
                                                        mpacc.Description");

                lstMPAccount = (List<DropDownDTO>)connection.Query<DropDownDTO>(sqlquery, new { iOrganisationID });
            }
            return lstMPAccount;
        }

        #endregion

        #region User Portal

        public List<DropDownDTO> GetPaymentTypesForMPAct(int iMPActID)
        {
            List<DropDownDTO> lstBillingMethod = new List<DropDownDTO>();

            using (var conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                conn.Open();
                string sqlQuery = @"SELECT actbm.BillingMethodID as Value, bm.BillingMethodName as Text
                                    FROM 
                                        tblMPAccountBillingMethod actbm
                                    JOIN
                                        tblBillingMethod  bm 
                                    ON 
                                        actbm.BillingMethodID = bm.BillingMethodID
                                    WHERE 
                                        actbm.MPAccountCode = @iMPActID";

                lstBillingMethod = (List<DropDownDTO>)conn.Query<DropDownDTO>(sqlQuery, new { iMPActID });

                conn.Close();
            }
            return lstBillingMethod;
        }

        public List<DropDownDTO> GetAllMPAccountsByOrg(int iOrganisationID)
        {
            List<DropDownDTO> lstMPAccount = new List<DropDownDTO>();

            using (var connection = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                string sqlquery = string.Format(@"SELECT mpacc.MPAccountCode as Value, mpacc.Description as Text,  convert(numeric(18,2),mpacc.Balance) AS  Attribute 
                                                  FROM tblMPAccount mpacc
                                                  WHERE 
                                                        mpacc.IsEnabled = 1 
                                                        AND 
                                                        mpacc.OrganisationID=@iOrganisationID
                                                  ORDER BY 
                                                        mpacc.Description");

                lstMPAccount = (List<DropDownDTO>)connection.Query<DropDownDTO>(sqlquery, new { iOrganisationID });
            }
            return lstMPAccount;
        }


        #endregion
    }
}
