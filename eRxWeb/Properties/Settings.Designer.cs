﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace eRxWeb.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.6.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://localhost:1646/DataManagerService/DataManagerSvc.asmx")]
        public string eRxWeb_DataManagerSvc_DataManagerSvc {
            get {
                return ((string)(this["eRxWeb_DataManagerSvc_DataManagerSvc"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://localhost:3245/ePrescribeService/ePrescribeSvc.asmx")]
        public string eRxWeb_ePrescribeSvc_ePrescribeSvc {
            get {
                return ((string)(this["eRxWeb_ePrescribeSvc_ePrescribeSvc"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://www.salesforce.com/services/Soap/u/12.0")]
        public string eRxWeb_sforce_SforceService {
            get {
                return ((string)(this["eRxWeb_sforce_SforceService"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://www.salesforce.com/services/Soap/u/12.0")]
        public string eRxWeb_sforce_SforceService1 {
            get {
                return ((string)(this["eRxWeb_sforce_SforceService1"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://portal.care180.com:443/care360-services/UserSummaryWebService")]
        public string eRxWeb_Care360ws_UserSummaryWebService {
            get {
                return ((string)(this["eRxWeb_Care360ws_UserSummaryWebService"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://localhost:3245/ePrescribeSvc.asmx")]
        public string eRxWeb_ePrescribeSvc2_ePrescribeSvc {
            get {
                return ((string)(this["eRxWeb_ePrescribeSvc2_ePrescribeSvc"]));
            }
        }
    }
}
