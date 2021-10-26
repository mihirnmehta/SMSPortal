using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Transactions;

using Dapper;

using SMSPortal.Models;
using SMSPortal.Repository.Organisation;
using SMSPortal.DapperDAL;

namespace SMSPortal.DapperDAL.Organisation
{
    public class MPAccountDAL : IMPAccountRepository
    {
        SqlConnection conn;

        public MPAccountDAL()
        {
            conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING);
        }

        #region Common Functions

        /// <summary>
        /// Checks whether Username is unique while adding MicroPayment account 
        /// </summary>
        /// <param name="strUsername">Username = AccountLogin</param>
        /// <returns>Returns true if username is unique else false</returns>
        public bool IsUsernameUnique(string strUsername)
        {
            bool bResult = false;

            conn.Open();

            string sqlquery = "SELECT COUNT(*) AS count from tblMPAccount where AccountLogin = @strUsername";

            int count = conn.Query<int>(sqlquery, new { strUsername }).Single();
            conn.Close();

            if (count == 0)
                bResult = true;
            else
                bResult = false;
            return bResult;
        }

        /// <summary>
        /// Checks whether Username is unique while updating MicroPayment account
        /// (Execute unique check with current record)
        /// </summary>
        /// <param name="strUsername">Username = AccountLogin</param>
        /// <param name="iAccountID">AccountId of current editing record</param>
        /// <returns>Returns true if username is unique else false</returns>
        public bool IsUsernameUniqueOnUpdate(string strUsername, int iAccountID)
        {
            bool bResult = false;

            conn.Open();
            
            string sqlquery = "SELECT COUNT(*) AS count from tblMPAccount where AccountLogin = @strUsername and MPAccountCode != @iAccountID";
            int count = conn.Query<int>(sqlquery, new { strUsername, iAccountID }).Single();
            
            conn.Close();
            
            if (count == 0)
                bResult = true;
            else
                bResult = false;
            return bResult;
        }

