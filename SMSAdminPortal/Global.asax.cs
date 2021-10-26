using System;
using System.Collections.Generic;
using System.Data.Entity;
//using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

//For Authorization
using System.Web.Security;
using System.Security.Principal;

using SMSAdminPortal.Commons;

namespace SMSAdminPortal
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Account", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_OnAuthenticateRequest(Object src, EventArgs e)
        {

            #region Commented code
            //if (HttpContext.Current.User != null)
            //{
            //    if (HttpContext.Current.User.Identity.IsAuthenticated)
            //    {
            //        if (HttpContext.Current.User.Identity is FormsIdentity)
            //        {
            //            FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;
            //            FormsAuthenticationTicket ticket = id.Ticket;
            //            string userData = ticket.UserData;

            //            // Roles is a helper class which places the roles of the
            //            // currently logged on user into a string array
            //            // accessable via the value property.
            //            Roles userRoles = new Roles(userData);

            //            HttpContext.Current.User = new GenericPrincipal(id, userRoles.Value);
            //        }
            //    }
            //}
            #endregion

            /*-------------------------------------------------------------------*/

            HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];

            // If the cookie can't be found, don't issue the ticket
            if (authCookie == null) return;
            //HttpContext.Current.Response.Cookies[PortalConstants.AUTHCOOKIE].Expires = DateTime.Now.AddDays(-1);
            // Get the authentication ticket and rebuild the principal & identity

            try
            {
                FormsAuthenticationTicket authTicket =  FormsAuthentication.Decrypt(authCookie.Value);                

                string[] UserData = authTicket.UserData.Split(new Char[] { '|' });                
                GenericIdentity userIdentity = new GenericIdentity(authTicket.Name);
                GenericPrincipal userPrincipal = new GenericPrincipal(userIdentity, UserData);
                Context.User = userPrincipal;
            }
            catch (System.Security.Cryptography.CryptographicException ce)
            {
                //If the cookie decryption failes - remove the cookie
                HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now.AddDays(-1);
            }           
        }

        protected void Application_EndRequest()
        {
            var context = new HttpContextWrapper(Context);
            // If there's an ajax request, and doing a 302, then we actually need to do a 401            
            if (Context.Response.StatusCode == 302 && context.Request.IsAjaxRequest())
            {
                Context.Response.Clear();
                Context.Response.StatusCode = 401;  // This will be picked up by ~/Scripts/SMSPortalCommon.js
            }
        }

        protected void Session_Start()
        {
            if (HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName] != null)
                HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now.AddDays(-1);

            //System.IO.StreamWriter file = new System.IO.StreamWriter("E:\\Mihir Mehta\\Session.txt", true);
            //file.WriteLine("----------------------------------");
            //file.WriteLine("In session start. ");
            //file.WriteLine("Time = " + DateTime.Now.ToString("hh:mm:ss"));
            //file.WriteLine("Session ID =" + Session.SessionID.ToString());
            //file.WriteLine("Email: " + SessionHelper.LoggedInUserEmail);
            //file.Close();
           
        }
    }
}