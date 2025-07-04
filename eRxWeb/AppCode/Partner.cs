using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Generic;
using ComponentSpace.CryptoHelper;
using Allscripts.ePrescribe.DatabaseSelector;
using DAL = Allscripts.ePrescribe.Data;

namespace eRxWeb
{
/// <summary>
/// Summary description for Partner
/// </summary>
public class Partner
{
    #region Member Variables

    private string _id;
    private string _name;
    private string _issuer;
    private X509Certificate _certificate;
    private X509Certificate2 _certificate2;
    private string _certificateFile;
    private string _certificateFile2;
    private string _theme;
    private string _destinationUrl;
    private string _assertionFormatString;
    private string _targetPage;
    private string _errorURL;
    private ArrayList _audienceRestrictionList;
    private string _certificateFilePassword;
    private string _certificateFile2Password;
    private RSA _privateKey;
    private string _logoutURL;
    private string _enterpriseClientEnabled;
    private string _enterpriseClientID;
    private bool _allowsUserNameAndPassword = false;
    private bool _allowShieldEnrollment = false;

    #endregion

    public Partner()
    {

    }

    public Partner(DataRow dr)
    {
        loadPartner(dr);
    }

    #region Properties

    public string ID
    {
        get { return _id; }
        set { _id = value; }
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public string Issuer
    {
        get { return _issuer; }
        set { _issuer = value; }
    }

    public string CertificateFile
    {
        get
        {
            return _certificateFile;
        }
        set
        {
            _certificateFile = value;
        }
    }

    public string CertificateFile2
    {
        get
        {
            return _certificateFile2;
        }
        set
        {
            _certificateFile2 = value;

            loadCertificate2(_certificateFile2, _certificateFile2Password);
        }
    }

    public string Theme
    {
        get { return _theme; }
        set { _theme = value; }
    }

    public string DestinationUrl
    {
        get { return _destinationUrl; }
        set { _destinationUrl = value; }
    }

    public string LogoutUrl
    {
        get { return _logoutURL; }
        set { _logoutURL = value; }
    }

    public string AssertionFormatString
    {
        get { return _assertionFormatString; }
        set { _assertionFormatString = value; }
    }

    public string TargetPage
    {
        get { return _targetPage; }
        set { _targetPage = value; }
    }

    public string ErrorURL
    {
        get
        {
            return _errorURL;
        }
        set
        {
            _errorURL = value;
        }
    }

    public ArrayList AudienceRestrictionList
    {
        get
        {
            return _audienceRestrictionList;
        }
        set
        {
            _audienceRestrictionList = value;
        }
    }

    public string CertificateFilePassword
    {
        get
        {
            return _certificateFilePassword;
        }
        set
        {
            _certificateFilePassword = value;

            if (_certificateFile.IndexOf(".pfx") > 0)
            {
                loadCertificate(_certificateFile, _certificateFilePassword);
            }
        }
    }

    public string CertificateFile2Password
    {
        get
        {
            return _certificateFile2Password;
        }
        set
        {
            _certificateFile2Password = value;
        }
    }

    public string EnterpriseClientID
    {
        get { return _enterpriseClientID; }
        set { _enterpriseClientID = value; }
    }

    public string EnterpriseClientEnabled
    {
        get { return _enterpriseClientEnabled; }
        set { _enterpriseClientEnabled = value; }
    }

    public X509Certificate Certificate
    {
        get { return _certificate; }
        set { _certificate = value; }
    }

    public X509Certificate2 Certificate2
    {
        get { return _certificate2; }
        set { _certificate2 = value; }
    }

    public Object PrivateKey /* had to cast to an object, RSA constructure issue */
    {
        get { return (Object)_privateKey; }
        set { _privateKey = (RSA)value; }
    }

    public bool AllowsUserNameAndPassword 
    {
        get { return _allowsUserNameAndPassword; }
        set { _allowsUserNameAndPassword = value; } 
    }

    public bool AllowShieldEnrollment
    {
        get { return _allowShieldEnrollment; }
        set { _allowShieldEnrollment = value; }
    }
        

    #endregion

    #region Methods

    private void loadPartner(DataRow dr)
    {
        System.Xml.XmlDocument config = new System.Xml.XmlDocument();
        config.LoadXml(dr["Config"].ToString());
        XmlNode root = config.DocumentElement;

        _id = dr["PartnerID"].ToString();
        _name = dr["PartnerName"].ToString();
        
        if (dr["AllowsUserNameAndPassword"] != DBNull.Value && 
            !string.IsNullOrEmpty(dr["AllowsUserNameAndPassword"].ToString()) && 
            bool.Parse(dr["AllowsUserNameAndPassword"].ToString()))
        {
            _allowsUserNameAndPassword = true;
        }

        System.Xml.XmlNode nd = root.SelectSingleNode("/configuration");

        if (nd != null)
        {
            _destinationUrl = getXMLNodeText(nd, "DestinationUrl");
            _issuer = getXMLNodeText(nd, "Issuer");
            _assertionFormatString = getXMLNodeText(nd, "AssertionFormatString");
            _targetPage = getXMLNodeText(nd, "TargetPage");

			//Caremark CVS
            if (_id.Equals("505C0B6C-D1BC-419A-AAE7-D857CC5B9119", StringComparison.OrdinalIgnoreCase))
            {
                _certificateFile2 = getXMLNodeText(nd, "CertificateFile");
                _certificateFile2Password = getXMLNodeText(nd, "CertificateFilePassword");

                loadCertificate2(_certificateFile2, _certificateFile2Password);
            }
            else
            {
                _certificateFile = getXMLNodeText(nd, "CertificateFile");
                _certificateFilePassword = getXMLNodeText(nd, "CertificateFilePassword");

                if (_certificateFile.IndexOf(".pfx") == -1)
                {
                    loadCertificate2(_certificateFile);
                }
                else if (_certificateFile.IndexOf(".pfx") > 0)
                {
                    loadCertificate(_certificateFile, _certificateFilePassword);
                }
            }

            _theme = getXMLNodeText(nd, "Theme");
            _logoutURL = getXMLNodeText(nd, "LogoutUrl");
            _enterpriseClientID = getXMLNodeAttributeValue(nd, "enterprise", "clientid");
            _enterpriseClientEnabled = getXMLNodeAttributeValue(nd, "enterprise", "enabled");
            _errorURL = getXMLNodeText(nd, "ErrorUrl");
            _allowShieldEnrollment = (getXMLNodeText(nd, "AllowShieldEnrollment").ToUpper().Equals("Y")) ? true : false;

            XmlNode audienceRestrictionsNode = nd.SelectSingleNode("AudienceRestrictions");

            if (audienceRestrictionsNode != null)
            {
                XmlNodeList audienceRestrictionNodeList = audienceRestrictionsNode.ChildNodes;
                ArrayList audienceRestrictionArrayList = new ArrayList();

                foreach (XmlNode n in audienceRestrictionNodeList)
                {
                    audienceRestrictionArrayList.Add(n.InnerText);
                }

                _audienceRestrictionList = audienceRestrictionArrayList;
            }
        }
    }

    private void loadCertificate(string fileName)
    {
        HttpContext context = HttpContext.Current;
        string filePath = null;

        if (context != null)
        {
            filePath = context.Server.MapPath(fileName);
        }

        if (File.Exists(fileName))
        {
			_certificate = X509Certificate.CreateFromCertFile(fileName);
        }
        else if (File.Exists(filePath))
        {
			_certificate = X509Certificate.CreateFromCertFile(filePath);
        }
    }

    private void loadCertificate(string fileName, string password)
    {
        HttpContext context = HttpContext.Current;
        string filePath = null;

        if (context != null)
        {
            filePath = context.Server.MapPath(fileName);
        }

        if (!File.Exists(filePath))
        {
            throw (new ArgumentException("The certificate file " + filePath + " doesn\'t exist."));
        }

        CertificateContext[] certificateContexts = null;
        CertificateStore certStore = null;

        try
        {
            certStore = (CertificateStore)CertificateStore.ImportPfxFile(filePath, password, CertificateStoreLocation.LocalMachine);
            certificateContexts = (CertificateContext[])CertificateContext.FindAllCertificates(certStore);

            if (certificateContexts == null || certificateContexts.Length == 0)
            {
                throw (new ArgumentException("The certificate file contains no certificates."));
            }

            _certificate = (X509Certificate)certificateContexts[0].X509Certificate;
            _privateKey = (RSA)certificateContexts[0].PrivateKey;

        }
        finally
        {
            if (certStore != null)
            {
                certStore.Close();
            }

            CertificateContext.CloseCertificateContexts(certificateContexts);
        }
    }

	private void loadCertificate2(string fileName)
	{
		HttpContext context = HttpContext.Current;
		string filePath = null;

		if (context != null)
		{
			filePath = context.Server.MapPath(fileName);
		}

		if (File.Exists(fileName))
		{
			_certificate2 = new X509Certificate2(fileName);
            _privateKey = (RSA)_certificate2.PrivateKey;
		}
		else if (File.Exists(filePath))
		{
			_certificate2 = new X509Certificate2(filePath);
            _privateKey = (RSA)_certificate2.PrivateKey;
		}
	}

    private void loadCertificate2(string fileName, string password)
    {
        HttpContext context = HttpContext.Current;
        string filePath = null;

        if (context != null)
        {
            filePath = context.Server.MapPath(fileName);
        }

        if (File.Exists(fileName))
        {
            _certificate2 = new X509Certificate2(fileName, password, X509KeyStorageFlags.MachineKeySet);
            _privateKey = (RSA)_certificate2.PrivateKey;
        }
        else if (File.Exists(filePath))
        {
            _certificate2 = new X509Certificate2(filePath, password, X509KeyStorageFlags.MachineKeySet);
            _privateKey = (RSA)_certificate2.PrivateKey;
        }
    }

    private string getXMLNodeText(XmlNode xmlNode, string xPath)
    {
        string text = "";

        if (xmlNode != null)
        {
            if (xmlNode.SelectSingleNode(xPath) != null)
            {
                text = xmlNode.SelectSingleNode(xPath).InnerText;
            }
        }

        return text;
    }

    private string getXMLNodeAttributeValue(XmlNode xmlNode, string xPath, string attribute)
    {
        string value = "";

        if (xmlNode != null)
        {
            if (xmlNode.SelectSingleNode(xPath) != null)
            {
                if (xmlNode.SelectSingleNode(xPath).Attributes != null && xmlNode.SelectSingleNode(xPath).Attributes[attribute] != null)
                {
                    value = xmlNode.SelectSingleNode(xPath).Attributes[attribute].Value;
                }
            }
        }

        return value;
    }

    #endregion
}

}