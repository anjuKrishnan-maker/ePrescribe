using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Allscripts.Impact;

namespace Allscripts.ePrescribe.Test
{

    public class RxUserTests
    {
        [TestClass]
        public class IsUserPartnerUserWithStandardLoginMethodTests
        {
            [TestMethod]
            public void should_return_false_if_user_not_belongs_to_any_partner()
            {
                // Arrange
                bool actualResult;

                var userPartnerSsoFlagValue = new List<Dictionary<string, bool>>();

                //Act
                actualResult = RxUser.IsUserSsoPartnerUserWithStandardLogin(userPartnerSsoFlagValue);
               
                //Assert
                Assert.IsFalse(actualResult);
            }


            [TestMethod]
            public void should_return_false_if_user_associated_to_sso_partner_but_standard_login_not_allowed()
            {
                // Arrange
                bool actualResult;

                var userPartnerSsoFlagValue = new List<Dictionary<string, bool>>();
                userPartnerSsoFlagValue.Add( new Dictionary<string, bool>
                    {
                        {"SSOPartner", true},
                        {"IsStandardLoginAllowedForSSOUser", false}
                    });

                //Act
                actualResult = RxUser.IsUserSsoPartnerUserWithStandardLogin(userPartnerSsoFlagValue);

                //Assert
                Assert.IsFalse(actualResult);
            }


            [TestMethod]
            public void should_return_false_if_user_not_associated_to_sso_partner_but_standard_login_allowed()
            {
                // Arrange
                bool actualResult;

                var userPartnerSsoFlagValue = new List<Dictionary<string, bool>>();
                userPartnerSsoFlagValue.Add(new Dictionary<string, bool>
                    {
                        {"SSOPartner", false},
                        {"IsStandardLoginAllowedForSSOUser", true}
                    });

                //Act
                actualResult = RxUser.IsUserSsoPartnerUserWithStandardLogin(userPartnerSsoFlagValue);

                //Assert
                Assert.IsFalse(actualResult);
            }

            [TestMethod]
            public void should_return_true_if_user_belongs_multiple_partner_and_atleast_associated_with_one_sso_partner_and_standard_login_allowed()
            {
                // Arrange
                bool actualResult;

                var userPartnerSsoFlagValue = new List<Dictionary<string, bool>>();
                userPartnerSsoFlagValue.Add(
                    new Dictionary<string, bool>
                    {
                        {"SSOPartner", false},
                        {"IsStandardLoginAllowedForSSOUser", true}
                    });
                userPartnerSsoFlagValue.Add(
                    new Dictionary<string, bool>
                    {
                        {"SSOPartner", true},
                        {"IsStandardLoginAllowedForSSOUser", true}
                    });

                //Act
                actualResult = RxUser.IsUserSsoPartnerUserWithStandardLogin(userPartnerSsoFlagValue);

                //Assert
                Assert.IsTrue(actualResult);
            }

            [TestMethod]
            public void should_return_false_if_user_belongs_multiple_partner_and_not_associated_with_any_sso_partner_and_standard_login_not_allowed()
            {
                // Arrange
                bool actualResult;

                var userPartnerSsoFlagValue = new List<Dictionary<string, bool>>();
                userPartnerSsoFlagValue.Add(
                     new Dictionary<string, bool>
                    {
                        {"SSOPartner", false},
                        {"IsStandardLoginAllowedForSSOUser", true}
                    });
                userPartnerSsoFlagValue.Add(
                    new Dictionary<string, bool>
                    {
                        {"SSOPartner", true},
                        {"IsStandardLoginAllowedForSSOUser", false}
                    });
                //Act
                actualResult = RxUser.IsUserSsoPartnerUserWithStandardLogin(userPartnerSsoFlagValue);

                //Assert
                Assert.IsFalse(actualResult);
            }
        }

    }
}

