using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using System.Text;


using SMSAdminPortal.Commons;
using SMSPortal.BusinessLogic;
using SMSPortal.Models;

namespace SMSAdminPortal.Controllers
{
    [MyAuthorize]
    public class ReportController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Report");
        }

        public ActionResult Report()
        {
            return View();
        }

        public JsonResult GetFinanceReportForAdminPortal(string sStartDate, string sEndDate, int iBillingMethodID, string sidx, string sord, int page, int rows)
        {
            decimal TopupAmountSum = 0;

            int iTotalRecords = 0;

            string sortColumn = sidx;
            string sortOrder = sord;
            int pageNumber = page;
            int pageSize = rows;


            sStartDate = CommonFunctions.ConvertDateToSQLFormatDate(sStartDate);
            sEndDate = CommonFunctions.ConvertDateToSQLFormatDate(sEndDate);

            List<InvoiceReportDTO> objInvoiceList = new List<InvoiceReportDTO>();

            ReportBL objReportBL = new ReportBL();
            objInvoiceList = objReportBL.GetFinanceReportForAdminPortal(sStartDate, sEndDate, iBillingMethodID, pageNumber, pageSize, out iTotalRecords, out TopupAmountSum, sortColumn, sortOrder);

            int totalPages = (int)Math.Ceiling((float)iTotalRecords / (float)pageSize);

            //If the user enters a page number greater than total number of pages...
            if (pageNumber > totalPages)
                pageNumber = totalPages;

            var result = new
            {
                total   = totalPages,
                page    = pageNumber,
                records = iTotalRecords,

                userdata = (objInvoiceList.Count > 0) ? "£ "+ TopupAmountSum.ToString("F2") : string.Empty,

                rows = (from x in objInvoiceList
                        select new
                        {
                            cell = new string[]
                            {
                                x.CustomerName.ToString(),
                                x.MPAccountName.ToString(),                           
                                x.sTransactionDate,
                                "£ " + x.sAmount,
                                x.BillingMethod.ToString(),
                                x.sOAExportDate
                            }
                        }).ToArray()
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsagePerDayReportForAdminPortal(string sStartDate, string sEndDate, string sidx, string sord, int page, int rows)
        {
            int TotalMessagesSent = 0;
            decimal TotalMessageCost = 0;

            int iTotalRecords = 0;

            string sortColumn = sidx;
            string sortOrder  = sord;
            int pageNumber    = page;
            int pageSize      = rows;

            sStartDate = CommonFunctions.ConvertDateToSQLFormatDate(sStartDate);
            sEndDate = CommonFunctions.ConvertDateToSQLFormatDate(sEndDate);

            List<UsagePerDayReportDTO> usagePerDayReportList = new List<UsagePerDayReportDTO>();
            
            ReportBL objReportBL = new ReportBL();
            usagePerDayReportList = objReportBL.GetUsagePerDayReportForAdminPortal(sStartDate, sEndDate, pageNumber, pageSize, out iTotalRecords, out TotalMessagesSent, out TotalMessageCost, sortColumn, sortOrder);

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

                userdata = (usagePerDayReportList.Count > 0) ? strFooterValue : string.Empty,

                rows = (from x in usagePerDayReportList
                        select new
                        {
                            cell = new string[]
                            {
                                x.sDate,
                                x.TotalMessagesSentPerDay.ToString(),
                                "£ " + x.sTotalNetAmountPerDay
                            }
                        }).ToArray()
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region Export CSV

        [HttpPost]
        public FileStreamResult GetReportForExport(string sStartDate, string sEndDate, string AdminTopLevelReport, int? BillingMethod)
        {
            if (String.IsNullOrEmpty(sStartDate) || String.IsNullOrEmpty(sEndDate) || String.IsNullOrEmpty(AdminTopLevelReport))
                return File(new MemoryStream(), "text/csv", "NoData.CSV");

            ReportBL objReportBL = new ReportBL();
            sStartDate           = CommonFunctions.ConvertDateToSQLFormatDate(sStartDate);
            sEndDate             = CommonFunctions.ConvertDateToSQLFormatDate(sEndDate);

            bool bFinanceReport = false;
            if (AdminTopLevelReport == "1")
                bFinanceReport = true;

            if (bFinanceReport && BillingMethod.HasValue)    //Its Finance Report
            {
                #region Finance Report

                List<string> lstFinanceReportRecords = new List<string>();
                lstFinanceReportRecords = objReportBL.GetFinanceReportForExport(sStartDate, sEndDate, BillingMethod.Value);

                MemoryStream output = new MemoryStream();
                StreamWriter writer = new StreamWriter(output, Encoding.UTF8);

                foreach (string strRecord in lstFinanceReportRecords)
                {
                    writer.WriteLine(strRecord);
                }

                writer.Flush();
                output.Position = 0;

                string strFileName = "FinanceReport_" + sStartDate + "_" + sEndDate + ".csv";

                #endregion

                return File(output, "text/csv", strFileName);
            }
            else // Its Usage Report
            {
                #region Usage Report

                List<string> strListOfUsagePerDayReport = new List<string>();
                strListOfUsagePerDayReport = objReportBL.GetUsagePerDayReportForExport(sStartDate, sEndDate);

                MemoryStream output = new MemoryStream();
                StreamWriter writer = new StreamWriter(output, Encoding.UTF8);

                foreach (string strRecord in strListOfUsagePerDayReport)
                {
                    writer.WriteLine(strRecord);
                }

                writer.Flush();
                output.Position = 0;

                string strFileName = "UsagePerDayReport_" + sStartDate + "_" + sEndDate + ".csv";

                #endregion

                return File(output, "text/csv", strFileName);
            }

        }       

        #endregion
    }
}
