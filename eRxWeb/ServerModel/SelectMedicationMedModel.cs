using Allscripts.ePrescribe.Data.Model;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.AppCode;
using eRxWeb.ServerModel.Common;
using eRxWeb.State;
using System;
using System.Collections;
using System.Data;

namespace eRxWeb.ServerModel
{
    public enum MedNameDisplayOption
    {
        DisplayLinkWithAdd = 1,
        DisplayLabel = 5,
        DisplayLinkWithLibraryLink = 10
    }
    public class SelectMedicationMedModel
    {
        public int index { get; set; }
        public string DDI { get; set; }
        public string DrugName { get; set; }
        public string DrugExt { get; set; }
        public string Quantity { get; set; }
        public bool DAW { get; set; }
        public int Refill { get; set; }
        public int DayOfSupply { get; set; }
        public string BenefitImageUrl { get; set; }
        public string LevelOfPreferedness { get; set; }
        public int FormularyImageUrl { get; set; }
        public int FormularyStatus { get; set; }
        public bool Selected { get; set; }
        public ImageModel BenefitImage { get; set; }
        public ImageModel FormularyImage { get; set; }
        public ImageModel DURImage { get; set; }
        public bool IsGeneric { get; set; }
        public MedNameDisplayOption DrugNameDisplayOption { get; set; }
        public string LibraryUrl { get; set; }
        public string Strength { get; set; }
        public string StrengthUOM { get; set; }
        public string DosageForm { get; set; }
        public string RouteofAdmin { get; set; }
        public string RxDate { get; set; }
        public string RxStatus { get; set; }
        public string RxStatusInfo { get; set; }
        public string FormStatusCode { get; set; }
        public bool IsOTC { get; set; }
        public string SIGID { get; set; }
        public string ControlledSubstanceCode { get; set; }
        public string GPPC { get; set; }
        public string PackageSize { get; set; }
        public string PackageQuantity { get; set; }
        public string PackageUOM { get; set; }
        public string PackageDescription { get; set; }
        public string MedicationName { get; set; }
        public string RouteofAdminCode { get; set; }
        public string DosageFormCode { get; set; }
        public string SIGText { get; set; }
        public int SIGTypeID { get; set; }
        public bool PriorAuth { get; set; }
        public string NDC { get; set; }
        public bool IsCouponAvailable { get; set; }
        public bool IsSpecialtyMed { get; set; }
        public string GPI { get; set; }
        public string PreDURDrugDrug { get; set; }
        public string PreDURDUP { get; set; }
        public string PreDURPAR { get; set; }

        public static SelectMedicationMedModel CreateFromDataRow(FullScriptHxMedDataModel selectMedicationHxMedDataModel, ref Hashtable quickDURs, IStateContainer session)
        {
            var basePage = new BasePage();
            basePage.PageState = session;

            string preDurDrugDrug, preDurPar, preDurDup;
            preDurDrugDrug = preDurPar = preDurDup = "N";

            if (string.IsNullOrEmpty(selectMedicationHxMedDataModel.Quantity))
                selectMedicationHxMedDataModel.Quantity = "0";

            var durImage = MedicationSearchDisplay.GetDURImage(selectMedicationHxMedDataModel.MedicationName,
                                                                selectMedicationHxMedDataModel.DDI,
                                                                selectMedicationHxMedDataModel.StrengthUOM,
                                                                selectMedicationHxMedDataModel.Strength,
                                                                selectMedicationHxMedDataModel.DosageFormCode,
                                                                selectMedicationHxMedDataModel.Quantity,
                                                                selectMedicationHxMedDataModel.DayOfSupply,
                                                                selectMedicationHxMedDataModel.Refill,
                                                                selectMedicationHxMedDataModel.RxId,
                                                                out preDurDrugDrug,
                                                                out preDurPar,
                                                                out preDurDup,
                                                                ref quickDURs,
                                                                session, "Others");
            var data = new SelectMedicationMedModel()
            {
                DDI = selectMedicationHxMedDataModel.DDI,
                DrugName = selectMedicationHxMedDataModel.MedicationFront,
                DrugExt = selectMedicationHxMedDataModel.MedicationBack,
                Quantity = selectMedicationHxMedDataModel.Quantity,
                DayOfSupply = selectMedicationHxMedDataModel.DayOfSupply,
                Refill = selectMedicationHxMedDataModel.Refill,
                BenefitImage = MedicationSearchDisplay.GetBenefitsImageUrl(selectMedicationHxMedDataModel.BenefitInfoAvailable,
                                                                        selectMedicationHxMedDataModel.IsCouponAvailable,
                                                                        selectMedicationHxMedDataModel.IsSpecialtyMed,
                                                                        basePage),
                FormularyImage = MedicationSearchDisplay.GetFormularyImage(selectMedicationHxMedDataModel.FormularyStatus,
                                                                        selectMedicationHxMedDataModel.IsOTC, basePage),
                DURImage = durImage,
                PreDURDrugDrug = preDurDrugDrug,
                PreDURDUP = preDurDup,
                PreDURPAR = preDurPar,
                FormularyStatus = selectMedicationHxMedDataModel.FormularyStatus,
                IsGeneric = selectMedicationHxMedDataModel.IsGeneric,
                DrugNameDisplayOption = MedicationSearchDisplay.GetDrugNameDisplayOption(basePage),
                LibraryUrl = MedicationSearchDisplay.GetLibraryUrl(selectMedicationHxMedDataModel.NDC, basePage),
                RxStatus = MedicationSearchDisplay.GetPrescriptionStatus(selectMedicationHxMedDataModel.RxStatus),
                RxStatusInfo = MedicationSearchDisplay.GetPrescriptionStatusInfo(selectMedicationHxMedDataModel.RxStatus,
                                                                                selectMedicationHxMedDataModel.RxCompletionDate,
                                                                                selectMedicationHxMedDataModel.RxDiscontinueDate),
                RxDate = Allscripts.Impact.Utilities.StringHelper.ConvertToUxDate(selectMedicationHxMedDataModel.RxDate),
                FormStatusCode = selectMedicationHxMedDataModel.FormStatusCode,
                IsOTC = selectMedicationHxMedDataModel.IsOTC,
                SIGID = selectMedicationHxMedDataModel.SigId,
                ControlledSubstanceCode = selectMedicationHxMedDataModel.ControlledSubstanceCode,
                GPPC = selectMedicationHxMedDataModel.GPPC,
                Strength = selectMedicationHxMedDataModel.Strength,
                StrengthUOM = selectMedicationHxMedDataModel.StrengthUOM,
                PackageSize = selectMedicationHxMedDataModel.PackageSize,
                PackageQuantity = selectMedicationHxMedDataModel.PackageQuantity,
                PackageUOM = selectMedicationHxMedDataModel.PackageUOM,
                PackageDescription = selectMedicationHxMedDataModel.PackageDescription,
                MedicationName = selectMedicationHxMedDataModel.MedicationName,
                RouteofAdminCode = selectMedicationHxMedDataModel.RouteOfAdminCode,
                RouteofAdmin = selectMedicationHxMedDataModel.RouteOfAdmin,
                DosageForm = selectMedicationHxMedDataModel.DosageForm,
                DosageFormCode = selectMedicationHxMedDataModel.DosageFormCode,
                SIGText = selectMedicationHxMedDataModel.SigText,
                SIGTypeID = selectMedicationHxMedDataModel.SigTypeID,
                PriorAuth = selectMedicationHxMedDataModel.PriorAuth,
                NDC = selectMedicationHxMedDataModel.NDC,
                IsCouponAvailable = selectMedicationHxMedDataModel.IsCouponAvailable,
                IsSpecialtyMed = selectMedicationHxMedDataModel.IsSpecialtyMed,
                GPI = selectMedicationHxMedDataModel.GPI,
                DAW = false

            };
            return data;
        }

