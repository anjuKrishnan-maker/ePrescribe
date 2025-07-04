using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode
{
    public class PatientDemographics
    {
    public string LastName { get; set; }
    public string PaternalName { get; set; }
    public string MaternalName { get; set; }
    public string FirstName { get; set; }
    public string MiddleName {get;set;}
    public string Dob { get; set; }		
    public string Mrn { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }
    public string Phone { get; set; }
    public string MobilePhone { get; set; }
    public string Gender { get; set; }
    public string Email { get; set; }
    public string IsPatientHistoryExcluded { get; set; }
    public string PreferredLanguageID { get; set; }
    public string Weight {get; set; }
    public string Height { get; set; }
    public string IsHealthPlanDisclosable { get; set; }
    }
}