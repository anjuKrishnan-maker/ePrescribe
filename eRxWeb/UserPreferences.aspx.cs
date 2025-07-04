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
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;

namespace eRxWeb
{
public partial class UserPreferences : BasePage 
{
    protected void Page_Load(object sender, EventArgs e)
    {
      
        if (!Page.IsPostBack)
        {
            InitializePreferences();
            //Added by JJ on Aug14th 
            //DisableDosage();
            chkPerformDosage.Attributes.Add("OnCheckedChanged", "enabledisableDosage()");
            chkRequireFormularyReason.Style["display"] = base.SessionLicense.EnterpriseClient.ForceFormularyWarning ? "inline" : "none";

            base.AuditLogLicenseInsert(ePrescribeSvc.AuditAction.LICENSE_DUR_VIEW, 0);
        }
    }
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        ((PhysicianMasterPageBlank)Master).hideTabs();
    }
    protected void DisableDosage()
    {
        
        //Added by JJ on Aug 14th
        //chkScreenMaxConsecutive.Checked = false;
        chkScreenMaxIndividualDose.Checked = false;
        chkScreenMinDuration.Checked = false;
        chkScreenMaxDose.Checked = false;
        chkScreenMinDose.Checked = false;
        chkScreenMaxDuration.Checked = false;

        //chkScreenMaxConsecutive.Enabled = false;
        chkScreenMaxIndividualDose.Enabled = false;
        chkScreenMinDuration.Enabled = false;
        chkScreenMaxDose.Enabled = false;
        chkScreenMinDose.Enabled = false;
        chkScreenMaxDuration.Enabled = false;
      
    }
    protected void EnableDosage()
    {
        //Added by JJ on Aug 14th
        //chkScreenMaxConsecutive.Enabled = true;
        chkScreenMaxIndividualDose.Enabled = true;
        chkScreenMinDuration.Enabled = true;
        chkScreenMaxDose.Enabled = true;
        chkScreenMinDose.Enabled = true;
        chkScreenMaxDuration.Enabled = true;

    }
    private void InitializePreferences()
    {
        if (Session["LICENSEID"] != null)
        {
            Preference pref;
            string sprefValue;
            string chkd;

            string licId = Session["LICENSEID"].ToString();
            Hashtable _providerPreferences = Preference.LoadUserPreferences(PreferenceCategory.RX_OPTIONS, base.SessionUserID, licId, base.DBID);

            //DUR - General
            //pref = (Allscripts.Impact.Preference)_providerPreferences["DUR_OVERRIDE_GEN"];
            //chkd = (string)pref.LicenseValue;
            //chkOverrideGeneral.Checked = (chkd == "Y" ? true : false);

            //pref = (Preference)_providerPreferences["PERFORM_FORMULARY"];
            //chkPerformFormulary.Checked = (pref.Value == "Y") ? true : false;
            //chkPerformFormulary.Attributes.Add("onclick", "enabledisable(this)");
            // set the CHECK_ALTERNATIVES preference differently
            // since it is a radio button and dependent on the
            // PERFORM_FORMULARY preference
            //pref = (Allscripts.Impact.Preference)_providerPreferences["CHECK_ALTERNATIVES"];

            //if ((pref.LicenseValue != "NULL") && (pref.LicenseValue.Length > 0))
            //{
            //    int prefValue = Convert.ToInt32(pref.LicenseValue);

            //    switch (prefValue)
            //    {
            //        case 0:
            //            radioNoAlternatives.Checked = true;
            //            break;
            //        case 1:
            //            radioYesAlternatives.Checked = true;
            //            break;
            //        case 2:
            //        //default:
            //        //    radioRedFaceAlternatives.Checked = true;
            //        //    break;
            //    }
            //}
            //else
            //{
            //    radioRedFaceAlternatives.Checked = true;
            //}
            //radioNoAlternatives.Enabled = chkPerformFormulary.Checked && chkPerformFormulary.Enabled;
            //radioYesAlternatives.Enabled = chkPerformFormulary.Checked && chkPerformFormulary.Enabled;
            //radioRedFaceAlternatives.Enabled = chkPerformFormulary.Checked && chkPerformFormulary.Enabled;

            //EAK WHY WERE THESE COMMENTED OUT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            pref = (Preference)_providerPreferences["PERFORM_PAR"];
            //chkPerformPAR.Checked = (pref.Value == "Y") ? true : false;
            chkPerformPAR.Checked = (pref.LicenseValue == "Y") ? true : false;

            pref = (Preference)_providerPreferences["PERFORM_INT"];
            //chkPerformInteraction.Checked = (pref.Value == "Y") ? true : false;
            chkPerformInteraction.Checked = (pref.LicenseValue == "Y") ? true : false;

            pref = (Preference)_providerPreferences["PERFORM_DOSE"];
            //chkPerformDosage.Checked = (pref.Value == "Y") ? true : false;
            chkPerformDosage.Checked = (pref.LicenseValue == "Y") ? true : false;

            pref = (Preference)_providerPreferences["PERFORM_DT"];
            //chkPerformDT.Checked = (pref.Value == "Y") ? true : false;
            chkPerformDT.Checked = (pref.LicenseValue == "Y") ? true : false;

            pref = (Preference)_providerPreferences["DUR_REQUIRE_REASON"];
            chkRequireReason.Checked = (pref.LicenseValue == "Y") ? true : false;

            pref = (Preference)_providerPreferences["FORM_REQUIRE_REASON"];
            chkRequireFormularyReason.Checked = (pref.LicenseValue == "Y") ? true : false;
            //EAK END WHY!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
           
            //DUR Duplicate Therapy
            //pref = (Allscripts.Impact.Preference)_providerPreferences["DUR_OVERRIDE_DT"];
            //chkd = (string)pref.LicenseValue;
            //chkOverrideDT.Checked = (chkd == "Y" ? true : false);

            pref = (Allscripts.Impact.Preference)_providerPreferences["DUR_DT_RANGE"];
            //sprefValue = (string)pref.Value;
            sprefValue = (string)pref.LicenseValue;

            if (sprefValue != "NULL" && ddlDTScreen.Items.FindByValue(sprefValue) != null)
                ddlDTScreen.Items.FindByValue(sprefValue).Selected = true;

            pref = (Allscripts.Impact.Preference)_providerPreferences["DUR_DT_WARNING"];
            //sprefValue = (string)pref.Value;
            sprefValue = (string)pref.LicenseValue;

            if (sprefValue != "NULL" && ddlDTWarning.Items.FindByValue(sprefValue)!=null)
                ddlDTWarning.Items.FindByValue(sprefValue).Selected = true;

            //DUR Drug Interaction

            //pref = (Allscripts.Impact.Preference)_providerPreferences["DUR_OVERRIDE_INT"];
            //chkd = (string)pref.LicenseValue;
            //chkOverrideInteractions.Checked = (chkd == "Y" ? true : false);

            pref = (Allscripts.Impact.Preference)_providerPreferences["DUR_INT_ONSET"];
            //sprefValue = (string)pref.Value;
            sprefValue = (string)pref.LicenseValue;

            if (sprefValue != "NULL" && ddlMinOnSet.Items.FindByValue(sprefValue) !=null)
                ddlMinOnSet.Items.FindByValue(sprefValue).Selected = true;

            pref = (Allscripts.Impact.Preference)_providerPreferences["DUR_INT_SEVERITY"];
            //sprefValue = (string)pref.Value;
            sprefValue = (string)pref.LicenseValue;
            if (sprefValue != "NULL" && ddlMinSeverity.Items.FindByValue(sprefValue) != null)
                ddlMinSeverity.Items.FindByValue(sprefValue).Selected = true;

            pref = (Allscripts.Impact.Preference)_providerPreferences["DUR_INT_DOC"];
            //sprefValue = (string)pref.Value;
            sprefValue = (string)pref.LicenseValue;
            if (sprefValue != "NULL" && ddlMinDocumentation.Items.FindByValue(sprefValue)!=null)
                ddlMinDocumentation.Items.FindByValue(sprefValue).Selected = true;


            pref = (Allscripts.Impact.Preference)_providerPreferences["DUR_INT_FOOD"];
            //chkd = (string)pref.Value;
            chkd = (string)pref.LicenseValue;
            chkScreenFood.Checked = (chkd == "Y" ? true : false);

            pref = (Allscripts.Impact.Preference)_providerPreferences["DUR_INT_ALCOHOL"];
            //chkd = (string)pref.Value;
            chkd = (string)pref.LicenseValue;
            chkScreenAlcohol.Checked = (chkd == "Y" ? true : false);
            

            //DUR Dosage

            //pref = (Allscripts.Impact.Preference)_providerPreferences["DUR_OVERRIDE_DOSE"];
            //chkd = (string)pref.LicenseValue;
            //chkPerformDosage.Checked = (chkd == "Y" ? true : false);

            //pref = (Allscripts.Impact.Preference)_providerPreferences["DUR_DOSE_MAX_CONSEC"];
            //chkd = (string)pref.Value;
            //chkd = (string)pref.LicenseValue;
            //chkScreenMaxConsecutive.Checked = (chkd == "Y" ? true : false);

            pref = (Allscripts.Impact.Preference)_providerPreferences["DUR_DOSE_MAX_DURATION"];
            //chkd = (string)pref.Value;
            chkd = (string)pref.LicenseValue;
            chkScreenMaxDuration.Checked = (chkd == "Y" ? true : false);

            pref = (Allscripts.Impact.Preference)_providerPreferences["DUR_DOSE_MAX_DOSE"];
            //chkd = (string)pref.Value;
            chkd = (string)pref.LicenseValue;
            chkScreenMaxDose.Checked = (chkd == "Y" ? true : false);

            pref = (Allscripts.Impact.Preference)_providerPreferences["DUR_DOSE_MAX_INDIV_DOSE"];
            //chkd = (string)pref.Value;
            chkd = (string)pref.LicenseValue;
            chkScreenMaxIndividualDose.Checked = (chkd == "Y" ? true : false);

            pref = (Allscripts.Impact.Preference)_providerPreferences["DUR_DOSE_MIN_DOSE"];
            //chkd = (string)pref.Value;
            chkd = (string)pref.LicenseValue;
            chkScreenMinDose.Checked = (chkd == "Y" ? true : false);

            pref = (Allscripts.Impact.Preference)_providerPreferences["DUR_DOSE_MIN_DURATION"];
            //chkd = (string)pref.Value;
            chkd = (string)pref.LicenseValue;
            chkScreenMinDuration.Checked = (chkd == "Y" ? true : false);
        }
    }
   
    
    protected void chkScreenMinDose_CheckedChanged(object sender, EventArgs e)
    {

    }
    //protected void chkPerformDosage_CheckedChanged(object sender, EventArgs e)
    //{
    //    //Added by JJ on Aug 14th
    //    if (chkPerformDosage.Checked == true)
    //        EnableDosage();
    //    else
            
