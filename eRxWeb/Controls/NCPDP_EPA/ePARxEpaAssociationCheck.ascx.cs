using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.Impact;

namespace eRxWeb
{
    public partial class Controls_ePARxEpaAssociationCheck : BaseControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public bool ShowIfPatientRxAssociatedWithEpaTask(string rxId)
        {
            if (ePA.IsRxIdAssociatedWithActiveEpaTask(rxId, base.DBID))
            {
                mpeRxEpaAssociation.Show();
                return true;
            }
            return false;
        }
    }
}