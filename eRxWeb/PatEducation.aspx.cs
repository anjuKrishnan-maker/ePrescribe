/****************************************************************************************************
**Change History
*****************************************************************************************************
**Date:         Author:         Description:
**-----------------------------------------------------------------------------
* 07/27/20009   Dharani Reddem  Fixed - Problem of getting blank page for patient ED sheet .
* 08/30/2012    Ravi Majji      Inheritance from BasePage was removed. When we display the page as popup window
*                               using window.open(),the parent window is postbacking as parent page is also
*                               inheriting from same BasePage.
******************************************************************************************************/

using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact;
using TieServiceClient;
using System.Web;
using System.Collections.Generic;
using System.Collections.Specialized;
using Allscripts.Impact.Utilities;
using eRxWeb.AppCode;

namespace eRxWeb
{
public partial class PatEducation : BasePage
{
    public IPlacementResponse PlacementResponse { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (Session["RxList"] != null)
            {
                ArrayList rxList = (ArrayList)Session["RxList"];
                Rx rx = (Rx)rxList[0];
                lblPatEdPrint.Visible = Convert.ToBoolean(Session["AutoPrintPatientEducation"]);

                if (rx.DDI != null)
                {
                    DataSet ds = CHPatient.GetPatientEducation(rx.DDI, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), (ConnectionStringPointer)Session["DBID"]);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        divGenericName.InnerHtml = dr["GenericMsg"].ToString().Replace("GENERIC NAME:", "<b>GENERIC NAME:</b>");
                        divCommon.InnerHtml = dr["CommMsg"].ToString().Replace("COMMON USES:", "<b>COMMON USES:</b>");
                        divHow.InnerHtml = dr["HowMsg"].ToString().Replace("HOW TO USE THIS MEDICINE:", "<b>HOW TO USE THIS MEDICINE:</b>");
                        divCautions.InnerHtml = dr["CautionsMsg"].ToString().Replace("CAUTIONS:", "<b>CAUTIONS:</b>");
                        divSideEffect.InnerHtml = dr["SideEffectMsg"].ToString().Replace("POSSIBLE SIDE EFFECTS:", "<b>POSSIBLE SIDE EFFECTS:</b>");
                        divBefore.InnerHtml = dr["BeforeMsg"].ToString().Replace("BEFORE USING THIS MEDICINE:", "<b>BEFORE USING THIS MEDICINE:</b>");
                        divOverDose.InnerHtml = dr["OverDoseMsg"].ToString().Replace("OVERDOSE:", "<b>OVERDOSE:</b>");
                        divAddition.InnerHtml = dr["AdditionMsg"].ToString().Replace("ADDITIONAL INFORMATION:", "<b>ADDITIONAL INFORMATION:</b>");
                    }
                }
            }
        }

        // Google Analytics
        PlacementResponse = TieUtility.GetAdPlacement(Request.Cookies, new TieUtility(), new TIEServiceManager(), new ConfigurationManager());
    }
}

}