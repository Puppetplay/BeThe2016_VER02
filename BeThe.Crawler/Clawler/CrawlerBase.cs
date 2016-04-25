//
// 웹크롤러 기본 클래스
//

using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace BeThe.Crawler
{
    abstract class CrawlerBase
    {
        #region Property & Values

        // 크롬드라이버
        protected ChromeDriver driver;

        #endregion

        #region Constructor

        // 크롬드라이버를 입력받는다.
        public CrawlerBase(ChromeDriver chromeDriver)
        {
            driver = chromeDriver;
        }

        #endregion

        #region Abstract Functions

        // 웹크롤링 결과 얻기
        public abstract String GetHTML();

        #endregion

        #region Protected Functions

        // 콤보박스선택
        protected void SetComboBox(String id, String value)
        {
            var combo = driver.FindElementById(id);
            var options = combo.FindElements(By.TagName("option"));
            var option = (from o in options
                          where o.Text == value
                          select o).First();
            if (option == null)
            {
                throw new Exception();
            }
            else
            {
                option.Click();
            }
        }

        // 주어진 Tick만큼 대기
        protected void Sleep(Int32 tick)
        {
            System.Threading.Thread.Sleep(tick);
        }

        // 버튼클릭
        protected void ClickButton(String id)
        {
            var button = driver.FindElementById(id);
            button.Click();
        }

        // 엘레멘트 존재여부 확인
        protected Boolean IsExistElement(String id)
        {
            try
            {
                driver.FindElementById(id);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
