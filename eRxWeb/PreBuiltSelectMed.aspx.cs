using System;
using System.Web;
using System.Web.UI;
using System.Collections;
using System.Web.UI.HtmlControls;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using Telerik.Web.UI;

namespace eRxWeb
{
    public partial class PreBuiltSelectMed : BasePage
    {
        private string _groupID = string.Empty;

        #region Properties

        /// <summary>
        /// Get/Set Searched Medications List
        /// </summary>
        private ePrescribeSvc.Medication[] searchResults
        {
            get
            {
                ePrescribeSvc.Medication[] medications = null;

                if (isNewSearch || ViewState["MedSearchResults"] == null)
                {
                    if (!string.IsNullOrWhiteSpace(SearchText) && SearchText.Length > 0)
                    {
                        txtSearchMed.Text = SearchText;
                        medications = GetMedications(SearchText);
                        ViewState["MedSearchResults"] = medications;
                    }
                    else
                    {
                        txtSearchMed.Text = SearchText;
                        medications = new ePrescribeSvc.Medication[0];
                        ViewState["MedSearchResults"] = medications;
                    }
                }

                return (ePrescribeSvc.Medication[])ViewState["MedSearchResults"];
            }
            set
            {
                ViewState["MedSearchResults"] = value;
            }
        }

        /// <summary>
        /// Get/Set Medication Search Text
        /// </summary>
        public string SearchText
        {
            get
            {
                return (ViewState["MedSearchText"] != null) ? ViewState["MedSearchText"].ToString() : string.Empty;
            }
            set
            {
                ViewState["MedSearchText"] = value;
            }
        }

        /// <summary>
        /// Get - Is this is a new search?
        /// </summary>
        private bool isNewSearch
        {
            get
            {
                bool isNewSearch = false;

                if (!txtSearchMed.Text.Equals(SearchText, StringComparison.OrdinalIgnoreCase))
                {
                    isNewSearch = true;
                    SearchText = txtSearchMed.Text;
                }

                return isNewSearch;
            }
        }
        #endregion

        #region Page Methods


        private ePrescribeSvc.Medication[] GetMedications(string SearchText)
        {
            ePrescribeSvc.Medication[] response = EPSBroker.LoadMedicationByName(
                SearchText,
                base.DBID);

            return response;
        }

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            _groupID = PageState.GetStringOrEmpty("PreBuiltPrescriptionGroupID");
            ucMessage.Visible = false;
            Form.DefaultButton = btnSearch.UniqueID;

            if (!Page.IsPostBack)
            {
                txtSearchMed.Focus();
                if (PageState.GetBooleanOrFalse("AddAnotherMed"))
                {
                    ucMessage.Visible = true;
                    ucMessage.MessageText = "Successfully added script to the group.";
                    ucMessage.Icon = Controls_Message.MessageType.SUCCESS;
                    PageState.Remove("AddAnotherMed");
                }               
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ((PhysicianMasterPageBlank)Master).hideTabs();
        }

        protected void btnSelectSig_Click(object sender, EventArgs e)
        {
            if (rgMedication.SelectedItems.Count == 1)
            {
                GridDataItem selectedMedData = (rgMedication.SelectedItems[0] as GridDataItem);

                ArrayList rxList = new ArrayList();
                Rx rx = new Rx();
                rx.DDI = selectedMedData.GetDataKeyValue("DDI").ToString();
                rx.MedicationName = HttpUtility.HtmlDecode(selectedMedData.GetDataKeyValue("MedicationName").ToString());
                rx.Strength = selectedMedData.GetDataKeyValue("Strength").ToString();
                rx.StrengthUOM = selectedMedData.GetDataKeyValue("StrengthUOM").ToString();
                rx.DosageFormCode = selectedMedData.GetDataKeyValue("DosageFormCode").ToString();
                rx.RouteOfAdminCode = selectedMedData.GetDataKeyValue("RouteOfAdminCode").ToString();

                rxList.Add(rx);
                PageState["RxList"] = rxList;

                Response.Redirect(Constants.PageNames.PRE_BUILT_MED_SELECT_SIG + "?GroupId=" + HttpUtility.UrlEncode(_groupID));
            }
        }
        
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            rgMedication.DataSource = searchResults;
            rgMedication.DataBind();
        }

        protected void rgMedication_PageIndexChanged(object source, Telerik.Web.UI.GridPageChangedEventArgs e)
        {
            rgMedication.DataSource = searchResults;
            rgMedication.DataBind();
        }

        protected void rgMedication_OnItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem tempDataItem = (GridDataItem)e.Item;

                HtmlInputRadioButton rbSelect = tempDataItem.FindControl("rbSelect") as HtmlInputRadioButton;
                if (rbSelect != null)
                {
                    rbSelect.Attributes.Add("DDI", tempDataItem.GetDataKeyValue("DDI").ToString());
                    rbSelect.Attributes.Add("onclick", "medSelectRadio('" + tempDataItem.GetDataKeyValue("DDI").ToString() + "')");
                }
            }
        }

        #endregion

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PageNames.PRE_BUILT_PRESCRIPTION_ADD_OR_EDIT + "?GroupId=" + HttpUtility.UrlEncode(_groupID));
        }
    }
}