        /// <summary>
        /// Gets BillingMethods allowed by Organisation of created MPAccount
        /// </summary>
        /// <param name="iOrgID">ID of Organisation under which MPAccount is created</param>
        /// <returns>BillingMethods allowed by Organisation</returns>
        public BillingMethodDTO GetOrgBillingMethods(int iOrgID)
        {
            BillingMethodDTO objBillingMethodDTO = null;
            conn.Open();
            string sqlQuery = (@"SELECT
	                                            CASE 
		                                        WHEN( 
				                                        SELECT COUNT(*) FROM tblOrgBillingMethod obm WHERE obm.BillingMethodID = 1 AND obm.OrganisationID = @iOrgID
			                                        ) = 0 
			                                        THEN CAST(0 AS BIT) 
			                                        ELSE CAST(1 AS BIT) 
			                                        END AS Paypal,
		                                        CASE 
		                                        WHEN( 
				                                        SELECT COUNT(*) FROM tblOrgBillingMethod obm WHERE obm.BillingMethodID = 2 AND obm.OrganisationID = @iOrgID
			                                        ) = 0 
			                                        THEN CAST(0 AS BIT) 
			                                        ELSE CAST(1 AS BIT) 
			                                        END AS Invoice");

            objBillingMethodDTO = conn.Query<BillingMethodDTO>(sqlQuery, new { iOrgID }).FirstOrDefault();
            conn.Close();
            return objBillingMethodDTO;
        }

        #endregion

        #region AdminPortal Micro Payment Account
        public List<MPAccountDTO> GetMPAccountsByOrganisationID(int iOrgID)
        {
            List<MPAccountDTO> mpAccountList = null;
            conn.Open();

            #region Query to Fetch MPAccount Code, Description, WarningLevel and List of PaymentMethods
            string sqlquery = (@"SELECT 
                                                    mpacc.MPAccountCode,
                                                    mpacc.[Description], 
                                                    convert(numeric(18,2),mpacc.Balance) AS Balance,
                                                    mpacc.SendLowBalanceWarnings, 
                                                    mpacc.BalanceWarningLimit,
    
                                                case 
	                                                when( 
			                                                (
				                                                select count(*) from tblMPAccountBillingMethod mpbm 
				                                                where 
					                                                mpacc.MPAccountCode = mpbm.MPAccountCode and mpbm.BillingMethodID = @iPaypal
			                                                )
		                                                ) = 0 
		                                                then 
			                                                CAST(0 AS BIT) 
		                                                else 
			                                                CAST(1 AS BIT) 
		                                                end as PayPal, 
                                                case 
	                                                when((
				                                                select count(*) from tblMPAccountBillingMethod mpbm 
				                                                where 
					                                                mpacc.MPAccountCode = mpbm.MPAccountCode and mpbm.BillingMethodID = @iInvoice
		                                                )) = 0 
		                                                then 
			                                                CAST(0 AS BIT) 
		                                                else 
			                                                CAST(1 AS BIT) 
		                                                end as Invoice,
                                                    mpacc.IsEnabled
                                                FROM 
                                                    tblMPAccount mpacc 
                                                where  OrganisationID = @iOrgID");
            #endregion

            mpAccountList = (List<MPAccountDTO>)conn.Query<MPAccountDTO>(sqlquery, new { iPaypal = Convert.ToInt32(PaymentTypes.PayPal), iInvoice = Convert.ToInt32(PaymentTypes.Invoice), iOrgID });
            conn.Close();
            return mpAccountList;
        }

        public bool AddMPAccount(bool bIsEnable, string sAccLogin, string sDesc, string sEncryptedPassword, int iOrgID,
                                 bool bSendLowBalWarn, int iBalWarnLmt, string sBalWarnEmail,
                                 bool bUsageRestriction, int iServiceCode, string sWorkingDayStart, string sWorkingDayFinish,
                                 string sNonWorkingDayStart, string sNonWorkingDayFinish,
                                 bool bPayPal, bool bInvoice, string sCreatedBy, string sUpdatedBy)
        {
            //1. Insert MPAccount basic details in tblMPAccount Table
            //2. Insert the selected Payments types in tblMPAccountBillingMethod Table
            //3. Insert the Usage restriction (SMS sending and recieving duration) in tblAccountServicePerms Table which will be used by CASSIA

            bool bIsAdded = false;

            using (var transactionScope = new TransactionScope())
            {
                conn.Open();
                #region Query to Insert MPAccount basic details in tblMPAccount Table
                string sqlQuery = string.Format(
                                @"INSERT INTO tblMPAccount
                                (
                                    AccountLogin,
	                                [Description],
	                                EncryptedPassword,
	                                OrganisationID,
	                                SendLowBalanceWarnings,
	                                BalanceWarningLimit,
	                                BalanceWarningEmail,
                                    IsEnabled,
                                    CreatedBy,
                                    UpdatedBy
                                    )
                                    VALUES 
                                    ( @sAccLogin, @sDesc, @sEncryptedPassword, @iOrgID, @iSendLowBalWarn, @iBalWarnLmt, @sBalWarnEmail, @iIsEnable, @sCreatedBy, @sUpdatedBy);
                                    SELECT CAST(SCOPE_IDENTITY() as int);");
                #endregion

                //returns MPAccountCode of the inserted record on execution
                int iMPAccountCode = conn.Query<int>(sqlQuery, new { sAccLogin, sDesc, sEncryptedPassword, iOrgID, iSendLowBalWarn = bSendLowBalWarn ? 1 : 0, iBalWarnLmt, sBalWarnEmail, iIsEnable = bIsEnable ? 1 : 0, sCreatedBy, sUpdatedBy }).Single();

                string sqlInsertBillingMethod = string.Empty;

                #region Query to Insert the selected Payments types in tblMPAccountBillingMethod Table
                if (bPayPal)
                {
                    sqlInsertBillingMethod
                        = string.Format("INSERT INTO tblMPAccountBillingMethod (MPAccountCode, BillingMethodID) VALUES(@iMPAccountCode, @iPaypal)");

                    conn.Execute(sqlInsertBillingMethod, new { iMPAccountCode, iPaypal = Convert.ToInt32(PaymentTypes.PayPal) });
                }

                if (bInvoice)
                {
                    sqlInsertBillingMethod
                        = string.Format("INSERT INTO tblMPAccountBillingMethod (MPAccountCode, BillingMethodID) VALUES(@iMPAccountCode, @iInvoice)");
                    conn.Execute(sqlInsertBillingMethod, new { iMPAccountCode, iInvoice = Convert.ToInt32(PaymentTypes.Invoice) });
                }
                #endregion

                if (bUsageRestriction)
                {
                    #region Query to Insert the Usage restriction (SMS sending and recieving duration) in tblAccountServicePerms Table which will be used by CASSIA
                    sqlQuery = (@"INSERT INTO tblAccountServicePerms
	                                (
		                                MPAccountCode,
		                                ServiceCode,
		                                WorkDayAllowedFrom,
		                                WorkDayAllowedTo,
		                                NonWorkDayAllowedFrom,
		                                NonWorkDayAllowedTo
	                                )
	                                VALUES
	                                (
                                        @iMPAccountCode,
                                        @iServiceCode,
                                        CAST(@sWorkingDayStart AS DATETIME),
                                        CAST(@sWorkingDayFinish AS DATETIME),
                                        CAST(@sNonWorkingDayStart AS DATETIME),
                                        CAST(@sNonWorkingDayFinish AS DATETIME))"
                                    );
                    #endregion
                    conn.Execute(sqlQuery, new { iMPAccountCode, iServiceCode, sWorkingDayStart, sWorkingDayFinish, sNonWorkingDayStart, sNonWorkingDayFinish });
                }

                conn.Close();

                transactionScope.Complete();
                bIsAdded = true;
            }


            return bIsAdded;
        }

        public MPAccountDTO GetMPAccountByID(int iAccountID)
        {
            MPAccountDTO objMPAccountDTO = null;
            conn.Open();

            #region Query to Fetch record of particular MPAccountCode from tblMPAccount, tblMPAccountBillingMethod and tblAccountServicePerms
            string sqlQuery = (@"SELECT 
                                    mpacc.MPAccountCode,
                                    mpacc.IsEnabled,
                                    mpacc.AccountLogin,
                                    mpacc.[Description],
                                    convert(numeric(18,2),mpacc.Balance) AS Balance,
                                    mpacc.EncryptedPassword AS Password,
                                    mpacc.OrganisationID,
                                    mpacc.SendLowBalanceWarnings,
                                    mpacc.BalanceWarningLimit,
                                    mpacc.BalanceWarningEmail,
                                    left(CONVERT(VARCHAR(8),acPerm.WorkDayAllowedFrom,108),5) AS WorkDayAllowedFrom,
                                    left(CONVERT(VARCHAR(8),acPerm.WorkDayAllowedTo,108),5) AS WorkDayAllowedTo,
                                    left(CONVERT(VARCHAR(8),acPerm.NonWorkDayAllowedFrom,108),5) AS NonWorkDayAllowedFrom,
                                    left(CONVERT(VARCHAR(8),acPerm.NonWorkDayAllowedTo,108),5) AS NonWorkDayAllowedTo,

                                case 
	                                when( 
			                                (
				                                SELECT COUNT(*) FROM tblMPAccountBillingMethod mpbm 
				                                WHERE 
					                                mpacc.MPAccountCode = mpbm.MPAccountCode AND mpbm.BillingMethodID = @iPaypal
			                                )
		                                ) = 0 
		                                then 
			                                CAST(0 AS BIT) 
		                                else 
			                                CAST(1 AS BIT) 
		                                end as PayPal, 
                                case 
	                                when((
				                                SELECT COUNT(*) FROM tblMPAccountBillingMethod mpbm 
				                                WHERE 
					                                mpacc.MPAccountCode = mpbm.MPAccountCode AND mpbm.BillingMethodID = @iInvoice
		                                )) = 0 
		                                then 
			                                CAST(0 AS BIT) 
		                                else 
			                                CAST(1 AS BIT) 
		                                end as Invoice     
                                FROM 
                                    tblMPAccount mpacc 
                                LEFT JOIN 
	                                tblAccountServicePerms acPerm
                                ON
	                                mpacc.MPAccountCode=acPerm.MPAccountCode    
                                WHERE  mpacc.MPAccountCode = @iAccountID");
            #endregion

            objMPAccountDTO = conn.Query<MPAccountDTO>(sqlQuery, new { iPaypal = Convert.ToInt32(PaymentTypes.PayPal), iInvoice = Convert.ToInt32(PaymentTypes.Invoice), iAccountID }).SingleOrDefault();

            if (objMPAccountDTO.WorkDayAllowedFrom != null)
            {
                objMPAccountDTO.UsageRestriction = true;
            }

            conn.Close();
            return objMPAccountDTO;
        }

        public bool UpdateMPAccount(int iMPAccountCode, bool bIsEnabled, string sAccLogin, string sDesc, string sEncryptedPassword,
                                    bool bSendLowBalWarn, int iBalWarnLmt, string sBalWarnEmail,
                                    bool iUsageRestriction, int iServiceCode, string sWorkingDayStart, string sWorkingDayFinish,
                                    string sNonWorkingDayStart, string sNonWorkingDayFinish, bool bPayPal, bool bInvoice, string sUpdatedBy)
        {
            //1. Update MPAccount basic details in tblMPAccount Table
            //2. Update the selected Payments types in tblMPAccountBillingMethod Table
            //3. Update the Usage restriction (SMS sending and recieving duration) in tblAccountServicePerms Table which will be used by CASSIA

            bool bIsUpdated = false;

            using (var transactionScope = new TransactionScope())
            {
                conn.Open();

                #region Query to Update tblMPAccount
                string sqlQuery = (@"UPDATE tblMPAccount 
                                        SET 
                                            IsEnabled = @ibIsEnabled,
	                                        AccountLogin = @sAccLogin,
	                                        [Description] = @sDesc,
	                                        EncryptedPassword = @sEncryptedPassword,
	                                        SendLowBalanceWarnings = @iSendLowBalWarn,
	                                        BalanceWarningLimit = @iBalWarnLmt,
	                                        BalanceWarningEmail = @sBalWarnEmail,
                                            UpdatedBy = @sUpdatedBy,
                                            UpdatedDate = GETDATE()
                                        WHERE
	                                        MPAccountCode = @iMPAccountCode"
                                    );
                #endregion

                conn.Execute(sqlQuery, new
                {
                    ibIsEnabled = bIsEnabled ? 1 : 0,
                    sAccLogin,
                    sDesc,
                    sEncryptedPassword,
                    iSendLowBalWarn = bSendLowBalWarn ? 1 : 0,
                    iBalWarnLmt,
                    sBalWarnEmail,
                    sUpdatedBy,
                    iMPAccountCode
                });

                #region Update MPAccount Payment Methods

                //Deleting all the related billing methods from the MPAccountBilling Method table
                sqlQuery = ("DELETE FROM tblMPAccountBillingMethod WHERE MPAccountCode = @iMPAccountCode");
                conn.Execute(sqlQuery, new { iMPAccountCode });


                //Inserting the Paymentmethods whichever are checked
                string sqlInsertBillingMethod = string.Empty;
                if (bPayPal)
                {
                    sqlInsertBillingMethod
                        = ("INSERT INTO tblMPAccountBillingMethod (MPAccountCode, BillingMethodID) VALUES(@iMPAccountCode, @iPaypal)");

                    conn.Execute(sqlInsertBillingMethod, new { iMPAccountCode, iPaypal = Convert.ToInt16(PaymentTypes.PayPal) });
                }

                if (bInvoice)
                {
                    sqlInsertBillingMethod
                        = ("INSERT INTO tblMPAccountBillingMethod (MPAccountCode, BillingMethodID) VALUES(@iMPAccountCode, @iInvoice)");
                    conn.Execute(sqlInsertBillingMethod, new { iMPAccountCode, iInvoice = Convert.ToInt16(PaymentTypes.Invoice) });
                }

                #endregion

                //Checks if the UsageRestriction is true to update or insert its values otherwise delete previous value
                if (iUsageRestriction)
                {
                    #region Update the Usage restriction (SMS sending and recieving duration) in tblAccountServicePerms Table which will be used by CASSIA

                    sqlQuery = ("Select Count(*) from tblAccountServicePerms WHERE  MPAccountCode = @iMPAccountCode");
                    bool bIsAccountServicePerms = conn.Query<int>(sqlQuery, new { iMPAccountCode }).Single() > 0 ? true : false;

                    if (bIsAccountServicePerms)
                    {
                        sqlQuery = (@"UPDATE tblAccountServicePerms
                                        SET
		                                    WorkDayAllowedFrom = CAST(@sWorkingDayStart AS DATETIME),
		                                    WorkDayAllowedTo = CAST(@sWorkingDayFinish AS DATETIME),
		                                    NonWorkDayAllowedFrom = CAST(@sNonWorkingDayStart AS DATETIME),
		                                    NonWorkDayAllowedTo = CAST(@sNonWorkingDayFinish AS DATETIME)
	                                    WHERE
                                            MPAccountCode=@iMPAccountCode"
                                    );
                        conn.Execute(sqlQuery, new { sWorkingDayStart, sWorkingDayFinish, sNonWorkingDayStart, sNonWorkingDayFinish, iMPAccountCode });
                    }
                    else
                    {
                        sqlQuery = (@"INSERT INTO tblAccountServicePerms
	                                    (
		                                    MPAccountCode,
		                                    ServiceCode,
		                                    WorkDayAllowedFrom,
		                                    WorkDayAllowedTo,
		                                    NonWorkDayAllowedFrom,
		                                    NonWorkDayAllowedTo
	                                    )
	                                    VALUES
	                                    (
                                            @iMPAccountCode,
                                            @iServiceCode,
                                            CAST(@sWorkingDayStart AS DATETIME),
                                            CAST(@sWorkingDayFinish AS DATETIME),
                                            CAST(@sNonWorkingDayStart AS DATETIME),
                                            CAST(@sNonWorkingDayFinish AS DATETIME)
                                        )"
                                    );
                        conn.Execute(sqlQuery, new { iMPAccountCode, iServiceCode, sWorkingDayStart, sWorkingDayFinish, sNonWorkingDayStart, sNonWorkingDayFinish });
                    }
                    #endregion
                }
                else
                {
                    sqlQuery = string.Format("DELETE FROM tblAccountServicePerms WHERE MPAccountCode = @iMPAccountCode");
                    conn.Execute(sqlQuery, new { iMPAccountCode });
                }

                conn.Close();

                transactionScope.Complete();

                bIsUpdated = true;
            }

            return bIsUpdated;
        }

        #endregion

        /*------------------------------------------------------*/
        /*------------------------------------------------------*/

        #region UserPortal Micro Payment Account

        public List<MPAccountDTO> GetMPAccountsByOrganisationIDForUserPortal(int iOrgID)
        {
            List<MPAccountDTO> mpAccountList = null;
            conn.Open();

            #region Query to Fetch MPAccount Code, Description, WarningLevel and List of PaymentMethods
            string sqlquery = (@"SELECT 
                                                    mpacc.MPAccountCode,
                                                    mpacc.[Description], 
                                                    convert(numeric(18,2),mpacc.Balance) AS Balance,
                                                    mpacc.SendLowBalanceWarnings, 
                                                    mpacc.BalanceWarningLimit,
    
                                                case 
	                                                when( 
			                                                (
				                                                select count(*) from tblMPAccountBillingMethod mpbm 
				                                                where 
					                                                mpacc.MPAccountCode = mpbm.MPAccountCode and mpbm.BillingMethodID = @iPaypal
			                                                )
		                                                ) = 0 
		                                                then 
			                                                CAST(0 AS BIT) 
		                                                else 
			                                                CAST(1 AS BIT) 
		                                                end as PayPal, 
                                                case 
	                                                when((
				                                                select count(*) from tblMPAccountBillingMethod mpbm 
				                                                where 
					                                                mpacc.MPAccountCode = mpbm.MPAccountCode and mpbm.BillingMethodID = @iInvoice
		                                                )) = 0 
		                                                then 
			                                                CAST(0 AS BIT) 
		                                                else 
			                                                CAST(1 AS BIT) 
		                                                end as Invoice,
                                                    mpacc.IsEnabled
                                                FROM 
                                                    tblMPAccount mpacc 
                                                where  OrganisationID = @iOrgID");
            #endregion

            mpAccountList = (List<MPAccountDTO>)conn.Query<MPAccountDTO>(sqlquery, new { iPaypal = Convert.ToInt32(PaymentTypes.PayPal), iInvoice = Convert.ToInt32(PaymentTypes.Invoice), iOrgID });
            conn.Close();
            return mpAccountList;
        }

        public bool AddMPAccountForUserPortal(bool bIsEnable, string sAccLogin, string sDesc, string sEncryptedPassword, int iOrgID,
                                 bool bSendLowBalWarn, int iBalWarnLmt, string sBalWarnEmail,
                                 bool bUsageRestriction, int iServiceCode, string sWorkingDayStart, string sWorkingDayFinish,
                                 string sNonWorkingDayStart, string sNonWorkingDayFinish,
                                 bool bPayPal, bool bInvoice, string sCreatedBy, string sUpdatedBy)
        {
            //1. Insert MPAccount basic details in tblMPAccount Table
            //2. Insert the selected Payments types in tblMPAccountBillingMethod Table
            //3. Insert the Usage restriction (SMS sending and recieving duration) in tblAccountServicePerms Table which will be used by CASSIA

            bool bIsAdded = false;

            using (var transactionScope = new TransactionScope())
            {
                conn.Open();
                #region Query to Insert MPAccount basic details in tblMPAccount Table
                string sqlQuery = string.Format(
                                @"INSERT INTO tblMPAccount
                                (
                                    AccountLogin,
	                                [Description],
	                                EncryptedPassword,
	                                OrganisationID,
	                                SendLowBalanceWarnings,
	                                BalanceWarningLimit,
	                                BalanceWarningEmail,
                                    IsEnabled,
                                    CreatedBy,
                                    UpdatedBy
                                    )
                                    VALUES 
                                    ( @sAccLogin, @sDesc, @sEncryptedPassword, @iOrgID, @iSendLowBalWarn, @iBalWarnLmt, @sBalWarnEmail, @iIsEnable, @sCreatedBy, @sUpdatedBy);
                                    SELECT CAST(SCOPE_IDENTITY() as int);");
                #endregion

                //returns MPAccountCode of the inserted record on execution
                int iMPAccountCode = conn.Query<int>(sqlQuery, new { sAccLogin, sDesc, sEncryptedPassword, iOrgID, iSendLowBalWarn = bSendLowBalWarn ? 1 : 0, iBalWarnLmt, sBalWarnEmail, iIsEnable = bIsEnable ? 1 : 0, sCreatedBy, sUpdatedBy }).Single();

                string sqlInsertBillingMethod = string.Empty;

                #region Query to Insert the selected Payments types in tblMPAccountBillingMethod Table
                if (bPayPal)
                {
                    sqlInsertBillingMethod
                        = string.Format("INSERT INTO tblMPAccountBillingMethod (MPAccountCode, BillingMethodID) VALUES(@iMPAccountCode, @iPaypal)");

                    conn.Execute(sqlInsertBillingMethod, new { iMPAccountCode, iPaypal = Convert.ToInt32(PaymentTypes.PayPal) });
                }

                if (bInvoice)
                {
                    sqlInsertBillingMethod
                        = string.Format("INSERT INTO tblMPAccountBillingMethod (MPAccountCode, BillingMethodID) VALUES(@iMPAccountCode, @iInvoice)");
                    conn.Execute(sqlInsertBillingMethod, new { iMPAccountCode, iInvoice = Convert.ToInt32(PaymentTypes.Invoice) });
                }
                #endregion

                if (bUsageRestriction)
                {
                    #region Query to Insert the Usage restriction (SMS sending and recieving duration) in tblAccountServicePerms Table which will be used by CASSIA
                    sqlQuery = (@"INSERT INTO tblAccountServicePerms
	                                (
		                                MPAccountCode,
		                                ServiceCode,
		                                WorkDayAllowedFrom,
		                                WorkDayAllowedTo,
		                                NonWorkDayAllowedFrom,
		                                NonWorkDayAllowedTo
	                                )
	                                VALUES
	                                (
                                        @iMPAccountCode,
                                        @iServiceCode,
                                        CAST(@sWorkingDayStart AS DATETIME),
                                        CAST(@sWorkingDayFinish AS DATETIME),
                                        CAST(@sNonWorkingDayStart AS DATETIME),
                                        CAST(@sNonWorkingDayFinish AS DATETIME))"
                                    );
                    #endregion
                    conn.Execute(sqlQuery, new { iMPAccountCode, iServiceCode, sWorkingDayStart, sWorkingDayFinish, sNonWorkingDayStart, sNonWorkingDayFinish });
                }

                conn.Close();

                transactionScope.Complete();
                bIsAdded = true;
            }


            return bIsAdded;
        }

        public MPAccountDTO GetMPAccountByIDForUserPortal(int iAccountID)
        {
            MPAccountDTO objMPAccountDTO = null;
            conn.Open();

            #region Query to Fetch record of particular MPAccountCode from tblMPAccount, tblMPAccountBillingMethod and tblAccountServicePerms
            string sqlQuery = (@"SELECT 
                                    mpacc.MPAccountCode,
                                    mpacc.IsEnabled,
                                    mpacc.AccountLogin,
                                    mpacc.[Description],
                                    convert(numeric(18,2),mpacc.Balance) AS Balance,
                                    mpacc.EncryptedPassword AS Password,
                                    mpacc.OrganisationID,
                                    mpacc.SendLowBalanceWarnings,
                                    mpacc.BalanceWarningLimit,
                                    mpacc.BalanceWarningEmail,
                                    left(CONVERT(VARCHAR(8),acPerm.WorkDayAllowedFrom,108),5) AS WorkDayAllowedFrom,
                                    left(CONVERT(VARCHAR(8),acPerm.WorkDayAllowedTo,108),5) AS WorkDayAllowedTo,
                                    left(CONVERT(VARCHAR(8),acPerm.NonWorkDayAllowedFrom,108),5) AS NonWorkDayAllowedFrom,
                                    left(CONVERT(VARCHAR(8),acPerm.NonWorkDayAllowedTo,108),5) AS NonWorkDayAllowedTo,

                                case 
	                                when( 
			                                (
				                                SELECT COUNT(*) FROM tblMPAccountBillingMethod mpbm 
				                                WHERE 
					                                mpacc.MPAccountCode = mpbm.MPAccountCode AND mpbm.BillingMethodID = @iPaypal
			                                )
		                                ) = 0 
		                                then 
			                                CAST(0 AS BIT) 
		                                else 
			                                CAST(1 AS BIT) 
		                                end as PayPal, 
                                case 
	                                when((
				                                SELECT COUNT(*) FROM tblMPAccountBillingMethod mpbm 
				                                WHERE 
					                                mpacc.MPAccountCode = mpbm.MPAccountCode AND mpbm.BillingMethodID = @iInvoice
		                                )) = 0 
		                                then 
			                                CAST(0 AS BIT) 
		                                else 
			                                CAST(1 AS BIT) 
		                                end as Invoice     
                                FROM 
                                    tblMPAccount mpacc 
                                LEFT JOIN 
	                                tblAccountServicePerms acPerm
                                ON
	                                mpacc.MPAccountCode=acPerm.MPAccountCode    
                                WHERE  mpacc.MPAccountCode = @iAccountID");
            #endregion

            objMPAccountDTO = conn.Query<MPAccountDTO>(sqlQuery, new { iPaypal = Convert.ToInt32(PaymentTypes.PayPal), iInvoice = Convert.ToInt32(PaymentTypes.Invoice), iAccountID }).SingleOrDefault();

            if (objMPAccountDTO.WorkDayAllowedFrom != null)
            {
                objMPAccountDTO.UsageRestriction = true;
            }

            conn.Close();
            return objMPAccountDTO;
        }

        public bool UpdateMPAccountForUserPortal(int iMPAccountCode, bool bIsEnabled, string sAccLogin, string sDesc, string sEncryptedPassword,
                                    bool bSendLowBalWarn, int iBalWarnLmt, string sBalWarnEmail,
                                    bool iUsageRestriction, int iServiceCode, string sWorkingDayStart, string sWorkingDayFinish,
                                    string sNonWorkingDayStart, string sNonWorkingDayFinish, bool bPayPal, bool bInvoice, string sUpdatedBy)
        {
            //1. Update MPAccount basic details in tblMPAccount Table
            //2. Update the selected Payments types in tblMPAccountBillingMethod Table
            //3. Update the Usage restriction (SMS sending and recieving duration) in tblAccountServicePerms Table which will be used by CASSIA

            bool bIsUpdated = false;

            using (var transactionScope = new TransactionScope())
            {
                conn.Open();

                #region Query to Update tblMPAccount
                string sqlQuery = (@"UPDATE tblMPAccount 
                                        SET 
                                            IsEnabled = @ibIsEnabled,
	                                        AccountLogin = @sAccLogin,
	                                        [Description] = @sDesc,
	                                        EncryptedPassword = @sEncryptedPassword,
	                                        SendLowBalanceWarnings = @iSendLowBalWarn,
	                                        BalanceWarningLimit = @iBalWarnLmt,
	                                        BalanceWarningEmail = @sBalWarnEmail,
                                            UpdatedBy = @sUpdatedBy,
                                            UpdatedDate = GETDATE()
                                        WHERE
	                                        MPAccountCode = @iMPAccountCode"
                                    );
                #endregion

                conn.Execute(sqlQuery, new
                {
                    ibIsEnabled = bIsEnabled ? 1 : 0,
                    sAccLogin,
                    sDesc,
                    sEncryptedPassword,
                    iSendLowBalWarn = bSendLowBalWarn ? 1 : 0,
                    iBalWarnLmt,
                    sBalWarnEmail,
                    sUpdatedBy,
                    iMPAccountCode
                });

                #region Update MPAccount Payment Methods

                //Deleting all the related billing methods from the MPAccountBilling Method table
                sqlQuery = ("DELETE FROM tblMPAccountBillingMethod WHERE MPAccountCode = @iMPAccountCode");
                conn.Execute(sqlQuery, new { iMPAccountCode });


                //Inserting the Paymentmethods whichever are checked
                string sqlInsertBillingMethod = string.Empty;
                if (bPayPal)
                {
                    sqlInsertBillingMethod
                        = ("INSERT INTO tblMPAccountBillingMethod (MPAccountCode, BillingMethodID) VALUES(@iMPAccountCode, @iPaypal)");

                    conn.Execute(sqlInsertBillingMethod, new { iMPAccountCode, iPaypal = Convert.ToInt16(PaymentTypes.PayPal) });
                }

                if (bInvoice)
                {
                    sqlInsertBillingMethod
                        = ("INSERT INTO tblMPAccountBillingMethod (MPAccountCode, BillingMethodID) VALUES(@iMPAccountCode, @iInvoice)");
                    conn.Execute(sqlInsertBillingMethod, new { iMPAccountCode, iInvoice = Convert.ToInt16(PaymentTypes.Invoice) });
                }

                #endregion

                //Checks if the UsageRestriction is true to update or insert its values otherwise delete previous value
                if (iUsageRestriction)
                {
                    #region Update the Usage restriction (SMS sending and recieving duration) in tblAccountServicePerms Table which will be used by CASSIA

                    sqlQuery = ("Select Count(*) from tblAccountServicePerms WHERE  MPAccountCode = @iMPAccountCode");
                    bool bIsAccountServicePerms = conn.Query<int>(sqlQuery, new { iMPAccountCode }).Single() > 0 ? true : false;

                    if (bIsAccountServicePerms)
                    {
                        sqlQuery = (@"UPDATE tblAccountServicePerms
                                        SET
		                                    WorkDayAllowedFrom = CAST(@sWorkingDayStart AS DATETIME),
		                                    WorkDayAllowedTo = CAST(@sWorkingDayFinish AS DATETIME),
		                                    NonWorkDayAllowedFrom = CAST(@sNonWorkingDayStart AS DATETIME),
		                                    NonWorkDayAllowedTo = CAST(@sNonWorkingDayFinish AS DATETIME)
	                                    WHERE
                                            MPAccountCode=@iMPAccountCode"
                                    );
                        conn.Execute(sqlQuery, new { sWorkingDayStart, sWorkingDayFinish, sNonWorkingDayStart, sNonWorkingDayFinish, iMPAccountCode });
                    }
                    else
                    {
                        sqlQuery = (@"INSERT INTO tblAccountServicePerms
	                                    (
		                                    MPAccountCode,
		                                    ServiceCode,
		                                    WorkDayAllowedFrom,
		                                    WorkDayAllowedTo,
		                                    NonWorkDayAllowedFrom,
		                                    NonWorkDayAllowedTo
	                                    )
	                                    VALUES
	                                    (
                                            @iMPAccountCode,
                                            @iServiceCode,
                                            CAST(@sWorkingDayStart AS DATETIME),
                                            CAST(@sWorkingDayFinish AS DATETIME),
                                            CAST(@sNonWorkingDayStart AS DATETIME),
                                            CAST(@sNonWorkingDayFinish AS DATETIME)
                                        )"
                                    );
                        conn.Execute(sqlQuery, new { iMPAccountCode, iServiceCode, sWorkingDayStart, sWorkingDayFinish, sNonWorkingDayStart, sNonWorkingDayFinish });
                    }
                    #endregion
                }
                else
                {
                    sqlQuery = string.Format("DELETE FROM tblAccountServicePerms WHERE MPAccountCode = @iMPAccountCode");
                    conn.Execute(sqlQuery, new { iMPAccountCode });
                }

                conn.Close();

                transactionScope.Complete();

                bIsUpdated = true;
            }

            return bIsUpdated;
        }
        #endregion

    }
}
