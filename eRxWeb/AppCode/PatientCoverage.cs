using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using eRxWeb.AppCode.PptPlusBPL;
using eRxWeb.Controller;
using eRxWeb.ServerModel.Request;
using eRxWeb.State;
using IPatient = Allscripts.Impact.Interfaces.IPatient;
using IPatientCoverage = Allscripts.Impact.Interfaces.IPatientCoverage;

namespace eRxWeb.AppCode
{
    public class PatientCoverage
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        public static void UpdatePatientCoverage(UpdatePatientSelectedCoverageRequest request, IStateContainer session, IPatientCoverage patient, IPptPlus pptPlus)
        {
            if (string.IsNullOrWhiteSpace(request.CoverageId))
            {
                ClearPatientCoverage(session);
                return;
            }
            var dbId = ApiHelper.GetDBID(session);

            var patCoverage = patient.GetCoverage(Convert.ToInt64(request.CoverageId), dbId);

            session[Constants.SessionVariables.SelectedCoverageId] = request.CoverageId;
            session[Constants.SessionVariables.PlanId] = patCoverage.PlanId;
            session[Constants.SessionVariables.FormularyId] = patCoverage.FormularyId;
            session[Constants.SessionVariables.FormularyActive] = patCoverage.FormularyActive;
            session[Constants.SessionVariables.OtcCoverage] = patCoverage.OtcCoverage;
            session[Constants.SessionVariables.GenericDrugPolicy] = patCoverage.GenericDrugPolicy;
            session[Constants.SessionVariables.UnListedDrugPolicy] = patCoverage.UnListedDrugPolicy;
            session[Constants.SessionVariables.CoverageId] = patCoverage.CoverageId;
            session[Constants.SessionVariables.CopayId] = patCoverage.CopayId;
            session[Constants.SessionVariables.AltPlanId] = patCoverage.AltPlanId;
            session[Constants.SessionVariables.InfoSourcePayerId] = patCoverage.InfoSourcePayerId;

            if (PPTPlus.IsPPTPlusPreferenceOn(session))
            {
                var rtbi = new PptPlusData().GetPatientRtbiInto(session.GetGuidOr0x0(Constants.SessionVariables.PatientId), request.CoverageId, dbId);
                session[Constants.SessionVariables.PptRtbiInfo] = rtbi;
                PPTPlus.ClearScriptPadCandidates(session);
            }
        }
        public static void ClearPatientCoverage(IStateContainer session)
        {
            session.Remove(Constants.SessionVariables.SelectedCoverageId);
            session.Remove(Constants.SessionVariables.PlanId);
            session.Remove(Constants.SessionVariables.FormularyId);
            session.Remove(Constants.SessionVariables.FormularyActive);
            session.Remove(Constants.SessionVariables.OtcCoverage);
            session.Remove(Constants.SessionVariables.GenericDrugPolicy);
            session.Remove(Constants.SessionVariables.UnListedDrugPolicy);
            session.Remove(Constants.SessionVariables.CoverageId);
            session.Remove(Constants.SessionVariables.CopayId);
            session.Remove(Constants.SessionVariables.AltPlanId);
            session.Remove(Constants.SessionVariables.InfoSourcePayerId);
            session.Remove(Constants.SessionVariables.PptRtbiInfo);
        }
    }
}