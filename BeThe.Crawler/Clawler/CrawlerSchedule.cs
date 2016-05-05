//
// Schedule 크롤러
//

using System;
using OpenQA.Selenium.Chrome;

namespace BeThe.Crawler
{
    internal class CrawlerSchedule : CrawlerBase
    {
        #region Property & Values

        private String itemYear;
        private String itemMonth;
        private readonly String URL = "http://www.koreabaseball.com/Schedule/GameList/General.aspx";

        #endregion

        #region Constructor

        public CrawlerSchedule(ChromeDriver chromeDriver)
            : base(chromeDriver)
        {

        }

        #endregion

        #region public Functions

        public void Init(Int32 year, Int32 month)
        {
            itemYear = year.ToString() + "년";
            itemMonth = month.ToString() + "월";
        }

        public override String GetHTML()
        {
            driver.Navigate().GoToUrl(URL);
            Sleep(1000);
            try
            {
                SetComboBox("cphContainer_cphContents_ddlYear", itemYear);
                Sleep(2000);
                SetComboBox("cphContainer_cphContents_ddlMonth", itemMonth);
                Sleep(2000);
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
