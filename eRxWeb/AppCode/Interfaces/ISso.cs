using System;
using System.Xml;
using ComponentSpace.SAML.Assertions;
using eRxWeb.ePrescribeSvc;
using eRxWeb.State;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;

namespace eRxWeb.AppCode.Interfaces
{
    public interface ISso
    {
        Partner VerifyPartnerAndSetValues(Partners partners, Assertion assertion, XmlElement xmlElement, IStateContainer pageState, string ipAddress, ConnectionStringPointer dbId);
        Partner FindAndSetPartnerValuesFromGuid(Partners partners, Guid partnerGuid, IStateContainer pageState);
        string CheckForActivationWizardRedirect(Partner partner, Guid userId, IStateContainer pageState, IEPSBroker epsBroker);
        bool VerifyResponseSigniture(Partner partner, XmlElement xmlElement);
    }
}