//
// Schedule 크롤러
//

using System;
using OpenQA.Selenium.Chrome;
using BeThe.Item;

namespace BeThe.Crawler
{
    internal class CrawlerSituation : CrawlerBase
    {
        #region Property & Values

        private Schedule itemSchedule;

        private readonly String URL = "http://www.koreabaseball.com/Schedule/Game/Situation.aspx?leagueId=1&seriesId=0&gameId={0}&gyear={1}";

        #endregion

        #region Constructor

        public CrawlerSituation(ChromeDriver chromeDriver)
            : base(chromeDriver)
        {

        }

        #endregion

        #region public Functions

        public void Init(Schedule schedule)
        {
            itemSchedule = schedule;
        }

        public override String GetHTML()
        {
            driver.Navigate().GoToUrl(String.Format(URL, itemSchedule.GameId, itemSchedule.Year));
            Sleep(500);
            try
            {
                var element = driver.FindElementById("ui-id-11");
                element.Click();
                Sleep(500);

                var elements = driver.FindElementsByClassName("btn_detail");
                foreach (var el in elements)
                {
                    el.Click();
                    Sleep(50);
                }
                return driver.FindElementByXPath("//HTML").GetAttribute("outerHTML");
            }
            catch
            {
                throw new OpenQA.Selenium.StaleElementReferenceException("페이지 로드오류");
            }
        }

        #endregion
    }
}
