using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SMSPortal.Models;
using SMSPortal.BusinessLogic;
using SMSPortal.BusinessLogic.Organisation;

using SMSAdminPortal.Commons;

namespace SMSAdminPortal.Controllers.Organisation
{
    [MyAuthorize]
    public partial class OrganisationController : Controller
    {
        //
        // GET: /Organisation/
        
        #region Organisation Home Page

        public ActionResult Index()
        {
            return RedirectToAction("ManageOrganisations");
        }

        public JsonResult GetListOfCompanies()
        {
            List<DropDownDTO> lstOfCompanies = CommonFunctions.GetListOfCompanies();

            return Json(lstOfCompanies, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManageOrganisations()
        {
            SessionHelper.ClearOrgSessionVariables();
            return View();
        }

        public JsonResult GetOrganisations(int CompanyID,string sidx, string sord, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            //sidx          - Sort Column, Value in the 'index' parameter.
            //sord          - Sort Order
            //page          - Current Page
            //rows          - Page size
            //_search       - true / false
            //searchField   - Value in the 'index' parameter.
            //searchOper    - eq, cn 
            //searchString 

            int iTotalRecords = 0;
            string sortColumn = sidx;
            string sortOrder  = sord;
            int pageNumber    = page;
            int pageSize      = rows;

            if (!_search)
            {
                searchField  = string.Empty;
                searchOper   = null;
                searchString = null;
            }

            OrganisationBL objOrgBL      = new OrganisationBL();
            List<OrganisationDTO> lstOrg = objOrgBL.GetSetupOrganisations(CompanyID, searchField, searchOper, searchString, pageNumber, pageSize, out iTotalRecords, sortColumn, sortOrder);

            int totalPages = (int)Math.Ceiling((float)iTotalRecords / (float)pageSize);

            //If the user enters a page number greater than total number of pages...
            if (pageNumber > totalPages)
                pageNumber = totalPages;

            var result = new
            {
                total   = totalPages,
                page    = pageNumber,
                records = iTotalRecords,

                rows = (from x in lstOrg
                        select new
                        {
                            id = x.OrganisationID.ToString(),
                            cell = new string[]
                            {
                                x.OrganisationID.ToString(),
                                x.OrganisationName,
                                x.OrgOpenAccountID.ToString(),
                                x.MPActCount.ToString(), 
                                x.CustomPLExist ? "Yes": "No",
                                x.PayPal ? "Yes" : "No",
                                x.Invoice ? "Yes" : "No"
                                //x.CustomPLExist.ToString()
                            }
                        }).ToArray()
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowAddOrganisationDialog(int CompanyID, int iOrganisationID)
        {
            ViewData["CompanyID"] = CompanyID;
            ViewData["OrganisationID"] = iOrganisationID;
            return PartialView("PopupAddEditOrg");
        }

        #endregion

        #region Organisation Details

       // [MyAuthorize(Roles = AdminPortalRoles.ADMIN)]
        public ActionResult OrganisationDetails(int? iOrganisationID)
        {
            if (iOrganisationID.HasValue)
                SessionHelper.OrganisationID = iOrganisationID.Value;

            if (SessionHelper.OrganisationID != null)
            {
                ViewData["OrganisationID"] = SessionHelper.OrganisationID;

                OrganisationDTO objOrgDTO = GetOrgDetails(SessionHelper.OrganisationID.Value);
                SessionHelper.OrganisationName    = objOrgDTO.OrganisationName;
                SessionHelper.OrganisationAddress = objOrgDTO.Address;

                return View();
            }
            else
                return RedirectToAction("ManageOrganisations");
        }

        public JsonResult GetOrganisationsNotSetup(int CompanyID)
        {
            OrganisationBL objOrgBL = new OrganisationBL();
            List<DropDownDTO> lstOrgNotSetup = objOrgBL.GetOrganisationsNotSetup(CompanyID);

            return Json(lstOrgNotSetup, JsonRequestBehavior.AllowGet);
        }

        #region CRUD Operations

        public string AddOrganisation(int iOrganisationID, string strContactName, string strContactEmail, string strContactPhone, bool bPayPal, bool bInvoice)
        {
            OrganisationBL objOrgBL = new OrganisationBL();

            bool bResult = objOrgBL.AddOrganisation(iOrganisationID, strContactName, strContactEmail, 
                                                    strContactPhone, bPayPal, bInvoice, SessionHelper.LoggedInUserEmail);

            if (bResult)
                return "true";
            else
                return "false";

        }

        public string UpdateOrganisation(string strContactName, string strContactEmail, string strContactPhone, bool bPayPal, bool bInvoice)
        {
            OrganisationBL objOrgBL = new OrganisationBL();

            bool bResult = objOrgBL.UpdateOrganisation(SessionHelper.OrganisationID.Value, strContactName, strContactEmail, 
                                                        strContactPhone, bPayPal, bInvoice, SessionHelper.LoggedInUserEmail);

            if (bResult)
                return "true";
            else
                return "false";

        }

        public JsonResult GetOrgDetailsByID()
        {
            int iOrgID = SessionHelper.OrganisationID.Value;

            OrganisationBL objOrgBL = new OrganisationBL();
            OrganisationDTO objOrgDTO = objOrgBL.GetOrgDetailsByID(iOrgID);
            
            SessionHelper.OrganisationName    = objOrgDTO.OrganisationName;
            SessionHelper.OrganisationAddress = objOrgDTO.Address;

            return Json(objOrgDTO, JsonRequestBehavior.AllowGet);
        }

        [NonAction]
        public OrganisationDTO GetOrgDetails(int iOrganisationID)
        {
            OrganisationBL objOrgBL = new OrganisationBL();
            OrganisationDTO objOrgDTO = objOrgBL.GetOrgDetailsByID(iOrganisationID);

            return objOrgDTO;
        }

        #endregion

        #endregion

        #region Functions

        public JsonResult PayPalMPActExist()
        {
            OrganisationBL objOrgBl = new OrganisationBL();
            bool bExist = objOrgBl.PayPalMPAccountExist(SessionHelper.OrganisationID.Value);

            if (bExist)
                return Json(true);
            else
                return Json(false);
        }

        public JsonResult InvoiceMPActExist()
        {
            OrganisationBL objOrgBl = new OrganisationBL();
            bool bExist = objOrgBl.InvoiceMPAccountExist(SessionHelper.OrganisationID.Value);

            if (bExist)
                return Json(true);
            else
                return Json(false);
        }

        #endregion

    }



    

}
