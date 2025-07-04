using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;
using System.Xml;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact.Interfaces;
using System.Collections.Concurrent;
namespace Allscripts.Impact
{
    
    public enum InstallableProducts
    {
        PrintServer = 0
    }

    public class SystemConfig : ISystemConfig
    {
        static ConcurrentDictionary<string,string> helpPages;
        private static Hashtable _systemAttributes = new Hashtable();
        private static Hashtable _importantInfo = new Hashtable();
        private static Hashtable _statePrintFormats = new Hashtable();
        private static Hashtable _stateSecurityPaper = new Hashtable();
        private static Hashtable _appInfo = new Hashtable();
        private static DateTime _nextIIPull = DateTime.UtcNow;
        private static DateTime _nextSFPPull = DateTime.UtcNow;
        private static int _iiPullInterval = 30;
    
        public static string GetSystemAttribute(string attribute, string licenseID, string userID, ConnectionStringPointer dbID)
        {
            object ret = null;
            if (!_systemAttributes.Contains(attribute))
            {
                ret = ePrescribe.Data.SystemConfig.GetSystemAttribute(attribute, dbID);
                if (ret != null)
                {
                    _systemAttributes.Add(attribute, ret.ToString());
                }
            }
            else
            {
                ret = _systemAttributes[attribute];
            }

            return ret != null ? ret.ToString() : null;
        }

        public static void GetStateList(DropDownList ddlState)
        {
            DataTable dtLisState = RxUser.ChGetState(ConnectionStringPointer.ERXDB_DEFAULT);
            ddlState.DataSource = dtLisState;
            ddlState.DataTextField = "State";
            ddlState.DataValueField = "State";
           
            ddlState.DataBind();
        }

        public static DataRowCollection GetImportantInfo(bool showOnRightPanel, bool showOnHomepage, bool showOnInventoryView, ConnectionStringPointer dbID)
        {
            DataRowCollection ret = null;
            int key = 0;
            key = key | (showOnRightPanel ? 0x1 : 0);
            key = key | (showOnHomepage ? 0x2 : 0);
            key = key | (showOnInventoryView ? 0x4 : 0);
            key = key | Convert.ToInt32(dbID) * 100;


            DateTime now = DateTime.UtcNow;
            if (_importantInfo.Contains(key) && now < _nextIIPull)
            {
                ret = _importantInfo[key] as DataRowCollection;
            }
            else
            {
                if (now >= _nextIIPull)
                {
                    _nextIIPull = now.AddMinutes(_iiPullInterval);
                    _importantInfo.Clear();
                }
                ret = Allscripts.ePrescribe.Data.SystemConfig.GetImportantInfo(
                        showOnRightPanel ? 1 : 0,
                        showOnHomepage ? 1 : 0,
                        showOnInventoryView ? 1 : 0,
                        dbID);
                _importantInfo.Add(key, ret);
            }

            return ret;
        }

        public static bool AllowsMultipleFormats(int StateFormats, bool bAllowFourUp)
        {
            return (StateFormats == 3 && bAllowFourUp) || (StateFormats == 6 && bAllowFourUp) || StateFormats == 5 || StateFormats == 7;
        }

        public static bool AllowPrintFormat(PrintFormats format, int StateFormats)
        {
            return (StateFormats & Convert.ToInt32(format)) > 0;
        }

        public static bool AllowPrintFormat(PrintFormats format, string State, ConnectionStringPointer dbID)
        {
            int stateFormats = GetStatePrintFormats(State);
            return (stateFormats & Convert.ToInt32(format)) > 0;
        }

        public static bool AllowPrintFormat(Constants.PrintFormats format, int stateFormats)
        {
            return (stateFormats & Convert.ToInt32(format)) > 0;
        }

        public static bool AllowPrintFormat(Constants.PrintFormats format, string state, ConnectionStringPointer dbID)
        {
            int stateFormats = GetStatePrintFormats(state);
            return (stateFormats & Convert.ToInt32(format)) > 0;
        }

        public static void ResetStates()
        {
            _statePrintFormats.Clear();
            _stateSecurityPaper.Clear();
        }

