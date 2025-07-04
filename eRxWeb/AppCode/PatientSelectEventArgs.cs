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
public class PatientSelectEventArgs
{
    public enum ActionType
    {
       SELECT_PATIENT =0,
       CLEAR_PATIENT
    }

    private string _name;
    private string _patientID;
    private ActionType _Action;

    public PatientSelectEventArgs(string name, string patientID, ActionType actionType)
    {
        _name = name;
        _patientID = patientID; 
        _Action = actionType;
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public string PatientID
    {
        get { return _patientID; }
        set { _patientID = value; }
    }

    public ActionType Action
    {
        get { return _Action; }
        set { _Action = value; }
    }

}

public delegate void PatientSelectEventHandler(object sender, PatientSelectEventArgs e);

}