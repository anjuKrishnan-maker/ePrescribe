namespace eRxWeb.AppCode.Interfaces
{
    public interface IBrowserUtil
    {
        bool IsBrowserUpgradeNeeded(string userAgent, string layoutEngine);
    }
}