        public static void ResetEULA()
        {
            ePrescribeSvc.ePrescribeSvc eps = new ePrescribeSvc.ePrescribeSvc();
            eps.Url = System.Configuration.ConfigurationManager.AppSettings["ePrescribeSvc.eprescribesvc"].ToString();
            ePrescribeSvc.CacheResetRequest cacheResetReq = new ePrescribeSvc.CacheResetRequest();
            cacheResetReq.ItemResetID = "EULA";
            eps.CacheReset(cacheResetReq);
        }

        public static void ResetAppVersion()
        {
            _appInfo.Clear();
        }

        public static int GetStatePrintFormats(string state)
        {
            int ret = 1;
            if (_statePrintFormats.Contains(state))
            {
                ret = Convert.ToInt32(_statePrintFormats[state]);
            }
            else
            {
                object statePrintFormat = ePrescribe.Data.License.LoadStatePrintFormats(state, ConnectionStringPointer.SHARED_DB);
                if (statePrintFormat != null && !int.TryParse(statePrintFormat.ToString(), out ret))
                {
                    ret = 1;
                }

                if (!_statePrintFormats.Contains(state))
                {
                    _statePrintFormats.Add(state, ret);
                }
                else
                {
                    _statePrintFormats[state] = ret;
                }
            }

            return ret;
        }

        public static void ResetHelp()
        {
            helpPages = null;
        }

        public static void ResetImportantInfo()
        {
            _importantInfo.Clear();
        }

        public static string GetHelp(string pageName, ConnectionStringPointer dbID)
        {
            
                if (helpPages != null && helpPages.ContainsKey(pageName))
                {
                    return helpPages[pageName].ToString();
                }
                else
                {
                    
                    object value = ePrescribe.Data.SystemConfig.GetHelp(pageName, ConnectionStringPointer.SHARED_DB);
                    string helpText = string.Empty;
                    if (value != null)
                        helpText = value.ToString();
                    if (helpPages == null)
                    {
                        helpPages = new ConcurrentDictionary<string, string>();
                    }
                    if (!string.IsNullOrWhiteSpace(helpText))
                    {
                        if (helpText.EndsWith("Tutorial:") || helpText.EndsWith("Tutorials:"))
                        {
                            helpText = helpText + "&nbsp;<A href=\"/media/tutorials/FullVideo/curriculum/1.html\" target=_blank>Tutorials</A>&nbsp;";
                            helpPages.TryAdd(pageName, helpText);
                        }
                        else
                        {
                            helpPages.TryAdd(pageName, helpText);
                        }
                    }
                    return helpText;
                }
            }
 
        public static DataSet GetHelpFAQ(int Top, string Active, string Category)
        {
            return Allscripts.ePrescribe.Data.SystemConfig.GetHelpFAQ(Top, Active, Category, ConnectionStringPointer.SHARED_DB);
        }

        public static bool CheckSecurityPaper(string state)
        {
            bool ret = false;
            if (_stateSecurityPaper.Contains(state))
            {
                ret = Convert.ToBoolean(_stateSecurityPaper[state]);
            }
            else
            {
                object needsPaper = ePrescribe.Data.License.SiteNeedsSecurityPaper(state, ConnectionStringPointer.SHARED_DB);
                if (needsPaper != null)
                {
                    ret = needsPaper.ToString().Equals("Y", StringComparison.OrdinalIgnoreCase);
                }

                if (!_stateSecurityPaper.Contains(state))
                {
                    _stateSecurityPaper.Add(state, ret);
                }
            }

            return ret;
        }

        public static string AddUserRequest(string licenseID, string userID, Constants.RequestType requestType, string requestText, ConnectionStringPointer dbID)
        {            
            object ret = ePrescribe.Data.SystemConfig.AddUserRequest(licenseID, userID, Convert.ToInt32(requestType), requestText, dbID);

            return ret.ToString();
        }

        public static void UpdateUserRequest(string requestID, string responseUserID, Constants.ResponseType responseType, string responseText, Constants.RequestStatus status, ConnectionStringPointer dbID)
        {
            ePrescribe.Data.SystemConfig.UpdateUserRequest(requestID, responseUserID, responseType, responseText, status, dbID);
        }

        public static void UpdateUserRequest(string requestID, Constants.RequestStatus status, ConnectionStringPointer dbID)
        {
            ePrescribe.Data.SystemConfig.UpdateUserRequest(requestID, status, dbID);
        }

