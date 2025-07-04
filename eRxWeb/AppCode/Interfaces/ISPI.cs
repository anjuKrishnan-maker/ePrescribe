using System;
using Allscripts.ePrescribe.DatabaseSelector;

namespace eRxWeb.AppCode.Interfaces
{
    public interface ISPI
    {
        string RetrieveSpiForSession(Guid userId, ConnectionStringPointer dbId);
    }
}