    //        DisableDosage();
 
    //}
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Server.Transfer(AngularStringUtil.CreateUrl(Constants.PageNames.SETTINGS));
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        string siteID = "";
        string sValue;
        Preference pref;
        Hashtable _rxPreferences = new Hashtable();
        string userid = Session["USERID"].ToString();
        bool DURSaveSuccessfull = true;

        //sValue = (chkPerformFormulary.Checked ? "Y" : "N");
        //pref = Preference.CreatePreference(siteID, "PERFORM_FORMULARY", PreferenceCategory.RX_OPTIONS, true, sValue);
        //_rxPreferences.Add("PERFORM_FORMULARY", pref);


        //sValue = (chkPerformPAR.Checked ? "Y" : "N");
        //if (radioNoAlternatives.Checked)
        //    sValue = "0";
        //else if (radioYesAlternatives.Checked)
        //    sValue = "1";
        //else
        //    sValue = "2";

        //pref = Preference.CreatePreference(siteID, "CHECK_ALTERNATIVES", PreferenceCategory.RX_OPTIONS, true, sValue);
        //_rxPreferences.Add("CHECK_ALTERNATIVES", pref);

        sValue = (chkPerformPAR.Checked ? "Y" : "N");
        //pref = Preference.CreatePreference(siteID, "PERFORM_PAR", PreferenceCategory.RX_OPTIONS, true, sValue);
        pref = Preference.CreatePreference(siteID, "PERFORM_PAR", PreferenceCategory.RX_OPTIONS, false, sValue);
      
