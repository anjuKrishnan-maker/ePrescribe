#region Using directives

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using eRxWeb.AppCode.Interfaces;
using Allscripts.Impact.Interfaces;


#endregion

/// <summary>
/// Summary description for IDology
/// </summary>
public class IDologyClass
{
    #region  * Results code Strings *
    public string[] AddressErrorCodes = new String[]
    {
        "resultcode.address.does.not.match",
        "resultcode.street.number.does.not.match",
        "resultcode.street.name.does.not.match",
        "resultcode.state.does.not.match",
        "resultcode.zip.code.does.not.match",
        "resultcode.input.address.is.po.box",
        "resultcode.located.address.is.po.box",
        "resultcode.newer.record.found",
        "resultcode.high.risk.adddress.alert",
        "resultcode.address.velocity.alert",
        "resultcode.address.stability.alert",
        "resultcode.address.longevity.alert",
        "resultcode.address.location.alert",
        "resultcode.alternate.address.alert",
        "resultcode.alternate.address.alert",
        "resultcode.alternate.address.alert",
        "resultcode.alternate.address.alert",
        "resultcode.alternate.address.alert",
        "resultcode.warm.address.alert",
        "resultcode.warm.address.alert",
        "resultcode.warm.address.alert",
        "resultcode.warm.address.alert",
        "resultcode.warm.address.alert",
        "resultcode.warm.address.alert",
        "resultcode.warm.address.alert",
        "resultcode.warm.address.alert",
        "resultcode.single.address.in.file"
    };
    private String[] ResultsCodes = new String[10]
      {
         "resultcode.of.age",
         "resultcode.confirm.age",
         "resultcode.new.record.found",
         "resultcode.no.dob.available",
         "resultcode.ssn.not.available",
         "resultcode.zip.does.not.match",
         "resultcode.ssn.does.not.match",
         "resultcode.dob.does.not.match",
         "resultcode.state.does.not.match",
         "resultcode.address.does.not.match"
      };

    private String[] FaitalResultsCode = new String[8]
      {
         "resultcode.under.age",
         "resultcode.coppa.alert",
         "resultcode.multiple.records.found",
         "resultcode.ssn.does.not.match",
         "resultcode.subject.deceased",
         "resultcode.dob.does.not.match",
         "resultcode.state.does.not.match",
         "resultcode.ssn.issued.prior.to.dob"
      };

    private String[] SummaryResultKeys = new String[5]
      {
         "id.failure",
         "id.success",
         "result.no.match",
         "id.not.eligible.for.questions",
         "result.questions.no.data"
      };

    #endregion


    #region  * Class Variables *

    // Root URL for all API calls.
    private String URL_ROOT;
    private String username;
    public String password; 

    public IDologyClass(IAppConfig appConfig, IConfigKeys iConfigKeys)
    {
        URL_ROOT = appConfig.GetAppSettings("IDologyRoot");
        username = appConfig.GetAppSettings("IDologyUName");
        password = iConfigKeys.IDologyPwd;// Getting the IDologyPwd from DB (ConfigKey)
    }
    // Credentials for the query. Either city and state or zip can be null.
    // SSN (last 4), and DOB month and year can also be null. If DOB month
    // is populated, DOB year must be as well.
    private String firstName = String.Empty;
    private String lastName = String.Empty;
    private String address = String.Empty;
    private String city = String.Empty;
    private String state = String.Empty;
    private String zip = String.Empty;

    // Optional params.
    private String dobMonth = String.Empty;
    private String dobYear = String.Empty;

    // Reporting params (optional)
    private String invoice = String.Empty; // invoice or or1der number
    private String amount = String.Empty; // subtotal
    private String shipping = String.Empty; // shipping amount
    private String tax = String.Empty; // tax amount
    private String total = String.Empty; // total (sum of the above amounts)
    private String idType = String.Empty; // type of id provided
    private String idIssuer = String.Empty; // issuer of id provided
    private String idNumber = String.Empty; // number on id provided
    private String paymentMethod = String.Empty; // payment method
    private String email = String.Empty; // Email
    private String uid = String.Empty; // UID
    private String sku = String.Empty; // sku
    private String l4SSN = String.Empty; // Last 4 of the SSN

