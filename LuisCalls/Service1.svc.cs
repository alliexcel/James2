using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace LuisCalls
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public string GetData(string GoogleText, string SetIntent)
        {
            string url = "";

            //Routing á hvaða SetIntent notandinn er búinn að setja sér. null er til að setja upphaflegt routing. 
            if (SetIntent == null) { url = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/947c8983-b854-4c2d-a210-fac6563593f1?subscription-key=ac9e91b3940344369648a914cdaa05ce&verbose=true&timezoneOffset=0&q=" + GoogleText; }
            if (SetIntent == "Banki") { url = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/59c0068c-2a43-4520-b2af-46c9bdeab61e?subscription-key=ac9e91b3940344369648a914cdaa05ce&verbose=true&timezoneOffset=0&q=" + GoogleText; }
            if (SetIntent == "Fréttir") { url = ""; }
            if (SetIntent == "Bíó") { url = ""; }
            if (SetIntent == "Símaskrá") { url = ""; }

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = WebRequestMethods.Http.Get;
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Accept = "text/json";
            httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();

            string result = "";
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                result = reader.ReadToEnd();

            }

            LuisClass.RootObject lc = JsonConvert.DeserializeObject<LuisClass.RootObject>(result);
            LuisClass.Response answer = GenerateText(lc, SetIntent);

            return answer.ToString();
        }

        public LuisClass.Response GenerateText(LuisClass.RootObject lc, string SetIntent)
        {
            LuisClass.Response ResponseClass = new LuisClass.Response();

            if (SetIntent == null)
            {
                ResponseClass.TextResponse = "Nú hefur þú aðgang að " + lc.entities[0].resolution.values.ToString();
                SetIntent = lc.entities[0].resolution.values.ToString();
                ResponseClass.SetIntent = SetIntent;
            }
            if (SetIntent == "Banki")
            {
                if (lc.topScoringIntent.intent == "Staðan")
                {
                    ResponseClass.TextResponse = "Þú ert að biðja um stöðu á " + lc.entities[0].resolution.values.ToString();
                    ResponseClass.SetIntent = "Banki";
                }
            }

            return ResponseClass;
        }
        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

    }
}
