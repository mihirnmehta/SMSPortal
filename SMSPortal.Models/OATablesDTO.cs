using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSPortal.Models
{
    public class BaseCompanyDTO
    {
        public string BaseCompanyID;
        public string CostCode;
        public string ExpenseCode;        
    }

    public class OAPeriodDTO
    {
        public int CompanyID;        
        public int PYear;
        public string PDates;
    }
}
