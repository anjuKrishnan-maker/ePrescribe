using Allscripts.Impact.Utilities;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.ServerModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

using static Allscripts.ePrescribe.Common.Constants;


namespace eRxWeb.AppCode.Users
{
    public class NewUserActivationCode : INewUserActivationCode
    {
        public bool EmailActivationCode(IEmail sendEmail,string userName, UserActivationInfoModel newUserActivationInfo)
        {        
            if(newUserActivationInfo!= null && newUserActivationInfo.PersonalEmail != null)
            {
                string xslPath = HttpContext.Current.Server.MapPath(ActivationCode.XsltTemplateUrl);
                StringWriter writer = sendEmail.CreateXsltTemplate(xslPath, userName, newUserActivationInfo.ActivationCode);
                string emailsubject = ConfigurationManager.GetValue(ActivationCode.ActivationEmailSubject).ToString();
                return sendEmail.Send(newUserActivationInfo.PersonalEmail, writer, emailsubject);
            }
            else
            {
                return false;
            }
        }                
    }
}