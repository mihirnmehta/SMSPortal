using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SMSPortal.Models;
using SMSPortal.BusinessLogic;
using SMSPortal.BusinessLogic.Organisation;

using System.Configuration;
using System.Net.Mail;
using System.Net;


using SMSAdminPortal.Commons;

namespace SMSAdminPortal.Controllers.Organisation
{
    public partial class OrganisationController : Controller
    {
        
        public ActionResult OrganisationUser()
        {
            return View();

        }

        #region CRUD Operations

        public JsonResult GetOrganisationUsers(string sidx, string sord, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            int pageNumber = page;
            int iTotalRecords = 0;
            int iPageSize = rows;


            OrganisationUserBL objManageOrgUsersBL = new OrganisationUserBL();
            int iOrganisationID = (int)SessionHelper.OrganisationID;

            List<OrganisationUserDTO> lstOrgUsers = objManageOrgUsersBL.GetOrganisationUsers(iOrganisationID);

            if (_search)
            {
                lstOrgUsers = lstOrgUsers.FindAll(searchOper, searchField, searchString);
            }

            List<OrganisationUserDTO> lstSorted = lstOrgUsers.OrderBy(sidx, sord);
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
                            id = x.OrganisationUserID.ToString(),
                            cell = new string[]
                          {
                              x.OrganisationUserID.ToString(),                           
                              x.Forename.ToString(),                             
                              x.Email.ToString(),
                              x.AccessLevel.ToString()
                          }
                        }).ToArray()
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrganisationUserByID(int iOrganisationUserID)
        {
            OrganisationUserBL objManageOrgUsersBL = new OrganisationUserBL();
            OrganisationUserDTO manageOrgUsers = objManageOrgUsersBL.GetOrganisationUserByID(iOrganisationUserID);
            return this.Json(manageOrgUsers);
        }

        public string AddOrganisationUser(string strForename, string strSurname, string strEmail, int iAccessLevelID)
        {
            OrganisationUserBL objManageOrgUsersBL = new OrganisationUserBL();
            string strPassword = CommonFunctions.AutogeneratePassword();
            
            int iOrganisationID = (int)SessionHelper.OrganisationID;

            bool bResult = false;
            bResult = objManageOrgUsersBL.AddOrganisationUser(iOrganisationID, strForename, strSurname, strEmail, 
                                                                strPassword, iAccessLevelID, SessionHelper.LoggedInUserEmail);
            bResult = PortalConstants.SendPasswordByEmail(strPassword, strEmail);
            if (bResult)
                return "true";
            else
                return "false";
        }

        public string UpdateOrganisationUser(int iOrganisationUserID, string strEmail, int iAccessLevelID)
        {
            OrganisationUserBL objManageOrgUsersBL = new OrganisationUserBL();
            bool bResult = objManageOrgUsersBL.UpdateOrganisationUser(iOrganisationUserID, strEmail, iAccessLevelID, SessionHelper.LoggedInUserEmail);
            if (bResult)
                return "true";
            else
                return "false";
        }

        public JsonResult ResetPassword(string strEmail)
        {
            bool bResult = false;
            bResult = PortalConstants.ResetPasswordForOrganisationUser(strEmail);

            if (bResult)
                return Json(true);
            else
                return Json(false);
        }

        public string DeleteOrganisationUser(int iOrganisationUserID)
        {
            OrganisationUserBL objManageOrgUsersBL = new OrganisationUserBL();
            bool bResult = objManageOrgUsersBL.DeleteOrganisationUser(iOrganisationUserID);
            if (bResult)
                return "true";
            else
                return "false";

        }

        #endregion

        #region Common Functions

        public JsonResult GetUserPortalAccessLevels()
        {
            OrganisationUserBL objManageOrgUsersBL = new OrganisationUserBL();
            List<DropDownDTO> lstUserAccessLevel = objManageOrgUsersBL.GetUserPortalAccessLevels();
            return Json(lstUserAccessLevel, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ShowAddEditOrganisationUsersDialog(int iOrganisationUserID)
        {

            ViewData["OrganisationUserID"] = iOrganisationUserID;
            return PartialView("PopupAddEditOrgUser");

        }

        public JsonResult IsEmailIDUnique(string strEmail)
        {
            OrganisationUserBL objManageOrgUsersBL = new OrganisationUserBL();
            bool bResult = objManageOrgUsersBL.IsEmailIDUnique(strEmail);
            return Json(bResult);
        }

        public JsonResult IsEmailIDUniqueOnUpdate(int iOrganisationUserID, string strEmail)
        {
            OrganisationUserBL objManageOrgUsersBL = new OrganisationUserBL();
            bool bResult = objManageOrgUsersBL.IsEmailIDUniqueOnUpdate(iOrganisationUserID, strEmail);
            return Json(bResult);
        }

        #endregion

    }
}
