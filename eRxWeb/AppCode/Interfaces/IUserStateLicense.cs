using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eRxWeb.State;
using eRxWeb.ServerModel;


namespace eRxWeb.AppCode.Interfaces
{
    public interface IUserStateLicense
    {
        StateLicense[] GetStateLicenses(IStateContainer session, string userId);

        string[][] GetStatesAndLicenseTypes(IStateContainer session);

        void AddProviderLicense(IStateContainer session, string userId, StateLicense license);

        void UpdateProviderLicense(IStateContainer session, string userId, StateLicense originalLicense, StateLicense updatedLicense);

        void DeleteProviderLicense(IStateContainer session, string userId, StateLicense license);
    }
}
