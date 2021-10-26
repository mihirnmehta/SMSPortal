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
    public class ReportBL
    {
        IReportRepository iRepository = new ReportDAL();

        #region Common Reports

        public List<InvoiceReportDTO> GetTopupDetailsByOrganisationID(int iOrgID, string startDate, string endDate, int iBillingMethodID)
        {
            return iRepository.GetTopupDetailsByOrganisationID(iOrgID, startDate, endDate, iBillingMethodID);
        }

        public List<DropDownDTO> GetMPActListByOrg(int iOrganisationID)
        {
            List<DropDownDTO> lstMPAccount = iRepository.GetMPActListByOrg(iOrganisationID);
            return lstMPAccount;
        }

        public List<UsageReportDTO> GetUsageDetailsByOrganisationID(int iOrgID, string sStartDate, string sEndDate, int iMPAccountCode, int pageNumber, int pageSize, out int iTotalRecords, out int TotalMessagesSent, out decimal TotalMessageCost, string sortColumn, string sortOrder)
        {
            return iRepository.GetUsageDetailsByOrganisationID(iOrgID, sStartDate, sEndDate, iMPAccountCode, pageNumber, pageSize, out iTotalRecords, out TotalMessagesSent, out TotalMessageCost, sortColumn, sortOrder);
        }

        #region Export to CSV file

        public List<string> GetTopupDetailsByOrgIDForExport(int iOrgID, string startDate, string endDate, int iBillingMethodID)
        {
            decimal dTotalCost = 0;

            List<string> strListOfTopupReport = new List<string>();

            List<InvoiceReportDTO> topupReportList = iRepository.GetTopupDetailsByOrganisationID(iOrgID, startDate, endDate, iBillingMethodID);

            strListOfTopupReport.Add("SMS Account, Date Time, Topup Amount, Method");
            dTotalCost = topupReportList.Sum(dto => dto.Amount);
            
            for (int i = 0; i < topupReportList.Count(); i++)
            {
                strListOfTopupReport.Add(
                                            topupReportList[i].MPAccountName + ", " +
                                            topupReportList[i].sTransactionDate + ", " +
                                            topupReportList[i].sAmount + ", " +
                                            topupReportList[i].BillingMethod
                                        );
            }
            strListOfTopupReport.Add(",,,Total Cost: " + dTotalCost.ToString("F2"));
            return strListOfTopupReport;
        }

        public List<string> GetUsageReportByOrgIDForExport(int iOrgID, string sStartDate, string sEndDate, int iMPAccountCode)
        {
            int iTotalMessageSent = 0;
            decimal dTotalMessageCost = 0;

            List<string> strListOfUsageReport = new List<string>();

            List<UsageReportDTO> usageList = iRepository.GetUsageReportByOrgIDForExport(iOrgID, sStartDate, sEndDate, iMPAccountCode);

            dTotalMessageCost = usageList.Sum(dto => dto.NetAmount);
            
            iTotalMessageSent = usageList.Count();
            
            strListOfUsageReport.Add("SMS Account, Date Time, Description, Cost");

            char[] csvTokens = new[] { '\"', ',', '\n', '\r' };
                   
            for (int i = 0; i < iTotalMessageSent; i++)
            {
                string strEscape = usageList[i].StatementDescription;
                
                if (strEscape.IndexOfAny(csvTokens) >= 0)
                {
                    strEscape = "\"" + strEscape.Replace("\"", "\"\"") + "\"";
                } 

                strListOfUsageReport.Add(usageList[i].MPAccountName + ", " + usageList[i].sDate + ", " + strEscape + ", " + usageList[i].sNetAmount);
            }
           
            strListOfUsageReport.Add(",,,Total Message Sent: " + iTotalMessageSent);
            strListOfUsageReport.Add(",,,Total Message Cost: " + dTotalMessageCost.ToString("F2"));

            return strListOfUsageReport;
        }

        #endregion

        #endregion

        #region Adminportal Reports

        public List<InvoiceReportDTO> GetFinanceReportForAdminPortal(string startDate, string endDate, int iBillingMethodID, int pageNumber, int pageSize, out int iTotalRecords, out decimal TopupAmountSum, string sortColumn, string sortOrder)
        {
            return iRepository.GetFinanceReportForAdminPortal(startDate, endDate, iBillingMethodID, pageNumber, pageSize, out iTotalRecords, out TopupAmountSum, sortColumn, sortOrder);
        }

        public List<UsagePerDayReportDTO> GetUsagePerDayReportForAdminPortal(string startDate, string endDate, int pageNumber, int pageSize, out int iTotalRecords, out int TotalMessagesSent, out decimal TotalMessageCost, string sortColumn, string sortOrder)
        {
            return iRepository.GetUsagePerDayReportForAdminPortal(startDate, endDate, pageNumber, pageSize, out iTotalRecords, out TotalMessagesSent, out TotalMessageCost, sortColumn, sortOrder);
        }

        #region Export to CSV

        public List<string> GetFinanceReportForExport(string sStartDate, string sEndDate, int iBillingMethodID)
        {
            decimal dTotalCost = 0;
            List<string> strListOfFinanceReport = new List<string>();

            List<InvoiceReportDTO> invoiceList = iRepository.GetFinanceReportForExport(sStartDate, sEndDate, iBillingMethodID);
            
            strListOfFinanceReport.Add("Customer Name, SMS Account, Date & Time, Topup Amount, Method, OA Export Date");
            
            dTotalCost = invoiceList.Sum(dto => dto.Amount);

            for (int i = 0; i < invoiceList.Count; i++)
            {
                strListOfFinanceReport.Add(
                                            invoiceList[i].CustomerName + ", " + 
                                            invoiceList[i].MPAccountName + ", " +
                                            invoiceList[i].sTransactionDate + ", " + 
                                            invoiceList[i].sAmount + ", " + 
                                            invoiceList[i].BillingMethod + ", " + 
                                            invoiceList[i].sOAExportDate
                                          );
            }

            strListOfFinanceReport.Add(",,,,,Total Cost: " + dTotalCost.ToString("F2"));

            return strListOfFinanceReport;
        }

        public List<string> GetUsagePerDayReportForExport(string sStartDate, string sEndDate)
        {
            int iTotalMessageSent = 0;
            decimal dTotalMessageCost = 0;

            List<string> strListOfUsagePerDayReport = new List<string>();
            List<UsagePerDayReportDTO> usagePerDayReportList = new List<UsagePerDayReportDTO>();

            usagePerDayReportList = iRepository.GetUsagePerDayReportForExport(sStartDate, sEndDate);

            strListOfUsagePerDayReport.Add("Date Time, Messages Sent, Amount");

            iTotalMessageSent = usagePerDayReportList.Sum(dto => dto.TotalMessagesSentPerDay);
            dTotalMessageCost = usagePerDayReportList.Sum(dto => dto.TotalNetAmountPerDay);

            for (int i = 0; i < usagePerDayReportList.Count; i++)
            {
                strListOfUsagePerDayReport.Add(
                                            usagePerDayReportList[i].sDate + ", " +
                                            usagePerDayReportList[i].TotalMessagesSentPerDay + ", " +
                                            usagePerDayReportList[i].sTotalNetAmountPerDay
                                          );
            }

            strListOfUsagePerDayReport.Add(",,Total Messages Sent: " + iTotalMessageSent);
            strListOfUsagePerDayReport.Add(",,Total Cost: " + dTotalMessageCost.ToString("F2"));

            return strListOfUsagePerDayReport;
        }

        #endregion

        #endregion
    }
}
