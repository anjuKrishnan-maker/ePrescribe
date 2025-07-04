namespace eRxWeb.ServerModel
{
    public class ErrorContextModel
    {
        public string Message { get; set; }
        public ErrorTypeEnum Error { get; set; } 
    }

    public enum ErrorTypeEnum
    {
        UserMessage,
        ServerError  
    }

    public class ApiResponse
    {
        public ErrorContextModel ErrorContext { get; set; }
        public dynamic Payload { get; set; }
    }
}