        public static DataTable GetUserRequests(string licenseID, string userID, Constants.RequestType requestType, Constants.ResponseType responseType, Constants.RequestStatus status, ConnectionStringPointer dbID)
        {
            return ePrescribe.Data.SystemConfig.GetUserRequests(licenseID, userID, requestType, responseType, status, dbID);
        }

        public static DataSet GetUpdates(InstallableProducts product, ConnectionStringPointer dbID)
        {
            return ePrescribe.Data.SystemConfig.GetUpdates(Convert.ToInt32(product), dbID);
        }

        public static int GetFailedRxCountForLicense(string licenseID, ConnectionStringPointer dbID)
        {
            return Convert.ToInt32(ePrescribe.Data.SystemConfig.GetFailedRxCountForLicense(licenseID, dbID));
        }

        public static int GetFailedRxCountForLicenseSite(string licenseID, int siteID, ConnectionStringPointer dbID)
        {
            return Convert.ToInt32(ePrescribe.Data.SystemConfig.GetFailedRxCountForLicenseSite(licenseID, siteID, dbID));
        }

        public static int GetDeniedRefReqCountForLicense(string licenseID, ConnectionStringPointer dbID)
        {
            return Convert.ToInt32(ePrescribe.Data.SystemConfig.GetDeniedRefReqCountForLicense(licenseID, dbID));
        }

        public static DataTable GetFailedRegistrationMessages(string licenseID, ConnectionStringPointer dbID)
        {
            return ePrescribe.Data.SystemConfig.GetFailedRegistrationsForLicense(licenseID, dbID);
        }

