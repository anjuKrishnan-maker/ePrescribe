using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eRxWeb.ePrescribeSvc;
using eRxWeb.State;

namespace eRxWeb.AppCode.Interfaces
{
    interface IEditUserSecretQuestions
    {
        SaveSecretQuestionAnswersResponse SaveSecretQuestionAnswers(ePrescribeSvc.SaveSecretQuestionAnswersRequest request, IStateContainer session);

        SecretQuestion[] GetUserShieldSecretQuestions(IStateContainer session);

        SecretQuestion[] GetAllShieldSecretQuestions();
    }
}
