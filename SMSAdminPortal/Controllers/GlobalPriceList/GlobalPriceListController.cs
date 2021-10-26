using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SMSPortal.Models;
using SMSPortal.BusinessLogic;

using SMSAdminPortal.Commons;

namespace SMSAdminPortal.Controllers.GlobalPriceList
{
    [MyAuthorize]
    public class GlobalPriceListController : Controller
    {

        public ActionResult Index()
        {
            return RedirectToAction("GlobalPriceList");
        }

        public ActionResult GlobalPriceList()
        {
            SessionHelper.ClearOrgSessionVariables();
            return View();
        }

        public JsonResult GetGlobalPriceList(string sidx, string sord, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            int iTotalRecords = 0;
            int iPageIndex = Convert.ToInt16(page) - 1;
            int iPageSize = rows;

            GlobalPriceListBL objGlobalPriceListBL = new GlobalPriceListBL();
            List<GlobalPriceListDTO> lstGlobalPriceList = objGlobalPriceListBL.GetGlobalPriceList();

            if (sidx.Equals(""))
            {
                sidx = "Band";
            }

            List<GlobalPriceListDTO> lstSortedGlobalPriceList = lstGlobalPriceList.OrderBy(sidx, sord);

            iTotalRecords = lstSortedGlobalPriceList.Count;
            int totalPages = (int)Math.Ceiling((float)iTotalRecords / (float)iPageSize);

            var result = new
            {
                total = totalPages,
                page = page,
                records = iTotalRecords,
                rows = (from x in lstSortedGlobalPriceList
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

        public string DeletePriceList(int iPriceListID)
        {
            GlobalPriceListBL objGlobalPriceListBL = new GlobalPriceListBL();
            bool bResult = objGlobalPriceListBL.DeletePriceList(iPriceListID);

            if (bResult)
                return "true";
            else
                return "false";
        }

        public ActionResult ShowAddEditPLDialog(int iTierID)
        {
            ViewData["TierID"] = iTierID;
            return PartialView("PopupAddPriceList");
        }

        public string AddPriceList(float fPricePerPence, int iBand)
        {
            GlobalPriceListBL objGlobalPriceListBL = new GlobalPriceListBL();
            bool bResult = objGlobalPriceListBL.AddPriceList(fPricePerPence, iBand);

            if (bResult)
                return "true";
            else
                return "false";
        }

        public JsonResult GetTierByID(int iTierID)
        {
            GlobalPriceListBL objGlobalPriceListBL = new GlobalPriceListBL();
            GlobalPriceListDTO globalPriceTier = objGlobalPriceListBL.GetTierByID(iTierID);
            return this.Json(globalPriceTier);
        }

        public string UpdatePriceList(int iTierID, float fPricePerPence, int iBand)
        {
            GlobalPriceListBL objGlobalPriceListBL = new GlobalPriceListBL();
            bool bResult = objGlobalPriceListBL.UpdatePriceList(iTierID, fPricePerPence, iBand);

            if (bResult)
                return "true";
            else
                return "false";
        }

        public JsonResult DoesBandExist(int iBand)
        {
            GlobalPriceListBL objGlobalPriceListBL = new GlobalPriceListBL();
            bool bResult = objGlobalPriceListBL.DoesBandExist(iBand);

            if (bResult)
                return Json(true);
            else
                return Json(false);
        }

        public JsonResult DoesBandExistOnUpdate(int iTierID, int iBand)
        {
            GlobalPriceListBL objGlobalPriceListBL = new GlobalPriceListBL();
            bool bResult = objGlobalPriceListBL.DoesBandExistOnUpdate(iTierID, iBand);

            if (bResult)
                return Json(true);
            else
                return Json(false);
        }

    }
}
