using System;

namespace eRxWeb
{
public partial class Controls_CSMedRefillRequestNotAllowed : BaseControl
{
    #region Private Members
		
    private string _yesMessage = string.Empty;
    private string _contactMeMessage = string.Empty;
    
	#endregion
    
    #region Public Members

    public event EventHandler OnPrintRefillRequest;
    public event EventHandler OnContactProvider;
    public event EventHandler OnCancel;

    public bool HideContactMe
    {       
        set
        {
            ViewState["HideContactMe"] = value;
        }
    }

    public bool HideContactMeText
    {
        set
        {
            ViewState["HideContactMeText"] = value;
        }
    }

    #endregion

    #region EventHandlers

    protected void Page_Load(object sender, EventArgs e)
    {
        if (ViewState["HideContactMe"] != null && Convert.ToBoolean(ViewState["HideContactMe"]))
        {
            btnContactMe.Visible = false;
        }

        if (ViewState["HideContactMeText"] != null && !string.IsNullOrEmpty(ViewState["HideContactMeText"].ToString()))
        {
            ltrContactMe.Visible = false;
        }
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        if (OnPrintRefillRequest != null)
        {
            OnPrintRefillRequest(sender, e);
        }
    }

    protected void btnContactMe_Click(object sender, EventArgs e)
    {
        if (OnContactProvider != null)
        {
            OnContactProvider(sender, e);
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        if (OnCancel != null)
        {
            OnCancel(sender, e);
        }
        else
        {
            modalCSWarningPopup.Hide();
            //Session.Remove("REFILLMSG"); // Remove the refill message from session, if the user wishes to cancel.
        }
    }

    public void ShowPopUp()
    {
        modalCSWarningPopup.Show();
    } 

    #endregion
}
}