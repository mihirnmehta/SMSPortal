using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSPortal.Models
{
    public class InvoiceDTO
    {
        public int OrgOpenAccountID;
        public int CompanyID;
        public string ProjectCode;
        public string BaseCompanyID;
        public string InvoiceCurrency;
        public string VATCode;
    }

    public class TransactionDTO
    {
        public int ID;
        public int MPAccountCode;
        public decimal TopupAmount;
        public string TransactionDate;
        public string InvoiceStatus;
        public string DateInvoiceExported;
    }


    public class OACurrencyDTO
    {
        public int CompanyID;
        public string Currency;
        public DateTime EffectiveDate;
        public float ConversionRate;
        public bool IsDeletedFromOA;
    }
}
