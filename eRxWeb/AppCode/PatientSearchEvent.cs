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
public class PatientSearchEventArgs
{
    public enum ActionType
    {
        SEARCH_ACTIVE = 0,
        SEARCH_INACTIVE,
        SEARCH_ALL,
        ADD_PATIENT,
        ERROR
    }

    private string _LastName;
    private string _FirstName;
    private string _DateOfBirth;
    private string _PatientID;
    private string _WildCard;
    private ActionType _Action;

    public PatientSearchEventArgs(string L, string F, string D, string P, string W, ActionType A)
    {
        _LastName = L;
        _FirstName = F;
        _DateOfBirth = D;
        _PatientID = P;
        _WildCard = W;
        _Action = A;
    }

    public string LastName
    { 
        get { return _LastName; } 
        set { _LastName = value; } 
    }

    public string FirstName 
    { 
        get { return _FirstName; } 
        set { _FirstName = value; } 
    }

    public string DateOfBirth
    {
        get { return _DateOfBirth; }
        set { _DateOfBirth = value; }
    }

    public string PatientID 
    { 
        get { return _PatientID; } 
        set { _PatientID = value; } 
    }

    public string WildCard 
    { 
        get { return _WildCard; } 
        set { _WildCard = value; } 
    }

    public ActionType Action 
    { 
        get { return _Action; } 
        set { _Action = value; } 
    }

}

public delegate void PatientSearchEventHandler(object sender, PatientSearchEventArgs e);

}