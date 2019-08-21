using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Data;

namespace QBusinessManager.Tests
{
    public class QuestionChoiceResultComparer : Comparer<QuestionChoiceResult>
    {
        public override int Compare(QuestionChoiceResult x, QuestionChoiceResult y)
        {
            int i = 0;
            i= x.Question.CHOICETYPE.CompareTo(y.Question.CHOICETYPE);
            i += x.Question.DESCRIPTION.CompareTo(y.Question.DESCRIPTION);
            i += x.Question.QID.CompareTo(y.Question.QID);
            i += x.Question.QIDENTIFIER.CompareTo(y.Question.QIDENTIFIER);
            i += x.Choices.Length.CompareTo(y.Choices.Length);

            return i;
        }
    }
    [TestFixture]
    public class QBusinessManagerTests
    {
        private QBusinessManager bManager;

        [SetUp]
        public void Setup()
        {
            bManager = new QBusinessManager();
        }

        [TearDown]
        public void Teardown()
        {
            bManager = null;
        }
        [Test]
        public void GetAnswerList_GetQuestionChoices_ReturnsExpectedData()
        {
            QuestionChoiceResult[] expectedResult = new QuestionChoiceResult[1];
            QuestionChoiceResult questionChoice = new QuestionChoiceResult();
            questionChoice.Question = new Question();
            questionChoice.Question.QID = 1;
            Choice[] choice = new Choice[1];
            Choice c = new Choice();
            c.QID = 1;
            c.CID = 1;
            c.CIDENTIFIER = "A";
            c.DESCRIPTION = "Test Choice";
            c.ANSWER_RATE = Convert.ToDecimal(50);
            choice[0] = c;
            questionChoice.Question.QIDENTIFIER = "1";
            questionChoice.Question.DESCRIPTION = "Test Question";
            questionChoice.Question.CHOICETYPE = false;
            questionChoice.Choices = choice;
            expectedResult[0] = questionChoice;

            //sending my list to prepare main object
            IEnumerable<QuestionChoiceResult> returnResult = bManager.GetQuestionChoices(true, QBusinessManagerTests.Getanswers(),QBusinessManagerTests.Getquestions());
            QuestionChoiceResult[] returnResultConvert = returnResult.ToArray();

            CollectionAssert.IsNotEmpty(returnResultConvert);
            Assert.AreEqual(expectedResult.Length, returnResultConvert.Count());
            CollectionAssert.AreEqual(expectedResult, returnResultConvert, new QuestionChoiceResultComparer());
        }
        [Test]
        public void GetChoiceList_GetQuestionChoices_ReturnsExpectedData()
        {
            QuestionChoiceResult[] expectedResult = new QuestionChoiceResult[1];
            QuestionChoiceResult questionChoice = new QuestionChoiceResult();
            questionChoice.Question = new Question();
            questionChoice.Question.QID = 1;
            Choice[] choice = new Choice[1];
            Choice c = new Choice();
            c.QID = 1;
            c.CID = 1;
            c.CIDENTIFIER = "A";
            c.DESCRIPTION = "Test Choice";
            choice[0] = c;
            questionChoice.Question.QIDENTIFIER = "1";
            questionChoice.Question.DESCRIPTION = "Test Question";
            questionChoice.Question.CHOICETYPE = false;
            questionChoice.Choices = choice;
            expectedResult[0] = questionChoice;

            //sending my list to prepare main object
            IEnumerable<QuestionChoiceResult> returnResult= bManager.GetQuestionChoices(false, QBusinessManagerTests.Getchoices(), QBusinessManagerTests.Getquestions());
            QuestionChoiceResult[] returnResultConvert = returnResult.ToArray();

            CollectionAssert.IsNotEmpty(returnResult);
            Assert.AreEqual(expectedResult.Length, returnResult.Count());
            CollectionAssert.AreEqual(expectedResult, returnResultConvert, new QuestionChoiceResultComparer());
        }
        [Test]
        public void InsertAnswer_ReturnsExceptionEmpty()
        {
            //sending empty result
            QuestionChoiceResult[] choicesTable = new QuestionChoiceResult[1];
            
            TestDelegate action = () => bManager.InsertAnswers(choicesTable, "TestUser");
            Assert.Throws<Exception>(action);
        }
        [Test]
        public void InsertAnswer_ReturnsExceptionWrongId()
        {
            QuestionChoiceResult[] choicesTable = new QuestionChoiceResult[1];
            QuestionChoiceResult questionChoice = new QuestionChoiceResult();
            Choice[] choices= new Choice[1];
            Choice c = new Choice();
            c.QID = 1;
            c.CID = 0;
            choices[0] = c;
            questionChoice.Choices = choices;
            TestDelegate action = () => bManager.InsertAnswers(choicesTable, "TestUser");
            Assert.Throws<Exception>(action);
        }
        public static DataTable Getchoices()
        {
            //This method prepares the QDataManager.GetChoiceData method imitation
            DataTable dt = new DataTable("CHOICE");

            DataColumn[] columns = new DataColumn[] { new DataColumn("QID", typeof(Int32)),
            new DataColumn("CID", typeof(Int32)),
            new DataColumn("ANSWER_RATE", typeof(object)),
            new DataColumn("CHECKED", typeof(Boolean)),
            new DataColumn("CIDENTIFIER", typeof(string)),
            new DataColumn("DESCRIPTION", typeof(string))};
            dt.Columns.AddRange(columns);
            DataRow dr = dt.NewRow();
            dr.ItemArray = new object[] { 1, 1, null, false, "A", "Test Choice" };
            dt.Rows.Add(dr);

            return dt;
        }
        public static DataTable Getanswers()
        {
            //This method prepares the QDataManager.GetAnswerData method imitation
            DataTable dt = new DataTable("CHOICE");

            DataColumn[] columns = new DataColumn[] { new DataColumn("QID", typeof(Int32)),
            new DataColumn("CID", typeof(Int32)),
            new DataColumn("ANSWER", typeof(object)),
            new DataColumn("CHECKED", typeof(Boolean)),
            new DataColumn("CIDENTIFIER", typeof(string)),
            new DataColumn("DESCRIPTION", typeof(string))};
            dt.Columns.AddRange(columns);
            DataRow dr = dt.NewRow();
            dr.ItemArray = new object[] { 1, 1, 50, false, "A", "Test Choice" };
            dt.Rows.Add(dr);

            return dt;
        }
        private static DataTable Getquestions()
        {
            //This method prepares the QDataManager.GetQuestionData method imitation
            DataTable dt = new DataTable("QUESTION");
            DataColumn[] columns = new DataColumn[] { new DataColumn("CHOICETYPE", typeof(Boolean)),
            new DataColumn("DESCRIPTION", typeof(string)),
            new DataColumn("QID", typeof(Int32)),
            new DataColumn("QIDENTIFIER", typeof(string)),
            new DataColumn("VALIDATED", typeof(Boolean))};
            dt.Columns.AddRange(columns);
            DataRow dr = dt.NewRow();
            dr.ItemArray = new object[] { false, "Test Question", 1, "1", false };
            dt.Rows.Add(dr);

            return dt;
        }
    }
}
