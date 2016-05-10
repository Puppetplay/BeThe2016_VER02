//
// 데이터 분석기
//

using BeThe.Item;
using BeThe.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeThe.DataAnalyzer
{
    public class Manager
    {
        public DateTime MaxDateTime { get; set; }

        public Manager()
        {
            MaxDateTime = DateTime.Now;
        }

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
                            where 
                                t.Number == 1 &&
                                s.Year * 10000 + s.Minute * 100 + s.Day <
                                MaxDateTime.Year * 10000 + MaxDateTime.Minute * 100 + MaxDateTime.Day
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
            var lineUps = dbMgr.SelectAll<LineUp>();

            // 입력된 팀의 최근 n경기를 가지고 온다.
            var tMatches = (from m in matches
                            join s in schedules
                            on m.GameId equals s.GameId
                            where s.HomeTeam == teamInitial || s.AwayTeam == teamInitial
                            &&
                                s.Year * 10000 + s.Minute * 100 + s.Day <
                                MaxDateTime.Year * 10000 + MaxDateTime.Minute * 100 + MaxDateTime.Day
                            orderby m.GameId descending
                            select m).Take(days);

            // 모든경기에 스타팅 멤버로 출전한 타자를 가지고 온다.
            var tPlayers = from m in tMatches
                           join l in lineUps
                           on m.Id equals l.MatchId
                           join p in players
                           on l.PlayerId equals p.PlayerId
                           where l.EntryType == EntryType.Starting
                           && p.Team == teamInitial
                           group p by p.Id into g
                           where g.Count() == days
                           select g.First();
            return tPlayers.ToList();
        }

        // 주어진 타자의 최근 N게임에서의 안타수를 구한다.
        public Int32 GetHitNumberForNDays(String teamInitial, Int32 playerId, Int32 days)
        {
            DatabaseManager dbMgr = new DatabaseManager();
            var players = dbMgr.SelectAll<Player>();
            var matches = dbMgr.SelectAll<Match>();
            var schedules = dbMgr.SelectAll<Schedule>();
            var ths = dbMgr.SelectAll<Th>();
            var bats = dbMgr.SelectAll<Bat>();

            // 입력된 팀의 최근 n경기를 가지고 온다.
            var tMatches = (from m in matches
                            join s in schedules
                            on m.GameId equals s.GameId
                            where s.HomeTeam == teamInitial || s.AwayTeam == teamInitial
                            &&
                                s.Year * 10000 + s.Minute * 100 + s.Day <
                                MaxDateTime.Year * 10000 + MaxDateTime.Minute * 100 + MaxDateTime.Day
                            orderby m.GameId descending
                            select m).Take(days);

            // 타자의 안타수를 구한다.
            var tBats = from m in tMatches
                        join t in ths
                            on m.Id equals t.MatchId
                        join b in bats
                            on t.Id equals b.ThId
                        where b.PResult == PResultType.Hit && b.HitterId == playerId
                        select b;

            return tBats.Count();
        }

        // 주어진 타자의 최근 N게임에서의 안타수를 구한다.
        public Boolean IsAllDayHit(String teamInitial, Int32 playerId, Int32 days)
        {
            DatabaseManager dbMgr = new DatabaseManager();
            var players = dbMgr.SelectAll<Player>();
            var matches = dbMgr.SelectAll<Match>();
            var schedules = dbMgr.SelectAll<Schedule>();
            var ths = dbMgr.SelectAll<Th>();
            var bats = dbMgr.SelectAll<Bat>();

            // 입력된 팀의 최근 n경기를 가지고 온다.
            var tMatches = (from m in matches
                            join s in schedules
                            on m.GameId equals s.GameId
                            where s.HomeTeam == teamInitial || s.AwayTeam == teamInitial
                            &&
                                s.Year * 10000 + s.Minute * 100 + s.Day <
                                MaxDateTime.Year * 10000 + MaxDateTime.Minute * 100 + MaxDateTime.Day
                            orderby m.GameId descending
                            select m).Take(days);

            // 모든경기에 안타가 있는지 확인
            var tBats = from m in tMatches
                        join t in ths
                            on m.Id equals t.MatchId
                        join b in bats
                            on t.Id equals b.ThId
                        where b.PResult == PResultType.Hit && b.HitterId == playerId
                        group b by m.GameId into g
                        select g;

            return tBats.Count() == days;
        }

        // 주어진 타자의 최근 N게임에서의 타율
        public Double GetHitRatio(String teamInitial, Int32 playerId, Int32 days)
        {
            DatabaseManager dbMgr = new DatabaseManager();
            var players = dbMgr.SelectAll<Player>();
            var matches = dbMgr.SelectAll<Match>();
            var schedules = dbMgr.SelectAll<Schedule>();
            var ths = dbMgr.SelectAll<Th>();
            var bats = dbMgr.SelectAll<Bat>();

            // 입력된 팀의 최근 n경기를 가지고 온다.
            var tMatches = (from m in matches
                            join s in schedules
                            on m.GameId equals s.GameId
                            where s.HomeTeam == teamInitial || s.AwayTeam == teamInitial
                            &&
                                s.Year * 10000 + s.Minute * 100 + s.Day <
                                MaxDateTime.Year * 10000 + MaxDateTime.Minute * 100 + MaxDateTime.Day
                            orderby m.GameId descending
                            select m).Take(days);

            // 모든경기에 안타가 있는지 확인
            var tBats = (from m in tMatches
                         join t in ths
                             on m.Id equals t.MatchId
                         join b in bats
                             on t.Id equals b.ThId
                         where b.HitterId == playerId
                         group b by b.HitterId into g
                         select
                         (
                             g.Count(x => x.PResult == PResultType.Hit) * 1.0 /
                             (g.Count() -
                             g.Count(x => x.PResult == PResultType.Pass))
                         )).First();

            // 모든경기에 안타가 있는지 확인
            var aa = (from m in tMatches
                      join t in ths
                          on m.Id equals t.MatchId
                      join b in bats
                          on t.Id equals b.ThId
                      where b.HitterId == playerId
                      group b by b into g
                      select new
                      {
                          a = g.Count(x => x.PResult == PResultType.Hit) * 1.0
                      });

            return tBats;
        }

        // 지난경기에 2루타 이상이 있었는지
        public Boolean IsLongHitLastGame(String teamInitial, Int32 playerId)
        {
            DatabaseManager dbMgr = new DatabaseManager();
            var players = dbMgr.SelectAll<Player>();
            var matches = dbMgr.SelectAll<Match>();
            var schedules = dbMgr.SelectAll<Schedule>();
            var ths = dbMgr.SelectAll<Th>();
            var bats = dbMgr.SelectAll<Bat>();

            // 입력된 팀의 최근 n경기를 가지고 온다.
            var tMatches = (from m in matches
                            join s in schedules
                            on m.GameId equals s.GameId
                            where s.HomeTeam == teamInitial || s.AwayTeam == teamInitial
                            &&
                                s.Year * 10000 + s.Minute * 100 + s.Day <
                                MaxDateTime.Year * 10000 + MaxDateTime.Minute * 100 + MaxDateTime.Day
                            orderby m.GameId descending
                            select m).Take(1);

            // 모든경기에 안타가 있는지 확인
            var tBats = (from m in tMatches
                         join t in ths
                             on m.Id equals t.MatchId
                         join b in bats
                             on t.Id equals b.ThId
                         where b.HitterId == playerId
                         select b);

            foreach(var b in tBats)
            {
                var result = b.Result.Trim();
                if(result.EndsWith("2") || result.EndsWith("3") || result.EndsWith("홈"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
