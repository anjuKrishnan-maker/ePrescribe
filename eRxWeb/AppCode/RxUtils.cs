/*****************************************************************************************************
**Change History
******************************************************************************************************
**  Date:         Author:                    Description:
**----------------------------------------------------------------------------------------------------
**05/28/2010    Sonal                   Mark a PBM reported med as �Received In Error� on clicking EIE 
******************************************************************************************************/

using System;
using System.Data;
using System.Configuration;
using Allscripts.Impact;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Common;
using System.Globalization;
using Allscripts.ePrescribe.Data.Model;
namespace eRxWeb
{
/// <summary>
/// Rx Utility methods, only static methods
/// </summary>
public class RxUtils
{
	private RxUtils()
	{
		//No instance, only static utils methods
	}

    public static string RenderRxDescription(string rxID, bool bPharmacy, string comments, string rxSource, string rxType, Constants.PrescriptionStatus rxStatus, string transMethod, string controlledSubstanceCode, string rxUpdatedDate, string transmissionStatus, int siteId, Guid licenseId, Constants.DrugHistoryType drugHistoryType, ConnectionStringPointer dbID)
	{
		string rxDescription = string.Empty;

		if (transMethod.ToUpper().Equals("D"))
		{
			rxDescription = "Dispensed";
		}
        else if (transMethod.ToUpper().Equals("PS") && rxStatus == Constants.PrescriptionStatus.NEW 
                    && !int.Equals(int.Parse(transmissionStatus), (int)Constants.TransmissionStatus.PHONED_IN))
        {
            rxDescription = "Printed and Sent";
        }
        else if (comments.ToLower().StartsWith("last filled on"))
		{
			rxDescription = comments; 
		}
		else if (
           rxStatus == Constants.PrescriptionStatus.PENDING_APPROVAL ||
           rxStatus == Constants.PrescriptionStatus.PENDING_TRANSMISSION)
		{
			rxDescription = "Pending";
		}
        else if (rxStatus == Constants.PrescriptionStatus.COMPLETE)
			rxDescription = "Complete";
        else if (rxStatus == Constants.PrescriptionStatus.ENTERED_IN_ERROR)
			rxDescription = "Entered In Error";
        else if (rxStatus == Constants.PrescriptionStatus.DISCONTINUED)
			rxDescription = "Discontinued";
        else if (rxStatus == Constants.PrescriptionStatus.REJECTED)
		{
			rxDescription = "Rejected";
		}
        else if (rxStatus == Constants.PrescriptionStatus.REJECTED_FROM_PENDING)
		{
			rxDescription = "Cancelled";
		}
        else if (rxStatus == Constants.PrescriptionStatus.RECIEVED_IN_ERROR)
        {
            rxDescription = "Recieved In Error";
        }
        else if (rxStatus == Constants.PrescriptionStatus.NEW)
                {
                var reviewHistoryModel = new ReviewHistoryDataModel()
                {
                    RxID = Guid.Parse(rxID),
                    Pharmacy = Convert.ToString(bPharmacy),
                    Comments= comments,
                    RxSource= rxSource,
                    Type= rxType,
                    TransmissionMethod = transMethod,
                    ControlledSubstanceCode=controlledSubstanceCode,
                    Updated= Convert.ToDateTime(rxUpdatedDate),
                    TransmissionStatus = transmissionStatus,                   
                    DrugHistoryType = drugHistoryType
                };
                rxDescription = GetNewPrescriptionStatusDescription(reviewHistoryModel, siteId, licenseId, dbID);
        }

		return rxDescription;
	}

