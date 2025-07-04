using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel.Request
{
    public class ReviewHistoryRequest
    {

    }

    public class GetReviewHistory
    {
        public StatusFilterEnum statusFilter { get; set; }
        public DataRetrievalContext dataRetrievalContext { get; set; }

    }
}