    private String queryID = String.Empty; // 
    private String[] questionTypes = new String[5];
    private String[] questionAnswers = new String[5];

    private bool ValidUserIdentification = false;
    private bool FatalError = false;
    private bool NonFatalError = false;
    private String ErrorTableList = String.Empty;
    private DataTable _emailRecord;
    private bool _isAddressValidatedByIDology = false;

    #endregion

    #region  * Properties *

    // Username and password assigned by IDology.
    public String FirstName
    {
        get { return firstName; }
        set { firstName = value; }
    }
    public String LastName
    {
        get { return lastName; }
        set { lastName = value; }
    }
    public String Address
    {
        set { address = value; }
    }
    public String City
    {
        set { city = value; }
    }
    public String State
    {
        set { state = value; }
    }
    public String Zip
    {
        set { zip = value; }
    }
    public String DobMonth
    {
        set { dobMonth = value; }
    }
    public String DOBYear
    {
        set { dobYear = value; }
    }

    // Optional Information
    public String Invoice
    {
        get { return invoice; }
        set { invoice = value; }
    }
    public String Amount
    {
        get { return amount; }
        set { amount = value; }
    }
    public String Shipping
    {
        get { return shipping; }
        set { shipping = value; }
    }
    public String Tax
    {
        get { return tax; }
        set { tax = value; }
    }
    public String Total
    {
        get { return total; }
        set { total = value; }
    }
    public String IdType
    {
        get { return idType; }
        set { idType = value; }
    }
    public String IdIssuer
    {
        get { return idIssuer; }
        set { idIssuer = value; }
    }
    public String IdNumber
    {
        get { return idNumber; }
        set { idNumber = value; }
    }
    //public String QueryID1
    //{
    //   get { return queryID1; }
    //}

    //public String QueryID2
    //{
    //   get { return queryID2; }
    //}
    public String PaymentMethod
    {
        get { return paymentMethod; }
        set { paymentMethod = value; }
    }
    public String Email
    {
        get { return email; }
        set { email = value; }
    }
    public String Uid
    {
        get { return uid; }
        set { uid = value; }
    }
    public String Sku
    {
        get { return sku; }
        set { sku = value; }
    }
    public String QueryID
    {
        get { return queryID; }
        set { queryID = value; }
    }
    public String L4SSN
    {
        get { return l4SSN; }
        set { l4SSN = value; }
    }
    public Boolean isValid
    {
        get { return ValidUserIdentification; }
        set { ValidUserIdentification = value; }
    }
    public Boolean hasFatalError
    {
        get { return FatalError; }
        set { FatalError = value; }
    }
    public Boolean hasNonFatalError
    {
        get { return NonFatalError; }
        set { NonFatalError = value; }
    }

    public string ErrorTableNames
    {
        get { return ErrorTableList; }
    }
    public DataTable EmailRecord
    {
        get { return _emailRecord; }
        set { _emailRecord = value; }
    }

    public bool IsAddressValidatedByIDology
    {
        get { return _isAddressValidatedByIDology; }
        set { _isAddressValidatedByIDology = value;}
    }

    public Constants.EmailRiskLevel EmailRisk { get; set; }
    public List<string> ResultCodeErrorsList { get; set; }
    public List<string> ErrorsList { get; set; }

    #endregion

    public DataSet GetIDologyQuestions(Guid UserToken)
    {
        String resultXML = callQuestionService(username, password, FirstName, LastName, address, city, state, zip, dobYear, L4SSN, UserToken);
        Allscripts.Impact.Audit.AddLogEntryIdology("Identity.cs", "GetIDologyQuestions", UserToken, resultXML);
        XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(resultXML));
        DataSet xDS = new DataSet("XMLQuestionSet");
        xDS.ReadXml(xmlTextReader);
        if (checkForErrors(xDS))
        {
            //handle error
            //call error function to find out what errors were called.
        }
        bool isInvalidUser = ValidateDataSet(ref xDS);

