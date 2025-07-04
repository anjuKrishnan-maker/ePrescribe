using System;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI;
using Allscripts.Impact;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Shared.Logging;

namespace eRxWeb
{
public partial class Controls_MyeRxHeader : BaseControl
{
    #region Properties
    private static readonly ILoggerEx logger = LoggerEx.GetLogger(); 
    #endregion
    
    #region Page Event

    protected void Page_Load(object sender, EventArgs e)
    {
        //Set default radio button
        setDefaultRadioButton();


        if (Session["LICENSEID"] != null)
        {
            if (Session["UserHasHadEpcsPermissionInTheLastTwoYears"] != null && !Convert.ToBoolean(Session["UserHasHadEpcsPermissionInTheLastTwoYears"]))
            {
                rbtnMyEPCSReports.Visible = false;
            }
        }
    }

    protected void rbtnMyProfile_CheckedChanged(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PageNames.MY_PROFILE);
    }

    protected void rbtnMyEPCSReports_CheckedChanged(object sender, EventArgs e)
    {
            if (ConfigKeys.UseCommonProtectedStoreEPCSReports)
            {
                logger.Debug("Redirecting to Protected Store EPCS Reports");
                EPSBroker.AuditLogUserInsert(ePrescribeSvc.AuditAction.ACCESSING_PROTECTED_STORE_EPCS_REPORTS, SessionLicenseID, SessionUserID, SessionUserID, Request.UserIpAddress(), DBID);
                Response.Redirect(Constants.PageNames.PROTECTED_STORE_EPCS_REPORTS);
            }
            else
            {
                Response.Redirect(Constants.PageNames.MY_EPCS_REPORTS);
            }
        
    }
 
    #endregion

    #region Private Event
    
    private void setDefaultRadioButton()
    {
        string defaultRadioButton = ((System.Web.UI.TemplateControl)(this.Parent.Page)).AppRelativeVirtualPath.Split('/')[1].ToString();
        if (defaultRadioButton.Equals(Constants.PageNames.MY_PROFILE,StringComparison.OrdinalIgnoreCase))
        {
            rbtnMyProfile.Checked = true;
        }
        else if (defaultRadioButton.Equals(Constants.PageNames.MY_EPCS_REPORTS, StringComparison.OrdinalIgnoreCase))
        {
            rbtnMyEPCSReports.Checked = true;
        }
      
    }
    
    #endregion
}
}