using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;
using System.Text;
using System.Configuration;
using System.Globalization;

using SMSPortal.Models;
using SMSPortal.BusinessLogic;

using SMSAdminPortal.Commons;

namespace SMSAdminPortal.Controllers
{


    public class TopupController : Controller
    {

        [MyAuthorize]
        public ActionResult Index()
        {
            return RedirectToAction("Topup");
        }

        [MyAuthorize]
        [HttpGet]
        public ActionResult Topup()
        {
            SessionHelper.ClearOrgSessionVariables();
            return View();
        }

        [MyAuthorize]
        [HttpPost]
        public ActionResult Topup(string Organisation, string MPAccount, string Amount)
        {

            //Works both ways - Either through Request.Form or Parameters
            //string strPaymentMethod  = Request.Form["PaymentMethod"];
            //string strOrganisationID = Request.Form["Organisation"];
            //string strMPAccountID    = Request.Form["MPAccount"];
            //string strAmount         = Request.Form["Amount"];   

            decimal dAmount;

            if (String.IsNullOrEmpty(Organisation) || String.IsNullOrEmpty(MPAccount) || String.IsNullOrEmpty(Amount) || !Decimal.TryParse(Amount, out dAmount))
            {
                ViewData["ErrorMessage"] = "There was a problem processing your request. Please try again.";
                return View();
            }

            bool bResult = RecordInvoiceTopupTransaction(Convert.ToInt32(MPAccount), dAmount, SessionHelper.LoggedInUserEmail);

            if (bResult)
                ViewData["SuccessMessage"] = "Invoice Topup Successful.";
            //return RedirectToAction("InvoiceSuccess"); //Redirect("/Topup/InvoiceSuccess");
            else
            {
                ViewData["ErrorMessage"] = "There was a problem processing your request. Please try again.";
            }
            return View();

        }

        [MyAuthorize]
        public ActionResult InvoiceSuccess()
        {
            return View();
        }


        #region Other Action Methods

        [MyAuthorize]
        public JsonResult GetListOfCompanies()
        {
            List<DropDownDTO> lstOfCompanies = CommonFunctions.GetListOfCompanies();

            return Json(lstOfCompanies, JsonRequestBehavior.AllowGet);
        }

        [MyAuthorize]
        public JsonResult GetOrganisationList(int CompanyID)
        {
            TopupBL objTopupBL = new TopupBL();

            List<DropDownDTO> lstOrganisation = objTopupBL.GetOrganisationList(CompanyID);
            return Json(lstOrganisation, JsonRequestBehavior.AllowGet);
        }

        [MyAuthorize]
        public JsonResult GetMPAcctListByOrg(int iOrganisationID)
        {
            TopupBL objTopupBL = new TopupBL();

            List<DropDownDTO> lstMPAccount = objTopupBL.GetMPActListByOrg(iOrganisationID);
            return Json(lstMPAccount, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region NonAction Methods

        [NonAction]
        public bool RecordInvoiceTopupTransaction(int iMPActID, decimal dAmount, string strLoggedInUserEmail)
        {
            bool bResult = false;
            try
            {
                TopupBL objTopup = new TopupBL();
                bResult = objTopup.RecordInvoiceTopupTransaction(iMPActID, dAmount, PortalConstants.INVOICETOPUPCURRENCY, strLoggedInUserEmail, true);
            }
            catch (Exception ex)
            {
                LogExceptionInFile(iMPActID, dAmount, PortalConstants.INVOICETOPUPCURRENCY, strLoggedInUserEmail, ex);
            }
            return bResult;
        }


        public void LogExceptionInFile(int iMPActID, decimal dAmount, string strCurrency, string strLoggedInUserEmail, Exception ex)
        {
            if (!Directory.Exists(PortalConstants.INVOICE_LOGFILEPATH))
                Directory.CreateDirectory(PortalConstants.INVOICE_LOGFILEPATH);

            string strInvoiceLogFileName = "\\ADMIN_InvoiceErrorLog.txt";
            string strInvoiceLogFilePath = PortalConstants.INVOICE_LOGFILEPATH + strInvoiceLogFileName;
            System.IO.StreamWriter file = new System.IO.StreamWriter(strInvoiceLogFileName, true);

            file.WriteLine("/*-------------------------------------------------" + DateTime.Now.ToString("dd/mm/yyyy HH:mm:ss") + "-------------------------------------------------*/\n");
            file.WriteLine("MPAccountID         : " + iMPActID.ToString());
            file.WriteLine("Amount              : " + dAmount);
            file.WriteLine("Currency            : " + strCurrency);
            file.WriteLine("LoggedInUserEmail   : " + strLoggedInUserEmail);

            if (!String.IsNullOrEmpty(ex.Message))
                file.WriteLine("Exception  : " + ex.Message + Environment.NewLine);

            if (!String.IsNullOrEmpty(ex.StackTrace))
            {
                file.WriteLine("StackTrace :" + Environment.NewLine);
                file.WriteLine(ex.StackTrace + Environment.NewLine);
            }
            file.WriteLine("/*------------------------------------------------------------------------------------------------------------------*/\n");
            file.Close();
        }

        #endregion
    }
}
