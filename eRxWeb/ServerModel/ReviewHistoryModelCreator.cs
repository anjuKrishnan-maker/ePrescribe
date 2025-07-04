using System;
using System.Globalization;
using System.Text;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data.Model;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.ExtensionMethods;
using eRxWeb.AppCode;
using eRxWeb.State;

namespace eRxWeb.ServerModel
{
    public static class ReviewHistoryModelCreator
    {
        public static ReviewHistoryModel Create(ReviewHistoryDataModel dataModel, ConnectionStringPointer dbId,
            IStateContainer session)
        {


            var statusDescription = ComputeStatusDescription(dataModel, session.Cast(Constants.SessionVariables.SiteId, 1), session.GetGuidOr0x0(Constants.SessionVariables.LicenseId), dbId);
            var selectionGroup = ComputeSelectionGroups(dataModel, statusDescription, session);
            var prescriptionText = CreatePrescriptionText(dataModel);
            var isScriptCancelRxEligible = CancelRx.IsScriptCancelRxEligile(dataModel.TransmissionMethod,
                dataModel.StartDate.ToString(CultureInfo.InvariantCulture), dataModel.ControlledSubstanceCode, dataModel.IsPharmacyCancelRxEligible.GetYorN(),
                dataModel.OriginalNewRxTrnsCtrlNo);
            var isPbmMed = dataModel.Status == 0 && dataModel.Type == "R" && statusDescription.ToLower() != "patient self reported";

                var result = new ReviewHistoryModel()
            {
                Diagnosis = dataModel.Diagnosis,
                MedHistoryExists = dataModel.MedHistoryExists,
                Prescription = prescriptionText,
                RxDate = dataModel.RxDate.ToShortDateString(),
                RxID = dataModel.RxID.ToString(),
                RxSource = string.IsNullOrWhiteSpace(dataModel.RxSourceDescription) ? "Unknown" : dataModel.RxSourceDescription,
                SelectionGroupId = selectionGroup,
                StatusDescription = statusDescription,
                ExtraDetailAvailable = dataModel.ExtraDetailAvailable,
                IsScriptCancelRxEligible = isScriptCancelRxEligible,
                OriginalNewRxTrnsCtrlNo = dataModel.OriginalNewRxTrnsCtrlNo,
                IsPbmMed = isPbmMed

            };

            return result;

        }

        public static string CreatePrescriptionText(ReviewHistoryDataModel dataModel)
        {
            var sb = new StringBuilder(dataModel.Prescription);

            if (dataModel.IsRegistryChecked)
            {
                sb.Append("&nbsp;&nbsp;<b>CS Registry Checked</b>");
            }

            if (dataModel.ControlledSubstanceCode.Trim() == "2" && dataModel.TransmissionMethod == Constants.PrescriptionTransmissionMethod.SENT)
            {
                sb.Append($"&nbsp;&nbsp;Effective Date - {dataModel.StartDate:d}");
            }

            return sb.ToString();
        }

        private static string ComputeStatusDescription(ReviewHistoryDataModel reviewHistoryItem, int siteId, Guid licenseId, ConnectionStringPointer dbId)
        {
            var statusDescription = RxUtils.RenderRxDescription(
                reviewHistoryItem.RxID.ToString(),
                reviewHistoryItem.Pharmacy.ToUpper().Equals("Y"),
                reviewHistoryItem.Comments,
                reviewHistoryItem.RxSource,
                reviewHistoryItem.Type,
                (Constants.PrescriptionStatus)reviewHistoryItem.Status,
                reviewHistoryItem.TransmissionMethod,
                reviewHistoryItem.ControlledSubstanceCode,
                reviewHistoryItem.Updated.ToShortDateString(),
                reviewHistoryItem.TransmissionStatus,
                siteId,
                licenseId,
                reviewHistoryItem.DrugHistoryType,
                dbId
                );

            return statusDescription;
        }

        public static SelectionGroup ComputeSelectionGroups(ReviewHistoryDataModel item, string statusDescription, IStateContainer session)
        {
            // legacy code uses this: gStatus == 2 || gStatus == 4 || gStatus == 5 || gStatus == 7
            // however, what if there are other statuses? The selection should not enable any buttons unless we know what to do
            // so, using this as a 'catch all' with no condition
            var selectionGroup = SelectionGroup.AlwaysDisabled;


            if (item.Status == 0 && item.Type == "R" && statusDescription.ToLower() != "patient self reported")
            {
                selectionGroup = SelectionGroup.EieComplete;
            }
            else if (item.Status == 0 || item.Status == 8)
            {
                selectionGroup = SelectionGroup.DiscontinueEieComplete;
            }
            else if (item.Status == 1 || item.Status == 3)
            {
                selectionGroup = SelectionGroup.Eie;
            }
            else if (session.GetStringOrEmpty("PRACTICESTATE") == "OH" && item.RxSource != null && item.RxSource.Equals("External", StringComparison.OrdinalIgnoreCase))
            // copied comments from legacy:
            //this last check for OH and External meds is only for OH certification since meds originally entered in EMR and then marked EIE in ePrescribe
            //don't have a rx_header.providerid and the provider detail report fails.
            {
                selectionGroup = SelectionGroup.DiscontinueComplete;
            }

            return selectionGroup;


        }
    }
}