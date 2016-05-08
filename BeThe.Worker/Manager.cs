//
// 전체 작업의 기본 클래스
//

using BeThe.Item;
using BeThe.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace BeThe.Worker
{
    public class Manager
    {
        #region Public Functions

        private readonly Int32 workCount = 10;
        #endregion

        #region Public Functions

        // Run 모든요청은 이함수로 받는다.
        public async Task Run(WorkType workType)
        {
            var tasks = CreateTaskTasks(workType);
            if (tasks == null)
            {
                return;
            }

            foreach (var task in tasks)
            {
                await task;
            }
        }

        // 테스트함수
        public void RunTest()
        {
            using (var crawlerMgr = new Crawler.Manager())
            {
                var temp = crawlerMgr.GetPlayer_W("삼성");
            }
        }

        #endregion

        #region Private Functions

        // Task 를 생성한다.
        private List<Task> CreateTaskTasks(WorkType workType)
        {
            if (workType == WorkType.Player_W)
            {
                return CreateTasks_PlayerW();
            }
            if (workType == WorkType.Player)
            {
                return CreateTasks_Player();
            }
            if (workType == WorkType.Schedule)
            {
                return CreateTasks_Schedule();
            }
            if (workType == WorkType.Situation)
            {
                return CreateTasks_Situation();
            }
            if (workType == WorkType.BoxScore)
            {
                return CreateTasks_BoxScore();
            }
            if (workType == WorkType.MakeMatch)
            {
                return CreateTasks_MakeMatch();
            }
            if (workType == WorkType.LineUp)
            {
                return CreateTasks_LineUp();
            }
            return null;
        }

        #region Crawler

        #region PlayerW

        // PlayerW Tasks 생성
        private List<Task> CreateTasks_PlayerW()
        {
            DatabaseManager dbMgr = new DatabaseManager();
            dbMgr.DataContext.ExecuteCommand("TRUNCATE TABLE Player_W");

            List<Task> tasks = new List<Task>();
            var itemGruop = SplitGroup(Util.Util.Teams, workCount);
            foreach (var item in itemGruop)
            {
                tasks.Add(Task.Factory.StartNew(() => RunPlayerW(item)));
            }
            return tasks;
        }

        // PlayerW  작업
        private void RunPlayerW(List<String> teams)
        {
            List<DbItemBase> player_Ws = new List<DbItemBase>();
            DatabaseManager dbMgr = new DatabaseManager();
            using (var crawlerMgr = new Crawler.Manager())
            {
                foreach (String team in teams)
                {
                    var items = crawlerMgr.GetPlayer_W(team);
                    player_Ws = player_Ws.Concat(items).ToList();
                }
            }
            dbMgr.Save(player_Ws);
        }

        // 전체작업을 작업 카운트에 따라서 분할
        private List<List<String>> SplitGroup(String[] items, Int32 count)
        {
            List<List<String>> itemGroup = new List<List<String>>();
            for (Int32 i = 0; i < count; ++i)
            {
                itemGroup.Add(new List<String>());
            }

            for (Int32 i = 0; i < items.Length; ++i)
            {
                Int32 mod = i % count;
                itemGroup[mod].Add(items[i]);
            }

            return itemGroup;
        }

        #endregion

        #region Player

        // PlayerW Tasks 생성
        private List<Task> CreateTasks_Player()
        {
            DatabaseManager dbMgr = new DatabaseManager();
            dbMgr.DataContext.ExecuteCommand("TRUNCATE TABLE Player");

            // 가져와야할 선수리스트 얻기
            var player_Ws = dbMgr.SelectAll<Player_W>();
            var itemGroup = SplitGroup(player_Ws.ToList(), workCount);
            List<Task> tasks = new List<Task>();
            foreach (var items in itemGroup)
            {
                tasks.Add(Task.Factory.StartNew(() => RunPlayer(items)));
            }
            return tasks;
        }

        // PlayerW  작업
        private void RunPlayer(List<Player_W> items)
        {
            List<DbItemBase> player_Ws = new List<DbItemBase>();
            DatabaseManager dbMgr = new DatabaseManager();
            using (var crawlerMgr = new Crawler.Manager())
            {
                foreach (var item in items)
                {
                    player_Ws.Add(crawlerMgr.GetPlayer(item));
                }
            }
            dbMgr.Save(player_Ws);
        }

        // 전체작업을 작업 카운트에 따라서 분할
        private List<List<Player_W>> SplitGroup(List<Player_W> items, Int32 count)
        {
            List<List<Player_W>> itemGroup = new List<List<Player_W>>();
            for (Int32 i = 0; i < count; ++i)
            {
                itemGroup.Add(new List<Player_W>());
            }

            for (Int32 i = 0; i < items.Count; ++i)
            {
                Int32 mod = i % count;
                itemGroup[mod].Add(items[i]);
            }

            return itemGroup;
        }

        #endregion

        #region Schedule

        // PlayerW Tasks 생성
        private List<Task> CreateTasks_Schedule()
        {
            DatabaseManager dbMgr = new DatabaseManager();

            // 일정불러오기 
            var allSchedule = dbMgr.SelectAll<Schedule>();
            Int32 startYear = 2013;
            Int32 startMonth = 3;

            if (allSchedule.Count() > 0)
            {
                startYear = (from schedule in allSchedule select schedule.Year).Max();
                startMonth = (from schedule in allSchedule where schedule.Year == startYear select schedule.Month).Max();
            }

            DateTime startDate = new DateTime(startYear, startMonth, 1);
            DateTime endDate = DateTime.Now;

            // 이전 데이터 지우기
            var scheduleTable = dbMgr.DataContext.GetTable<Schedule>();
            var delSchedule = from schedule in allSchedule
                              where schedule.Year == startDate.Year && schedule.Month >= startDate.Month
                              select schedule;

            foreach (var schedule in delSchedule)
            {
                scheduleTable.DeleteOnSubmit(schedule);
            }
            dbMgr.DataContext.SubmitChanges();

            // 가지고와야할 리스트 만들기
            List<DateTime> dateTimes = new List<DateTime>();
            while (startDate <= endDate)
            {
                dateTimes.Add(startDate);
                startDate = startDate.AddMonths(1);
            }

            // 가져와야할 스케줄 DateTime 얻기
            var itemGroup = SplitGroup(dateTimes, workCount);
            List<Task> tasks = new List<Task>();
            foreach (var items in itemGroup)
            {
                tasks.Add(Task.Factory.StartNew(() => RunSchedule(items)));
            }
            return tasks;
        }

        // PlayerW  작업
        private void RunSchedule(List<DateTime> items)
        {
            List<DbItemBase> schedules = new List<DbItemBase>();
            DatabaseManager dbMgr = new DatabaseManager();
            using (var crawlerMgr = new Crawler.Manager())
            {
                foreach (var item in items)
                {
                    var tSchedules = crawlerMgr.GetSchedule(item.Year, item.Month);
                    schedules = schedules.Concat(tSchedules).ToList();
                }
            }
            dbMgr.Save(schedules);
        }

        // 전체작업을 작업 카운트에 따라서 분할
        private List<List<DateTime>> SplitGroup(List<DateTime> items, Int32 count)
        {
            List<List<DateTime>> itemGroup = new List<List<DateTime>>();
            for (Int32 i = 0; i < count; ++i)
            {
                itemGroup.Add(new List<DateTime>());
            }

            for (Int32 i = 0; i < items.Count; ++i)
            {
                Int32 mod = i % count;
                itemGroup[mod].Add(items[i]);
            }

            return itemGroup;
        }

        #endregion

        #region Situation

        // PlayerW Tasks 생성
        private List<Task> CreateTasks_Situation()
        {
            DatabaseManager dbMgr = new DatabaseManager();

            // 불러와야할 스케줄 가져오기
            var schedules = from schedule in dbMgr.SelectAll<Schedule>()
                            join situation in dbMgr.SelectAll<Situation_W>()
                            on schedule.GameId equals situation.GameId into t
                            from subSituation in t.DefaultIfEmpty()
                            where schedule.LeagueId == 1 && schedule.SeriesId == 0
                            && schedule.Href != null && subSituation.Content == null
                            select schedule;

            // 작업그룹 만들기 
            var itemGroup = SplitGroup(schedules.ToList(), workCount);

            // 작업
            List<Task> tasks = new List<Task>();
            foreach (var items in itemGroup)
            {
                tasks.Add(Task.Factory.StartNew(() => RunSchedule(items)));
            }
            return tasks;
        }

        // PlayerW  작업
        private void RunSchedule(List<Schedule> items)
        {
            List<DbItemBase> situations = new List<DbItemBase>();
            DatabaseManager dbMgr = new DatabaseManager();
            using (var crawlerMgr = new Crawler.Manager())
            {
                foreach (var item in items)
                {
                    var situation = crawlerMgr.GetSituation(item);
                    if (situation != null)
                    {
                        situations.Add(situation);
                    }
                }
            }
            dbMgr.Save(situations);
        }

        // 전체작업을 작업 카운트에 따라서 분할
        private List<List<Schedule>> SplitGroup(List<Schedule> items, Int32 count)
        {
            List<List<Schedule>> itemGroup = new List<List<Schedule>>();
            for (Int32 i = 0; i < count; ++i)
            {
                itemGroup.Add(new List<Schedule>());
            }

            for (Int32 i = 0; i < items.Count; ++i)
            {
                Int32 mod = i % count;
                itemGroup[mod].Add(items[i]);
            }

            return itemGroup;
        }

        #endregion

        #region BoxScore

        // BoxScore Tasks 생성
        private List<Task> CreateTasks_BoxScore()
        {
            DatabaseManager dbMgr = new DatabaseManager();

            // 불러와야할 스케줄 가져오기
            var schedules = from schedule in dbMgr.SelectAll<Schedule>()
                            join boxScore in dbMgr.SelectAll<BoxScore_W>()
                            on schedule.GameId equals boxScore.GameId into t
                            from subBoxScore in t.DefaultIfEmpty()
                            where schedule.LeagueId == 1 && schedule.SeriesId == 0
                            && schedule.Href != null && subBoxScore.AwayHitter == null
                            select schedule;

            // 작업그룹 만들기 
            var itemGroup = SplitGroup(schedules.ToList(), 3);

            // 작업
            List<Task> tasks = new List<Task>();
            foreach (var items in itemGroup)
            {
                tasks.Add(Task.Factory.StartNew(() => RunBoxScore(items)));
            }
            return tasks;
        }

        // PlayerW  작업
        private void RunBoxScore(List<Schedule> items)
        {
            List<DbItemBase> boxScores = new List<DbItemBase>();
            DatabaseManager dbMgr = new DatabaseManager();
            using (var crawlerMgr = new Crawler.Manager())
            {
                foreach (var item in items)
                {
                    var boxScore = crawlerMgr.GetBoxScore(item);
                    if (boxScore != null)
                    {
                        boxScores.Add(boxScore);
                    }
                }
            }
            dbMgr.Save(boxScores);
        }

        #endregion

        #endregion

        #region Maker

        #endregion

        // MakeMath 생성
        private List<Task> CreateTasks_MakeMatch()
        {
            List<Task> tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(() => MakeMatch()));
            return tasks;
        }

        private void MakeMatch()
        {
            DataMaker.Manager mgr = new DataMaker.Manager();
            Util.DatabaseManager dbMgr = new Util.DatabaseManager();
            var schedules = from schedule in dbMgr.SelectAll<Schedule>()
                            join match in dbMgr.SelectAll<Match>()
                            on schedule.GameId equals match.GameId into t
                            from subMatch in t.DefaultIfEmpty()
                            where schedule.LeagueId == 1 && schedule.SeriesId == 0
                            && schedule.Href != null && subMatch.GameId == null
                            select schedule;

            foreach (var schedule in schedules)
            {
                dbMgr = new Util.DatabaseManager();
                var situation = (from s in dbMgr.SelectAll<Situation_W>()
                                 where s.GameId == schedule.GameId
                                 select s).First();

                var boxScore = (from b in dbMgr.SelectAll<BoxScore_W>()
                                where b.GameId == schedule.GameId
                                select b).First();

                var match = mgr.MakeMatch(situation, boxScore);

                // Match 저장
                List<Match> matchs = new List<Match>();
                matchs.Add(match);
                dbMgr.Save<Match>(matchs);

                // Th 저장
                List<Th> ths = new List<Th>();
                foreach (var th in match.Ths)
                {
                    th.MatchId = match.Id;
                    ths.Add(th);
                }
                dbMgr.Save<Th>(ths);

                // Bat 저장
                List<Bat> bats = new List<Bat>();
                foreach (var th in match.Ths)
                {
                    if (th.Bats != null)
                    {
                        foreach (var bat in th.Bats)
                        {
                            bat.ThId = th.Id;
                            bats.Add(bat);
                        }
                    }
                }
                dbMgr.Save<Bat>(bats);

                // Ball 저장
                List<Ball> balls = new List<Ball>();
                foreach (var th in match.Ths)
                {
                    if (th.Bats != null)
                    {
                        foreach (var bat in th.Bats)
                        {
                            foreach (var ball in bat.Balls)
                            {
                                ball.BatId = bat.Id;
                                balls.Add(ball);
                            }
                        }
                    }
                }
                dbMgr.Save<Ball>(balls);
            }
        }

        // LineUp Tasks 생성
        private List<Task> CreateTasks_LineUp()
        {
            List<Task> tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(() => MakeLineUp()));
            return tasks;
        }

        private void MakeLineUp()
        {
            DataMaker.Manager mgr = new DataMaker.Manager();
            try
            {
                Util.DatabaseManager dbMgr = new Util.DatabaseManager();

                var matches = from match in dbMgr.SelectAll<Match>()
                              join lineUp in dbMgr.SelectAll<LineUp>()
                              on match.Id equals lineUp.Id into t
                              from subLineUp in t.DefaultIfEmpty()
                              where subLineUp == null
                              select match;

                foreach (var match in matches)
                {
                    var boxScore = (from b in dbMgr.SelectAll<BoxScore_W>()
                                    where b.GameId == match.GameId
                                    select b).First();
                    var lineUps = mgr.MakeLineUp(match.Id, boxScore);
                    dbMgr.Save<LineUp>(lineUps);
                }
            }
            finally
            {
            }
        }


        #endregion
    }
}
