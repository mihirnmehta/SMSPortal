using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System;
using Dapper;

using SMSPortal.Repository;
using SMSPortal.Models;

namespace SMSPortal.DapperDAL
{
    public class ReportDAL : IReportRepository
    {
        SqlConnection conn, cassiaConn;

        public ReportDAL()
        {
            conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING);
            cassiaConn = new SqlConnection(ConfigurationManager.ConnectionStrings["CassiaConnectionString"].ConnectionString);
        }

        #region Common Reports

        public List<InvoiceReportDTO> GetTopupDetailsByOrganisationID(int iOrgID, string startDate, string endDate, int iBillingMethodID)
        {
            List<InvoiceReportDTO> topupList = new List<InvoiceReportDTO>();
            conn.Open();

            string strQuery = string.Format(@"SELECT
	                                            mpAcc.[Description] AS MPAccountName,
	                                            bm.BillingMethodName AS BillingMethod,
	                                            mpTrans.TransactionDate AS TransactionDate,
	                                            CONVERT(numeric(18,2),mpTrans.Amount) AS Amount
                                            FROM 
	                                            tblMicropaymentTransactions mpTrans 
                                            JOIN
	                                            tblMPAccount mpAcc
                                            ON 
	                                            mpTrans.MicropaymentAccountID = mpAcc.MPAccountCode
                                            JOIN
	                                            tblBillingMethod bm
                                            ON
	                                            mpTrans.PaymentMethodID = bm.BillingMethodID
                                            WHERE
	                                            mpAcc.OrganisationID={0}
                                                AND mpTrans.TransactionDate BETWEEN '{1}'
                                                AND DATEADD(DAY, 1, '{2}')", iOrgID, startDate, endDate);

            if (iBillingMethodID != 0)
            {
                strQuery += string.Format(" AND mpTrans.PaymentMethodID = {0}", iBillingMethodID);
            }
            topupList = (List<InvoiceReportDTO>)conn.Query<InvoiceReportDTO>(strQuery);
            conn.Close();

            return topupList;
        }

        #region Organisation based Usage Report

        public List<DropDownDTO> GetMPActListByOrg(int iOrganisationID)
        {
            List<DropDownDTO> lstMPAccount = new List<DropDownDTO>();
            cassiaConn.Open();

            string sqlquery = string.Format(@"SELECT mpacc.MPAccountCode as Value, mpacc.Description as Text
                                                  FROM 
                                                        tblMPAccount mpacc
                                                  WHERE 
                                                        mpacc.Description != '' AND mpacc.OrganisationID=@iOrganisationID
                                                  ORDER BY 
                                                        mpacc.Description");

            lstMPAccount = (List<DropDownDTO>)cassiaConn.Query<DropDownDTO>(sqlquery, new { iOrganisationID });

            cassiaConn.Close();
            return lstMPAccount;
        }

        public List<UsageReportDTO> GetUsageDetailsByOrganisationID(int iOrgID, string sStartDate, string sEndDate, int iMPAccountCode, int pageNumber, int pageSize, out int iTotalRecords, out int TotalMessagesSent, out decimal TotalMessageCost, string sortColumn, string sortOrder)
        {
            iTotalRecords     = 0;
            TotalMessageCost  = 0;
            TotalMessagesSent = 0;
            
            List<UsageReportDTO> usageList = new List<UsageReportDTO>();
            cassiaConn.Open();

            string strTotalRecQuery = string.Format(@"SELECT 
		                                                COUNT(*) TotalRecords,
                                                         SUM(mp.NetAmount) TotalMessageCost
	                                                FROM 
		                                                tblMicroPayment mp JOIN tblMPAccount mpacc
	                                                ON
		                                                mp.MPAccountCode = mpacc.MPAccountCode
	                                                WHERE
                                                            mpacc.OrganisationID = {0} 
                                                            AND [DateTime] BETWEEN '{1}' 
                                                            AND DATEADD(DAY, 1, '{2}')",
                                                    iOrgID, sStartDate, sEndDate);

            if (iMPAccountCode != 0)
            {
                strTotalRecQuery += string.Format(" AND mp.MPAccountCode = {0}", iMPAccountCode);
            }

            var result = cassiaConn.Query(strTotalRecQuery).Single();
            
            iTotalRecords = result.TotalRecords;
           
            int TotalPages = (int)Math.Ceiling((float)iTotalRecords / (float)pageSize);
            if (pageNumber > TotalPages)
                pageNumber = TotalPages;

            if (iTotalRecords != 0)
            {
                TotalMessagesSent = iTotalRecords;
                TotalMessageCost = result.TotalMessageCost;

                var parameters = new DynamicParameters();
                parameters.Add("@sortColumn", sortColumn);
                parameters.Add("@sortOrder", sortOrder);
                parameters.Add("@pageNumber", pageNumber);
                parameters.Add("@pageSize", pageSize);

                parameters.Add("@OrganisatonID", iOrgID);
                parameters.Add("@MPAccountCode", iMPAccountCode);
                parameters.Add("@StartDate", sStartDate);
                parameters.Add("@EndDate", sEndDate);

                usageList = (List<UsageReportDTO>)cassiaConn.Query<UsageReportDTO>("usp_GetUsageRptForUserPortal", parameters, commandType: CommandType.StoredProcedure);
            }

            return usageList;
        }

        #endregion

        #region Export to CSV file

        public List<UsageReportDTO> GetUsageReportByOrgIDForExport(int iOrgID, string sStartDate, string sEndDate, int iMPAccountCode)
        {
            List<UsageReportDTO> usageList = new List<UsageReportDTO>();
            cassiaConn.Open();

            var parameters = new DynamicParameters();

            parameters.Add("@OrganisatonID", iOrgID);
            parameters.Add("@MPAccountCode", iMPAccountCode);
            parameters.Add("@StartDate", sStartDate);
            parameters.Add("@EndDate", sEndDate);

            usageList = (List<UsageReportDTO>)cassiaConn.Query<UsageReportDTO>("usp_GetUsageRptForExport", parameters, commandType: CommandType.StoredProcedure);

            return usageList;
        }

        #endregion


        #endregion

        #region Adminportal Reports

        public List<InvoiceReportDTO> GetFinanceReportForAdminPortal(string sStartDate, string sEndDate, int iBillingMethodID, int pageNumber, int pageSize, out int iTotalRecords, out decimal TopupAmountSum, string sortColumn, string sortOrder)
        {
            iTotalRecords = 0;
            TopupAmountSum = 0;

            List<InvoiceReportDTO> invoiceList = new List<InvoiceReportDTO>();

            conn.Open();

            string strTotalRecQuery = string.Format(@"SELECT 
		                                                COUNT(*) TotalRecords,
                                                        SUM(mpTrans.Amount) TotalAmount
	                                                FROM 
		                                                tblMicropaymentTransactions mpTrans 
	                                                WHERE                                                        
                                                        mpTrans.TransactionDate BETWEEN '{0}' 
                                                        AND DATEADD(DAY, 1, '{1}')", sStartDate, sEndDate);

            if (iBillingMethodID != 0)
            {
                strTotalRecQuery += string.Format(" AND mpTrans.PaymentMethodID ={0}", iBillingMethodID);
            }

            var result = conn.Query(strTotalRecQuery).Single();
            iTotalRecords = result.TotalRecords;

            int TotalPages = (int)Math.Ceiling((float)iTotalRecords / (float)pageSize);
            if (pageNumber > TotalPages)
                pageNumber = TotalPages;

            if (iTotalRecords != 0)
            {
                TopupAmountSum = result.TotalAmount;

                var parameters = new DynamicParameters();
                parameters.Add("@sortColumn", sortColumn);
                parameters.Add("@sortOrder", sortOrder);
                parameters.Add("@pageNumber", pageNumber);
                parameters.Add("@pageSize", pageSize);

                parameters.Add("@StartDate", sStartDate);
                parameters.Add("@EndDate", sEndDate);
                parameters.Add("@BillingMethodID", iBillingMethodID);
                invoiceList = (List<InvoiceReportDTO>)conn.Query<InvoiceReportDTO>("usp_GetFinanceRptForAdminPortal", parameters, commandType: CommandType.StoredProcedure);

            }
            return invoiceList;
        }

        public List<UsagePerDayReportDTO> GetUsagePerDayReportForAdminPortal(string sStartDate, string sEndDate, int pageNumber, int pageSize, out int iTotalRecords, out int TotalMessagesSent, out decimal TotalMessageCost, string sortColumn, string sortOrder)
        {
            iTotalRecords = 0;
            TotalMessagesSent = 0;
            TotalMessageCost = 0;

            List<UsagePerDayReportDTO> usagePerDayReportList = new List<UsagePerDayReportDTO>();

            string strTotalRecQuery = @"SELECT 
	                                        COUNT(*) 
                                        FROM 
	                                        (SELECT 
			                                        COUNT(*) Over()AS TotalRecords
		                                        FROM 
			                                        tblMicroPayment mp
		                                        WHERE
			                                        mp.[DateTime] BETWEEN @sStartDate AND DATEADD(DAY, 1, @sEndDate)
		                                        GROUP BY CONVERT(VARCHAR(30),mp.[DateTime],106)
                                            )
                                        AS TotalDaysTable";


            iTotalRecords = cassiaConn.Query<int>(strTotalRecQuery, new { sStartDate, sEndDate }).Single();

            int TotalPages = (int)Math.Ceiling((float)iTotalRecords / (float)pageSize);
            if (pageNumber > TotalPages)
                pageNumber = TotalPages;

            if (iTotalRecords != 0)
            {
                string strTotalMessagesSentQuery = @"SELECT 
	                                                    COUNT(*)AS TotalMessageSent,
                                                        Sum(NetAmount) AS TotalNetAmount
                                                    FROM 
	                                                    tblMicroPayment mp
                                                    WHERE
	                                                    mp.[DateTime] BETWEEN  @sStartDate AND DATEADD(DAY, 1, @sEndDate)";

                var result = cassiaConn.Query(strTotalMessagesSentQuery, new { sStartDate, sEndDate }).Single();

                TotalMessagesSent = result.TotalMessageSent;
                TotalMessageCost = result.TotalNetAmount;

                var parameters = new DynamicParameters();
                parameters.Add("@sortColumn", sortColumn);
                parameters.Add("@sortOrder", sortOrder);
                parameters.Add("@pageNumber", pageNumber);
                parameters.Add("@pageSize", pageSize);

                parameters.Add("@StartDate", sStartDate);
                parameters.Add("@EndDate", sEndDate);
                usagePerDayReportList = (List<UsagePerDayReportDTO>)cassiaConn.Query<UsagePerDayReportDTO>("usp_GetUsagePerDayRptForAdminPortal", parameters, commandType: CommandType.StoredProcedure);
            }

            return usagePerDayReportList;
        }

        #region Export to CSV file

        public List<InvoiceReportDTO> GetFinanceReportForExport(string sStartDate, string sEndDate, int iBillingMethodID)
        {
            List<InvoiceReportDTO> invoiceList = new List<InvoiceReportDTO>();

            conn.Open();

            var parameters = new DynamicParameters();

            parameters.Add("@StartDate", sStartDate);
            parameters.Add("@EndDate", sEndDate);
            parameters.Add("@BillingMethodID", iBillingMethodID);

            invoiceList = (List<InvoiceReportDTO>)conn.Query<InvoiceReportDTO>("usp_GetFinanceRptForExport", parameters, commandType: CommandType.StoredProcedure);

            return invoiceList;
        }

        public List<UsagePerDayReportDTO> GetUsagePerDayReportForExport(string sStartDate, string sEndDate)
        {
            List<UsagePerDayReportDTO> usagePerDayReportList = new List<UsagePerDayReportDTO>();

            var parameters = new DynamicParameters();

            parameters.Add("@StartDate", sStartDate);
            parameters.Add("@EndDate", sEndDate);

            usagePerDayReportList = (List<UsagePerDayReportDTO>)cassiaConn.Query<UsagePerDayReportDTO>("usp_GetUsagePerDayRptForExport", parameters, commandType: CommandType.StoredProcedure);

            return usagePerDayReportList;
        }

        #endregion

        #endregion

    }
}
