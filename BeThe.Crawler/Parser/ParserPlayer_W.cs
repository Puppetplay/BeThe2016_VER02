//
// Player_W 크롤링 결과에서 데이터를 추출하는 Parser
//

using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using BeThe.Item;

namespace BeThe.Crawler.Parser
{
    internal class ParserPlayer_W : ParserBase
    {
        #region Singleton

        private ParserPlayer_W()
        {

        }

        public static ParserPlayer_W Instance
        {
            get { return Nested.instance; }

        }

        private class Nested
        {
            static Nested()
            {
            }
            internal static readonly ParserPlayer_W instance = new ParserPlayer_W();
        }

        #endregion

        #region Public Functions

        public List<Player_W> Parse(String html, String team)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var nodes = doc.DocumentNode.SelectNodes("//tr");
            List<Player_W> players = new List<Player_W>();
            foreach (var node in nodes)
            {
                var items = node.SelectNodes("td");

                Player_W player = new Player_W
                {
                    Href = items[1].SelectSingleNode("a").GetAttributeValue("href", ""),
                    Team = team
                };
                players.Add(player);
            }

            return players;
        }

        #endregion

        #region Private Functions

        // Player 얻기
        private Player CreatePlayer_FromNode(HtmlNode node)
        {
            String temp = node.InnerHtml;
            try
            {
                return new Player
                {

                };
            }
            catch (Exception exception)
            {
                throw exception;
            }

        }

        private Int32? GetBackNumber(String number)
        {
            Int32 temp = 0;
            if (Int32.TryParse(number, out temp))
            {
                return temp;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
