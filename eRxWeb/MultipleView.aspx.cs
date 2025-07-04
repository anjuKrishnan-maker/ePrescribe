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
using System.IO;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;


namespace eRxWeb
{
public partial class MultipleView : BasePage 
{

    string[] RxIdArray;// 
    Int64[] TaskIDArray;
    
    
    protected void Page_Load(object sender, EventArgs e)
    {

            if (!Page.IsPostBack)
		{
            checkPaper();

            if ((Request.QueryString["PrintScript"] != null && Request.QueryString["PrintScript"] == "YES") &&
                (Request.QueryString["From"] != null && Request.QueryString["From"].Contains(Constants.PageNames.SCRIPT_PAD.ToLower())))
            {
                //straightforward workflow. allow for cancelling here.
                btnCancel.Visible = true;
                btnCancel.Enabled = true;

                btnBack.Visible = false;

                btnPrint.Visible = true;
                btnPrint.Enabled = true;
            }
            else
            {
                //task workflow. do not allow for cancelling at this time.
                btnCancel.Visible = false;

                btnBack.Visible = true;
                btnBack.Enabled = true;

                btnPrint.Visible = true;
                btnPrint.Enabled = true;
            }

			Session["CurrentIndex"] = 0;

            //ProcessList is for non-task printed scripts
            if (Session["ProcessList"] != null)
            {
                int rxCount = ((ArrayList)Session["ProcessList"]).Count;
                if (rxCount > 0)
                {
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
                    if (RxIdArray.Length == 1)
                    {
                        btnPrevious.Visible = false;
                        btnNext.Visible = false;
                    }
                }
            }
            
            if (Session["HTTaskRxID"] == null)
            {
                //EAK removing because it's breaking nav, but why was it here in the first place?
                //Session["CameFrom"] = "";
                //Session["CameFrom"] = Request.Path; //Request.UrlReferrer.ToString();
            }             
            else
			// AFter printing and sending script the session to be cleared.(IMP)
			{
                    
				// Iterate through the Hashtable Extract the values and then updated the table.
				Hashtable htScriptTaskRxID = (System.Collections.Hashtable)Session["HTTaskRxID"];
				Session.Remove("HTTaskRxID");
				RxIdArray = new string[htScriptTaskRxID.Count];
				htScriptTaskRxID.Values.CopyTo(RxIdArray, 0);
				//EAK START
				TaskIDArray = new Int64[htScriptTaskRxID.Count];
				htScriptTaskRxID.Keys.CopyTo(TaskIDArray, 0);

				//EAK END
				//Dhiraj start
				Session["TaskIDArray"] = TaskIDArray;
				//dhiraj end

				Session["RxIDArray"] = RxIdArray;
				{
					Session["MultipleRxID"] = RxIdArray[0];// Always assigne the First Script and display by default..

					//EAK
					Session["MultipleTaskID"] = TaskIDArray[0];
					//END EAK

					//---Added code to supress the buttons when only one script is selected July 1st 2006
					if (RxIdArray.Length == 1)
					{
						btnPrevious.Visible = false;
						btnNext.Visible = false;
					}
					//---Added code to supress the buttons when only one script is selected July 1st 2006                  
				}

			}             
		}
		else
        {
            System.Threading.Thread.Sleep(1000);
            //If the page request is post back, first make sure the printed script task is deleted from site's task list. 
            // This code is used to identify which control has done the post back//
            //this method checks if the PRINT button has done the post back or not
            //if not then the Cancel button has done the POSTBACK//
            //Here we are not getting the ID but using the text of the control to determine this!!!.
            bool bFound=false ;

            btnCancel.Visible = false;
            btnBack.Visible = true;
            btnPrint.Enabled = false;

            if (Request.Params.Get("__EVENTTARGET") == "")
            {

                foreach (string ctl in Request.Form)
                {
                    Control c = FindControlInContainer(Page, ctl);
                    if (c is Button)
                    {
                        if (((Button)c).Text == "Print")
                        {
                            //do nothing as it's already handled
                            bFound = true;
                            break;
                        }
                    }
                }
            }
            if (bFound) //Yes the Post back has happend because of click on PRINT BUTTON
            {
            // This code is used to UPDATE the RX_DETAILs PRINTEDBYID with the logged in USERID value..
                //Changed it so that it will update the SenderBy field for having only 3 columns displayed April 12th 2007
                string[] RxID = Session["RxIDArray"] as string[];
                string RxStatus = "PRINTED"; //If you want to capture the PrintedBYID just uncomment this and then comment the senttopharmacy.
                //string RxStatus = "SENTTOPHARMACY";  //wtf
                Allscripts.Impact.Prescription.UpdatePrescriptionStatus(RxID[0].ToString(), 1, Constants.PrescriptionStatus.NEW, Session["USERID"].ToString(), base.DBID);
                string extFacilityCode = Session["ExtFacilityCd"] != null ? Session["ExtFacilityCd"].ToString() : null;
                Allscripts.Impact.Prescription.UpdateRxDetailStatus(Session["LICENSEID"].ToString(), Session["USERID"].ToString(), RxID[0].ToString(), RxStatus, extFacilityCode, base.DBID);
                sendPrintRxToSwitch(RxID[0].ToString());             
                //EAK added because they weren't deleting here
                DeleteCurrentTask();
            }
            else   // Else the POSTBACK has happend due to the Cancel Button.
            {
                DeleteCurrentTask();  
            }

        }
    }

