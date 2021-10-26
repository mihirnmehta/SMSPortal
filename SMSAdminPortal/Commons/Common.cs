using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;
using System.Web.Configuration;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using System.Web.Routing;
using SMSPortal.BusinessLogic;
using SMSPortal.BusinessLogic.Organisation;


namespace SMSAdminPortal.Commons
{

    public enum AdminAccessLevels
    {
        Admin = 1,
        AccountManagement = 2,
        AddPostPay = 3,
        AddTopUp = 4,
        ReadOnly = 5
    }

    public enum UserAccessLevels
    {
        Admin = 1,
        TopUp = 2
    }

    public class PortalConstants
    {
        public static string SMTPSERVERADDRESS      = ConfigurationManager.AppSettings["SMTPServerAddress"];
        public static string SMTPUSERNAME           = ConfigurationManager.AppSettings["SmtpUserName"];
        public static string SMTPPASSWORD           = ConfigurationManager.AppSettings["SmtpPassword"];
        public static int SMTPCLIENTPORT            = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpClientPort"]);

        public static string PASSWORDEMAILSUBJECTLINE = ConfigurationManager.AppSettings["PaswordEmailSubjectLine"];
        public static string PASSWORDEMAILFROMADDRESS = ConfigurationManager.AppSettings["PasswordEmailFromAddress"];
        
        public static int SERVICE_CODE            = Convert.ToInt32(ConfigurationManager.AppSettings["ServiceCode"]);
        public static int MINPASSWORDLENGTH       = Convert.ToInt32(ConfigurationManager.AppSettings["MinPasswordLength"]);
        public static decimal PRICELIST_MINPRICE  = Convert.ToDecimal(ConfigurationManager.AppSettings["Pricelist_MinPrice"]);

        public static string INVOICETOPUPCURRENCY = "GBP";
        public static string INVOICE_LOGFILEPATH  = ConfigurationManager.AppSettings["InvoiceLogFilePath"];
                
        #region Reset Password

        public static bool ResetPasswordForManagmentUser(string strEmail)
        {

            ManagementUserBL objMgmtUserBL = new ManagementUserBL();
            string strPassword = CommonFunctions.AutogeneratePassword();
            string strUpdatedBy = SessionHelper.LoggedInUserEmail;

            bool bIsPswdUpdated = objMgmtUserBL.UpdateMgmtUserPassword(strEmail, strPassword, strUpdatedBy);
            bool bResult = false;
            if (bIsPswdUpdated)
            {
                bResult = true;
                bResult = SendPasswordByEmail(strPassword, strEmail);
            }

            return bResult;
        }

        public static bool ResetPasswordForOrganisationUser(string strEmail)
        {
            string strPassword = CommonFunctions.AutogeneratePassword();

            OrganisationUserBL objManageOrgUsersBL = new OrganisationUserBL();

            bool bIsPswdUpdated = objManageOrgUsersBL.UpdateOrgUserPassword(strEmail, strPassword, SessionHelper.LoggedInUserEmail);
            bool bResult = false;
            if (bIsPswdUpdated)
            {
                bResult = true;
                bResult= SendPasswordByEmail(strPassword, strEmail);
            }
            return bResult;
        }

       
        public static bool SendPasswordByEmail(string strPassword, string strEmail)
        {
            OrganisationUserBL objManageOrgUsersBL = new OrganisationUserBL();

            string SMTPServerAddress = PortalConstants.SMTPSERVERADDRESS;
            string SMTPUserName      = PortalConstants.SMTPUSERNAME;
            string SMTPPassword      = PortalConstants.SMTPPASSWORD;
            int SMTPClientPort       = PortalConstants.SMTPCLIENTPORT;
            
            string PaswordEmailSubjectLine  = PortalConstants.PASSWORDEMAILSUBJECTLINE;
            string PasswordEmailFromAddress = PortalConstants.PASSWORDEMAILFROMADDRESS;
            

            #region Code for Email
            //try
            //{

                MailMessage objMail = new MailMessage();
                string EmailBody        = string.Empty;
                EmailBody += "Your SMS Portal Password has been reset. Please login with the below mentioned password.\n\n";
                EmailBody += "Email: " + strEmail + "\n\n";
                EmailBody += "Password: " + strPassword + "\n\n";
                EmailBody += "Please change your password after logging in.\n\n";

                SmtpClient client  = new SmtpClient();
                client.Host        = SMTPServerAddress;
                client.Credentials = new System.Net.NetworkCredential(SMTPUserName, SMTPPassword);
                client.Port        = SMTPClientPort; 
                
                client.Timeout        = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                
                objMail.To.Add(new MailAddress(strEmail));
                objMail.From       = new MailAddress(PasswordEmailFromAddress);
                objMail.Subject    = PaswordEmailSubjectLine;
                objMail.Body       = EmailBody;
                objMail.IsBodyHtml = true;
                
                client.EnableSsl = true;
                //client.Send(objMail);

                return true;
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}