    internal static string GetNewPrescriptionStatusDescription(ReviewHistoryDataModel reviewModel, int siteId, Guid licenseId, ConnectionStringPointer dbID)
        {
            string rxDescription="";
            int nControlledSubstanceCode;
            bool bPharmacy = Convert.ToBoolean(reviewModel.Pharmacy);
            if (reviewModel.Type == Constants.PrescriptionType.REPORTED)
            {
                rxDescription = "Patient Self Reported";
            }
            else if (reviewModel.DrugHistoryType == Constants.DrugHistoryType.P2)
            {
                rxDescription = "Pharmacy Reported Rx History";

            }
            else if (reviewModel.DrugHistoryType == Constants.DrugHistoryType.PY)
            {
                rxDescription = "PBM Reported Rx History";
            }
            else if (!bPharmacy && reviewModel.Comments.ToLower().Contains("rx history"))
            {
                rxDescription = "PBM Reported Rx History";
            }
            else if (!bPharmacy &&
                !string.IsNullOrEmpty(reviewModel.ControlledSubstanceCode) &&
                int.TryParse(reviewModel.ControlledSubstanceCode, out nControlledSubstanceCode) &&
                nControlledSubstanceCode > 0 && nControlledSubstanceCode < 6)
            {
                rxDescription = "Printed";
            }
            else if (reviewModel.RxSource == Constants.ERX_NOW_RX)
            {
                if (bPharmacy)
                {
                    if (reviewModel.Comments.ToLower() == "reprinted rx")
                    {
                        rxDescription = "Reprinted on " + Convert.ToString(reviewModel.Updated);
                    }
                    else if (reviewModel.TransmissionMethod.ToUpper().Equals("S") && !int.Equals(int.Parse(reviewModel.TransmissionStatus), (int)Constants.TransmissionStatus.PHONED_IN))
                    {
                        rxDescription = "Sent to Pharmacy";
                    }
                    else if (!string.IsNullOrWhiteSpace(reviewModel.TransmissionStatus) && int.Equals(int.Parse(reviewModel.TransmissionStatus), (int)Constants.TransmissionStatus.PHONED_IN))
                    {
                        // rxDescription = "Phoned in";
                        var errorConfirmInfo = RxError.GetErrorConfirmInfo(reviewModel.RxID.ToString(), siteId, licenseId, dbID);
                        if (errorConfirmInfo != null && !string.IsNullOrEmpty(errorConfirmInfo.Name))
                        {
                            rxDescription = "Error / Confirmed by " + errorConfirmInfo.Name;
                        }
                        else
                        {
                            rxDescription = "Error / Confirmed";
                        }
                    }
                    else if (string.Equals(reviewModel.TransmissionMethod, "P", StringComparison.CurrentCultureIgnoreCase) && !int.Equals(int.Parse(reviewModel.TransmissionStatus), (int)Constants.TransmissionStatus.PHONED_IN))
                    {
                        rxDescription = "Printed";
                    }
                }
                else
                {
                    rxDescription = "Printed";
                }
            }
            else if (reviewModel.RxSource == Constants.EXTERNAL_RX)
            {
                switch (reviewModel.TransmissionMethod.ToUpper())
                {
                    case Constants.PrescriptionTransmissionMethod.PRINTED:
                        rxDescription = "Printed";
                        break;
                    case Constants.PrescriptionTransmissionMethod.SENT:
                        rxDescription = "Sent to Pharmacy";
                        break;
                    case Constants.PrescriptionTransmissionMethod.RECORDED:
                    case Constants.PrescriptionTransmissionMethod.HISTORY:
                        rxDescription = "Patient Self Reported";
                        break;
                }
            }
            return rxDescription;
    }
    public static string GetPrescritpionStatusDescription(Constants.PrescriptionStatus? status)
    {
        string statusDescription = string.Empty;
        if (status != null)
        {
            switch (status)
            {
                case Constants.PrescriptionStatus.NEW:
                    statusDescription = "New";
                    break;
                case Constants.PrescriptionStatus.PENDING_APPROVAL:
                case Constants.PrescriptionStatus.PENDING_TRANSMISSION:
                    statusDescription = "Pending";
                    break;
                case Constants.PrescriptionStatus.DISCONTINUED:
                    statusDescription = "Discontinued";
                    break;
                case Constants.PrescriptionStatus.COMPLETE:
                    statusDescription = "Complete";
                    break;
                case Constants.PrescriptionStatus.REJECTED:
                    statusDescription = "Rejected";
                    break;
            }
        }

        return statusDescription;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="name"></param>
    /// <returns>returns decimal</returns>
    public static decimal GetDecimal(string value, string name)
    {
        decimal number;
        CultureInfo culture = null;
        // Return if string is empty
        if (String.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(String.Format("'{0}' has invalid value : '{1}'", name, value));

        // Determine if value can be parsed using current culture.
        try
        {
            culture = CultureInfo.CurrentCulture;
            number = decimal.Parse(value, culture);
            return number;
        }
        catch { }
        // If Parse operation fails, see if there's a neutral culture.
        try
        {
            culture = culture.Parent;
            number = decimal.Parse(value, culture);
            return number;
        }
        catch { }
        // If there is no neutral culture or if parse operation fails, use
        // the invariant culture.
        culture = CultureInfo.InvariantCulture;
        try
        {
            number = decimal.Parse(value, culture);
            return number;
        }
        // All attempts to parse the string have failed; rethrow the exception.
        catch (FormatException e)
        {
            throw new FormatException(String.Format("Unable to parse '{0}' with value : '{1}'.", name, value), e);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="name"></param>
    /// <returns>returns 32-bit value</returns>
    public static int GetInteger(string value, string name)
    {
        int number;
        CultureInfo culture = null;
        // Return if string is empty
        if (String.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(String.Format("'{0}' has invalid value : '{1}'", name, value));

        // Determine if value can be parsed using current culture.
        try
        {
            culture = CultureInfo.CurrentCulture;
            number = int.Parse(value, culture);
            return number;
        }
        catch { }
        // If Parse operation fails, see if there's a neutral culture.
        try
        {
            culture = culture.Parent;
            number = int.Parse(value, culture);
            return number;
        }
        catch { }
        // If there is no neutral culture or if parse operation fails, use
        // the invariant culture.
        culture = CultureInfo.InvariantCulture;
        try
        {
            number = int.Parse(value, culture);
            return number;
        }
        // All attempts to parse the string have failed; rethrow the exception.
        catch (FormatException e)
        {
            throw new FormatException(String.Format("Unable to parse '{0}' with value : '{1}'.", name, value), e);
        }
    }

}

}