using System;

namespace eRxWeb.State
{
    public interface IStateContainer
    {
        
        object this[string name] { get; set; }
        void Remove(string name);
        void Clear();
        int Timeout { get; set; }
        string GetString(string keyName, string defaultValue);
        bool GetBoolean(string keyName, bool defaultValue);
        bool? GetBooleanOrNull(string keyName);
        bool ContainsKey(string keyName);
        string GetStringOrEmpty(string keyName);
        bool GetBooleanOrFalse(string keyName);
        bool EqualsToEnum<T>(string keyName, T enumVar);
        T Cast<T>(string keyName, T defaultValue);
        DateTime GetDateTimeOrMin(string keyName);
        DateTime GetDateTimeOrNow(string keyName);
        Guid GetGuidOr0x0(string keyName);
        int GetInt(string keyName, int defaultValue);
        long GetLong(string keyName, long defaultValue);
        string GetSessionContentString();
        void Abandon();
        int Count { get; }
    }
}