            #endregion
        }

        #endregion
    }

    public static class AdminPortalRoles
    {
        public const string ADMIN = "Admin";
        public const string ACCOUNTMANAGEMENT = "AccountManagement";
        public const string ADDPOSTPAY = "AddPostPay";
        public const string ADDTOPUP = "AddTopUp";
        public const string READONLY = "ReadOnly";
    }

    public class MyAuthorize : AuthorizeAttribute
    {

        /*
         *  OnAuthorization method is the main that calls the other virtual methods, 
         *  it calls the AuthorizeCore to know whether the user is authenticated and authorized to access the controller or action. 
         *  The method returns false if the authorization fails and is it is then the OnAuthorization
         *  calls the HandleUnAuthorizeRequest to take appropriate action.
         */

        //public override void OnAuthorization(AuthorizationContext filterContext)
        //{
        //    base.OnAuthorization(filterContext);

        //    if (filterContext.Result is HttpUnauthorizedResult)
        //    {
        //        filterContext.Result = new RedirectToRouteResult(
        //            new RouteValueDictionary(new
        //            {
        //                controller = "Error",
        //                action = "UnAuthorizedAccess"
        //            }));

        //    }
        //}

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            HttpContextBase context = filterContext.RequestContext.HttpContext;

            if (context.User.Identity.IsAuthenticated)
            {
                context.Response.RedirectToRoute(new { controller = "Account", action = "UnAuthorizedAccess" });
            }
            else
            {
                // The RawUrl is defined as the part of the URL following the domain information.
                // In the URL string http://www.contoso.com/articles/recent.aspx, the RawUrl is /articles/recent.aspx.

                string strReturnURL = string.Empty;
                if (filterContext.HttpContext.Request != null)
                    strReturnURL = filterContext.HttpContext.Request.RawUrl;
                //strReturnURL = filterContext.HttpContext.Request.Url.AbsoluteUri;

                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.StatusCode = 401;
                }
                base.HandleUnauthorizedRequest(filterContext);

                //else
                //{
                //        Redirect the user to the login page
                //    context.Response.RedirectToRoute(new { controller = "Account", action = "Login", returnURL = strReturnURL });
                //}
            }
           
        }

        //protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        //{
        //    string url = string.Format("{0}?returnURL={1}", System.Web.Security.FormsAuthentication.LoginUrl,
        //        filterContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.RawUrl));
            
        //    if (filterContext.HttpContext.Request.IsAjaxRequest())
        //    {
        //        var redirectResult = filterContext.Result as RedirectResult;
        //        if (filterContext.Result is RedirectResult)
        //        {
        //            // It was a RedirectResult => we need to calculate the url
        //            var result = filterContext.Result as RedirectResult;
        //            url = UrlHelper.GenerateContentUrl(result.Url, filterContext.HttpContext);
        //        }
        //        else if (filterContext.Result is RedirectToRouteResult)
        //        {
        //            // It was a RedirectToRouteResult => we need to calculate
        //            // the target url
        //            var result = filterContext.Result as RedirectToRouteResult;
        //            url = UrlHelper.GenerateUrl(result.RouteName, null, null, result.RouteValues, RouteTable.Routes, filterContext.RequestContext, false);
        //        }
        //        filterContext.Result = new JsonResult
        //        {
        //            Data = new { Redirect = url },
        //            JsonRequestBehavior = JsonRequestBehavior.AllowGet
        //        };
        //    }
        //    else
        //    {
        //        //non-ajax request
        //        base.HandleUnauthorizedRequest(filterContext);
        //    }

        //}
    }


}