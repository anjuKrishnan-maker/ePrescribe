using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace eRxWeb
{
/// <summary>
/// Contains utility methods for ScriptManager controls.
/// </summary>
public class ScriptManagerCommon
{
    private ScriptManagerCommon()
    {
        // Private constructor to prevent instantiation of this class
    }

    /// <summary>
    /// This method is the event handler target for the ScriptManagers in our Master pages.  We need
    /// the event handlers to ensure that AJAX-related errors are logged in the database.
    /// </summary>
    /// <param name="ex"></param>
    public static void LogException(Exception ex)
    {
        string errorMessage = String.Empty;
        errorMessage += @"<b>ASP.NET AJAX ERROR</b> :";
        errorMessage += @"<b> Error Message :</b>" + ex;                //Get the error message
        errorMessage += @"<b> Error Source :</b>" + ex.Source;          //Source of the message
        errorMessage += @"<b> Error Stack Trace :</b>" + ex.StackTrace; //Stack Trace of the error
        errorMessage += @"<b> Error TargetSite :</b>" + ex.TargetSite;  //Method where the error occurred

        Global.LogError(errorMessage);
    }
}

}