    private void sendPrintRxToSwitch(string rxID)
    {
        try
        {
            string messageID = ScriptMessage.CreateScriptMessage(
                rxID,
                1,
                Constants.MessageTypes.PRINTRX,
                Session["LicenseID"].ToString(),
                Session["UserID"].ToString(),
                string.Empty, // base.ShieldSecurityToken,
                base.SessionSiteID,
                base.DBID);

            if (!string.IsNullOrEmpty(messageID))
            {
                if (Session["STANDING"].ToString() == "1")
                {
                    ScriptMessage.SendOutboundInfoScriptMessage(messageID, Session["LicenseID"].ToString(), Session["USERID"].ToString(), base.DBID);
                }
            }
            else
            {
                Audit.AddException(Session["UserID"].ToString(), Session["LicenseID"].ToString(), "Error sending PRTRX to switch: ScriptMessageID is null. RxID: " + rxID, Request.UserIpAddress(), "", "", base.DBID);
            }

            messageID = ScriptMessage.CreateDUREVTScriptMessage(rxID, 1, Session["LicenseID"].ToString(), Session["UserID"].ToString(), string.Empty, base.DBID);
            if ((Session["STANDING"].ToString() == "1") && (!string.IsNullOrEmpty(messageID)))
            {
                ScriptMessage.SendOutboundInfoScriptMessage(messageID, Session["LicenseID"].ToString(), Session["UserID"].ToString(), base.DBID);
            }
        }
        catch (Exception ex)
        {
            Audit.AddException(Session["UserID"].ToString(), Session["LicenseID"].ToString(), "Error sending PRTRX to switch: " + ex.ToString(), Request.UserIpAddress(), "", "", base.DBID);
        }
    }

    private Control FindControlInContainer(Control container, string controlId)
    {
        if (container.FindControl(controlId) != null)
            return container.FindControl(controlId);
        foreach (Control c in container.Controls)
        {
            Control controlFound = FindControlInContainer(c, controlId);
            if (controlFound != null)
                return controlFound;
        }
        return null;
    }

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        ((PhysicianMasterPageBlank)Master).hideTabs();
    }


    private void checkPaper()
    {
        if (Allscripts.Impact.SystemConfig.CheckSecurityPaper(Session["PRACTICESTATE"].ToString()))
        {
            //string msg = "Prescriptions printed in this state require special security paper.  If you have not obtained this special paper, please visit our paper vendor, <a target=_paper href='http://www.rxpaper.com/ePrescribe/'>http://www.rxpaper.com/ePrescribe/</a>.  Select the state you are practicing in and follow the link for instructions on ordering.  It is the users' responsibility to obtain and use the proper paper to comply with state requirements.";
            //panelMessageHeader.Style.Add("BACKGROUND", "#ad0808");
            //lblPrintingInfo.Style.Add("BACKGROUND", "#ad0808");
            //((LiteralControl)panelMessage.Controls[0]).Text = msg;
        }
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        RxIdArray = Session["RxIDArray"] as string[];
        TaskIDArray = Session["TaskIDArray"] as Int64[];
        int currenuIndex = Convert.ToInt32(Session["CurrentIndex"]);
        currenuIndex += 1;


        if (currenuIndex < RxIdArray.Length)
        {
            Session["MultipleRxID"] = RxIdArray[currenuIndex];
            Session["CurrentIndex"] = currenuIndex;
            Session["MultipleTaskID"] = TaskIDArray[currenuIndex];
        }
        if (currenuIndex == RxIdArray.Length - 1)
        {
            btnNext.Enabled = false;            
            btnPrevious.Enabled = true;
            

        }
        else
            if (currenuIndex > 0 && currenuIndex < RxIdArray.Length)
            {
                btnPrevious.Enabled = true;            
                btnNext.Enabled = true;            

            }
    }
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        RxIdArray = Session["RxIDArray"] as string[];
        TaskIDArray = Session["TaskIDArray"] as Int64[];
        int currenuIndex = Convert.ToInt32(Session["CurrentIndex"]);
        currenuIndex -= 1;

