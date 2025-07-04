using eRxWeb.AppCode.Interfaces;
using eRxWeb.ePrescribeSvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel.Registration
{
    public class CreateUserRequest
    {
        public  string UserHostAddress { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PersonalEmail { get; set; }
        public string Password { get; set; }
        public RecoveryAnswerDto[] secretQuestionsField { get; set; }
        public string CapchaText { get; set; }
    }
}