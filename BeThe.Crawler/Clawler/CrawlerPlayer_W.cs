//
// Player_W 크롤러
//

using System;
using OpenQA.Selenium.Chrome;

namespace BeThe.Crawler
{
    internal class CrawlerPlayer_W : CrawlerBase
    {
        #region Property & Values

        // 데이터를 가지고 올 URL주소
        private readonly String URL = "http://www.koreabaseball.com/Player/Search.aspx";

        private String teamInitial;
        private Int32 page;

        #endregion

        #region Constructor

        public CrawlerPlayer_W(ChromeDriver chromeDriver)
            : base(chromeDriver)
        {

        }

        #endregion

        #region Public Functoins

        // 초기 조건 데이터 설정
        public void Init(String teamInitial, Int32 page)
        {
            this.teamInitial = teamInitial;
            this.page = page;
        }

        // 크롤링 후 데이터 얻기
        public override String GetHTML()
        {
            driver.Navigate().GoToUrl(URL);
            SetComboBox("cphContainer_cphContents_ddlTeam", teamInitial);
            Sleep(2000);
            if (page > 5)
            {
                if (IsExistElement("cphContainer_cphContents_ucPager_btnNext"))
                {
                    ClickButton("cphContainer_cphContents_ucPager_btnLast");
                    Sleep(2000);
                }
                else
                {
                    return null;
                }
            }

            Int32 index = (page - 1 % 5) + 1;
            String id = "cphContainer_cphContents_ucPager_btnNo" + index.ToString();
            if (IsExistElement(id))
            {
                ClickButton(id);
                Sleep(2000);
                return driver.FindElementByXPath("//tbody").GetAttribute("outerHTML");
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
