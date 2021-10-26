using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SMSPortal.Models;

namespace SMSPortal.Repository
{
    public interface IGlobalPriceListRepository
    {
        List<GlobalPriceListDTO> GetGlobalPriceList();

        bool DeletePriceList(int iPriceListID);

        bool AddPriceList(float fPrice, int iBanding);

        bool UpdatePriceList(int iTierID, float fPrice, int iBanding);

        GlobalPriceListDTO GetTierByID(int iTierID);

        bool DoesBandExist(int iBanding);

        bool DoesBandExistOnUpdate(int iTierID, int iBanding);
    }
}