        _rxPreferences.Add("PERFORM_PAR", pref);

        sValue = (chkPerformInteraction.Checked ? "Y" : "N");
        //pref = Preference.CreatePreference(siteID, "PERFORM_INT", PreferenceCategory.RX_OPTIONS, true, sValue);
        pref = Preference.CreatePreference(siteID, "PERFORM_INT", PreferenceCategory.RX_OPTIONS, false, sValue);
      
        _rxPreferences.Add("PERFORM_INT", pref);

        sValue = (chkPerformDosage.Checked ? "Y" : "N");
        //pref = Preference.CreatePreference(siteID, "PERFORM_DOSE", PreferenceCategory.RX_OPTIONS, true, sValue);
        pref = Preference.CreatePreference(siteID, "PERFORM_DOSE", PreferenceCategory.RX_OPTIONS, false, sValue);
        _rxPreferences.Add("PERFORM_DOSE", pref);

        sValue = (chkPerformDT.Checked ? "Y" : "N");
        //pref = Preference.CreatePreference(siteID, "PERFORM_DT", PreferenceCategory.RX_OPTIONS, true, sValue);
        pref = Preference.CreatePreference(siteID, "PERFORM_DT", PreferenceCategory.RX_OPTIONS, false, sValue);
       
        _rxPreferences.Add("PERFORM_DT", pref);

