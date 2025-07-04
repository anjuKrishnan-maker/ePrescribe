using System.Collections.Generic;

namespace eRxWeb.ServerModel
{
    public class RightBoxModel
    {
        public RightBoxModel()
        {
            this.Links = new List<RightBoxLink>();
        }
        public string RightBoxHeaderText { get; set; }
        public string RightBoxImageURL { get; set; }
        public string RightBoxBodyText { get; set; }
        public List<RightBoxLink> Links { get; set; }
    }

    public class RightBoxLink
    {
        public string Url { get; set; }
        public string Text { get; set; }
    }
}