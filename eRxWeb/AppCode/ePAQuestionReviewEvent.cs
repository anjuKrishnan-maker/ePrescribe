using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Allscripts.ePrescribe.Common;
namespace eRxWeb
{
/// <summary>
/// Summary description for ePAQuestionReviewEvent
/// </summary>
public class ePAQuestionReviewEventArgs
{
    private Constants.ePAQuestionReviewUIEvents _eventType = Constants.ePAQuestionReviewUIEvents.Nothing;
    string _taskID = string.Empty;
    Constants.ePATransPriority _ePATransPriority = Constants.ePATransPriority.NOT_URGENT;

	public ePAQuestionReviewEventArgs()
	{
	}

    public ePAQuestionReviewEventArgs(Constants.ePAQuestionReviewUIEvents eventType)
	{
        _eventType = eventType;
	}

    public ePAQuestionReviewEventArgs(Constants.ePAQuestionReviewUIEvents eventType, string taskID, Constants.ePATransPriority ePATransPriority)
    {
        _eventType = eventType;
        _taskID = taskID;
        _ePATransPriority = ePATransPriority;
    }

    public string TaskID { get { return _taskID; } }
    public Constants.ePATransPriority EPATransPriority { get { return _ePATransPriority; } }
    public Constants.ePAQuestionReviewUIEvents EventType { get { return _eventType; } }
}

public delegate void ePAQuestionReviewEventHandler(object sender, ePAQuestionReviewEventArgs e);
}