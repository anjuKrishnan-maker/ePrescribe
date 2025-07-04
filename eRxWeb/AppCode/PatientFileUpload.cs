using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using eRxWeb.AppCode;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Net;
using eRxWeb.State;
using Allscripts.ePrescribe.Common;
using System.Reflection;
using System.Linq.Expressions;
using Allscripts.ePrescribe.ExtensionMethods;
using static Allscripts.Impact.PDI.PDI_ImportStagingPatient;
using System.Web;
using Allscripts.Impact.PDI;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact.Interfaces;
using System.Data;
using eRxWeb.AppCode.Interfaces;
using Allscripts.Impact;
using System.Threading;
using Allscripts.ePrescribe.Shared.Logging;
using System.Collections;
using System.Web.Caching;
using System.Runtime.Remoting.Contexts;

namespace eRxWeb.ServerModel
{
    public static class PatientFileUpload
    {

        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        public static bool IsShowPatientUpload(IStateContainer session)
        {
            var sessionLicense = session.Cast(Constants.SessionVariables.SessionLicense,
                                              default(ApplicationLicense));

            bool isShowPatientPreferenceEnabled = session.GetBooleanOrFalse(Constants.LicensePreferences.PatientUpload);


            return (sessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.On &&
                Allscripts.Impact.ConfigKeys.PatientDemographicUpload && isShowPatientPreferenceEnabled);
        }

        #region SavePDIData

        internal static PatientUploadResponse SavePatientFileData(string patientData, IStateContainer _session)
        {
            var dbid = _session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
            var licenseID = _session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
            var response = new PatientUploadResponse();
            try
            {
                //Save to stage table
                response.CurrentJob = SavePatientData(patientData, _session, new PDI_ImportStagingPatient());

                //Invoke job processing if valid
                if (response.CurrentJob.ID > 0)
                {
                    InvokePatientProcessingJob(response.CurrentJob.ID, _session, new PDI_ImportBatch());
                }

                //Set OK response 
                response.StatusCode = HttpStatusCode.OK;
                response.UploadSuccess = true;
            }
            catch (Exception ex)
            {
                Audit.AddException(Guid.Empty.ToString(), licenseID, string.Concat("Error saving PDI records: ", ex.ToString()), null, null, null, dbid);
                logger.Error("Error creating or saving PDI records: " + ex.ToString());
                response.ErrorMessage = ex.Message;
                response.UploadSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.CurrentJob = new PDI_ImportBatch
                {
                    Status = Status.FAILED.ToString(),
                    RecordsProcessed = 0,
                    ProcessBegin = DateTime.Now
                };
            }

            return response;
        }

