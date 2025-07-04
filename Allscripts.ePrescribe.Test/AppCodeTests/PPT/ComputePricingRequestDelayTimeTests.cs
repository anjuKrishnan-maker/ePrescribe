using eRxWeb.AppCode.PptPlusBPL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allscripts.ePrescribe.Test.AppCodeTests.PPT
{
   
    [TestClass]
    public class ComputePricingRequestDelayTimeTests
    {
   
        [TestInitialize]
        public void init()
        {
        }

        [TestMethod, TestCategory("PPT")]
        public void should_return_difference_if_higher_is_greater_than_lower_and_difference_less_than_max_value()
        {
            //arrange
            DateTime higherDT = DateTime.Now;
            DateTime lowerDT = higherDT.AddSeconds(-1);

            //act
            int actual = PPTPlus.ComputePricingRequestDelayTime(higherDT, lowerDT, 4000);

            //assert
            Assert.AreEqual(3000, actual);
        }

        [TestMethod, TestCategory("PPT")]
        public void should_return_max_value_if_higher_is_lesser_than_lower()
        {
            //arrange
            DateTime higherDT = DateTime.Now;
            DateTime lowerDT = higherDT.AddSeconds(1);

            //act
            int actual = PPTPlus.ComputePricingRequestDelayTime(higherDT, lowerDT, 4000);

            //assert
            Assert.AreEqual(4000, actual);
        }

        [TestMethod, TestCategory("PPT")]
        public void should_return_zero_if_higher_and_lower_difference_more_than_max_value()
        {
            //arrange
            DateTime higherDT = DateTime.Now;
            DateTime lowerDT = higherDT.AddSeconds(-5);

            //act
            int actual = PPTPlus.ComputePricingRequestDelayTime(higherDT, lowerDT, 4000);

            //assert
            Assert.AreEqual(0, actual);
        }


        [TestMethod, TestCategory("PPT")]
        public void should_return_max_value_if_input_date_differ_and_total_milliseconds_is_zero()
        {
            //arrange
            DateTime TrnsReceivedTime = DateTime.UtcNow;
            DateTime utcNow = TrnsReceivedTime.AddTicks(1000);

            //act
            int actual = PPTPlus.ComputePricingRequestDelayTime(utcNow, TrnsReceivedTime, 4000);

            //assert
            Assert.AreEqual(4000, actual);
        }
    }
}
