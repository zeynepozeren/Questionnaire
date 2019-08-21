using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using QBusinessManager;

namespace Questionaire
{
    public partial class _Default : Page
    {
        QuestionChoiceResult[] questionChoices;
        const string CID = "CID_";
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            //Gets main list to populate screen dynamically
            GetMainData();
            //Creates screen objects by using the main data object
            SetDataTable();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //Canceling cache
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetNoStore();
            Response.AppendHeader("pragma", "no-cache");
            
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                //validation for mandatory questions by controlling the checked objects.Choice selections are signed. 
                bool AreAllAnswered = ValidateQuestionaire();
                if (AreAllAnswered == false)
                {
                    Label2.Text = "  * All questions are mandatory.";
                }
                else
                {
                    Label2.Text = "Sending to compute";
                    //Getting the result data with answers
                    object Rcount = GetCalculatedData();
                    if (Rcount != null)
                    {
                        //if everything is OK creates screen objects
                        SetDataTable();
                        Button1.Visible = false;
                        Label2.Text = "Number of Respondents: "+ Rcount;
                    }
                    else Label2.Text = "No Result";
                }
            }
           
        }
        private bool ValidateQuestionaire()
        {
            bool result = true;
            //Controlling the table objects. Searching for the control we created. 
            for (int i = 0; i < Table1.Rows.Count; i++)
            {
                TableRow tr = Table1.Rows[i];

                for (int j = 0; j < tr.Cells.Count; j++)
                {
                    TableCell tcell = tr.Cells[j];
                    if (tcell.Controls.Count > 0)
                    {
                        foreach (WebControl c in tcell.Controls)
                        {
                            //Finding the object with custom identifier prefix.
                            if (c.ID.StartsWith(CID))
                            {
                                int choiceId = Convert.ToInt32(c.ID.Replace(CID, ""));
                                bool cellchecked = false;
                                if (c.GetType() == typeof(CheckBox))
                                {
                                    cellchecked = ((CheckBox)c).Checked;
                                }
                                else if (c.GetType() == typeof(RadioButton))
                                {
                                    cellchecked = ((RadioButton)c).Checked;
                                }
                                //Set the checked value of the choice object on our main object. We will save this data later.
                                questionChoices.Single(x => x.Choices.Any(y => y.CID == choiceId)).Choices.Single(z=>z.CID==choiceId).CHECKED=cellchecked;
                                if (cellchecked)
                                {

                                    int qid = questionChoices.Single(x => x.Choices.Any(y => y.CID == choiceId)).Choices.Single(z => z.CID == choiceId).QID;
                                    //One selection is found. Set the validated valueof question object.
                                    questionChoices.Single(x => x.Question.QID == qid).Question.VALIDATED = cellchecked;
                                }
                            }
                        }
                    }
                }
            }
            //Check if any unvalidated question exist
            if (questionChoices.Where(x => x.Question.VALIDATED == true).ToArray().Length != questionChoices.Length) result = false;
            return result;
        }
        private void GetMainData()
        {
            //Gets question and choice objects
            QBusinessManager.QBusinessManager bm = new QBusinessManager.QBusinessManager();
            questionChoices= bm.GetChoiceList().ToArray();
        }
        private string GetCalculatedData()
        {
            //Sets answers and gets question, choice and answers
            QBusinessManager.QBusinessManager bm = new QBusinessManager.QBusinessManager();
            //insert the checked choices
            string processResult = SetAnswerData();
            int RCount = 0;
            questionChoices = bm.GetAnswerList(ref RCount).ToArray();
            
            return RCount+ " "+processResult;
        }
        private string SetAnswerData()
        {
            
            QBusinessManager.QBusinessManager bm = new QBusinessManager.QBusinessManager();
            //Create a unique respondId to count the respondents.
            string ResponseId = Request.ServerVariables["REMOTE_ADDR"] + Session.SessionID;//Some data contains user,session and request info
            string processResult = "";
            try
            {
                bm.InsertAnswers(questionChoices, ResponseId);
            }
            catch (Exception ex)
            {
                processResult = ex.Message;
            }
            return processResult;
        }
        private void SetDataTable()
        {
            Table1.Rows.Clear();
            //by using the QuestionChoiceResult object prepare the screen.
            foreach (QuestionChoiceResult dr in questionChoices)
            {
                TableRow tr = new TableRow();

                TableCell tcell = new TableCell();
                tcell.Text = dr.Question.QIDENTIFIER + " . " + dr.Question.DESCRIPTION;
                tcell.Font.Bold = true;
                tr.Font.Underline = true;
                tr.Font.Bold = false;
                tr.Cells.Add(tcell);
                Table1.Rows.Add(tr);

                int singleChoice = Convert.ToByte(dr.Question.CHOICETYPE);
                foreach (Choice choiceRow in dr.Choices)
                {
                    TableRow tr2 = new TableRow();

                    TableCell tcell2 = new TableCell();
                    tcell2.Width = 1000;
                    tcell2.HorizontalAlign = HorizontalAlign.Left;

                    WebControl c;
                    //If it is a result data create a label to show rates
                    if (choiceRow.ANSWER_RATE!=null)
                    {
                        Label lb = new Label();
                        lb.Text = choiceRow.CIDENTIFIER + " . " +choiceRow.DESCRIPTION+ " " + choiceRow.ANSWER_RATE + " %";
                        c = lb;
                    }
                    else
                    {
                        //If it is not a result data create a controls to prepare questionnaire
                        if (singleChoice == 0)
                        {
                            RadioButton b = new RadioButton();
                            b.GroupName = "Group" + choiceRow.QID;
                            b.Text = choiceRow.CIDENTIFIER + " . " + choiceRow.DESCRIPTION;
                            c = b;
                        }
                        else
                        {
                            CheckBox cb = new CheckBox();
                            cb.Text = choiceRow.CIDENTIFIER + " . " + choiceRow.DESCRIPTION;
                            c = cb;
                        }
                    }
                    c.ID = CID + choiceRow.CID.ToString();
                    c.Font.Italic = true;

                    tcell2.Controls.Add(c);
                    tr2.Cells.Add(tcell2);
                    Table1.Rows.Add(tr2);
                }
            }

        }
    }
    
}