using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSPortal.Models
{
    public class ManagementUserDTO
    {
        public int ManagementUserID;
        public string Forename;
        public string Surname;
        public string Email;
        public string PhoneNumber;
        public string Password;
        public int AccessLevelID;
        public string AccessLevel;        
        public string UpdatedBy;
        public DateTime UpdatedDate;
    }

    public class OrganisationUserDTO
    {
        public int OrganisationUserID;
        public int OrganisationID;
        public string Forename;
        public string Surname;
        public string Email;
        public string Password;
        public int AccessLevelID;
        public string AccessLevel;
        public string UpdatedBy;
        public DateTime UpdatedDate;
    }
}
