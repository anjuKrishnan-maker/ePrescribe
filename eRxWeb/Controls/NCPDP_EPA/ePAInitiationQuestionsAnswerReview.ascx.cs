using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
    public partial class Controls_ePAInitiationQuestionsAnswerReview : BaseControl
    {
        private EPAInitQuestionSet _baseQuestionSet
        {
            get
            {
                EPAInitQuestionSet ret = new EPAInitQuestionSet();
                if (Session["ePACurrentInitiationQuestionSet"] != null)
                {
                    ret = (EPAInitQuestionSet)(Session["ePACurrentInitiationQuestionSet"]);
                }
                else
                {
                    ret = new EPAInitQuestionSet();
                }

                return ret;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            loadReview();
        }

        public void Hide()
        {
            fileUploader.Attributes["src"] = string.Empty;
            questionReviewHolder.Style["display"] = "none";
        }

        public void Show()
        {
            fileUploader.Attributes["src"] = "~/" + Constants.PageNames.EPA_INIT_QA_FILE_UPLOADER;
            loadReview();
            questionReviewHolder.Style["display"] = "inline";
        }

        private void loadReview()
        {
            questionReview.Controls.Clear();
            string answerList = string.Empty;
            string comments = string.Empty;
            string queNum = string.Empty;
            HtmlTable reviewTable = new HtmlTable();
            HtmlTableRow newTableRow = new HtmlTableRow();
            HtmlTableCell colOne = new HtmlTableCell();
            HtmlTableCell colTwo = new HtmlTableCell();
            HtmlTableCell colThree = new HtmlTableCell();
            List<EPAInitQuestionSetDetail> answeredQuestions = _baseQuestionSet.GetAnsweredQuestions();

            //create header row
            colOne.InnerText = "";
            colOne.Attributes["class"] = "ePAHeaderRowColNum";

            colTwo.InnerText = "Question";
            colTwo.Attributes["class"] = "ePAHeaderRowCol";

            colThree.InnerText = "Answers";
            colThree.Attributes["class"] = "ePAHeaderRowCol";

            newTableRow.Cells.Add(colOne);
            newTableRow.Cells.Add(colTwo);
            newTableRow.Cells.Add(colThree);

            reviewTable.Rows.Add(newTableRow);
            reviewTable.Attributes["class"] = "ePAReviewTable";
         
            for (int i = 0; i < answeredQuestions.Count; i++)
            {
                answerList = string.Empty;
                comments = string.Empty;
                queNum = (i + 1).ToString();
                newTableRow = new HtmlTableRow();
                colOne = new HtmlTableCell();
                colTwo = new HtmlTableCell();
                colThree = new HtmlTableCell();

                if (i % 2 != 0)
                {
                    colOne.Attributes["class"] = "ePAAltRowColNum";
                    colTwo.Attributes["class"] = "ePAAltRowCol";
                    colThree.Attributes["class"] = "ePAAltRowCol";
                }
                else
                {
                    colOne.Attributes["class"] = "ePARowColNum";
                    colTwo.Attributes["class"] = "ePARowCol";
                    colThree.Attributes["class"] = "ePARowCol";
                }

                colOne.InnerText = queNum;
                colTwo.InnerText = answeredQuestions[i].QuestionText;
                colThree.InnerHtml = answeredQuestions[i].DisplayAnswerInHTML;
                
                newTableRow.Cells.Add(colOne);
                newTableRow.Cells.Add(colTwo);
                newTableRow.Cells.Add(colThree);

                reviewTable.Rows.Add(newTableRow);
            }

            questionReview.Controls.Add(reviewTable);
        }
    } 
}