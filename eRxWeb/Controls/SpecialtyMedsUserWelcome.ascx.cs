using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eRxWeb
{
    public partial class SpecialtyMedsUserWelcome : BaseControl, Controls.Interfaces.IControls_SpecialtyMedsUserWelcome
    {

        public delegate void SpecMedsWelcomeOK(EventArgs EventArgs);

        //private static readonly ILoggerEx logger = LoggerEx.GetLogger();
        ////
        //// event declaration - this will be used to close the user control and raise event to be consumed by parent page

        public event SpecMedsWelcomeOK OnSpecMedsWelcomeOK;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void Show()
        {
            mpeSpecMedsWelcome.Show();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            mpeSpecMedsWelcome.Hide();
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            if (OnSpecMedsWelcomeOK != null)
            {
                OnSpecMedsWelcomeOK(e);
            }
        }


    }
}