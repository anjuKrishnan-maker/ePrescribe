using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Allscripts.ePrescribe.DatabaseSelector;

namespace eRxWeb.AppCode.SpecialtyMedWorkflow
{
    public class DXCRecommendationContext
    {
        #region eRxSession
        public string LicenseID { get; set; }
        public Guid UserID { get; set; }
        public ConnectionStringPointer DbID { get; set; }
        #endregion

        #region PatientInformation
        public DateTime PatientDOB { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }
        public string PatientGender { get; set; }
        public string PatientID { get; set; }
        public string PatientMRN { get; set; }
        public string PatientStreetAddress1 { get; set; }
        public string PatientStreetAddress2 { get; set; }
        public string PatientCity { get; set; }
        public string PatientState { get; set; }
        public string PatientPostalCode { get; set; }
        public string PatientTelephone { get; set; }
        public string PatientMobile { get; set; }
        public string PatientEmail { get; set; }
        #endregion

        #region ProviderInformation
        public string ProviderFirstName { get; set; }
        public string ProviderLastName { get; set; }
        public string ProviderDegree { get; set; }
        public string ProviderID { get; set; }//corresponds to NPI
        public string DEANumber { get; set; }//corresponds to DEA
        #endregion

        #region OrganizationInformation
        public string OrganizationName { get; set; }
        public string OrganizationStreetAddress1 { get; set; }
        public string OrganizationStreetAddress2 { get; set; }
        public string OrganizationCity { get; set; }
        public string OrganizationState { get; set; }
        public string OrganizationPostalCode { get; set; }
        public string OrganizationPhone { get; set; }
        #endregion


    }
}