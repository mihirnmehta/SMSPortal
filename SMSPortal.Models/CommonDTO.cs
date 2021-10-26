using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSPortal.Models
{
    public class DropDownDTO
    {
        public int Value;
        public string Text;
        public string Attribute;
    }

    public class BillingMethodDTO
    {
        public bool Paypal;
        public bool Invoice;
    }
}
