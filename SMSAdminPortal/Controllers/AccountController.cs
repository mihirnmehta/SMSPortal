using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//For Authorization
using System.Web.Security;
using System.Security.Principal;


using SMSPortal.Models;
using SMSPortal.BusinessLogic;

using SMSAdminPortal.Commons;

namespace SMSAdminPortal.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }
                
        [HttpGet]
        public ActionResult Login(string returnURL)
        {
            // UrlReferrer is the URL of the previous webpage from which a link was followed.
            // It can be null - if you someone opened a browser and just entered your site address (without pressing a link to get there)
            //if (string.IsNullOrEmpty(returnURL) && Request.UrlReferrer != null)
            //    returnURL = Server.UrlEncode(Request.UrlReferrer.PathAndQuery);

            ViewData["ReturnURL"] = returnURL;
            return View();
        }
               
        [HttpPost]
        public ActionResult Login(string UserName, string Password, string returnURL)
        {
            string strFullName     = string.Empty;
            string strEmailAddress = string.Empty;
            string strAccessLevel  = string.Empty;

            ManagementUserDTO objUser;
            AccountManagementBL objAccMgmt = new AccountManagementBL();

            Password = CommonFunctions.Encrypt(Password);

            objUser = objAccMgmt.DoLogin(UserName, Password);

            if (objUser == null)
            {
                ViewData["ErrorMessage"] = "Invalid Username and Password combination.";
                return View();
            }
            else
            {
                strFullName = objUser.Forename + " " + objUser.Surname;
                strEmailAddress = objUser.Email;

                AdminAccessLevels eAccessLevel = ((AdminAccessLevels)objUser.AccessLevelID);
                strAccessLevel = eAccessLevel.ToString();

                SessionHelper.LoginID              = objUser.ManagementUserID.ToString();
                SessionHelper.LoggedInUserEmail    = UserName;
                SessionHelper.LoggedInUserFullName = strFullName;
                SessionHelper.LoggedInUserForeName = objUser.Forename;
                SessionHelper.AccessLevel          = strAccessLevel;

                #region Set the Forms Authentication Cookie

                FormsAuthenticationTicket tkt = new FormsAuthenticationTicket(1, strEmailAddress, DateTime.Now, DateTime.Now.AddMinutes(FormsAuthentication.Timeout.Minutes), false, strAccessLevel);
                
                string strEncryptedTkt = FormsAuthentication.Encrypt(tkt);  // This step is not necessary.
                
                
                HttpCookie authcookie = new HttpCookie(FormsAuthentication.FormsCookieName, strEncryptedTkt);
                authcookie.Expires = DateTime.Now.AddMinutes(FormsAuthentication.Timeout.Minutes);
                //authcookie.Secure = false;

                System.Web.HttpContext.Current.Response.Cookies.Add(authcookie);

                #endregion
            }

            string decodedUrl = string.Empty;
            if (!string.IsNullOrEmpty(returnURL))
                decodedUrl = Server.UrlDecode(returnURL);
                        
            if (Url.IsLocalUrl(decodedUrl))
                return Redirect(decodedUrl);
            else
                return Redirect("~/Organisation");
        }

        #region Change Password

        [MyAuthorize]
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [MyAuthorize]
        [HttpPost]
        public ActionResult ChangePassword(string CurrentPassword, string NewPassword)
        {

            bool bVerified = VerifyCurrentPassword(CurrentPassword);

            if (!bVerified)
            {
                ViewData["ErrorMessage"] = "Invalid current password.";
                return View();
            }

            AccountManagementBL objAccMgmtBL = new AccountManagementBL();
            string strLoggedInUserEmail      = SessionHelper.LoggedInUserEmail;

            bool bResult = objAccMgmtBL.DoChangePassword(strLoggedInUserEmail, NewPassword);
            if (bResult)
               ViewData["SuccessMessage"] = "Your password has been updated.";
            else
                ViewData["ErrorMessage"] = "Error occured while trying to change password. Please try again.";
            
            return View();
            
        }
                
        [NonAction]
        public bool VerifyCurrentPassword(string strPassword)
        {
            AccountManagementBL objAccMgmtBL = new AccountManagementBL();
            string sLoggedInUserEmail = SessionHelper.LoggedInUserEmail;
            bool bExist = objAccMgmtBL.VerifyCurrentPassword(sLoggedInUserEmail, strPassword);
            
            return bExist;
        }

        #endregion

        #region Forgot password

        public JsonResult CheckEmailExists(string strEmail)
        {
            AccountManagementBL objAccMgmtBL = new AccountManagementBL();
            bool bExist = objAccMgmtBL.CheckEmailExists(strEmail);
            if (bExist)
                return Json(true);
            else
                return Json(false);            
        }

        public JsonResult ResetPassword(string strEmail)
        {
            bool bResult = false;
            bResult = PortalConstants.ResetPasswordForManagmentUser(strEmail);
            return Json(bResult);
        }

        #endregion

        public ActionResult Logout()
        {
            //System.Web.HttpContext.Current.Response.Cookies[PortalConstants.AUTHCOOKIE].Clear();
            Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now.AddDays(-1);
            Session.Clear();    //Clears all session values only, User still retains the same session ID            
            Session.Abandon();  //Kills the session which results in Session_End event being fired in Global.asax.

            return RedirectToAction("Login");
        }

        public ActionResult Error()
        {
            return View();
        }

        #region Access Level Functions

        public JsonResult HasAdminAccess()
        {
            if (SessionHelper.AccessLevel == AdminPortalRoles.ADMIN)
            {
                return Json(true);
            }
            return Json(false);
        }

        public JsonResult HasAccountManagementAccess()
        {
            if (SessionHelper.AccessLevel == AdminPortalRoles.ACCOUNTMANAGEMENT)
            {
                return Json(true);
            }
            return Json(false);
        }

        public JsonResult HasAddPostPayAccess()
        {
            if (SessionHelper.AccessLevel == AdminPortalRoles.ADDPOSTPAY)
            {
                return Json(true);
            }
            return Json(false);
        }

        public JsonResult HasAddTopupAccess()
        {
            if (SessionHelper.AccessLevel == AdminPortalRoles.ADDTOPUP)
            {
                return Json(true);
            }
            return Json(false);
        }

        public JsonResult HasReadOnlyAccess()
        {
            if (SessionHelper.AccessLevel == AdminPortalRoles.READONLY)
            {
                return Json(true);
            }
            return Json(false);
        }

        #endregion
    }
}