        public static DataSet GetRefRefDeniesAndError(string licenseID, string userID, ConnectionStringPointer dbID)
        {
            DataSet ds = new DataSet();
            DataTable dtDeniedRefReqs = new DataTable();
            dtDeniedRefReqs.TableName = "DeniedRefReqs";

            dtDeniedRefReqs.Columns.Add(new DataColumn("RequestID", typeof(string)));
            dtDeniedRefReqs.Columns.Add(new DataColumn("PatientName", typeof(string)));
            dtDeniedRefReqs.Columns.Add(new DataColumn("PatientDOB", typeof(string)));
            dtDeniedRefReqs.Columns.Add(new DataColumn("DrugDescription", typeof(string)));
            dtDeniedRefReqs.Columns.Add(new DataColumn("DaysSupply", typeof(string)));
            dtDeniedRefReqs.Columns.Add(new DataColumn("Refills", typeof(string)));
            dtDeniedRefReqs.Columns.Add(new DataColumn("PharmacyName", typeof(string)));
            dtDeniedRefReqs.Columns.Add(new DataColumn("PharmacyAddress", typeof(string)));
            dtDeniedRefReqs.Columns.Add(new DataColumn("PharmacyPhone", typeof(string)));
            dtDeniedRefReqs.Columns.Add(new DataColumn("PharmacyFax", typeof(string)));

            DataTable refreqDeniedDT = GetUserRequests(licenseID, null, Constants.RequestType.REFREQ_DENY_SCHEDULED, Constants.ResponseType.PUBLIC, Constants.RequestStatus.UNREAD, dbID);
            if (refreqDeniedDT.Rows.Count > 0)
            {
                DataRow drDeniedRefReqs;
                foreach (DataRow dr in refreqDeniedDT.Rows)
                {
                    ScriptMessage refreqMsg = new ScriptMessage(dr["RequestText"].ToString(), licenseID, userID, dbID);

                    drDeniedRefReqs = dtDeniedRefReqs.NewRow();
                    drDeniedRefReqs["RequestID"] = dr["RequestID"].ToString();
                    drDeniedRefReqs["PatientName"] = refreqMsg.PatientFirstName + " " + refreqMsg.PatientLastName;
                    drDeniedRefReqs["PatientDOB"] = refreqMsg.PatientDOB;
                    drDeniedRefReqs["DrugDescription"] = refreqMsg.DBDrugDescription + ", Qty: " + refreqMsg.RxQuantity + ", " + refreqMsg.RxSIGText;
                    drDeniedRefReqs["Refills"] = refreqMsg.RxRefills;
                    drDeniedRefReqs["PharmacyName"] = refreqMsg.PharmacyName;
                    drDeniedRefReqs["PharmacyAddress"] = refreqMsg.PharmacyAddress1 + ", " + refreqMsg.PharmacyCity + ", " + refreqMsg.PharmacyState + " " + refreqMsg.PharmacyZip;
                    drDeniedRefReqs["PharmacyPhone"] = refreqMsg.PharmacyPhoneNumber;
                    drDeniedRefReqs["PharmacyFax"] = refreqMsg.PharmacyFaxNumber;

                    dtDeniedRefReqs.Rows.Add(drDeniedRefReqs);
                }
            }

            DataTable dtErrorRefReqs = new DataTable();
            dtErrorRefReqs.TableName = "ErrorRefReqs";

            dtErrorRefReqs.Columns.Add(new DataColumn("RequestID", typeof(string)));
            dtErrorRefReqs.Columns.Add(new DataColumn("PatientName", typeof(string)));
            dtErrorRefReqs.Columns.Add(new DataColumn("PatientDOB", typeof(string)));
            dtErrorRefReqs.Columns.Add(new DataColumn("DrugDescription", typeof(string)));
            dtErrorRefReqs.Columns.Add(new DataColumn("DaysSupply", typeof(string)));
            dtErrorRefReqs.Columns.Add(new DataColumn("Refills", typeof(string)));
            dtErrorRefReqs.Columns.Add(new DataColumn("PharmacyName", typeof(string)));
            dtErrorRefReqs.Columns.Add(new DataColumn("PharmacyAddress", typeof(string)));
            dtErrorRefReqs.Columns.Add(new DataColumn("PharmacyPhone", typeof(string)));
            dtErrorRefReqs.Columns.Add(new DataColumn("PharmacyFax", typeof(string)));

            DataTable refreqErrorDT = SystemConfig.GetUserRequests(licenseID, null, Constants.RequestType.AUTODENY_ERROR, Constants.ResponseType.PUBLIC, Constants.RequestStatus.UNREAD, dbID);
            DataRow drRefReqsErrors;

            foreach (DataRow dr in refreqErrorDT.Rows)
            {
                ScriptMessage refreqMsg = new ScriptMessage(dr["RequestText"].ToString(), licenseID, userID, dbID);

                drRefReqsErrors = dtErrorRefReqs.NewRow();
                drRefReqsErrors["RequestID"] = dr["RequestID"].ToString();
                drRefReqsErrors["PatientName"] = refreqMsg.PatientFirstName + " " + refreqMsg.PatientLastName;
                drRefReqsErrors["PatientDOB"] = refreqMsg.PatientDOB;
                drRefReqsErrors["DrugDescription"] = refreqMsg.DBDrugDescription + ", Qty: " + refreqMsg.RxQuantity + ", " + refreqMsg.RxSIGText;
                drRefReqsErrors["Refills"] = refreqMsg.RxRefills;
                drRefReqsErrors["PharmacyName"] = refreqMsg.PharmacyName;
                drRefReqsErrors["PharmacyAddress"] = refreqMsg.PharmacyAddress1 + ", " + refreqMsg.PharmacyCity + ", " + refreqMsg.PharmacyState + " " + refreqMsg.PharmacyZip;
                drRefReqsErrors["PharmacyPhone"] = refreqMsg.PharmacyPhoneNumber;
                drRefReqsErrors["PharmacyFax"] = refreqMsg.PharmacyFaxNumber;

                dtErrorRefReqs.Rows.Add(drRefReqsErrors);
            }

            ds.Tables.Add(dtDeniedRefReqs);
            ds.Tables.Add(dtErrorRefReqs);

            return ds;
        }

