using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using Allscripts.Impact;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.AppCode.Interfaces;
using ServiceStack;
using RxUser = eRxWeb.ePrescribeSvc.RxUser;

namespace eRxWeb
{
    /// <summary>
    /// Summary description for SPI
    /// </summary>
    public class SPI : ISPI
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();
        private string sFirstName = String.Empty;
        private string sLastName = String.Empty;
        private string sAddressLine1 = String.Empty;
        private string sAddressLine2 = String.Empty;
        private string sCity = String.Empty;
        private string sState = String.Empty;
        private string sZipCode = String.Empty;
        private string sEmail = String.Empty;
        private string sSitePhone = String.Empty;
        private string sSiteFax = String.Empty;
        private string sOfficeTitle = String.Empty;
        private string sDEANo = String.Empty;
        private string sNPI = String.Empty;
        private bool sIsActive = false;

        public void GetSpi(RxUser user, Guid currentUserGuid, bool enableRenewals, int siteId)
        {
            if (user != null)
            {
                ePrescribeSvc.LicenseSite licenseSite = EPSBroker.GetLicenseSiteByID(user.LicenseID, siteId);

                sFirstName = user.FirstName;
                sLastName = user.LastName;
                sAddressLine1 = licenseSite.Address1;
                sAddressLine2 = licenseSite.Address2;
                sCity = licenseSite.City;
                sState = licenseSite.State;
                sZipCode = licenseSite.ZIPCode;
                sEmail = user.Email;
                sSitePhone = licenseSite.PhoneAreaCode + licenseSite.PhoneNumber;
                sSiteFax = licenseSite.FaxAreaCode + licenseSite.FaxNumber;
                sOfficeTitle = licenseSite.SiteName;
                sIsActive = user.Active;

                if (user.DEALicenses != null && user.DEALicenses.Length > 0)
                    sDEANo = user.DEALicenses[0].DEANumber;

                sNPI = user.NPI;

                string LogMessage;
                bool epcsInEprescribe = false;

                //only enable for EPCS service level if the license is Shield enabled and the user has the EPCS permission
                if (user.AppPermissions != null)
                {
                    if (user.AppPermissions.Any(x => x.Name == Constants.UserPermissionNames.EPCS_CAN_PRESCRIBE))
                    {
                        epcsInEprescribe = true;
                    }
                }

                try
                {
                    ePrescribeSvc.EnrollProviderResponse response = EPSBroker.EnrollProvider(sFirstName, sLastName, sIsActive, sAddressLine1, sAddressLine2, sCity, sState, sZipCode, sEmail,
                                                                                                sSitePhone, sSiteFax, sOfficeTitle, sDEANo, sNPI, user.UserID, licenseSite.AccountID,
                                                                                                enableRenewals, epcsInEprescribe, (ConnectionStringPointer)user.DBID, user.LicenseID);

                    logger.Debug("GetSPI() - Enroll Response {0}", response.ToLogString());

                    if (!response.Success)
                        throw new Exception(response.Messages[0].ToString());

                    string result = response.ResponseXML;

                    result = result.Replace("\"", "'");
                    DataSet xDS = new DataSet("WSResponse");
                    //SPI oRxNow = new SPI();
                    try
                    {
                        StringReader oReader = new StringReader(result);
                        xDS.ReadXml(oReader);
                    }
                    catch (Exception ex)
                    {
                        LogMessage = ex.ToString();
                        //Log.AddLogEntry(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("Error reading XML into a Dataset ex.Message: {0}", ex.Message));
                    }
                    string LastTable = string.Empty;

                    foreach (DataTable t in xDS.Tables)
                    {
                        switch (t.TableName.ToString())
                        {
                            case "Error":
                                //Log.AddLogEntry(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("Error with {0} - {1} - {2}", LastTable, t.Rows[0]["Code"].ToString(), t.Rows[0]["Description"].ToString()));
                                break;
                            case "PerSe":
                                try
                                {
                                    DataSet ds = new DataSet();
                                    DataRow dr;
                                    DataTable dt = new DataTable("Enrollments");
                                    ds.Tables.Add(dt);

                                    ds.Tables["Enrollments"].Columns.Add("PrescriberID");
                                    ds.Tables["Enrollments"].Columns.Add("ScriptSwId");
                                    ds.Tables["Enrollments"].Columns.Add("SenderId");

                                    dr = dt.NewRow();
                                    dr["PrescriberID"] = user.UserID;
                                    dr["ScriptSwId"] = "SURESCRIPTS";
                                    dr["SenderId"] = t.Rows[0]["SPI"].ToString();

                                    //dr.AcceptChanges();
                                    dt.Rows.Add(dr);
                                    dt.AcceptChanges();
                                    ds.AcceptChanges();

                                    Provider.SaveEnrollments(user.UserID, ds, user.LicenseID, currentUserGuid.ToString(), (ConnectionStringPointer)user.DBID);
                                }
                                catch (Exception ex)
                                {
                                    //CREATE AJAX NOTICE
                                    CreateAJAXNotice(user.LicenseID, user.UserID, sFirstName, sLastName, (ConnectionStringPointer)user.DBID);

                                    Audit.AddException(currentUserGuid.ToString(), user.LicenseID, "Exception calling Enrollment web service: " + ex.ToString() + " XML response: " + result, "", "", "", (ConnectionStringPointer)user.DBID);
                                }
                                break;
                            //case "RxHub":
                            //   try
                            //   {
                            //      oRxNow.AddPrescriberToSwitch(UserID, t.Rows[0]["SPI"].ToString(), "RXHUB");
                            //   }
                            //   catch (Exception ex)
                            //   {
                            //      //Log.AddLogEntry(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("Blank? RxHub Error - {0}", ex.Message));
                            //   }
                            //   break;
                            case "Allscripts":
                                break;
                            default:
                                break;
                        }
                        LastTable = t.TableName.ToString();
                    }
                }
                catch (Exception e)
                {
                    //ok, so the Enroll ws call failed...what to do?
                    CreateAJAXNotice(user.LicenseID, user.UserID, sFirstName, sLastName, (ConnectionStringPointer)user.DBID);

                    Audit.AddException(currentUserGuid.ToString(), user.LicenseID, "Exception calling Enrollment web service: " + e.ToString(), "", "", "", (ConnectionStringPointer)user.DBID);

                }
            }
        }

