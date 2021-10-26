using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;

using SMSPortal.Models;
using SMSPortal.BusinessLogic;
using SMSPortal.BusinessLogic.Organisation;

using SMSAdminPortal.Commons;

namespace SMSAdminPortal.Controllers.Organisation
{
    public partial class OrganisationController : Controller
    {
       #region Organisation Price List

        public ActionResult CustomPriceList()
        {
            // Remove these line later
            //int iOrganisationID = Convert.ToInt32(Session["OrganisationID"].ToString()); 
            // ViewData["OrganisationID"] = iOrganisationID;             

            if(SessionHelper.OrganisationID == null)
                return RedirectToAction("ManageOrganisations");            
            else
                return View();
        }

        public JsonResult GetCustomPriceList(string sidx, string sord, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            int iTotalRecords = 0;
            int iPageIndex = Convert.ToInt16(page) - 1;
            int iPageSize = rows;

            CustomPriceListBL objCustomPriceListBL = new CustomPriceListBL();

            int iOrganisationID = SessionHelper.OrganisationID.Value;
            
            List<CustomPriceTierDTO> lstCustomPriceList = objCustomPriceListBL.GetCustomPriceList(iOrganisationID);
            
            if (sidx.Equals(""))
            {
                sidx = "Band";
            }

            List<CustomPriceTierDTO> lstSortedCustomPriceList = lstCustomPriceList.OrderBy(sidx, sord);
            
            iTotalRecords  = lstSortedCustomPriceList.Count;
            int totalPages = (int)Math.Ceiling((float)iTotalRecords / (float)iPageSize);

            var result = new
            {
                total = totalPages,
                page = page,
                records = iTotalRecords,
                rows = (from x in lstSortedCustomPriceList
                        select new
                        {
                            id = x.TierID.ToString(),
                            cell = new string[]
                            {
                                x.TierID.ToString(),
                                x.PricePerSMS.ToString(),
                                x.Band.ToString()
                            }
                        }).ToArray()
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string DeleteCustomTier(int iTierID)
        {
            CustomPriceListBL objCustomPriceListBL = new CustomPriceListBL();
            bool bResult = objCustomPriceListBL.DeleteCustomTier(iTierID);

            if (bResult)
                return "true";
            else
                return "false";
        }

        public ActionResult ShowAddEditPLDialog(int iTierID)
        {
            ViewData["TierID"] = iTierID;
            return PartialView("PopupAddCustomPriceList");
        }

        public string AddCustomTier(float fPricePerPence, int iBand)
        {
            int iOrganisationID = SessionHelper.OrganisationID.Value;

            CustomPriceListBL objCustomPriceListBL = new CustomPriceListBL();

            bool bResult = objCustomPriceListBL.AddCustomTier(iOrganisationID, fPricePerPence, iBand);

            if (bResult)
                return "true";
            else
                return "false";
        }

        public JsonResult GetCustomTierByID(int iTierID)
        {
            CustomPriceListBL objCustomPriceListBL = new CustomPriceListBL();
            CustomPriceTierDTO customPriceTier = objCustomPriceListBL.GetCustomTierByID(iTierID);
            return Json(customPriceTier);
        }

        public string UpdateCustomTier(int iTierID, float fPricePerPence, int iBand)
        {
            CustomPriceListBL objCustomPriceListBL = new CustomPriceListBL();
            bool bResult = objCustomPriceListBL.UpdateCustomTier(iTierID, fPricePerPence, iBand);

            if (bResult)
                return "true";
            else
                return "false";
        }

        public JsonResult DoesBandExist(int iBand)
        {
            int iOrgID = SessionHelper.OrganisationID.Value;

            CustomPriceListBL objCustomPriceListBL = new CustomPriceListBL();
            bool bResult = objCustomPriceListBL.DoesBandExist(iOrgID, iBand);

            if (bResult)
                return Json(true);
            else
                return Json(false);
        }

        public JsonResult DoesBandExistOnUpdate(int iTierID, int iBand)
        {
            int iOrgID = SessionHelper.OrganisationID.Value;

            CustomPriceListBL objCustomPriceListBL = new CustomPriceListBL();
            bool bResult = objCustomPriceListBL.DoesBandExistOnUpdate(iOrgID, iTierID, iBand);

            if (bResult)
                return Json(true);
            else
                return Json(false);
        }
        
       #endregion
    }
}