        internal static PDI_ImportBatch SavePatientData(string PatientStreamData, IStateContainer pageState, IPDI_ImportStagingPatient patients)
        {
            List<PDI_ImportStagingPatient> records = new List<PDI_ImportStagingPatient>();
            var licenseId = pageState.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
            var userId = pageState.GetStringOrEmpty(Constants.SessionVariables.UserId);
            var errorLines = new ArrayList();

            try
            {
                var fieldMap = TSVMapping<PDI_ImportStagingPatient>.PropertySetters();
                StringReader sr = new StringReader(DecodePatientData(PatientStreamData));

                using (TextFieldParser txtParser = new TextFieldParser(sr))
                {
                    txtParser.TextFieldType = FieldType.Delimited;
                    txtParser.Delimiters = new string[] { "\t" };
                    txtParser.HasFieldsEnclosedInQuotes = false;
                    string[] header = txtParser.ReadFields();
                    var headerMap = header.Select((h, i) => new { h, i }).ToDictionary(hi => hi.h.ToUpper(), hi => hi.i);

                    while (!txtParser.EndOfData)
                    {
                        var patientRecord = new PDI_ImportStagingPatient { LicenseID = licenseId, UserID = userId };
                        var lineNumber = txtParser.LineNumber;
                        string[] fields = txtParser.ReadFields();
                        if (fields.Length == headerMap.Count)
                        {
                            foreach (var field in fieldMap.Keys)
                            {
                                if (headerMap.ContainsKey(field) && !string.IsNullOrEmpty(fields[headerMap[field]]))
                                {
                                    fieldMap[field](patientRecord, fields[headerMap[field]]);
                                }
                            }
                        }
                        else
                        {
                            errorLines.Add(lineNumber.ToString());
                        }

                        records.Add(patientRecord);
                    }

                    if (records.Count == 0)
                        throw new Exception("No valid records found. Please ensure file matches example provided.");
                }

                var newJob = new PDI_ImportBatch
                {
                    PatientRecords = records.Where(r => r.IsValid(r)).ToList(),
                    Status = Status.NEW.ToString(),
                    ProcessBegin = DateTime.Now,
                    BatchSize = records.Where(r => r.IsValid(r)).Count(),
                    RecordsFailed = records.Where(r => !r.IsValid(r)).Count(),
                    ErrorLines = string.Join(",", errorLines.ToArray())
                };

                if (records.Count > 0)
                {
                    return SavePatientRecordsToStage(newJob, pageState, patients);
                }
                else
                {
                    return newJob;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static PDI_ImportBatch SavePatientRecordsToStage(PDI_ImportBatch job, IStateContainer pageState, IPDI_ImportStagingPatient patients)
        {
            var dbid = pageState.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
            var licenseID = pageState.GetStringOrEmpty(Constants.SessionVariables.LicenseId);

            try
            {
                job.ID = patients.CreatePatientUploadBatchJob(job, licenseID, dbid);
            }
            catch (Exception)
            {
                throw new Exception("There was an issue saving your patient data");
            }

            return job;
        }

        public static void InvokePatientProcessingJob(int batchImportId, IStateContainer pageState, IPDI_ImportBatch batchJob)
        {

            var dbid = pageState.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
            var licenseId = pageState.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
            var userId = pageState.GetStringOrEmpty(Constants.SessionVariables.UserId);

            batchJob.ProcessPDIBatchJob(batchImportId, licenseId, userId, dbid);
        }

        private static bool IsBase64(this string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

        private static string DecodePatientData(string encodedPatientData)
        {
            string DecodedPatientData = string.Empty;
            try
            {
                encodedPatientData = encodedPatientData.Split(',')[1];

                if (IsBase64(encodedPatientData))
                {
                    byte[] data = Convert.FromBase64String(encodedPatientData);
                    DecodedPatientData = Encoding.UTF8.GetString(data);
                }
            }
            catch (Exception)
            {
                throw new Exception("File contents cannot be empty");
            }

            return DecodedPatientData;
        }

        #endregion

        #region PDIJobStatusReporting

        internal static PatientUploadResponse CheckJobStatus (IStateContainer pageState, IPDI_ImportBatch batches)
        {
            try
            {
                var dbid = pageState.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
                var licenseId = pageState.GetStringOrEmpty(Constants.SessionVariables.LicenseId);

                var batchJobs = batches.CheckForExistingBatchJobs(licenseId, dbid);

                var response = new PatientUploadResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    ImportBatchJobHistory = batchJobs,
                    CurrentJob = batchJobs.Where(j => j.ProcessEnd == null && (j.BatchStatus == Status.NEW || j.BatchStatus == Status.INPROCESS))
                    .OrderByDescending(jD => jD.ProcessBegin)
                    .FirstOrDefault()
                };

                return response;

            }
            catch (Exception ex)
            {
                return new PatientUploadResponse
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessage = ex.Message
                };
            }
        }

        internal static PatientUploadResponse GenerateJobHistoryReport (IStateContainer pageState, PDI_ImportBatch job, IPDI_ImportBatch batches)
        {
            try
            {
                var dbid = pageState.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
                var report = batches.GenerateJobReport(job, dbid);

                return new PatientUploadResponse
                {
                    CurrentJob = report,
                    StatusCode = HttpStatusCode.OK
                };

            }
            catch (Exception ex)
            {
                return new PatientUploadResponse
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessage = ex.Message
                };
            }
        }

        #endregion

        public static class TSVMapping<T>
        {
            public static Dictionary<string, Action<T, object>> PropertySetters()
            {
                var t = typeof(T);
                var propsOrFields = t.GetMembers(BindingFlags.Instance | BindingFlags.Public).Where(m => m.MemberType == MemberTypes.Property || m.MemberType == MemberTypes.Field);

                var ans = new Dictionary<string, Action<T, object>>(StringComparer.OrdinalIgnoreCase);
                foreach (var m in propsOrFields)
                {
                    if (!Attribute.IsDefined(m, typeof(TSVNoColumnName)) && m.GetCanWrite())
                    {
                        var ca = (TSVColumnName)Attribute.GetCustomAttribute(m, typeof(TSVColumnName));
                        var csvname = (ca != null) ? ca.ColumnName : m.Name;
                        var paramobj = Expression.Parameter(t);
                        var paramval = Expression.Parameter(typeof(object));
                        var body = Expression.Assign(Expression.PropertyOrField(paramobj, m.Name), Expression.Convert(paramval, m.GetMemberType()));
                        var setter = Expression.Lambda<Action<T, object>>(body, new[] { paramobj, paramval });
                        ans.Add(csvname, setter.Compile());
                    }
                }
                return ans;
            }
        }
    }
}
