using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Allscripts.Impact;
using System.Data;

namespace Allscripts.ePrescribe.Test.Common
{
    [TestClass]
    public class GenericListToDataTable
    {
        [TestMethod]
        public void should_return_proper_datatable_for_genric_list()
        {
            //arrange
            List<Allscripts.ePrescribe.Data.PatientDiagnosis> listSnoMedTest = new List<Allscripts.ePrescribe.Data.PatientDiagnosis>()
            {
                new Allscripts.ePrescribe.Data.PatientDiagnosis() {ICD9Code="11111", ICD10Code="22222", SnomedCode="33333", Description="fever"},
                new Allscripts.ePrescribe.Data.PatientDiagnosis() {ICD9Code="11112", ICD10Code="22223", SnomedCode="33334", Description="fever with cold"},
                new Allscripts.ePrescribe.Data.PatientDiagnosis() {ICD9Code="11113", ICD10Code="22224", SnomedCode="33335", Description="fever with headache"}
            };

            DataTable dtExpected = new DataTable();
            dtExpected.Columns.Add("ICD9Code", typeof(String));
            dtExpected.Columns.Add("ICD10Code", typeof(String));
            dtExpected.Columns.Add("SnomedCode", typeof(String));
            dtExpected.Columns.Add("Description", typeof(String));

            foreach (var patientDiag in listSnoMedTest)
            {
                DataRow ro = dtExpected.NewRow();
                ro["ICD9Code"] = patientDiag.ICD9Code;
                ro["ICD10Code"] = patientDiag.ICD10Code;
                ro["SnomedCode"] = patientDiag.SnomedCode;
                ro["Description"] = patientDiag.Description;
                dtExpected.Rows.Add(ro);
            }

            //act

            DataTable dtlistSnoMedTest =  Allscripts.Impact.Utilities.DataTableHelpers.ToDataTable<Allscripts.ePrescribe.Data.PatientDiagnosis>(listSnoMedTest);


            //assert
            Assert.AreEqual(dtlistSnoMedTest.Rows[0]["ICD9Code"], dtExpected.Rows[0]["ICD9Code"]);
            Assert.AreEqual(dtlistSnoMedTest.Rows[0]["ICD10Code"], dtExpected.Rows[0]["ICD10Code"]);
            Assert.AreEqual(dtlistSnoMedTest.Rows[0]["SnomedCode"], dtExpected.Rows[0]["SnomedCode"]);
            Assert.AreEqual(dtlistSnoMedTest.Rows[0]["Description"], dtExpected.Rows[0]["Description"]);

            Assert.AreEqual(dtlistSnoMedTest.Rows[1]["ICD9Code"], dtExpected.Rows[1]["ICD9Code"]);
            Assert.AreEqual(dtlistSnoMedTest.Rows[1]["ICD10Code"], dtExpected.Rows[1]["ICD10Code"]);
            Assert.AreEqual(dtlistSnoMedTest.Rows[1]["SnomedCode"], dtExpected.Rows[1]["SnomedCode"]);
            Assert.AreEqual(dtlistSnoMedTest.Rows[1]["Description"], dtExpected.Rows[1]["Description"]);

            Assert.AreEqual(dtlistSnoMedTest.Rows[2]["ICD9Code"], dtExpected.Rows[2]["ICD9Code"]);
            Assert.AreEqual(dtlistSnoMedTest.Rows[2]["ICD10Code"], dtExpected.Rows[2]["ICD10Code"]);
            Assert.AreEqual(dtlistSnoMedTest.Rows[2]["SnomedCode"], dtExpected.Rows[2]["SnomedCode"]);
            Assert.AreEqual(dtlistSnoMedTest.Rows[2]["Description"], dtExpected.Rows[2]["Description"]);

        }
    }
}
