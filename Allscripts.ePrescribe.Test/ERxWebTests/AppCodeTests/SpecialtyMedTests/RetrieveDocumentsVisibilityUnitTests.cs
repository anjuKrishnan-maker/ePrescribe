using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb.AppCode;
using System.Collections.Generic;
using Allscripts.Impact.services.SpecialtyMedUtils;
using SpecialtyMed = eRxWeb.AppCode.SpecialtyMed;
using System.Data;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SpecialtyMedTests
{
    [TestClass]
    public class RetrieveDocumentsVisibilityUnitTests
    {
        [TestMethod]
        public void should_return_empty_dictionary_when_null_Dataset_supplied()
        {
            //Arrange
            SpecialtyMed specialtyMed = new SpecialtyMed();

            //Act
            Dictionary<AttachmentTypes, bool> documentsVisibility = specialtyMed.RetrieveDocumentsVisibility(null);

            //Assert
            Assert.AreEqual(0, documentsVisibility.Count);
        }
        [TestMethod]
        public void should_return_expected_dictionary_when_Dataset_supplied_contains_AttachmentTypes()
        {
            //Arrange
            SpecialtyMed specialtyMed = new SpecialtyMed();
            DataTable tableSpecialtyMedsAttachments = new DataTable("SpecMedAttachments");
            tableSpecialtyMedsAttachments.Columns.Add("SpecialtyMedAttachmentTypeID", typeof(int));
            tableSpecialtyMedsAttachments.Columns.Add("AttachmentContent");
            tableSpecialtyMedsAttachments.Rows.Add(0, "BLAH");
            tableSpecialtyMedsAttachments.Rows.Add(1, "BLAH");

            // Create a DataSet and put both tables in it.
            DataSet set = new DataSet("SpecMedsDataSet");
            set.Tables.Add(tableSpecialtyMedsAttachments);

            //Act
            Dictionary<AttachmentTypes, bool> documentsVisibility = specialtyMed.RetrieveDocumentsVisibility(set);

            //Assert
            Assert.AreEqual(true, documentsVisibility.ContainsKey(AttachmentTypes.PRIOR_AUTHORIZATION_FILE));
            Assert.AreEqual(true, documentsVisibility.ContainsKey(AttachmentTypes.SPECIALTY_ENROLLMENT_STATUS_FILE));
        }

        public void should_return_expected_dictionary_when_Dataset_supplied_contains_unexpected_AttachmentTypes()
        {
            //Arrange
            SpecialtyMed specialtyMed = new SpecialtyMed();
            DataTable tableSpecialtyMedsAttachments = new DataTable("SpecMedAttachments");
            tableSpecialtyMedsAttachments.Columns.Add("SpecialtyMedAttachmentTypeID", typeof(int));
            tableSpecialtyMedsAttachments.Columns.Add("AttachmentContent");
            tableSpecialtyMedsAttachments.Rows.Add(2, "BLAH");//unexpected AttachmentTypeID = 2

            // Create a DataSet and put both tables in it.
            DataSet set = new DataSet("SpecMedsDataSet");
            set.Tables.Add(tableSpecialtyMedsAttachments);

            //Act
            Dictionary<AttachmentTypes, bool> documentsVisibility = specialtyMed.RetrieveDocumentsVisibility(set);

            //Assert
            Assert.AreEqual(1, documentsVisibility.Count);
            Assert.AreEqual(false, documentsVisibility.ContainsKey(AttachmentTypes.PRIOR_AUTHORIZATION_FILE));
            Assert.AreEqual(false, documentsVisibility.ContainsKey(AttachmentTypes.SPECIALTY_ENROLLMENT_STATUS_FILE));
        }
    }
}
