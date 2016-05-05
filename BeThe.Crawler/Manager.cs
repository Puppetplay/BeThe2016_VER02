//
// 웹크롤러 작업 매니저
//

using System;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using BeThe.Item;
using System.Drawing;
using BeThe.Util;
using System.Linq;
using System.Threading;

namespace BeThe.Crawler
{
    public class Manager : IDisposable
    {
        #region Property & Values

        private ChromeDriver chromeDriver;

        #endregion

        #region Public Functions

        #region Player_W

        // Player_W 정보 얻기
        public List<DbItemBase> GetPlayer_W(String team)
        {
            chromeDriver = InitCromeDriver();
            List<DbItemBase> players = new List<DbItemBase>();
            String teamName = team;
            String teamInitial = Util.Util.GetTeamInitialFromName(teamName);

            if (teamName.ToUpper() == "KT")
            {
                teamName = "kt";
            }

            Int32 errorCount = 0;
            for (Int32 i = 1; i < 7; ++i)
            {
                try
                {
                    CrawlerPlayer_W crawler = new CrawlerPlayer_W(chromeDriver);
                    crawler.Init(teamName, i);
                    String html = crawler.GetHTML();
                    if (html != null)
                    {
                        List<Player_W> ps = ParserPlayer_W.Instance.Parse(html, teamInitial);
                        players = players.Concat(ps).ToList();
                    }
                    errorCount = 0;
                }
                catch (Exception exception)
                {
                    Thread.Sleep(1000);
                    i--;
                    errorCount++;
                    if (errorCount > 5)
                    {
                        throw exception;
                    }
                    continue;
                }
            }
            return players;
        }

        #endregion

        #region Player

        // Player정보 얻기
        public DbItemBase GetPlayer(Player_W player_W)
        {
            Int32 errorCount = 0;
            while (true)
            {
                try
                {
                    chromeDriver = InitCromeDriver();
                    CrawlerPlayer crawler = new CrawlerPlayer(chromeDriver);
                    crawler.Init(player_W.Href);
                    String html = crawler.GetHTML();
                    String[] items = player_W.Href.Split(new String[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    Int32 playerId = Convert.ToInt32(items[items.Length - 1]);
                    DbItemBase player = ParserPlayer.Instance.Parse(html, player_W.Team, playerId);
                    return player;
                }
                catch (Exception e)
                {
                    errorCount++;
                    if (errorCount > 5)
                    {
                        throw e;
                    }
                }
            }
        }

        #endregion

        #region Schedule

        // Schedule 정보 얻기
        public List<DbItemBase> GetSchedule(Int32 year, Int32 month)
        {
            Int32 errorCount = 0;
            while (true)
            {
                try
                {
                    List<DbItemBase> schedules = new List<DbItemBase>();
                    chromeDriver = InitCromeDriver();
                    CrawlerSchedule crawler = new CrawlerSchedule(chromeDriver);
                    crawler.Init(year, month);
                    String html = crawler.GetHTML();
                    var tSchedules = ParserShedule.Instance.Parse(html, year, month);
                    schedules = schedules.Concat(tSchedules).ToList();
                    return schedules;
                }
                catch(Exception e)
                {
                    errorCount++;
                    if(errorCount > 5)
                    {
                        throw e;
                    }
                }
            }
        }

        #endregion

        #region Situation

        // Situation 정보 얻기
        public DbItemBase GetSituation(Schedule schedule)
        {
            Int32 errorCount = 0;
            while (true)
            {
                try
                {
                    List<DbItemBase> schedules = new List<DbItemBase>();
                    chromeDriver = InitCromeDriver();

                    CrawlerSituation crawler = new CrawlerSituation(chromeDriver);
                    crawler.Init(schedule);
                    String html = crawler.GetHTML();
                    DbItemBase situation = ParserSituation_W.Instance.Parse(schedule, html);
                    return situation;

                }
                catch (Exception)
                {
                    errorCount++;
                    if (errorCount > 5)
                    {
                        return null;
                    }
                }
            }
        }

        #endregion

        #region Situation

        // Situation 정보 얻기
        public DbItemBase GetBoxScore(Schedule schedule)
        {
            Int32 errorCount = 0;
            while (true)
            {
                try
                {
                    List<DbItemBase> schedules = new List<DbItemBase>();
                    chromeDriver = InitCromeDriver();

                    CrawlerBoxScore crawler = new CrawlerBoxScore(chromeDriver);
                    crawler.Init(schedule);
                    String html = crawler.GetHTML();
                    DbItemBase boxScore = ParserBoxScore_W.Instance.Parse(schedule, html);
                    return boxScore;

                }
                catch (Exception)
                {
                    errorCount++;
                    if (errorCount > 5)
                    {
                        return null;
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Private Functions

        private ChromeDriver InitCromeDriver()
        {
            if (chromeDriver == null)
            {
                while (true)
                {
                    try
                    {
                        DisposeDriver(chromeDriver);
                        var chromeDriverService = ChromeDriverService.CreateDefaultService();
                        var chromeOptions = new ChromeOptions();
                        chromeDriverService.HideCommandPromptWindow = true;
                        var driver = new ChromeDriver(chromeDriverService, chromeOptions);
                        Thread.Sleep(1000);
                        driver.Manage().Window.Size = new Size(500, 800);
                        return driver;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            return chromeDriver;
        }

        private void DisposeDriver(ChromeDriver chromeDriver)
        {
            if (chromeDriver != null)
            {
                chromeDriver.Dispose();
                chromeDriver = null;
            }
        }

        public void Dispose()
        {
            DisposeDriver(chromeDriver);
        }

        #endregion
    }
}
