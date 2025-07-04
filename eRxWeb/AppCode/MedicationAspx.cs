using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using Telerik.Web.UI;

namespace eRxWeb.AppCode
{
    public class MedicationAspx
    {
        public static void AddClientRowClickEvent(RadGrid radGrid)
        {
            radGrid.ClientSettings.ClientEvents.OnRowClick = "RowClick";
        }

        public static void SetSearchOptions(Constants.PrescriptionTaskType workflow, RadioButtonList rblSearch, HtmlGenericControl divShowCategory)
        {
            switch (workflow)
            {
                case Constants.PrescriptionTaskType.RXCHG_PRIORAUTH:
                case Constants.PrescriptionTaskType.RXCHG:
                {
                    var p = rblSearch.Items.FindByValue("P");
                    if (p != null) rblSearch.Items.Remove(p);

                    var m = rblSearch.Items.FindByValue("M");
                    if (m != null) rblSearch.Items.Remove(m);

                    divShowCategory.Visible = false;
                    break;
                }
            }
        }
    }
}