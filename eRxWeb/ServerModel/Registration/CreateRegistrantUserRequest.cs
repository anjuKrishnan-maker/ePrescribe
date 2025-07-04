using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Objects.Registrant;

namespace eRxWeb.ServerModel.Registration
{
    public class RegistrantUserRequest 
    {
        public RegistrantUser RegistrantUser { get; set; }

        public List<SecretAnswer> SecretAnswers { get; set; }
        
        public string Captcha { get; set; }

        public bool IsLinkExistingShieldUser { get; set; }
       
    }
}
