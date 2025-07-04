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
using System.Collections.Generic;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
public partial class UpdateAllergy : BasePage 
{
    string allergyType = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        
        string patAllergyID = Request.QueryString["PatientAllergyID"];
        if (!Page.IsPostBack)
        {
            int tasks = 0;
         
            
            ((PhysicianMasterPage)Master).toggleTabs("patient", tasks);
            
         
            // Added by AKS as on AUG 28 to get the allergy and assign them after the edit has happend
            if (Session["PATIENTID"] != null)
            {
                Session["ALLERGY"] = GetAllAllergies(Session["PATIENTID"].ToString());
            }
            //End on AUG 28 , 2006
            string med;
            DataSet ds = Allscripts.Impact.PatientAllergy.Load(patAllergyID, base.DBID);
            med = ds.Tables["PatientAllergy"].Rows[0]["AllergyName"].ToString();
            Session["AllergyID"] = ds.Tables["PatientAllergy"].Rows[0]["AllergyID"].ToString();
            Session["Start"] = ds.Tables["PatientAllergy"].Rows[0]["Start"].ToString();
            Session["End"] = ds.Tables["PatientAllergy"].Rows[0]["End"].ToString();

            rbtActive.Checked = (ds.Tables["PatientAllergy"].Rows[0]["Active"].ToString() == "Y" ? true : false);
            rbtInActive.Checked = (ds.Tables["PatientAllergy"].Rows[0]["Active"].ToString() == "N" ? true : false);
            divMedication.InnerText = med;

            //KNC 02/24/2009
            rdAllergy.Checked = (ds.Tables["PatientAllergy"].Rows[0]["AllergyCategory"].ToString() == "A" ? true : false);
            rdIntolerance.Checked = (ds.Tables["PatientAllergy"].Rows[0]["AllergyCategory"].ToString() == "I" ? true : false);
            allergyType = ds.Tables["PatientAllergy"].Rows[0]["AllergyType"].ToString();


            GetReactions();

            DataTable dtAllergyReaction = ds.Tables["PatientAllergyReaction"];
            foreach (DataRow dr in dtAllergyReaction.Rows)
            {
                ListItem lstItem = lstReactions.Items.FindByValue(dr["AllergenReactionID"].ToString());
                if (lstItem != null)
                    lstItem.Selected = true;
            }
            DataRow[] drOthers = dtAllergyReaction.Select("AllergenReactionID='00000000-0000-0000-0000-000000000000'");
            if (drOthers.Length > 0)
            {
                //chkOther.Checked = true;
                txtOther.Text = drOthers[0]["Note"].ToString();
            }
        }
      

    }
    // Added by AKS as om Aug 25 for getting ALL the allergies for a specific patient
    protected string GetAllAllergies(string patId)
    {

        Patient pat = new Patient(patId, true, false, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);
        DataSet patds = pat.DS;
        string allergy = "";

        if (patds.Tables["PatientAllergy"].Rows.Count > 0)
        {
            DataRow[] PatAllergy = patds.Tables["PatientAllergy"].Select("Active='Y'");

            foreach (DataRow dr in PatAllergy)
            {
                allergy = allergy + dr["AllergyName"].ToString() + ",";
            }
            if (allergy.EndsWith(","))
                allergy = allergy.Substring(0, allergy.Length - 1);
            return allergy;
        }
        else
            return allergy;

    }


    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        LinkButton lnk = Master.FindControl("lnkPatient") as LinkButton;
        if (lnk != null)
            lnk.Enabled = false;
        lnk = Master.FindControl("lnkMedication") as LinkButton;
        if (lnk != null)
            lnk.Enabled = false;
        lnk = Master.FindControl("lnkDiagnosis") as LinkButton;
        if (lnk != null)
            lnk.Enabled = false;
    }
    // Function to Load all the Reactions that are Active from the DB
    private void GetReactions()
    {
        DataSet drReactions;
        drReactions = new Allergy().GetAllergyReactions(base.DBID);
        
        if (drReactions.Tables[0].Rows.Count > 0)
        {
            //Get and Bind the dataset to the control..
            lstReactions.DataSource = drReactions;
            lstReactions.DataTextField = "Name";
            lstReactions.DataValueField = "AllergenReactionID";
            lstReactions.DataBind();

        }
       

    }

  
    void SaveAllergy()
    {
        string patientAllergyID = Request.QueryString["PatientAllergyID"];
        string patientId = Session["PATIENTID"].ToString(); 
        
        // string allergyType = "M";

        string allergyID = Session["AllergyID"].ToString();
        bool active = rbtActive.Checked;
        string start = Session["Start"].ToString();
        string end = Session["End"].ToString();
        List<string> reactions = new List<string>();   //List of ReactionID 
        bool other = (txtOther.Text.Trim().Length>0?true:false);// chkOther.Checked;
        string otherNote = txtOther.Text.Trim();
        string userid = Session["USERID"].ToString();

        string category;

        if (rdAllergy.Checked == true)
        {
            category = "A";
        }
        else
        {
            category = "I";
        }


        //Get all the Selected Reactions ...
        if (lstReactions.SelectedIndex > -1)
        {

            foreach (ListItem li in lstReactions.Items)
            {

                if (li.Selected == true)
                {
                    reactions.Add(li.Value.ToString());

                }

            }

        }
        
        // Added Allergy Category
        Allscripts.Impact.PatientAllergy.Save(patientAllergyID, patientId, allergyType, allergyID, active, start, end, reactions.ToArray(), other, otherNote, Session["USERID"].ToString(), Session["LICENSEID"].ToString(), category, base.DBID);      
        base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_ALLERGY_MODIFY, patientId);
    }


    //protected void chkOther_CheckedChanged(object sender, EventArgs e)
    //{

    //}
    protected void btSaveAllergy_Click(object sender, EventArgs e)
    {
        SaveAllergy();
        Session["ALLERGY"] = GetAllAllergies(Session["PATIENTID"].ToString());
        PageState[Constants.SessionVariables.DurPatientAllergies] = DURMedispanUtils.RetrieveAllergy(Session["PATIENTID"].ToString(), Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);

        Server.Transfer(Constants.PageNames.PATIENT_ALLERGY);
    }
 }

}