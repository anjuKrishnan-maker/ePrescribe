using System;

namespace eRxWeb
{
    public partial class BasicName : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Name.Text = Environment.MachineName;
        }
    }
}