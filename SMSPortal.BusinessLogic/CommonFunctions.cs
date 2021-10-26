using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMSPortal.Models;

using CommonFunctionsDAL = SMSPortal.DapperDAL.CommonFunctions;

namespace SMSPortal.BusinessLogic
{
    public enum PaymentTypes
    {
        None = 0,
        PayPal = 1,
        Invoice = 2
    }
    
    public enum SortOrder
    {
        asc,
        desc
    }
       

    public static class CommonFunctions
    {
        public static string AutogeneratePassword()
        {
            int pNumber, pSpecialChar;

            string allowedChars = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";

            string allowedNumbers = "1,2,3,4,5,6,7,8,9,0";

            string allowedSpecialChars = "!,%,@,#,$,*,_";

            string[] arrAplhabets = allowedChars.Split(',');

            string[] arrNumbers = allowedNumbers.Split(',');

            string[] arrSpecialChars = allowedSpecialChars.Split(',');

            string strPassword = "";
            string temp        = "";
            int passwordLength = 8;
            string posArray    = "01234567";

            Random rand = new Random();            

           string randomChar = posArray.ToCharArray()[rand.Next(posArray.Length)].ToString();
            pNumber = int.Parse(randomChar); posArray = posArray.Replace(randomChar, "");
            
            randomChar = posArray.ToCharArray()[rand.Next(arrSpecialChars.Length-1)].ToString();
            pSpecialChar = int.Parse(randomChar); posArray = posArray.Replace(randomChar, "");

            for (int i = 0; i < passwordLength; i++)
            {
                if (i == pNumber)
                {
                    temp = arrNumbers[pNumber];
                }
                else if (i == pSpecialChar)
                {
                    temp = arrSpecialChars[pSpecialChar];
                }
                else
                {
                    temp = arrAplhabets[rand.Next(0, arrAplhabets.Length)];
                }
                strPassword += temp;
            }

            return strPassword;
        }

        #region Encrypt Decrypt Password

        /* Encrypt */
        public static string Encrypt(string sPassword)
        {
            StringBuilder sbEcryptedPassword = new StringBuilder("");
            string sHexChar;

            for (int I = sPassword.Length - 1; I >= 0; I--)
            {
                sHexChar = Convert.ToString(Convert.ToInt32(sPassword[I] + I - 29), 16);
                // Add on the leading 0 if it's missing
                sHexChar = sHexChar.PadLeft(2, '0');

                sbEcryptedPassword = sbEcryptedPassword.Append(sHexChar);
            }

            return sbEcryptedPassword.ToString().ToUpper();

           // return sPassword;
        }

        /* Decrypt */
        public static string Decrypt(string sEncryptedPassword)
        {
            if (sEncryptedPassword == null)
                return null;

            StringBuilder sbPassword = new StringBuilder("");

            try
            {
                int iStrLength = sEncryptedPassword.Length / 2;

                for (int I = iStrLength; I > 0; I--)
                {
                    StringBuilder sbHexString = new StringBuilder("");
                    sbHexString.Append(sEncryptedPassword, (I - 1) * 2, 2);
                    int iCharInt = Convert.ToInt32(sbHexString.ToString(), 16);
                    iCharInt = iCharInt + 29 - iStrLength + I;
                    sbPassword.Append(Convert.ToChar(iCharInt));
                }
            }
            catch (System.FormatException)
            {
            }
            return sbPassword.ToString();
            //return sEncryptedPassword;
        }

        #endregion

        /// <summary>
        /// Converts the dd/mm/yyyy format to yyyy-MM-dd
        /// </summary>
        /// <param name="sDateToConvert">Must be in dd/mm/yyyyy</param>
        /// <returns>Return date in yyyy-MM-dd format</returns>
        public static string ConvertDateToSQLFormatDate(string sDateToConvert)
        {
            IFormatProvider culture = new System.Globalization.CultureInfo("en-GB", true);
            DateTime dtDateToConvert = DateTime.Parse(sDateToConvert, culture);
            string sConvertedDate = dtDateToConvert.ToString("yyyy-MM-dd");
            return sConvertedDate;
        }

        public static List<DropDownDTO> GetListOfCompanies()
        {
            return CommonFunctionsDAL.GetListOfCompanies();
        }

    }

    public static class ExtensionMethods
    {
        public static List<T> OrderBy<T>(this List<T> lstSource, string SortColumn, string SortDirection)
        {

            List<T> lstSorted = lstSource;

            if (!string.IsNullOrEmpty(SortColumn) && lstSource != null && lstSource.Count > 0)
            {
                Type t = lstSource[0].GetType();

                if (SortDirection == SortOrder.asc.ToString())
                {
                    lstSorted =
                        lstSource.OrderBy(a => t.InvokeMember(SortColumn, System.Reflection.BindingFlags.GetField, null, a, null)).ToList();
                }
                else
                {
                    lstSorted =
                          lstSource.OrderByDescending(a => t.InvokeMember(SortColumn, System.Reflection.BindingFlags.GetField, null, a, null)).ToList();
                }
            }

            return lstSorted;
        }

        public static List<T> FindAll<T>(this List<T> lstSource, string SearchOperator, string SearchColumn, string SearchText)
        {
            List<T> lstSearched = lstSource;

            if (lstSource.Count > 0)
            {
            Type t = lstSearched[0].GetType();
                if (SearchOperator.Equals("eq"))
                {
                    lstSearched = lstSearched.FindAll(a => t.InvokeMember(SearchColumn, System.Reflection.BindingFlags.GetField, null, a, null).ToString().Equals(SearchText, StringComparison.CurrentCultureIgnoreCase));
                }
                if (SearchOperator.Equals("cn"))
                {
                    lstSearched = lstSearched.FindAll(a => (t.InvokeMember(SearchColumn, System.Reflection.BindingFlags.GetField, null, a, null)).ToString().ToLower().Contains(SearchText.ToLower()));
                }
            }
            return lstSearched;
        }
    }
}
