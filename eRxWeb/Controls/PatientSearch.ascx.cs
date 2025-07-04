#region References
/******************************************************************************
**Change History
*******************************************************************************
**Date:         Author:                 Description:
**-----------------------------------------------------------------------------
**04/14/2009    Anand Kumar Krishnan    Search Criteria validation changed 
**                                      from 3 required characters to 2 Issue#2182
**06/10/2009    Sonal Saxena            Search Criteria validation changed, will 
**                                      not allow special character other than dot,
**                                      apostrophe and hyphen(#2634)
**08/12/2010    Matthew Molina          Removed RadDatePicker and added RadDateInput 
** 
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
using System.Text.RegularExpressions;
using Allscripts.ePrescribe.Common; 
#endregion

namespace eRxWeb
{
public partial class Controls_PatientSearch : BaseControl
{
    #region Page Variables
    public PatientSearchEventArgs eventArgs;
    public event PatientSearchEventHandler PatientSearchEvent;
    #endregion

    #region Page Events
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {           
            searchError.Style["Display"] = "none";

            rdiDOB.MinDate = DateTime.Today.AddYears(-120);
            rdiDOB.MaxDate = DateTime.Today;
            rdiDOB.Culture = System.Globalization.CultureInfo.CurrentCulture;
            rdiDOB.IncrementSettings.InterceptArrowKeys = false;
            rdiDOB.IncrementSettings.InterceptMouseWheel = false;

            HttpCookie eRxNowSearchCookie;
            if (Request.Cookies["eRxNowSearchCookie"] != null)
            {
                eRxNowSearchCookie = Request.Cookies.Get("eRxNowSearchCookie");
                if (eRxNowSearchCookie["PatientSearch"] == "PatientId")
                {
                    txtPatientID.Focus();
                }
                else if (eRxNowSearchCookie["PatientSearch"] == "LastName")
                {
                    txtLastName.Focus();
                }
                else if (eRxNowSearchCookie["PatientSearch"] == "FirstName")
                {
                    txtFirstName.Focus();
                }
                else if (eRxNowSearchCookie["PatientSearch"] == "DateOfBirth")
                {
                    rdiDOB.Focus();
                }
                else
                {
                    txtLastName.Focus();
                }
            }
            else
            {
                txtLastName.Focus();
            }
        }

        if (!base.SessionLicense.EnterpriseClient.ShowPatientSearch)
        {
            txtLastName.Enabled = false;
            txtFirstName.Enabled = false;
            txtPatientID.Enabled = false;
            rdiDOB.Enabled = false;
            searchTypeMenu.Enabled = false;
            btnSearch.Enabled = false;
            btnAddPatient.Enabled = false;
        }
        else
        {
            if ((Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.PATIENTLOCKDOWNMODE)
                || !base.SessionLicense.EnterpriseClient.ShowInactivePatientSearch)
            {
                Img1.Style["Display"] = "none";
            }
        }

        if (!ShowAddPatient)
        {
            AddPanel.Style["Display"] = "none";
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        ThrowEvent(PatientSearchEventArgs.ActionType.SEARCH_ACTIVE);
    }

    protected void btnAddPatient_Click(object sender, EventArgs e)
    {
        ThrowEvent(PatientSearchEventArgs.ActionType.ADD_PATIENT);
    }

    #endregion    

    #region Page Methods
    protected void setTypeAndSearch(object sender, Telerik.Web.UI.RadMenuEventArgs e)
    {
        ThrowEvent((PatientSearchEventArgs.ActionType)Convert.ToInt32(e.Item.Value));
    }

    protected virtual void OnPatientSearchEvent(PatientSearchEventArgs e)
    {
        if (PatientSearchEvent != null)
        {
            PatientSearchEvent(this, e);
        }
    }

    public bool ShowAddPatient
    {
        get { return this.AddPanel.Visible; }
        set { this.AddPanel.Visible = value; }
    }

    public void ResetSearchFilters()
    {
        txtLastName.Text = string.Empty;
        txtFirstName.Text = string.Empty;
        txtPatientID.Text = string.Empty;
        rdiDOB.Text = string.Empty;
        spanSearchError.InnerHtml = string.Empty;
    }

    public void ThrowEvent(PatientSearchEventArgs.ActionType actionType)
    {
        if (actionType == PatientSearchEventArgs.ActionType.ADD_PATIENT || !validSearch())
        {
            OnPatientSearchEvent(new PatientSearchEventArgs(string.Empty,
                                                           string.Empty,
                                                           string.Empty,
                                                           string.Empty,
                                                           string.Empty,
                                                           actionType));
        }
        else
        {
            HttpCookie eRxNowSearchCookie;
            if (Request.Cookies["eRxNowSearchCookie"] != null)
            {
                eRxNowSearchCookie = Request.Cookies.Get("eRxNowSearchCookie");
            }
            else
            {
                eRxNowSearchCookie = new HttpCookie("eRxNowSearchCookie");
            }

            eRxNowSearchCookie.Values.Remove("PatientSearch");

            if (string.IsNullOrWhiteSpace(txtLastName.Text) &&
                string.IsNullOrWhiteSpace(txtFirstName.Text) &&
                !string.IsNullOrWhiteSpace(txtPatientID.Text))
            {
                txtPatientID.Focus();
                eRxNowSearchCookie.Values.Add("PatientSearch", "PatientId");
            }
            else if (string.IsNullOrWhiteSpace(txtLastName.Text) &&
                    string.IsNullOrWhiteSpace(txtFirstName.Text) &&
                    !rdiDOB.IsEmpty)
            {
                rdiDOB.Focus();
                eRxNowSearchCookie.Values.Add("PatientSearch", "DateOfBirth");
            }
            else
            {
                txtLastName.Focus();
                eRxNowSearchCookie.Values.Add("PatientSearch", "LastName");
            }

            string strDOB = string.Empty;

            if (rdiDOB.SelectedDate != null)
            {
                strDOB = ((DateTime)(rdiDOB.SelectedDate)).ToString("MM/dd/yyyy");
            }

            OnPatientSearchEvent(new PatientSearchEventArgs(txtLastName.Text,
                                                            txtFirstName.Text,
                                                            strDOB,
                                                            txtPatientID.Text,
                                                            string.Empty,
                                                            actionType));

            eRxNowSearchCookie.Expires = DateTime.Parse(DateTime.Today.ToShortDateString()).AddYears(1);
            Response.Cookies.Remove(eRxNowSearchCookie.Name);
            Response.Cookies.Add(eRxNowSearchCookie);
        }
    }

    private bool validSearch()
    {
        bool hasCharacters = false;
        bool hasDate = false;
        bool isValidTextLength = true;
        bool isValidTextFormat = true;
        bool isValidDate = true;
        bool isValid = false;

        spanSearchError.InnerHtml = string.Empty;

        // check the Text property of the RadDateInput control as it will contain what the user entered
        // even if it is not a valid date 
        if (!string.IsNullOrWhiteSpace(rdiDOB.Text))
        {
            hasDate = true;

            // if the enterd data is NOT a valid date IsEmpty will be true, if it is valid IsEmpty is false
            isValidDate = !rdiDOB.IsEmpty;
        }

        // check if any of the search text boxes have text entered
        if (!string.IsNullOrWhiteSpace(txtLastName.Text) || !string.IsNullOrWhiteSpace(txtFirstName.Text) || !string.IsNullOrWhiteSpace(txtPatientID.Text))
        {
            hasCharacters = true;

            // check if at least one of the text boxes has at least 2 chars, use regex "\s" to find and remove any whitespace
            isValidTextLength = (Regex.Replace(txtLastName.Text, @"[\s]+", string.Empty).Length >= 2 ||
                                Regex.Replace(txtFirstName.Text, @"[\s]+", string.Empty).Length >= 2 ||
                                Regex.Replace(txtPatientID.Text, @"[\s]+", string.Empty).Length >= 2);
        }

        if (!hasCharacters & !hasDate)
        {
            // nothing has been entered for search criteria
            spanSearchError.InnerHtml = "•  Please enter at least 2 valid characters for Last Name, First Name or Patient ID or enter a Date of Birth.";
        }
        else
        {
            if (hasCharacters & !isValidTextLength)
            {
                spanSearchError.InnerHtml = "•  Please enter at least 2 valid characters for Last Name, First Name or Patient ID.";
            }
            else if (hasCharacters & isValidTextLength & (!string.IsNullOrWhiteSpace(txtLastName.Text) || !string.IsNullOrWhiteSpace(txtFirstName.Text)))
            {
                //Regex regexPattern = new Regex(@"([a-zA-Z0-9]{2,})|[a-zA-Z0-9]{1}([\.|\'|\-]{1})");
                Regex regexPattern = new Regex(@"^[a-zA-Z0-9\.|\'|\-|\s]+$");

                if (!string.IsNullOrWhiteSpace(txtLastName.Text) && !regexPattern.IsMatch(txtLastName.Text))
                {
                    isValidTextFormat = false;
                    spanSearchError.InnerHtml = "•  Please enter a valid format for Last Name.";
                }

                if (!string.IsNullOrWhiteSpace(txtFirstName.Text) && !regexPattern.IsMatch(txtFirstName.Text))
                {
                    isValidTextFormat = false;

                    if (string.IsNullOrWhiteSpace(spanSearchError.InnerHtml))
                    {
                        spanSearchError.InnerHtml = "•  Please enter a valid format for First Name.";
                    }
                    else
                    {
                        spanSearchError.InnerHtml += "<br/>•  Please enter a valid format for First Name.";
                    }
                }
            }

            if ((hasDate & !isValidDate))
            {
                if (string.IsNullOrWhiteSpace(spanSearchError.InnerHtml))
                {
                    spanSearchError.InnerHtml = "•  Date of Birth must be in the format mm/dd/yyyy.";
                }
                else
                {
                    spanSearchError.InnerHtml += "<br/>•  Date of Birth must be in the format mm/dd/yyyy.";
                }
            }
        }

        // if the user did NOT enter a DOB or FirstName/LastName/PatientID 
        // or if the DOB entered is not valid
        // or if whatever was entered in FirstName/LastName is not valid
        // then display the error
        if ((!hasCharacters & !hasDate) ||
            (hasCharacters & (!isValidTextLength || !isValidTextFormat)) ||
            (hasDate & !isValidDate))
        {
            searchError.Style["Display"] = "block";
        }
        else
        {
            searchError.Style["Display"] = "none";
            isValid = true;
        }

        return isValid;
    } 
    #endregion
}

}