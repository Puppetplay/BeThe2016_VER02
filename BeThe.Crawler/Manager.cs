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

namespace BeThe.Crawler
{
    public class Manager
    {
        #region Property & Values

        #endregion

        #region Public Functions

        #region Player_W

        // Player_W 정보 얻기
        public List<DbItemBase> GetPlayer_W()
        {
            ChromeDriver chromeDriver = InitCromeDriver();
            List<DbItemBase> players = new List<DbItemBase>();
            try
            {
                foreach (var team in Util.Util.Teams)
                {
                    String teamName = team;
                    String teamInitial = Util.Util.GetTeamInitialFromName(teamName);

                    if (teamName.ToUpper() == "KT")
                    {
                        teamName = "kt";
                    }

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
                        }
                        catch(Exception e)
                        {
                            i--;
                            continue;
                        }
                    }
                }
            }
            finally
            {
                DisposeDriver(chromeDriver);
            }
            return players;
        }

        #endregion

        #region Player

        // Player정보 얻기
        public List<DbItemBase> GetPlayer()
        {
            ChromeDriver chromeDriver = InitCromeDriver();
            List<DbItemBase> players = new List<DbItemBase>();
            try
            {
                CrawlerPlayer crawler = new CrawlerPlayer(chromeDriver);
                DatabaseManager dbMgr = new DatabaseManager();
                var player_Ws = dbMgr.SelectAll<Player_W>();
                foreach (var player_W in player_Ws)
                {
                    try
                    {
                        crawler.Init(player_W.Href);
                        String html = crawler.GetHTML();
                        String[] items = player_W.Href.Split(new String[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                        Int32 playerId = Convert.ToInt32(items[items.Length - 1]);
                        var player = ParserPlayer.Instance.Parse(html, player_W.Team, playerId);
                        players.Add(player);
                    }
                    catch
                    {

                    }
                }
            }
            finally
            {
                DisposeDriver(chromeDriver);
            }
            return players;
        }

        #endregion

        #endregion

        #region Private Functions

        private ChromeDriver InitCromeDriver()
        {

            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            var chromeOptions = new ChromeOptions();
            chromeDriverService.HideCommandPromptWindow = true;
            var chromeDriver = new ChromeDriver(chromeDriverService, chromeOptions);
            chromeDriver.Manage().Window.Size = new Size(500, 800);
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
        #endregion
    }
}
