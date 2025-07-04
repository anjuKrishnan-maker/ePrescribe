using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eRxWeb
{
public partial class Controls_SecureProcessing : System.Web.UI.UserControl
{
	private string _messageText;

    protected void Page_Load(object sender, EventArgs e)
    {
		if (!Page.IsPostBack)
		{
			if (!string.IsNullOrEmpty(_messageText))
			{
				lblMessage.Text = _messageText;
			}
		}
    }

	/// <summary>
	/// Gets or sets the message text that is displayed
	/// </summary>
	public string MessageText
	{
		get
		{
			return _messageText;
		}
		set
		{
			_messageText = value;
		}
	}

	public class SecureProcessingMessages
	{
        public const string MultipleRx = "Please wait as we process your order.";
        public const string SingleRx = "Please wait as we process your order.";
	}
}
}