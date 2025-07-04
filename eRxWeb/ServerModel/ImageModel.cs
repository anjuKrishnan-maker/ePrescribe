using System.Collections.Generic;

namespace eRxWeb.ServerModel.Common
{
    public class ImageModel
    {
        public ImageModel()
        {
            Style = new Dictionary<string, string>();
            ImageUrl = string.Empty;
        }
        public Dictionary<string,string> Style { get; set; }
        public string ImageUrl { get; set; }
        public string ToolTip { get; set; }

    }
}