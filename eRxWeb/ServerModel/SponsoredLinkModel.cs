using Allscripts.Impact;
using System.Collections.Generic;


namespace eRxWeb.ServerModel
{
    public class SponsoredLinkModel
    {
        public SponsoredLinkModel()
        {
            this.MessageLines = new List<SponsoredLinkMessageLines>();
        }
        public int MessageId { get; set; }
        public List<SponsoredLinkMessageLines> MessageLines { get; set; }
    }
}