using eRxWeb.Controls;
using eRxWeb.Controls.Interfaces;
using eRxWeb.State;
using Telerik.Web.UI;

namespace eRxWeb.PageInterfaces
{
    public interface IScriptPad
    {
        /// <summary>
        /// Checks for secondary workflow before saving scripts
        /// </summary>
        void CheckForSecondaryWorkflow(IScriptPad iScriptPad, int epcsMedCount, IControls_EPCSDigitalSigning epcsControl, IControlsPatMissingInfo patControl, IStateContainer pageState, GridDataItemCollection scriptPadItems, IControls_SpecialtyMedsUserWelcome SpecMedsWelcome, bool isMissingPatientInformationEntered);

        bool IsSendingMedToMailOrder(GridDataItemCollection scriptPadItems);
        bool IsSendingToPharmacy(GridDataItemCollection scriptPadItems);
       // bool IsSendingMedToApproval(GridDataItemCollection scriptPadItems);
        void saveScripts();

        /// <summary>
        /// check if any cs meds where eligible to be sent electronically and Send To Pharmacy/Send To Mail Order was selected
        /// </summary>
        /// <returns></returns>
        bool isEPCSDigitalSigningRequired();

        bool IsSendingSpecMedsTaskList(GridDataItemCollection scriptPadItems);

    }
}