        if (currenuIndex > -1)
        {
            Session["MultipleRxID"] = RxIdArray[currenuIndex];
            Session["CurrentIndex"] = currenuIndex;
            Session["MultipleTaskID"] = TaskIDArray[currenuIndex];

        }
        if (currenuIndex == 0)
        {

            btnPrevious.Enabled = false;            
            btnNext.Enabled = true;
            

        }
        else
            if (currenuIndex > 0 && currenuIndex < RxIdArray.Length)
            {
                btnPrevious.Enabled = true;                
                btnNext.Enabled = true;
                
            }
    }
    protected void btnBack_Click(object sender, EventArgs e)
    {
        Session["REMOVETASKATPRINT"] = "N";
        Session.Remove("MultipleRxID");
        Session.Remove("MultipleTaskID");
          //Server.Transfer(Request.UrlReferrer.AbsolutePath);

        if (Session["CameFrom"].ToString().Contains(Constants.PageNames.TASK_SCRIPT_LIST))
            Server.Transfer(Constants.PageNames.TASK_SCRIPT_LIST);

        if (Session["CameFrom"].ToString().Contains(Constants.PageNames.PHARMACY))
        {
            //shouldn't ever go back to pharmacy
            //Server.Transfer("Pharmacy.aspx");
            
            //close out the task
            if (Session["TASKID"] != null && Session["TASKTYPE"] != null)
            {
                Int64 taskID = Convert.ToInt64(Session["TASKID"]);
                Prescription.UpdateRxTask(taskID, "", "", (int)Constants.PrescriptionTaskStatus.PROCESSED, Constants.PrescriptionStatus.NEW, Session["USERID"].ToString(), Session["LICENSEID"].ToString(), base.DBID);
            }
            Session["SentTo"] = "Printer";
            Session["PrintInd"] = "Y";
            Server.Transfer(Constants.PageNames.START_NEW_RX_PROCESS);
        }

		// Added by AKS as on Sep 13th 2006.
        if (Session["CameFrom"].ToString().Contains(Constants.PageNames.REVIEW_HISTORY))
        {
            string componentName = Allscripts.ePrescribe.Common.Constants.PageNames.REVIEW_HISTORY;
            string componentParameters = string.Empty;
            Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR +"?componentName="+componentName+"&componentParameters="+componentParameters);
        }

        if (Request.QueryString["To"] != null
            && (Request.QueryString["To"].ToString().Contains(Constants.PageNames.LIST_SEND_SCRIPTS)))
            Server.Transfer(Request.QueryString["To"].ToString());

		if (Request.QueryString["PrintScript"] != null && Request.QueryString["PrintScript"] == "YES")
		{
			Session["PrintInd"] = Request.Form["PrintInd"];
            Server.Transfer(Constants.PageNames.START_NEW_RX_PROCESS);
		}

		if (Request.QueryString["To"] != null)
			Server.Transfer(Request.QueryString["To"]);
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        //this button should only be enabled when the user came from the script pad
        Response.Redirect(Constants.PageNames.SCRIPT_PAD + "?from=" + Constants.PageNames.MULTIPLE_VIEW.ToLower());
    }

    public int getTableHeight()
    {
        int tableHeight = 0;
        if (Session["PAGEHEIGHT"] != null)
        {
            
                    tableHeight = Convert.ToInt32(Session["PAGEHEIGHT"]) - 220;
            
        }
        else
            Server.Transfer(Constants.PageNames.LOGOUT + "?Timeout=YES");
        return tableHeight;
    }

	protected void DeleteCurrentTask()
	{
		if (Session["REMOVETASKATPRINT"] != null && Session["REMOVETASKATPRINT"].ToString() == "Y")
		{
			int taskID = (Session["MultipleTaskID"] != null) ? (int)Session["MultipleTaskID"]: -1;
			string rxID = (Session["MultipleRxID"] != null) ? Session["MultipleRxID"].ToString() : string.Empty;
			string printInd = Request.Form["PrintInd"].Trim();

			//If the script is printed, then update the rx status and delete the task from user's task queue
			if (printInd.Equals("Y", StringComparison.OrdinalIgnoreCase))
			{
				//update the prescription after print
				if (!string.IsNullOrEmpty(rxID))
					Allscripts.Impact.Prescription.UpdatePrescriptionStatus(
						rxID,
						1,
                        Constants.PrescriptionStatus.NEW,
						Session["USERID"].ToString(),
                        base.DBID);

                if (taskID > 0)
                {
                    Prescription.UpdateRxTask(taskID, "", "", (int)Constants.PrescriptionTaskStatus.PROCESSED, Constants.PrescriptionStatus.NEW, Session["USERID"].ToString(), Session["LICENSEID"].ToString(), base.DBID);
                }
			}
		}
	}
} 
}