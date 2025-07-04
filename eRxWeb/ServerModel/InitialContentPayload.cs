using System.Collections.Generic;

namespace eRxWeb.ServerModel
{
    class InitialContentPayload
    {
        public ContentRefreshPayload ContentRefreshPayload { get; set; }
        public UserPreferenceModel UserPreferencePayload { get; set; }
        public SiteInfo SitePayload { get; set; }
        public CommonUiIInitialPayload CommonUiInitialPayload { get; set; }
        public AppInfo FooterPayload { get; set; }
        public List<MenuItem> MenuPayload { get; set; }
        public int SessionTimeoutPayload  { get; set; }
        public SelectPatientStartupParameters SelectPatientPayload { get; set; }
        public MedicationSearchHelp HelpSearchPayload { get; set; }
    }
}