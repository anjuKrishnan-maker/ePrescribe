using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact.Utilities;
using Telerik.WebControls;
using Allscripts.ePrescribe.Objects;

namespace eRxWeb
{
public partial class Controls_ePAStatusInfo : BaseControl
{
    private string _patientFullName;
    private string _taskID;
    private string _medicationName;
    private Constants.ePATaskStatus _ePATaskStatus = Constants.ePATaskStatus.UNKNOWN;

    public string PatientFullName
    {
        get
        {
            return _patientFullName;
        }
        set
        {
            _patientFullName = value;
        }
    }
    public string MedicationName
    {
        get
        {
            return _medicationName;
        }
        set
        {
            _medicationName = value;
        }
    }
    public string TaskID
    {
        get
        {
            return _taskID;
        }
        set
        {
            _taskID = value;
        }
    }
    public Constants.ePATaskStatus EPATaskStatus
    {
        get
        {
            return _ePATaskStatus;
        }
        set
        {
            _ePATaskStatus = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnClose_Click(object sender, EventArgs e)
    {
        Hide();
    }
    protected void gridEPAStatus_ItemCommand(object source, Telerik.WebControls.GridCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "ViewAttachment":
                GridDataItem requestItem = (GridDataItem)e.Item;
                string documentName = requestItem.OwnerTableView.DataKeyValues[requestItem.ItemIndex]["DocumentName"].ToString();
                byte[] byteString = ((byte[])requestItem.OwnerTableView.DataKeyValues[requestItem.ItemIndex]["DocumentBody"]);
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + documentName);
                Response.AddHeader("Content-Length", byteString.Length.ToString());
                Response.ContentType = "application/octet-stream";
                Response.BinaryWrite(byteString);
                Response.End();
                ePAStatusHolder.Style["display"] = "inline";
                mpeEPAStatus.Show();
                break;
        }
    }
    protected void gridEPAStatus_ItemCreated(object sender, Telerik.WebControls.GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem tempDataItem = (GridDataItem)e.Item;
            Image attachmentImg = tempDataItem.FindControl("imageAttachmentType") as Image;
            if (attachmentImg != null)
            {
                switch ((Constants.ePAAttachmentType)Convert.ToInt32(tempDataItem.OwnerTableView.DataKeyValues[tempDataItem.ItemIndex]["DocumentType"].ToString()))
                {
                    case Constants.ePAAttachmentType.PDF:
                        attachmentImg.ImageUrl = "../images/pdf.jpg";
                        break;
                    case Constants.ePAAttachmentType.TIFF:
                        attachmentImg.ImageUrl = "../images/tiff.jpg";
                        break;
                    case Constants.ePAAttachmentType.JPEG:
                        attachmentImg.ImageUrl = "../images/jpeg.jpg";
                        break;
                    default:
                        break;
                }
            }
        }
    }
   
    public void Hide()
    {
        ePAStatusHolder.Style["display"] = "none";
        mpeEPAStatus.Hide();
    }

    public void Show()
    {
        clearEPAStatusInfo();
        loadEPAStatusInfo();
        ePAStatusHolder.Style["display"] = "inline";
        mpeEPAStatus.Show();
    }

