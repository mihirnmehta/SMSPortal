using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using SMSPortal.Models;
using System.Data.SqlClient;

using Dapper;

namespace SMSPortal.DapperDAL
{

    public enum PaymentTypes
    {
        None    = 0,
        PayPal  = 1,
        Invoice = 2
    }

    public enum InvoiceStatus
    { 
        InvoicePending,
        InvoiceRequestSent,
        InvoiceCreated
    }

    public static class CommonFunctions
    {
        public static string SMSPORTALCONNECTIONSTRING = ConfigurationManager.ConnectionStrings["SMSPortalConnectionString"].ConnectionString;


        public static List<DropDownDTO> GetListOfCompanies()
        {
            List<DropDownDTO> lstCompanies;
            using (SqlConnection conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING))
            {
                string sqlquery = "select distinct ID AS Value, CompanyName AS Text from OACompany";

                lstCompanies = (List<DropDownDTO>)conn.Query<DropDownDTO>(sqlquery);

                return lstCompanies;
            }

        }
    }


}
