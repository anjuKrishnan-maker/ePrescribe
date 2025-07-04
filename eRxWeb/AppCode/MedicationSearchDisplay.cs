using Allscripts.ePrescribe.Common;
using eRxWeb.ServerModel;
using eRxWeb.ServerModel.Common;
using System.Data;
using System.Web.UI.WebControls;
using System;
using Allscripts.Impact;
using System.Collections.Generic;
using eRxWeb.State;
using System.Linq;
using eRxWeb.AppCode.StateUtils;
using System.Collections;
using Constants = Allscripts.ePrescribe.Common.Constants;
using Allergy = Allscripts.ePrescribe.Medispan.Clinical.Model.Allergy;

using Allscripts.ePrescribe.Medispan.Clinical.Model;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Request;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Settings;
using Allscripts.ePrescribe.ExtensionMethods;
using MedicationInfo = eRxWeb.AppCode.StateUtils.MedicationInfo;

namespace eRxWeb.AppCode
{
    public class MedicationSearchDisplay
    {
        public const string SearchTextBoxDefault = "Search Medication";
      
        public static void SetBenefitsImage(ref Image benefitImg, bool benefitInfoAvailable, bool couponAvailable, bool isSpecialtyMed, bool canApplyFinancialOffers)
        {
            if (benefitImg != null)
            {
                if (benefitInfoAvailable || couponAvailable || isSpecialtyMed)
                {
                    if (canApplyFinancialOffers)
                    {
                        if (couponAvailable)
                        {
                            benefitImg.Style["cursor"] = "pointer";
                            benefitImg.ImageUrl = "images/dollar.png";
                            benefitImg.ToolTip = "Patient is Eligible for Financial Savings.";
                        }
                    }

                    if (isSpecialtyMed)
                    {
                        SetBenefitImgAsSpecialtyMed(ref benefitImg);
                    }
                    else if (benefitInfoAvailable)
                    {
                        benefitImg.Style["cursor"] = "pointer";
                        benefitImg.ImageUrl = "images/dollar.png";

                        if (benefitImg.ToolTip.Length == 0)
                        {
                            benefitImg.ToolTip = "Benefit information available. Click to update right panel.";
                        }
                        else
                        {
                            benefitImg.ToolTip = benefitImg.ToolTip + " Benefit information available. Click to update right panel.";
                        }
                    }
                }
            }
        }

        public static void SetBenefitImgAsSpecialtyMed(ref Image benefitImg)
        {
            benefitImg.Style["cursor"] = "pointer";
            benefitImg.ImageUrl = "images/dollar.png";
            benefitImg.ToolTip = "Prior Authorization and Patient Access Services available.  " + benefitImg.ToolTip;
        }

