using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SMSPortal.Models;
using SMSPortal.Repository;
using SMSPortal.DapperDAL;

namespace SMSPortal.BusinessLogic
{
    public class InvoiceBL
    {

        IInvoiceRepository iRepository = new InvoiceDAL();

        public BaseCompanyDTO GetBaseCompanyDetails(string sBaseCompanyID)
        {
            BaseCompanyDTO objBaseCompanyDetails = iRepository.GetBaseCompanyDetails(sBaseCompanyID);
            return objBaseCompanyDetails;
        }

        public void GetOAPeriods(int iCompanyID, out int PostingYear, out int PostingMonth)
        {
             iRepository.GetInvoicePostingInfo(iCompanyID, out PostingYear, out PostingMonth);            
        }

        
    }
}
