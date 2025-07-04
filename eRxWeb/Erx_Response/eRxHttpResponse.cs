using System.Web;

namespace eRxWeb.Erx_Response
{
    public class eRxHttpResponse : IeRxHttpResponse
    {
        private readonly HttpResponse _response;

        public eRxHttpResponse(HttpResponse response)
        {
            _response = response;
        }

        public void Redirect(string url)
        {
            _response.Redirect(url);
        }
    }
}