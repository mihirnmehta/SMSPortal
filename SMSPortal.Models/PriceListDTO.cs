using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSPortal.Models
{
    public class GlobalPriceListDTO
    {
        public int TierID;        
        public float PricePerSMS;
        public int Band;
    }

    public class CustomPriceTierDTO
    {
        public int TierID;
        public int OrganisationID;
        public float PricePerSMS;
        public int Band;
    }

}
