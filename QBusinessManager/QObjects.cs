using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QBusinessManager
{
    public class Choice
    {
        public Int32 CID { get; set; }
        public Int32 QID { get; set; }
        public string DESCRIPTION { get; set; }
        public string CIDENTIFIER { get; set; }
        public object ANSWER_RATE { get; set; }
        public Boolean CHECKED { get; set; }
        
    }
    public class Question
    {  
        public Int32 QID { get; set; }
        public string DESCRIPTION { get; set; }
        public string QIDENTIFIER { get; set; }
        public Boolean CHOICETYPE { get; set; }
        public Boolean VALIDATED { get; set; }
    }
    public class QuestionChoiceResult
    {
        //Main data type to transfer questionnaire information
        public Question Question { get; set; }
        public Choice[] Choices { get; set; }
    }

}
