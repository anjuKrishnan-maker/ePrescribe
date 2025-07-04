using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using eRxWeb.Controls;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using eRxWeb.ePrescribeSvc;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Sec;
using Telerik.Web.UI;
using eRxWeb.ServerModel;
using System.Web.Script.Serialization;
using ShieldTraitName = eRxWeb.ePrescribeSvc.ShieldTraitName;
using eRxWeb.AppCode;

namespace eRxWeb
{
    public partial class EPCSRegistration : BasePage
    {
        protected Controls_Message ucMessage;

        #region Constants

        private const string ShowAllProviderViewValue = "ShowAllProviders";
        private const string CanPrescribeViewValue = "CanPrescribeAssignmentView";
        private const string GrantEpcsViewValue = "CanEnrollAssignmentView";
        private const string SearchByDeaViewValue = "SearchByDEA";
        private const string errorMessage =
            "There was a problem setting this permission for some of the selected providers; the display has been updated to represent the most recent user settings.";
        #endregion

        private enum View
        {
            NonAdmin,
            CanEnroll,
            CanPrescribe,
            SearchByDea,
            ShowAllProviders
        }

        private class Modes
        {
            internal const string Manage = "Manage";
        }


        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                var mode = Convert.ToString(Request.QueryString["Mode"]);
                if (PageState.GetBooleanOrFalse("IsAdmin"))
                {
                    ddlProviderView.Items.FindByValue(SearchByDeaViewValue).Enabled = true;
                }
                else
                {
                    panelDeaSearch.Visible = false;
                }

