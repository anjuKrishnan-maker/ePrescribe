using System.Web.UI.WebControls;
using eRxWeb.State;
using System.Data;
using System.Text;
using eRxWeb.AppCode.Interfaces;
using Allscripts.ePrescribe.DatabaseSelector;
using System.Collections.Generic;
using Allscripts.Impact.Tasks.Interfaces;

namespace eRxWeb.AppCode
{
    public class SigAspx: ISigAspx
    {
        public static void CheckAndSetControlsForEpcs(bool canTryEpcs, string controlledSubstanceCode, Button btnApproveRequest, Controls_Message ucMessage)
        {
            if (!canTryEpcs && !string.IsNullOrWhiteSpace(controlledSubstanceCode))
            {
                btnApproveRequest.Enabled = false;
                ucMessage.MessageText = "You are not enabled to send controlled substances electronically!";
                ucMessage.Icon = Controls_Message.MessageType.ERROR;
                ucMessage.Visible = true;
            }
            else
            {
                btnApproveRequest.Enabled = true;
                ucMessage.Visible = false;
            }
        }
        public DataTable SigLoadByID(string sigID, ConnectionStringPointer dbID)
        {
            return Allscripts.ePrescribe.Data.Prescription.SigLoadBySigID(sigID, dbID);
        }
        public string GetEnglishDescriptionFromSigID(string sigID, ConnectionStringPointer dbID)
        {
            return Allscripts.ePrescribe.Data.Prescription.GetEnglishDescriptionFromSigID (sigID, dbID);
        }

        public string ComputeSigText(string sigTypeValue, Dictionary<string, string> sigTextDictionary, Dictionary<string, int> sigTextSelectedIndexDictionary)
        {
            string Sig = string.Empty;

            if (sigTypeValue == "P")
            {
                if (sigTextSelectedIndexDictionary["P"] > -1)
                {
                    Sig = sigTextDictionary["P"];
                }
            }
            else if (sigTypeValue == "A")
            {
                if (sigTextSelectedIndexDictionary["A"] > -1)
                {
                    Sig = sigTextDictionary["A"];
                }
            }
            else if (sigTypeValue == "F")
            {
                Sig = sigTextDictionary["F"];
            }
            return Sig;
        }

        public string ComputeSigID(string sigTypeValue, Dictionary<string, string> sigIDDictionary, Dictionary<string, int> sigTextSelectedIndexDictionary)
        {
            string SigID = string.Empty;

            if (sigTypeValue == "P")
            {
                if (sigTextSelectedIndexDictionary["P"] > -1)
                {
                    SigID = sigIDDictionary["P"];
                }
            }
            else if (sigTypeValue == "A")
            {
                if (sigTextSelectedIndexDictionary["A"] > -1)
                {
                    SigID = sigIDDictionary["A"];
                }
            }
            else if (sigTypeValue == "F")
            {
                SigID = sigIDDictionary["F"];
            }

            if (SigID != string.Empty)
            {
                if (SigID.Contains("[DQ="))
                {
                    int DQindex = SigID.IndexOf("[DQ=");
                    SigID = SigID.Substring(0, DQindex);
                }
            }

            return SigID;
        }
    }
}