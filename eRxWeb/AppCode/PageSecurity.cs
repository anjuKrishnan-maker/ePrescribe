/// <summary>
/// Summary description for PageSecurity
/// </summary>
using Allscripts.ePrescribe.Common;

public class PageSecurity
{
    private string ProcessStartingPage;

    public PageSecurity()
    {
        ProcessStartingPage = Constants.PageNames.IDPROOFING_REQUIRED_INFO;
    }

    public string StartingPage
    {
        get { return ProcessStartingPage; }
    }

    public bool ValidConnection(string FromPage, string CurrentPage)
    {
        FromPage = FromPage.ToUpper();
        CurrentPage = CurrentPage.ToUpper();
        bool trusted = false;
        switch (CurrentPage)
        {
            case ("ASP.IDPROOFINGREQUIREDINFO_ASPX"):
                if (string.Equals(FromPage, Constants.PageNames.IDPROOFING_REQUIRED_INFO, System.StringComparison.CurrentCultureIgnoreCase))
                    trusted = true;
                break;

            case ("ASP.IDOLOGYANSWERS_ASPX"):
                if (string.Equals(FromPage, Constants.PageNames.USER_INTERROGATION, System.StringComparison.CurrentCultureIgnoreCase))
                    trusted = true;
                break;

            case ("ASP.USERCHALLENGE_ASPX"):
                if (string.Equals(FromPage, Constants.PageNames.IDOLOGY_ANSWERS, System.StringComparison.CurrentCultureIgnoreCase))
                    trusted = true;
                break;

            case ("ASP.USERINTERROGATION_ASPX"):
                if (string.Equals(FromPage, Constants.PageNames.USER_INTERROGATION, System.StringComparison.CurrentCultureIgnoreCase))
                    trusted = true;
                else if (string.Equals(FromPage, Constants.PageNames.IDPROOFING_REQUIRED_INFO, System.StringComparison.CurrentCultureIgnoreCase))
                    trusted = true;
                break;
            default:
                trusted = false;
                break;
        }
        return trusted;
    }
}