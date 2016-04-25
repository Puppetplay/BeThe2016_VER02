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
            ChromeDriver chromeDriver = null;
            InitCromeDriver(chromeDriver);
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
                                List<Player_W> ps = Parser.ParserPlayer_W.Instance.Parse(html, teamInitial);
                                players = players.Concat(ps).ToList();
                            }
                        }
                        catch
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

        #endregion

        #region Private Functions

        private void InitCromeDriver(ChromeDriver chromeDriver)
        {
            if (chromeDriver == null)
            {
                var chromeDriverService = ChromeDriverService.CreateDefaultService();
                var chromeOptions = new ChromeOptions();
                chromeDriverService.HideCommandPromptWindow = true;
                chromeDriver = new ChromeDriver(chromeDriverService, chromeOptions);
                chromeDriver.Manage().Window.Size = new Size(500, 800);
            }
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
