using System;
using System.Web;


namespace SMSAdminPortal.Commons
{
    public static class SessionHelper
    {

        public static string LoggedInUserEmail
        {
            get {
                if (HttpContext.Current.Session[SessionKeys.USER_EMAILADDRESS] == null)
                {
                    return String.Empty;
                }
                return HttpContext.Current.Session[SessionKeys.USER_EMAILADDRESS].ToString();
            }
            set {
                HttpContext.Current.Session[SessionKeys.USER_EMAILADDRESS] = value;
            }
        }

        public static string LoginID
        {
            get
            {
                if (HttpContext.Current.Session[SessionKeys.USERID] == null)
                {
                    return String.Empty;
                }
                return HttpContext.Current.Session[SessionKeys.USERID].ToString();
            }
            set
            {
                HttpContext.Current.Session[SessionKeys.USERID] = value;
            }
        }

        public static string LoggedInUserFullName
        {
            get 
            {

                if (HttpContext.Current.Session[SessionKeys.USERFULLNAME] == null)
                {
                    return String.Empty;
                }
                return HttpContext.Current.Session[SessionKeys.USERFULLNAME].ToString();
            }
            set
            {
                HttpContext.Current.Session[SessionKeys.USERFULLNAME] = value;
            }
        }

        public static string LoggedInUserForeName
        {
            get
            {

                if (HttpContext.Current.Session[SessionKeys.USERFORENAME] == null)
                {
                    return String.Empty;
                }
                return HttpContext.Current.Session[SessionKeys.USERFORENAME].ToString();
            }
            set
            {
                HttpContext.Current.Session[SessionKeys.USERFORENAME] = value;
            }
        }

        public static int? OrganisationID
        {
            get 
            {

                if (HttpContext.Current.Session[SessionKeys.ORGANISATION_ID] == null)
                {
                    return null;
                }
                return Convert.ToInt32(HttpContext.Current.Session[SessionKeys.ORGANISATION_ID].ToString());
            }
            set 
            {
                HttpContext.Current.Session[SessionKeys.ORGANISATION_ID] = value;
            }
        }

        public static string OrganisationName
        {
            get
            {
                if (HttpContext.Current.Session[SessionKeys.ORGANISATION_NAME] == null)
                {
                    return String.Empty;
                }
                return HttpContext.Current.Session[SessionKeys.ORGANISATION_NAME].ToString();
            }
            set
            {
                HttpContext.Current.Session[SessionKeys.ORGANISATION_NAME] = value;
            }
        }

        public static string OrganisationAddress
        {
            get
            {
                if (HttpContext.Current.Session[SessionKeys.ORGANISATION_ADDRESS] == null)
                {
                    return String.Empty;
                }
                return HttpContext.Current.Session[SessionKeys.ORGANISATION_ADDRESS].ToString();
            }
            set
            {
                HttpContext.Current.Session[SessionKeys.ORGANISATION_ADDRESS] = value;
            }
        }

        public static string AccessLevel
        {
            get
            {
                if (HttpContext.Current.Session[SessionKeys.ACCESS_LEVEL] == null)
                {
                    return String.Empty;
                }
                return HttpContext.Current.Session[SessionKeys.ACCESS_LEVEL].ToString();
            }
            set
            {
                HttpContext.Current.Session[SessionKeys.ACCESS_LEVEL] = value;
            }
        }

        public static void ClearOrgSessionVariables()
        {
            SessionHelper.OrganisationID      = null;
            SessionHelper.OrganisationAddress = string.Empty;
            SessionHelper.OrganisationName    = string.Empty;
        }

    }
  
    public static class SessionKeys
    {
        public const string USER_EMAILADDRESS        = "UserEmailAddress";
        public const string USERID                   = "LoginID";
        public const string USERFULLNAME             = "LoginFullName"; //FirstName + Surname
        public const string USERFORENAME             = "LoginForeName";
        public const string ORGANISATION_ID          = "OrganisationID";
        public const string ORGANISATION_NAME        = "OrganisationName";
        public const string ORGANISATION_ADDRESS     = "OrganisationAddress";
        public const string ACCESS_LEVEL             = "AccessLevel";
    }
}