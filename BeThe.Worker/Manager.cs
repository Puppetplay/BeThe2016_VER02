//
// 전체 작업의 기본 클래스
//

using BeThe.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BeThe.Worker
{
    public class Manager
    {
        #region Public Functions

        // Player_W 얻어오기
        public async Task Run(WorkType workType)
        {
            Task task = new Task(CreateAction());
            task.Start();
            await task;
        }                

        #endregion

        #region Private Functions

        // 각 작업타입의 액션을 생성한다.
        private List<DbItemBase> CreateAction(WorkType workType)
        {
            var CrawlerMgr = new Crawler.Manager();
            List<DbItemBase> items = null;
            if (workType == WorkType.Player_W)
            {
                items = CrawlerMgr.GetPlayer_W();
            }

            return items;
        }

        private Action CreateAction()
        {
            var action = new Action(AA);
            return action;
        }

        private void AA()
        {
            Console.WriteLine("AA");
        }
        #endregion
    }
}