        sValue = (chkRequireReason.Checked ? "Y" : "N");
        pref = Preference.CreatePreference(siteID, "DUR_REQUIRE_REASON", PreferenceCategory.RX_OPTIONS, false, sValue);
        _rxPreferences.Add("DUR_REQUIRE_REASON", pref);

        sValue = (chkRequireFormularyReason.Checked ? "Y" : "N");
        pref = Preference.CreatePreference(siteID, "FORM_REQUIRE_REASON", PreferenceCategory.RX_OPTIONS, false, sValue);
        _rxPreferences.Add("FORM_REQUIRE_REASON", pref);

        //DUR Duplicate Therapy

        sValue = ddlDTScreen.SelectedValue;
        //pref = Preference.CreatePreference(siteID, "DUR_DT_RANGE", PreferenceCategory.RX_OPTIONS, true, sValue);
        pref = Preference.CreatePreference(siteID, "DUR_DT_RANGE", PreferenceCategory.RX_OPTIONS, false, sValue);
       
        _rxPreferences.Add("DUR_DT_RANGE", pref);

        sValue = ddlDTWarning.SelectedValue;
        //pref = Preference.CreatePreference(siteID, "DUR_DT_WARNING", PreferenceCategory.RX_OPTIONS, true, sValue);
        pref = Preference.CreatePreference(siteID, "DUR_DT_WARNING", PreferenceCategory.RX_OPTIONS, false, sValue);
       
        _rxPreferences.Add("DUR_DT_WARNING", pref);

        //DUR Interaction

        sValue = ddlMinOnSet.SelectedValue;
        //pref = Preference.CreatePreference(siteID, "DUR_INT_ONSET", PreferenceCategory.RX_OPTIONS, true, sValue);
        pref = Preference.CreatePreference(siteID, "DUR_INT_ONSET", PreferenceCategory.RX_OPTIONS, false, sValue);
        
        _rxPreferences.Add("DUR_INT_ONSET", pref);

        sValue = ddlMinSeverity.SelectedValue;
        //pref = Preference.CreatePreference(siteID, "DUR_INT_SEVERITY", PreferenceCategory.RX_OPTIONS, true, sValue);
        pref = Preference.CreatePreference(siteID, "DUR_INT_SEVERITY", PreferenceCategory.RX_OPTIONS, false, sValue);
        
        _rxPreferences.Add("DUR_INT_SEVERITY", pref);

        sValue = ddlMinDocumentation.SelectedValue;
        //pref = Preference.CreatePreference(siteID, "DUR_INT_DOC", PreferenceCategory.RX_OPTIONS, true, sValue);
        pref = Preference.CreatePreference(siteID, "DUR_INT_DOC", PreferenceCategory.RX_OPTIONS, false, sValue);
       
        _rxPreferences.Add("DUR_INT_DOC", pref);

        sValue = (chkScreenFood.Checked ? "Y" : "N");
        //pref = Preference.CreatePreference(siteID, "DUR_INT_FOOD", PreferenceCategory.RX_OPTIONS, true, sValue);
        pref = Preference.CreatePreference(siteID, "DUR_INT_FOOD", PreferenceCategory.RX_OPTIONS, false, sValue);
       
        _rxPreferences.Add("DUR_INT_FOOD", pref);

