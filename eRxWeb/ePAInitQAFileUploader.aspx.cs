using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.Impact;

namespace eRxWeb
{
    public partial class ePAInitQAFileUploader : BasePage
    {
        private EPAInitQuestionSet _baseQuestionSet
        {
            get
            {
                EPAInitQuestionSet ret = new EPAInitQuestionSet();
                if (Session["ePACurrentInitiationQuestionSet"] != null)
                {
                    ret = (EPAInitQuestionSet)(Session["ePACurrentInitiationQuestionSet"]);
                }
                else
                {
                    ret = new EPAInitQuestionSet();
                }

                return ret;
            }
        }
        protected void Page_Init(object sender, EventArgs e)
        {
            lnkDefaultStyleSheet.Href += "?version=" + SessionAppVersion;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            maxAttachmentSize.InnerText = string.Format("{0} MB Max", ConfigKeys.NcpdpEpaAttachmentSize);
            checkForAllowedNumberOfAttachment();
        }

        private void checkForAllowedNumberOfAttachment()
        {
            if (_baseQuestionSet.Attachments.Count >= ConfigKeys.NcpdpEpaAttachmentCount)
            {
                btAttach.Disabled = true;
            }
            else
            {
                btAttach.Disabled = false;
            }
        }
        protected void removeFile(object sender, EventArgs e)
        {
            _baseQuestionSet.RemoveAttachment((Convert.ToInt32((sender as ImageButton).Attributes["AttachmentIndex"])), base.SessionUserID);
            if (_baseQuestionSet.Attachments.Count < ConfigKeys.NcpdpEpaAttachmentCount)
            {
                sizeWarning.Style["display"] = "none";
            }
            checkForAllowedNumberOfAttachment();
        }

        protected void file_DataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                ImageButton removeButton = (ImageButton)e.Item.FindControl("Remove");
                EPAInitAttachment attachment = (EPAInitAttachment)e.Item.DataItem;

                for (int i = 0; i < _baseQuestionSet.Attachments.Count; i++)
                {
                    if (attachment.FileName == _baseQuestionSet.Attachments[i].FileName &&
                        attachment.FileSize == _baseQuestionSet.Attachments[i].FileSize)
                    {
                        removeButton.Attributes.Add("AttachmentIndex", i.ToString());
                        break;
                    }
                }
            }
        }

        protected void btnProcessFile_Click(object sender, EventArgs e)
        {
            if (fileUploader.PostedFile != null)
            {
                string fileName = fileUploader.PostedFile.FileName;
                fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);

                byte[] input = new byte[fileUploader.PostedFile.ContentLength];

                int allowedAttachmentSizeInBytes = ConfigKeys.NcpdpEpaAttachmentSize * 1048576; // 1MegaBytes = 1048576 Bytes
                if ((_baseQuestionSet.AttachmentsSize + fileUploader.PostedFile.ContentLength) > allowedAttachmentSizeInBytes)
                {
                    sizeWarning.InnerText = string.Format("File cannot be added because it would put total attachment size over {0}MB.", ConfigKeys.NcpdpEpaAttachmentSize);
                    sizeWarning.Style["display"] = "inline";
                }
                else
                {
                    sizeWarning.Style["display"] = "none";
                    // Read the file into the byte array.
                    fileUploader.PostedFile.InputStream.Read(input, 0, fileUploader.PostedFile.ContentLength);

                    EPAInitAttachment newAttachment = new EPAInitAttachment(input, fileName, _baseQuestionSet.QuestionSetHeaderID, _baseQuestionSet.DBID);

                    _baseQuestionSet.AddAttachment(newAttachment, base.SessionUserID);

                }
            }

            btAttach.Value = "Attach";
            checkForAllowedNumberOfAttachment();
        }
    }
}