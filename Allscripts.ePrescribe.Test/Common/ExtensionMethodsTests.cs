using System;
using System.Collections.Generic;
using System.Data;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Shared.Data;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks.Constraints;
using System.Linq;

namespace Allscripts.ePrescribe.Test.Common
{
    [TestClass]
    public class ExtensionMethodsTests
    {
        [TestMethod]
        public void should_flatten_list_of_string_to_single_string_correctly()
        {
            // arrange

            var list = new List<string> {"one", "two", "three"};

            // act

            var flat = list.ListToString();

            // assert

            Assert.AreEqual("one, two, three", flat);

        }

        [TestMethod]
        public void should_print_complex_object_properties()
        {
            // arrange
            LocalLogContext.SetLoggingContext(new LoggingInfo() { EnableLogging = true });
            var parameters = new GetScriptPadOptionsParameters();

          
            parameters.MailOrderPharmacyEPCSAuthorizedSchedules = new List<string>{"one", "two"};
            parameters.RetailPharmacyEPCSAuthorizedSchedules = new List<string> {"aye", "bee"};
            parameters.IsDelegateProvider = true;

            // act

            var printed = parameters.ToLogString();

            // assert

            var expected =
                "<Allscripts.Impact.Parameters.GetScriptPadOptionsParameters><IsControlledSubstance>False</IsControlledSubstance><IsFreeFormMedControlledSubstance>False</IsFreeFormMedControlledSubstance><IsCSMedEPCSEligibleForRetail>False</IsCSMedEPCSEligibleForRetail><IsCSMedEPCSEligibleForMailOrder>False</IsCSMedEPCSEligibleForMailOrder><IsPriorAuthRequired>False</IsPriorAuthRequired><IsRetailPharmacyElectronicEnabled>False</IsRetailPharmacyElectronicEnabled><IsUserEligibleToTryEPCS>False</IsUserEligibleToTryEPCS><IsRetailPharmacyAvailable>False</IsRetailPharmacyAvailable><IsLastPharmacyAvailable>False</IsLastPharmacyAvailable><IsRetailPharmacyEPCSEnabled>False</IsRetailPharmacyEPCSEnabled><IsMailOrderPharmacyElectronicEnabled>False</IsMailOrderPharmacyElectronicEnabled><IsMailOrderPharmacyEPCSEnabled>False</IsMailOrderPharmacyEPCSEnabled><IsPatientEligibleForMailOrderBenefit>False</IsPatientEligibleForMailOrderBenefit><IsDelegateProvider>True</IsDelegateProvider><IsPrescriptionToProvider>False</IsPrescriptionToProvider><IsSendAndPrintState>False</IsSendAndPrintState><IsLicenseShieldEnabled>False</IsLicenseShieldEnabled><IsCopiedRx>False</IsCopiedRx><HasSessionSitePharmacyID>False</HasSessionSitePharmacyID><IsSiteAdmin>False</IsSiteAdmin><HasMobNABP>False</HasMobNABP><IsMobNABPEqualsX>False</IsMobNABPEqualsX><HasDUR>False</HasDUR><ShowEPA>False</ShowEPA><IsUserEPAEnabled>False</IsUserEPAEnabled><IsRxIdAssociatedWithActiveCoverage>False</IsRxIdAssociatedWithActiveCoverage><UserType><GENERAL_USER></GENERAL_USER></UserType><ReconciledCSCodeForRetailPharmacy></ReconciledCSCodeForRetailPharmacy><ReconciledCSCodeForMailOrderPharmacy></ReconciledCSCodeForMailOrderPharmacy><Status>0</Status><UserSPI></UserSPI><SiteEPCSAuthorizedSchedules></SiteEPCSAuthorizedSchedules><RetailPharmacyEPCSAuthorizedSchedules><item>aye</item><item>bee</item></RetailPharmacyEPCSAuthorizedSchedules><MailOrderPharmacyEPCSAuthorizedSchedules><item>one</item><item>two</item></MailOrderPharmacyEPCSAuthorizedSchedules><IsEAuthExcludedInfoSourcePayer>False</IsEAuthExcludedInfoSourcePayer></Allscripts.Impact.Parameters.GetScriptPadOptionsParameters>";
             

            Assert.AreEqual(expected, printed);
        }

        [TestMethod]
        public void should_handle_data_table_correctly()
        {
            // arrange 
            LocalLogContext.SetLoggingContext(new LoggingInfo() { EnableLogging = true });

            var some = new SomeClass()
            {
                SomeDateTime = new DateTime(2001, 2, 2, 0, 0, 0, 0, DateTimeKind.Utc),
                SomeInt = 100,
                SomeString = "SomeStringValue"
            };

            // act

            string printed = some.ToLogString();

            // assert

            var expectedTemplate =
                @"<Allscripts.ePrescribe.Test.Common.SomeClass><SomeString>SomeStringValue</SomeString><SomeInt>100</SomeInt><SomeDateTime>{0}</SomeDateTime><SomeTable><DocumentElement>
  <DataTable>
    <Test1>value0</Test1>
    <Test2>0</Test2>
  </DataTable>
  <DataTable>
    <Test1>value1</Test1>
    <Test2>1</Test2>
  </DataTable>
  <DataTable>
    <Test1>value2</Test1>
    <Test2>2</Test2>
  </DataTable>
</DocumentElement></SomeTable></Allscripts.ePrescribe.Test.Common.SomeClass>";

            var expected = string.Format(expectedTemplate, some.SomeDateTime);
            

            Assert.AreEqual(expected, printed);
        }

       

       
    }

    internal class SomeClass
    {
        public string SomeString { get; set; }
        public int SomeInt { get; set; }
        public DateTime SomeDateTime { get; set; }
        public DataTable SomeTable { get { return GetTable(); } }

        private static DataTable GetTable()
        {
            var table = new DataTable();

            table.Columns.Add(new DataColumn("Test1", typeof(string)));
            table.Columns.Add(new DataColumn("Test2", typeof(int)));

            for (int i = 0; i < 3; i++)
            {
                var row = table.NewRow();
                row["Test1"] = "value" + i;
                row["Test2"] = i;

                table.Rows.Add(row);
            }

            table.AcceptChanges();

            return table;
        }

        
    }
}