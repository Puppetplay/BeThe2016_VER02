//
// Shedule 크롤링 결과에서 데이터를 추출하는 Parser
//

using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using BeThe.Item;

namespace BeThe.Crawler
{
    internal class ParserShedule : ParserBase
    {
        #region Singleton

        private ParserShedule()
        {

        }

        public static ParserShedule Instance
        {
            get { return Nested.instance; }

        }

        private class Nested
        {
            static Nested()
            {
            }
            internal static readonly ParserShedule instance = new ParserShedule();
        }

        #endregion

        #region Public Functions

        public List<Schedule> Parse(String html, Int32 year, Int32 month)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var nodes = doc.DocumentNode.SelectNodes("//tbody//tr");
            List<Schedule> schedules = new List<Schedule>();

            Int32 currentMonth = 0;
            Int32 currrntDay = 0;
            foreach (var node in nodes)
            {
                String day = GetDay(node);
                if (day != null)
                {
                    currentMonth = Convert.ToInt32(day.Substring(0, 2));
                    currrntDay = Convert.ToInt32(day.Substring(3, 2));
                    if (currentMonth != month)
                    {
                        throw new OpenQA.Selenium.StaleElementReferenceException("페이지가 완전히 로드되지 않았습니다.");
                    }
                }

                if (currentMonth != 0 && currrntDay != 0)
                {
                    Schedule_W schedule_W = CreateSchedule_WFromNode(node, year, currentMonth, currrntDay);
                    if (schedule_W != null)
                    {
                        Schedule schedule = ConvertToSchedule(schedule_W);
                        if (schedule != null)
                        {
                            schedules.Add(schedule);
                        }
                    }
                }
            }
            return schedules;

        }

        #endregion

        #region Private Functions

        // Schedule_W 얻기
        private Schedule_W CreateSchedule_WFromNode(HtmlNode node, Int32 year, Int32 month, Int32 day)
        {
            try
            {
                if (GetInnerHtml(node, "none") != null || GetInnerHtml(node, "relay") == null)
                {
                    return null;
                }

                return new Schedule_W
                {
                    Year = year,
                    Month = month,
                    Day = day,
                    Time = GetInnerHtml(node, "time"),
                    Play = GetInnerHtml(node, "play"),
                    Relay = GetInnerHtml(node, "relay"),
                    BallPark = GetInnerHtml(node, "ballpark"),
                    Etc = GetInnerHtml(node, "etc"),
                };
            }
            catch (Exception exception)
            {
                throw exception;
            }

        }

        // 일자얻기
        private String GetDay(HtmlNode node)
        {
            return GetInnerHtmlFromPath(node, "td [@class='day']//a//span");
        }

        // Schedule_W에서 Schedule로 변환
        private Schedule ConvertToSchedule(Schedule_W schedule_W)
        {
            Schedule schedule = new Schedule
            {
                Year = schedule_W.Year,
                Month = schedule_W.Month,
                Day = schedule_W.Day,
                BallPark = schedule_W.BallPark,
                Etc = schedule_W.Etc
            };

            schedule.Hour = Convert.ToInt32(schedule_W.Time.Substring(0, 2));
            schedule.Minute = Convert.ToInt32(schedule_W.Time.Substring(3, 2));

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(schedule_W.Play);
            var nodes = doc.DocumentNode.SelectNodes("span");

            schedule.AwayTeam = Util.Util.GetTeamInitialFromName(nodes[0].InnerHtml);
            schedule.HomeTeam = Util.Util.GetTeamInitialFromName(nodes[1].InnerHtml);

            if (schedule_W.Etc == "-")
            {
                // 경기가 진행되었으면
                nodes = doc.DocumentNode.SelectNodes("em/span");
                schedule.AwayTeamScore = Convert.ToInt32(nodes[0].InnerHtml);
                schedule.HomeTeamScore = Convert.ToInt32(nodes[2].InnerHtml);

                doc.LoadHtml(schedule_W.Relay);
                schedule.Href = doc.DocumentNode.SelectSingleNode("a").GetAttributeValue("href", "");

                try
                {
                    schedule.GameId = schedule.Href.Split(new char[] { '&', '=' })[5];
                    schedule.LeagueId = Convert.ToInt32(schedule.Href.Split(new char[] { '&', '=' })[1]);
                    schedule.SeriesId = Convert.ToInt32(schedule.Href.Split(new char[] { '&', '=' })[3]);
                }
                catch (Exception)
                {
                    return null;
                }

            }
            return schedule;
        }
        #endregion
    }
}
