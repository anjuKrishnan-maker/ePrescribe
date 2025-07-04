using Allscripts.Impact;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.AppCode.Users;
using eRxWeb.Controller;
using eRxWeb.ServerModel;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System.Collections.Generic;
using System.IO;

namespace Allscripts.ePrescribe.Test.UserCreationTests.AddUserTests
{
    [TestClass]
    public class AddUserTests
    {        
        public INewUserActivationCode _newUserActivationCode;
        public IEmail _email;
        [TestMethod]
        public void should_return_valid_message_when_only_print_is_selected()
        {
            // Arrange
            var controller = new UserApiController();
            UserActivationInfoModel newUserActivationModalPopUp = new UserActivationInfoModel();
            newUserActivationModalPopUp.UserCommunicationStatus = new UserCommunicationStatusModel();
            newUserActivationModalPopUp.FirstName = "FirstName";
            newUserActivationModalPopUp.LastName = "LastName";
            newUserActivationModalPopUp.UserCommunicationStatus.IsPrintChecked = true;
            string usersName = string.Concat(newUserActivationModalPopUp.FirstName + " " + newUserActivationModalPopUp.LastName);
            string message = $"User {usersName} has been successfully saved.Activation code printed.";

            // Act
            var result = controller.GetResponseMessage(newUserActivationModalPopUp);

            //Assert
            Assert.AreEqual(result.ToString(), message);



        }

        [TestMethod]
        public void should_return_valid_message_when_only_email_is_selected_and_mail_sent_successful()
        {
            // Arrange
            var controller = new UserApiController();

            UserActivationInfoModel newUserActivationModalPopUp = new UserActivationInfoModel();
            newUserActivationModalPopUp.UserCommunicationStatus = new UserCommunicationStatusModel();
            newUserActivationModalPopUp.FirstName = "FirstName";
            newUserActivationModalPopUp.LastName = "LastName";
            newUserActivationModalPopUp.PersonalEmail = "test@test.com";
            newUserActivationModalPopUp.UserCommunicationStatus.IsEmailChecked = true;
            newUserActivationModalPopUp.UserCommunicationStatus.IsEmailAttempted = true;
            newUserActivationModalPopUp.UserCommunicationStatus.IsEmailSentSuccessfully = true;
            string usersName = string.Concat(newUserActivationModalPopUp.FirstName + " " + newUserActivationModalPopUp.LastName);
            string message = $"User {usersName} has been successfully saved.Activation code sent to {newUserActivationModalPopUp.PersonalEmail}"; ;

            // Act
            var result = controller.GetResponseMessage(newUserActivationModalPopUp);

            //Assert
            Assert.AreEqual(result.ToString(), message);
        }

        [TestMethod]
        public void should_return_valid_message_when_only_email_is_selected_and_mail_sent_failed()
        {
            // Arrange
            var controller = new UserApiController();

            UserActivationInfoModel newUserActivationModalPopUp = new UserActivationInfoModel();
            newUserActivationModalPopUp.UserCommunicationStatus = new UserCommunicationStatusModel();
            newUserActivationModalPopUp.FirstName = "FirstName";
            newUserActivationModalPopUp.LastName = "LastName";
            newUserActivationModalPopUp.ActivationCode = "testcode";
            newUserActivationModalPopUp.UserCommunicationStatus.IsEmailChecked = true;
            newUserActivationModalPopUp.UserCommunicationStatus.IsEmailAttempted = true;
            newUserActivationModalPopUp.UserCommunicationStatus.IsEmailSentSuccessfully = false;
            string usersName = string.Concat(newUserActivationModalPopUp.FirstName + " " + newUserActivationModalPopUp.LastName);
            string message = $"User {usersName} has been successfully saved.However,there was an error in emailing the Activation Code. Please provide this Activation Code to the user. Activation Code = { newUserActivationModalPopUp.ActivationCode}";

            // Act
            var result = controller.GetResponseMessage(newUserActivationModalPopUp);

            //Assert
            Assert.AreEqual(result.ToString(), message);
        }

