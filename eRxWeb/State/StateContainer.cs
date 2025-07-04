using System;
using System.Data;
using System.Text;
using System.Web.SessionState;

namespace eRxWeb.State
{
    public class StateContainer : IStateContainer
    {
        private readonly HttpSessionState httpSession;

        public StateContainer(HttpSessionState session)
        {
            httpSession = session;
        }

        public object this[string name]
        {
            get { return httpSession[name]; }
            set { httpSession[name] = value; }
        }

        public void Remove(string name)
        {
            httpSession.Remove(name);
        }

        public void Clear()
        {
            httpSession.Clear();
        }

        public int Timeout
        {
            get { return httpSession.Timeout; }
            set { httpSession.Timeout = value; }
        }

        public int Count {
            get { return httpSession.Count; }            
        }

        public string GetString(string keyName, string defaultValue)
        {
            if (httpSession[keyName] == null)
                return defaultValue;

            return httpSession[keyName].ToString();
        }

        public string GetStringOrEmpty(string keyName)
        {
            return GetString(keyName, string.Empty);
        }

        public bool GetBoolean(string keyName, bool defaultValue)
        {

            if (httpSession[keyName] == null)
                return defaultValue;

            bool parsedValue;
            if (bool.TryParse(httpSession[keyName].ToString(), out parsedValue))
            {
                return parsedValue;
            }

            return defaultValue;
        }

        public bool EqualsToEnum<T>(string keyName, T enumValue)
        {
            try
            {
                if (httpSession[keyName] == null)
                    return false;

                var sessionValue = (T)httpSession[keyName];

                return sessionValue.Equals(enumValue);
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }

        public bool? GetBooleanOrNull(string keyName)
        {
            if (httpSession[keyName] == null)
                return null;

            bool parsedValue;
            if (bool.TryParse(httpSession[keyName].ToString(), out parsedValue))
            {
                return parsedValue;
            }

            return null;
        }

        public bool GetBooleanOrFalse(string keyName)
        {
            return GetBoolean(keyName, false);
        }

        public bool ContainsKey(string keyName)
        {
            return httpSession[keyName] != null;
        }

        public T Cast<T>(string keyName, T defaultValue)
        {
            if (httpSession[keyName] == null)
                return defaultValue;
            try
            {
                return (T)httpSession[keyName];
            }
            catch
            {
                return defaultValue;
            }
        }

        public Guid GetGuidOr0x0(string keyName)
        {
            if (httpSession[keyName] == null)
                return Guid.Empty;
            try
            {
                Guid guid;
                if (Guid.TryParse(httpSession[keyName].ToString(), out guid))
                {
                    return guid;   
                }
            }
            catch
            {
                return Guid.Empty;
            }

            return Guid.Empty;
        }

        public DateTime GetDateTimeOrMin(string keyName)
        {
            DateTime parsedTime;
            if (DateTime.TryParse(Convert.ToString(httpSession[keyName]), out parsedTime))
            {
                return parsedTime;
            }

            return DateTime.MinValue;
        }

        public DateTime GetDateTimeOrNow(string keyName)
        {
            DateTime parsedTime;
            if (DateTime.TryParse(Convert.ToString(httpSession[keyName]), out parsedTime))
            {
                return parsedTime;
            }

            return DateTime.Now;
        }

        public int GetInt(string keyName, int defaultValue)
        {
            int val;
            if(int.TryParse(Convert.ToString(httpSession[keyName]), out val))
            {
                return val;
            }

            return defaultValue;
        }

        public long GetLong(string keyName, long defaultValue)
        {
            long val;
            if (long.TryParse(Convert.ToString(httpSession[keyName]), out val))
            {
                return val;
            }

            return defaultValue;
        }
        public string GetSessionContentString()
        {
            var builder = new StringBuilder();
            builder.Append("<sessionContent>");

            for (int i = 0; i < httpSession.Keys.Count; i++)
            {
                var value = httpSession[i];
                string stringValue;
                if (value is DataTable)
                {
                    stringValue = PrintDataTable(value as DataTable);
                }
                else
                {
                    stringValue = System.Security.SecurityElement.Escape(Convert.ToString(value));
                }
                builder.AppendFormat("<{0}>{1}</{0}>", httpSession.Keys[i], stringValue);
            }
            builder.Append("</sessionContent>");

            return builder.ToString();
        }
        private string PrintDataTable(DataTable dataTable)
        {
            if (dataTable == null)
                return string.Empty;

            using (var writer = new System.IO.StringWriter())
            {
                if (string.IsNullOrWhiteSpace(dataTable.TableName))
                    dataTable.TableName = "DataTable";
                dataTable.WriteXml(writer);
                return writer.ToString();
            }
        }

        public void Abandon()
        {
            httpSession.Abandon();
        }
    }
}