                if (!PageState.GetBooleanOrFalse("IsAdmin") && !PageState.GetBooleanOrFalse("EpcsApproverFromEditUser") && mode != Modes.Manage)
                {
                    var redirectUrl = GetRedirectForNonAdmin(Request.Url.ToString());
                    if (!string.IsNullOrEmpty(redirectUrl))
                    {
                        Response.Redirect(Constants.PageNames.UrlForRedirection(redirectUrl));
                    }
                    else
                    {
                        SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
                        {
                            PatientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)
                        };
                        RedirectToSelectPatient(null, selectPatientComponentParameters);
                    }
                }
                else if (PageState.GetBooleanOrFalse("EpcsApproverFromEditUser") || mode == Modes.Manage)
                {
                    ddlProviderView.SelectedIndex = 2;
                    if (!PageState.GetBooleanOrFalse("isAdmin"))
                    {
                        ShowHideButtonsByView(View.NonAdmin);
                    }
                    gvProviders.Rebind();
                    ResetView(null, null);
                }
            }
            ucDueDiligence.OnDueDiligenceAccepted+=ucDueDiligence_OnDueDiligenceAccepted;
            ucConfirmIdentCompromise.OnIdentityComprised += ucConfirmIdentCompromise_OnIdentityComprised;
        }

        private void ShowHideButtonsByView(View view)
        {
            bool isCanEnrollViewEnabled = true;
            bool isShowAllProvidersViewEnabled = true;

            bool areIdentityCompromisedBtnsVisible = true;
            bool isGrantEpcsBtnVisible = true;
            bool isApproveSigningBtnVisible = true;

            switch (view)
            {
                case View.NonAdmin:
                {
                    isCanEnrollViewEnabled = false;
                    isShowAllProvidersViewEnabled = false;
                    areIdentityCompromisedBtnsVisible = false;
                    isGrantEpcsBtnVisible = false;
                    break;
                }
                case View.CanEnroll:
                {
                    isApproveSigningBtnVisible = false;
                    areIdentityCompromisedBtnsVisible = false;
                    break;
                }
                case View.CanPrescribe:
                {
                    isGrantEpcsBtnVisible = false;
                    areIdentityCompromisedBtnsVisible = false;
                    break;
                }
                case View.SearchByDea:
                {
                    isGrantEpcsBtnVisible = false;
                    isApproveSigningBtnVisible = false;
                    break;
                }
                case View.ShowAllProviders:
                {
                    isGrantEpcsBtnVisible = false;
                    isApproveSigningBtnVisible = false;
                    areIdentityCompromisedBtnsVisible = false;
                    break;
                }
            }

            ddlProviderView.Items.FindByValue(GrantEpcsViewValue).Enabled = isCanEnrollViewEnabled;
            ddlProviderView.Items.FindByValue(ShowAllProviderViewValue).Enabled = isShowAllProvidersViewEnabled;

            btnGrantEpcsPrivilege.Visible = isGrantEpcsBtnVisible;
            btnApproveEpcsSigning.Visible = isApproveSigningBtnVisible;
            btnReinstateEpcs.Visible = areIdentityCompromisedBtnsVisible;
            btnSuspendEpcs.Visible = areIdentityCompromisedBtnsVisible;

            if (PageState.GetBooleanOrFalse("IsAdmin"))
            {
                btnApproveEpcsSigning.Visible = isApproveSigningBtnVisible;
                btnReinstateEpcs.Visible = areIdentityCompromisedBtnsVisible;
            }
        }


        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ((PhysicianMasterPageBlank)Master).hideTabs();
        }

        public void GetDataSource(object sender, GridNeedDataSourceEventArgs gridNeedDataSourceEventArgs)
        {
            gvProviders.DataSource = EPSBroker.GetAllProvidersAndTraitsForLicense(SessionLicenseID, null, EprescribeExternalAppInstanceID, ShieldSecurityToken);
        }

        public static string GetRedirectForNonAdmin(string requestUrl)
        {
            string redirect = string.Empty;
            if (!requestUrl.ToLower().Contains(Constants.PageNames.EPCS_REGISTRATION.ToLower()))
            {
                redirect = requestUrl;
            }

            return redirect;
        }

        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            var fromPage = Request.QueryString["From"];
            string status = Request.QueryString["Status"];
            string cameFrom = Request.QueryString["CameFrom"];
            
            HttpContext.Current.RewritePath(Request.Path, Request.PathInfo, string.Empty);
            PageState.Remove("EpcsApproverFromEditUser");            
            if (fromPage != null)
            {
                var redirectUrl = Constants.PageNames.UrlForRedirection(fromPage);
                // FORTIFY: Not considered an open re-direct as already filtered for local page above ^^
                if (redirectUrl == "EditUser")
                {
                    if (!string.IsNullOrEmpty(status))
                    {
                        string componentParameter = $"{{\"mode\": \"Edit\",\"status\": \"{status}\"}}";
                        Response.Redirect($"{ Constants.PageNames.REDIRECT_TO_ANGULAR}?componentName={Constants.PageNames.EDIT_USER}&componentParameters={componentParameter}");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(cameFrom))
                        {
                            cameFrom = Constants.PageNames.SELECT_PATIENT;
                        }
                        string componentParameter = $"{{\"mode\": \"Edit\",\"CameFrom\": \"{cameFrom}\"}}";
                        Response.Redirect($"{ Constants.PageNames.REDIRECT_TO_ANGULAR}?componentName={Constants.PageNames.EDIT_USER}&componentParameters={componentParameter}");
                    }
                }
                else
                    Response.Redirect(redirectUrl);
            }
            else
            {
                SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
                {
                    PatientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)
                };
                RedirectToSelectPatient(null, selectPatientComponentParameters);
            }
        }

        private void SetErrorMessage()
        {
            ucMessage.Visible = true;
            ucMessage.MessageText = errorMessage;
            ucMessage.Icon = Controls_Message.MessageType.ERROR;
        }

        private void ResetGrid()
        {
            btnSuspendEpcs.Visible = false;
            btnReinstateEpcs.Visible = false;
            btnGrantEpcsPrivilege.Visible = false;
            btnApproveEpcsSigning.Visible = false;
            ddlProviderView.SelectedIndex = 0;
            gvProviders.Rebind();
        }

        private List<UserNameWithUserGuidPair> BuildSelectedProviderList()
        {
            var providerList = new List<UserNameWithUserGuidPair>();
            foreach (GridDataItem selectedRow in gvProviders.SelectedItems)
            {
                var provider = new UserNameWithUserGuidPair
                {
                    UserName = ((HiddenField)selectedRow["colShieldUserName"].Controls[1]).Value,
                    UserGuid = ((HiddenField)selectedRow["colProviderGuid"].Controls[1]).Value
                };
                providerList.Add(provider);
            }

            return providerList;
        }

        private Boolean showMessage;

       void ucDueDiligence_OnDueDiligenceAccepted(DueDiligenceEventArgs dSEventArgs)
        {
            showMessage = false;

            if (dSEventArgs.Success)
            {
                if (dSEventArgs.IsCancel)
                {
                    ResetGrid();
                    return;
                }

                SetTraitsForMultipleProvidersAndResetGrid(ShieldTraitName.CanPrescribe, ShieldTraitValue.YES, dSEventArgs.UsersAccepted,
                    dSEventArgs.OtpSecurityToken, dSEventArgs.IdentitySecurityToken, SessionLicenseID, SessionUserID);
            }
                
            else
            {
                ResetGrid();
                showMessage = true;
                ucMessage.Visible = true;
                ucMessage.MessageText = dSEventArgs.Message;
                ucMessage.Icon = Controls_Message.MessageType.ERROR;
            }
        }

        void ucConfirmIdentCompromise_OnIdentityComprised(IdentityCompromiseEventArgs icEventArgs)
        {
            if (icEventArgs.ConfirmChange == Controls_ConfirmIdentCompromise.ConfirmIdentityChange.ConfirmChangeNo)
            {
                ddlProviderView.SelectedValue = SearchByDeaViewValue;
            }
            else if (icEventArgs.ConfirmChange == Controls_ConfirmIdentCompromise.ConfirmIdentityChange.ConfirmChangeYes)
            {
                var isInstitutional =  AppCode.StateUtils.UserInfo.GetIdProofingMode(PageState) == ShieldTenantIDProofingModel.Institutional;

                if (icEventArgs.IdentityAction == Controls_ConfirmIdentCompromise.IdentityActionEnum.Compromise)
                {
                    var providers = BuildSelectedProviderListForCompIdentity(Controls_ConfirmIdentCompromise.IdentityActionEnum.Compromise);
                    var idCompromised = new ShieldTraitInfo { TraitName = ShieldTraitName.IsIdentityCompromised, TraitValueEnum = ShieldTraitValue.YES };
                    bool actionSuccess = EPSBroker.SetTraitForMultipleProviders(providers, idCompromised, EprescribeExternalAppInstanceID, null, null, 
                        ShieldSecurityToken, null, null, isInstitutional);
                    if (!actionSuccess)
                    {
                        SetErrorMessage();
                    }
                }
                else
                {
                    var providers = BuildSelectedProviderListForCompIdentity(Controls_ConfirmIdentCompromise.IdentityActionEnum.Secure);
                    var idCompromised = new ShieldTraitInfo { TraitName = ShieldTraitName.IsIdentityCompromised, TraitValueEnum = ShieldTraitValue.NO };
                    bool actionSuccess = EPSBroker.SetTraitForMultipleProviders(providers, idCompromised, EprescribeExternalAppInstanceID, null, null, ShieldSecurityToken, null, null, isInstitutional);
                    if (!actionSuccess)
                    {
                        SetErrorMessage();
                    }
                }

                ResetGrid();
            }
        }

        private List<UserNameWithUserGuidPair> BuildSelectedProviderListForCompIdentity(Controls_ConfirmIdentCompromise.IdentityActionEnum action)
        {
            var providerList = new List<UserNameWithUserGuidPair>();
            foreach (GridDataItem selectedRow in gvProviders.SelectedItems)
            {
                var isIdComrpromised = Convert.ToBoolean(((Label)selectedRow["colIdCompromised"].Controls[1]).Text);

                //Check if identity is already set to the value we are trying to set.  If it is already set correctly ignore provider.
                if (action == Controls_ConfirmIdentCompromise.IdentityActionEnum.Compromise && isIdComrpromised)
                {
                    continue;
                }

                if (action == Controls_ConfirmIdentCompromise.IdentityActionEnum.Secure && !isIdComrpromised)
                {
                    continue;
                }

                var provider = new UserNameWithUserGuidPair
                {
                    UserName = ((HiddenField)selectedRow["colShieldUserName"].Controls[1]).Value,
                    UserGuid = ((HiddenField)selectedRow["colProviderGuid"].Controls[1]).Value
                };
                providerList.Add(provider);
            }

            return providerList;
        }

        private void SetTraitsForMultipleProvidersAndResetGrid(ShieldTraitName traitName, ShieldTraitValue traitValue, List<UserNameWithUserGuidPair> usersAccepted, string otpToken, string identitySecurityToken, string licenseID, string userID)
        {
            var isInstitutional =  AppCode.StateUtils.UserInfo.GetIdProofingMode(PageState) == ShieldTenantIDProofingModel.Institutional;
            bool actionSuccess = Users.SetTraitForMultipleUsers(usersAccepted, traitName, traitValue, EprescribeExternalAppInstanceID,
                ShieldSecurityToken, otpToken, identitySecurityToken, licenseID, userID, isInstitutional);
            ResetGrid();
            if (!actionSuccess)
            {
                ucMessage.Visible = true;
                ucMessage.MessageText = errorMessage;
                ucMessage.Icon = Controls_Message.MessageType.ERROR;
            }
        }

        protected void ResetView(object sender, EventArgs e)
        {
            panelDeaSearch.Visible = false;
            if (showMessage == true)
            {
                ucMessage.Visible = true;
                showMessage = false;

            }
            else
                ucMessage.Visible = false;
            
            if (ddlProviderView.SelectedValue == GrantEpcsViewValue)
            {
                foreach (GridDataItem gridRow in gvProviders.Items)
                {
                    var userMaySet = Convert.ToBoolean(((HiddenField) gridRow["colUserMaySetCanEnroll"].Controls[1]).Value);
                    var canEnrollPrivilegeGranted = Convert.ToBoolean(((Label) gridRow["colCanEnrollPrivilege"].Controls[1]).Text);
                    if (!userMaySet || canEnrollPrivilegeGranted)
                    {
                        gridRow.Visible = false;
                    }
                    else
                    {
                        gridRow.Visible = true;
                        EnableDisableRowSelection(gridRow, true);
                    }
                }

                ShowHideButtonsByView(View.CanEnroll);
            }
            else if (ddlProviderView.SelectedValue == ShowAllProviderViewValue)
            {
                gvProviders.Rebind();
                foreach (GridDataItem gridRow in gvProviders.Items)
                {
                    gridRow.Visible = true;
                    EnableDisableRowSelection(gridRow, false);
                }
                ShowHideButtonsByView(View.ShowAllProviders);
            }
            else if (ddlProviderView.SelectedValue == CanPrescribeViewValue)
            {
                foreach (GridDataItem gridRow in gvProviders.Items)
                {
                    var userMaySet =
                        Convert.ToBoolean(((HiddenField) gridRow["colUserMaySetCanPrescribe"].Controls[1]).Value);
                    var canPrescribePrivilegeGranted =
                        Convert.ToBoolean(((Label) gridRow["colEPCSSigningGranted"].Controls[1]).Text);
                    if (!userMaySet || canPrescribePrivilegeGranted)
                    {
                        gridRow.Visible = false;
                    }
                    else
                    {
                        gridRow.Visible = true;
                        EnableDisableRowSelection(gridRow, true);
                    }
                }

                ShowHideButtonsByView(View.CanPrescribe);
            }
            else if (ddlProviderView.SelectedValue == SearchByDeaViewValue)
            {
                foreach (GridDataItem gridRow in gvProviders.Items)
                {
                    gridRow.Visible = true;
                    EnableDisableRowSelection(gridRow, true);
                }
                panelDeaSearch.Visible = true;
                txtDeaSearch.Text = string.Empty;

                ShowHideButtonsByView(View.SearchByDea);
            }
        }

        private static void EnableDisableRowSelection(GridDataItem gridRow, bool enabled)
        {
            var checkbox = (CheckBox) gridRow["colCheckboxes"].Controls[0];
            checkbox.Enabled = enabled;
            gridRow.SelectableMode = enabled ? GridItemSelectableMode.ServerAndClientSide : GridItemSelectableMode.None;
        }

        protected void btnSearch_OnClick(object sender, EventArgs e)
        {
            if (txtDeaSearch != null)
            {
                string searchString = null;
                if (txtDeaSearch.Text.Trim() != string.Empty)
                {
                    searchString = txtDeaSearch.Text.Trim();
                }
                gvProviders.DataSource = EPSBroker.GetAllProvidersAndTraitsForLicense(SessionLicenseID, searchString,
                    EprescribeExternalAppInstanceID, ShieldSecurityToken);
                gvProviders.Rebind();


            }
        }

        protected void btnGrantEpcsPrivilege_Click(object sender, EventArgs e)
        {
            List<UserNameWithUserGuidPair> providersToGrantCanEnroll = BuildSelectedProviderList();
            ShieldTraitInfo canEnroll = new ShieldTraitInfo { TraitName = ShieldTraitName.CanEnroll, TraitValueEnum = ShieldTraitValue.YES };
            var isInstitutional =  AppCode.StateUtils.UserInfo.GetIdProofingMode(PageState) == ShieldTenantIDProofingModel.Institutional;
            bool actionSuccess = EPSBroker.SetTraitForMultipleProviders(providersToGrantCanEnroll, canEnroll, EprescribeExternalAppInstanceID, null, null, ShieldSecurityToken, null, null, isInstitutional);
            if (!actionSuccess)
            {
                SetErrorMessage();
            }
            ResetGrid();
        }

        protected void btnApproveEpcsSigning_Click(object sender, EventArgs e)
        {
            ucDueDiligence.ListOfUsersToApprove = BuildSelectedProviderList();
            ucDueDiligence.Show();
        }

        protected void btnSuspendEpcs_Click(object sender, EventArgs e)
        {
            ucConfirmIdentCompromise.Show(Controls_ConfirmIdentCompromise.IdentityActionEnum.Compromise);
        }

        protected void btnReinstateEpcs_Click(object sender, EventArgs e)
        {
            ucConfirmIdentCompromise.Show(Controls_ConfirmIdentCompromise.IdentityActionEnum.Secure);
        }
    }
}