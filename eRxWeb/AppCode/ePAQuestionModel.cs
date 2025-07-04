using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode
{
    public class ePAQuestionModel
    {
        public string AnswerChoiceID { get; set; }
        public string ParentQuestionID { get; set; }
        public string Value { get; set; }
        public string AddText { get; set; }
    }

    public class ePAQuestionListModel
    {
        public List<ePAQuestionModel> Answers { get; set; }
    }
}