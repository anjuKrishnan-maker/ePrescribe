using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using System.Data;

namespace eRxWeb
{
public partial class Controls_ObsoletePARClassMappingChange : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    
    public string PatientObsoletedAllergyCheckCalledFrom
    {
        set
        {
            Session["PatientAllergyCalledFrom"] = value;
        }
    }
    public bool ShowIfPatientHasActiveObsoletedParClass()
    {
        if (rlbObsoletedPatientParClass.Items.Count > 0)
        {
            mpePARClasses.Show();
            return true;
        }
        return false;
    }

    public void LoadPatientActiveObsoletePARClass()
    {
        Session.Remove("PatientAllergyCalledFrom");
        if ((Session["PATIENTID"] != null) &&
            Session["PATIENTID"].ToString() != Guid.Empty.ToString())
        {
            DataView allergiesDV = (DataView)odsObsoletedPatientParClass.Select();
            allergiesDV.RowFilter = "Active = 'Y' and AllergyType = 'C' and ClassActiveStatus = 'N'";

            rlbObsoletedPatientParClass.DataSource = allergiesDV;
            rlbObsoletedPatientParClass.DataTextField = "AllergyName";
            rlbObsoletedPatientParClass.DataValueField = "AllergyName";

            rlbObsoletedPatientParClass.DataBind();
        }
    }

    protected void btnPARContinue_Click(object sender, EventArgs e)
    {
        if (Session["PatientAllergyCalledFrom"] == null)
        {
            Session["PatientAllergyCalledFrom"] = getCurrentPageWithParameters();
        }

        Response.Redirect(Constants.PageNames.PATIENT_ALLERGY);
    }

    public string getCurrentPageWithParameters()
    {
        string strPath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
        System.IO.FileInfo fInfo = new System.IO.FileInfo(strPath);
        string strPageName = fInfo.Name;
        string strRet = strPageName + Request.Url.Query;
        return strRet.ToLower();
    }
}
}