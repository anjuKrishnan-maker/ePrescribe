using System;

namespace eRxWeb
{
    public partial class EPCSRegistrationContol : BaseControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (linksHit.Value != "")
        {
            int linkCount = linksHit.Value.Split('|').Length;
            linksHit.Value = "";
        }
    }
}

}