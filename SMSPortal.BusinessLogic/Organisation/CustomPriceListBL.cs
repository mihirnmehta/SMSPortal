using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SMSPortal.Models;
using SMSPortal.Repository.Organisation;
using SMSPortal.DapperDAL.Organisation;


namespace SMSPortal.BusinessLogic.Organisation
{
    public class CustomPriceListBL
    {
        ICustomPriceListRepository iRepository = new CustomPriceListDAL();

        #region Custom Price List

        public List<CustomPriceTierDTO> GetCustomPriceList(int iOrgID)
        {
            List<CustomPriceTierDTO> lstcustomPriceList = iRepository.GetCustomPriceList(iOrgID);

            return lstcustomPriceList;
        }

        public bool DeleteCustomTier(int iTierID)
        {
            return iRepository.DeleteCustomTier(iTierID);
        }

        public CustomPriceTierDTO GetCustomTierByID(int iTierID)
        {
            return iRepository.GetCustomTierByID(iTierID);
        }

        public bool AddCustomTier(int iOrgID, float fPricePerPence, int iBand)
        {
            return iRepository.AddCustomTier(iOrgID, fPricePerPence, iBand);
        }

        public bool UpdateCustomTier(int iTierID, float fPricePerPence, int iBand)
        {
            return iRepository.UpdateCustomTier(iTierID, fPricePerPence, iBand);
        }

        public bool DoesBandExist(int iOrgID, int iBanding)
        {
            return iRepository.DoesBandExist(iOrgID, iBanding);
        }

        public bool DoesBandExistOnUpdate(int iOrgID, int iTierID, int iBanding)
        {
            return iRepository.DoesBandExistOnUpdate(iOrgID, iTierID, iBanding);
        }

        #endregion
    }
}
