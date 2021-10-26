using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using Dapper;

using SMSPortal.Repository;
using SMSPortal.Models;


namespace SMSPortal.DapperDAL
{
    public class GlobalPriceListDAL : IGlobalPriceListRepository
    {

        SqlConnection conn;

        public GlobalPriceListDAL()
        {
            conn = new SqlConnection(CommonFunctions.SMSPORTALCONNECTIONSTRING);
        }

        public List<GlobalPriceListDTO> GetGlobalPriceList()
        {
            List<GlobalPriceListDTO> lstGlobalPriceListDTOList = null;

            conn.Open();
            string sqlQuery = "SELECT TierID, PricePerSMS, Banding AS Band FROM tblGlobalPriceList";
            lstGlobalPriceListDTOList = (List<GlobalPriceListDTO>)conn.Query<GlobalPriceListDTO>(sqlQuery);
            conn.Close();

            return lstGlobalPriceListDTOList;
        }

        public bool DeletePriceList(int iTierID)
        {
            bool bIsDeleted = false;
            conn.Open();
            string sqlQuery = "DELETE FROM tblGlobalPriceList WHERE TierID=@iTierID";
            conn.Execute(sqlQuery, new { iTierID });
            conn.Close();
            bIsDeleted = true;
            
            return bIsDeleted;
        }

        public bool AddPriceList(float fPrice, int iBanding)
        {
            bool bIsAdded = false;
            
            conn.Open();
            string sqlQuery = "INSERT INTO tblGlobalPriceList(PricePerSMS,Banding) VALUES (@fPrice, @iBanding)";
            conn.Execute(sqlQuery, new { fPrice, iBanding});
            conn.Close();

            bIsAdded = true;
            
            return bIsAdded;
        }

        public GlobalPriceListDTO GetTierByID(int iTierID)
        {
            GlobalPriceListDTO objGlobalPriceTier = null;

            conn.Open();
            string sqlQuery = "Select TierID,PricePerSMS,Banding AS Band FROM tblGlobalPriceList WHERE TierID=@iTierID";
            objGlobalPriceTier = conn.Query<GlobalPriceListDTO>(sqlQuery, new { iTierID }).SingleOrDefault();
            conn.Close();

            return objGlobalPriceTier;
        }

        public bool UpdatePriceList(int iTierID, float fPrice, int iBanding)
        {
            bool bIsUpdated = false;
            
            conn.Open();
            string sqlQuery = "UPDATE tblGlobalPriceList SET PricePerSMS = @fPrice ,Banding = @iBanding WHERE TierID = @iTierID";
            conn.Execute(sqlQuery, new { fPrice, iBanding, iTierID});
            conn.Close();

            bIsUpdated = true;
            
            return bIsUpdated;
        }

        public bool DoesBandExist(int iBanding)
        {
            bool bExist = false;
            conn.Open();
            string strQuery = string.Format("SELECT COUNT(*) FROM tblGlobalPriceList WHERE Banding = @iBanding");

            int count = conn.Query<int>(strQuery, new { iBanding }).SingleOrDefault();

            if (count > 0)
                bExist = true;

            return bExist;
        }

        public bool DoesBandExistOnUpdate(int iTierID, int iBanding)
        {
            bool bExist = false;
            conn.Open();
            string strQuery = string.Format("SELECT COUNT(*) FROM tblGlobalPriceList WHERE TierID != @iTierID AND Banding = @iBanding");

            int count = conn.Query<int>(strQuery, new { iTierID, iBanding }).SingleOrDefault();

            if (count > 0)
                bExist = true;

            return bExist;
        }

    }
}
