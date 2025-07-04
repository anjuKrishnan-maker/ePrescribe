namespace eRxWeb.AppCode
{
    public interface IPptPlusServiceBroker
    {
        string InitiatePricingInquiry(string fhirRequest, string pptTokenB64);
        string RetrievePricingInfo(string trnsId, string accountId, string ipAddress, string pptTokenB64);
        string RetrieveCouponInfo(string fhirRequest, string pptTokenB64);
        string SubmitUsageReportRequest(string fhirRequest, string pptTokenB64);
    }
}