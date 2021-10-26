using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


using SMSPortal.Models;

using SMSPortal.BusinessLogic;
using SMSPortal.BusinessLogic.Organisation;

using System.Configuration;

using SMSAdminPortal.Commons;
using System.IO;


namespace SMSAdminPortal.Controllers.Organisation
{
    public partial class OrganisationController : Controller
    {

        public ActionResult MPAccount()
        {
            if (SessionHelper.OrganisationID == null)
                return RedirectToAction("ManageOrganisations");
            else
                return View();
        }

        public JsonResult GetMPAccountsByOrganisationID(string sidx, string sord, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {

            //sidx  - Sort Index
            //sord  - Sort Order
            //page  - Current Page
            //rows  - Page size
            //_search - true / false
            //searchField
            //searchOper:     <, > , = 
            //searchString

            int iOrgID = SessionHelper.OrganisationID.Value;

            int pageNumber = page;
            int iTotalRecords = 0;
            int iPageSize = rows;

            MPAccountBL objMPAccountBL = new MPAccountBL();

            List<MPAccountDTO> lstMPAccount = objMPAccountBL.GetMPAccountsByOrganisationID(iOrgID);

            List<MPAccountDTO> lstSortedMPAccount = lstMPAccount.OrderBy(sidx, sord);

            if (_search)
            {
                lstSortedMPAccount = lstSortedMPAccount.FindAll(searchOper, searchField, searchString);
            }

            iTotalRecords = lstSortedMPAccount.Count;
            int totalPages = (int)Math.Ceiling((float)iTotalRecords / (float)iPageSize);

            //If the user enters a page number greater than total number of pages...
            if (pageNumber > totalPages)
                pageNumber = totalPages;

            int iPageIndex = Convert.ToInt16(pageNumber) - 1;

            if (iPageSize > 0 && iTotalRecords > iPageSize)
            {
                lstSortedMPAccount = lstSortedMPAccount.Skip(iPageIndex * iPageSize).Take(iPageSize).ToList();
            }
            string PaymentMethod = "";
            var result = new
            {
                total = totalPages,
                page = pageNumber,
                records = iTotalRecords,
                rows = (from x in lstSortedMPAccount
                        select new
                        {
                            id = x.MPAccountCode.ToString(),
                            cell = new string[]
                            {
                                x.MPAccountCode.ToString(),
                                x.Description.ToString(),
                                x.Balance.ToString(),
                                x.SendLowBalanceWarnings ? "Yes" : "No",
                                x.SendLowBalanceWarnings ? x.BalanceWarningLimit.ToString() : "--" ,
                                PaymentMethod=(x.Paypal ? " PayPal" : "")  +
                                (((x.Paypal) && (x.Invoice)) ? "," : "") +
                                (x.Invoice ? " Invoice" : ""),
                                x.IsEnabled ? "Yes" : "No"
                            }
                        }).ToArray()
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowAddMPAccountDialog(int iAccountID)
        {
            //int iOrgID = SessionHelper.OrganisationID.Value;

            ViewData["AccountID"] = iAccountID;
            return PartialView("PopupAddMPAccount");
        }

        public JsonResult GetOrgBillingMethods()
        {
            int iOrgID = SessionHelper.OrganisationID.Value;

            MPAccountBL objMPAccountBL = new MPAccountBL();
            BillingMethodDTO objBillingMethodDTO = objMPAccountBL.GetOrgBillingMethods(iOrgID);
            return this.Json(objBillingMethodDTO, JsonRequestBehavior.AllowGet);
        }

        public string AddMPAccount(bool bIsEnabled, string sAccLogin, string sDesc, string sPassword,
                                   bool bSendLowBalWarn, int iBalWarnLmt, string sBalWarnEmail,
                                   bool bUsageRestriction, string sWorkingDayStart, string sWorkingDayFinish, string sNonWorkingDayStart, string sNonWorkingDayFinish,
                                   bool bPayPal, bool bInvoice)
        {

            int iOrgID = SessionHelper.OrganisationID.Value;

            string sCreatedBy, sUpdatedBy;

            sCreatedBy = sUpdatedBy = SessionHelper.LoggedInUserEmail;

            bool bIsAdded = false;
            int iServiceCode = PortalConstants.SERVICE_CODE;

            MPAccountBL objMPAccountBL = new MPAccountBL();

            bIsAdded = objMPAccountBL.AddMPAccount(bIsEnabled, sAccLogin, sDesc, sPassword, iOrgID,
                                                  bSendLowBalWarn, iBalWarnLmt, sBalWarnEmail,
                                                  bUsageRestriction, iServiceCode,
                                                  sWorkingDayStart, sWorkingDayFinish, sNonWorkingDayStart, sNonWorkingDayFinish,
                                                  bPayPal, bInvoice, sCreatedBy, sUpdatedBy);

            if (bIsAdded)
                return "true";
            else
                return "false";
        }

        public JsonResult GetMPAccountByID(int iAccountID)
        {
            MPAccountBL objMPAccountBL = new MPAccountBL();
            MPAccountDTO objMPAccountDTO = objMPAccountBL.GetMPAccountByID(iAccountID);
            return this.Json(objMPAccountDTO);
        }

        public string UpdateMPAccount(int iAccountID, bool bIsEnabled, string sAccLogin, string sDesc, string sPassword,
                                      bool bSendLowBalWarn, int iBalWarnLmt, string sBalWarnEmail,
                                      bool bUsageRestriction, string sWorkingDayStart, string sWorkingDayFinish, string sNonWorkingDayStart, string sNonWorkingDayFinish,
                                      bool bPayPal, bool bInvoice)
        {

            string sUpdatedBy = SessionHelper.LoggedInUserEmail;
            int iServiceCode = PortalConstants.SERVICE_CODE;

            MPAccountBL objMPAccountBL = new MPAccountBL();
            bool bIsUpdated = objMPAccountBL.UpdateMPAccount(iAccountID, bIsEnabled, sAccLogin, sDesc, sPassword,
                                                          bSendLowBalWarn, iBalWarnLmt, sBalWarnEmail,
                                                          bUsageRestriction, iServiceCode,
                                                          sWorkingDayStart, sWorkingDayFinish, sNonWorkingDayStart, sNonWorkingDayFinish,
                                                          bPayPal, bInvoice, sUpdatedBy);

            if (bIsUpdated)
                return "true";
            else
                return "false";
        }

        public JsonResult IsUsernameUnique(string strUsername)
        {
            MPAccountBL objMPAccountBL = new MPAccountBL();
            bool bResult = objMPAccountBL.IsUsernameUnique(strUsername);
            return Json(bResult);
        }

        public JsonResult IsUsernameUniqueOnUpdate(string strUsername, int iAccountID)
        {
            MPAccountBL objMPAccountBL = new MPAccountBL();
            bool bResult = objMPAccountBL.IsUsernameUniqueOnUpdate(strUsername, iAccountID);
            return Json(bResult);
        }

        #region Function to test Invoice Generation Locally

        public ActionResult ImportInvoices()
        {
            MPAccountBL objMPAccountBL = new MPAccountBL();
            List<string> strInvoiceList = objMPAccountBL.ImportInvoices();

            string strBatchNumber = "";

            if (strInvoiceList.Count > 0)
            {
                strBatchNumber = strInvoiceList[0];

                List<string> lstInvoice = new List<string>(strInvoiceList.Skip(1));
                bool bInvoiceCreated = WriteInvoiceFile(lstInvoice, strBatchNumber);
            }

            return View("MPAccount");
        }

        #region Invoices

        bool WriteInvoiceFile(List<string> lstInvoice, string strBatchNumber)
        {
            try
            {
                //Staffplan PAYG111213-161718.txt
                string strInvoiceFileName = "Staffplan PAYG" + strBatchNumber + ".txt";
                string filePath = "~/App_Data/Invoice/";
                if (!Directory.Exists(filePath));
                    Directory.CreateDirectory(Server.MapPath(filePath));

                string strInvoicePath = Server.MapPath(filePath + strInvoiceFileName);

                System.IO.StreamWriter file = new System.IO.StreamWriter(strInvoicePath);
                foreach (string str in lstInvoice)
                {
                    file.WriteLine(str);
                }
                file.Close();

                MPAccountBL objMPAccountBL = new MPAccountBL();
                objMPAccountBL.UpdateBatchInvoiceAcknowledged(strBatchNumber);

                return true;
            }
            catch (Exception ex)
            {

                WriteInvoiceError(ex, strBatchNumber);
                return false;
            }
        }

        void WriteInvoiceError(Exception ex, string strBatchNumber)
        {
            try
            {
                string strInvoicePath = "~/App_Data/Invoice/InvoiceError.txt";

                System.IO.StreamWriter file = new System.IO.StreamWriter(strInvoicePath, true);
                file.WriteLine("----------------- Batch Number: " + strBatchNumber + " -----------------");

                if (!String.IsNullOrEmpty(ex.Message))
                    file.WriteLine(ex.Message);
                if (!String.IsNullOrEmpty(ex.StackTrace))
                    file.WriteLine(ex.StackTrace);

                file.Close();
            }
            catch (Exception exc)
            { }
        }

        void WriteWCFServiceError(Exception ex)
        {
            DateTime dt = DateTime.Now;
            string strBatchNumber = dt.ToString("ddMMyy") + "-" + dt.ToString("HHmmss");

            try
            {
                string strWebServiceErrorPath = "~/App_Data/Invoice/WCFServiceError.txt";

                System.IO.StreamWriter file = new System.IO.StreamWriter(strWebServiceErrorPath, true);
                file.WriteLine("----------------- Date & Time: " + strBatchNumber + " -----------------");

                if (!String.IsNullOrEmpty(ex.Message))
                    file.WriteLine(ex.Message);
                if (!String.IsNullOrEmpty(ex.StackTrace))
                    file.WriteLine(ex.StackTrace);

                file.Close();
            }
            catch (Exception exc)
            { }
        }

        #endregion

        #endregion
    }
}
