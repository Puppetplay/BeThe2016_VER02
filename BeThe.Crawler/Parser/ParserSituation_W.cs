//
// Situation 크롤링 결과에서 데이터를 추출하는 Parser
//

using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using BeThe.Item;

namespace BeThe.Crawler
{
    internal class ParserSituation_W : ParserBase
    {
        #region Singleton

        private ParserSituation_W()
        {

        }

        public static ParserSituation_W Instance
        {
            get { return Nested.instance; }

        }

        private class Nested
        {
            static Nested()
            {
            }
            internal static readonly ParserSituation_W instance = new ParserSituation_W();
        }

        #endregion

        #region Public Functions

        public Situation_W Parse(Schedule schedule, String html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var element = doc.DocumentNode.SelectSingleNode("//*[@class=\"situation\"]");

            Situation_W situation_W = new Situation_W();
            situation_W.GameId = schedule.GameId;
            situation_W.Content = element.OuterHtml;
            return situation_W;
        }

        #endregion
    }
}
