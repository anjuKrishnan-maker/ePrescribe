using System.Collections.Generic;

namespace eRxWeb
{
    public interface IMassOpiateMessage
    {
        string GenerateMassOpiateMessage(string controlledSubstanceCode, string previousNotes, string state, string GPI, bool isDEAExpired, List<string> schedulesAllowed);
    }
}