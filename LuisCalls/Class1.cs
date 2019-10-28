using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuisCalls
{
    public class LuisClass
    {
        public class TopScoringIntent
        {
            public string intent { get; set; }
            public double score { get; set; }
        }

        public class Intent
        {
            public string intent { get; set; }
            public double score { get; set; }
        }

        public class Resolution
        {
            public List<string> values { get; set; }
        }

        public class Entity
        {
            public string entity { get; set; }
            public string type { get; set; }
            public int startIndex { get; set; }
            public int endIndex { get; set; }
            public Resolution resolution { get; set; }
        }

        public class RootObject
        {
            public string query { get; set; }
            public TopScoringIntent topScoringIntent { get; set; }
            public List<Intent> intents { get; set; }
            public List<Entity> entities { get; set; }
        }

        public class Response
        {
            public string TextResponse { get; set; }
            public string SetIntent { get; set; }

        }

    }
}