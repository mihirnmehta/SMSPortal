using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using Dapper;

using SMSPortal.Models;
using SMSPortal.Repository.Organisation;

namespace SMSPortal.DapperDAL.Organisation
{
    public class CustomPriceListDAL : ICustomPriceListRepository
    {
        SqlConnection conn;

        public CustomPriceListDAL()
        {
            conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING);
        }

        public List<CustomPriceTierDTO> GetCustomPriceList(int iOrgID)
        {
            List<CustomPriceTierDTO> customPriceTierList = null;
            conn.Open();
            string sqlQuery = "SELECT TierID, OrganisationID, PricePerSMS, Banding AS Band FROM tblOrgPriceList where OrganisationID = @iOrgID";
            customPriceTierList = (List<CustomPriceTierDTO>)conn.Query<CustomPriceTierDTO>(sqlQuery, new { iOrgID });
            conn.Close();
            return customPriceTierList;
        }

        public bool DeleteCustomTier(int iTierID)
        {           
            conn.Open();
            string sqlQuery = "DELETE FROM tblOrgPriceList where TierID = @iTierID";
            conn.Execute(sqlQuery, new { iTierID });
            conn.Close();
         
            return true;
        }

        public CustomPriceTierDTO GetCustomTierByID(int iTierID)
        {
            CustomPriceTierDTO objCustomPriceTier = null;

            conn.Open();
            string sqlQuery = "SELECT TierID, OrganisationID, PricePerSMS, Banding AS Band FROM tblOrgPriceList where TierID = @iTierID";
            objCustomPriceTier = conn.Query<CustomPriceTierDTO>(sqlQuery, new { iTierID }).SingleOrDefault();
            conn.Close();

            return objCustomPriceTier;
        }

        public bool AddCustomTier(int iOrgID, float fPricePerPence, int iBand)
        {
            conn.Open();
            string sqlQuery = "INSERT INTO tblOrgPriceList (OrganisationID, PricePerSMS, Banding) VALUES(@iOrgID, @fPricePerPence, @iBand)";
            conn.Execute(sqlQuery, new { iOrgID,fPricePerPence,iBand});
            conn.Close();

            return true;            
        }

        public bool UpdateCustomTier(int iTierID, float fPricePerPence, int iBand)
        {
            conn.Open();
            string sqlQuery = "UPDATE tblOrgPriceList SET PricePerSMS = @fPricePerPence,Banding = @iBand WHERE TierID = @iTierID";
            conn.Execute(sqlQuery, new { fPricePerPence,iBand,iTierID});
            conn.Close();
            return true;           
        }

        public bool DoesBandExist(int iOrgID, int iBanding)
        {
            bool bExist = false;
            conn.Open();
            string strQuery = string.Format("SELECT COUNT(*) FROM tblOrgPriceList WHERE OrganisationID = @iOrgID AND Banding = @iBanding");

            int count = conn.Query<int>(strQuery, new { iOrgID, iBanding }).SingleOrDefault();

            if (count > 0)
                bExist = true;

            return bExist;
        }

        public bool DoesBandExistOnUpdate(int iOrgID, int iTierID, int iBanding)
        {
            bool bExist = false;
            conn.Open();
            string strQuery = string.Format("SELECT COUNT(*) FROM tblOrgPriceList WHERE OrganisationID = @iOrgID AND TierID != @iTierID AND Banding = @iBanding");

            int count = conn.Query<int>(strQuery, new { iOrgID, iTierID, iBanding }).SingleOrDefault();

            if (count > 0)
                bExist = true;

            return bExist;
        }
    }
}
