using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Text;
using System.IO;
using Allscripts.Impact;
using Allscripts.Impact.Utilities.Win32;
using Allscripts.Impact.Utilities;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
public partial class PatientReceipt : BasePage
{
    private string _styleSheet
    {
            get
            {
                string returnValue = "StyleXSLv3.css";
                if (System.Configuration.ConfigurationManager.AppSettings["SCRIPT_STYLE_SHEET"] != null)
                {
                    returnValue = System.Configuration.ConfigurationManager.AppSettings["SCRIPT_STYLE_SHEET"].ToString().Trim();
                }


                returnValue = "Style/" + returnValue;


                return returnValue;
            }
    }

    private string _formOutput = string.Empty;
    private bool _currentRxIsCS = false;
  
    private string _userName
    {
        get
        {
            if (Session["UserName"] != null)
            {
                return Session["UserName"].ToString();
            }

            return string.Empty;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            try
            {
                if (Session["PatientReceiptList"] != null && ((ArrayList)Session["PatientReceiptList"]).Count > 0)
                {
                    ProcessReceiptList((List<string>)Session["PatientReceiptList"]);
                }

                if (Session["AllowPatientReceiptsList"] != null && ((List<string>)Session["AllowPatientReceiptsList"]).Count > 0)
                {
                    ProcessReceiptList((List<string>)Session["AllowPatientReceiptsList"]);
                }

            }
            catch (Exception ex) 
            {
                _formOutput += "Error in Printing" + ex.Message.ToString();
            }

        }
    }

    private void ProcessReceiptList(List<string> receiptList)
    {
        _formOutput = string.Empty;
        PrintingPatientReceipt(receiptList);
        lnkStyleSheet.Attributes["href"] = _styleSheet;
        scriptContent.InnerHtml = _formOutput;
    }

