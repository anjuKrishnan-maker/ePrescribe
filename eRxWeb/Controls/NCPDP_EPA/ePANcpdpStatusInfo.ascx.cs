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
    public partial class Controls_ePANcpdpStatusInfo : BaseControl
    {
        public string PatientFullName { get; set; }
        public string MedicationName { get; set; }
        public string TaskID { get; set; }
        public Constants.ePATaskStatus EPATaskStatus { get; set; }
        

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
                    mpeEpaNcpdpStatus.Show();
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
                            attachmentImg.ImageUrl = "../../images/pdf.jpg";
                            break;
                        case Constants.ePAAttachmentType.TIFF:
                            attachmentImg.ImageUrl = "../../images/tiff.jpg";
                            break;
                        case Constants.ePAAttachmentType.JPEG:
                            attachmentImg.ImageUrl = "../../images/jpeg.jpg";
                            break;
                        case Constants.ePAAttachmentType.PNG:
                            attachmentImg.ImageUrl = "../../images/png.jpg";
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
            mpeEpaNcpdpStatus.Hide();
        }

        public void Show()
        {
            clearEPAStatusInfo();
            loadEPAStatusInfo();
            ePAStatusHolder.Style["display"] = "inline";
            mpeEpaNcpdpStatus.Show();
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
            litPatient.Text = PatientFullName;

            DataSet statusDS;
            statusDS = ePA.GetTaskNcpdpStatusInfo(TaskID, EPATaskStatus, base.DBID);

            if (statusDS != null)
            {
                if (EPATaskStatus == Constants.ePATaskStatus.EPA_FAILED)
                {
                    litMedication.Text = String.Format("Medication Name: {0}", MedicationName); ;
                    litContactMessage.Text = string.Empty;
                    if ((statusDS.Tables.Count > 0) && statusDS.Tables[0].Rows.Count > 0)
                    {
                        litGeneralStatusNote.Text = string.Empty;
                        litNotes.Text = String.Format("Error has occured with {0} transaction, please try again later by recreating ePA Task.",
                            statusDS.Tables[0].Rows[0]["TransactionType"].ToString());
                    }
                    else
                    {
                        litNotes.Text = "Error has occured with transaction, please try again later by recreating ePA Task.";
                    }
                }
                else
                {
                    litGeneralStatusNote.Text = (EPATaskStatus == Constants.ePATaskStatus.EPA_DENIED) ? "We're sorry, your ePA request has been denied."
                          : (EPATaskStatus == Constants.ePATaskStatus.EPA_CLOSED) ? "Thank you for submitting, your ePA request has been closed."
                          : (EPATaskStatus == Constants.ePATaskStatus.EPA_APPROVED) ? "Thank you for submitting, your ePA request has been approved." : "";

                    if ((statusDS.Tables.Count > 2) && (statusDS.Tables[2].Rows.Count > 0))
                    {
                        string phoneNumber = string.Empty;

                        litMedication.Text = (statusDS.Tables[2].Rows[0]["Title"] != Convert.DBNull) ? statusDS.Tables[2].Rows[0]["Title"].ToString() : string.Empty;
                        string contactMsg = string.Empty;
                        contactMsg = (statusDS.Tables[2].Rows[0]["Description"] != Convert.DBNull) ? statusDS.Tables[2].Rows[0]["Description"].ToString().Trim() : string.Empty;
                        phoneNumber = (statusDS.Tables[2].Rows[0]["PhoneNumber"] != Convert.DBNull) ? statusDS.Tables[2].Rows[0]["PhoneNumber"].ToString() : string.Empty;
                        if (!string.IsNullOrWhiteSpace(phoneNumber))
                        {
                            contactMsg = contactMsg.EndsWith(".") ? contactMsg : (contactMsg + ".");
                            contactMsg += "\n Please Contact " + phoneNumber + " with questions.";
                        }
                        litContactMessage.Text = contactMsg;
                    }

                    if ((statusDS.Tables.Count > 0) && (statusDS.Tables[0].Rows.Count > 0))
                    {
                        litNotes.Text = statusDS.Tables[0].Rows[0]["PANote"].ToString();
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