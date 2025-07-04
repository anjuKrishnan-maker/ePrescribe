using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using Allscripts.Impact;
using Allscripts.ePrescribe.DatabaseSelector;

namespace eRxWeb
{
/// <summary>
/// Summary description for AuditLogInfo
/// </summary>
public class AuditLogInfo
{
    private string m_UserName;
    private string m_DataClass;
    private string m_DataOperation;
    private string m_ActivityTime;

    public AuditLogInfo()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public AuditLogInfo(string userName, string dataClass, string dataOperation, string activityTime)
    {
        // TODO: Add constructor logic here
        m_UserName=userName;
        m_DataClass=dataClass;
        m_DataOperation=dataOperation;
        m_ActivityTime=activityTime;

    }
    public string UserName
    {
        get
        {
            return m_UserName;
        }
    }
    public string DataClass
    {
        get
        {
            return m_DataClass;
        }
    }
    public string DataOperation
    {
        get
        {
            return m_DataOperation;
        }
    }
    public string ActivityTime
    {
        get
        {
            return m_ActivityTime;
        }
    }
}
public class AuditLogDetails
{
    private List<AuditLogInfo> m_AuditLogInfo = null;

   
    public List<AuditLogInfo> GetAuditLogData()
    {
        return m_AuditLogInfo;
    }
}


}