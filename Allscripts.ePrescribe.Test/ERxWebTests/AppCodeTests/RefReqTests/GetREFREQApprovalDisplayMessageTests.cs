using System;
using eRxWeb.AppCode;
using eRxWeb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.RefReqTests
{
    [TestClass]
    public class GetREFREQApprovalDisplayMessageTests
    {
        [TestMethod]
        public void should_return_minimal_String_when_all_parameters_null()
        {
            //Arrange

            //Act
            string actualString = new RefReq().GetREFREQApprovalDisplayMessage(null, null, null);

            //Assert
            Assert.AreEqual("Refill approved", actualString);
        }

        [TestMethod]
        public void should_return_Valid_String_when_all_parameters_supplied_whether_empty_Or_Not()
        {
            //Arrange

            //Act

            //Assert
            Assert.AreEqual("Refill for ADDERALL approved", new RefReq().GetREFREQApprovalDisplayMessage("ADDERALL", null, null));
            Assert.AreEqual("Refill approved for Ankit Singh", new RefReq().GetREFREQApprovalDisplayMessage(null, "Ankit", "Singh"));
            Assert.AreEqual("Refill for ADDERALL approved for Ankit Singh", new RefReq().GetREFREQApprovalDisplayMessage("ADDERALL", "Ankit", "Singh"));
        }

        [TestMethod]
        public void should_return_parameterized_query_string_values()
        {
            //arrange
            var urlParams = "RefillPharmacy=Brooklyn @ Gates Pharmacy, 92 Gates Ave , Brooklyn, NY, (347) 425-1221&RxDetails=Tylenol with Codeine #3 300-30 MG Oral Tablet, SIG: TAKE 1 TABLET TWICE DAILY AS NEEDED FOR PAIN., Quantity: 30, Days Supply: 15, Fills: 30, Original Rx Date: 7/18/2019, Last Fill Date: 6/11/2019, Notes: Medication Dispensed&from=ApproveRefillTask.aspx&SearchText=Tylenol";
            var expectedParams = "{%22RefillPharmacy%22%3a+%22Brooklyn+%40+Gates+Pharmacy%2c+92+Gates+Ave+%2c+Brooklyn%2c+NY%2c+(347)+425-1221%22%2c%22RxDetails%22%3a+%22Tylenol+with+Codeine+%233+300-30+MG+Oral+Tablet%2c+SIG%3a+TAKE+1+TABLET+TWICE+DAILY+AS+NEEDED+FOR+PAIN.%2c+Quantity%3a+30%2c+Days+Supply%3a+15%2c+Fills%3a+30%2c+Original+Rx+Date%3a+7%2f18%2f2019%2c+Last+Fill+Date%3a+6%2f11%2f2019%2c+Notes%3a+Medication+Dispensed%22%2c%22from%22%3a+%22ApproveRefillTask.aspx%22%2c%22SearchText%22%3a+%22Tylenol%22}";

            //act
            var result = JsonHelper.ConvertUrlParameters(urlParams);

            //assert
            Assert.AreEqual(expectedParams, result);
        }
    }
}
