using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;

using Dapper;

using SMSPortal.Repository.Organisation;
using SMSPortal.Models;


namespace SMSPortal.DapperDAL.Organisation
{
    public class OrganisationDAL : IOrganisationRepository
    {
        public List<OrganisationDTO> GetSetupOrganisations(int CompanyID, string searchField, string searchOperator, string searchText, int pageNumber, int pageSize, out int TotalRecords, string sortColumn = "OrganisationName", string sortOrder = "ASC")
        {
            List<OrganisationDTO> lstOrganisations = new List<OrganisationDTO>();

            using (SqlConnection conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {

                string strTotalRecQuery = string.Format(@"SELECT count(*) FROM tblOrganisation 
                                                        WHERE CompanyID = {0} 
                                                            AND IsSetup = 1 
                                                            AND IsDeletedFromOA = 0", CompanyID);
                if(!String.IsNullOrEmpty(searchField))
                {
                    if(searchOperator == "cn")
                        strTotalRecQuery += string.Format(@" AND {0} like '%{1}%'", searchField, searchText);
                    else
                        strTotalRecQuery += string.Format(@" AND {0} = '{1}'", searchField, searchText);
                }

                TotalRecords = conn.Query<int>(strTotalRecQuery).Single();

                if (TotalRecords > 0)
                {
                    int TotalPages = (int)Math.Ceiling((float)TotalRecords / (float)pageSize);
                    if (pageNumber > TotalPages)
                        pageNumber = TotalPages;

                    var parameters = new DynamicParameters();
                    parameters.Add("@CompanyID", CompanyID);
                    parameters.Add("@sortColumn", sortColumn);
                    parameters.Add("@sortOrder", sortOrder);
                    parameters.Add("@searchColumn", searchField);
                    parameters.Add("@searchOperator", searchOperator);
                    parameters.Add("@searchText", searchText);
                    parameters.Add("@pageNumber", pageNumber);
                    parameters.Add("@pageSize", pageSize);

                    lstOrganisations = (List<OrganisationDTO>)conn.Query<OrganisationDTO>("usp_GetSetupOrganisations", parameters, commandType: CommandType.StoredProcedure);
                }
               
                #region CommentedCode
             
                /*
                string strQuery = string.Format(@"SELECT org.OrganisationID, 
		                                        org.OrganisationName, 
		                                        org.OrgOpenAccountID,
		                                        ISNULL(t.MPAccounts, 0) as MPActCount, 
		                                        CASE ISNULL(p.PLCount,0)		 
                                                      WHEN 0 then 0
                                                      ELSE 1
                                                      END as CustomPLExist,
                                                CASE WHEN( (select count(*) from tblOrgBillingMethod obm 
                                                            where obm.OrganisationID = org.OrganisationID and obm.BillingMethodID = {0})) = 0 
                                                     THEN CAST(0 AS BIT) 
                                                     ELSE CAST(1 AS BIT) 
                                                     END as PayPal,
		                                        CASE WHEN( (select count(*) from tblOrgBillingMethod obm 
                                                            where obm.OrganisationID = org.OrganisationID and obm.BillingMethodID = {1})) = 0 
                                                            then
                                                                CAST(0 AS BIT) 
                                                            else
                                                                CAST(1 AS BIT) 
                                                            end as Invoice
                                                FROM tblOrganisation org
                                                LEFT JOIN
	                                                (select OrganisationID, count(*) as MPAccounts 
		                                                from tblMPAccount
		                                                group by OrganisationID) t
                                                ON org.OrganisationID = t.OrganisationID
                                                LEFT JOIN
	                                                (select OrganisationID, count(*) as PLCount
		                                                from tblOrgPriceList
		                                                group by OrganisationID) p
                                                ON org.OrganisationID = p.OrganisationID
                                                WHERE org.IsSetup = 1 and org.IsDeletedFromOA = 0", (int)PaymentTypes.PayPal, (int)PaymentTypes.Invoice );
                
                  lstOrganisations = (List<OrganisationDTO>)conn.Query<OrganisationDTO>(strQuery);
                 
                 */
                #endregion

               
            }
            return lstOrganisations;
        }

        public List<DropDownDTO> GetOrganisationsNotSetup(int CompanyID)
        {
            List<DropDownDTO> lstOrganisations;

            using (SqlConnection conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                conn.Open();
                string strQuery = @"SELECT org.OrganisationID AS Value, org.OrganisationName AS Text, org.Address AS Attribute
		                                FROM tblOrganisation org
                                        WHERE org.CompanyID=@CompanyID
                                              AND
                                              org.IsSetUp = 'false'
                                              AND 
                                              org.IsDeletedFromOA = 'false'
                                        ORDER BY org.OrganisationName";
                lstOrganisations = (List<DropDownDTO>)conn.Query<DropDownDTO>(strQuery, new { CompanyID });
                conn.Close();

            }

            return lstOrganisations;
        }

        public bool AddOrganisation(int iOrganisationID, string strContactName, string strContactEmail, string strContactPhone, bool bPayPal, bool bInvoice, string strUpdatedBy) 
        {

            using (TransactionScope ts = new TransactionScope())
            {
                using (SqlConnection conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
                {
                   
                    conn.Open();

                    string strOrgQuery = string.Format("UPDATE tblOrganisation SET IsSetup = 1, UpdatedBy = '{0}', UpdatedDate=GETDATE() WHERE OrganisationID = '{1}'", strUpdatedBy, iOrganisationID);
                    conn.Execute(strOrgQuery);


                    string strContactQuery = @"INSERT INTO tblOrgContact(OrganisationID,  ContactName, ContactEmail, ContactPhone)
                                                    VALUES (@iOrganisationID, @strContactName, @strContactEmail, @strContactPhone)";
                    conn.Execute(strContactQuery, new { iOrganisationID, strContactName, strContactEmail, strContactPhone});

                    if (bPayPal)
                    {
                        string strBMQuery = string.Format("INSERT INTO tblOrgBillingMethod(OrganisationID,  BillingMethodID)"
                                                        + "VALUES ('{0}', '{1}')", iOrganisationID, (int)PaymentTypes.PayPal);
                        conn.Execute(strBMQuery);
                    }

                    if (bInvoice)
                    {
                        string strBMQuery = string.Format("INSERT INTO tblOrgBillingMethod(OrganisationID,  BillingMethodID)"
                                                        + "VALUES ('{0}', '{1}')", iOrganisationID, (int)PaymentTypes.Invoice);
                        conn.Execute(strBMQuery);
                    }

                    ts.Complete();
                    conn.Close();

                    return true;
                   
                }
            }
        }

        public bool UpdateOrganisation(int iOrganisationID, string strContactName, string strContactEmail, string strContactPhone, bool bPayPal, bool bInvoice, string strUpdatedBy)
        {

            using (TransactionScope ts = new TransactionScope())
            {
                using (SqlConnection conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
                {
                   
                    conn.Open();

                    string strOrgQuery = string.Format("UPDATE tblOrganisation SET UpdatedBy = '{0}', UpdatedDate=GETDATE() WHERE OrganisationID = '{1}'", strUpdatedBy, iOrganisationID);
                    conn.Execute(strOrgQuery);

                    string strContactQuery = @"UPDATE tblOrgContact 
                                               SET ContactName = @strContactName, ContactEmail = @strContactEmail, ContactPhone = @strContactPhone
                                               WHERE OrganisationID = @iOrganisationID";
                    conn.Execute(strContactQuery, new { strContactName , strContactEmail, strContactPhone, iOrganisationID} );

                    string strDelBillingMethods = string.Format("DELETE FROM tblOrgBillingMethod WHERE OrganisationID='{0}'", iOrganisationID);
                    conn.Execute(strDelBillingMethods);

                    if (bPayPal)
                    {
                        string strBMQuery = string.Format("INSERT INTO tblOrgBillingMethod(OrganisationID,  BillingMethodID)"
                                                        + "VALUES ('{0}', '{1}')", iOrganisationID, (int)PaymentTypes.PayPal);
                        conn.Execute(strBMQuery);
                    }

                    if (bInvoice)
                    {
                        string strBMQuery = string.Format("INSERT INTO tblOrgBillingMethod(OrganisationID,  BillingMethodID)"
                                                        + "VALUES ('{0}', '{1}')", iOrganisationID, (int)PaymentTypes.Invoice);
                        conn.Execute(strBMQuery);
                    }

                    ts.Complete();
                    conn.Close();
                    return true;                   
                }
            }


        }

        public OrganisationDTO GetOrganisationDetailsByID(int iOrganisationID)
        {
            OrganisationDTO objOrgDTO = new OrganisationDTO();

            using (SqlConnection conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {

                string strQuery = string.Format("select org.OrganisationID, org.OrganisationName, org.Address as [Address], "
                                               + "ct.ContactName, ct.ContactEmail, ct.ContactPhone, "
                                               + "case when( (select count(*) from tblOrgBillingMethod obm "
                                                    + "where obm.OrganisationID = org.OrganisationID "
                                                    + "and obm.BillingMethodID = 1)) = 0 "
                                                    + "then CAST(0 AS BIT) "
                                                    + "else CAST(1 AS BIT) "
                                                    + "end as PayPal, "
                                               + "case when( (select count(*) from tblOrgBillingMethod obm "
                                                    + "where obm.OrganisationID = org.OrganisationID "
                                                    + "and obm.BillingMethodID = 2)) = 0 "
                                                    + "then CAST(0 AS BIT) "
                                                    + "else CAST(1 AS BIT) "
                                                    + "end as Invoice "
                                               + "from tblOrganisation org "
                                               + "join tblOrgContact ct on org.OrganisationID = ct.OrganisationID "
                                               + "where org.OrganisationID = '{0}'", iOrganisationID);

                objOrgDTO = conn.Query<OrganisationDTO>(strQuery).Single();

                return objOrgDTO;
            }

        }

        public bool PayPalMPAccountExist(int iOrganisationID)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                conn.Open();
                string strQuery = string.Format(@"select count(*) 
                                                    from tblOrganisation org
                                                    join tblMPAccount mpa on org.OrganisationID = mpa.OrganisationId
                                                    join tblMPAccountBillingMethod abm on mpa.MPAccountCode = abm.MPAccountCode
                                                    where org.OrganisationID = {0} and abm.BillingMethodID = {1}", iOrganisationID, (int)PaymentTypes.PayPal);
                
                int count = conn.Query<int>(strQuery).Single();
                conn.Close();

                if (count > 0)
                    return true;
                else
                    return false;                
            }
            
        }

        public bool InvoiceMPAccountExist(int iOrganisationID)
        {
            using (SqlConnection conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                conn.Open();
                string strQuery = string.Format(@"select count(*) 
                                                    from tblOrganisation org
                                                    join tblMPAccount mpa on org.OrganisationID = mpa.OrganisationId
                                                    join tblMPAccountBillingMethod abm on mpa.MPAccountCode = abm.MPAccountCode
                                                    where org.OrganisationID = {0} and abm.BillingMethodID = {1}", iOrganisationID, (int)PaymentTypes.Invoice);

                int count = conn.Query<int>(strQuery).Single();
                conn.Close();

                if (count > 0)
                    return true;
                else
                    return false;
            }
        }
    }
}
