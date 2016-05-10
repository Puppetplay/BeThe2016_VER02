using BeThe.DataAnalyzer;
using BeThe.Item;
using BeThe.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator
{
    class Manager
    {
        public List<Pick> Pick(DateTime date)
        {
            // 경기를 가져온다.
            DatabaseManager dbMgr = new DatabaseManager();
            var players = dbMgr.SelectAll<Player>();
            var matches = dbMgr.SelectAll<Match>();
            var schedules = dbMgr.SelectAll<Schedule>();
            var ths = dbMgr.SelectAll<Th>();
            var bats = dbMgr.SelectAll<Bat>();

            // 경기를 가져온다.
            var tMatches = from m in matches
                           where m.GameId.StartsWith(date.ToString("yyyyMMdd"))
                           select m;

            // 경기별로 돌면서 작업
            BeThe.DataAnalyzer.Manager analyzer = new BeThe.DataAnalyzer.Manager();
            analyzer.MaxDateTime = date;

            foreach (var match in tMatches)
            {
                // 홈팀가져오기 20141004WOLG0
                String teamInitial = match.GameId.Substring(10, 2);
                // 선발투수
                var pitcherId = (from t in ths
                                 join b in bats
                                 on t.Id equals b.ThId
                                 where t.MatchId == match.Id
                                 && t.Number == 1 && t.ThType == ThType.초
                                 select b).First().PitcherId;

                // 타자리스트 얻어오기
                var hitters = analyzer.GetStartPlayerForNDays(teamInitial, 4);
                List
                foreach(var hitter in hitters)
                {
                    GetHitterInfo(hitter.PlayerId);
                }

                var pitcherInfo = analyzer.GetPitcherInfo(pitcherId, 4);
            }
            return null;
        }

        private HitterInfo GetHitterInfo(Int32 playerId)
        {
            return null;
        }
    }
}
