using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace eRxWeb
{
    public partial class Controls_StateLicenseDetails : System.Web.UI.UserControl
    {
        private object _dataItem = null;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public object DataItem
        {
            get
            {
                return this._dataItem;
            }
            set
            {
                this._dataItem = value;
            }
        }

        /// <summary>
        /// Defaults to selected site's state if this is an insert. If update, do nothing.
        /// </summary>
        public string CheckState(object state)
        {
            if (this._dataItem is Telerik.Web.UI.GridInsertionObject)
            {
                return Session["PRACTICESTATE"].ToString();
            }
            else
            {
                return state.ToString();
            }
        }

        public string CheckLicenseType(object type)
        {
            if (this._dataItem is Telerik.Web.UI.GridInsertionObject)
            {
                return "State License Number";
            }
            else
            {
                return type.ToString();
            }
        }

    }

}