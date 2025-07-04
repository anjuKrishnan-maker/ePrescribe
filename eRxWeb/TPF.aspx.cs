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
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
public partial class TPF : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        DataTable dtLisState = RxUser.ChGetState(base.DBID);
        cmbState.DataSource = dtLisState;
        cmbState.DataTextField = "State";
        cmbState.DataValueField = "State";
        cmbState.DataBind();
    }
    protected void btnGo_Click(object sender, EventArgs e)
    {       
        ArrayList alProcess = new ArrayList();

        for (int i = 0; i < Convert.ToInt32(cmbCount.SelectedValue); i++)
        {
            if (i == 0)
                alProcess.Add("5230325A-A16E-4183-B0A5-071EC701C0FF");
            if (i == 1)
                alProcess.Add("8ed543b4-7481-4a9a-a566-844b38922233");
            if (i == 2)
                alProcess.Add("023391CF-00B2-4E96-ADEB-B828B15A1904");
            if (i == 3)
                alProcess.Add("030F9E36-34F1-4911-A127-C02BA3259A46");
            if (i == 4)
                alProcess.Add("DFEF83B4-1D7B-43AF-9DCB-34AAFE13565A");
            if (i == 5)
                alProcess.Add("7B40C0FB-41B0-490B-A145-3E144F385B68");
            if (i == 6)
                alProcess.Add("20D36D2C-AE38-457F-B3E2-AB85EE44B100");
            if (i == 7)
                alProcess.Add("4A3DE389-F35B-4B89-A947-AD1051B8B849");
            if (i == 8)
                alProcess.Add("A7A54915-A461-40DC-B8BB-D4861A940AD6");
            if (i == 9)
                alProcess.Add("CEDFE3F9-0AE3-435B-A25F-F72CD4A323CF");
            if (i == 10)
                alProcess.Add("5A54373D-EBCF-4437-836E-FE2B1E482386");
        }

        string printServer = string.Empty;
        if (rdoSystem.SelectedValue == "P")
            printServer = "&PDF=True";

        switch (rdoPage.SelectedValue)
        {
            case "M":
                Session["CameFrom"] = Constants.PageNames.TPF;
                Session["ProcessList"] = alProcess;
                Server.Transfer(Constants.PageNames.MULTIPLE_VIEW_CSS + "?StateOverride=" + cmbState.SelectedValue + "&PrintScript=YES&From=" + Constants.PageNames.TPF);
                break;
            default:
                Session["CameFrom"] = Constants.PageNames.TPF;
                Session["ProcessList"] = alProcess;
                if (Session["ProcessList"] != null)
                {
                    int rxCount = ((ArrayList)Session["ProcessList"]).Count;
                    if (rxCount > 0)
                    {
                        string[] RxIdArray;
                        RxIdArray = new string[rxCount];
                        int currRx = 0;
                        foreach (object item in (ArrayList)Session["ProcessList"])
                        {
                            if (item.GetType() == typeof(string))
                            {
                                RxIdArray[currRx] = item.ToString();
                                currRx++;
                            }
                        }
                        Session.Remove("ProcessList");
                        Session["RxIDArray"] = RxIdArray;
                        Session["MultipleRxID"] = RxIdArray[0];
                    }
                }
                Server.Transfer(Constants.PageNames.PRINT_SCRIPT_CSS + "?PrintBy=" + rdoPrintingOption.SelectedValue + "&StateOverride=" + cmbState.SelectedValue + printServer);
                break;
        }
    }
}

}