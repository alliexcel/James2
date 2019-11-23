using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using James.Models;
using System.IO;
using LuisCalls;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace James.Controllers
{
    public class SaidsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Saids
        public ActionResult Index()
        {
            return View(db.Saids.ToList());
        }

        // GET: Saids/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Said said = db.Saids.Find(id);
            if (said == null)
            {
                return HttpNotFound();
            }
            return View(said);
        }

        public JsonResult GetData(string GoogleText, string SetIntent)
        {
            string url = "";
            string SetTopic = "";
            SetTopic = SetIntent;

            //http://localhost:7013/Saids/GetData?GoogleText="Lestu þrjár fréttir á mbl"&SetIntent=""

            GoogleText = Regex.Replace(GoogleText, "\"", "");
            SetTopic = Regex.Replace(SetTopic, "\"", "");


            //virðist ekki vera að notfæra mér topicið. Allt í einni gervigreind. 

            url = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/c08df70d-b17c-4619-be57-784dc9f83aac?subscription-key=ac9e91b3940344369648a914cdaa05ce&timezoneOffset=-360&q=" + GoogleText;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = WebRequestMethods.Http.Get;
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Accept = "text/json";
            httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();

            string result = "";
            LuisClass.Response answer = new LuisClass.Response();
           
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                result = reader.ReadToEnd();
            }

            LuisClass.RootObject lc = JsonConvert.DeserializeObject<LuisClass.RootObject>(result);
            answer = GenerateText(lc, SetTopic);
      
            return Json(answer,JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetExchangeRate(string mynt)
        {
            string man = System.DateTime.Today.Month.ToString();
            string ar = System.DateTime.Today.Year.ToString();
            string ar2 = "";
            string man2 = "";

            if (man == "1")
            {
                man2 = "12";
                ar2 = (Convert.ToInt32(ar) - 1).ToString();
            }
            else
            {
                man2 = (Convert.ToInt32(man) - 1).ToString();
                ar2 = ar;
            }

            System.Xml.XmlDocument rssXmlDoc = new XmlDocument();

            // Load the RSS file from the RSS URL
            rssXmlDoc.Load("https://www.landsbankinn.is/modules/markets/services/XMLGengi.asmx/Gengisthroun?mynt=" + mynt + "&strTegund=0&manFra=" + man2 + "&arFra=" + ar2 + "&manTil=" + man + "&arTil=" + ar);

            // Parse the Items in the RSS file9
            XmlNodeList rssNodes = rssXmlDoc.GetElementsByTagName("GjaldmidillRow");
            XmlNode item = rssNodes[0];
            XmlNode kaup = item["Kaup"];
            XmlNode sala = item["Sala"];

            string kaupstring = kaup.InnerText.ToString();
            string salastring = sala.InnerText.ToString();
            double kaupdouble = Convert.ToDouble(kaupstring);
            double saladouble = Convert.ToDouble(salastring);

            kaupstring = kaupstring.Replace(".", ",");
            salastring = salastring.Replace(".", ",");

            kaupstring = kaupdouble.ToString("0.##");
            salastring = saladouble.ToString("0.##");

            kaupstring = kaupstring.Replace(".", ",");
            salastring = salastring.Replace(".", ",");

            string cr = "Kaupgengi er " + kaupstring + " og sölugengi er " + salastring;
            return Json(cr, JsonRequestBehavior.AllowGet);
        }

        public LuisClass.Response GenerateText(LuisClass.RootObject lc, string SetTopic)
        {
            LuisClass.Response ResponseClass = new LuisClass.Response();
            Said s = new Said();
            s.DateTime = System.DateTime.Today.Date;
            s.PrimaryTopic = SetTopic;
            s.GoogleSpeechText = lc.query;
            s.AnswerIntent = lc.topScoringIntent.intent;

            
                db.Saids.Add(s);
                db.SaveChanges();



          if (lc.topScoringIntent.intent == "Gengi" || lc.topScoringIntent.intent == "Stadan")
          {
                Bankinn bankinn = new Bankinn();
                ResponseClass = bankinn.BankiTextResult(lc);
                ResponseClass.SetTopic = "Banki";
          }
          if (lc.topScoringIntent.intent == "Fyrirsagnir" || lc.topScoringIntent.intent == "Lesa")
          {
                FrettirController frettir = new FrettirController();              
                ResponseClass = frettir.FrettirTextResult(lc, SetTopic);
                

            }
          if (lc.topScoringIntent.intent == "None")
          {
                ResponseClass.SetTopic = SetTopic;
                ResponseClass.TextResponse = "Geturðu tala skýrar eða endurorðað spurninguna.";

          }

            return ResponseClass;
        }



        // GET: Saids/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Saids/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PrimaryTopic,GoogleSpeechText,AnswerIntent,DateTime")] Said said)
        {
            if (ModelState.IsValid)
            {
                db.Saids.Add(said);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(said);
        }

        // GET: Saids/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Said said = db.Saids.Find(id);
            if (said == null)
            {
                return HttpNotFound();
            }
            return View(said);
        }

        // POST: Saids/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PrimaryTopic,GoogleSpeechText,AnswerIntent,DateTime")] Said said)
        {
            if (ModelState.IsValid)
            {
                db.Entry(said).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(said);
        }

        // GET: Saids/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Said said = db.Saids.Find(id);
            if (said == null)
            {
                return HttpNotFound();
            }
            return View(said);
        }

        // POST: Saids/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Said said = db.Saids.Find(id);
            db.Saids.Remove(said);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
