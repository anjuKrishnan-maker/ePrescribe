using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.ExtensionMethods;

namespace eRxWeb.ServerModel
{
    public class ReviewHistoryModel
    {
        public string RxID { get; set; }
        public string RxDate { get; set; }
        public string Prescription { get; set; }
        public string Diagnosis { get; set; }
        public string StatusDescription { get; set; }
        public string RxSource { get; set; }
        public bool MedHistoryExists { get; set; }
        public bool ExtraDetailAvailable { get; set; }
        public bool IsScriptCancelRxEligible { get; set; }
        public Guid OriginalNewRxTrnsCtrlNo { get; set; }
        public SelectionGroup SelectionGroupId { get; set; }
        public bool IsPbmMed { get; set; }

        // helper vars
        public  bool Selected { get; set; }
        public  bool IsSelectionDisabled { get; set; }
        public bool DisplayFillDetails { get; set; }
        public string FillDetailsHtml { get; set; }

    }

    public enum StatusFilterEnum
    {
        Active = 0,
        Inactive = 1,
        All = 2
    }


    public class GetPatientReviewHistoryResponse
    {
        public List<ReviewHistoryModel> HistoryItems { get; set; }

        public bool MoreRowsAvailable { get; set; }

        public bool ActiveMedsPresent { get; set; }
    }

    public static class StatusFilterEnumExtensions
    {
        public static string ToDatabaseConstant(this StatusFilterEnum statusFilter)
        {
            var result = "All";
            switch (statusFilter)
            {
                case StatusFilterEnum.Active:
                    result = "Act";
                    break;
                case StatusFilterEnum.Inactive:
                    result = "Dis";
                    break;
            }
            return result;
        }

    }
    /// <summary>
    /// When an item from this group will be selected, corresponding buttons will be enabled, and all items from other groups disabled.
    /// </summary>
    public enum SelectionGroup
    {
        EieComplete = 0,
        DiscontinueComplete = 1,
        DiscontinueEieComplete = 2,
        Eie = 3,
        AlwaysDisabled

    }
    public enum SortDirectionEnum
    {
        ASC = 0,
        DESC = 1
    }


    public class DataRetrievalContext
    {
        public string SortColumnName { get; set; }
        public SortDirectionEnum SortDirection { get; set; }
        public int SkipRows { get; set; }
        public int FetchRows { get; set; }

    }

    public class EieActionRequestModel
    {
        public string RxID { get; set; }
        public bool IsPbmMed { get; set; }
    }

    public class ReviewHistoryStartupParameters
    {
        public bool IsAddDiagnosisVisible { get; set; }
        public bool IsSsoLockdownMode { get; set; }
        public bool IsInactivePatient { get; set; }
        public List<ReviewHistoryProvider> Providers { get; set; }
        public string DelegateProviderId { get;set; }

        public ReviewHistoryUserType UserType { get; set; }
        public bool IsRestrictedPatient { get; set; }
    }


    public class ReviewHistoryProvider
    {
        public string ProviderName { get; set; }
        public string ProviderId { get; set; }
        public int UserTypeID { get; set; }
    }

    public enum ReviewHistoryUserType
    {
        Provider = 1,
        PAwithSupervision = 2,
        Staff = 3
    }
}


