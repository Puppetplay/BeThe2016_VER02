//
// 데이터 분석기
//

using BeThe.DataAnalyzer.item;
using BeThe.Item;
using BeThe.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeThe.DataAnalyzer
{
    public class Manager
    {
        // 투수의 분석 정보를 가져온다. gameCount : 지난 몇개의 경기를 볼껀지
        public PitcherInfo GetPitcherInfo(Int32 pitcherId, Int32 gameCount)
        {
            PitcherInfo pitcherInfo = new PitcherInfo();

            DatabaseManager dbMgr = new DatabaseManager();
            var players = dbMgr.SelectAll<Player>();
            var matches = dbMgr.SelectAll<Match>();
            var schedules = dbMgr.SelectAll<Schedule>();
            var ths = dbMgr.SelectAll<Th>();
            var bats = dbMgr.SelectAll<Bat>();

            // 투수가 출전한 경기를 가져온다.
            var matchIds = (from m in matches
                            join s in schedules
                                on m.GameId equals s.GameId
                            join t in ths
                                on m.Id equals t.MatchId
                            join b in bats
                                on t.Id equals b.ThId
                            group t by new { m.Id, t.Number, b.PitcherId } into g
                            where g.Key.PitcherId == pitcherId && g.Key.Number == 1
                            select g.Key.Id);

            var tMatches = (from m in matches
                            where matchIds.Contains(m.Id)
                            orderby m.GameId descending
                            select m).Take(gameCount);





            // 각경기마다 투수의 기록을 구한다.
            var tBats = from m in tMatches
                        join t in ths
                            on m.Id equals t.MatchId
                        join b in bats
                            on t.Id equals b.ThId
                        join p in players
                            on b.HitterId equals p.PlayerId
                        where b.PitcherId == pitcherId
                        select b;

            // 출루율
            pitcherInfo.OnBaseRatio = (from b in tBats
                                       group b by b.PitcherId into g
                                       select
                                       g.Count(x => (x.PResult == PResultType.Pass ||
                                            x.PResult == PResultType.Hit)) * 1.0
                                        / g.Count(x => x.PitcherId == pitcherId)).First();

            // 안타율
            pitcherInfo.HitRatio = (from b in tBats
                                    group b by b.PitcherId into g
                                    select
                                    g.Count(x => x.PResult == PResultType.Hit) * 1.0
                                     / (g.Count(x => x.PitcherId == pitcherId)
                                     - g.Count(x => x.PResult == PResultType.Pass))
                                        ).First();

            // 좌타자 안타율
            var lBats = from b in tBats
                        join p in players
                        on b.HitterId equals p.PlayerId
                        where b.PitcherId == pitcherId && p.Hand.Contains("좌타")
                        select b;
            pitcherInfo.HitRatioLHand = (from b in lBats
                                         group b by b.PitcherId into g
                                         select
                                         g.Count(x => x.PResult == PResultType.Hit) * 1.0
                                          / (g.Count(x => x.PitcherId == pitcherId)
                                          - g.Count(x => x.PResult == PResultType.Pass))
                                        ).First();

            // 우타자 안타율
            var rBats = from b in tBats
                        join p in players
                        on b.HitterId equals p.PlayerId
                        where b.PitcherId == pitcherId && p.Hand.Contains("우타")
                        select b;
            pitcherInfo.HitRatioRHand = (from b in rBats
                                         group b by b.PitcherId into g
                                         select
                                         g.Count(x => x.PResult == PResultType.Hit) * 1.0
                                          / (g.Count(x => x.PitcherId == pitcherId)
                                          - g.Count(x => x.PResult == PResultType.Pass))
                                        ).First();

            // 우타자 안타율
            var xBats = from b in tBats
                        join p in players
                        on b.HitterId equals p.PlayerId
                        where b.PitcherId == pitcherId && (p.Hand.Contains("우타") == false &&
                            p.Hand.Contains("좌타") == false)
                        select b;

            return pitcherInfo;
        }

        // 팀별로 최근 N경기에 모두 선발 출장인 타자를 가지고 온다.
        public List<Player> GetStartPlayerForNDays(String teamInitial, Int32 days)
        {
            DatabaseManager dbMgr = new DatabaseManager();
            var players = dbMgr.SelectAll<Player>();
            var matches = dbMgr.SelectAll<Match>();
            var schedules = dbMgr.SelectAll<Schedule>();
            var ths = dbMgr.SelectAll<Th>();
            var bats = dbMgr.SelectAll<Bat>();
            var boxScores = dbMgr.SelectAll<BoxScore_W>();

            // 입력된 팀의 최근 n경기를 가지고 온다.
            var tMatches = (from m in matches
                            join s in schedules
                            on m.GameId equals s.GameId
                            where s.HomeTeam == teamInitial || s.AwayTeam == teamInitial
                            orderby m.GameId descending
                            select m).Take(days); 


            
            // 모든경기에 스타팅 멤버로 출전한 타자를 가지고 온다.
            var tBats = from m in tMatches
                        join t in ths
                        on m.Id equals t.MatchId
                        join b in bats
                        on t.Id equals b.ThId
                        select b;


            return null;
        }
    }
}
