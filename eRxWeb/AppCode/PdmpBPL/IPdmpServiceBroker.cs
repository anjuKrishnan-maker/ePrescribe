namespace eRxWeb.AppCode.PdmpBPL
{
    public interface IPdmpServiceBroker
    {
        string PatientRetrieveResponse(string fhirRequest, string pptTokenB64);
    }
}