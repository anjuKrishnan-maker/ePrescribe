using Allscripts.Impact;

namespace eRxWeb.AppCode.Interfaces
{
    public interface IBasePage
    {
        string GetSiteUrl();
        bool CanApplyFinancialOffers { get; }
        bool CheckFormAltsAndCopay { get; }
        ApplicationLicense SessionLicense { get; }
        bool IsLexicompEnabled { get; }
    }
}