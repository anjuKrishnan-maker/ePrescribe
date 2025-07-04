// Revision History
/******************************************************************************
**Change History
*******************************************************************************
**Date:      Author:                    Description:
**-----------------------------------------------------------------------------
* 02/08/2010   Anand Kumar Krishnan     Defect#3335 - Task count is retained when Tool tab is selected. 
********************************************************************************/
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;

namespace eRxWeb
{
public partial class DeluxAdContainer : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
            adControl.RedirectFromSelectMedLink = true;
            adControl.FeaturedModule = Module.ModuleType.DELUXE;          
            adControl.Show = true;
        }   
}

}