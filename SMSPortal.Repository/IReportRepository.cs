using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SMSPortal.Models;

namespace SMSPortal.Repository
{
    public interface IReportRepository
    {
        #region Adminportal Reports

        List<InvoiceReportDTO> GetFinanceReportForAdminPortal(string startDate, string endDate, int iBillingMethodID, int pageNumber, int pageSize, out int iTotalRecords, out decimal NetAmountSum, string sortColumn, string sortOrder);

        List<UsagePerDayReportDTO> GetUsagePerDayReportForAdminPortal(string startDate, string endDate, int pageNumber, int pageSize, out int iTotalRecords, out int TotalMessagesSent, out decimal TotalMessageCost, string sortColumn, string sortOrder);

        #region Export to CSV

        List<InvoiceReportDTO> GetFinanceReportForExport(string sStartDate, string sEndDate, int iBillingMethodID);
        
        #endregion

        #endregion

        #region Common Reports

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startDate">Must be in yyyy-MM-dd format</param>
        /// <param name="endDate">Must be in yyyy-MM-dd format</param>
        /// <returns></returns>
        List<InvoiceReportDTO> GetTopupDetailsByOrganisationID(int iOrgID, string startDate, string endDate, int iBillingMethodID);
        
        List<DropDownDTO> GetMPActListByOrg(int iOrganisationID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sStartDate">Must be in yyyy-MM-dd format</param>
        /// <param name="sEndDate">Must be in yyyy-MM-dd format</param>
        /// <returns></returns>
        List<UsageReportDTO> GetUsageDetailsByOrganisationID(int iOrgID, string sStartDate, string sEndDate, int iMPAccountCode, int pageNumber, int pageSize, out int iTotalRecords, out int TotalMessagesSent, out decimal TotalMessageCost, string sortColumn, string sortOrder);

        #region Export to CSV file
        
        List<UsageReportDTO> GetUsageReportByOrgIDForExport(int iOrgID, string sStartDate, string sEndDate, int iMPAccountCode);
        
        List<UsagePerDayReportDTO> GetUsagePerDayReportForExport(string sStartDate, string sEndDate);
        
        #endregion

        #endregion
    }    
}