        public static string GetSelfMPLURL(string LicenseID, string UserID, ConnectionStringPointer dbID)
        {
            string accountID = string.Empty;
            string siteName = string.Empty;
            string siteAddress1 = string.Empty;
            string siteAddress2 = string.Empty;
            string siteCity = string.Empty;
            string siteState = string.Empty;
            string siteZip = string.Empty;
            string firstName = string.Empty;
            string lastName = string.Empty;
            string title = string.Empty;
            string phone = string.Empty;
            string email = string.Empty;

            if (LicenseID != null)
            {
                DataTable dtSiteInfo = ApplicationLicense.Load(LicenseID, dbID).Tables[0];
                accountID = dtSiteInfo.Rows[0]["AccountID"].ToString();
                siteName = dtSiteInfo.Rows[0]["PracticeName"].ToString();
                siteAddress1 = dtSiteInfo.Rows[0]["Address1"].ToString();
                siteAddress2 = dtSiteInfo.Rows[0]["Address2"].ToString();
                siteCity = dtSiteInfo.Rows[0]["City"].ToString();
                siteState = dtSiteInfo.Rows[0]["State"].ToString();
                siteZip = dtSiteInfo.Rows[0]["ZipCode"].ToString();
            }

            if (UserID != null)
            {
                DataTable dtUserInfo = RxUser.Load(UserID, dbID).Tables["User"];
                firstName = dtUserInfo.Rows[0]["FirstName"].ToString();
                lastName = dtUserInfo.Rows[0]["LastName"].ToString();
                title = dtUserInfo.Rows[0]["Title"].ToString();
                email = dtUserInfo.Rows[0]["Email"].ToString();
            }

            //don't save this request for the Keynote test license
            if (!LicenseID.Equals("E28C7525-DF99-4DCD-8E22-1DBA70FBC29A", StringComparison.OrdinalIgnoreCase))
            {
                SaveInterfaceRequest(
                    LicenseID,// Parameter name is UserGUID, but we need to pass LicenseID as UserGUID to Hyperbridge API
                    accountID,
                    siteName,
                    siteAddress1,
                    siteAddress2,
                    siteCity,
                    siteState,
                    siteZip,
                    firstName,
                    lastName,
                    title,
                    phone,
                    email,
                    "MPL",
                    string.Empty,
                    string.Empty,
                    -1,
                    LicenseID,
                    ConnectionStringPointer.REGISTRATION_DB,
                    dbID);
            }

            string result = string.Empty;

            try
            {
                PostSubmitter post = new PostSubmitter();
                post.Url = ConfigurationManager.AppSettings["HilgraeveInterfaceURL"].ToString();

                post.PostItems.Add("UserGUID", LicenseID);// As per Hyperbridge we need to pass LicenseID as UserGUID
                post.PostItems.Add("AccountId", accountID);
                post.PostItems.Add("SiteName", siteName);
                post.PostItems.Add("Address1", siteAddress1);
                post.PostItems.Add("Address2", siteAddress2);
                post.PostItems.Add("City", siteCity);
                post.PostItems.Add("State", siteState);
                post.PostItems.Add("ZipCode", siteZip);
                post.PostItems.Add("firstname", firstName);
                post.PostItems.Add("lastname", lastName);
                post.PostItems.Add("phone", phone);
                post.PostItems.Add("email", email);
                post.PostItems.Add("pms", string.Empty);
                post.PostItems.Add("version", string.Empty);
                post.PostItems.Add("interfaceType", "MPL");
                post.PostItems.Add("date", string.Empty);
                post.PostItems.Add("time", string.Empty);

                post.Type = PostSubmitter.PostTypeEnum.Post;
                result = post.PostAndReturnURIForSelfMPL();
            }
            catch (Exception)
            {
                result = string.Empty;
            }

            return result;
        }

        public static void SaveInterfaceRequest(string userGUID, string accountID, string siteName, string siteAddress1, string siteAddress2, string siteCity,
            string siteState, string siteZipCode, string submitterFirstName, string submitterLastName, string submitterTitle, string submitterPhone,
            string submitterEmail, string pmsSystemName, string pmsVersion, string pmsAccountID, int interfaceID, string licenseID,
            ConnectionStringPointer regDBID, ConnectionStringPointer licenseDBID)
        {
            ePrescribe.Data.License.SaveLicenseInterface(licenseID, interfaceID, licenseDBID);
            ePrescribe.Data.SystemConfig.SaveInterfaceRequest(userGUID, accountID, siteName, siteAddress1, siteAddress2, siteCity, siteState, siteZipCode, submitterFirstName, submitterLastName, submitterTitle, submitterPhone, submitterEmail, pmsSystemName, pmsVersion, pmsAccountID, regDBID);
        }

        public static DataTable GetInterfaceSystems(string active, string free)
        {
            return (ePrescribe.Data.SystemConfig.GetInterfaceSystems(active, free, ConnectionStringPointer.SHARED_DB) as DataSet).Tables[0];
        }
        public static DataTable GetInterfacePriceList()
        {
            return (ePrescribe.Data.SystemConfig.GetInterfacePriceList(ConnectionStringPointer.SHARED_DB) as DataSet).Tables[0];
        }
        public static DataTable GetInterfaceSystemByID(int interfaceID)
        {
            return (ePrescribe.Data.SystemConfig.GetInterfaceSystemByID(interfaceID, ConnectionStringPointer.SHARED_DB) as DataSet).Tables[0];
        }

