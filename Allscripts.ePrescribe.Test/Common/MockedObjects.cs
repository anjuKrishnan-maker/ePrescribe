using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Settings;
using eRxWeb.State;
using Rhino.Mocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.Http.Routing;
using System.Web.Routing;
using System.Web.SessionState;
using Model= Allscripts.ePrescribe.Medispan.Clinical.Model;
using eRxWeb.AppCode.DurBPL.ResponseModel;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using static Allscripts.ePrescribe.Common.Constants;
using Allscripts.Impact;
using Allscripts.ePrescribe.Objects;
using Rx = Allscripts.Impact.Rx;
using DURSettings = Allscripts.ePrescribe.Objects.DURSettings;
using Allscripts.ePrescribe.Medispan.Clinical.Model;
using System.Data;
using System.Data.SqlClient;
using Allscripts.ePrescribe.DataHelpers;
//using DURSettings = Allscripts.ePrescribe.Medispan.Clinical.Model.Settings.DURSettings;
using Allergy = Allscripts.ePrescribe.Medispan.Clinical.Model.Allergy;
using AllergyType = Allscripts.ePrescribe.Medispan.Clinical.Model.AllergyType;
using StateLicense = eRxWeb.ServerModel.StateLicense;

namespace Allscripts.ePrescribe.Test.Common
{
    public class MockedObjects
    {
        public const string xmlScriptMessage = "<AllscriptsMessageEnvelope><MessageContent><PrescriptionMessage><Patient><FirstName>TestFirstName</FirstName><LastName>TestLastName</LastName></Patient></PrescriptionMessage></MessageContent></AllscriptsMessageEnvelope>";
        public const string URL = "http://localhost/api/RxDurReviewApi/CSMedRefillRequestNotAllowedOnPrintRefill";
        public IStateContainer _pageState;

        
        public static HttpContext MockHttpContext()
        {
            var httpRequest = new HttpRequest("", URL, "");
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponse);

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                                                    new HttpStaticObjectsCollection(), 10, true,
                                                    HttpCookieMode.AutoDetect,
                                                    SessionStateMode.InProc, false);


            httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                                        BindingFlags.NonPublic | BindingFlags.Instance,
                                        null, CallingConventions.Standard,
                                        new[] { typeof(HttpSessionStateContainer) },
                                        null)
                                .Invoke(new object[] { sessionContainer });

            
            return httpContext;
        }
        public HttpContextBase GetMockedHttpContext()
        {
            var context = MockRepository.GenerateMock<HttpContextBase>();
            var request = MockRepository.GenerateMock<HttpRequestBase>();
            var response = MockRepository.GenerateMock<HttpResponseBase>();
            var session = MockRepository.GenerateMock<HttpSessionStateBase>();
            var server = MockRepository.GenerateMock<HttpServerUtilityBase>();
            var user = MockRepository.GenerateMock<IPrincipal>();
            var identity = MockRepository.GenerateMock<IIdentity>();
            var urlHelper = MockRepository.GenerateMock<UrlHelper>();

            RequestContext requestContext = new RequestContext();

            requestContext.Stub(x => x.HttpContext).Return(context);
            context.Stub(ctx => ctx.Request).Return(request);
            context.Stub(ctx => ctx.Response).Return(response);
            context.Stub(ctx => ctx.Session).Return(session);
            context.Stub(ctx => ctx.Server).Return(server);
            context.Stub(ctx => ctx.User).Return(user);
            user.Stub(ctx => ctx.Identity).Return(identity);
            identity.Stub(id => id.IsAuthenticated).Return(true);
            identity.Stub(id => id.Name).Return("test");
            request.Stub(req => req.Url).Return(new Uri("http://localhost/api/RxDurReviewApi/CSMedRefillRequestNotAllowedOnPrintRefill"));
            request.Stub(req => req.RequestContext).Return(requestContext);
            requestContext.Stub(x => x.RouteData).Return(new RouteData());
            request.Stub(req => req.Headers).Return(new NameValueCollection());

            return context;
        }
        
        public ArrayList SetupRxToProviderList()
        {
            var rxToProviderList = new ArrayList();
            rxToProviderList.Add(Guid.Parse("56124999-4B5B-4300-8D5A-33B35CC906A0"));
            return rxToProviderList;
        }

        public List<Guid> SetupRxIDList()
        {
            var guidList = new List<Guid>();
            guidList.Add(Guid.Parse("9D6BDD82-B32E-4DAD-A592-200E2DE4C933"));
            guidList.Add(Guid.Parse("43D191D1-6F71-432C-893D-2B7C699B3780"));
            return guidList;
        }

        public Guid GetGuid()
        {
            return Guid.NewGuid();
        }
        public string GetGuidInString()
        {
            return Guid.NewGuid().ToString();
        }
        public Guid SetUpTaskScriptMessageId()
        {
            return Guid.NewGuid();
        }
        public Guid SetUpUserId()
        {
            return Guid.NewGuid();
        }
        public Guid SetLicienceId()
        {
            return Guid.NewGuid();
        }

        public ConnectionStringPointer SetUpDBId()
        {
            return ConnectionStringPointer.AUDIT_ERXDB_SERVER_1;
        }
        public RxDurResponseModel SetupRxList()
        {
            RxDurResponseModel rxDurResponseModel = new RxDurResponseModel();
            var rxList = new List<Rx>();
            var rx = new Impact.Rx();
            rx.DDI = "184865";
            rx.PPTxTransactionHeaderID = "3b09faa2-0701-4b82-a75b-7d271154fdb9";
            rx.NDCNumber = "51672211704";
            rxList.Add(rx);
            rxDurResponseModel.RxList = rxList;
            return rxDurResponseModel;
        }
        public DURCheckResponse SetupDurWarnings()
        {
            DURCheckResponse durResponse = new DURCheckResponse();
            durResponse.DrugInteractions = new DrugToDrugInteractionResponse
            {
                Interactions = new List<DrugToDrugInteractionLineItem>
                {
                    new DrugToDrugInteractionLineItem
                    {
                        InteractingMedicationLineItems=new List<InteractingMedicationLineItem>
                        {
                            new InteractingMedicationLineItem
                            {

                            }
                        },
                        Onset="onset",
                        Severity="High",
                        Documentation="Duplicate",
                        WarningText="warning text",
                        FullWarningText="some text",
                        Copyright="Veradigm"
                    },
                    
                }
            };
            durResponse.AlcoholInteractions = new DrugToAlcoholInteractionResponse {
               Interactions=new List<DrugToAlcoholInteractionLineItem>
               {
                   new DrugToAlcoholInteractionLineItem
                   {
                       DDI=184865,
                       MedName="FeverAll Adults",
                       Ingredient="",
                       Onset="",
                       Severity="High",
                       Documentation="",
                       FullWarningText="some text",
                       Copyright="Veradigm"
                   }
               }
            };
            durResponse.FoodInteractions = new FoodInteractionResponse
            {
                Interactions= new List<DrugToFoodInteractionLineItem>
                {
                    new DrugToFoodInteractionLineItem
                    {
                         DDI=184865,
                       MedName="FeverAll Adults",
                       Ingredient="",
                       Onset="",
                       Severity="High",
                       Documentation="",
                       FullWarningText="some text",
                       Copyright="Veradigm"
                    }
                }
            };
            durResponse.PriorAdverseReactions = new PriorAdverseReactionResonse
            {
                Reactions = new List<PriorAdverseReactionCategory>
                {
                    new PriorAdverseReactionCategory
                    {
                        FullWarningText="some text",
                        WarningText="warning text",
                        LineItems=new List<PriorAdverseReactionLineItem>
                        {
                            new PriorAdverseReactionLineItem
                            {
                            ReactedAllergy=new Allergy(1,AllergyType.MEDICATION,AllergyReactionType.ALLERGIC,new List<string>{"fever","stomach pain" },"medicine allergy"),
                            DDI=184865,
                            PrescribedDrugName="FeverAll Adults",
                            PrescribedDrugClass="Medicine",
                            ReportedAllergyDrugName="some drug",
                            ReportedAllergyDrugClass="some allergy",
                            Copyright="veradigm",
                            ExternalId="1234"
                            }  
                        },
                        LineNumber=2,
                        DurIndex=4
                    }
                }
            };
            durResponse.DuplicateTherapy = new DuplicateTherapyResponse {
                Results = new List<DuplicateTherapyLineItem>
                {
                    new DuplicateTherapyLineItem
                    {
                        Copyright="",
                        DupDrugs=new List<DrugToCheck>
                        {
                            new DrugToCheck
                            {
                                DDI=184865,
                                DoseUnit=DoseUnit.TABLET,
                                DaysSupply=30,
                                DoseFrequency=2
                            }
                        },
                        WarningText="some text",
                        FullWarningText="warning text"                       
                    }
                }
            };
            durResponse.Dosage = new DosageResponse();
            durResponse.Dosage.DosageCheckMedications = new List<DosageLineItem>();
            var dosageCheckMedications = new DosageLineItem
            {
                Copyright = "",
                DDI = 184865,
                DaysSupply = 10,
                DurIndex = 1,
                ExternalId = "",
                IndividualDose = 0,
                LineNumber = 0,
                Quantity = 500,
                MedicationName = "FeverAll Adults",
                Refills = 0,
                Strength = "500",
                StrengthUnit = "MG",
                UnitsPerDay = 50,
                Warnings = new List<string>() { "The daily dose of 32,500 mg exceeds the usual dose of 650 to 3,900 mg.",
                    "The frequency of 50 times per day exceeds the usual frequency of 1 to 6 times per day."
                },
                WarningsForUI = "The daily dose of 32,500 mg exceeds the usual dose of 650 to 3,900 mg."
            };

            durResponse.Dosage.DosageCheckMedications.Add(dosageCheckMedications);
            durResponse.Copyright = "";
            durResponse.Warnings = new List<string> { "1", "2" };           
            return durResponse;
        }
        public void SetSessionVariable()
        {           
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.PatientDOB] = "12/12/1990";
            _pageState[SessionVariables.Gender] = "M";
            _pageState[SessionVariables.UserId] = "3661b948-796a-42e6-b4cd-5bdbbd198f04";
            _pageState[SessionVariables.LicenseId] = "efc07b53-a39a-4122-8fec-965393f7eec3";
            _pageState[SessionVariables.PracticeState] = "IL";
            _pageState[SessionVariables.PatientId] = "0ef45408-7c2f-41c9-b610-49eac407f86f";
            _pageState[SessionVariables.DbId] = ConnectionStringPointer.ERXDB_SERVER_1;
            _pageState[SessionVariables.ACTIVEMEDDDILIST] = SetActiveMedList();
            _pageState["CURRENT_SCRIPT_PAD_MEDS"] = SetScriptsPadMeds();
            _pageState[SessionVariables.DurPatientAllergies] = SetDurPatientAllergies();
            _pageState[SessionVariables.DURSettings] = SetDurSettings();
            _pageState[SessionVariables.SiteId] = 1;
        }
        public List<string> SetActiveMedList()
        {
            List<string> activeMedList = new List<string> { "044725", "184865", "051054" };
            return activeMedList;
        }
        public List<Rx> SetScriptsPadMeds()
        {
            List<Rx> rxList = new List<Rx>();
            Rx rx1 = new Rx
            {
                AlternativeIgnoreReason = -1,
                CanEditEffectiveDate = false,
                ControlledSubstanceCode = "",
                CoverageID = 0,
                DAW = false,
                DAWDetail = "",
                DDI = "044725",
                DaysSupply = 30,
                Destination = null,
                DosageFormCode = "TABS",
                DosageFormDescription = "Tablet",
                FormularyAlternativeShown = "N",
                HasSupplyItem = false,
                IsFreeFormMedControlSubstance = false,
                LineNumber = 1,
                MedicationName = "Amoxicillin",
                NDCNumber = "00093226301"
            };
            Rx rx2 = new Rx
            {
                AlternativeIgnoreReason = -1,
                CanEditEffectiveDate = false,
                ControlledSubstanceCode = "",
                CoverageID = 0,
                DAW = false,
                DAWDetail = "",
                DDI = "044725",
                DaysSupply = 30,
                Destination = null,
                DosageFormCode = "TABS",
                DosageFormDescription = "Tablet",
                FormularyAlternativeShown = "N",
                HasSupplyItem = false,
                IsFreeFormMedControlSubstance = false,
                LineNumber = 1,
                MedicationName = "Amoxicillin",
                NDCNumber = "00093226301"
            };
            Rx rx3 = new Rx
            {
                AlternativeIgnoreReason = -1,
                CanEditEffectiveDate = false,
                ControlledSubstanceCode = "",
                CoverageID = 0,
                DAW = true,
                DAWDetail = "",
                DDI = "051054",
                DaysSupply = 90,
                Destination = null,
                DosageFormCode = "TABS",
                DosageFormDescription = "Tablet",
                FormularyAlternativeShown = "N",
                HasSupplyItem = false,
                IsFreeFormMedControlSubstance = false,
                LineNumber = 1,
                MedicationName = "Lipitor",
                NDCNumber = "00071015723"
            };
            rxList.Add(rx1);
            rxList.Add(rx2);
            rxList.Add(rx3);
            return rxList;
        }
        public List<Rx> SetScriptsPadCsMeds()
        {
            List<Rx> rxList = new List<Rx>();
            Rx rx1 = new Rx
            {
                AlternativeIgnoreReason = -1,
                CanEditEffectiveDate = false,
                ControlledSubstanceCode = "0100",
                CoverageID = 0,
                DAW = false,
                DAWDetail = "",
                DDI = "044725",
                DaysSupply = 30,
                Destination = null,
                DosageFormCode = "TABS",
                DosageFormDescription = "Tablet",
                FormularyAlternativeShown = "N",
                HasSupplyItem = false,
                IsFreeFormMedControlSubstance = false,
                LineNumber = 1,
                MedicationName = "Amoxicillin",
                NDCNumber = "00093226301"
            };
            Rx rx2 = new Rx
            {
                AlternativeIgnoreReason = -1,
                CanEditEffectiveDate = false,
                ControlledSubstanceCode = "cd12",
                CoverageID = 0,
                DAW = false,
                DAWDetail = "",
                DDI = "044725",
                DaysSupply = 30,
                Destination = null,
                DosageFormCode = "TABS",
                DosageFormDescription = "Tablet",
                FormularyAlternativeShown = "N",
                HasSupplyItem = false,
                IsFreeFormMedControlSubstance = false,
                LineNumber = 1,
                MedicationName = "Amoxicillin",
                NDCNumber = "00093226301"
            };
            Rx rx3 = new Rx
            {
                AlternativeIgnoreReason = -1,
                CanEditEffectiveDate = false,
                ControlledSubstanceCode = "121212",
                CoverageID = 0,
                DAW = true,
                DAWDetail = "",
                DDI = "051054",
                DaysSupply = 90,
                Destination = null,
                DosageFormCode = "TABS",
                DosageFormDescription = "Tablet",
                FormularyAlternativeShown = "N",
                HasSupplyItem = false,
                IsFreeFormMedControlSubstance = false,
                LineNumber = 1,
                MedicationName = "Lipitor",
                NDCNumber = "00071015723"
            };
            rxList.Add(rx1);
            rxList.Add(rx2);
            rxList.Add(rx3);
            return rxList;
        }

        public List<Rx> SetScriptsPadCsMedsFreeFromCsMed()
        {
            List<Rx> rxList = new List<Rx>();
            Rx rx1 = new Rx
            {
                AlternativeIgnoreReason = -1,
                CanEditEffectiveDate = false,
                ControlledSubstanceCode = "0100",
                CoverageID = 0,
                DAW = false,
                DAWDetail = "",
                DDI = "044725",
                DaysSupply = 30,
                Destination = null,
                DosageFormCode = "TABS",
                DosageFormDescription = "Tablet",
                FormularyAlternativeShown = "N",
                HasSupplyItem = false,
                IsFreeFormMedControlSubstance = true,
                LineNumber = 1,
                MedicationName = "Amoxicillin",
                NDCNumber = "00093226301"
            };
            Rx rx2 = new Rx
            {
                AlternativeIgnoreReason = -1,
                CanEditEffectiveDate = false,
                ControlledSubstanceCode = "cd12",
                CoverageID = 0,
                DAW = false,
                DAWDetail = "",
                DDI = "044725",
                DaysSupply = 30,
                Destination = null,
                DosageFormCode = "TABS",
                DosageFormDescription = "Tablet",
                FormularyAlternativeShown = "N",
                HasSupplyItem = false,
                IsFreeFormMedControlSubstance = false,
                LineNumber = 1,
                MedicationName = "Amoxicillin",
                NDCNumber = "00093226301"
            };
            Rx rx3 = new Rx
            {
                AlternativeIgnoreReason = -1,
                CanEditEffectiveDate = false,
                ControlledSubstanceCode = "121212",
                CoverageID = 0,
                DAW = true,
                DAWDetail = "",
                DDI = "051054",
                DaysSupply = 90,
                Destination = null,
                DosageFormCode = "TABS",
                DosageFormDescription = "Tablet",
                FormularyAlternativeShown = "N",
                HasSupplyItem = false,
                IsFreeFormMedControlSubstance = false,
                LineNumber = 1,
                MedicationName = "Lipitor",
                NDCNumber = "00071015723"
            };
            rxList.Add(rx1);
            rxList.Add(rx2);
            rxList.Add(rx3);
            return rxList;
        }
        public DataTable SetIgnoreReasonsDataTable()
        {
            DataTable dtIgnoreReasons = new DataTable();
            dtIgnoreReasons.Columns.Add("ReasonID", typeof(int));
            dtIgnoreReasons.Columns.Add("ReasonDescription", typeof(string));
            dtIgnoreReasons.Columns.Add("Category", typeof(int));
            dtIgnoreReasons.Columns.Add("Active", typeof(char));
            dtIgnoreReasons.Columns.Add("Created", typeof(string));
            dtIgnoreReasons.Columns.Add("Modified", typeof(string));
            DataRow dr = dtIgnoreReasons.NewRow();

            dr["ReasonID"] = -1;
            dr["ReasonDescription"] = "*** Choose Ignore Reason ***";
            dr["Category"] = 1;
            dr["Active"] = 'Y';
            dr["Created"] = "5/30/2009 2:23:45 PM";
            dr["Modified"] = "";
            dtIgnoreReasons.Rows.Add(dr);
            return dtIgnoreReasons;
        }
        public RxTaskModel SetRxTaskModel()
        {
            var rxTaskModel = new RxTaskModel
            {
                PatientId = "0ef45408-7c2f-41c9-b610-49eac407f86f",
                Sex = "M",
                PatientDOB = "12/12/1990",
                ActiveMedList = SetActiveMedList(),
                LicenseId = Guid.NewGuid(),//new Guid("efc07b53-a39a-4122-8fec-965393f7eec3"),
                UserId = Guid.NewGuid(),
                SiteId = 1,
                PracticeState = "IL",
                DURWarnings = SetupDurWarnings(),
                TaskType = PrescriptionTaskType.DEFAULT,
                DbId = ConnectionStringPointer.ERXDB_SERVER_1
            };
            return rxTaskModel;
        }
        //public List<Rx> SetScriptsPadMeds()
        //{
        //    List<Rx> rxList = new List<Rx>();
        //    Rx rx1 = new Rx
        //    {
        //        AlternativeIgnoreReason = -1,
        //        CanEditEffectiveDate = false,
        //        ControlledSubstanceCode = "",
        //        CoverageID = 0,
        //        DAW = false,
        //        DAWDetail = "",
        //        DDI = "044725",
        //        DaysSupply = 30,
        //        Destination = null,
        //        DosageFormCode = "TABS",
        //        DosageFormDescription = "Tablet",
        //        FormularyAlternativeShown = "N",
        //        HasSupplyItem = false,
        //        IsFreeFormMedControlSubstance = false,
        //        LineNumber = 1,
        //        MedicationName = "Amoxicillin",
        //        NDCNumber = "00093226301"
        //    };
        //    Rx rx2 = new Rx
        //    {
        //        AlternativeIgnoreReason = -1,
        //        CanEditEffectiveDate = false,
        //        ControlledSubstanceCode = "",
        //        CoverageID = 0,
        //        DAW = false,
        //        DAWDetail = "",
        //        DDI = "044725",
        //        DaysSupply = 30,
        //        Destination = null,
        //        DosageFormCode = "TABS",
        //        DosageFormDescription = "Tablet",
        //        FormularyAlternativeShown = "N",
        //        HasSupplyItem = false,
        //        IsFreeFormMedControlSubstance = false,
        //        LineNumber = 1,
        //        MedicationName = "Amoxicillin",
        //        NDCNumber = "00093226301"
        //    };
        //    Rx rx3 = new Rx
        //    {
        //        AlternativeIgnoreReason = -1,
        //        CanEditEffectiveDate = false,
        //        ControlledSubstanceCode = "",
        //        CoverageID = 0,
        //        DAW = true,
        //        DAWDetail = "",
        //        DDI = "051054",
        //        DaysSupply = 90,
        //        Destination = null,
        //        DosageFormCode = "TABS",
        //        DosageFormDescription = "Tablet",
        //        FormularyAlternativeShown = "N",
        //        HasSupplyItem = false,
        //        IsFreeFormMedControlSubstance = false,
        //        LineNumber = 1,
        //        MedicationName = "Lipitor",
        //        NDCNumber = "00071015723"
        //    };
        //    rxList.Add(rx1);
        //    rxList.Add(rx2);
        //    rxList.Add(rx3);
        //    return rxList;
        //}
        public List<Model.Allergy> SetDurPatientAllergies()
        {
            List<Model.Allergy> allergies = new List<Model.Allergy>();
            var Symptoms = new List<string> { "Abdominal pain" };
            Model.Allergy allergy = new Model.Allergy(1157, Model.AllergyType.MEDICATION, AllergyReactionType.ALLERGIC, Symptoms, "Amoxicillin - Oral");
            allergies.Add(allergy);
            return allergies;
        }
        public Model.Request.DURCheckRequest SetDURCheckRequest()
        {
            Model.Request.DURCheckRequest durCheckRequest = new Model.Request.DURCheckRequest();
            List<Model.Allergy> allergies = SetDurPatientAllergies();
            List<DrugToCheck> DrugsToCheck = new List<DrugToCheck>{
            new DrugToCheck
            {
                DDI = 044725,
                DaysSupply = 30,
                StrengthUOM="",
                DoseFrequency=2,
                DosageFormCode="TABS",
                Quantity=60,
                Refills=0,
                MedicationName= "Amoxicillin",
                Strength="",
                DoseUnit= Model.DoseUnit.TABLET
            },
            new DrugToCheck
            {
                DDI = 044725,
                DaysSupply = 30,
                StrengthUOM="",
                DoseFrequency=2,
                DosageFormCode="TABS",
                Quantity=60,
                Refills=0,
                MedicationName= "Amoxicillin",
                Strength="",
                DoseUnit= DoseUnit.TABLET
            },
            new DrugToCheck
            {
                DDI = 051054,
                DaysSupply = 90,
                StrengthUOM="",
                DoseFrequency=2,
                DosageFormCode="TABS",
                Quantity=60,
                Refills=0,
                MedicationName= "Lipitor",
                Strength="",
                DoseUnit= DoseUnit.TABLET
            }
        };
            List<int> ExistingMedsDDIs = new List<int> { 44725, 184865 };
            durCheckRequest.Allergies = allergies;
            durCheckRequest.DrugsToCheck = DrugsToCheck;
            durCheckRequest.ExistingMedsDDIs = ExistingMedsDDIs;
            durCheckRequest.MinimumDocumentationLevelForMajorSeverity = InteractionDocumentationLevel.Doubtful;
            durCheckRequest.MinimumDocumentationLevelForMinorSeverity = InteractionDocumentationLevel.Doubtful;
            durCheckRequest.MinimumDocumentationLevelForModerateSeverity = InteractionDocumentationLevel.Doubtful;
            durCheckRequest.MinimumManagementLevel = InteractionsManagementLevel.PotentialInteractionRisk;
            durCheckRequest.MinimumSeverity = InteractionSeverity.Minor;
            durCheckRequest.Settings = SetDurSettings();
            return durCheckRequest;
        }
        public DURSettings SetDurSettings()
        {

            DURSettings durSettings = new DURSettings
            {
                CheckAlcoholInteraction = YesNoSetting.Yes,
                CheckDrugToDrugInteraction = YesNoSetting.Yes,
                CheckDuplicateTherapy = YesNoSetting.Yes,
                CheckDuplicateTherapyRange = YesNoSetting.Yes,
                CheckDuplicateTherapyWarning = YesNoSetting.Yes,
                CheckFoodInteraction = YesNoSetting.Yes,
                CheckMaxConsecutiveDose = YesNoSetting.Yes,
                CheckMaxDose = YesNoSetting.Yes,
                CheckMaxDurationDose = YesNoSetting.Yes,
                CheckMaxIndividualDose = YesNoSetting.Yes,
                CheckMinDose = YesNoSetting.Yes,
                CheckMinDurationDose = YesNoSetting.Yes,
                CheckPar = YesNoSetting.Yes,
                CheckPerformDose = YesNoSetting.Yes,
                DuplicateTherapyRangeCheckType = DuplicateTherapyRange.AbuseOrDependency,
                DuplicateTherapyWarningCheckType = DuplicateTherapyWarning.MediSpanDuplicationAllowanceExceeded,
                IgnoreReasons = DURIgnoreReason.REQUIRED,
                InteractionDocumentType = InteractionDocumentationLevel.Doubtful,
                InteractionOnsetCheckType = InteractionOnset.Delayed,
                InteractionSeverityCheckType = InteractionSeverity.Minor
            };
            return durSettings;
        }
        public List<IgnoreReasonsResponse> BuildIgnoreReasons()
        {
            List<IgnoreReasonsResponse> lstIgnoreReasonsResponse = new List<IgnoreReasonsResponse>
            {
            new IgnoreReasonsResponse
            {
            ReasonID = -1,
            ReasonDescription = "*** Choose Ignore Reason ***",
            Category = 1,
            Active = 'Y',
            Created = "5/30/2009 2:23:45 PM",
            Modified = ""
            },
             new IgnoreReasonsResponse
            {
            ReasonID = 61,
            ReasonDescription = "Allergy history is unreliable",
            Category = 1,
            Active = 'Y',
            Created = "5/30/2009 2:23:45 PM",
            Modified = ""
            },
              new IgnoreReasonsResponse
            {
            ReasonID = 62,
            ReasonDescription = "Benefit of treatment outweighs risk",
            Category = 1,
            Active = 'Y',
            Created = "5/30/2009 2:23:45 PM",
            Modified = ""
            },
               new IgnoreReasonsResponse
            {
            ReasonID = 63,
            ReasonDescription = "Aware will monitor",
            Category = 1,
            Active = 'Y',
            Created = "5/30/2009 2:23:45 PM",
            Modified = ""
            },
               new IgnoreReasonsResponse
            {
            ReasonID = 64,
            ReasonDescription = "Benefit outweighs risk in this patient",
            Category = 1,
            Active = 'Y',
            Created = "5/30/2009 2:23:45 PM",
            Modified = ""
            },
               new IgnoreReasonsResponse
            {
            ReasonID = 66,
            ReasonDescription = "Discussed and Patient understands risk, alternative, benefits",
            Category = 1,
            Active = 'Y',
            Created = "5/30/2009 2:23:45 PM",
            Modified = ""
            },
              new IgnoreReasonsResponse
            {
            ReasonID = 67,
            ReasonDescription = "Intolerance, not allergic",
            Category = 1,
            Active = 'Y',
            Created = "5/30/2009 2:23:45 PM",
            Modified = ""
            },
               new IgnoreReasonsResponse
            {
            ReasonID = 68,
            ReasonDescription = "Lack of alternate medication",
            Category = 1,
            Active = 'Y',
            Created = "5/30/2009 2:23:45 PM",
            Modified = ""
            },
               new IgnoreReasonsResponse
            {
            ReasonID = 69,
            ReasonDescription = "Low risk: appropriate warnings given to patient",
            Category = 1,
            Active = 'Y',
            Created = "5/30/2009 2:23:45 PM",
            Modified = ""
            },
               new IgnoreReasonsResponse
            {
            ReasonID = 110,
            ReasonDescription = "No reasonable alternative for this patient",
            Category = 1,
            Active = 'Y',
            Created = "5/30/2009 2:23:45 PM",
            Modified = ""
            },
               new IgnoreReasonsResponse
            {
            ReasonID = 70,
            ReasonDescription = "Other",
            Category = 1,
            Active = 'Y',
            Created = "5/30/2009 2:23:45 PM",
            Modified = ""
            },
                new IgnoreReasonsResponse
            {
            ReasonID = 71,
            ReasonDescription = "Patient currently on medication with no problems",
            Category = 1,
            Active = 'Y',
            Created = "5/30/2009 2:23:45 PM",
            Modified = ""
            },
               new IgnoreReasonsResponse
            {
            ReasonID = 72,
            ReasonDescription = "Patient does not have condition causing alert",
            Category = 1,
            Active = 'Y',
            Created = "5/30/2009 2:23:45 PM",
            Modified = ""
            },
               new IgnoreReasonsResponse
            {
            ReasonID = 73,
            ReasonDescription = "Reaction nonsystemic",
            Category = 1,
            Active = 'Y',
            Created = "5/30/2009 2:23:45 PM",
            Modified = ""
            },
            };
            return lstIgnoreReasonsResponse;
        }
        public List<FreeFormRxDrug> SetFreeFormRxDrug()
        {
            List<FreeFormRxDrug> lstfreeFormDrugs = new List<FreeFormRxDrug>
             {
                 new FreeFormRxDrug{
                MedicationName="something",
                 Strength ="10",
                 StrengthUOM="mg",
                 ExternalId ="1321",
                 WarningText="etrtyryt"
                }
             };
            return lstfreeFormDrugs;
        }

        public ArrayList SetRxList()
        {
            Impact.Rx rx = new Impact.Rx();
            rx.RxID = Guid.NewGuid().ToString();
            rx.MedicationName = "Lipitor";
            rx.Strength = "650";
            rx.StrengthUOM = "MG";
            rx.DosageFormCode = "SUPP";
            rx.RouteOfAdminCode = "OR";
            rx.ControlledSubstanceCode = "cs001";
            rx.ScheduleUsed = 1;
            rx.IsFreeFormMedControlSubstance = false;
            rx.DDI = "112";
            ArrayList arr = new ArrayList();
            arr.Add(rx);
            return arr;
        }
        public ArrayList SetRxListWithoutDDI()
        {
            Impact.Rx rx = new Impact.Rx();
            rx.RxID = Guid.NewGuid().ToString();
            rx.MedicationName = "Lipitor";
            rx.Strength = "650";
            rx.StrengthUOM = "MG";
            rx.DosageFormCode = "SUPP";
            rx.RouteOfAdminCode = "OR";
            rx.ControlledSubstanceCode = "cs001";
            rx.ScheduleUsed = 1;
            rx.IsFreeFormMedControlSubstance = false;
            rx.DDI = "";
            ArrayList arr = new ArrayList();
            arr.Add(rx);
            return arr;
        }

        public ArrayList SetRxListWithScheduleUsedTwo()
        {
            Impact.Rx rx = new Impact.Rx();
            rx.RxID = Guid.NewGuid().ToString();
            rx.MedicationName = "Lipitor";
            rx.Strength = "650";
            rx.StrengthUOM = "MG";
            rx.DosageFormCode = "SUPP";
            rx.RouteOfAdminCode = "OR";
            rx.ControlledSubstanceCode = "cs001";
            rx.ScheduleUsed = 2;
            rx.IsFreeFormMedControlSubstance = true;
            rx.DDI = "";
            ArrayList arr = new ArrayList();
            arr.Add(rx);
            return arr;
        }
        public ArrayList SetRxListWithScheduledAsZero()
        {
            Impact.Rx rx = new Impact.Rx();
            rx.RxID = Guid.NewGuid().ToString();
            rx.MedicationName = "Lipitor";
            rx.Strength = "650";
            rx.StrengthUOM = "MG";
            rx.DosageFormCode = "SUPP";
            rx.RouteOfAdminCode = "OR";
            rx.ControlledSubstanceCode = "cs001";
            rx.ScheduleUsed = 0;
            rx.IsFreeFormMedControlSubstance = false;
            rx.DDI = "";
            ArrayList arr = new ArrayList();
            arr.Add(rx);
            return arr;
        }

        public ArrayList SetRxListIsFreeFormMedControlSubstance()
        {
            Impact.Rx rx = new Impact.Rx();
            rx.RxID = Guid.NewGuid().ToString();
            rx.MedicationName = "Lipitor";
            rx.Strength = "650";
            rx.StrengthUOM = "MG";
            rx.DosageFormCode = "SUPP";
            rx.RouteOfAdminCode = "OR";
            rx.ControlledSubstanceCode = "cs001";
            rx.ScheduleUsed = 1;
            rx.IsFreeFormMedControlSubstance = true;
            rx.DDI = "";
            ArrayList arr = new ArrayList();
            arr.Add(rx);
            return arr;
        }
        public string GetShieldToken()
        {
            return
                "<saml:Assertion MajorVersion=\"1\" MinorVersion=\"1\" AssertionID=\"_7b99507a-15c4-46ea-8c97-22ae20ecab2f\" Issuer=\"https://rp.allscripts.com/shield/shieldauthority\" IssueInstant=\"2016-11-09T21:53:53.434Z\" xmlns:saml=\"urn:oasis:names:tc:SAML:1.0:assertion\"><saml:Conditions NotBefore=\"2016-11-09T21:53:53.434Z\" NotOnOrAfter=\"2016-11-09T22:13:53.434Z\"><saml:AudienceRestrictionCondition><saml:Audience>http://rp.allscripts.com/ePrescribe/MainApp</saml:Audience></saml:AudienceRestrictionCondition></saml:Conditions><saml:AttributeStatement><saml:Subject><saml:SubjectConfirmation><saml:ConfirmationMethod>urn:oasis:names:tc:SAML:1.0:cm:holder-of-key</saml:ConfirmationMethod><KeyInfo xmlns=\"http://www.w3.org/2000/09/xmldsig#\"><trust:BinarySecret xmlns:trust=\"http://docs.oasis-open.org/ws-sx/ws-trust/200512\">AjNlRa50FA1S2U5m4Y0p3DjyDqer3XQ32w6DPygzoeU=</trust:BinarySecret></KeyInfo></saml:SubjectConfirmation></saml:Subject><saml:Attribute AttributeName=\"name\" AttributeNamespace=\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims\" a:OriginalIssuer=\"http://authenticate.cloudsrt.local/adfs/services/trust\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2009/09/identity/claims\"><saml:AttributeValue>CSprovider2</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"surname\" AttributeNamespace=\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims\" a:OriginalIssuer=\"http://authenticate.cloudsrt.local/adfs/services/trust\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2009/09/identity/claims\"><saml:AttributeValue>Smullen</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"givenname\" AttributeNamespace=\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims\" a:OriginalIssuer=\"http://authenticate.cloudsrt.local/adfs/services/trust\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2009/09/identity/claims\"><saml:AttributeValue>Michael</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"pwdexpiretime\" AttributeNamespace=\"http://schemas.allscripts.com/shield\" a:OriginalIssuer=\"http://authenticate.cloudsrt.local/adfs/services/trust\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2009/09/identity/claims\"><saml:AttributeValue>131243993417882058</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"changepwduri\" AttributeNamespace=\"http://schemas.allscripts.com/shield\" a:OriginalIssuer=\"http://authenticate.cloudsrt.local/adfs/services/trust\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2009/09/identity/claims\"><saml:AttributeValue>https://authenticate.cloudsrt.local/adfs/ls/ChangePassword.aspx</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"middlename\" AttributeNamespace=\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims\" a:OriginalIssuer=\"http://authenticate.cloudsrt.local/adfs/services/trust\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2009/09/identity/claims\"><saml:AttributeValue /></saml:Attribute><saml:Attribute AttributeName=\"emailaddress\" AttributeNamespace=\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims\" a:OriginalIssuer=\"http://authenticate.cloudsrt.local/adfs/services/trust\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2009/09/identity/claims\"><saml:AttributeValue>judy.keadle@allscripts.com</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"tokenreferencelist\" AttributeNamespace=\"http://schema.allscripts.com/security/sts\"><saml:AttributeValue>77u/PEFycmF5T2ZUb2tlblJlZmVyZW5jZSB4bWxuczppPSJodHRwOi8vd3d3LnczLm9yZy8yMDAxL1hNTFNjaGVtYS1pbnN0YW5jZSIgeG1sbnM9Imh0dHA6Ly9zY2hlbWFzLmRhdGFjb250cmFjdC5vcmcvMjAwNC8wNy9NZHJ4LlNoaWVsZC5Db21tb24uU2VjdXJpdHkiPjxUb2tlblJlZmVyZW5jZT48QXV0aGVudGljYXRpb25JRD5fNzI1MjhlMDEtZDBmYS00NDEyLWI3ZTAtNjk5NmQ5ZGM4NzM5PC9BdXRoZW50aWNhdGlvbklEPjxBdXRoZW50aWNhdGlvblR5cGU+UGFzc3dvcmQ8L0F1dGhlbnRpY2F0aW9uVHlwZT48SXNzdWVyVXJpPmh0dHA6Ly9hdXRoZW50aWNhdGUuY2xvdWRzcnQubG9jYWwvYWRmcy9zZXJ2aWNlcy90cnVzdDwvSXNzdWVyVXJpPjwvVG9rZW5SZWZlcmVuY2U+PC9BcnJheU9mVG9rZW5SZWZlcmVuY2U+</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"role\" AttributeNamespace=\"http://schemas.microsoft.com/ws/2008/06/identity/claims\"><saml:AttributeValue>http://schema.allscripts.com/ePrescribe/MainApp/Roles/Admin</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/ePrescribe/MainApp/Roles/Provider</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/shield/TenantAdmin</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"permission\" AttributeNamespace=\"http://schemas.allscripts.com/shield\"><saml:AttributeValue>http://schema.allscripts.com/ePrescribe/MainApp/Permissions/ViewSettings</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/ePrescribe/MainApp/Permissions/ProcessTasks</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/ePrescribe/MainApp/Permissions/EditPatients</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/ePrescribe/MainApp/Permissions/ManageDeluxeBilling</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/ePrescribe/MainApp/Permissions/AddPatients</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/ePrescribe/MainApp/Permissions/ePA</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/ePrescribe/MainApp/Permissions/Prescribe</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/security/epcs/canenroll</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/security/epcs/canapprove</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/security/epcs/canprescribe</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/security/epcs/canviewproviderreports</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/security/epcs/canviewsecurityreports</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"identityId\" AttributeNamespace=\"http://schemas.allscripts.com/shield\"><saml:AttributeValue>MSmullen</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"applicationInstanceId\" AttributeNamespace=\"http://schemas.allscripts.com/shield\"><saml:AttributeValue>ERX-2451634A</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"environmentId\" AttributeNamespace=\"http://schemas.allscripts.com/shield\"><saml:AttributeValue>ERX-2451634I</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"environmentName\" AttributeNamespace=\"http://schemas.allscripts.com/shield\"><saml:AttributeValue>Judysite</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"tenantId\" AttributeNamespace=\"http://schemas.allscripts.com/shield\"><saml:AttributeValue>ERX-2451634</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"tenantName\" AttributeNamespace=\"http://schemas.allscripts.com/shield\"><saml:AttributeValue>Judysite</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"profileId\" AttributeNamespace=\"http://schemas.allscripts.com/shield\"><saml:AttributeValue>b426ab57-be4a-4571-b0ca-35cd6c111944</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"dearegistrantstatus\" AttributeNamespace=\"http://schema.allscripts.com/security/epcs\"><saml:AttributeValue>Valid</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"level\" AttributeNamespace=\"http://schema.allscripts.com/security/shield/idproofing\"><saml:AttributeValue>3</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"CompromisedIdentity\" AttributeNamespace=\"http://schema.allscripts.com/shield\"><saml:AttributeValue>False</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"tenantidproofingmodel\" AttributeNamespace=\"http://schema.allscripts.com/security/epcs\"><saml:AttributeValue>Individual</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"status\" AttributeNamespace=\"http://schema.allscripts.com/security/shield/idproofing\"><saml:AttributeValue>Passed</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"application\" AttributeNamespace=\"http://schemas.allscripts.com/shield\"><saml:AttributeValue>http://rp.allscripts.com/ePrescribe/MainApp</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"scopetype\" AttributeNamespace=\"http://schemas.allscripts.com/shield\"><saml:AttributeValue>ApplicationInstance</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"scopefilter\" AttributeNamespace=\"http://schemas.allscripts.com/shield\"><saml:AttributeValue>ERX-2451634A</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"name\" AttributeNamespace=\"http://schema.allscripts.com/security/shield/account\"><saml:AttributeValue>CSprovider2</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"objectid\" AttributeNamespace=\"http://schema.allscripts.com/security/shield/account\"><saml:AttributeValue>9c58869f-1ac9-40ad-8347-c160f6332d6b</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"primarysid\" AttributeNamespace=\"http://schemas.microsoft.com/ws/2008/06/identity/claims\"><saml:AttributeValue>S-1-5-21-1795162713-183561324-162468270-1885</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"typeofuser\" AttributeNamespace=\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims\"><saml:AttributeValue>StandardUser</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"first\" AttributeNamespace=\"http://schema.allscripts.com/security/shield/account/name\"><saml:AttributeValue>Michael</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"middle\" AttributeNamespace=\"http://schema.allscripts.com/security/shield/account/name\"><saml:AttributeValue /></saml:Attribute><saml:Attribute AttributeName=\"last\" AttributeNamespace=\"http://schema.allscripts.com/security/shield/account/name\"><saml:AttributeValue>Smullen</saml:AttributeValue></saml:Attribute></saml:AttributeStatement><saml:AuthenticationStatement AuthenticationMethod=\"urn:oasis:names:tc:SAML:1.0:am:password\" AuthenticationInstant=\"2016-11-09T21:53:51.786Z\"><saml:Subject><saml:SubjectConfirmation><saml:ConfirmationMethod>urn:oasis:names:tc:SAML:1.0:cm:holder-of-key</saml:ConfirmationMethod><KeyInfo xmlns=\"http://www.w3.org/2000/09/xmldsig#\"><trust:BinarySecret xmlns:trust=\"http://docs.oasis-open.org/ws-sx/ws-trust/200512\">AjNlRa50FA1S2U5m4Y0p3DjyDqer3XQ32w6DPygzoeU=</trust:BinarySecret></KeyInfo></saml:SubjectConfirmation></saml:Subject></saml:AuthenticationStatement><ds:Signature xmlns:ds=\"http://www.w3.org/2000/09/xmldsig#\"><ds:SignedInfo><ds:CanonicalizationMethod Algorithm=\"http://www.w3.org/2001/10/xml-exc-c14n#\" /><ds:SignatureMethod Algorithm=\"http://www.w3.org/2001/04/xmldsig-more#rsa-sha256\" /><ds:Reference URI=\"#_7b99507a-15c4-46ea-8c97-22ae20ecab2f\"><ds:Transforms><ds:Transform Algorithm=\"http://www.w3.org/2000/09/xmldsig#enveloped-signature\" /><ds:Transform Algorithm=\"http://www.w3.org/2001/10/xml-exc-c14n#\" /></ds:Transforms><ds:DigestMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#sha256\" /><ds:DigestValue>RwHl5EyvEL4UkQuPe3iKEkf02Es2SvRSrxYmkEPChg4=</ds:DigestValue></ds:Reference></ds:SignedInfo><ds:SignatureValue>at0maA0kob5Z5ZRyfp1OAUei5f8oDwoJG0mFLZikpPiaRam8HVOXZCGlLbUshPFYiA/SXO+q/QuriBmjudwhuyxPr7hcA0SFaK/4yCH2HbZlX/yYaTJTQWrjdBR3V6y8bxjjVZgGDuJmSxmLnTSGOkraJpJBegNmJRtu0nzbYazkW8OagIp0mET4nBTB6gi89qr1vbx3heM9A6hTpfLFGUim+HISSy690FxsaRuLEdDmKIjuQ+zZ644DdJkemTid1VC5b9QjiTY0msVgXcxlY7Vokn193e/brzGHVVq0wECmqCLy37GpO/ifBLm6sFihZ0ORLFn8PCJBHycxZXmc1w==</ds:SignatureValue><KeyInfo xmlns=\"http://www.w3.org/2000/09/xmldsig#\"><X509Data><X509Certificate>MIIEqDCCA5CgAwIBAgITVQAAACdsq/t1I+jABAAAAAAAJzANBgkqhkiG9w0BAQowADBQMRUwEwYKCZImiZPyLGQBGRYFbG9jYWwxGDAWBgoJkiaJk/IsZAEZFghjbG91ZHNydDEdMBsGA1UEAxMUSXNzdWluZ0NBLUNsb3VkU1JUQ0EwHhcNMTUwNDMwMTg0NTAzWhcNMjAwNDMwMTg1NTAzWjBNMRUwEwYKCZImiZPyLGQBGRYFbG9jYWwxGDAWBgoJkiaJk/IsZAEZFghjbG91ZHNydDEaMBgGA1UEAxMRc2FzLXRva2VuLXNpZ25pbmcwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDEC4wmZ864hKO0RSE+jPDZDc+7Wv9t1tgUE4bkMP0yYkXdsairfVeamR5tgT6TeHwRVmmupYCi5tADNDS0qQm+GIDnKDMqOvi8eBiyWxUmw+fUUbpauG8UTyroj7KcGsHgG3vzsRDyeIRfwGIr+bc+jGnP4uvjgp+agBmKy4xhZ2jJbst1HDspR2A3OtqkLsNsmFCyYSA/q4ktw7HMxhKd6Xfef2j8Ca80ll17F6Vw0MgFvuETPOkYhs3Bd2E1Cz2FCZLN2cvK1g49lx6aSBkOmXGHOa3+PaqquK/5fS22Hixf2YvQkyAQS8u1wsW7zCneL2SBso+R6NO3uzYKrd0vAgMBAAGjggF8MIIBeDA9BgkrBgEEAYI3FQcEMDAuBiYrBgEEAYI3FQiF34BR/L4ng4mTNoL91HaCtYc8gQuExq0Bg5SqEQIBZAIBDDATBgNVHSUEDDAKBggrBgEFBQcDATAOBgNVHQ8BAf8EBAMCBaAwGwYJKwYBBAGCNxUKBA4wDDAKBggrBgEFBQcDATAdBgNVHQ4EFgQU55viaV0VglsKVo1h/v9tSuy1+sIwHwYDVR0jBBgwFoAUJbi7SO3ov31K5bR7tgB/CUNHiOUwRwYDVR0fBEAwPjA8oDqgOIY2aHR0cDovL3d3dy5jbG91ZHNydC5sb2NhbC9wa2kvSXNzdWluZ0NBLUNsb3VkU1JUQ0EuY3JsMGwGCCsGAQUFBwEBBGAwXjBcBggrBgEFBQcwAoZQaHR0cDovL3d3dy5jbG91ZHNydC5sb2NhbC9wa2kvQ0xPVURTUlRDQS5jbG91ZHNydC5sb2NhbF9Jc3N1aW5nQ0EtQ2xvdWRTUlRDQS5jcnQwDQYJKoZIhvcNAQEKMAADggEBAFwEY/J0FNEZfZQT2FXnFC7oSCDlOMxGmSKK0sU54MR2KzZFfXrItVLpNyamEc9RkzmLqzcntpORVRv4u6S8PNzeEyioeP2tV5cLd0d6Vpqs0oACCJaB0Uq/E6Im5u7SG+5mWxNo9PcFjs7YJRRAye7Oa2UlAXaUB1WqoMW26vmIvYvTQDKrYoqGCxLjF2+oZaqy3XG91+0EnrJm1nMdi2U/UDwdMmsOeJy3xb3NVykUDcwY1tLNe59lufcqjsvZyXVO7yKZv3iBFxk9gJ98wp17xq8bXQa5tm+x35Iy1uEgZHZH0u0ZIHqojZoaSvWkR7uzwNXcKjEJ/u8GY2XtV5w=</X509Certificate></X509Data></KeyInfo></ds:Signature></saml:Assertion>";
        }

        public DataSet GetAllPermissionList(List<string> sPermissions, ConnectionStringPointer dbID)
        {
            List<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@ClaimValues", GetAllClaimDataTable(sPermissions)));

            return DataAccessHelper.ExecuteDataSet(
                dbID,
                CommandType.StoredProcedure,
                "GetPermissionsNames",
                paramList);

        }
        public DataTable GetAllClaimDataTable(List<string> sPermissions)
        {
            DataTable dt = new DataTable("ClaimValues");

            DataColumn dc = new DataColumn("Value", Type.GetType("System.String"));
            dc.Unique = true;
            dt.Columns.Add(dc);

            foreach (string str in sPermissions)
            {
                DataRow dr = dt.NewRow();
                dr["Value"] = str;
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public List<Objects.Permission> buildPermissionList(List<string> sPermissions, ConnectionStringPointer dbID)
        {
            List<Objects.Permission> permissions = new List<Objects.Permission>();
            DataSet dsPerm = GetAllPermissionList(sPermissions, dbID);
            DataTable dtPerm = dsPerm.Tables[0];
            foreach (DataRow row in dtPerm.Rows)
            {
                permissions.Add(new Objects.Permission(row["Name"].ToString(), row["Value"].ToString()));
            }
            return permissions;
        }

        public StateLicense[] mockedStateLicenseList()
        {
            List<StateLicense> stateLicenseList = new List<StateLicense>();
            
            StateLicense sl1 = new StateLicense();
            sl1.ExpiringDate = "2025/01/01";
            sl1.LicenseNo ="IL1234567";
            sl1.LicenseType = "State License Number";
            sl1.State="IL";
            
            StateLicense sl2 = new StateLicense();
            sl2.ExpiringDate = "2025/01/01";
            sl2.LicenseNo  = "MI1234568";
            sl2.LicenseType = "State License Number";
            sl2.State = "MI";
            
            stateLicenseList.Add(sl1);
            stateLicenseList.Add(sl2);

            return stateLicenseList.ToArray();
        }
    }

}
