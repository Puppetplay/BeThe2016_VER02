//
// 전체 작업의 기본 클래스
//

using BeThe.Item;
using BeThe.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeThe.Worker
{
    public class Manager
    {
        #region Public Functions

        // Player_W 얻어오기
        public async Task Run(WorkType workType)
        {
            var task = CreateTask(workType);
            await task;
        }                

        #endregion

        #region Private Functions

        // Task 를 생성한다.
        public Task CreateTask(WorkType workType)
        {
            var task = Task.Factory.StartNew(() => DoWork(workType));
            return task;
        }

        // 작업을한다.
        private void DoWork(WorkType workType)
        {
            // 웹 크롤링하기
            var items = Select(workType);

            // 데이터 저장하기
            Save(workType, items);
        }

        // 데이터 크롤링하기
        private List<DbItemBase> Select(WorkType workType)
        {
            List<DbItemBase> items = null;
            var CrawlerMgr = new Crawler.Manager();
            if (workType == WorkType.Player_W)
            {
                items = CrawlerMgr.GetPlayer_W();
            }
            else if(workType == WorkType.Player)
            {
                items = CrawlerMgr.GetPlayer();
            }
            return items;
        }

        // 데이터 저장하기
        private void Save(WorkType workType, List<DbItemBase> items)
        {
            if(items == null) { return; }

            DatabaseManager dbMgr = new DatabaseManager();

            if(workType == WorkType.Player_W)
            {
                dbMgr.DataContext.ExecuteCommand("TRUNCATE TABLE Player_W");
            }
            else if(workType == WorkType.Player)
            {
                dbMgr.DataContext.ExecuteCommand("TRUNCATE TABLE Player");
            }

            dbMgr.Save(items);
        }

        #endregion
    }
}
