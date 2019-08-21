using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QDataManager;
using System.Data;


namespace QBusinessManager
{
    public class QBusinessManager
    {
        public IEnumerable<QuestionChoiceResult> GetChoiceList()
        {
            //Gets the empty choice list with the main data type QuestionChoiceResult
            int NumOfRespondent;

            DataTable choiceList;
            DataTable questionsList; 

            //gets the datatables without answers to initialize the questionaire
            PrepareResultDataTable(out questionsList, out choiceList, out NumOfRespondent, false);

            //GetQuestionChoices method prepares the main data object structure by filling with data table rows 
            return GetQuestionChoices(false, choiceList, questionsList);
        }
        public IEnumerable<QuestionChoiceResult> GetAnswerList(ref int NumOfRespondent)
        {
            DataTable choiceList;
            DataTable questionsList ;
            //Gets the choice list and answer count column with the main data type QuestionChoiceResult
            //gets the datatables with answers to show the result of the questionaire
            PrepareResultDataTable(out questionsList, out choiceList, out NumOfRespondent, true);

            //Calculates the percentage of answer counts
            choiceList.Columns["ANSWER"].ReadOnly = false;
            if (NumOfRespondent > 0)
            {
                foreach (DataRow dr in choiceList.Rows)
                {
                    dr["ANSWER"] = Math.Round((Convert.ToDecimal(dr["ANSWER"]) / NumOfRespondent) * 100, 2);
                }
            }

            //GetQuestionChoices method prepares the main data object structure by filling with data table rows 
            return GetQuestionChoices(true, choiceList, questionsList);
        }
        private void PrepareResultDataTable(out DataTable questionsList, out DataTable choiceList,out int numOfRespondent, bool answerIncluded)
        {
            QDataManager.QDataManager getD = new QDataManager.QDataManager();
            //Gets the data with or without answers by the help of answerIncluded boolean parameter
            if (answerIncluded)
            {
                choiceList = getD.GetAnswerData();
                questionsList = getD.GetQuestionData();
            }
            else
            {
                choiceList = getD.GetChoiceData();
                questionsList = getD.GetQuestionData();
            }
            numOfRespondent= getD.GetNumOfRespondent();
        }
        public IEnumerable<QuestionChoiceResult> GetQuestionChoices(bool answerIncluded,DataTable choicesList,DataTable questionsList)
        {
            //creates the main data object 
            QuestionChoiceResult[] questionChoices = new QuestionChoiceResult[questionsList.Rows.Count];

            for (int i = 0; i < questionsList.Rows.Count; i++)
            {
                //assigns the values of inner objects for the whole list
                QuestionChoiceResult ar = new QuestionChoiceResult();
                int j = 0;
                Question question = new Question();
                question.QID = Convert.ToInt32(questionsList.Rows[i]["QID"].ToString());
                DataRow[] drChoices = choicesList.Select("QID='" + question.QID + "'", "CID ASC");
                Choice[] choices = new Choice[drChoices.Length];
                //for each question, choices are selected and populated
                foreach (DataRow dr in drChoices)
                {
                    Choice c = new Choice();
                    c.QID = question.QID;
                    c.CID = Convert.ToInt32(dr["CID"]);
                    c.CIDENTIFIER = dr["CIDENTIFIER"].ToString();
                    c.DESCRIPTION = dr["DESCRIPTION"].ToString();
                    //if answers included assign answer property too, if not leave it null
                    decimal answerRate = 0;
                    if (answerIncluded)
                    {
                        Decimal.TryParse(dr["ANSWER"].ToString(), out answerRate);
                        c.ANSWER_RATE = answerRate;
                    }
                    choices[j] = c;
                    j++;
                }

                //assigning the question properties
                question.QIDENTIFIER = questionsList.Rows[i]["QIDENTIFIER"].ToString();
                question.DESCRIPTION = questionsList.Rows[i]["DESCRIPTION"].ToString();
                question.CHOICETYPE = Convert.ToBoolean(questionsList.Rows[i]["CHOICETYPE"]);
                ar.Question = question;
                ar.Choices = choices;
                questionChoices[i] = ar;
            }
            return questionChoices;
        }
        public void InsertAnswers(QuestionChoiceResult[] ChoicesTable, string UserInfo)
        {
            string error = string.Empty;
            try
            {
                //Each of the user selections are being inserting to DB 
                foreach (QuestionChoiceResult c in ChoicesTable)
                {
                    //This part can by designed to use transactional insertion if the data integrity is necessary
                    foreach (Choice selectedItem in c.Choices.Where(x => x.CHECKED == true))
                    {
                        int result = InsertAnswer(selectedItem.CID, UserInfo);
                        if (result == 0)
                            error += "Question ID: " + selectedItem.CID + System.Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Your answers can not be processed.");
            }
            if (error != string.Empty)
                throw new Exception("Your answers can not be processed. Error at ");
        }
        private int InsertAnswer(int CID, string UserInfo)
        {
            QDataManager.QDataManager setD = new QDataManager.QDataManager();
            return setD.InsertAnswer(CID, UserInfo);
        }
    }
}
