using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using QBusinessManager;
using System.Web.Script.Serialization;

namespace Questionaire
{
    public class QRateController : ApiController
    {
        public string Get()
        {
            //https://localhost:44320/api/QRate
            QBusinessManager.QBusinessManager bm = new QBusinessManager.QBusinessManager();
            int noOfRespondent = 0;
            List<QuestionChoiceResult> questionChoices = bm.GetAnswerList(ref noOfRespondent).ToList();
            //Serialize custom object list to json formatted data
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(questionChoices);
            return json;
        }

    }
}