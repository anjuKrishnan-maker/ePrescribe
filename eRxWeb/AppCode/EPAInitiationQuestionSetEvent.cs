using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Allscripts.ePrescribe.Common;
/// <summary>
/// Summary description for ePAQuestionReviewEvent
/// </summary>
public class EPAInitiationQuestionSetEventArgs
{
    private Constants.EPAInitiationQuestionSetUIEvents _eventType = Constants.EPAInitiationQuestionSetUIEvents.Nothing;
    string _taskID = string.Empty;

    public string TaskID { get { return _taskID; } }
    public Constants.EPAInitiationQuestionSetUIEvents EventType { get { return _eventType; } }

    public EPAInitiationQuestionSetEventArgs()
    {
    }

    public EPAInitiationQuestionSetEventArgs(Constants.EPAInitiationQuestionSetUIEvents eventType)
    {
        _eventType = eventType;
    }

    public EPAInitiationQuestionSetEventArgs(Constants.EPAInitiationQuestionSetUIEvents eventType, string taskID)
    {
        _eventType = eventType;
        _taskID = taskID;
    }
}

public delegate void EPAInitiationQuestionSetEventHandler(object sender, EPAInitiationQuestionSetEventArgs e);