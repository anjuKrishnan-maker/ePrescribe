using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace eRxWeb
{
    public static class MessageBox 
    {
        public static void Show(this Page Page, String Message)
        {
            ScriptManager.RegisterClientScriptBlock(
               Page,
               Page.GetType(),
               "alert",
               "alert('" + Message + "')",
               true
            );
        }
    }
}