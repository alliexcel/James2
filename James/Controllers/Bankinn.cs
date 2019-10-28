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
using System.Xml;

namespace James.Controllers
{
    public class Bankinn
    {

        public LuisCalls.LuisClass.Response BankiTextResult(LuisCalls.LuisClass.RootObject lc) {

            LuisCalls.LuisClass.Response ResponseClass = new LuisCalls.LuisClass.Response();
            if (lc.topScoringIntent.intent == "Stadan"){ResponseClass = Stadan(lc);}
            if (lc.topScoringIntent.intent == "Gengi")
            {
                if (lc.entities.Count == 0)
                {
                    ResponseClass.SetTopic = "Banki";
                    ResponseClass.TextResponse = "Endurtaktu spurninguna, ég náði ekki myntinni";
                }
                else
                {
                    if (lc.entities[0].type == "Hlutabréf")
                    {
                        ResponseClass = Gengiskallhb(lc);
                    }

                    if (lc.entities[0].type == "Myntir")
                    {
                        ResponseClass = Gengiskall(lc);
                    }
                }
            }           
            if (ResponseClass.TextResponse == null){ ResponseClass.TextResponse = ""; }
            ResponseClass.SetTopic = "Gengi";
            

            return ResponseClass;
        }

        private LuisClass.Response Gengiskallhb(LuisClass.RootObject lc)
        {
            LuisClass.Response ResponseClass = new LuisClass.Response();
            try
            {
                Service1Client sc = new Service1Client();
                string bref = lc.entities[0].resolution.values[0].ToString();
                ResponseClass.TextResponse = GengiHlutabrefa(bref); 
                ResponseClass.SetTopic = "Banki";
            }
            catch
            {
                ResponseClass.TextResponse = "Geturðu endurtekið spurninguna ég náði ekki félaginu";
            }

            return ResponseClass;
        }

        private string GengiHlutabrefa(string bref)
        {
            string TextResponse = "";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load("https://www.landsbankinn.is/einstaklingar/sparnadurogfjarfestingar/hlutabref/");
            HtmlAgilityPack.HtmlNode[] Yfirsagnir = document.DocumentNode.SelectNodes("//tr[@class=\"right\"]//td").ToArray();

            int x = 1;
            string gengi = "";
            string breyting = "";
            foreach (var item in Yfirsagnir)
            {
               if (item.InnerText == bref)
                {
                    gengi = Yfirsagnir[x].InnerText;
                    breyting = Yfirsagnir[x + 1].InnerText;
                }

                x++;         
            }
            TextResponse = "Gengið á " + bref + " er " + gengi + " og er breytingin " + breyting + " prósent innan dags.";

            return TextResponse;
        }

        private LuisClass.Response Millifaersla(LuisClass.RootObject lc)
        {
            LuisClass.Response ResponseClass = new LuisClass.Response();
            try
            {
                string fornafn = "";
                string eftirnafn = "";
                string fjarhaed = "";

                foreach (var item in lc.entities)
                {
                    if (item.type == "Viðtakandi::First Name") { fornafn = item.entity; }
                    if (item.type == "Viðtakandi::Last Name") { eftirnafn = item.entity; }
                    if (item.type == "Fjöldi" || item.type == "Upphæð") { fjarhaed = item.entity; }
                }

                if (fornafn != "" && eftirnafn != "" && fjarhaed != "")
                {
                    ResponseClass.TextResponse = "Viltu millifæra " + fjarhaed + " krónur á " + fornafn + " " + eftirnafn + ".";

                }
                else
                {
                    if (fornafn == "") { ResponseClass.TextResponse = "Ég náði ekki fornafni, geturðu endurtekið beiðnina"; }
                    if (eftirnafn == "") { ResponseClass.TextResponse = "Ég náði ekki eftirnafni, geturðu endurtekið beiðnina"; }
                    if (fjarhaed == "")
                    { ResponseClass.TextResponse = "Ég náði ekki fjárhæðinni, geturðu endurtekið beiðnina"; }

                }
            }
            catch
            { ResponseClass.TextResponse = "Ég náði ekki því sem þú sagðir, geturðu endurtekið spurninguna."; }

            ResponseClass.SetTopic = "Banki";


            return ResponseClass;
        }

        public LuisClass.Response Gengiskall(LuisClass.RootObject lc)
        {
            LuisClass.Response ResponseClass = new LuisClass.Response();
            try
            {
          
                string mynt = lc.entities[0].resolution.values[0].ToString();
                ResponseClass.TextResponse = GetData(mynt);
                ResponseClass.SetTopic = "Banki";
            }
            catch
            {
                ResponseClass.TextResponse = "Geturðu endurtekið spurninguna ég náði ekki myntinni.";
            }

            

            return ResponseClass;
        }

        public LuisClass.Response Stadan(LuisClass.RootObject lc)
        {

            LuisClass.Response ResponseClass = new LuisClass.Response();
            try
            {
                if (lc.entities[0].resolution.values[0].ToString() == "Aukakrónur")
                {
                    ResponseClass.TextResponse = "Staðan á aukakrónum þínum er 2.346 krónur.";
                }
                if (lc.entities[0].resolution.values[0].ToString() == "Innlán")
                {
                    ResponseClass.TextResponse = "Heildarstaða innlána er 234.239 krónur.";
                }
                if (lc.entities[0].resolution.values[0].ToString() == "Kreditkort")
                {
                    ResponseClass.TextResponse = "Til ráðstöfunar á kreditkorti þínu eru 234.234 krónur.";
                }
                if (lc.entities[0].resolution.values[0].ToString() == "Veltureikningur")
                {
                    ResponseClass.TextResponse = "Til ráðstöfunar á tékkareikningi er 23.320 krónur";
                }
                if (lc.entities[0].resolution.values[0].ToString() == "Íbúðalán")
                {
                    ResponseClass.TextResponse = "Heildarstaða íbúðalána þinna eru 23.323.456 krónur.";
                }
                if (lc.entities[0].resolution.values[0].ToString() == "Lífeyrissjóði")
                {
                    ResponseClass.TextResponse = "Heildarstaða í Íslenska lífeyrissjóðinum er 8.963.659 krónur.";
                }
                if (lc.entities[0].resolution.values[0].ToString() == "Ógreiddir reikningar")
                {
                    ResponseClass.TextResponse = "Heildarstaða ógreiddra reikninga eru 23.320 krónur.";
                }
            }
            catch
            {
                ResponseClass.TextResponse = "Geturðu talað skýrar eða endurorðað spurninguna.";
            }

            ResponseClass.SetTopic = "Banki";

            return ResponseClass;

        }

        public string GetData(string mynt)
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

            return cr;
        }



    }
}