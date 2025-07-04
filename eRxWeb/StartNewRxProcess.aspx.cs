using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact.Utilities;
using eRxWeb.AppCode;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;

namespace eRxWeb
{
public partial class StartNewRxProcess : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        /////not serializable...
		///RxUser currentUser = ((RxIdentity)Session["UserIdentity"]).User;
        RxUser currentUser = new RxUser(base.SessionUserID, base.DBID);

        bool successful = true;

		//If the user is a provider, then set the patient pharmacy. 
        if (currentUser.IsProvider &&
           ((Request.QueryString["PatPharm"] != null && Request.QueryString["PatPharm"] == "Y") ||
           Request.QueryString["MailOrder"] != null && Request.QueryString["MailOrder"] == "Y"))
        {
            if (Session["RXID"] != null)
            {
                string rxID = Session["RXID"].ToString();
				successful = sendRxMessage(rxID);
            }

            if (Request.QueryString["MailOrder"] == "Y")
                Session["SentTo"] = Session["MOB_Name"].ToString();
            else
                Session["SentTo"] = Session["LASTPHARMACYNAME"].ToString();
        }
		else if
            (currentUser.IsDelegateProvider &&
            ((Request.QueryString["PatPharm"] != null && Request.QueryString["PatPharm"] == "Y") ||
            Request.QueryString["MailOrder"] != null && Request.QueryString["MailOrder"] == "Y"))
		{
			if (Session["RXID"] != null)
			{
				string rxID = Session["RXID"].ToString();
				//If there is no task associate with the script, then the script is ready to be send.
				//Otherwise script needs to be reviewed. 
				if (Session["TASKID"] == null)
				{
					successful = sendRxMessage(rxID);

                    if (Request.QueryString["MailOrder"] == "Y")
                        Session["SentTo"] = Session["MOB_Name"].ToString();
                    else
                        Session["SentTo"] = Session["LASTPHARMACYNAME"].ToString();
				}
			}
		}
      
        System.Text.StringBuilder msg = new System.Text.StringBuilder("");

        if (!successful)
        {
            msg.Append("The prescription failed to send. Please gather the patient and prescription details and call in the prescription.");
        }
        else
        {
            if (Session["CameFrom"] != null && Session["CameFrom"].ToString().ToLower().Contains(Constants.PageNames.SCRIPT_PAD.ToLower()))
            {
                var hiddenMsg = HttpUtility.ParseQueryString(Request.QueryString["From"].Replace(" ", "+")).Get("scriptpad.aspx?Msg");

                if (hiddenMsg != null)
                {
                    msg.Append(hiddenMsg);
                }
                else if (Request.QueryString["Msg"] != null)
                {
                    msg.Append(Request.QueryString["Msg"].ToString());
                }
                else
                {
                    msg.Append("Script pad successfully processed.");
                }
            }
            else
            {
                msg.Append("The Prescription for ");
                msg.Append((Session["MEDICATIONNAME"] != null) ? Session["MEDICATIONNAME"].ToString() + " " : "");
                msg.Append((Session["STRENGTH"] != null) ? Session["STRENGTH"].ToString() + " " : "");
                msg.Append((Session["STRENGTHUOM"] != null) ? Session["STRENGTHUOM"].ToString().Trim() + ", " : "");
                msg.Append((Session["QUANTITY"] != null) ? "#" + Session["QUANTITY"].ToString() + ", " : "");
                msg.Append("refills ");
                msg.Append((Session["REFILL"] != null) ? Session["REFILL"].ToString() : "");

                string sentTo = string.Empty;

                if (Session["SentTo"] != null)
                    sentTo = Session["SentTo"].ToString();

                if (!string.IsNullOrEmpty(sentTo) && sentTo.Equals("Printer"))
                {
                    string printInd = string.Empty;
                    if (Session["PrintInd"] != null)
                        printInd = Session["PrintInd"].ToString();

                    if (!string.IsNullOrEmpty(printInd) && printInd.Equals("Y"))
                    {
                        msg.Append(" has been sent to Printer");
                    }
                    else
                    {
                        msg.Append(" has been saved. Go to patient history to process the prescription.");
                    }
                }
                else
                {
                    msg.Append(" has been sent to ");
                    if (Session["SentTo"] != null)
                        msg.Append(Session["SentTo"].ToString());
                }
            }
        }

        base.ClearMedicationInfo(false);
        base.ClearPatientInfo();

        Session.Remove("RXID");
        Session.Remove(Constants.SessionVariables.TaskScriptMessageId);
        Session.Remove("NOTES");

        Session.Remove("PHARMACYID");

		Session.Remove("PrintInd");

		Session.Remove("TASKID");
		Session.Remove("TASKTYPE");

		Session.Remove("Rx"); //Remove the Prescription from the session. 
        Session.Remove("ORIGINALDDI");

        Session.Remove("SuccessMsg");

        // Google Analytics
        PlacementResponse = TieUtility.GetAdPlacement(Request.Cookies, new TieUtility(), new TIEServiceManager(), new ConfigurationManager());

        DefaultRedirect("Msg=" + Server.UrlEncode(msg.ToString().Replace("&amp;", "&")));        
     
    }
	private bool sendRxMessage(string rxID)
	{
		try
		{
            //**********************Added to update the SENTTOPHARMACYBY field AKS April 06 ***************
           //When you say send to pharmacy 
            string RxStatus = "SENTTOPHARMACY"; //Setting the status of the Prescription SENTBYID April 3 2007..
            Allscripts.Impact.Prescription.UpdateRxDetailStatus(Session["LICENSEID"].ToString(), Session["USERID"].ToString(), rxID, RxStatus, base.DBID);
            //********************************************************************************
            string scriptId = ScriptMessage.CreateScriptMessage(rxID, 1, Constants.MessageTypes.NEWRX, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.ShieldSecurityToken,base.SessionSiteID, base.DBID);
            if (scriptId != string.Empty)
            {
                if (Session["STANDING"].ToString() == "1")
                {
                    ScriptMessage.SendThisMessage(scriptId, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);
                }
            }

            scriptId = ScriptMessage.CreateDUREVTScriptMessage(rxID, 1, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), string.Empty, base.DBID);
            if ((Session["STANDING"].ToString() == "1") && (!string.IsNullOrEmpty(scriptId)))
            {
                ScriptMessage.SendOutboundInfoScriptMessage(scriptId, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);
            }

            bool successful = (scriptId != string.Empty);

            return successful;
		}
		catch (Exception)
		{
            return false;
		}
	}
}

}