using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SMSPortal.Models;

namespace SMSPortal.Repository.Organisation
{
    public interface ICustomPriceListRepository
    {
        List<CustomPriceTierDTO> GetCustomPriceList(int iOrgID);
        
        bool DeleteCustomTier(int iTierID);
        
        CustomPriceTierDTO GetCustomTierByID(int iTierID);
        
        bool AddCustomTier(int iOrgID, float fPricePerPence, int iBand);

        bool UpdateCustomTier(int iTierID, float fPricePerPence, int iBand);

        bool DoesBandExist(int iOrgID, int iBanding);

        bool DoesBandExistOnUpdate(int iOrgID, int iTierID, int iBanding);
    }
}