        public static ImageModel GetDURImage(string medName, string ddi, string 
                            strengthUom, string strength, string dosageFormCode,
                            string qty ,int daysSupply,int refills,string rxId,
                            out string preDurDrugDrug, out string preDurPar, out string preDurDup, 
                            ref Hashtable quickDURs, IStateContainer session, string forSource)
        {
            ImageModel model = new ImageModel();

            DateTime patientDobDateTime;
            DateTime.TryParse(session.GetStringOrEmpty("PATIENTDOB"), out patientDobDateTime);

            List<int> activeMedsDdIs = DURMedispanUtils.RetrieveActiveMedsList(session.Cast(Constants.SessionVariables.ACTIVEMEDDDILIST, new List<string>()));
            List<int> scriptPadMedDdis = DURMedispanUtils.RetrieveScriptPadMedsList(MedicationInfo.GetScriptPadMeds(session));
            var existingMedsList = activeMedsDdIs.Concat(scriptPadMedDdis ?? new List<int>()).ToList();

            List<Allergy> durPatientAllergies = session.Cast(Constants.SessionVariables.DurPatientAllergies, new List<Allergy>());
            DURSettings durSettings = DurInfo.GetDURSettings(session);
            DURCheckResponse durCheckResponse = null;
            preDurDrugDrug = preDurPar = preDurDup = "N";

            switch (forSource)
            {
                case "Others":
                    if (quickDURs.Contains(ddi))
                    {
                        durCheckResponse = (quickDURs[ddi] as DURCheckResponse);
                    }
                    else
                    {
                        durCheckResponse = DURMSC.DURQuickCheck
                        (
                            new DURPatient()
                            {
                                PatientDOB = patientDobDateTime,
                                PatientGender =
                                    DURMedispanUtils.RetrieveDurPatientGender(
                                        session.GetStringOrEmpty(Constants.SessionVariables.SEX))
                            },
                            new DrugToCheck()
                            {
                                DDI = Convert.ToInt32(ddi),
                                StrengthUOM = strengthUom,
                                DosageFormCode = dosageFormCode,
                                Quantity = Convert.ToDouble(qty),
                                DaysSupply = daysSupply,
                                Refills = refills,
                                ExternalId = rxId,
                                MedicationName = medName,
                                Strength = strength,
                                DoseUnit = DURMedispanUtils.GetDoseUnitFromDoseFormCode(dosageFormCode)
                            },
                            existingMedsList,
                            durPatientAllergies,
                            durSettings,
                            InteractionsManagementLevel.PotentialInteractionRisk
                        );
                        quickDURs.Add(ddi, durCheckResponse);
                    }
                    break;
                case "AllMeds":
                    if (quickDURs.Contains(ddi))
                    {
                        durCheckResponse = (quickDURs[ddi] as DURCheckResponse);
                    }
                    else
                    {
                        durCheckResponse = DURMSC.DURQuickCheck
                        (
                            new DURPatient()
                            {
                                PatientDOB = patientDobDateTime,
                                PatientGender =
                                    DURMedispanUtils.RetrieveDurPatientGender(
                                        session.GetStringOrEmpty(Constants.SessionVariables.SEX))
                            },
                            new DrugToCheck()
                            {
                                DDI = Convert.ToInt32(ddi),
                                StrengthUOM = strengthUom,
                                DosageFormCode = dosageFormCode,
                                MedicationName = medName,
                                Strength = strength,
                                DoseUnit = DURMedispanUtils.GetDoseUnitFromDoseFormCode(dosageFormCode)
                            },
                            existingMedsList,
                            durPatientAllergies,
                            durSettings,
                            InteractionsManagementLevel.PotentialInteractionRisk
                        );
                        quickDURs.Add(ddi, durCheckResponse);
                    }
                    break;
            }
            if (durCheckResponse != null)
            {
                model.ImageUrl = "images/warning-24x24.png";
               
                if (durCheckResponse.DrugInteractions.Interactions.HasItems())
                {
                    preDurDrugDrug = "Y";
                    model.ToolTip += "This medication will interact with " +
                                DURMedispanUtils.RetrievePreemptiveInteractionsList(durCheckResponse.DrugInteractions.Interactions, ddi) +
                                ".  ";
                }
                if (durCheckResponse.DuplicateTherapy.Results.HasItems())
                {
                    preDurDup = "Y";
                    model.ToolTip += "This medication will result in duplicate therapy.";
                }

                if (durCheckResponse.PriorAdverseReactions.Reactions.HasItems())
                {
                    preDurPar = "Y";

                    string parsymptoms = string.Empty;
                    foreach (PriorAdverseReactionCategory reaction in durCheckResponse.PriorAdverseReactions.Reactions)
                    {
                        parsymptoms = parsymptoms + reaction.WarningText + ". ";
                    }
                    if (parsymptoms.Trim().Length == 0)
                    {
                        model.ToolTip +=
                            "This patient has reported an unknown adverse reaction while using this medication.  ";
                    }
                    else
                    {
                        model.ToolTip += parsymptoms;
                    }
                }
                if (durCheckResponse.PriorAdverseReactions.LookupWarnings.HasItems())
                {
                    durCheckResponse.PriorAdverseReactions.LookupWarnings.ForEach(warning =>
                    {
                        model.ToolTip += warning;

                    });
                }
            }
            return model;
        }

        public static ImageModel GetBenefitsImageUrl(bool benefitInfoAvailable, bool couponAvailable, bool isSpecialtyMed, Interfaces.IBasePage basePage)
        {
            ImageModel model = new ImageModel();
            bool canApplyFinancialOffers = basePage.CanApplyFinancialOffers;

            if (benefitInfoAvailable || couponAvailable || isSpecialtyMed)
            {
                if (canApplyFinancialOffers)
                {
                    if (couponAvailable)
                    {
                        model.Style["cursor"] = "pointer";
                        model.ImageUrl = "images/dollar.png";
                        model.ToolTip = "Patient is Eligible for Financial Savings.";
                    }
                }

                if (isSpecialtyMed)
                {
                    model.Style["cursor"] = "pointer";
                    model.ImageUrl = "images/dollar.png";
                    model.ToolTip = "Prior Authorization and Patient Access Services available.  " + model.ToolTip;

                }
                else if (benefitInfoAvailable)
                {
                    model.Style["cursor"] = "pointer";
                    model.ImageUrl = "images/dollar.png";

                    if (model.ToolTip != null)
                    {
                        model.ToolTip = "Benefit information available. Click to update right panel.";
                    }
                    else
                    {
                        model.ToolTip = model.ToolTip + " Benefit information available. Click to update right panel.";
                    }
                }
            }
            return model;

        }

        public static ImageModel GetFormularyImage(int ahsFormularyStatus, bool isOTC, Interfaces.IBasePage basePage)
        {
            var img = new ImageModel();

            img.Style["cursor"] = "pointer";
            string imageUrl = string.Empty;
            string toolTip = string.Empty;

            GetFormularyImagePathWithTooltip(ahsFormularyStatus, isOTC, out imageUrl, out toolTip);

            img.ImageUrl = imageUrl;
            img.ToolTip = toolTip;

            if (basePage.CheckFormAltsAndCopay)
                img.ToolTip = img.ToolTip + ". Click to update right panel formulary information.";

            return img;
        }

