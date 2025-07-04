using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb;
using eRxWeb.State;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SigAspxTests
{
    [TestClass]
    public class SigAspxHelperTests
    {
        
        [TestMethod]
        public void function_GetEnglishDescriptionFromSigID_returns_EmptyString_If_Exception_Handled_iInside_FunctionCall()
        {
            //Arrange
            string sigId = "HelloWorld";
            ConnectionStringPointer dbPtr = ConnectionStringPointer.SHARED_DB;


            //Act
            string returnVal = new SigAspx().GetEnglishDescriptionFromSigID(sigId, dbPtr);


            //Assert
            Assert.AreEqual(returnVal, String.Empty);
        }
        [TestMethod]
        public void function_SigText_returns_valid_SigText_when_Correct_Input_Supplied()
        {
            //Arrange
            string LstPreferedSigSelectedItemText = "Prefered Text";
            string LstSigSelectedItemText = "All Text";
            string txtFreeTextSigText = "Free form text";
            

            int LstPreferedSigSelectedIndex = 2;
            int LstSigSelectedIndex = 2;

            Dictionary<string, string> sigTextDictionary = new Dictionary<string, string>
            {
                { "P", LstPreferedSigSelectedItemText},
                { "A", LstSigSelectedItemText },
                { "F", txtFreeTextSigText}
            };
            Dictionary<string, int> sigTextSelectedIndexDictionary = new Dictionary<string, int>
            {
                { "P", LstPreferedSigSelectedIndex},
                { "A", LstSigSelectedIndex }
            };

            //Act and Assert
            Assert.AreEqual("Prefered Text", new SigAspx().ComputeSigText("P", sigTextDictionary, sigTextSelectedIndexDictionary));
            Assert.AreEqual("All Text", new SigAspx().ComputeSigText("A", sigTextDictionary, sigTextSelectedIndexDictionary));
            Assert.AreEqual("Free form text", new SigAspx().ComputeSigText("F", sigTextDictionary, sigTextSelectedIndexDictionary));
        }

        [TestMethod]
        public void function_SigText_returns_empty_SigText_when_Invalid_Input_Supplied()
        {
            //Arrange
            string LstPreferedSigSelectedItemText = "Prefered Text";
            string LstSigSelectedItemText = "All Text";
            string txtFreeTextSigText = "Free form text";


            int LstPreferedSigSelectedIndex = -99;
            int LstSigSelectedIndex = -99;

            Dictionary<string, string> sigTextDictionary = new Dictionary<string, string>
            {
                { "P", LstPreferedSigSelectedItemText},
                { "A", LstSigSelectedItemText },
                { "F", txtFreeTextSigText}
            };
            Dictionary<string, int> sigTextSelectedIndexDictionary = new Dictionary<string, int>
            {
                { "P", LstPreferedSigSelectedIndex},
                { "A", LstSigSelectedIndex }
            };

            //Act and Assert
            Assert.AreEqual(string.Empty, new SigAspx().ComputeSigText("P", sigTextDictionary, sigTextSelectedIndexDictionary));
            Assert.AreEqual(string.Empty, new SigAspx().ComputeSigText("A", sigTextDictionary, sigTextSelectedIndexDictionary));
        }
        [TestMethod]
        public void function_SigID_returns_valid_SigID_when_Correct_Input_Supplied()
        {
            //Arrange

            string LstPreferedSigSelectedValue = "Prefered ID";
            string LstSigSelectedValue = "All ID";

            int LstPreferedSigSelectedIndex = 2;
            int LstSigSelectedIndex = 2;
            
            Dictionary<string, string> sigIDDictionary = new Dictionary<string, string>
            {
                { "P", LstPreferedSigSelectedValue},
                { "A", LstSigSelectedValue }
            };
            Dictionary<string, int> sigTextSelectedIndexDictionary = new Dictionary<string, int>
            {
                { "P", LstPreferedSigSelectedIndex},
                { "A", LstSigSelectedIndex }
            };

            //Act and Assert
            Assert.AreEqual("Prefered ID", new SigAspx().ComputeSigID("P", sigIDDictionary, sigTextSelectedIndexDictionary));
            Assert.AreEqual("All ID", new SigAspx().ComputeSigID("A", sigIDDictionary, sigTextSelectedIndexDictionary));
        }

        [TestMethod]
        public void function_SigID_returns_empty_SigID_when_Invalid_Input_Supplied()
        {
            //Arrange
            string LstPreferedSigSelectedValue = "Prefered ID";
            string LstSigSelectedValue = "All ID";

            int LstPreferedSigSelectedIndex = -99;
            int LstSigSelectedIndex = -99;

            Dictionary<string, string> sigIDDictionary = new Dictionary<string, string>
            {
                { "P", LstPreferedSigSelectedValue},
                { "A", LstSigSelectedValue }
            };
            Dictionary<string, int> sigTextSelectedIndexDictionary = new Dictionary<string, int>
            {
                { "P", LstPreferedSigSelectedIndex},
                { "A", LstSigSelectedIndex }
            };

            //Act and Assert
            Assert.AreEqual(string.Empty, new SigAspx().ComputeSigID("P", sigIDDictionary, sigTextSelectedIndexDictionary));
            Assert.AreEqual(string.Empty, new SigAspx().ComputeSigID("A", sigIDDictionary, sigTextSelectedIndexDictionary));
        }

    }

}
