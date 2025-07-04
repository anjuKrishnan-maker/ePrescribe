using eRxWeb.State;

namespace eRxWeb.AppCode.Interfaces
{
    public interface IUserProperties
    {
        string GetUserPropertyValue(string name, IStateContainer pageState);
    }
}