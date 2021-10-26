using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SMSAdminPortal.Commons;
using SMSPortal.BusinessLogic;
using SMSPortal.Models;

namespace SMSAdminPortal.Controllers.Organisation
{
    [MyAuthorize]
    public partial class OrganisationController : Controller
    {
        public ActionResult OrganisationReport()
        {
            return View();
        }

        public JsonResult GetTopupDetailsByOrganisationID(string sStartDate, string sEndDate, int iBillingMethodID, string sidx, string sord, int page, int rows)
        {
            int iOrgID = SessionHelper.OrganisationID.Value;
            //int iOrgID = 1635;

            int pageNumber = page;
            int iTotalRecords = 0;
            int iPageSize = rows;

            sStartDate = CommonFunctions.ConvertDateToSQLFormatDate(sStartDate);
            sEndDate = CommonFunctions.ConvertDateToSQLFormatDate(sEndDate);

            List<InvoiceReportDTO> objTopupList = new List<InvoiceReportDTO>();
            ReportBL objReportBL = new ReportBL();
            objTopupList = objReportBL.GetTopupDetailsByOrganisationID(iOrgID, sStartDate, sEndDate, iBillingMethodID);

            List<InvoiceReportDTO> lstSorted = objTopupList.OrderBy(sidx, sord);
            iTotalRecords = lstSorted.Count;

            decimal TotalTopupAmount = 0;
            
            TotalTopupAmount = lstSorted.Sum(dto => dto.Amount);

            int totalPages = (int)Math.Ceiling((float)iTotalRecords / (float)iPageSize);

            //If the user enters a page number greater than total number of pages...
            if (pageNumber > totalPages)
                pageNumber = totalPages;

            int iPageIndex = Convert.ToInt16(pageNumber) - 1;

            if (iPageSize > 0 && iTotalRecords > iPageSize)
            {
                lstSorted = lstSorted.Skip(iPageIndex * iPageSize).Take(iPageSize).ToList();
            }

            var result = new
            {
                total = totalPages,
                page = pageNumber,
                records = iTotalRecords,

                userdata = (lstSorted.Count > 0) ? "£ " + TotalTopupAmount.ToString("F2") : string.Empty,
                
                rows = (from x in lstSorted
                        select new
                        {
                            cell = new string[]
                            {
                                x.MPAccountName.ToString(),                           
                                x.sTransactionDate,
                                "£ "+ x.sAmount,
                                x.BillingMethod.ToString()
                            }
                        }).ToArray()
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMPAcctListByOrg()
        {
            int iOrganisationID = 5319;//SessionHelper.OrganisationID.Value;
            ReportBL objReportBL = new ReportBL();

            List<DropDownDTO> lstMPAccount = objReportBL.GetMPActListByOrg(iOrganisationID);
            return Json(lstMPAccount, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsageDetailsByOrganisationID(string sStartDate, string sEndDate, int iMPAccountCode, string sidx, string sord, int page, int rows)
        {
            int iOrgID = 5319;//SessionHelper.OrganisationID.Value;

            decimal TotalMessageCost = 0;
            int TotalMessagesSent    = 0;

            int iTotalRecords = 0;
            int iPageIndex    = Convert.ToInt16(page) - 1;

            string sortColumn = sidx;
            string sortOrder  = sord;
            int pageNumber    = page;
            int pageSize      = rows;

            sStartDate = CommonFunctions.ConvertDateToSQLFormatDate(sStartDate);
            sEndDate = CommonFunctions.ConvertDateToSQLFormatDate(sEndDate);

            List<UsageReportDTO> objUsageList = new List<UsageReportDTO>();
            ReportBL objReportBL = new ReportBL();
            objUsageList = objReportBL.GetUsageDetailsByOrganisationID(iOrgID, sStartDate, sEndDate, iMPAccountCode, pageNumber, pageSize, out iTotalRecords,out TotalMessagesSent, out TotalMessageCost, sortColumn, sortOrder);

            int totalPages = (int)Math.Ceiling((float)iTotalRecords / (float)pageSize);

            //If the user enters a page number greater than total number of pages...
            if (pageNumber > totalPages)
                pageNumber = totalPages;

            string strFooterValue = TotalMessagesSent.ToString() + ",£ " + TotalMessageCost.ToString("F2");

            var result = new
            {
                total   = totalPages,
                page    = pageNumber,
                records = iTotalRecords,

                userdata = (objUsageList.Count > 0) ? strFooterValue : string.Empty,

                rows = (from x in objUsageList
                        select new
                        {
                            cell = new string[]
                            {
                                x.MPAccountName.ToString(),                           
                                x.sDate,
                                x.StatementDescription.ToString(),
                                "£ "+ x.sNetAmount
                            }
                        }).ToArray()
            };
            //GetUsageReportByOrgIDForExport(sStartDate, sEndDate, iMPAccountCode);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region Export CSV

         [HttpPost]
        public FileStreamResult GetReportForExport(string sStartDate, string sEndDate, string OrgBasedReport, int? BillingMethod, int? MPAccount)
        {
            int iOrgID = SessionHelper.OrganisationID.Value;

            if (String.IsNullOrEmpty(sStartDate) || String.IsNullOrEmpty(sEndDate) || String.IsNullOrEmpty(OrgBasedReport))
                return File(new MemoryStream(), "text/csv", "NoData.CSV");

            ReportBL objReportBL = new ReportBL();
            sStartDate           = CommonFunctions.ConvertDateToSQLFormatDate(sStartDate);
            sEndDate             = CommonFunctions.ConvertDateToSQLFormatDate(sEndDate);

            bool bFinanceReport = false;
            if (OrgBasedReport == "1")
                bFinanceReport = true;

            if (bFinanceReport && BillingMethod.HasValue)    //Its Finance Report
            {
                #region Finance Report
                List<string> strListOfTopupReport = new List<string>();

                strListOfTopupReport = objReportBL.GetTopupDetailsByOrgIDForExport(iOrgID, sStartDate, sEndDate, BillingMethod.Value);

                MemoryStream output = new MemoryStream();
                StreamWriter writer = new StreamWriter(output, Encoding.UTF8);

                foreach (string strRecord in strListOfTopupReport)
                {
                    writer.WriteLine(strRecord);
                }

                writer.Flush();
                output.Position = 0;

                string strFileName = "TopupReportByOrgID_" + sStartDate + "_" + sEndDate + ".csv";

                return File(output, "text/csv", strFileName);
                #endregion
            }
            else
            {
                #region Usage Report
                iOrgID = 5319;//SessionHelper.OrganisationID.Value;

                List<string> strListOfUsageReport = new List<string>();

                strListOfUsageReport = objReportBL.GetUsageReportByOrgIDForExport(iOrgID, sStartDate, sEndDate, MPAccount.Value);

                MemoryStream output = new MemoryStream();
                StreamWriter writer = new StreamWriter(output, Encoding.UTF8);

                foreach (string strRecord in strListOfUsageReport)
                {
                    writer.WriteLine(strRecord);
                }

                writer.Flush();
                output.Position = 0;

                string strFileName = "UsageReportByOrgID_" + sStartDate + "_" + sEndDate + ".csv";

                return File(output, "text/csv", strFileName);
                
                #endregion
            }
        }

        #endregion
    }
}
