using System;
using System.Web.UI.WebControls;

namespace eRxWeb
{
public partial class Help_faq : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.Form.Attributes.Add("autocomplete", "off");
            ((HelpMasterPage)this.Master).CurrentPage = HelpMasterPage.HelpPage.FAQ;
        }

        // For not logged in users, Display the message for non logged in users " To see additional FAQ's , Please login above"
        hdnFldSessionUserId.Value = (string.IsNullOrEmpty(SessionUserID) ? "0" : SessionUserID);
    }

    protected void FAQ_NOT_TOP_FIVE_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //TFS#543821 - On FAQ page 13 thru 20 and number 25 will be behind the login credentials since they are are functional FAQ. 
        if (string.IsNullOrEmpty(SessionUserID))
        {
            //if ((e.Item.ItemIndex > 6 && e.Item.ItemIndex < 15) || (e.Item.ItemIndex == 25))
            //{
            //    e.Item.Visible = false;

            //}
            //TFS#543821 - On FAQ page 13 thru 20 and number 25 will be behind the login credentials since they are are functional FAQ. 
            // Based on FAQ number i.e between 157 to 163 and 171...
            int faq = Convert.ToInt32((((System.Data.DataRowView)(e.Item.DataItem))).Row.ItemArray[0].ToString());
            if (faq >= 157 && faq <= 163)
            {
                e.Item.Visible = false;
            }

            string questionCertificate = ((System.Data.DataRowView)(e.Item.DataItem)).Row.ItemArray[1].ToString();
            if (string.Compare(questionCertificate, "Where can I find the certificate showing Veradigm ePrescribe complies with the required DEA requirements and has passed the 3rd party audit?", true) == 0)
            {
                e.Item.Visible = false;
            }
            //(((System.Data.DataRowView)(e.Item.DataItem))).Row.ItemArray[0].ToString();
    
        }

        //"What is the pricing structure for electronic prescribing of controlled substances (EPCS)?" will not be displayed to the user. 
        string question=((System.Data.DataRowView)(e.Item.DataItem)).Row.ItemArray[1].ToString();
        if (string.Compare(question, "What is the pricing structure for electronic prescribing of controlled substances (EPCS)?", true) == 0)
        {
            e.Item.Visible = false;
        }
        //if (e.Item.ItemIndex == 18)
        //{
        //    e.Item.Visible = false;
        //}
    }


}


}