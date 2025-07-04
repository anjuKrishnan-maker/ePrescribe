/******************************************************************************
**Change History
*******************************************************************************
**Date:      Author:                    Description:
**-----------------------------------------------------------------------------
* 02/19/2010   Anand Kumar Krishnan     Patient NKA is returned in CCR in GetPatientSnapShot
*******************************************************************************/
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
using System.Text;

namespace eRxWeb
{
public partial class PatientSnapShot : BasePage 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            string _patientGUID = Request.QueryString["patientGUID"];

            GetPatientSnapShot(_patientGUID);
        }
        catch (Exception Exception)
        {
            Trace.Warn(Exception.ToString());

            throw (Exception);
        }
    }
    private string ChkNull(string arg)
    {
        if (arg == null)
            return "";
        else
            return arg;
    }
    private void AddNode(System.Xml.XmlDocument doc, System.Xml.XmlElement parent, string name, string value)
    {
        System.Xml.XmlElement node = doc.CreateElement(name);
        node.InnerText = value;
        parent.AppendChild(node);
    }

    public void GetPatientSnapShot(string patientID)
    {
        string _lastpharmid = null;
        string _userid = (string) Session["USERID"];
        string _lic = (string) Session["LICENSEID"];       

        System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
        System.Xml.XmlElement xmlroot = xmldoc.CreateElement("Patient");
        xmldoc.AppendChild(xmlroot);  

        Patient pat = new Patient(patientID, _lic, _userid, base.DBID); //, false, true);
        
        DataSet patds = pat.DS;
      
        if (patds.Tables["Patient"].Rows.Count > 0)
        {
            System.Xml.XmlElement patccrxmlelm = xmldoc.CreateElement("Demographics");
            xmlroot.AppendChild(patccrxmlelm);
            //AddNode(xmldoc, patccrxmlelm, "APPGROUP", "eRxNow");
            AddNode(xmldoc, patccrxmlelm, "NAME", "Patient");
           // AddNode(xmldoc, patccrxmlelm, "PLURAL", "false");
           // AddNode(xmldoc, patccrxmlelm, "ACTION", "Load");
            AddNode(xmldoc, patccrxmlelm, "ID", patds.Tables["Patient"].Rows[0]["PatientID"].ToString());
            string _patname =
                StringUtil.formatDisplayName(
                            patds.Tables["Patient"].Rows[0]["LastName"].ToString(),
                            patds.Tables["Patient"].Rows[0]["FirstName"].ToString(),
                            patds.Tables["Patient"].Rows[0]["MiddleName"].ToString());

            AddNode(xmldoc, patccrxmlelm, "FULLNAME", _patname);
            AddNode(xmldoc, patccrxmlelm, "LASTNAME", patds.Tables["Patient"].Rows[0]["LastName"].ToString());
            AddNode(xmldoc, patccrxmlelm, "FIRSTNAME", patds.Tables["Patient"].Rows[0]["FirstName"].ToString());
            AddNode(xmldoc, patccrxmlelm, "MIDDLENAME", patds.Tables["Patient"].Rows[0]["MiddleName"].ToString());


            AddNode(xmldoc, patccrxmlelm, "SEX", patds.Tables["Patient"].Rows[0]["SEX"].ToString());
            AddNode(xmldoc, patccrxmlelm, "DATEOFBIRTH", patds.Tables["Patient"].Rows[0]["DOB"].ToString());
            AddNode(xmldoc, patccrxmlelm, "ADDRESSLINE1", patds.Tables["Patient"].Rows[0]["Address1"].ToString());
            AddNode(xmldoc, patccrxmlelm, "ADDRESSLINE2", patds.Tables["Patient"].Rows[0]["Address2"].ToString());
            AddNode(xmldoc, patccrxmlelm, "CITY", patds.Tables["Patient"].Rows[0]["City"].ToString());
            AddNode(xmldoc, patccrxmlelm, "STATE", patds.Tables["Patient"].Rows[0]["State"].ToString());
            AddNode(xmldoc, patccrxmlelm, "ZIPCODE", patds.Tables["Patient"].Rows[0]["Zip"].ToString());
            AddNode(xmldoc, patccrxmlelm, "HOMEPHONE", patds.Tables["Patient"].Rows[0]["Phone"].ToString());
            AddNode(xmldoc, patccrxmlelm, "PROVIDERNAME", (string)Session["UserName"]);

            _lastpharmid = patds.Tables["Patient"].Rows[0]["LASTPHARMACYID"].ToString();
            if (_lastpharmid != null && _lastpharmid != "" && _lastpharmid != "00000000-0000-0000-0000-000000000000")
            {
                AddNode(xmldoc, patccrxmlelm, "PHARMACYDE", patds.Tables["Patient"].Rows[0]["LASTPHARMACYID"].ToString());
                AddNode(xmldoc, patccrxmlelm, "PHARMACY", patds.Tables["Patient"].Rows[0]["LASTPHARMACYNAME"].ToString());
            }
            //AddNode(xmldoc, patccrxmlelm, "PATIENTXID", patds.Tables["Patient"].Rows[0]["ChartID"].ToString());

            if (patds.Tables["PatientAllergy"].Rows.Count > 0)
                AddNode(xmldoc, patccrxmlelm, "ALLERGY", "Yes");
            else
                AddNode(xmldoc, patccrxmlelm, "ALLERGY", "No");

            if (patds.Tables["Plan"].Rows.Count > 0)
            {
                AddNode(xmldoc, patccrxmlelm, "PRIMARYINSURANCE", patds.Tables["Plan"].Rows[0]["Name"].ToString());
            }


        }
        if (_lastpharmid != null && _lastpharmid != "" && _lastpharmid != "00000000-0000-0000-0000-000000000000")
        {
            DataSet pharmancyds =   Allscripts.Impact.Pharmacy.LoadPharmacy(_lastpharmid, base.DBID);

            if (pharmancyds.Tables["Pharmacy"].Rows.Count > 0)
            {
                System.Xml.XmlElement patapharmccrxmlelm = xmldoc.CreateElement("Pharmacy");
                xmlroot.AppendChild(patapharmccrxmlelm);
                DataRow[] Pharm = pharmancyds.Tables["Pharmacy"].Select(); 
                foreach (DataRow dr in Pharm)
                {
                   // AddNode(xmldoc, patapharmccrxmlelm, "APPGROUP", "eRxNow");
                    AddNode(xmldoc, patapharmccrxmlelm, "NAME", "Pharmacy");
                   // AddNode(xmldoc, patapharmccrxmlelm, "PLURAL", "false");
                   // AddNode(xmldoc, patapharmccrxmlelm, "ACTION", "Load");
                    AddNode(xmldoc, patapharmccrxmlelm, "ID", dr["PharmacyID"].ToString());
                    AddNode(xmldoc, patapharmccrxmlelm, "ENTRYNAME", dr["Name"].ToString());
                    AddNode(xmldoc, patapharmccrxmlelm, "ADDRESSLINE1", dr["Address1"].ToString());
                    AddNode(xmldoc, patapharmccrxmlelm, "ADDRESSLINE2", dr["Address1"].ToString());
                    AddNode(xmldoc, patapharmccrxmlelm, "CITY", dr["City"].ToString());
                    AddNode(xmldoc, patapharmccrxmlelm, "STATE", dr["State"].ToString());
                    AddNode(xmldoc, patapharmccrxmlelm, "ZIPCODE", dr["Zip"].ToString());
                    AddNode(xmldoc, patapharmccrxmlelm, "NABPNUMBER", dr["NABP"].ToString());
                    AddNode(xmldoc, patapharmccrxmlelm, "STATUS", dr["Active"].ToString());
                    AddNode(xmldoc, patapharmccrxmlelm, "PHONEAREA", dr["PhoneAreaCode"].ToString());
                    AddNode(xmldoc, patapharmccrxmlelm, "PHONENUMBER", dr["PhoneNumber"].ToString());
                }

            }

        }
 
        if (patds.Tables["PatientAllergy"].Rows.Count > 0)
        {
            System.Xml.XmlElement patallergysccrxmlelm = xmldoc.CreateElement("Allergies");
            xmlroot.AppendChild(patallergysccrxmlelm);
            //AddNode(xmldoc, patallergysccrxmlelm, "APPGROUP", "eRxNow");
            AddNode(xmldoc, patallergysccrxmlelm, "NAME", "Allergy");
            //AddNode(xmldoc, patallergysccrxmlelm, "PLURAL", "false");
            //AddNode(xmldoc, patallergysccrxmlelm, "ACTION", "Load");
            AddNode(xmldoc, patallergysccrxmlelm, "PATIENTID", patientID);
            
            DataRow[] PatAllergy = patds.Tables["PatientAllergy"].Select("Active='Y'");
            string AllergyStatId = "";
            string AllergyStatDesc = "";
            foreach (DataRow dr in PatAllergy)
            {
                System.Xml.XmlElement patallergyccrxmlelm = xmldoc.CreateElement("ITEM");
                System.Xml.XmlAttribute patallergyccrxmlattr = xmldoc.CreateAttribute("ID");
                patallergyccrxmlattr.Value = dr["PatientAllergyID"].ToString();
                patallergyccrxmlelm.Attributes.Append(patallergyccrxmlattr);
                patallergysccrxmlelm.AppendChild(patallergyccrxmlelm);
                AddNode(xmldoc, patallergyccrxmlelm, "ID", dr["PatientAllergyID"].ToString());
                AddNode(xmldoc, patallergyccrxmlelm, "ENTRYNAME", ChkNull(dr["AllergyName"].ToString().Trim()));
                
                AllergyStatId = dr["Active"].ToString();
                switch (AllergyStatId )
                {
                    case "Y" : 
                        AllergyStatDesc  = "Active";
                        break;
                    case "N" : 
                        AllergyStatDesc  = "Not Active";
                        break;
                    case "E" : 
                        AllergyStatDesc  = "Entered in error";
                        break;
                    default:
                        AllergyStatDesc = "Unknown";
                        break;

                }
                AddNode(xmldoc, patallergyccrxmlelm, "ALLERGYSTATUS", AllergyStatDesc);
                AddNode(xmldoc, patallergyccrxmlelm, "ALLERGYSTATUSCODE", dr["Active"].ToString());
                AddNode(xmldoc, patallergyccrxmlelm, "ALLERGYCODE", dr["AllergyID"].ToString());
                AddNode(xmldoc, patallergyccrxmlelm, "ALLERGYCATEGORY", dr["AllergyType"].ToString());

            }
        }
        else if ((patds.Tables["Patient"].Rows.Count > 0) && 
            (patds.Tables["Patient"].Rows[0]["NoKnownAllergies"].ToString() == "Y"))
        {
            System.Xml.XmlElement patallergysccrxmlelm = xmldoc.CreateElement("Allergies");
            xmlroot.AppendChild(patallergysccrxmlelm);
            //addNode(xmldoc, patallergysccrxmlelm, "APPGROUP", "eRxNow");
            AddNode(xmldoc, patallergysccrxmlelm, "NAME", "Allergy");
            //addNode(xmldoc, patallergysccrxmlelm, "PLURAL", "false");
            //addNode(xmldoc, patallergysccrxmlelm, "ACTION", "Load");
            AddNode(xmldoc, patallergysccrxmlelm, "PATIENTID", patientID);

            System.Xml.XmlElement patallergyccrxmlelm = xmldoc.CreateElement("ITEM");
            System.Xml.XmlAttribute patallergyccrxmlattr = xmldoc.CreateAttribute("ID");
            patallergyccrxmlattr.Value = "33333333-3333-3333-3333-333333333333";
            patallergyccrxmlelm.Attributes.Append(patallergyccrxmlattr);
            patallergysccrxmlelm.AppendChild(patallergyccrxmlelm);
            AddNode(xmldoc, patallergyccrxmlelm, "ID", "33333333-3333-3333-3333-333333333333");
            AddNode(xmldoc, patallergyccrxmlelm, "ENTRYNAME", "No Known Allergies");
            AddNode(xmldoc, patallergyccrxmlelm, "ALLERGYSTATUS", "Active");
        }

        //Retrieve the patient's distinct active medications 
        if (patds.Tables["PatientPrescription"].Rows.Count > 0)
        {
            System.Xml.XmlElement patamedsccrxmlelm = xmldoc.CreateElement("Medications_Active");
            xmlroot.AppendChild(patamedsccrxmlelm);
           // AddNode(xmldoc, patamedsccrxmlelm, "APPGROUP", "eRxNow");
            AddNode(xmldoc, patamedsccrxmlelm, "NAME", "Medication");
           // AddNode(xmldoc, patamedsccrxmlelm, "PLURAL", "false");
           // AddNode(xmldoc, patamedsccrxmlelm, "ACTION", "Load");
            AddNode(xmldoc, patamedsccrxmlelm, "PATIENTID", patientID);

            DataRow[] PatMeds = patds.Tables["PatientPrescription"].Select("Status=0");
            string MedStatId = "";
            string MedStatDesc = "";
            foreach (DataRow dr in PatMeds)
            {
                System.Xml.XmlElement patamedccrxmlelm = xmldoc.CreateElement("ITEM");
                System.Xml.XmlAttribute patamedccrxmlattr = xmldoc.CreateAttribute("ID");
                patamedccrxmlattr.Value = dr["RxID"].ToString();
                patamedccrxmlelm.Attributes.Append(patamedccrxmlattr);
                patamedsccrxmlelm.AppendChild(patamedccrxmlelm);
                AddNode(xmldoc, patamedccrxmlelm, "ID", dr["RxID"].ToString());
                AddNode(xmldoc, patamedccrxmlelm, "ENTRYNAME", ChkNull(dr["DrugDescription"].ToString()).Trim());
                MedStatId = dr["Status"].ToString();
                switch (MedStatId)
                {
                    case "0":
                        MedStatDesc = "New";
                        break;
                    case "1":
                        MedStatDesc = "Pending Approval";
                        break;
                    case "2":
                        MedStatDesc = "Rejected";
                        break;
                    case "3":
                        MedStatDesc = "Pending Transmission";
                        break;
                    case "4":
                        MedStatDesc = "Discontinued";
                        break;
                    default:
                        MedStatDesc = "Unknown";
                        break;

                }
                AddNode(xmldoc, patamedccrxmlelm, "DDI", dr["DDI"].ToString());
                AddNode(xmldoc, patamedccrxmlelm, "MEDICATIONSTATUS", MedStatDesc);
                AddNode(xmldoc, patamedccrxmlelm, "MEDICATIONSTATUSCODE", MedStatId);
                AddNode(xmldoc, patamedccrxmlelm, "PRESCRIBEDBY", ChkNull(dr["ProviderName"].ToString()).Trim());
                //AddNode(xmldoc, patamedccrxmlelm, "FREQUNITS", dr["ProviderName"].ToString());
                AddNode(xmldoc, patamedccrxmlelm, "DOSE", ChkNull(dr["DosageFormCode"].ToString()).Trim());
                AddNode(xmldoc, patamedccrxmlelm, "ROUTEOFADMINDISPLAY", ChkNull(dr["RouteOfAdminCode"].ToString()).Trim());
                AddNode(xmldoc, patamedccrxmlelm, "NDCNUMBER", dr["NDC"].ToString());
                AddNode(xmldoc, patamedccrxmlelm, "SIG", ChkNull(dr["SIGText"].ToString()).Trim());
                AddNode(xmldoc, patamedccrxmlelm, "EXPIRATIONDT", dr["ExpirationDate"].ToString());
                AddNode(xmldoc, patamedccrxmlelm, "DRUGNAME", ChkNull(dr["MedicationName"].ToString()).Trim());
                AddNode(xmldoc, patamedccrxmlelm, "STARTDATE", dr["StartDate"].ToString());
                AddNode(xmldoc, patamedccrxmlelm, "ENDDATE", dr["EndDate"].ToString());
                AddNode(xmldoc, patamedccrxmlelm, "STRENGTH", ChkNull(dr["Strength"].ToString()).Trim());
                AddNode(xmldoc, patamedccrxmlelm, "STRENGTHUOM", ChkNull(dr["StrengthUOM"].ToString()).Trim());
                AddNode(xmldoc, patamedccrxmlelm, "QUANTITY", ChkNull(dr["Quantity"].ToString()).Trim());
                AddNode(xmldoc, patamedccrxmlelm, "DAYSSUPPLY", ChkNull(dr["DaysSupply"].ToString()).Trim());
                AddNode(xmldoc, patamedccrxmlelm, "REFILLS", ChkNull(dr["RefillQuantity"].ToString()).Trim());


            }
        }

        //Retrieve the patient's distinct active diagnosis
        DataSet activeDx = Patient.PatientDiagnosis(patientID, _lic, _userid, base.DBID);
        if (activeDx.Tables["PatientDiagnosis"].Rows.Count > 0)
        {
            System.Xml.XmlElement patadiagsccrxmlelm;
            DataRow[] PatDiags = activeDx.Tables["PatientDiagnosis"].Select("Active='Y'");
            if (PatDiags.Length > 0)
            {
                patadiagsccrxmlelm = xmldoc.CreateElement("Problems_Active");
                xmlroot.AppendChild(patadiagsccrxmlelm);
             //   AddNode(xmldoc, patadiagsccrxmlelm, "APPGROUP", "eRxNow");
                AddNode(xmldoc, patadiagsccrxmlelm, "NAME", "Problem");
             //   AddNode(xmldoc, patadiagsccrxmlelm, "PLURAL", "false");
             //   AddNode(xmldoc, patadiagsccrxmlelm, "ACTION", "Load");
                AddNode(xmldoc, patadiagsccrxmlelm, "PATIENTID", patientID);
                foreach (DataRow dr in PatDiags)
                {
                    System.Xml.XmlElement patadiagccrxmlelm = xmldoc.CreateElement("ITEM");
                    System.Xml.XmlAttribute patadiagccrxmlattr = xmldoc.CreateAttribute("ID");
                    patadiagccrxmlattr.Value = dr["PatientDiagnosisID"].ToString();
                    patadiagccrxmlelm.Attributes.Append(patadiagccrxmlattr);
                    patadiagsccrxmlelm.AppendChild(patadiagccrxmlelm);
                    AddNode(xmldoc, patadiagccrxmlelm, "ID", dr["PatientDiagnosisID"].ToString());
                    AddNode(xmldoc, patadiagccrxmlelm, "PROBLEM", ChkNull(dr["Description"].ToString()).Trim());
                    AddNode(xmldoc, patadiagccrxmlelm, "PROBLEMSTATUS", "Active");
                    AddNode(xmldoc, patadiagccrxmlelm, "ICD9DIAGNOSISCODE", ChkNull(dr["ICD9Code"].ToString()).Trim());
                }
            }
            PatDiags = activeDx.Tables["PatientDiagnosis"].Select("Active='N'");
            if (PatDiags.Length > 0)
            {

                patadiagsccrxmlelm = xmldoc.CreateElement("Problems_PH");
                xmlroot.AppendChild(patadiagsccrxmlelm);
              //  AddNode(xmldoc, patadiagsccrxmlelm, "APPGROUP", "eRxNow");
                AddNode(xmldoc, patadiagsccrxmlelm, "NAME", "Problem");
              //  AddNode(xmldoc, patadiagsccrxmlelm, "PLURAL", "false");
              //  AddNode(xmldoc, patadiagsccrxmlelm, "ACTION", "Load");
                AddNode(xmldoc, patadiagsccrxmlelm, "PATIENTID", patientID);
              //  AddNode(xmldoc, patadiagsccrxmlelm, "LOADTYPE", "PH");
                foreach (DataRow dr in PatDiags)
                {
                    System.Xml.XmlElement patadiagccrxmlelm = xmldoc.CreateElement("ITEM");
                    System.Xml.XmlAttribute patadiagccrxmlattr = xmldoc.CreateAttribute("ID");
                    patadiagccrxmlattr.Value = dr["PatientDiagnosisID"].ToString();
                    patadiagccrxmlelm.Attributes.Append(patadiagccrxmlattr);
                    patadiagsccrxmlelm.AppendChild(patadiagccrxmlelm);
                    AddNode(xmldoc, patadiagccrxmlelm, "ID", dr["PatientDiagnosisID"].ToString());
                    AddNode(xmldoc, patadiagccrxmlelm, "PROBLEM", ChkNull(dr["Description"].ToString()).Trim());
                    AddNode(xmldoc, patadiagccrxmlelm, "PROBLEMSTATUS", "Resolved");
                    AddNode(xmldoc, patadiagccrxmlelm, "ICD9DIAGNOSISCODE", ChkNull(dr["ICD9Code"].ToString()).Trim());
                }
            }

            System.Xml.XmlElement patccrxmlelm = xmldoc.CreateElement("Options");
            xmlroot.AppendChild(patccrxmlelm);
            System.Xml.XmlElement toccrxmlelm = xmldoc.CreateElement("TO");
            patccrxmlelm.AppendChild(toccrxmlelm);
            AddNode(xmldoc, toccrxmlelm, "PATIENT", "True");
            AddNode(xmldoc, toccrxmlelm, "IS", "True");
            System.Xml.XmlElement purposeccrxmlelm = xmldoc.CreateElement("PURPOSE");
            patccrxmlelm.AppendChild(purposeccrxmlelm);
            AddNode(xmldoc, purposeccrxmlelm, "PATIENT", "False");   /* set if the patient request the CCR */
            AddNode(xmldoc, purposeccrxmlelm, "PATIENTTEXT", "Patient Request");
            AddNode(xmldoc, purposeccrxmlelm, "REFERRAL", "False");  /* set if the CCR is for a referral */
            AddNode(xmldoc, purposeccrxmlelm, "REFERRALTEXT", "Request for Referral");
            AddNode(xmldoc, purposeccrxmlelm, "PARTNER", "True");    /* set if the eRxNow Partner Request Patient snapshot */
            AddNode(xmldoc, purposeccrxmlelm, "PARTNERTEXT", "Partner Request.");
            AddNode(xmldoc, purposeccrxmlelm, "TRANSFER", "False");  /* set if the eRxNow user requested patient snapshot to transfer to other site */
            AddNode(xmldoc, purposeccrxmlelm, "TRANSFERTEXT", "eRxNow Request");         


            RxPrincipal rx = null;
            string userguid = (string)Session["USERID"];
            rx = new RxPrincipal(userguid, Guid.Empty.ToString(), Guid.Empty.ToString(), 1);
            patccrxmlelm = xmldoc.CreateElement("Source");
            xmlroot.AppendChild(patccrxmlelm);
            AddNode(xmldoc, patccrxmlelm, "ERXVERSION", "eRxNow v1.0");
            AddNode(xmldoc, patccrxmlelm, "LICENSE", ChkNull(((RxIdentity)rx.Identity).LicenseID).Trim());
            AddNode(xmldoc, patccrxmlelm, "SITENAME", ChkNull(((RxIdentity)rx.Identity).SiteName).Trim());
            AddNode(xmldoc, patccrxmlelm, "SITENUMBER", ((RxIdentity)rx.Identity).SiteID.ToString());
            AddNode(xmldoc, patccrxmlelm, "PRACTICENAME", ChkNull(((RxIdentity)rx.Identity).CurrentLicense.PracticeName).Trim());
            AddNode(xmldoc, patccrxmlelm, "ADDRESSLINE1", ChkNull(((RxIdentity)rx.Identity).CurrentLicense.PracticeAddress1).Trim());
            AddNode(xmldoc, patccrxmlelm, "ADDRESSLINE2", ChkNull(((RxIdentity)rx.Identity).CurrentLicense.PracticeAddress2).Trim());
            AddNode(xmldoc, patccrxmlelm, "CITY", ChkNull(((RxIdentity)rx.Identity).CurrentLicense.PracticeCity).Trim());
            AddNode(xmldoc, patccrxmlelm, "STATE", ChkNull(((RxIdentity)rx.Identity).CurrentLicense.PracticeState).Trim());
            AddNode(xmldoc, patccrxmlelm, "ZIPCODE", ChkNull(((RxIdentity)rx.Identity).CurrentLicense.PracticeZip).Trim());
            AddNode(xmldoc, patccrxmlelm, "PHONE", ChkNull(((RxIdentity)rx.Identity).CurrentLicense.PracticePhone).Trim());
            AddNode(xmldoc, patccrxmlelm, "FAX", ChkNull(((RxIdentity)rx.Identity).CurrentLicense.PracticeFax).Trim());
            AddNode(xmldoc, patccrxmlelm, "TIN", ChkNull(((RxIdentity)rx.Identity).CurrentLicense.TaxID).Trim());
            AddNode(xmldoc, patccrxmlelm, "PATIENTID", patientID);

            patccrxmlelm = xmldoc.CreateElement("Target");
            xmlroot.AppendChild(patccrxmlelm);
            AddNode(xmldoc, patccrxmlelm, "REPOSITORY", "");

            patccrxmlelm = xmldoc.CreateElement("Document");
            xmlroot.AppendChild(patccrxmlelm);
            AddNode(xmldoc, patccrxmlelm, "SESSIONID", (string)Session.SessionID);
            string dt =  System.DateTime.Now.ToString("yyyy-MM-dd") + "T" + System.DateTime.Now.ToString("HHmmss") + "Z";
            AddNode(xmldoc, patccrxmlelm, "CreateDTTMZ", dt);
        }

        patds.Clear();
        patds.AcceptChanges();
        patds.Dispose();
        patds = null;


        HttpContext context = HttpContext.Current;
        string filePath = null;
        if (context != null)
           filePath = context.Server.MapPath("patccr.xml");
       xmldoc.Save(filePath); 

        
    }

}

}