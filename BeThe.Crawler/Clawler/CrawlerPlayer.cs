using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeThe.Crawler
{
    internal class CrawlerPlayer : CrawlerBase
    {
        #region Property & Values

        private readonly String URL = "http://www.koreabaseball.com";
        private String address;

        #endregion

        #region Constructor

        public CrawlerPlayer(ChromeDriver chromeDriver)
            : base(chromeDriver)
        {

        }

        #endregion

        #region public Functions

        public void Init(String href)
        {
            address = (URL + href);
        }

        public override String GetHTML()
        {
            driver.Navigate().GoToUrl(address);
            Sleep(1000);
            try
            {
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
