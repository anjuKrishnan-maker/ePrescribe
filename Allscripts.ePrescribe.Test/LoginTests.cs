using eRxWeb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allscripts.ePrescribe.Test
{
    public class LoginTests
    {
        [TestClass]
        public class IsDefaultRedirectRequiredTests
        {
            [TestMethod]
            public void should_return_true_when_redirectUrl_is_empty()
            {
                //arrange                
                string redirect_url = "";

                //act
                bool isDefaultRedirect = new _Default().IsDefaultRedirectRequired(redirect_url);

                //assert
                Assert.IsTrue(isDefaultRedirect);
            }           

            [TestMethod]
            public void should_return_true_when_redirectUrl_is_searchpatient()
            {
                //arrange                
                string redirect_url = "~/selectpatient";

                //act
                bool isDefaultRedirect = new _Default().IsDefaultRedirectRequired(redirect_url);

                //assert
                Assert.IsTrue(isDefaultRedirect);
            }

            [TestMethod]
            public void should_return_true_when_redirectUrl_is_default()
            {
                //arrange                
                string redirect_url = "~/selectpatient";

                //act
                bool isDefaultRedirect = new _Default().IsDefaultRedirectRequired(redirect_url);

                //assert
                Assert.IsTrue(isDefaultRedirect);
            }

            [TestMethod]
            public void should_return_false_when_redirectUrl_is_anyotherpage()
            {
                //arrange                
                string redirect_url = "~/resetpassword.aspx";

                //act
                bool isDefaultRedirect = new _Default().IsDefaultRedirectRequired(redirect_url);

                //assert
                Assert.IsFalse(isDefaultRedirect);
            }
        }
    }
}
