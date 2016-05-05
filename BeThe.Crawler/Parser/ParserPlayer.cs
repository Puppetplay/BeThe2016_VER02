//
// Player 크롤링 결과에서 데이터를 추출하는 Parser
//

using BeThe.Item;
using HtmlAgilityPack;
using System;

namespace BeThe.Crawler
{
    internal class ParserPlayer : ParserBase
    {
        #region Singleton

        private ParserPlayer()
        {

        }

        public static ParserPlayer Instance
        {
            get { return Nested.instance; }

        }

        private class Nested
        {
            static Nested()
            {
            }
            internal static readonly ParserPlayer instance = new ParserPlayer();
        }

        #endregion

        #region Public Functions

        public Player Parse(String html, String team, Int32 playerId)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var node = doc.DocumentNode.SelectSingleNode(".//div [@class='player_info']");

            Player player = new Player();

            Int32 tempNumber = 0;
            String[] spliter = new String[] { " " };

            // 플레이어 아이디
            player.PlayerId = playerId;

            // 이미지 파일명
            try
            {
                player.SCR = node.SelectSingleNode(".//div//img").GetAttributeValue("src", "");
            }
            catch
            {
                player.SCR = null;
            }

            // 팀명
            player.Team = team;

            // 이름
            var nodes = node.SelectNodes(".//li//span");
            player.Name = nodes[0].InnerText;

            // 백넘버
            if (Int32.TryParse(nodes[1].InnerText, out tempNumber))
            {
                player.BackNumber = tempNumber;
            }
            else
            {
                player.BackNumber = null;
            }

            // 생년월일
            String birthDate = nodes[2].InnerText;
            birthDate = birthDate.Replace("년", "").Replace("월", "").Replace("일", "").Replace(" ", "");
            if (birthDate.Length == 8)
            {
                player.BirthDate = birthDate.Replace("년", "").Replace("월", "").Replace("일", "").Replace(" ", "");
            }

            // 포지션, 손잡이
            String position = nodes[3].InnerText;
            position = position.Replace(@"(", " ").Replace(@")", "");
            if (position.Split(spliter, StringSplitOptions.RemoveEmptyEntries).Length == 2)
            {
                player.Position = position.Split(spliter, StringSplitOptions.RemoveEmptyEntries)[0];
                player.Hand = position.Split(spliter, StringSplitOptions.RemoveEmptyEntries)[1];
            }

            // 키, 몸무게
            String height = nodes[4].InnerText;
            height = height.Replace(@"cm/", " ").Replace(@"kg", "");
            if (height.Split(spliter, StringSplitOptions.RemoveEmptyEntries).Length == 2)
            {
                player.Height = Convert.ToInt32(height.Split(spliter, StringSplitOptions.RemoveEmptyEntries)[0]);
                player.Weight = Convert.ToInt32(height.Split(spliter, StringSplitOptions.RemoveEmptyEntries)[1]);
            }

            // 경력
            player.Career = nodes[5].InnerText;

            // 계약금
            player.Deposit = nodes[6].InnerText;
            // 연봉
            player.Salary = nodes[7].InnerText;
            // 지명순위
            player.Rank = nodes[8].InnerText;
            // 입단년도

            player.JoinYear = nodes[9].InnerText;
            return player;

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
