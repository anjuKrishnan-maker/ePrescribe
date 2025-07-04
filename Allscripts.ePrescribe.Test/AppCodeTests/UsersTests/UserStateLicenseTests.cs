using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eRxWeb.AppCode.Interfaces;
using Allscripts.ePrescribe.Test.Common;
using System.Web;
using eRxWeb.ServerModel;
using eRxWeb.State;
using static Allscripts.ePrescribe.Common.Constants;

namespace Allscripts.ePrescribe.Test.AppCodeTests.UsersTests
{
    [TestClass]
    public class UserStateLicenseTests
    {
        MockedObjects mockedObjects = new MockedObjects();
        IStateContainer session;

        [TestMethod]
        public void Should_Return_State_Licenses_Existing_For_A_User() 
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            session = new StateContainer(HttpContext.Current.Session);
            var mockUser = "";
            var userStateLicenseMock = MockRepository.GenerateMock<IUserStateLicense>();
            userStateLicenseMock.Stub(_=>_.GetStateLicenses(session, mockUser)).Return(mockedObjects.mockedStateLicenseList());
            var response = userStateLicenseMock.GetStateLicenses(session, session.GetStringOrEmpty(SessionVariables.UserId).ToString());
            Assert.IsNotNull(response);
        }        
    }
}
