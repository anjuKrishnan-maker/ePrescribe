using System;
using System.Collections.Generic;

namespace eRxWeb
{
    public class PatientDemographicsEditEventArgs : EventArgs
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public PatientDemographicsEditEventArgs()
        {
        }

        public PatientDemographicsEditEventArgs(bool success)
        {
            Success = success;
        }
    }
}