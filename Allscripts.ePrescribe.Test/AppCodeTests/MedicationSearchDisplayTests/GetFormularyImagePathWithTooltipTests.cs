using System;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.AppCodeTests.MedicationSearchDisplayTests
{
    [TestClass]
    public class GetFormularyImagePathWithTooltipTests
    {
        [TestMethod]
        public void should_return_blank_gif_and_tooltip_for_formulary_0()
        {
            int formularyStatus = 0; //[0-7]
            bool isOTC = false;
            string imageUrl = string.Empty;
            string tooltip = string.Empty;

            MedicationSearchDisplay.GetFormularyImagePathWithTooltip(formularyStatus, isOTC, out imageUrl, out tooltip);

            string expectedImageUrl = "images/fs_blank.gif";
            string expectedTooltip = string.Empty;

            Assert.AreEqual(expectedImageUrl, imageUrl);
            Assert.AreEqual(expectedTooltip, tooltip);
        }

        [TestMethod]
        public void should_return_blank_gif_and_tooltip_for_formulary_1_otc_false()
        {
            int formularyStatus = 1; 
            bool isOTC = false;
            string imageUrl = string.Empty;
            string tooltip = string.Empty;

            MedicationSearchDisplay.GetFormularyImagePathWithTooltip(formularyStatus, isOTC, out imageUrl, out tooltip);

            string expectedImageUrl = "images/smile.gif";
            string expectedTooltip = "Preferred status";

            Assert.AreEqual(expectedImageUrl, imageUrl);
            Assert.AreEqual(expectedTooltip, tooltip);
        }

        [TestMethod]
        public void should_return_blank_gif_and_tooltip_for_formulary_1_otc_true()
        {
            int formularyStatus = 1;
            bool isOTC = true;
            string imageUrl = string.Empty;
            string tooltip = string.Empty;

            MedicationSearchDisplay.GetFormularyImagePathWithTooltip(formularyStatus, isOTC, out imageUrl, out tooltip);

            string expectedImageUrl = "images/fs_smiley_OTC.gif";
            string expectedTooltip = "Preferred status";

            Assert.AreEqual(expectedImageUrl, imageUrl);
            Assert.AreEqual(expectedTooltip, tooltip);
        }

        [TestMethod]
        public void should_return_blank_gif_and_tooltip_for_formulary_2_otc_false()
        {
            int formularyStatus = 2;
            bool isOTC = false;
            string imageUrl = string.Empty;
            string tooltip = string.Empty;

            MedicationSearchDisplay.GetFormularyImagePathWithTooltip(formularyStatus, isOTC, out imageUrl, out tooltip);

            string expectedImageUrl = "images/fs_yellow.gif";
            string expectedTooltip = "Approved status";

            Assert.AreEqual(expectedImageUrl, imageUrl);
            Assert.AreEqual(expectedTooltip, tooltip);
        }

        [TestMethod]
        public void should_return_blank_gif_and_tooltip_for_formulary_2_otc_true()
        {
            int formularyStatus = 2;
            bool isOTC = true;
            string imageUrl = string.Empty;
            string tooltip = string.Empty;

            MedicationSearchDisplay.GetFormularyImagePathWithTooltip(formularyStatus, isOTC, out imageUrl, out tooltip);

            string expectedImageUrl = "images/fs_yellow_OTC.gif";
            string expectedTooltip = "Approved status";

            Assert.AreEqual(expectedImageUrl, imageUrl);
            Assert.AreEqual(expectedTooltip, tooltip);
        }

        [TestMethod]
        public void should_return_blank_gif_and_tooltip_for_formulary_3_otc_false()
        {
            int formularyStatus = 3;
            bool isOTC = false;
            string imageUrl = string.Empty;
            string tooltip = string.Empty;

            MedicationSearchDisplay.GetFormularyImagePathWithTooltip(formularyStatus, isOTC, out imageUrl, out tooltip);

            string expectedImageUrl = "images/fs_red.gif";
            string expectedTooltip = "Non-approved status";

            Assert.AreEqual(expectedImageUrl, imageUrl);
            Assert.AreEqual(expectedTooltip, tooltip);
        }

        [TestMethod]
        public void should_return_blank_gif_and_tooltip_for_formulary_3_otc_true()
        {
            int formularyStatus = 3;
            bool isOTC = true;
            string imageUrl = string.Empty;
            string tooltip = string.Empty;

            MedicationSearchDisplay.GetFormularyImagePathWithTooltip(formularyStatus, isOTC, out imageUrl, out tooltip);

            string expectedImageUrl = "images/fs_red_OTC.gif";
            string expectedTooltip = "Non-approved status";

            Assert.AreEqual(expectedImageUrl, imageUrl);
            Assert.AreEqual(expectedTooltip, tooltip);
        }

        [TestMethod]
        public void should_return_blank_gif_and_tooltip_for_formulary_4_otc_false()
        {
            int formularyStatus = 4;
            bool isOTC = false;
            string imageUrl = string.Empty;
            string tooltip = string.Empty;

            MedicationSearchDisplay.GetFormularyImagePathWithTooltip(formularyStatus, isOTC, out imageUrl, out tooltip);

            string expectedImageUrl = "images/smile.gif";
            string expectedTooltip = "Prior authorization is required";

            Assert.AreEqual(expectedImageUrl, imageUrl);
            Assert.AreEqual(expectedTooltip, tooltip);
        }

        [TestMethod]
        public void should_return_blank_gif_and_tooltip_for_formulary_4_otc_true()
        {
            int formularyStatus = 4;
            bool isOTC = true;
            string imageUrl = string.Empty;
            string tooltip = string.Empty;

            MedicationSearchDisplay.GetFormularyImagePathWithTooltip(formularyStatus, isOTC, out imageUrl, out tooltip);

            string expectedImageUrl = "images/fs_smiley_OTC.gif";
            string expectedTooltip = "Prior authorization is required";

            Assert.AreEqual(expectedImageUrl, imageUrl);
            Assert.AreEqual(expectedTooltip, tooltip);
        }

        [TestMethod]
        public void should_return_blank_gif_and_tooltip_for_formulary_5()
        {
            int formularyStatus = 5; 
            bool isOTC = false;
            string imageUrl = string.Empty;
            string tooltip = string.Empty;

            MedicationSearchDisplay.GetFormularyImagePathWithTooltip(formularyStatus, isOTC, out imageUrl, out tooltip);

            string expectedImageUrl = "images/fs_otc.gif";
            string expectedTooltip = "Over-the-counter drug not covered by insurance";

            Assert.AreEqual(expectedImageUrl, imageUrl);
            Assert.AreEqual(expectedTooltip, tooltip);
        }

        [TestMethod]
        public void should_return_blank_gif_and_tooltip_for_formulary_6_otc_false()
        {
            int formularyStatus = 6;
            bool isOTC = false;
            string imageUrl = string.Empty;
            string tooltip = string.Empty;

            MedicationSearchDisplay.GetFormularyImagePathWithTooltip(formularyStatus, isOTC, out imageUrl, out tooltip);

            string expectedImageUrl = "images/fs_red_NR.gif";
            string expectedTooltip = "Non-reimbursable";

            Assert.AreEqual(expectedImageUrl, imageUrl);
            Assert.AreEqual(expectedTooltip, tooltip);
        }

        [TestMethod]
        public void should_return_blank_gif_and_tooltip_for_formulary_6_otc_true()
        {
            int formularyStatus = 6;
            bool isOTC = true;
            string imageUrl = string.Empty;
            string tooltip = string.Empty;

            MedicationSearchDisplay.GetFormularyImagePathWithTooltip(formularyStatus, isOTC, out imageUrl, out tooltip);

            string expectedImageUrl = "images/fs_red_NR_OTC.gif";
            string expectedTooltip = "Non-reimbursable OTC";

            Assert.AreEqual(expectedImageUrl, imageUrl);
            Assert.AreEqual(expectedTooltip, tooltip);
        }

        [TestMethod]
        public void should_return_blank_gif_and_tooltip_for_formulary_7()
        {
            int formularyStatus = 7;
            bool isOTC = false;
            string imageUrl = string.Empty;
            string tooltip = string.Empty;

            MedicationSearchDisplay.GetFormularyImagePathWithTooltip(formularyStatus, isOTC, out imageUrl, out tooltip);

            string expectedImageUrl = "images/question.png";
            string expectedTooltip = "Unknown status";

            Assert.AreEqual(expectedImageUrl, imageUrl);
            Assert.AreEqual(expectedTooltip, tooltip);
        }
    }
}
