using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SMSPortal.Models;

namespace SMSPortal.Repository.Organisation
{
    public interface IOrganisationRepository
    {
        List<OrganisationDTO> GetSetupOrganisations(int CompanyID, string searchField, string searchOperator, string searchText, int pageNumber, int pageSize, out int TotalRecords, string sortColumn, string sortOrder);

        List<DropDownDTO> GetOrganisationsNotSetup(int CompanyID);

        bool AddOrganisation(int iOrganisationID, string strContactName, string strContactEmail, string strContactPhone, bool bPayPal, bool bInvoice, string strUpdatedBy);

        bool UpdateOrganisation(int iOrganisationID, string strContactName, string strContactEmail, string strContactPhone, bool bPayPal, bool bInvoice, string strUpdatedBy);

        OrganisationDTO GetOrganisationDetailsByID(int iOrganisationID);

        bool PayPalMPAccountExist(int iOrganisationID);
        
        bool InvoiceMPAccountExist(int iOrganisationID);
        
    }
}
