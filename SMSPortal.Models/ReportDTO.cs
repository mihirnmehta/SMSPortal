using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SMSPortal.Models
{
    //For FinanceReport and TopupReport
    public class InvoiceReportDTO
    {
        //public int OrganisationID;
        public string CustomerName; // Organisation Name
        //public int MPAccountID;
        public string MPAccountName; //Description(tblMPAccount)
       
        public DateTime TransactionDate;
                
        /// <summary>
        /// Returns the TransactionDate in the 'dd MMM yyyy HH:mm' format.
        /// </summary>
        public string sTransactionDate
        { 
            get {
                return TransactionDate.ToString("dd MMM yyyy HH:mm");
            }
        }
        
        public decimal Amount;
        
        /// <summary>
        /// Returns the Amount formatted to 2 decimal places. (#.## Format)
        /// </summary>
        public string sAmount
        {
            get
            {
                return Amount.ToString("F2");
            }
        }

        //public int BillingMethodID;
        public string BillingMethod;
        
        public DateTime? OAExportDate;

        /// <summary>
        /// Returns the OA ExportDate in the 'dd MMM yyyy HH:mm' format.
        /// </summary>
        public string sOAExportDate
        {
            get
            {
                if (OAExportDate == null)
                    return string.Empty;
                return OAExportDate.Value.ToString("dd MMM yyyy");
            }
        }
        
    }

    public class UsageReportDTO
    {
        public string MPAccountName; //Description(tblMPAccount)
        
        public DateTime Date; //DateTime(tblMicroPayment)

        /// <summary>
        /// Returns the Date in the 'dd MMM yyyy HH:mm' format.
        /// </summary>
        public string sDate {
            get
            {
                return Date.ToString("dd MMM yyyy HH:mm");
            }
        }
        public string StatementDescription;
        
        public decimal NetAmount;
        
        /// <summary>
        /// Returns the NetAmount formatted to 2 decimal places. (#.## Format)
        /// </summary>
        public string sNetAmount
        {
            get
            {
                return NetAmount.ToString("F2");
            }
        }
    }

    //For Usage Per Day Report in Admin portal
    public class UsagePerDayReportDTO
    {
        public DateTime Date;//DateTime(tblMicroPayment)

        /// <summary>
        /// Returns the Date in the 'dd MMM yyyy' format.
        /// </summary>
        public string sDate
        {
            get {
                return Date.ToString("dd MMM yyyy");
            }
        }
        
        public int TotalMessagesSentPerDay;
      
        public decimal TotalNetAmountPerDay;

        /// <summary>
        /// Returns the TotalNetAmountPerDay formatted to 2 decimal places. (#.## Format)
        /// </summary>
        public string sTotalNetAmountPerDay
        {
            get
            {
                return TotalNetAmountPerDay.ToString("F2");
            }
        }
    }


}
