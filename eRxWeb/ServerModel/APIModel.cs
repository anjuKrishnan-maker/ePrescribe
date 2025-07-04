using Allscripts.ePrescribe.Common;
using System.Collections.Generic;

namespace eRxWeb.ServerModel
{
    public class APIModel
    {
 
    }
    class SiteInfo
    {
        public string siteDetails { get; set; }
        public string siteName { get; set; }
        public string LastLoginDate { get; set; }
        public User user { get; set; }
        public string shieldStatus { get; set; }
        public string SelectSiteUrl { get; set; }
        public bool IsMultipleSite { get; set; }
        public bool IsTieAdsEnabled { get; internal set; }
        public bool IsRestrictedMenu { get; set; }

    }

    public class UserInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MI { get; set; }
        public string LoginId { get; set; }
        public string Email { get; set; }
        public string ConfirmEmail { get; set; }
        public bool IsActive { get; set; }
        public string lastLogin { get; set; }

    }
    public enum UserType
    {
        Provider = 1,
        PAwoSup,
        PAwithSup,
        POBnoReview,
        POBSomeReview,
        POBAllReview,
        Staff
    }
    public class UserEPCSSetting
    {
        public bool IsApprover { get; set; }
        public bool IsEPCSOn { get; set; }
    }
    public class User
    {
        public User()
        {
            this.Login = new UserInfo();
            this.UserCredential = new UserCredential();
        }
        public UserInfo Login { get; set; }
        public Constants.UserCategory UserType { get; set; }
        public bool IsAdmin { get; set; }
        public UserEPCSSetting EPCSPermission { get; set; }
        public UserCredential UserCredential { get; set; }
        public ePrescribeSvc.DEALicense[] DEALicenses { get; set; }
        public List<StateLicense> StateLicenses { get; set; }
    }

    public class StateLicense
    {
        public string State { get; set; }
        public string ExpiringDate { get; set; }
        public string LicenseNo { get; set; }
        public string LicenseType { get; set; }
    }
    public class UserCredential
    {
        public string Title { get; set; }
        public string Suffix { get; set; }
        public string NPI { get; set; }
        public string Spaciality1 { get; set; }
        public string GenericLicense { get; set; }
        public string Spaciality2 { get; set; }
    }

    class KeyValue
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    class ComponentPorperty
    {
        public bool visible { get; set; }
        public string htmlTag { get; set; }
        public string name { get; set; }

    }
    public class ILearnModel
    {
        public string Url { get; set; }
        public string ILearnNotification { get; set; }
    }
    public class AppInfo
    {
        public string VersionNumber { get; set; }
    }
}