    private void PrintingPatientReceipt(List<string> rxList)
    {
        DataSet superDS = null; string xslPath = string.Empty;
        //StreamWriter swTest = new StreamWriter("Testo-1.xml");
        DataSet ds1 = Prescription.LoadPrintData(rxList[0], "1", Session["UserType"].ToString(), (ConnectionStringPointer)Session["DBID"]);

        if (Session["TimeZone"] == null)
        {
            DataSet dsSite = ApplicationLicense.SiteLoad(ds1.Tables["Prescription"].Rows[0]["LicenseID"].ToString(), int.Parse(ds1.Tables["Prescription"].Rows[0]["SiteID"].ToString()), (ConnectionStringPointer)Session["DBID"]);
            if (dsSite != null && ds1.Tables[0].Rows.Count > 0)
            {
                Session["TimeZone"] = dsSite.Tables[0].Rows[0]["TimeZone"].ToString();
            }
        }

		DateTime dt = SystemConfig.GetLocalTime(Session["TimeZone"].ToString(), DateTime.UtcNow);
        string CurrentDate = string.Empty;
        CurrentDate = dt.ToString("MM/dd/yyyy");
        DataColumn dcCurrentDate = new DataColumn("CurrentDate", System.Type.GetType("System.String"));
        ds1.Tables["Practice"].Columns.Add(dcCurrentDate);
        ds1.Tables["Practice"].Rows[0]["CurrentDate"] = CurrentDate;

        DataColumn dcLoginUser = new DataColumn("LoginUser", System.Type.GetType("System.String"));
        ds1.Tables["Practice"].Columns.Add(dcLoginUser);
        ds1.Tables["Practice"].Rows[0]["LoginUser"] = _userName;

        string addrOne = ds1.Tables["Practice"].Rows[0]["address1"].ToString();
        string addrTwo = ds1.Tables["Practice"].Rows[0]["address2"].ToString();
        string city = ds1.Tables["Practice"].Rows[0]["city"].ToString();
        string fullAddress = FormatAddress(addrOne, addrTwo, city);
        DataColumn dcAddress = new DataColumn("Address", System.Type.GetType("System.String"));
        ds1.Tables["Practice"].Columns.Add(dcAddress);
        ds1.Tables["Practice"].Rows[0]["Address"] = fullAddress;

        superDS = ds1.Clone();
        superDS.Merge(ds1.Tables["Provider"]);
        superDS.Merge(ds1.Tables["Patient"]);
        superDS.Merge(ds1.Tables["Practice"]);



        for (int m = 0; m < rxList.Count; m++)
        {
            DataSet ds = Prescription.LoadPrintData(rxList[m], "1", Session["UserType"].ToString(), (ConnectionStringPointer)Session["DBID"]);

            string CSCode = ds.Tables["Prescription"].Rows[0]["ControlledSubstanceCode"].ToString().Trim();
            _currentRxIsCS = !string.IsNullOrEmpty(CSCode) && CSCode != "U";

            string CurrentDate1 = string.Empty;
            DateTime dt1 = SystemConfig.GetLocalTime(Session["TimeZone"].ToString(), DateTime.UtcNow);
            CurrentDate1 = dt1.ToString("MM/dd/yyyy");
            DataColumn dcCurrentDate1 = new DataColumn("CurrentDate", System.Type.GetType("System.String"));
            ds.Tables["Practice"].Columns.Add(dcCurrentDate1);
            ds.Tables["Practice"].Rows[0]["CurrentDate"] = CurrentDate1;

            DataColumn dcLoginUser1 = new DataColumn("LoginUser", System.Type.GetType("System.String"));
            ds.Tables["Practice"].Columns.Add(dcLoginUser1);
            ds.Tables["Practice"].Rows[0]["LoginUser"] = _userName;

            int refillQuantity = 0; refillQuantity = int.Parse(ds.Tables["Prescription"].Rows[0]["RefillQuantity"].ToString().Replace("**", ""));
            DataColumn dcrefillQuantity = new DataColumn("RefillQuantity1", System.Type.GetType("System.String"));
            ds.Tables["Prescription"].Columns.Add(dcrefillQuantity);
            ds.Tables["Prescription"].Rows[0]["RefillQuantity1"] = refillQuantity;

            decimal quantity = Convert.ToDecimal(ds.Tables["Prescription"].Rows[0]["Quantity"]);
            decimal packSize = Convert.ToDecimal(ds.Tables["Prescription"].Rows[0]["PackageSize"]);
            decimal packqty = Convert.ToDecimal(ds.Tables["Prescription"].Rows[0]["PackageQuantity"]);
            string packdesc = ds.Tables["Prescription"].Rows[0]["PackageDescription"].ToString();
            string packUOM = ds.Tables["Prescription"].Rows[0]["PackageUOM"].ToString();
            string DosageDescription = ds.Tables["Prescription"].Rows[0]["DosageDescription"].ToString();
            int daySupply = Convert.ToInt32(ds.Tables["Prescription"].Rows[0]["DaysSupply"]);
            string strength = ds.Tables["Prescription"].Rows[0]["Strength"].ToString().Trim();
            string strengthUOM = ds.Tables["Prescription"].Rows[0]["StrengthUOM"].ToString().Trim();
            string dosageFormCode = ds.Tables["Prescription"].Rows[0]["DosageDescription"].ToString().Trim();
                   

            if ((Constants.PrescriptionStatus)Convert.ToInt32(ds.Tables["Prescription"].Rows[0]["Status"].ToString().Trim()) == Constants.PrescriptionStatus.NEW
                || (Constants.PrescriptionStatus)Convert.ToInt32(ds.Tables["Prescription"].Rows[0]["Status"].ToString().Trim()) == Constants.PrescriptionStatus.SCRIPT_PAD)
            {
                if (ds.Tables["Pharmacy"] != null && ds.Tables["Pharmacy"].Rows.Count > 0)
                {
                    string pharmacyID = string.Empty;
                    pharmacyID = ds.Tables["Pharmacy"].Rows[0]["PharmacyID"] != DBNull.Value ? ds.Tables["Pharmacy"].Rows[0]["PharmacyID"].ToString() : string.Empty;

                    DataSet dsPharmacy =  Allscripts.Impact.Pharmacy.LoadPharmacy(pharmacyID, (ConnectionStringPointer)Session["DBID"]);
                    string addrOnePharm = dsPharmacy.Tables[0].Rows[0]["address1"].ToString().Trim();
                    string addrTwoPharm = dsPharmacy.Tables[0].Rows[0]["address2"].ToString().Trim();
                    string cityPharm = dsPharmacy.Tables[0].Rows[0]["city"].ToString().Trim();
                    string fullAddressPharm = FormatAddress(addrOnePharm, addrTwoPharm, cityPharm);
                    DataColumn dcAddressPharm = new DataColumn("Address", System.Type.GetType("System.String"));
                    dsPharmacy.Tables[0].Columns.Add(dcAddressPharm);
                    dsPharmacy.Tables[0].Rows[0]["Address"] = fullAddressPharm;

                    if (dsPharmacy != null && ds.Tables[0].Rows.Count > 0)
                    {
                        bool MOBPharmacy = false;
                        MOBPharmacy = Convert.ToBoolean(dsPharmacy.Tables[0].Rows[0]["MOBFlag"]);
                        if (MOBPharmacy)
                        {
                            string medication = ds.Tables["Prescription"].Rows[0]["MedicationName"].ToString().Trim() + ' ' + strength + ' ' + strengthUOM + ' ' + dosageFormCode;
                            DataColumn dcmedname = new DataColumn("MedName", System.Type.GetType("System.String"));

                            dsPharmacy.Tables[0].TableName = "MailOrderPharmacy";
                            ds.Tables["Prescription"].TableName = "MailOrderPrescription";
                            ds.Tables["MailOrderPrescription"].Columns.Add(dcmedname);
                            ds.Tables["MailOrderPrescription"].Rows[0]["MedName"] = medication;
                            superDS.Merge(ds.Tables["MailOrderPrescription"]);
                            superDS.Merge(dsPharmacy.Tables["MailOrderPharmacy"]);
                        }
                        else
                        {
                            string medication = ds.Tables["Prescription"].Rows[0]["MedicationName"].ToString().Trim() + ' ' + strength + ' ' + strengthUOM + ' ' + dosageFormCode;
                            DataColumn dcmedname = new DataColumn("MedName", System.Type.GetType("System.String"));
                            ds.Tables["Prescription"].TableName = "RetailPrescription";
                            dsPharmacy.Tables[0].TableName = "RetailPharmacy";
                            ds.Tables["RetailPrescription"].Columns.Add(dcmedname);
                            ds.Tables["RetailPrescription"].Rows[0]["MedName"] = medication;
                            superDS.Merge(ds.Tables["RetailPrescription"]);
                            superDS.Merge(dsPharmacy.Tables["RetailPharmacy"]);

                            
                        }
                    }
                }
                else
                {
                    string medication = ds.Tables["Prescription"].Rows[0]["MedicationName"].ToString().Trim() + ' ' + strength + ' ' + strengthUOM + ' ' + dosageFormCode;
                    DataColumn dcmedname = new DataColumn("MedName", System.Type.GetType("System.String"));
                    ds.Tables["Prescription"].TableName = "PrintedPrescription";
                    ds.Tables["PrintedPrescription"].Columns.Add(dcmedname);
                    ds.Tables["PrintedPrescription"].Rows[0]["MedName"] = medication;
                    superDS.Merge(ds.Tables["PrintedPrescription"]);

                }
            }
            else if ((Constants.PrescriptionStatus)Convert.ToInt32(ds.Tables["Prescription"].Rows[0]["Status"].ToString().Trim()) == Constants.PrescriptionStatus.PENDING_APPROVAL || (Constants.PrescriptionStatus)Convert.ToInt32(ds.Tables["Prescription"].Rows[0]["Status"].ToString().Trim()) == Constants.PrescriptionStatus.PENDING_TRANSMISSION)
            {
                string medication = ds.Tables["Prescription"].Rows[0]["MedicationName"].ToString().Trim() + ' ' + strength + ' ' + strengthUOM + ' ' + dosageFormCode;
                DataColumn dcmedname = new DataColumn("MedName", System.Type.GetType("System.String"));
                ds.Tables["Prescription"].TableName = "PendingPrescription";
                ds.Tables["PendingPrescription"].Columns.Add(dcmedname);
                ds.Tables["PendingPrescription"].Rows[0]["MedName"] = medication;
                superDS.Merge(ds.Tables["PendingPrescription"]);
            }
        }

            string patientID = string.Empty; patientID = ds1.Tables["Patient"].Rows[0]["PatientID"].ToString();
            string auditUserID = string.Empty; auditUserID = Session["USERID"].ToString();
            string retailPharmacyID = string.Empty;
            retailPharmacyID = ds1.Tables["Patient"].Rows[0]["LastPharmacyID"].ToString();
            //if (superDS.Tables["RetailPharmacy"] == null && !String.IsNullOrEmpty(retailPharmacyID) )
            //{
            //    DataSet dsPharmacy2 = Pharmacy.LoadPharmacy(retailPharmacyID, (ConnectionStringPointer)Session["DBID"]);
            //    string addrOnePharm = dsPharmacy2.Tables[0].Rows[0]["address1"].ToString().Trim();
            //    string addrTwoPharm = dsPharmacy2.Tables[0].Rows[0]["address2"].ToString().Trim();
            //    string cityPharm = dsPharmacy2.Tables[0].Rows[0]["city"].ToString().Trim();
            //    string fullAddressPharm = FormatAddress(addrOnePharm, addrTwoPharm, cityPharm);
            //    DataColumn dcAddressPharm = new DataColumn("Address", System.Type.GetType("System.String"));
            //    dsPharmacy2.Tables[0].Columns.Add(dcAddressPharm);
            //    dsPharmacy2.Tables[0].Rows[0]["Address"] = fullAddressPharm;
            //    dsPharmacy2.Tables[0].TableName = "RetailPharmacy";
            //    superDS.Merge(dsPharmacy2);
            //}
            //string mob_nabp = string.Empty;
            //mob_nabp = ds1.Tables["Patient"].Rows[0]["MOB_NABP"].ToString();
            //if (superDS.Tables["MailOrderPharmacy"] == null && !String.IsNullOrEmpty(mob_nabp))
            //{
            //    DataSet dsPharmacy2 = Pharmacy.LoadPharmacyByNABP(mob_nabp, (ConnectionStringPointer)Session["DBID"]);
            //    string addrOnePharm = dsPharmacy2.Tables[0].Rows[0]["address1"].ToString().Trim();
            //    string addrTwoPharm = dsPharmacy2.Tables[0].Rows[0]["address2"].ToString().Trim();
            //    string cityPharm = dsPharmacy2.Tables[0].Rows[0]["city"].ToString().Trim();
            //    string fullAddressPharm = FormatAddress(addrOnePharm, addrTwoPharm, cityPharm);
            //    DataColumn dcAddressPharm = new DataColumn("Address", System.Type.GetType("System.String"));
            //    dsPharmacy2.Tables[0].Columns.Add(dcAddressPharm);
            //    dsPharmacy2.Tables[0].Rows[0]["Address"] = fullAddressPharm;
            //    dsPharmacy2.Tables[0].TableName = "MailOrderPharmacy";
            //    superDS.Merge(dsPharmacy2);
            //}
        
        //swTest.WriteLine(superDS.GetXml());
        //swTest.Close();
        xslPath = Server.MapPath("Scripts/PatientReceipt.xsl");
        MemoryStream stream = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(superDS.GetXml()));
        XPathDocument document = new XPathDocument(stream);
        StringWriter writer = new StringWriter();
        XslCompiledTransform transformContent = new XslCompiledTransform();
        XsltArgumentList xslArg = new XsltArgumentList();
        transformContent.Load(xslPath);
        transformContent.Transform(document, xslArg, writer);
        _formOutput += writer.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
    }

    private string getXSLPath()
    {
        string printFormat = Request.QueryString["PrintBy"] == null ? "1Up" : Request.QueryString["PrintBy"];
        string xslPath = "Scripts/";

        switch (printFormat)
        {
            case "4Row":
                xslPath += "Row/";
                break;
            default:
                xslPath += "Up/";
                break;
        }

        return xslPath;
    }

    public string ConvertToText(string numbers)
    {
        string returnValue;
        string text = "";
        string[] bits = new string[91];
        string[] sAnd = new string[2];
        string[] hundred = new string[2];
        short flag = 0;
        int[] groups;
        short secondDigit;
        short firstDigit;
        short thirdDigit;
        short D;
        short H;
        short A;
        short x;
        string minus = "";

        numbers = numbers.Replace(",", "");

        string[] divisor = new string[] { "Trillion ", "Billion ", "Million ", "Thousand ", "" };

        //groups = new short[Information.UBound(divisor, System.Convert.ToInt32(Type.Missing)) + 1];

        groups = new int[divisor.Length];

        sAnd[0] = "";

        sAnd[1] = "and ";
        hundred[0] = "";

        hundred[1] = "Hundred ";

        if (numbers.Substring(0, 1) == "-")
        {
            minus = "minus ";
            numbers = numbers.Substring(1);
        }


        while ((numbers.Length > 0) && !(numbers.Substring(0, 1) != "0"))
        {
            numbers = numbers.Substring(1);
        }

        if (numbers == "")
        {
            returnValue = "zero";
            return returnValue;
        }

        short i;
        for (i = 1; i <= numbers.Length; i++)
        {
            //if (StringType.StrLike(System.Convert.ToString(!System.Convert.ToBoolean(numbers.Substring(i - 1, 1))), "[0-9]", CompareMethod.Binary))
            if (!(numbers.Substring(i - 1, 1) == "0") && !(numbers.Substring(i - 1, 1) == "1") && !(numbers.Substring(i - 1, 1) == "2") && !(numbers.Substring(i - 1, 1) == "3") &&
           !(numbers.Substring(i - 1, 1) == "4") && !(numbers.Substring(i - 1, 1) == "5") && !(numbers.Substring(i - 1, 1) == "6") && !(numbers.Substring(i - 1, 1) == "7") &&
           !(numbers.Substring(i - 1, 1) == "8") && !(numbers.Substring(i - 1, 1) == "9"))
            {
                returnValue = "invalid data";
                return returnValue;
            }
        }

        if (numbers.Length > ((divisor.Length - 1) + 1) * 3)
        {
            returnValue = "outside of number range";
            return returnValue;
        }

        bits[0] = "";

        bits[1] = "One ";
        bits[2] = "Two ";

        bits[3] = "Three ";
        bits[4] = "Four ";

        bits[5] = "Five ";
        bits[6] = "Six ";

        bits[7] = "Seven ";
        bits[8] = "Eight ";

        bits[9] = "Nine ";
        bits[10] = "Ten ";

        bits[11] = "Eleven ";
        bits[12] = "Twelve ";

        bits[13] = "Thirteen ";
        bits[14] = "Fourteen ";

        bits[15] = "Fifteen ";
        bits[16] = "Sixteen ";

        bits[17] = "Seventeen ";
        bits[18] = "Eighteen ";

        bits[19] = "Nineteen ";
        bits[20] = "Twenty ";

        bits[30] = "Thirty ";
        bits[40] = "Forty ";

        bits[50] = "Fifty ";
        bits[60] = "Sixty ";

        bits[70] = "Seventy ";
        bits[80] = "Eighty ";

        bits[90] = "Ninety ";

        x = (short)(divisor.Length - 1);

        while (!(numbers.Length == 0))
        {


            if (numbers.Length > 3)
                groups[x] = System.Convert.ToInt32(numbers.Substring(numbers.Length - 3, 3));
            else
                groups[x] = System.Convert.ToInt32(numbers);

            //groups[x] = System.Convert.ToInt32(numbers.Substring(numbers.Length - 3, 3));

            if (numbers.Length <= 3)
            {
                numbers = "";
            }
            else
            {

                numbers = numbers.Substring(0, numbers.Length - 3);
            }
            x--;
        }

        for (i = (short)(x + 1); i <= (divisor.Length - 1); i++) //convert in groups - 111,222,333,444,555
        {
            firstDigit = (short)(groups[i] / 100);
            secondDigit = (short)((groups[i] - (groups[i] / 100) * 100) / 10);
            thirdDigit = (short)(groups[i] - (firstDigit * 100) - (secondDigit * 10));

            if (secondDigit == 1)
            {
                secondDigit = (short)(secondDigit + 9 + thirdDigit);
                thirdDigit = 0;
            }
            else
            {

                secondDigit = (short)(secondDigit * 10);
            }

            if (i < (divisor.Length - 1) && (firstDigit + secondDigit + thirdDigit) > 0)
            {
                flag = 1; //setting flag for "and" in last group
            }

            if (firstDigit > 0)
            {
                H = 1;
            }
            else
            {
                H = 0;
            }

            if ((firstDigit > 0 || (i == (divisor.Length - 1) && flag == 1)) && (secondDigit > 0 || thirdDigit > 0))
            {
                A = 1;
            }
            else
            {
                A = 0;
            }

            if ((firstDigit + secondDigit + thirdDigit) > 0)
            {
                D = i;
            }
            else
            {
                D = (short)(divisor.Length - 1);
            }

            text = text + bits[firstDigit] + hundred[H] + sAnd[A] + bits[secondDigit] + bits[thirdDigit] + divisor[D];
        }

        returnValue = minus + text;

        return returnValue;
    }


    private string FormatAddress(string addrOne, string addrTwo, string city)
    {

        if (addrOne.Length > 0)
        {
            addrOne = addrOne + ", ";
        }

        if (addrTwo.Length > 0)
        {
            addrTwo = addrTwo + ", ";
        }

        if (city.Length > 0)
        {
            city = city + ", ";
        }

        string address = addrOne + addrTwo + city;
        return address;
    }

    private string MedicationQty(decimal Quantity, decimal PackageSize, decimal PackageQuantity, string PackageDescription, string PackUOM, string DosageDescription, int DaysSupply)
    {
        string TestStr = Convert.ToString(Quantity); ;
        string PckQty;
        string PackStr;
        string QtyOutStr;
        string DaysOutStr;
        decimal PCK_SZ;
        decimal PCK_QTY;
        decimal MET_QTY = Quantity;
        PCK_SZ = PackageSize;
        PCK_QTY = PackageQuantity;


        if (Quantity == (int)Quantity)
            TestStr = "**" + Convert.ToString((int)Quantity) + "**";

        if (PCK_SZ * PCK_QTY == 0)
            PckQty = Convert.ToString(MET_QTY);
        else
            PckQty = Convert.ToString((MET_QTY / (PCK_SZ * PCK_QTY)));

        if (PackageDescription == "EA")
            PackStr = "";
        else
            PackStr = "(" + PckQty + " X " + PackageDescription.Trim();

        if (PCK_QTY > 1)
            PackStr = PackStr + " - Pack of " + TestStr + ")";
        else if (PackStr.Length > 0)
            PackStr = PackStr + ")";

        if (Quantity > 0)
            QtyOutStr = "Quantity: " + TestStr + " " + zzUM(PackUOM, DosageDescription) + " " + PackStr + zzToWords(Quantity, PackUOM, DosageDescription);//dhiraj 141
        else
            QtyOutStr = "";

        if (DaysSupply > 0)
            DaysOutStr = "Days:" + DaysSupply.ToString();
        else
            DaysOutStr = "";

        return QtyOutStr + "  " + DaysOutStr;
    }

    private string zzUM(string PackageUOM, string DosageFormCode)
    {
        if (PackageUOM != "EA")
            return PackageUOM.ToLower();
        else
            ///Todo get description from dosage form table
            return Allscripts.Impact.Medication.GetDosageFormDesc(DosageFormCode, (ConnectionStringPointer)Session["DBID"]);
    }

    private string zzToWords(decimal Quantity, string PackageUOM, string DosageFormCode)
    {
        string frac = "";
        string whole = Convert.ToString((int)Quantity);
        if (Quantity > (int)Quantity)
        {
            frac = Convert.ToString(Quantity - (int)Quantity);
            while (frac.EndsWith("0"))
                frac = frac.Substring(0, frac.Length - 1);
            frac = frac.Substring(frac.IndexOf(".") + 1);
        }
        if (frac.Trim().Length == 0)
            return " (" + ConvertToText(whole) + " " + zzUM(PackageUOM, DosageFormCode) + ")";

        else if (frac.Trim().Length == 1)
            if (Convert.ToInt32(frac) == 0)
                return " (" + ConvertToText(whole) + " " + zzUM(PackageUOM, DosageFormCode) + ")";
            else
                return " (" + ConvertToText(whole) + " and " + ConvertToText(frac) + " tenths " + zzUM(PackageUOM, DosageFormCode) + ")";

        else if (frac.Trim().Length == 2)
            return " (" + ConvertToText(whole) + " and " + ConvertToText(frac) + " hundredths " + zzUM(PackageUOM, DosageFormCode) + ")";

        else if (frac.Trim().Length == 3)
            return " (" + ConvertToText(whole) + " and " + ConvertToText(frac) + " thousandths " + zzUM(PackageUOM, DosageFormCode) + ")";

        else if (frac.Trim().Length == 4)
            return " (" + ConvertToText(whole) + " and " + ConvertToText(frac) + " ten-thousandths " + zzUM(PackageUOM, DosageFormCode) + ")";

        else if (frac.Trim().Length == 5)
            return " (" + ConvertToText(whole) + " and " + ConvertToText(frac) + " hundred-thousandths " + zzUM(PackageUOM, DosageFormCode) + ")";

        else if (frac.Trim().Length == 6)
            return " (" + ConvertToText(whole) + " and " + ConvertToText(frac) + " millionths " + zzUM(PackageUOM, DosageFormCode) + ")";

        else
            return "";

    }
}

}