        public static DataTable PartnerLicenseSiteCheck(string licenseID, string partnerID, ConnectionStringPointer dbID)
        {
            return (ePrescribe.Data.SystemConfig.PartnerLicenseSiteCheck(licenseID, partnerID, dbID) as DataSet).Tables[0];
        }

        public static string GetFullApplicationVersion(int applicationID)
        {
            string ret = string.Empty;
            if (_appInfo.Contains(applicationID))
            {
                DataRow dr = (DataRow)_appInfo[applicationID];
                string majorVer = dr["MajorVersion"].ToString();
                string majorRev = dr["MajorRevision"].ToString();
                string minorVer = dr["MinorVersion"].ToString();
                string minorRev = dr["MinorRevision"].ToString();
                ret = majorVer + "." + majorRev + "." + minorVer + "." + minorRev;
            }
            else
            {
                DataTable dt = ePrescribe.Data.SystemConfig.GetApplicationVersion(applicationID, ConnectionStringPointer.SHARED_DB);
                if (dt != null && dt.Rows.Count == 1)
                {
                    if (_appInfo.Contains(applicationID))
                    {
                        _appInfo[applicationID] = dt.Rows[0];
                    }
                    else
                    {
                        _appInfo.Add(applicationID, dt.Rows[0]);
                    }
                    string majorVer = dt.Rows[0]["MajorVersion"].ToString();
                    string majorRev = dt.Rows[0]["MajorRevision"].ToString();
                    string minorVer = dt.Rows[0]["MinorVersion"].ToString();
                    string minorRev = dt.Rows[0]["MinorRevision"].ToString();
                    ret = majorVer + "." + majorRev + "." + minorVer + "." + minorRev;
                }
            }
            return ret;
        }

        public static DataTable GetTimeZones(ConnectionStringPointer dbID)
        {
            return ePrescribe.Data.SystemConfig.GetTimeZones(dbID);
        }

		/// <summary>
		/// Converts a UTC time into the local time of the passed in time zone
		/// </summary>
		/// <param name="timeZone">String of the time zone key. 
		///		Valid values include:
		///		Alaskan Standard Time, Atlantic Standard Time, Central Standard Time, West Pacific Standard Time, Eastern Standard Time
		///		Hawaiian Standard Time, Mountain Standard Time, Pacific Standard Time
		/// </param>
		/// <param name="utcTimeToConvert">String representation of the time to convert in UTC</param>
		/// <returns>DateTime of the local time of the passed in time zone</returns>
		public static DateTime GetLocalTime(string timeZone, string utcTimeToConvert)
		{
			return GetLocalTime(timeZone, DateTime.Parse(utcTimeToConvert));
		}

		/// <summary>
		/// Converts a UTC time into the local time of the passed in time zone
		/// </summary>
		/// <param name="timeZone">String of the time zone key. 
		///		Valid values include:
		///		Alaskan Standard Time, Atlantic Standard Time, Central Standard Time, West Pacific Standard Time, Eastern Standard Time
		///		Hawaiian Standard Time, Mountain Standard Time, Pacific Standard Time
		/// </param>
		/// <param name="utcTimeToConvert">Time to convert in UTC</param>
		/// <returns>DateTime of the local time of the passed in time zone</returns>
		public static DateTime GetLocalTime(string timeZone, DateTime utcTimeToConvert)
		{
			DateTime localTime = utcTimeToConvert;

			try
			{
				//
				// ID from "HKEY_LOCAL_MACHINE\Software\Microsoft\Windows NT\CurrentVersion\Time Zone"
				// See http://msdn.microsoft.com/en-us/library/system.timezoneinfo.id.aspx
				//
				TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
				localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTimeToConvert, timeZoneInfo);
			}
			catch
			{
				//just keep it in UTC and don't fail the user
				localTime = utcTimeToConvert;
			}

			return localTime;
		}

        public static DataTable GetWebConfigKeys(int applicationID, ConnectionStringPointer dbID)
        {
            return ePrescribe.Data.SystemConfig.GetWebConfigKeys(applicationID, dbID);
        }

        public string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
