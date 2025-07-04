using System;
using System.Text;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;

internal static class RefillTaskDisplay
{
    public static string BuildDispensedMedMarkup(string patientName, DispensedRx dispensedRx)
    {
        var sb = new StringBuilder();
        sb.Append("<b>Pharmacy Renewal Request:  </b>");
        sb.Append("<BR>");

        if (!String.IsNullOrWhiteSpace(patientName))
        {
            sb.Append(patientName);
            sb.Append("<BR>");
        }

        sb.Append("<b>Medication Requested: </b>");
        sb.Append(dispensedRx?.Description.ToHTMLEncode());
        sb.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");


        sb.Append("<b>SIG:</b> ");
        sb.Append(dispensedRx?.SigText.ToHTMLEncode());

        sb.Append("<BR><b>Total number of dispenses requested:</b> ");
        sb.Append(dispensedRx?.Refills.ToHTMLEncode());
        sb.Append(", <b>Quantity:</b> ");
        sb.Append(dispensedRx?.Quantity.ToString().ToHTMLEncode());

        if (dispensedRx?.DaysSupply > 0)
        {
            sb.Append(", <b>Days Supply:</b> ");
            sb.Append(dispensedRx.DaysSupply);
        }

        if (dispensedRx?.Daw == Constants.CommonAbbreviations.YES || dispensedRx?.Daw == "1")
        {
            sb.Append(", <b>DAW</b>");
        }

        if (!String.IsNullOrEmpty(dispensedRx?.ProviderOfRecord))
        {
            sb.Append(", <b>Provider of record:</b> ");
            sb.Append(dispensedRx.ProviderOfRecord.ToHTMLEncode());
            sb.Append("&nbsp;&nbsp;&nbsp;");
        }

        if (!String.IsNullOrEmpty(dispensedRx?.Notes))
        {
            sb.Append("<BR><b>Notes: ");
            sb.Append(dispensedRx?.Notes.ToHTMLEncode());
        }
        sb.Append("</b>&nbsp;&nbsp;&nbsp;");


        if (!String.IsNullOrEmpty(dispensedRx?.PharmacyDetails))
        {
            sb.Append("<BR><b>Pharmacy: </b>");
            sb.Append(dispensedRx.PharmacyDetails.ToHTMLEncode());
        }
        sb.Append("&nbsp;&nbsp;&nbsp;");

        if (!String.IsNullOrEmpty(dispensedRx?.CreateDate))
        {
            sb.Append("<b>Original Rx Date:</b> ");
            sb.Append(dispensedRx.CreateDate.ToHTMLEncode());
            sb.Append("&nbsp;&nbsp;&nbsp;");
        }

        if (!String.IsNullOrEmpty(dispensedRx?.LastFillDate))
        {
            sb.Append("<b>Last Fill Date:</b> ");
            sb.Append(dispensedRx.LastFillDate.ToHTMLEncode());
            sb.Append("&nbsp;&nbsp;&nbsp;");
        }

        return sb.ToString();
    }

    public static string BuildOriginalPrescriptionMarkup(TaskRxDetailsModel prscriptionModel)
    {
        StringBuilder sbDispensedRxDetails = new StringBuilder();

        sbDispensedRxDetails.Append("<b><BR>Original Medication Prescribed: </b>");
        sbDispensedRxDetails.Append(prscriptionModel.DrugDescription.ToHTMLEncode());

        sbDispensedRxDetails.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>SIG: </b>");
        sbDispensedRxDetails.Append(prscriptionModel.SigText.ToHTMLEncode());

        //LINE 2 as per jerry
        sbDispensedRxDetails.Append("<BR><b>Total number of refills requested:</b> ");

        if (String.IsNullOrWhiteSpace(prscriptionModel.Refills.ToHTMLEncode()))
        {
            sbDispensedRxDetails.Append(CommonTerms.NotSpecified);
        }
        else
        {
            sbDispensedRxDetails.Append(prscriptionModel.Refills.ToHTMLEncode());
        }

        sbDispensedRxDetails.Append(", <b>Quantity:</b> ");
        sbDispensedRxDetails.Append(prscriptionModel.Quantity.ToHTMLEncode());

        if (prscriptionModel.Daw == Constants.CommonAbbreviations.YES ||
            prscriptionModel.Daw == "1")
        {
            sbDispensedRxDetails.Append(", <b>DAW</b>");
        }

        //LINE 4 as per Jerry
        if (!String.IsNullOrEmpty(prscriptionModel.RxNotes))
        {
            sbDispensedRxDetails.Append("<BR><b>Notes: ");
            sbDispensedRxDetails.Append(prscriptionModel.RxNotes.ToHTMLEncode());
            sbDispensedRxDetails.Append("</b>&nbsp;&nbsp;&nbsp;");
        }
        else
        {
            sbDispensedRxDetails.Append("&nbsp;&nbsp;&nbsp;");
        }

        //LINE 5 as per Jerry
        if (!String.IsNullOrEmpty(prscriptionModel.CreatedDate))
        {
            sbDispensedRxDetails.Append("<BR><b>Original Rx Date:</b> ");
            sbDispensedRxDetails.Append(prscriptionModel.CreatedDate.ToHTMLEncode());
            sbDispensedRxDetails.Append("&nbsp;&nbsp;&nbsp;");
        }

        if (!String.IsNullOrEmpty(prscriptionModel.LastFillDate))
        {
            sbDispensedRxDetails.Append("<b>Last Fill Date:</b> ");
            sbDispensedRxDetails.Append(prscriptionModel.LastFillDate.ToHTMLEncode());
            sbDispensedRxDetails.Append("&nbsp;&nbsp;&nbsp;");
        }

        return sbDispensedRxDetails.ToString();
    }
}