        public void GetSPI(string userId, string licenseID, string sCurrentUserID, bool enableRenewals, int siteID, ConnectionStringPointer dbID)
        {
            ePrescribeSvc.GetUserResponse getUserRes = EPSBroker.GetRxUser(
                ePrescribeSvc.ValueType.UserGUID,
                userId,
                licenseID,
                sCurrentUserID,
                licenseID,
                dbID);

            GetSpi(getUserRes.RxUser, sCurrentUserID.ToGuidOr0x0(), enableRenewals, siteID);
        }

        internal void CreateAJAXNotice(string LicenseID, string UserID, string UserFirstName, string UserLastName, ConnectionStringPointer dbID)
        {
            //first create the "user request" 
            string message = ConfigurationManager.AppSettings["PerSe_SPI_FailMessage"].ToString();
            message = string.Format(message, string.Format("{0} {1}", sFirstName, sLastName));
            string reqid;
            reqid = Allscripts.Impact.SystemConfig.AddUserRequest(LicenseID, UserID, Constants.RequestType.REGISTRATION, message, dbID);
            //now create the "mail message" to indicate there's unread mail 
            Allscripts.Impact.SystemConfig.UpdateUserRequest(reqid, UserID, Constants.ResponseType.PUBLIC, "",
                                           Constants.RequestStatus.UNREAD, dbID);
        }

        public string TruncateString(string StringToTruncate)
        {
            int MaxSize = 35;
            int Size = StringToTruncate.Length > MaxSize ? MaxSize : StringToTruncate.Length;
            return StringToTruncate.Substring(0, Size);
        }

        public string RetrieveSpiForSession(Guid userId, ConnectionStringPointer dbId)
        {
            var dsSPI = Provider.GetSPI(userId.ToString(), dbId);
            if (dsSPI.Tables.Count > 0)
            {
                DataRow[] drSPI = dsSPI.Tables[0].Select("ScriptSwId='SURESCRIPTS'");
                //should only be one row for SURESCRIPTS...grab the first and only
                if (drSPI.Length > 0 && drSPI[0] != null && drSPI[0]["SenderId"] != DBNull.Value && drSPI[0]["SenderId"].ToString() != "")
                {
                    return drSPI[0]["SenderID"].ToString();
                }
            }

            return null;
        }
    }

}