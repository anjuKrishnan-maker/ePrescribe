using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using Telerik.Web.UI;

namespace eRxWeb
{
    public partial class Controls_DEALicenseDetails : System.Web.UI.UserControl
    {
        private object _dataItem = null;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Data Item for Control
        /// </summary>
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
        /// Check value and cast to bool
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        public bool CheckAndCast(object objValue)
        {
            if (objValue.GetType() == typeof(bool))
                return bool.Parse(objValue.ToString());
            else
                return false;
        }
        public bool IsDeaSelected(object objValue, string deaLicenseTypeId)
        {
            if (objValue?.GetType() == typeof(ePrescribeSvc.DeaLicenseType) || objValue?.GetType() == typeof(int))
            {
                if ((DeaLicenseType)objValue == DeaLicenseType.DefaultDea && deaLicenseTypeId == "DefaultDea" ||
                    (DeaLicenseType)objValue == DeaLicenseType.Dea && deaLicenseTypeId == "Dea" ||
                    (DeaLicenseType)objValue == DeaLicenseType.NaDean && deaLicenseTypeId == "NaDean")
                {
                    return true;
                }
            }
            return false;
        }
    }
}