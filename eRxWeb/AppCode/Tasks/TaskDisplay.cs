using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact.Tasks;

namespace eRxWeb.AppCode.Tasks
{
    public class TaskDisplay
    {
        public static TaskRxDetailsModel CreateRxDetail(ScriptMessageNodes sm, string formDescription, string pharmacyDetails)
        {
            var model = new TaskRxDetailsModel();

            if (sm.Prescription != null)
            {
                model.DrugDescription = sm.Prescription.SelectSingleNode("DrugDescription")?.InnerText.Trim() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(formDescription) && (!string.IsNullOrEmpty(model.DrugDescription) && !model.DrugDescription.Contains(formDescription))) model.DrugDescription += $" {formDescription}";
                if (string.IsNullOrEmpty(model.DrugDescription)) model.DrugDescription = CommonTerms.NotSpecified;

                model.Quantity = sm.Prescription.SelectSingleNode("Quantity")?.InnerText.Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(model.Quantity)) model.Quantity = CommonTerms.NotSpecified;

                model.DaysSupply = sm.Prescription.SelectSingleNode("DaysSupply")?.InnerText.Trim() ?? string.Empty;

                model.Refills = GetRefillText(sm.Prescription.SelectSingleNode("RefillQuantity")?.InnerText.Trim() ?? string.Empty, sm.Prescription.SelectSingleNode("RefillQuantityQual")?.InnerText.Trim() ?? string.Empty);

                model.SigText = sm.Prescription.SelectSingleNode("SIGText")?.InnerText.Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(model.SigText)) model.SigText = CommonTerms.NotSpecified;

                model.RxNotes = sm.Prescription.SelectSingleNode("PharmacyNotes")?.InnerText.Trim() ?? sm.Prescription.SelectSingleNode("RxNotes")?.InnerText.Trim() ?? string.Empty;

                model.CreatedDate = GetDateOrNotSpecified(sm.Prescription.SelectSingleNode("Created")?.InnerText.Trim());

                model.LastFillDate = GetDateOrNotSpecified(sm.Prescription.SelectSingleNode("LastFillDate")?.InnerText.Trim());

                model.Daw = sm.Prescription.SelectSingleNode("DAW")?.InnerText.Trim() ?? string.Empty;

                if (sm.Provider != null)
                {
                    if (sm.SupervisingProvider != null)
                    {
                        model.ProviderOfRecord = $"{sm.SupervisingProvider.SelectSingleNode("LastName")?.InnerText.Trim()}, {sm.SupervisingProvider.SelectSingleNode("FirstName")?.InnerText.Trim()}";
                    }
                    else
                    {
                        model.ProviderOfRecord = $"{sm.Provider.SelectSingleNode("LastName")?.InnerText.Trim()}, {sm.Provider.SelectSingleNode("FirstName")?.InnerText.Trim()}";
                    }
                }

                model.PharmacyDetails = pharmacyDetails;
            }

            model.ChangeReason = ChangeRxTask.GetReasonText(sm.ChangeRxRequest);

            return model;
        }

        public static string GetDateOrNotSpecified(string date)
        {
            DateTime datetime;
            var formattedDate = CommonTerms.NotSpecified;

            if (!string.IsNullOrWhiteSpace(date))
            {
                DateTime.TryParse(date.Trim(), out datetime);
                if (datetime != DateTime.MinValue)
                {
                    formattedDate = datetime.ToShortDateString();
                }
            }

            return formattedDate;
        }


        public static string GetRefillText(string quantity, string quantityQual)
        {
            int refill = 0;
            Int32.TryParse(quantity, out refill);

            string text = null;

            if (refill == 0 && quantity != "0")
            {
                if (!string.IsNullOrWhiteSpace(quantityQual))
                {
                    text = quantityQual;
                }
            }
            else
            {
                text = refill.ToString();
            }

            if (refill > 99)
            {
                text = "0";
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                text = CommonTerms.NotSpecified;
            }

            return text;
        }

        public static string GetFormattedRxDetails(string patientName, bool isGeneric, TaskRxDetailsModel model)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(patientName))
            {
                sb.Append(patientName);
                sb.Append("<BR>");
            }

            if (!string.IsNullOrWhiteSpace(model.ChangeReason))
            {
                sb.Append($"<b>{model.ChangeReason.ToHTMLEncode()}</b><hr>");
            }

            if (!isGeneric)
            {
                sb.Append("<b>Original Medication Prescribed: ");
                sb.Append(model?.DrugDescription.ToHTMLEncode());
                sb.Append("</b>");
            }
            else
            {
                sb.Append("<b>Original Medication Prescribed: </b>");
                sb.Append(model?.DrugDescription.ToHTMLEncode());
            }

            sb.Append("<BR><b>SIG:</b> ");
            sb.Append(model?.SigText.ToHTMLEncode());

            sb.Append("<BR><b>Total number of refills requested:</b> ");
            sb.Append(model?.Refills.ToHTMLEncode());
            sb.Append(", <b>Quantity:</b> ");
            sb.Append(model?.Quantity.ToHTMLEncode());

            if (!string.IsNullOrWhiteSpace(model?.DaysSupply))
            {
                sb.Append(", <b>Days Supply:</b> ");
                sb.Append(model.DaysSupply.ToHTMLEncode());
            }

            if (string.Compare(model?.Daw, Constants.CommonAbbreviations.YES, true) == 0 || string.Compare(model?.Daw, "1", true) == 0)
            {
                sb.Append(", <b>DAW</b>");
            }

            if (!string.IsNullOrEmpty(model?.RxNotes))
            {
                sb.Append("<BR><b>Notes: ");
                sb.Append(model.RxNotes.ToHTMLEncode());
                sb.Append("</b>");
            }
            sb.Append("&nbsp;&nbsp;&nbsp;");

            if (!string.IsNullOrEmpty(model?.PharmacyDetails))
            {
                sb.Append("<BR><b>Pharmacy: </b>");
                sb.Append(model.PharmacyDetails.ToHTMLEncode());
            }
            sb.Append("&nbsp;&nbsp;&nbsp;");


            if (!string.IsNullOrEmpty(model?.ProviderOfRecord))
            {
                sb.Append("<BR><b>Provider of record:</b> ");
                sb.Append(model.ProviderOfRecord.ToHTMLEncode());
                sb.Append("&nbsp;&nbsp;&nbsp;");
            }

            if (!string.IsNullOrEmpty(model?.CreatedDate) && model?.CreatedDate != CommonTerms.NotSpecified)
            {
                sb.Append("<b>Original Rx Date:</b> ");
                sb.Append(model.CreatedDate.ToHTMLEncode());
                sb.Append("&nbsp;&nbsp;&nbsp;");
            }

            if (!string.IsNullOrEmpty(model?.LastFillDate) && model?.LastFillDate != CommonTerms.NotSpecified)
            {
                sb.Append("<b>Last Fill Date:</b> ");
                sb.Append(model.LastFillDate.ToHTMLEncode());
                sb.Append("&nbsp;&nbsp;&nbsp;");
            }

            return sb.ToString();
        }
    }
}