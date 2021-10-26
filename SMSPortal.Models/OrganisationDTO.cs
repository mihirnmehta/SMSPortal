using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSPortal.Models
{
    public class OrganisationDTO
    {
        // The following 6 fields come from the OA Database:

        //Company (Advanced company i.e. Advanced Learning, Advanced Health and Care etc)
        //Company ID (OA company ID ie 350 for Advanced Health and Care)
        //Customer Name (Customer company Name)
        //Open Accounts ID
        //Address
        //Invoice Currency
        /*--------------------------------------------------------------------------------------*/
        
        //This is how the above fields map:

        //---SMS Portal-------||----OA Database----
        // Organisation Name   =     Customer Name
        // OpenAccounts ID     =     OrganisationID in OA 
        // Account ID on Home  =     OrganisationID in OA 
        // Company             =     Branding Text

        public string OrganisationID;
        public string CompanyName;  // Advanced company i.e. Advanced Learning, Advanced Health and Care etc for Branding
        public int CompanyID;       // OA company ID ie 350 for Advanced Health and Care
        public string OrganisationName; // Organisation Name
        public int OrgOpenAccountID;  // ID of Organisation in OA Database
        public string Address;
        public string InvoiceCurrency;  // For Export Format

        public string ContactName;
        public string ContactEmail;
        public string ContactPhone;

        public bool PayPal;
        public bool Invoice;
        //public bool DirectDebit; // For phase 2.

        public int MPActCount;
        public int LowBalanceActCount;
        public bool CustomPLExist;        
    }
}
