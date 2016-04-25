//
// 크롤링 결과에서 데이터를 추출하는 ParserBase
//

using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeThe.Crawler
{
    class ParserBase
    {
        #region Abstract Functions

        // HTML Node에서 ClassName으로 Class의 InnerHtml얻기
        protected String GetInnerHtml(HtmlNode node, String className)
        {
            HtmlNode selectedNode = node.SelectSingleNode(String.Format("td [@class='{0}']", className));
            if (selectedNode == null || String.IsNullOrEmpty(selectedNode.InnerHtml.Trim()))
            {
                return null;
            }
            else
            {
                return selectedNode.InnerHtml.Trim();
            }
        }

        // HTML Node에서 path로 InnerHtml얻기
        protected String GetInnerHtmlFromPath(HtmlNode node, String path)
        {
            HtmlNode selectedNode = node.SelectSingleNode(path);
            if (selectedNode == null || String.IsNullOrEmpty(selectedNode.InnerHtml.Trim()))
            {
                return null;
            }
            else
            {
                return selectedNode.InnerHtml.Trim();
            }
        }

        #endregion
    }
}