    private void clearEPAStatusInfo()
    {
        litContactMessage.Text = string.Empty;
        litMedication.Text = string.Empty;
        litPatient.Text = string.Empty;
        litNotes.Text = string.Empty;
        litGeneralStatusNote.Text = string.Empty;
        gridEPAStatus.DataSource = null;
        gridEPAStatus.Visible = false;
    }
    private void loadEPAStatusInfo()
    {
        litPatient.Text = _patientFullName;
      
        DataSet statusDS;
        statusDS = ePA.GetTaskStatusInfo(_taskID, _ePATaskStatus, base.DBID);

        if (statusDS != null)
        {
            if (_ePATaskStatus == Constants.ePATaskStatus.EPA_FAILED)
            {
                litMedication.Text = String.Format("Medication Name: {0}", _medicationName); ;
                litContactMessage.Text = string.Empty;
                if ((statusDS.Tables.Count > 0) && statusDS.Tables[0].Rows.Count > 0)
                {
                    if (statusDS.Tables[0].Rows[0]["TransactionType"].ToString().Trim() == "Response")
                    {
                        litGeneralStatusNote.Text = string.Empty;
                        litNotes.Text = String.Format("Network error has occured with your {0} submission, please try again later.",
                            (Convert.ToInt32(statusDS.Tables[0].Rows[0]["TransactionID"]) == Convert.ToInt32(Constants.ePAResponseType.EPAINITIATION_RESPONSE))
                            ? "Question Set Response" : "ePA Response");

                    }
                    else
                    {
                        litGeneralStatusNote.Text = string.Empty;
                        litNotes.Text = String.Format("Network error has occured with your {0} submission, please try again later.",
                            (Convert.ToInt32(statusDS.Tables[0].Rows[0]["TransactionID"]) == Convert.ToInt32(Constants.ePARequestType.EPAINITIATION_REQUEST))
                            ? "Question Set Request" : "ePA Request");
                    }
                }
                else
                {
                    litNotes.Text = "Network error, please try again later.";
                }
            }
            else
            {
                litGeneralStatusNote.Text = (_ePATaskStatus == Constants.ePATaskStatus.EPA_DENIED)
                      ? "We're sorry, your ePA request has been denied."
                      : (_ePATaskStatus == Constants.ePATaskStatus.EPA_NOT_AVAILABLE) ? "Thank you for submitting.  ePA is not available for this patient or drug."
                      : (_ePATaskStatus == Constants.ePATaskStatus.EPA_CLOSED) ? "Thank you for submitting, your ePA request has been closed."
                      : (_ePATaskStatus == Constants.ePATaskStatus.EPA_APPROVED) ? "Thank you for submitting, your ePA request has been approved." : "";

                if ((statusDS.Tables.Count > 0) && statusDS.Tables[0].Rows.Count > 0)
                {
                    if ((_ePATaskStatus != Constants.ePATaskStatus.EPA_NOT_AVAILABLE) && (statusDS.Tables.Count > 2) && (statusDS.Tables[2].Rows.Count > 0))
                    {
                        string phoneNumber = string.Empty;

                        litMedication.Text = (statusDS.Tables[2].Rows[0]["Title"] != Convert.DBNull) ? statusDS.Tables[2].Rows[0]["Title"].ToString() : string.Empty;
                        string contactMsg = string.Empty;
                        contactMsg = (statusDS.Tables[2].Rows[0]["Description"] != Convert.DBNull) ? statusDS.Tables[2].Rows[0]["Description"].ToString().Trim() : string.Empty;
                        phoneNumber = (statusDS.Tables[2].Rows[0]["PhoneNumber"] != Convert.DBNull) ? statusDS.Tables[2].Rows[0]["PhoneNumber"].ToString() : string.Empty;
                        if (!string.IsNullOrWhiteSpace(phoneNumber))
                        {
                            contactMsg = contactMsg.EndsWith(".") ? contactMsg : (contactMsg + ".");
                            contactMsg += " Please Contact " + phoneNumber + " with questions.";
                        }
                        litContactMessage.Text = contactMsg;
                    }

                    if (statusDS.Tables[0].Rows.Count > 0)
                    {
                        litNotes.Text = statusDS.Tables[0].Rows[0]["Note"].ToString();
                    }
                    
                    if ((statusDS.Tables.Count > 1) && (statusDS.Tables[1].Rows.Count > 0))
                    {
                        gridEPAStatus.Visible = true;
                        gridEPAStatus.DataSource = statusDS.Tables[1];
                        gridEPAStatus.DataBind();
                    }
                }
              
            }
        }
    }
}

}