        public static void GetFormularyImagePathWithTooltip(int formularyStatus, bool isOTC, out string imageUrl, out string toolTip)
        {
            imageUrl = string.Empty;
            toolTip = string.Empty;

            switch (formularyStatus)
            {
                case 0: //                                  NONE = 0,
                    imageUrl = "images/fs_blank.gif";
                    toolTip = string.Empty;
                    break;
                case 1: //display a Smile bitmap ,          ON =1 Preferred
                    imageUrl = isOTC ? "images/fs_smiley_OTC.gif" : "images/smile.gif";
                    toolTip = "Preferred status";
                    break;
                case 2: //display a Neutral bitmap ,        ACCEPTED=2
                    imageUrl = isOTC ? "images/fs_yellow_OTC.gif" : "images/fs_yellow.gif";
                    toolTip = "Approved status";
                    break;
                case 3: //display a Frown bitmap  ,   
                    imageUrl = isOTC ? "images/fs_red_OTC.gif" : "images/fs_red.gif";
                    toolTip = "Non-approved status";
                    break;
                case 4: //display a Smile bitmap
                    imageUrl = isOTC ? "images/fs_smiley_OTC.gif" : "images/smile.gif";
                    //   PRIOR_AUTHORIZATION, =4
                    toolTip = "Prior authorization is required";
                    break;
                case 5: //                                  OTC =5
                    imageUrl = "images/fs_otc.gif";
                    toolTip = "Over-the-counter drug not covered by insurance";
                    break;
                case 6: //non-reimbursable, NR = 6
                    imageUrl = isOTC ? "images/fs_red_NR_OTC.gif" : "images/fs_red_NR.gif";
                    toolTip = isOTC ? "Non-reimbursable OTC" : "Non-reimbursable";
                    break;
                case 7: //unknown
                    imageUrl = "images/question.png";
                    toolTip = "Unknown status";
                    break;
            }
        }

        public static MedNameDisplayOption GetDrugNameDisplayOption(Interfaces.IBasePage basePage)
        {
            MedNameDisplayOption option = MedNameDisplayOption.DisplayLabel;
            switch (basePage.SessionLicense.GetFeatureStatus(Constants.DeluxeFeatureType.iFC))
            {
                case Constants.DeluxeFeatureStatus.On:                
                    option = MedNameDisplayOption.DisplayLinkWithLibraryLink;
                    break;
                case Constants.DeluxeFeatureStatus.Off:
                case Constants.DeluxeFeatureStatus.Disabled:
                    option = MedNameDisplayOption.DisplayLinkWithAdd;
                    break;
                default:
                    option = MedNameDisplayOption.DisplayLabel;
                    break;
            }
            return option;
        }
        public static string GetLibraryUrl(string NDC, Interfaces.IBasePage basePage)
        {
            string url = string.Empty;
            if (basePage.IsLexicompEnabled)
            {
                url = string.Format(Allscripts.Impact.ConfigKeys.LexicompMedicationSearchURL, NDC);
            }
            else
            {
                url = string.Format(Allscripts.Impact.ConfigKeys.FactsComparisonsMedicationSearchURL, NDC);
            }
            return url;

        }

        public static string GetPrescriptionStatus(string rxStatus)
        {
            Constants.PrescriptionStatus status;
            //Fetch all four fields and create a string here.
            string prescriptionStatus = string.Empty;
            if (Enum.TryParse(rxStatus, out status))
            {
                prescriptionStatus = RxUtils.GetPrescritpionStatusDescription(status);
            }
            return prescriptionStatus;
        }
        public static string GetPrescriptionStatusInfo(string rxStatus, string completedDate, string disContinueDate)
        {
            Constants.PrescriptionStatus status;

            //Fetch all four fields and create a string here.
            string statusInfo = string.Empty;
            DateTime statusDate = DateTime.MinValue;

            if (Enum.TryParse(rxStatus, out status))
            {
                if (status == Constants.PrescriptionStatus.COMPLETE)
                {
                    if (DateTime.TryParse(completedDate,
                        out statusDate))
                    {
                        statusInfo += $"Completed On: {statusDate.Date.ToShortDateString()}";
                    }
                    else
                    {
                        statusInfo = "Completed On: not available";
                    }
                }
                else if (status == Constants.PrescriptionStatus.DISCONTINUED)
                {
                    if (DateTime.TryParse(disContinueDate, out statusDate))
                    {
                        statusInfo = statusDate.Date.ToShortDateString();
                    }
                    else
                    {
                        statusInfo = "Discontinued On: not available";
                    }
                }
            }
            return statusInfo;  

        }
    }
}