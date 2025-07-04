using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eRxWeb.ePrescribeSvc;

namespace eRxWeb
{

    public partial class Controls_ConfirmIdentCompromise : BaseControl
    {


        public delegate void IdentityComprisedHandler(IdentityCompromiseEventArgs icEventArgs);

        public event IdentityComprisedHandler OnIdentityComprised;

        public enum ConfirmIdentityChange
        {
            ConfirmChangeYes,
            ConfirmChangeNo
        }

        public enum IdentityActionEnum
        {
            Compromise,
            Secure,
            Unknown
        }

        public string CompromiseSecure
        {
            get { return GetStringValue("CompromiseSecure"); }
            set { ViewState["CompromiseSecure"] = value; }
        }


    protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnYes_OnClick(object sender, EventArgs e)
        {
            if (OnIdentityComprised != null)
            {
                IdentityCompromiseEventArgs icEventArgs = new IdentityCompromiseEventArgs();
                icEventArgs.ConfirmChange = ConfirmIdentityChange.ConfirmChangeYes;
                icEventArgs.IdentityAction = (IdentityActionEnum)Enum.Parse(typeof(IdentityActionEnum), CompromiseSecure); 
                OnIdentityComprised(icEventArgs);
            }

        }



        protected void btnNo_OnClick(object sender, EventArgs e)
        {
            if (OnIdentityComprised != null)
            {
                IdentityCompromiseEventArgs icEventArgs = new IdentityCompromiseEventArgs();
                icEventArgs.ConfirmChange = ConfirmIdentityChange.ConfirmChangeNo;
                icEventArgs.IdentityAction = (IdentityActionEnum)Enum.Parse(typeof(IdentityActionEnum), CompromiseSecure); ;

                OnIdentityComprised(icEventArgs);
            }
        }


        public void Show(IdentityActionEnum identityAction)
        {
            CompromiseSecure = identityAction.ToString();
            if (identityAction == IdentityActionEnum.Compromise)
            {
                panelCompromise.Visible = true;
                panelSecure.Visible = false;
            }
            else
            {
                panelCompromise.Visible = false;
                panelSecure.Visible = true;
            }
            mpeConfirmIdentityCompromised.Show();
        }



        #region GetStringValue

        private string GetStringValue(string value)
        {
            if (ViewState[value] == null)
            {
                return string.Empty;
            }
            else
            {
                return ViewState[value].ToString();
            }
        }
        #endregion GetStringValue

    }

    public class IdentityCompromiseEventArgs : EventArgs
    {
        public Controls_ConfirmIdentCompromise.ConfirmIdentityChange ConfirmChange { get; set; }
        public Controls_ConfirmIdentCompromise.IdentityActionEnum IdentityAction { get; set; }
    }
}