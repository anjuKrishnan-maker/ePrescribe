using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace eRxWeb
{
public class FormularyIgnoreWarningProcessedEventArgs
{
    private string _destination = string.Empty;
    private string _searchValue = string.Empty;

    public FormularyIgnoreWarningProcessedEventArgs(string destination, string searchValue)
    {
        _destination = destination;
        _searchValue = searchValue;
    }

    public string Destination { get { return _destination; } set { _destination = value; } }
    public string SearchValue { get { return _searchValue; } set { _searchValue = value; } }

}

public delegate void FormularyIgnoreWarningProcessedEventHandler(object sender, FormularyIgnoreWarningProcessedEventArgs e);

}