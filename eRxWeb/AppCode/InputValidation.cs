using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace eRxWeb.AppCode
{
    public class InputValidation
    {
        public static void SetRegexValidator(ref RegularExpressionValidator regexControl, string regex, string errorMessage)
        {
            regexControl.ErrorMessage = errorMessage;
            regexControl.ValidationExpression = regex;
        }
    }
}