        public static SelectMedicationMedModel CreateFromDataAllMedRow(FullScriptMedDataModel selectMedicationMedDataModel, ref Hashtable quickDURs, IStateContainer session)
        {
            var basePage = new BasePage();
            basePage.PageState = session;


            string preDurDrugDrug, preDurPar, preDurDup;
            preDurDrugDrug = preDurPar = preDurDup = "N";
            var durImage = MedicationSearchDisplay.GetDURImage(selectMedicationMedDataModel.DisplayName,
                                                                selectMedicationMedDataModel.DDI,
                                                                selectMedicationMedDataModel.StrengthUOM,
                                                                selectMedicationMedDataModel.Strength,
                                                                selectMedicationMedDataModel.DosageFormCode,
                                                                "0", 0, 0, string.Empty, out preDurDrugDrug,
                                                                out preDurPar, out preDurDup, ref quickDURs, session, "AllMeds");

            var data = new SelectMedicationMedModel()
            {
                DDI = selectMedicationMedDataModel.DDI,
                DrugName = selectMedicationMedDataModel.DisplayName,
                BenefitImage = MedicationSearchDisplay.GetBenefitsImageUrl(selectMedicationMedDataModel.BenefitInfoAvailable,
                                                                        selectMedicationMedDataModel.IsCouponAvailable,
                                                                        selectMedicationMedDataModel.IsSpecialtyMed,
                                                                        basePage),
                FormularyImage = MedicationSearchDisplay.GetFormularyImage(selectMedicationMedDataModel.FormularyStatus,
                                                                        selectMedicationMedDataModel.IsOTC, basePage),
                DURImage = durImage,
                PreDURDrugDrug = preDurDrugDrug,
                PreDURDUP = preDurDup,
                PreDURPAR = preDurPar,
                IsGeneric = selectMedicationMedDataModel.IsGeneric,
                DrugNameDisplayOption = MedicationSearchDisplay.GetDrugNameDisplayOption(basePage),
                LibraryUrl = MedicationSearchDisplay.GetLibraryUrl(selectMedicationMedDataModel.NDC, basePage),
                Strength = selectMedicationMedDataModel.Strength,
                StrengthUOM = selectMedicationMedDataModel.StrengthUOM,
                DosageForm = selectMedicationMedDataModel.DosageForm,
                RouteofAdmin = selectMedicationMedDataModel.RouteofAdmin,
                ControlledSubstanceCode = selectMedicationMedDataModel.ControlledSubstanceCode,
                RouteofAdminCode = selectMedicationMedDataModel.RouteofAdminCode,
                DosageFormCode = selectMedicationMedDataModel.DosageFormCode,
                FormularyStatus = selectMedicationMedDataModel.FormularyStatus,
                IsCouponAvailable = selectMedicationMedDataModel.IsCouponAvailable,
                PriorAuth = selectMedicationMedDataModel.PriorAuth,
                NDC = selectMedicationMedDataModel.NDC,
                IsSpecialtyMed = selectMedicationMedDataModel.IsSpecialtyMed,
                GPI = selectMedicationMedDataModel.GPI,
                MedicationName = selectMedicationMedDataModel.Name
                // why not including SIG data?
            };
            return data;
        }

    }
}