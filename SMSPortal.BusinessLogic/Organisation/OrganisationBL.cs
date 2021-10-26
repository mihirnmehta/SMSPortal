using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SMSPortal.Models;
using SMSPortal.Repository.Organisation;
using SMSPortal.DapperDAL.Organisation;

namespace SMSPortal.BusinessLogic.Organisation
{
    public class OrganisationBL
    {

        IOrganisationRepository OrgRepository = new OrganisationDAL();

        public OrganisationBL()
        { 

        }

        public List<OrganisationDTO> GetSetupOrganisations(int CompanyID,string searchField, string searchOperator, string searchText, int pageNumber, int pageSize, out int TotalRecords, string sortColumn, string sortOrder)
        {
            List<OrganisationDTO> lstOrganisations = OrgRepository.GetSetupOrganisations(CompanyID, searchField, searchOperator, searchText, pageNumber, pageSize, out TotalRecords, sortColumn, sortOrder);
            return lstOrganisations;
        }

        public List<DropDownDTO> GetOrganisationsNotSetup(int CompanyID)
        {
            List<DropDownDTO> lstOrganisations = OrgRepository.GetOrganisationsNotSetup(CompanyID);
            return lstOrganisations;
        }

        public bool AddOrganisation(int iOrganisationID, string strContactName, string strContactEmail, string strContactPhone, bool bPayPal, bool bInvoice, string strUpdatedBy)
        {
            bool bResult = OrgRepository.AddOrganisation(iOrganisationID, strContactName, strContactEmail, strContactPhone, bPayPal, bInvoice, strUpdatedBy);
            return bResult;
        }

        public bool UpdateOrganisation(int iOrganisationID, string strContactName, string strContactEmail, string strContactPhone, bool bPayPal, bool bInvoice, string strUpdatedBy)
        {
            bool bResult = OrgRepository.UpdateOrganisation(iOrganisationID, strContactName, strContactEmail, strContactPhone, bPayPal, bInvoice, strUpdatedBy);
            return bResult;
        }

        public OrganisationDTO GetOrgDetailsByID(int iOrganisationID)
        {
            OrganisationDTO objOrgDTO = OrgRepository.GetOrganisationDetailsByID(iOrganisationID);
            return objOrgDTO;
        }
        
        public bool PayPalMPAccountExist(int iOrganisationID)
        {
            bool bExist = OrgRepository.PayPalMPAccountExist(iOrganisationID);
            return bExist;
        }

        public bool InvoiceMPAccountExist(int iOrganisationID)
        {
            bool bExist = OrgRepository.InvoiceMPAccountExist(iOrganisationID);
            return bExist;
        }
    }
}
