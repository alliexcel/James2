using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using James.Models;
using System.Net;
using James.ServiceReference1;
using LuisCalls;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Text;
using System.Xml;
using Newtonsoft.Json;

namespace James.Controllers
{
    public class FrettirController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public LuisCalls.LuisClass.Response FrettirTextResult(LuisCalls.LuisClass.RootObject lc, string SetTopic)
        {

            LuisCalls.LuisClass.Response ResponseClass = new LuisCalls.LuisClass.Response();
            if (lc.topScoringIntent.intent == "Fyrirsagnir") { ResponseClass = Fyrirsagnir(lc, SetTopic); }
            if (lc.topScoringIntent.intent == "Lesa") { ResponseClass = Lesa(lc, SetTopic); }
            if (ResponseClass.TextResponse == null) { ResponseClass.TextResponse = ""; }

            return ResponseClass;
        }


        private LuisClass.Response Fyrirsagnir(LuisClass.RootObject lc, string SetTopic)
        {
            string responsestring = "";
            LuisClass.Response ResponseClass = new LuisClass.Response();

            Extract ex = new Extract();
            ex = Extractfjoldi(lc);
            ex.topic = "Fréttir " + ex.midill;

            ex = ReturnURL(lc, ex);
            string Url = ex.URL;
            int fjoldi = ex.fjoldi;
            string midill = ex.midill;
            string tegund = ex.tegund;

            HtmlWeb web = new HtmlWeb();
            web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";
            



            string myString = "";

            //HVernig MBL er skrapaður
            if (midill == null) { responsestring = "Geturðu endurtekið spurninguna?"; }

            if (midill == "mbl")
            {
                System.Xml.XmlDocument rssXmlDoc = new XmlDocument();

                // Load the RSS file from the RSS URL
                rssXmlDoc.Load(Url);
                responsestring = "";
                // Parse the Items in the RSS file9
                XmlNodeList rssNodes = rssXmlDoc.SelectNodes("//item//title");
                string tala = "";
                for (int x = 1; x <= ex.fjoldi; x++)
                {
                    tala = Tolutexti(x);
                    XmlNode item = rssNodes[x];
                    responsestring = responsestring + "Frétt númer " + tala + ". " + item.FirstChild.InnerText + ". ";

                }

            }

            if (midill == "dv")
            {
                HtmlDocument document = web.Load(Url);
                HtmlAgilityPack.HtmlNode[] Yfirsagnir = document.DocumentNode.SelectNodes("//div[@class=\"alphadelta\"]//article//h2").ToArray();
                //Strengur hnýttur saman7
                responsestring = "";
                string tala = "";
                for (int x = 0; x < fjoldi; x++)
                {
                    tala = Tolutexti(x + 1);
                    myString = System.Net.WebUtility.HtmlDecode(Yfirsagnir[x].InnerText);
                    responsestring = "D V býður ekki upp á r s s streymi og því er ekki hægt að lesa fréttir.";
                    // responsestring = responsestring + "Frétt númer " + tala + ". " + Regex.Replace(myString, @"\t|\n|\r", "") + ". ";
                }
            }

            if (midill == "vísir")
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                System.Net.ServicePointManager.Expect100Continue = false;
           

                HtmlDocument document = web.Load(Url);
                HtmlAgilityPack.HtmlNode[] Yfirsagnir = document.DocumentNode.SelectNodes("//div[@class= \"row ui-mb-xs-30\"]//div[@class= \"article-item__content\"]").ToArray();
                
                foreach(var item in Yfirsagnir)
                {
                   
                   // item.InnerText
                  
                }




                //Strengur hnýttur saman7
                responsestring = "";
                string tala = "";
                for (int x = 0; x < fjoldi; x++)
                {
                    tala = Tolutexti(x + 1);
                    myString = System.Net.WebUtility.HtmlDecode(Yfirsagnir[x].InnerText);
                    responsestring = "D V býður ekki upp á r s s streymi og því er ekki hægt að lesa fréttir.";
                    // responsestring = responsestring + "Frétt númer " + tala + ". " + Regex.Replace(myString, @"\t|\n|\r", "") + ". ";
                }

            }

