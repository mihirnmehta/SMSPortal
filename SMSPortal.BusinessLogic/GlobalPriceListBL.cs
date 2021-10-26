using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SMSPortal.Models;
using SMSPortal.DapperDAL;
using SMSPortal.Repository;

namespace SMSPortal.BusinessLogic
{
    public class GlobalPriceListBL
    {
        IGlobalPriceListRepository iRepository = new GlobalPriceListDAL();

        public List<GlobalPriceListDTO> GetGlobalPriceList()
        {
            List<GlobalPriceListDTO> lstPL = iRepository.GetGlobalPriceList();

            return lstPL;
        }

        public bool DeletePriceList(int iPriceListID)
        {
            return iRepository.DeletePriceList(iPriceListID);
        }

        public bool AddPriceList(float fPrice, int iBanding)
        {
            return iRepository.AddPriceList(fPrice, iBanding);
        }

        public bool UpdatePriceList(int iTierID, float fPrice, int iBanding)
        {
            return iRepository.UpdatePriceList(iTierID, fPrice, iBanding);
        }

        public GlobalPriceListDTO GetTierByID(int iTierID)
        {
            return iRepository.GetTierByID(iTierID);
        }

        public bool DoesBandExist(int iBanding)
        {
            return iRepository.DoesBandExist(iBanding);
        }

        public bool DoesBandExistOnUpdate(int iTierID, int iBanding)
        {
            return iRepository.DoesBandExistOnUpdate(iTierID, iBanding);
        }
    }
}
