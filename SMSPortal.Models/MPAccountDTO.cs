using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSPortal.Models
{
    public class MPAccountDTO
    {
        public int MPAccountCode;   
        public int OrganisationID;

        public bool IsEnabled;

        public string AccountLogin; //UserName
        public string Description;  //Account Name
        public decimal Balance;
        public string Password; //Encrypted Password
        public bool SendLowBalanceWarnings; //Enable Low Balance Warning
        public float BalanceWarningLimit;
        public string BalanceWarningEmail;

        public bool UsageRestriction;

        public string WorkDayAllowedFrom;
        public string WorkDayAllowedTo;
        public string NonWorkDayAllowedFrom;
        public string NonWorkDayAllowedTo;

        public bool Paypal;
        public bool Invoice;
    }
}