            if (midill == "rúv")
            {
                System.Xml.XmlDocument rssXmlDoc = new XmlDocument();

                // Load the RSS file from the RSS URL
                rssXmlDoc.Load(Url);
                responsestring = "";
                // Parse the Items in the RSS file9
                XmlNodeList rssNodes = rssXmlDoc.GetElementsByTagName("//item//title");
                string tala = "";
                for (int x = 1; x <= ex.fjoldi; x++)
                {
                    tala = Tolutexti(x);
                    XmlNode item = rssNodes[x];
                    responsestring = responsestring + "Frétt númer " + tala + ". " + item.FirstChild.InnerText + ". ";

                }
            }

            ResponseClass.TextResponse = responsestring;
            ResponseClass.SetTopic = "Fréttir " + midill + " " + tegund;
            return ResponseClass;

        }



        private LuisClass.Response Lesa(LuisClass.RootObject lc, string SetTopic)
        {
            Extract ex = new Extract();
            ex.topic = SetTopic;

            if (ex.topic.Contains("mbl") == true) { ex.midill = "mbl"; }
            if (ex.topic.Contains("dv") == true) { ex.midill = "dv"; }
            if (ex.topic.Contains("vísir") == true) { ex.midill = "vísir"; }
            if (ex.topic.Contains("rúv") == true) { ex.midill = "rúv"; }
            if (ex.topic.Contains("íþróttir") == true) { ex.tegund = "íþróttir"; }
            if (ex.topic.Contains("viðskipti") == true) { ex.tegund = "viðskipti"; }
            if (ex.topic.Contains("slúður") == true) { ex.tegund = "slúður"; }


                ex = ReturnURL(lc, ex);
                if (ex.URL == null) { ex.URL = ""; }
                LuisClass.Response r = new LuisClass.Response();
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = web.Load(ex.URL);
                string responsestring = "";

                Extract ex2 = new Extract();
                ex2 = Extractfjoldi(lc);
                ex.fjoldi = ex2.fjoldi - 1;
          
            if (SetTopic.Contains("vísir") == true)
            {

                System.Xml.XmlDocument rssXmlDoc = new XmlDocument();

                ex2 = ReturnURL(lc, ex);
                string Url = ex2.URL;
                // Load the RSS file from the RSS URL
                rssXmlDoc.Load(Url);
                responsestring = "";
                // Parse the Items in the RSS file9
                XmlNodeList rssNodes = rssXmlDoc.SelectNodes("//item//description");

                responsestring = rssNodes[ex2.fjoldi - 1].FirstChild.Value;


                responsestring = Regex.Replace(responsestring, @"<[^>]*>", String.Empty, RegexOptions.IgnoreCase).Trim();

        }

            if(SetTopic.Contains("rúv")== true)
            {
                System.Xml.XmlDocument rssXmlDoc = new XmlDocument();
                
                ex2 = ReturnURL(lc,ex);
                string Url = ex2.URL;
                // Load the RSS file from the RSS URL
                rssXmlDoc.Load(Url);
                responsestring = "";
                // Parse the Items in the RSS file9
                XmlNodeList rssNodes = rssXmlDoc.GetElementsByTagName("//item//description");              
                responsestring = rssNodes[ex2.fjoldi -1].FirstChild.Value;

            }


            if (SetTopic.Contains("dv")== true)
            {
                responsestring = "Þú vilt ekkert vera að skoða fréttir á DV!";
            }

            if (SetTopic.Contains("mbl") == true)
            {

                System.Xml.XmlDocument rssXmlDoc = new XmlDocument();

                ex2 = ReturnURL(lc, ex);
                string Url = ex2.URL;
                // Load the RSS file from the RSS URL
                rssXmlDoc.Load(Url);
                responsestring = "";
                // Parse the Items in the RSS file9
                XmlNodeList rssNodes = rssXmlDoc.SelectNodes("//item//description");

                responsestring = rssNodes[ex2.fjoldi - 1].FirstChild.Value;

   
                responsestring = Regex.Replace(responsestring, @"<[^>]*>", String.Empty, RegexOptions.IgnoreCase).Trim();

            }
            r.SetTopic = SetTopic;
            r.TextResponse = responsestring;
            return r;
        }

        private Extract ReturnURL(LuisClass.RootObject lc, Extract ex)
        {

            if (ex.midill == "mbl" && ex.tegund == "íþróttir") { ex.URL = "https://www.mbl.is/feeds/sport/"; }
            if (ex.midill == "mbl" && ex.tegund == "viðskipti") { ex.URL = "https://www.mbl.is/feeds/vidskipti/"; }
            if (ex.midill == "mbl" && ex.tegund == "slúður") { ex.URL = "https://www.mbl.is/feeds/smartland/"; }
            if (ex.midill == "mbl" && (ex.tegund == "" || ex.tegund == null )) { ex.URL = "https://www.mbl.is/feeds/fp/"; }

            if (ex.midill == "dv" && (ex.tegund == "" || ex.tegund == null)) { ex.URL = "http://www.dv.is/frettir/"; }
            if (ex.midill == "dv" && ex.tegund == "íþróttir") { ex.URL = "http://www.dv.is/frettir/"; }
            if (ex.midill == "dv" && ex.tegund == "viðskipti") { ex.URL = "http://www.dv.is/frettir/"; }
            if (ex.midill == "dv" && ex.tegund == "slúður") { ex.URL = "http://www.dv.is/frettir/"; }

            if (ex.midill == "vísir" && ex.tegund == "" || ex.midill == "vísir" && (ex.tegund == null)) { ex.URL = "http://www.visir.is/rss/allt"; }
            if (ex.midill == "vísir" && ex.tegund == "íþróttir") { ex.URL = "https://www.visir.is/f/sport"; }
            if (ex.midill == "vísir" && ex.tegund == "slúður") { ex.URL = "https://www.visir.is/rss/lifid"; }
            if (ex.midill == "vísir" && ex.tegund == "viðskipti") { ex.URL = "https://www.visir.is/rss/vidskipti"; }

            if (ex.midill == "rúv" && (ex.tegund == null || ex.tegund == "")) { ex.URL = "http://www.ruv.is/rss/frettir"; }
            if (ex.midill == "rúv" && ex.tegund == "íþróttir") { ex.URL = "http://www.ruv.is/rss/ithrottir"; }

            return ex;
           
        }



        private Extract Extractfjoldi(LuisClass.RootObject lc)
        {
            Extract ex = new Extract();
            foreach (var item in lc.entities)
            {
                if (item.type == "Miðlar") { ex.midill = item.resolution.values[0]; }
                if (item.type == "Tegund") { ex.tegund = item.resolution.values[0]; }
                if(item.resolution.values[0] == null)
                {
                    ex.fjoldi = 5;
                }
                else
                {
                    if (item.type == "Fjoldi") { ex.fjoldi = Convert.ToInt32(item.resolution.values[0]); }
                }
               
            }

            return ex;
        }

        private string Tolutexti(int x)
        {
            string tala = "";
            if (x == 1) { tala = "eitt"; }
            if (x == 2) { tala = "tvö"; }
            if (x == 3) { tala = "þrjú"; }
            if (x == 4) { tala = "fjögur"; }
            if (x == 5) { tala = "fimm"; }
            if (x == 6) { tala = "sex"; }
            if (x == 7) { tala = "sjö"; }
            if (x == 8) { tala = "átta"; }
            if (x == 9) { tala = "níu"; }
            if (x == 10) { tala = "tíu"; }
            if (x == 11) { tala = "ellefu"; }
            if (x == 12) { tala = "tólf"; }
            if (x == 13) { tala = "þrettán"; }
            if (x == 14) { tala = "fjórtán"; }
            if (x == 15) { tala = "fimmtán"; }
            if (x == 16) { tala = "sextán"; }
            if (x == 17) { tala = "sautján"; }
            if (x == 18) { tala = "átján"; }
            if (x == 19) { tala = "nítján"; }
            if (x == 20) { tala = "tuttugu"; }
            if (x == 21) { tala = "tuttugu og einn"; }
            if (x == 22) { tala = "tuttugu og tveir"; }
            if (x == 23) { tala = "tuttugu og þrír"; }
            if (x == 24) { tala = "tuttugu og fjórir"; }
            if (x == 25) { tala = "tuttugu og fimm"; }
            if (x == 26) { tala = "tuttugu og sex"; }
            if (x == 27) { tala = "tuttugu og sjö"; }
            if (x == 28) { tala = "tuttugu og átta"; }
            if (x == 29) { tala = "tuttugu og níu"; }



            return tala;
        }

    }
}