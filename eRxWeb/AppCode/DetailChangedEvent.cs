
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
public class DetailChangedEventArgs
{
    private bool _detailChange;
    private string _patientID;
    private bool _redirect;
    private string _url;

    public DetailChangedEventArgs(bool c)
    {
        _detailChange = c;
        _redirect = false;
        _url = "";
    }

    public DetailChangedEventArgs(bool c, string p)
    {
        _detailChange = c;
        _patientID = p;
        _redirect = false;
        _url = "";
    }

    public DetailChangedEventArgs(string u)
    {
        _detailChange = false;
        _redirect = true;
        _url = u;
    }

    public bool DetailChange { get { return _detailChange; } set { _detailChange = value; } }
    public string PatientID { get { return _patientID; } set { _patientID = value; } }
    public bool Redirect { get { return _redirect; } set { _redirect = value; } }
    public string URL { get { return _url; } set { _url = value; } }
}

public delegate void DetailChangedEventHandler(object sender, DetailChangedEventArgs e);

}