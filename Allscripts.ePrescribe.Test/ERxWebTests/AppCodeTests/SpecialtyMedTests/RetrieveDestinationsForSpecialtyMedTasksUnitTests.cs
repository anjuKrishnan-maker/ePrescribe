using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb.AppCode;
using eRxWeb.AppCode.SpecialtyMedWorkflow;
using Allscripts.Impact;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SpecialtyMedTests
{
    [TestClass]
    public class RetrieveDestinationsForSpecialtyMedTasksUnitTests
    {
        //[TestMethod]
        //public void should_always_contain_EIE_And_Print_in_Returned_Dictionary() // not if they were already processsed
        //{
        //    //Arrange
        //    SpecialtyMed specialtyMed = new SpecialtyMed();

        //    //Act
        //    Dictionary<string, string> destinationsDictionary = specialtyMed.RetrieveDestinationsForSpecialtyMedTasks(new SpecialtyMedDestinationOptionsParameters());

        //    //Assert
        //    bool doesDictionaryContainEIEKey = destinationsDictionary.ContainsKey(Patient.ENTERED_IN_ERROR_DESTINATION_OPTION);
        //    bool doesDictionaryContainEIEValue = destinationsDictionary.ContainsValue(Patient.ENTERED_IN_ERROR);
        //    Assert.AreEqual(true, doesDictionaryContainEIEKey);
        //    Assert.AreEqual(true, doesDictionaryContainEIEValue);
        //    bool doesDictionaryContainPrintKey = destinationsDictionary.ContainsKey(Patient.PRINT_KEY);
        //    bool doesDictionaryContainPrintValue = destinationsDictionary.ContainsValue(Patient.PRINT);
        //    Assert.AreEqual(true, doesDictionaryContainPrintKey);
        //    Assert.AreEqual(true, doesDictionaryContainPrintValue);
        //}
    }
}