        sValue = (chkScreenAlcohol.Checked ? "Y" : "N");
        pref = Preference.CreatePreference(siteID, "DUR_INT_ALCOHOL", PreferenceCategory.RX_OPTIONS, false, sValue);
        _rxPreferences.Add("DUR_INT_ALCOHOL", pref);

        //DUR Dosage

        //sValue = (chkScreenMaxConsecutive.Checked ? "Y" : "N");
        //pref = Preference.CreatePreference(siteID, "DUR_DOSE_MAX_CONSEC", PreferenceCategory.RX_OPTIONS, true, sValue);;
        //pref = Preference.CreatePreference(siteID, "DUR_DOSE_MAX_CONSEC", PreferenceCategory.RX_OPTIONS, false, sValue);
       
        //_rxPreferences.Add("DUR_DOSE_MAX_CONSEC", pref);

        sValue = (chkScreenMaxDuration.Checked ? "Y" : "N");
        //pref = Preference.CreatePreference(siteID, "DUR_DOSE_MAX_DURATION", PreferenceCategory.RX_OPTIONS, true, sValue);
        pref = Preference.CreatePreference(siteID, "DUR_DOSE_MAX_DURATION", PreferenceCategory.RX_OPTIONS, false, sValue);
       
        _rxPreferences.Add("DUR_DOSE_MAX_DURATION", pref);

        sValue = (chkScreenMaxDose.Checked ? "Y" : "N");
        //pref = Preference.CreatePreference(siteID, "DUR_DOSE_MAX_DOSE", PreferenceCategory.RX_OPTIONS, true, sValue);
        pref = Preference.CreatePreference(siteID, "DUR_DOSE_MAX_DOSE", PreferenceCategory.RX_OPTIONS, false, sValue);
       
        _rxPreferences.Add("DUR_DOSE_MAX_DOSE", pref);

        sValue = (chkScreenMaxIndividualDose.Checked ? "Y" : "N");
        //pref = Preference.CreatePreference(siteID, "DUR_DOSE_MAX_INDIV_DOSE", PreferenceCategory.RX_OPTIONS, true, sValue);
        pref = Preference.CreatePreference(siteID, "DUR_DOSE_MAX_INDIV_DOSE", PreferenceCategory.RX_OPTIONS, false, sValue);
        
        _rxPreferences.Add("DUR_DOSE_MAX_INDIV_DOSE", pref);

        sValue = (chkScreenMinDose.Checked ? "Y" : "N");
        //pref = Preference.CreatePreference(siteID, "DUR_DOSE_MIN_DOSE", PreferenceCategory.RX_OPTIONS, true, sValue);
        pref = Preference.CreatePreference(siteID, "DUR_DOSE_MIN_DOSE", PreferenceCategory.RX_OPTIONS, false, sValue);
        
        _rxPreferences.Add("DUR_DOSE_MIN_DOSE", pref);

        sValue = (chkScreenMinDuration.Checked ? "Y" : "N");
        //pref = Preference.CreatePreference(siteID, "DUR_DOSE_MIN_DURATION", PreferenceCategory.RX_OPTIONS, true, sValue);
        pref = Preference.CreatePreference(siteID, "DUR_DOSE_MIN_DURATION", PreferenceCategory.RX_OPTIONS, false, sValue);
        
        _rxPreferences.Add("DUR_DOSE_MIN_DURATION", pref);

        Preference[] preferences = new Preference[_rxPreferences.Count];
        _rxPreferences.Values.CopyTo(preferences, 0);
        try
        {
            //Preference.SaveUserPreferences(userid, PreferenceCategory.RX_OPTIONS, preferences);
            if(Session["LICENSEID"] != null)
                Preference.SaveLicensePreferences(PreferenceCategory.RX_OPTIONS, preferences, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);
            else
                Server.Transfer(Constants.PageNames.LOGOUT + "?Timeout=YES");

            base.AuditLogLicenseInsert(ePrescribeSvc.AuditAction.LICENSE_DUR_MODIFY, 0);
        }
        catch (Exception prefex)
        {
            DURSaveSuccessfull = false;
            Console.WriteLine(prefex.ToString());
        }

        var message = DURSaveSuccessfull
            ? new MessageModel("DUR settings saved successfully.")
            : new MessageModel("An error occurred while saving DUR settings.", MessageIcon.Error);

            Server.Transfer(AngularStringUtil.CreateUrl(Constants.PageNames.SETTINGS, message));
        }
}

}