        if (Idology.checkForQualifiers(ref xDS))
        {
            ResultCodeErrorsList = Idology.GetListOfQualifiers(xDS);
        }
        //Check for error
        if(Idology.checkForErrors(xDS))
        {
            ErrorsList = Idology.GetListOfErrors(xDS);//will be empty if successful
        }
        //Address Checker
        _isAddressValidatedByIDology = (isInvalidUser)?false:IsProviderAddressValidatedByIDology(ResultCodeErrorsList, ErrorsList);
        return xDS;
    }

    public bool IsProviderAddressValidatedByIDology(List<string> resultCodeErrorsList, List<string> resultErrorsList)
    {
        bool isAddressValidatedByIDology = true;
        if (resultCodeErrorsList != null)
        {
            foreach (string currentErrorCode in resultCodeErrorsList)
            {
                bool isCurrentErrorCodeAnAddressError = Array.IndexOf(AddressErrorCodes, currentErrorCode) >= 0;
                if (isCurrentErrorCodeAnAddressError)
                {
                    //Even if one problem, return immediately as not validated
                    return false;
                }
            }
        }
        if(resultErrorsList !=null && resultErrorsList.Count>0)
        {
            isAddressValidatedByIDology = false;
        }

        return isAddressValidatedByIDology;
    }
    public DataSet GetIDologyChallengeQuestions(Guid UserToken)
    {
        String resultXML = callChallengeQuestionService(username, password, QueryID, UserToken);
        //LogActivity.AddLogEntry("Identity.cs", "GetIDologyChallengeQuestions", UserToken, resultXML);

        XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(resultXML));
        DataSet xDS = new DataSet("XMLQuestionSet");
        xDS.ReadXml(xmlTextReader);
        if (checkForErrors(xDS))
        {
            //handle error
            //call error function to find out what errors were called.
        }

        if (ValidateDataSet(ref xDS))
        {
            //Error.
        }

        //setValidResult(ref xDS);

        //if (checkForAnyError(ref xDS))
        //{
        //   //a column was named error.  need to handle that code.
        //}
        //if (checkForQualifiers(ref xDS))
        //{
        //   //handle qualifiers
        //}
        return xDS;
    }

    //
    // Entry point for this example code.
    //
    public DataSet SendIDologyAnswers(ref String[] aAnswerType, ref String[] aAnswerText)
    {
        // The query ID is used to identify the ExpectIDIQ
        // request that was previously made.
        //
        // The question types and answers need to come from
        // the ExpectIDIQ response identified by this ID.

        // The question types and answers.

        questionTypes = aAnswerType;
        questionAnswers = aAnswerText;

        String responseXML = callAnswerService(username, password, queryID, questionTypes, questionAnswers, "/idliveq-answers.svc");

        XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(responseXML));

        DataSet xDS;
        xDS = new DataSet("XMLAnswerSet");
        xDS.ReadXml(xmlTextReader);
        return xDS;
    }

    //
    // Entry point for this example code.
    //
    public DataSet SendIDologyChallengeAnswers(ref String[] aAnswerType, ref String[] aAnswerText)
    {
        // The query ID is used to identify the ExpectIDIQ
        // request that was previously made.
        //
        // The question types and answers need to come from
        // the ExpectIDIQ response identified by this ID.

        // The question types and answers.

        questionTypes = aAnswerType;
        questionAnswers = aAnswerText;

        String responseXML = callAnswerService(username, password, queryID, questionTypes, questionAnswers, "/idliveq-challenge-answers.svc");

        XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(responseXML));

        DataSet xDS;
        xDS = new DataSet("XMLAnswerSet");
        xDS.ReadXml(xmlTextReader);
        return xDS;
    }

    //
    // Make the call to the web service with the given data.
    //
    private String callQuestionService(String aUsername, String aPassword, String aFirstName, String aLastName,
                                       String aAddress, String aCity, String aState, String aZip,
                                       String aDobYear, String assnLast4, Guid UserToken)
    {
        String url = URL_ROOT + "/idiq.svc";

        if (null == aUsername || null == aPassword)
        {
            throw new ApplicationException("username and password are required.");
        }

        String requestParams = "username=" + aUsername + "&password=" + aPassword;

        // This sample will send the request regardless of whether or not
        // these fields are missing, but if required fields are left empty,
        // errors will be returned.
        requestParams += getParamString("firstName", aFirstName);
        requestParams += getParamString("lastName", aLastName);
        requestParams += getParamString("address", aAddress);
        requestParams += getParamString("city", aCity);
        requestParams += getParamString("state", aState);
        requestParams += getParamString("zip", aZip);
        requestParams += getParamString("dobYear", aDobYear);
        requestParams += getParamString("ssnLast4", assnLast4);
        Allscripts.Impact.Audit.AddLogEntryIdology("Identity.cs", "callQuestionService", UserToken, String.Format("IDologyParms: {0}", requestParams));
        return httpPost(url, requestParams, UserToken);
    }

    //
    // Make the call to the web service with the given data.
    //
    private String callChallengeQuestionService(String aUsername, String aPassword, String aQueryID, Guid UserToken)
    {
        String url = URL_ROOT + "/idliveq-challenge.svc";

        if (null == aUsername || null == aPassword)
        {
            throw new ApplicationException("username and password are required.");
        }

        String requestParams = "username=" + aUsername + "&password=" + aPassword;

        // This sample will send the request regardless of whether or not
        // these fields are missing, but if required fields are left empty,
        // errors will be returned.
        requestParams += getParamString("idNumber", aQueryID);
        Allscripts.Impact.Audit.AddLogEntryIdology("Identity.cs", "callChallengeQuestionService", UserToken, String.Format("IDologyParms: {0}", requestParams));
        return httpPost(url, requestParams, UserToken);
    }

    //
    // Make the call to the web service with the given data.
    //
    private String callAnswerService(String aUsername, String aPassword, String aQueryId, String[] aQuestionTypes,
                                     String[] aQuestionAnswers, String link)
    {
        String url = URL_ROOT + link;

        if (null == aUsername || null == aPassword || null == aQueryId)
        {
            throw new ApplicationException("username and password are required.");
        }

        // Create the body of the request (name and value pairs).
        String requestParams = "username=" + aUsername + "&password=" + aPassword;
        requestParams += "&idNumber=" + aQueryId;
        for (int i = 0; i < aQuestionTypes.Length; i++)
        {
            requestParams += getParamString("question" + (i + 1) + "Type", aQuestionTypes[i]);
            requestParams += getParamString("question" + (i + 1) + "Answer", aQuestionAnswers[i]);
        }

        return httpPost(url, requestParams, Guid.Empty);
    }

    //
    // Determine if there were errors in the request, such as
    // missing parameters
    //
    //private bool checkForErrors(XmlDocument aXmlDocument)
    //{
    //   //
    //   // Determine if there were errors in the request, such as
    //   // missing parameters
    //   //
    //   bool errorFound = false;

    //   // Extract error elements from the parsed XML.
    //   XmlNodeList nodes = aXmlDocument.GetElementsByTagName("error");

    //   // Print out the error messages to the console.
    //   for (int i = 0; i < nodes.Count; i++)
    //   {
    //      Console.WriteLine("Error: " + nodes[i].InnerXml);
    //   }

    //   // Extract failed elements from the parsed XML.
    //   nodes = aXmlDocument.GetElementsByTagName("failed");

    //   // Print out the fatal error messages to the console.
    //   for (int i = 0; i < nodes.Count; i++)
    //   {
    //      Console.WriteLine("Fatal error: " + nodes[i].InnerXml);
    //   }

    //   return errorFound;
    //}

    private bool ValidateDataSet(ref DataSet ds)
    {
        bool errorFound = false;

        try
        {
            //<summary-result><key>id.failure</key><message>Match Not Found</message></summary-result>
            if ((ds.Tables["summary-result"].Rows[0]["key"].ToString() == SummaryResultKeys[0].ToString()) ||
                (ds.Tables["idliveq-error"] != null && ds.Tables["idliveq-error"].Rows[0][0].ToString() == SummaryResultKeys[4].ToString()))
            {
                ValidUserIdentification = false;
                FatalError = true;
                errorFound = true;
            }
            else if (ds.Tables["summary-result"].Rows[0]["key"].ToString() == SummaryResultKeys[1].ToString())  //ID.SUCCESS
            {
                ValidUserIdentification = true;
            }
        }
        catch (Exception)
        {
            FatalError = true;
            errorFound = true;
        }
        return errorFound;
    }
    private bool checkForErrors(DataSet ds)
    {
        //
        // Determine if there were errors in the request, such as
        // missing parameters
        //
        bool errorFound = false;

        try
        {
            foreach (DataTable d in ds.Tables)
            {
                if (d.ToString() == "error")
                {
                    ErrorTableList = d.ToString();
                    errorFound = true;
                }
                if (d.ToString() == "failed")
                {
                    ErrorTableList = d.ToString();
                    errorFound = true;
                }
                if (d.ToString() == "restriction")
                {
                    ErrorTableList = d.ToString();
                    errorFound = true;
                }
                if (d.ToString() == "velocity-results")
                {
                    ErrorTableList = d.ToString();
                    errorFound = true;
                }
                if (d.ToString() == "idliveq-error")
                {
                    ErrorTableList = d.ToString();
                    errorFound = true;
                }
            }
        }
        catch (Exception e)
        {
            throw (e);
        }
        return errorFound;
    }

    private bool checkForQualifiers(ref DataSet ds)
    {
        //
        // Determine if there were errors in the request, such as
        // missing parameters
        //
        bool errorFound = false;

        try
        {
            foreach (DataTable d in ds.Tables)
            {
                if (d.ToString() == "qualifier")
                {
                    ErrorTableList = d.ToString();
                    errorFound = true;
                }
                if (d.ToString() == "qualifiers")
                {
                    ErrorTableList = d.ToString();
                    errorFound = true;
                }
                if (d.ToString() == "restriction")
                {
                    ErrorTableList = d.ToString();
                    errorFound = true;
                }
                if (d.ToString() == "velocity-results")
                {
                    ErrorTableList = d.ToString();
                    errorFound = true;
                }
            }
        }
        catch (Exception e)
        {
            throw (e);
        }
        return errorFound;
    }

    private String getParamString(String aParamName, String aParamValue)
    {
        if (null != aParamValue)
        {
            //return "&" + aParamName + "=" + System.Web.HttpUtility.UrlEncode(aParamValue);
            return "&" + aParamName + "=" + aParamValue;
        }
        return String.Empty;
    }

    //
    // make the http POST request to the server
    //
    private String httpPost(String aUrl, String aParams, Guid UserToken)
    {
        // Use the built in WebRequest object to submit the API call.
        WebRequest req = WebRequest.Create(aUrl);

        byte[] bytes = Encoding.ASCII.GetBytes(aParams);

        req.Method = "POST";
        req.ContentType = "application/x-www-form-urlencoded";
        req.ContentLength = bytes.Length;

        WebResponse response;

        try
        {
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(bytes, 0, bytes.Length);
            reqStream.Close();
            response = req.GetResponse();
        }
        catch (WebException e)
        {
            Allscripts.Impact.Audit.AddLogEntryIdology("Identity.cs", "httpPost", UserToken, "HTTP post error calling Idology: " + e.ToString());
            return null;
        }

        // Using a stream reader, extract the result document and return it.
        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
        {
            String result = sr.ReadToEnd();

            // uncomment the following line to see the response xml in the console
            //Console.WriteLine(result);

            sr.Close();

            return result;
        }
    }

    public void GetIDologyErrorDetails(String CallForSupport, int i, out String ErrorHeader, out String ErrorDetail, string ShieldLoginID)
    {
        switch (i)
        {
            case (1000): // VerifyLicense.aspx
                ErrorHeader = "The DEA Verification has failed.";
                ErrorDetail = "The DEA Number you entered could not be verified. " + CallForSupport;
                break;
            case (1001): // UserInterrogation.aspx
                ErrorHeader = "The ID Verification has failed.";
                ErrorDetail = "Your ID cannot be verified with the information provided. " + CallForSupport;
                break;
            case (1002): // page.aspx
                ErrorHeader = string.Empty;
                ErrorDetail = "Error on the page.  Missing Record ID. " + CallForSupport;
                break;
            case (1003): // VerifyLicense.aspx
                ErrorHeader = "Verify License Page";
                ErrorDetail = "Used";
                break;
            case (1004): // page.aspx
                ErrorHeader = "Max Attempts";
                ErrorDetail = "Too many attempts were made to create an account. " + CallForSupport;
                break;
            case (1005): // page.aspx
                ErrorHeader = "IDology Fatal Error Received.";
                ErrorDetail = "IDology Fatal Error Received. " + CallForSupport;
                break;
            case (1006): // page.aspx
                ErrorHeader = "Credential Test Missing Table";
                ErrorDetail = "Credential Test Missing Table. " + CallForSupport;
                break;
            case (1007): // page.aspx
                ErrorHeader = "Account Already Created with this link.";
                ErrorDetail = "An account was already created with this link. " + CallForSupport;
                break;
            case (1008): // page.aspx
                ErrorHeader = "Expired Link";
                ErrorDetail =
                   "The link in the email has expired.  Please start the registration process again.  <br />If you need help from support, " +
                   CallForSupport;
                break;
            case (1009): // page.aspx
                ErrorHeader = "Questions Exhausted.";
                ErrorDetail = "The list of questions available to verify your identity have been exhausted.  <br />" +
                              CallForSupport;
                break;
            case (1010): // page.aspx
                ErrorHeader = "Questions Missing.";
                ErrorDetail =
                   "The list of questions available to verify your identity are missing and your identity cannot be verified.  <br />" +
                   CallForSupport;
                break;
            case (1011): // page.aspx
                ErrorHeader = "Missing User Account";
                ErrorDetail = "The Account you are trying to access doesn't exist.  <br />" + CallForSupport;
                break;
            case (1012): // page.aspx
                ErrorHeader = "Answer Collection Error";
                ErrorDetail = "There was an error collecting the answers provided.  <br />" + CallForSupport;
                break;
            case (1013): // page.aspx
                ErrorHeader = "Challenge Answer Collection Error";
                ErrorDetail = "There was an error collecting the challenge answers provided.  <br />" + CallForSupport;
                break;
            case (1014): // page.aspx
                ErrorHeader = "Account Already Created";
                ErrorDetail = "There is an account already created for your Name & DEA.  <br />" + CallForSupport;
                break;
            case (1015): // ActivateUser.aspx
                ErrorHeader = "User Activation Code Error";
                ErrorDetail = "There was an error generating an Veradigm Shield Activation Code.  <br />" + CallForSupport;
                break;
            case (1016): // ActivationProcessor.aspx
                ErrorHeader = "User Activation Error";
                if (ShieldLoginID != null)
                {
                    ErrorDetail = "Even though an error was encountered, your Shield user account looks to have been still created. Your Login ID is: " + ShieldLoginID + "<br />" + CallForSupport;
                }
                else
                {
                    ErrorDetail = "There was an error creating your Shield user account. <br />" + CallForSupport;
                }
                break;
            case (1017): // page.aspx
                ErrorHeader = string.Empty;
                ErrorDetail = CallForSupport;
                break;
            case (1018): // page.aspx
                ErrorHeader = string.Empty;
                ErrorDetail = CallForSupport;
                break;
            case (1019): // page.aspx
                ErrorHeader = string.Empty;
                ErrorDetail = CallForSupport;
                break;
            case (1020): // page.aspx
                ErrorHeader = "Unexpected Error";
                ErrorDetail = "We apologize, but an unexpected error occurred.  <br />" + CallForSupport;
                break;
            case (1021): // page.aspx
                ErrorHeader = "Identity Question Timeout";
                ErrorDetail = "The allocated time to answer the identity questions has expired.  <br />" + CallForSupport;
                break;
            default:
                ErrorHeader = "Error: Unexpected Error - Default";
                ErrorDetail = "We apologize, but an unexpected error occurred.  <br />" + CallForSupport;
                break;
        }
        ErrorDetail = string.Format("{0}. {1}", i, ErrorDetail);
    }
}