        [TestMethod]
        public void should_return_valid_message_when_email_and_print_are_selected_and_email_failed()
        {
            var controller = new UserApiController();

            UserActivationInfoModel newUserActivationModalPopUp = new UserActivationInfoModel();
            newUserActivationModalPopUp.UserCommunicationStatus = new UserCommunicationStatusModel();
            newUserActivationModalPopUp.FirstName = "FirstName";
            newUserActivationModalPopUp.LastName = "LastName";
            newUserActivationModalPopUp.ActivationCode = "testcode";
            newUserActivationModalPopUp.UserCommunicationStatus.IsPrintChecked = true;
            newUserActivationModalPopUp.UserCommunicationStatus.IsEmailChecked = true;
            newUserActivationModalPopUp.UserCommunicationStatus.IsEmailSentSuccessfully = false;
            newUserActivationModalPopUp.UserCommunicationStatus.IsEmailAttempted = true;
            string usersName = string.Concat(newUserActivationModalPopUp.FirstName + " " + newUserActivationModalPopUp.LastName);
            string message = $"User {usersName} has been successfully saved.The Activation code was printed, however there was an error in emailing the Activation Code. Please provide this Activation Code to the user. Activation Code = {newUserActivationModalPopUp.ActivationCode}";

            // Act
            var result = controller.GetResponseMessage(newUserActivationModalPopUp);

            //Assert
            Assert.AreEqual(result.ToString(), message);
        }

        [TestMethod]
        public void should_return_valid_message_when_email_andprint_are_selected_and_email_successful()
        {
            var controller = new UserApiController();

            UserActivationInfoModel newUserActivationModalPopUp = new UserActivationInfoModel();
            newUserActivationModalPopUp.UserCommunicationStatus = new UserCommunicationStatusModel();
            newUserActivationModalPopUp.FirstName = "FirstName";
            newUserActivationModalPopUp.LastName = "LastName";
            newUserActivationModalPopUp.ActivationCode = "testcode";
            newUserActivationModalPopUp.UserCommunicationStatus.IsPrintChecked = true;
            newUserActivationModalPopUp.UserCommunicationStatus.IsEmailChecked = true;
            newUserActivationModalPopUp.UserCommunicationStatus.IsEmailSentSuccessfully = true;
            newUserActivationModalPopUp.UserCommunicationStatus.IsEmailAttempted = true;
            string usersName = string.Concat(newUserActivationModalPopUp.FirstName + " " + newUserActivationModalPopUp.LastName);
            string message = $"User {usersName} has been successfully saved.Activation code printed and email sent to {newUserActivationModalPopUp.PersonalEmail}"; ; ;

            // Act
            var result = controller.GetResponseMessage(newUserActivationModalPopUp);

            //Assert
            Assert.AreEqual(result.ToString(), message);
        }

        [TestMethod]
        public void should_return_empty_when_object_is_null()
        {
            //Arrange
            var controller = new UserApiController();
            UserActivationInfoModel newUserActivationModalPopUp = null;
            string message = string.Empty;

            //Act
            var result = controller.GetResponseMessage(newUserActivationModalPopUp);

            //Assert
            Assert.AreEqual(result.ToString(), message);
        }
               
        [TestMethod]
        public void should_return_valid_message_modal_when_email_alone_selected()
        {
            //Arrange
            var controller = new UserApiController();
            UserActivationInfoModel newUserActivationModalPopUp = new UserActivationInfoModel();
            newUserActivationModalPopUp.UserCommunicationStatus = new UserCommunicationStatusModel();
            newUserActivationModalPopUp.UserCommunicationStatus.IsEmailChecked = true;
            newUserActivationModalPopUp.FirstName = "FirstName";
            newUserActivationModalPopUp.LastName = "LastName";
            newUserActivationModalPopUp.PersonalEmail = "test@test.com";
            string usersName = string.Concat(newUserActivationModalPopUp.FirstName + " " + newUserActivationModalPopUp.LastName);
            string message = $"User {usersName} has been successfully saved.Activation code sent to {newUserActivationModalPopUp.PersonalEmail}"; ;
           
            bool sentMailSuccessfully = true;
            var newUserActivationCode = MockRepository.GenerateMock<INewUserActivationCode>();
            var email = MockRepository.GenerateMock<IEmail>();
            newUserActivationCode.Stub(_ => _.EmailActivationCode(null,null,null)).IgnoreArguments().Return(sentMailSuccessfully);

            //Act
            var result = controller.ProcessNewUserCreation(newUserActivationCode, email, newUserActivationModalPopUp);

            //Assert            
            Assert.AreEqual(result.Message, message);
        }        

        [TestMethod]
        public void should_return_false_if_object_is_null()
        {
            //Arrange
            var newUserActivation = new NewUserActivationCode();
            UserActivationInfoModel newUserActivationModalPopUp = new UserActivationInfoModel();
            var email = MockRepository.GenerateMock<IEmail>();

            //Act
            bool result = newUserActivation.EmailActivationCode(email, null, newUserActivationModalPopUp);

            //Assert
            Assert.IsFalse(result);
        }        
    }
    
}
