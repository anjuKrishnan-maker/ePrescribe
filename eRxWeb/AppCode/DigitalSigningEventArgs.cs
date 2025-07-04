using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb
{
/// <summary>
/// Custom Event Arg class for EPCS Digital Signing
/// </summary>
public class DigitalSigningEventArgs : EventArgs
{
    public bool Success { get; set; }
    public Dictionary<string, string> SignedMeds { get; set; }
    public List<string> UnsignedMeds { get; set; }
    public string Message { get; set; }
    public bool ForceLogout { get; set; }
    public bool EpcsRightsRemoved { get; set; }
    public bool IsAddressCityMissing { get; set; }
    public bool IsMismatch { get; set; }

    public DigitalSigningEventArgs()
    {
        ForceLogout = false;
        EpcsRightsRemoved = false;
        IsAddressCityMissing = false;
        IsMismatch = false;
    }

    public DigitalSigningEventArgs(bool success)
    {
        Success = success;
        ForceLogout = false;
        EpcsRightsRemoved = false;
        IsAddressCityMissing = false;
        IsMismatch = false;
    }
}
}