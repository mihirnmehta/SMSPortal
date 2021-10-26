using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using SMSPortal.Models;

using SMSPortal.BusinessLogic;

using System.Configuration;
using System.Net.Mail;
using System.Net;

using SMSAdminPortal.Commons;

namespace SMSAdminPortal.Controllers.ManageManagementUser
{
    [MyAuthorize]
    public class ManagementUserController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("ManageManagementUsers");
        }
        
        public ActionResult ManageManagementUsers()
        {
            SessionHelper.ClearOrgSessionVariables();
            ViewData["LoginID"] = SessionHelper.LoginID;
            return View();
        }
                
        public ActionResult ShowAddEditManagementUsersDialog(int iManagementUserID)
        {
            ViewData["ManagementUserID"] = iManagementUserID;
            return PartialView("PopupAddEditMgmtUser");
        }

        #region CRUD Operations

        public JsonResult GetManagementUser(string sidx, string sord, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            int pageNumber = page;
            int iTotalRecords = 0;
            int iPageSize = rows;

            ManagementUserBL objManagementUserBL = new ManagementUserBL();

            int iAccessLevelID = 0;

            AdminAccessLevels eAccessLevel = 0;
            Enum.TryParse(SessionHelper.AccessLevel, out eAccessLevel);
            iAccessLevelID = (int)eAccessLevel;

            List<ManagementUserDTO> lstMgmtUsers = objManagementUserBL.GetManagementUsers(iAccessLevelID);

            if (_search)
            {
                lstMgmtUsers = lstMgmtUsers.FindAll(searchOper, searchField, searchString);
            }

            List<ManagementUserDTO> lstSorted = lstMgmtUsers.OrderBy(sidx, sord);
            iTotalRecords = lstSorted.Count;

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
                rows = (from x in lstSorted
                        select new
                        {
                            id = x.ManagementUserID.ToString(),
                            cell = new string[]
                          {
                              x.ManagementUserID.ToString(),
                             x.Forename.ToString(),
                              x.Email.ToString(),
                              x.PhoneNumber.ToString(),
                              x.AccessLevel.ToString(),
                          }
                        }).ToArray()
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserByID(int iManagementUserID)
        {
            ManagementUserBL objManagementUsers = new ManagementUserBL();
            ManagementUserDTO managementUser = objManagementUsers.GetUserByID(iManagementUserID);
            return this.Json(managementUser);
        }

        public string AddManagementUser(string strForename, string strSurname, string strContactEmailAddress, int iAccessLevelID, string strContactPhonenumber)
        {

            ManagementUserBL objManagementUserBL = new ManagementUserBL();

            string strPassword = CommonFunctions.AutogeneratePassword();

            string strUpdatedBy = SessionHelper.LoggedInUserEmail;

            bool bResult = objManagementUserBL.AddManagementUser(strForename, strSurname, strContactEmailAddress, iAccessLevelID, strContactPhonenumber, strPassword, strUpdatedBy);
            PortalConstants.SendPasswordByEmail(strPassword, strContactEmailAddress);            

            if (bResult)            
                return "true";
            else
                return "false";

        }

        public string UpdateManagementUser(int iMgmtUserID, string strContactEmailAddress, int iAccessLevelID, string strContactPhonenumber, string sLoggedInUserEmail)
        {
            ManagementUserBL objManagementUserBL = new ManagementUserBL();

            string strUpdatedBy = SessionHelper.LoggedInUserEmail;

            bool bResult = objManagementUserBL.UpdateManagementUser(iMgmtUserID, strContactEmailAddress, iAccessLevelID, strContactPhonenumber, strUpdatedBy);
            if (bResult)
            {
                //Reset the Session variable on logged in user record edit
                if (SessionHelper.LoggedInUserEmail.Equals(sLoggedInUserEmail))
                {
                    SessionHelper.LoggedInUserEmail = strContactEmailAddress;

                    AdminAccessLevels eAccessLevel = ((AdminAccessLevels)iAccessLevelID);
                    SessionHelper.AccessLevel = eAccessLevel.ToString();
                }
                return "true";
            }
            else
            {
                return "false";
            }
        }

        public JsonResult ResetPassword(string strEmail)
        {
            bool bResult = false;
            bResult = PortalConstants.ResetPasswordForManagmentUser(strEmail);

            if (bResult)
                return Json(true);
            else
                return Json(false);
        }

        public string DeleteManagementUser(int iManagementUserID)
        {
            ManagementUserBL objManagementUserBL = new ManagementUserBL();
            bool bResult = objManagementUserBL.DeleteManagementUser(iManagementUserID);
            if (bResult)
                return "true";
            else
                return "false";
        }

        #endregion

        #region Common Functions

        public JsonResult GetAdminPortalAccessLevels()
        {
            ManagementUserBL objManagementUserBL = new ManagementUserBL();

            int UserCurrentAccessLevelID = 0;
            
            AdminAccessLevels eAccessLevel = 0;
            Enum.TryParse(SessionHelper.AccessLevel, out eAccessLevel);
            UserCurrentAccessLevelID = (int)eAccessLevel;

            List<DropDownDTO> lstAccessLevel = objManagementUserBL.GetAdminPortalAccessLevels(UserCurrentAccessLevelID);
            return Json(lstAccessLevel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsEmailIDUnique(string strContactEmailAddress)
        {
            ManagementUserBL objManagementUserBL = new ManagementUserBL();

            bool bResult = objManagementUserBL.IsEmailIDUnique(strContactEmailAddress);

            if (bResult)
                return Json(true);
            else
                return Json(false);
        }

        public JsonResult IsEmailIDUniqueOnUpdate(int iManagementUserID, string strContactEmailAddress)
        {
            ManagementUserBL objManagementUserBL = new ManagementUserBL();

            bool bResult = objManagementUserBL.IsEmailIDUniqueOnUpdate(iManagementUserID, strContactEmailAddress);

            if (bResult)
                return Json(true);
            else
                return Json(false);
        }       
        
        #endregion

    }
}
