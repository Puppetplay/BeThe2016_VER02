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
        private DatabaseManager dbMgr;

        IQueryable<Player> players;
        IQueryable<Match> matches;
        IQueryable<Schedule> schedules;
        IQueryable<Th> ths;
        IQueryable<Bat> bats;
        IQueryable<LineUp> lineUps;

        public Manager()
        {
            MaxDateTime = DateTime.Now;
            dbMgr = new DatabaseManager();
            players = dbMgr.SelectAll<Player>();
            matches = dbMgr.SelectAll<Match>();
            schedules = dbMgr.SelectAll<Schedule>();
            ths = dbMgr.SelectAll<Th>();
            bats = dbMgr.SelectAll<Bat>();
            lineUps = dbMgr.SelectAll<LineUp>();
        }

        // 투수의 분석 정보를 가져온다. gameCount : 지난 몇개의 경기를 볼껀지
        public PitcherInfo GetPitcherInfo(Int32 playerId, Int32 gameCount)
        {
            PitcherInfo pitcherInfo = new PitcherInfo();
            pitcherInfo.PlayerId = playerId;
            pitcherInfo.PlayerName = GetPlayerName(playerId);
            pitcherInfo.Hand = GetPlayerHand(playerId);


            Int32 dateNumber = MaxDateTime.Year * 10000 + MaxDateTime.Month * 100 + MaxDateTime.Day;
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
                                s.Year * 10000 + s.Month * 100 + s.Day <
                                dateNumber
                            group t by new { m.Id, t.Number, b.PitcherId } into g
                            where g.Key.PitcherId == playerId && g.Key.Number == 1
                            select g.Key.Id);

            var tMatches = (from m in matches
                            where matchIds.Contains(m.Id)
                            orderby m.GameId descending
                            select m).Take(gameCount);


            // 각경기마다 투수의 기록을 구한다.
            var tBats = (from m in tMatches
                         join t in ths
                             on m.Id equals t.MatchId
                         join b in bats
                             on t.Id equals b.ThId
                         join p in players
                             on b.HitterId equals p.PlayerId
                         where b.PitcherId == playerId
                         select b).ToList();

            // 출루율 
            pitcherInfo.OnBaseRatio = (from b in tBats
                                       group b by b.PitcherId into g
                                       select
                                       g.Count(x => (x.PResult == PResultType.Pass ||
                                            x.PResult == PResultType.Hit)) * 1.0
                                        / g.Count()).First();

            // 안타율
            pitcherInfo.HitRatio = (from b in tBats
                                    group b by b.PitcherId into g
                                    select
                                    g.Count(x => x.PResult == PResultType.Hit) * 1.0
                                     / (g.Count())
                                        ).First();

            // 좌타자 안타율
            var lBats = from b in tBats
                        join p in players
                        on b.HitterId equals p.PlayerId
                        where b.PitcherId == playerId && p.Hand.Contains("좌타")
                        select b;
            pitcherInfo.HitRatioLHand = (from b in lBats
                                         group b by b.PitcherId into g
                                         select
                                         g.Count(x => x.PResult == PResultType.Hit) * 1.0
                                          / (g.Count())
                                        ).First();

            // 우타자 안타율
            var rBats = from b in tBats
                        join p in players
                        on b.HitterId equals p.PlayerId
                        where b.PitcherId == playerId && p.Hand.Contains("우타")
                        select b;
            pitcherInfo.HitRatioRHand = (from b in rBats
                                         group b by b.PitcherId into g
                                         select
                                         g.Count(x => x.PResult == PResultType.Hit) * 1.0
                                          / (g.Count())
                                        ).First();

            return pitcherInfo;
        }


        // 팀별로 최근 N경기에 모두 선발 출장인 타자를 가지고 온다.
        public List<Player> GetStartPlayerForNDays(String teamInitial, Int32 days)
        {
            Int32 dateNumber = MaxDateTime.Year * 10000 + MaxDateTime.Month * 100 + MaxDateTime.Day;

            // 입력된 팀의 최근 n경기를 가지고 온다.
            var tMatches = (from m in matches
                            join s in schedules
                            on m.GameId equals s.GameId
                            where (s.HomeTeam == teamInitial || s.AwayTeam == teamInitial)
                            &&
                                s.Year * 10000 + s.Month * 100 + s.Day <
                                dateNumber
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
            Int32 dateNumber = MaxDateTime.Year * 10000 + MaxDateTime.Month * 100 + MaxDateTime.Day;

            // 입력된 팀의 최근 n경기를 가지고 온다.
            var tMatches = (from m in matches
                            join s in schedules
                            on m.GameId equals s.GameId
                            where (s.HomeTeam == teamInitial || s.AwayTeam == teamInitial)
                            &&
                                s.Year * 10000 + s.Month * 100 + s.Day <
                                dateNumber
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
            Int32 dateNumber = MaxDateTime.Year * 10000 + MaxDateTime.Month * 100 + MaxDateTime.Day;

            // 입력된 팀의 최근 n경기를 가지고 온다.
            var tMatches = (from m in matches
                            join s in schedules
                            on m.GameId equals s.GameId
                            where (s.HomeTeam == teamInitial || s.AwayTeam == teamInitial)
                            &&
                                s.Year * 10000 + s.Month * 100 + s.Day <
                                dateNumber
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

        // 주어진 타자의 연속 안타수를 구한다.
        public Int32 GetHitDay(String teamInitial, Int32 playerId)
        {
            Int32 dateNumber = MaxDateTime.Year * 10000 + MaxDateTime.Month * 100 + MaxDateTime.Day;

            // 입력된 팀의 최근 n경기를 가지고 온다.
            var tMatches = (from m in matches
                            join s in schedules
                            on m.GameId equals s.GameId
                            where (s.HomeTeam == teamInitial || s.AwayTeam == teamInitial)
                            &&
                                s.Year * 10000 + s.Month * 100 + s.Day <
                                dateNumber
                            orderby m.GameId descending
                            select m).Take(20).ToList();

            // 연속안타수를 구한다.
            Int32 count = 0;
            foreach(var match in tMatches)
            {
                var c = (from t in ths
                             join b in bats
                                 on t.Id equals b.ThId
                             where t.MatchId == match.Id && b.PResult == PResultType.Hit && b.HitterId == playerId
                             select b).Count();
                if(c > 0)
                {
                    count++;
                }
                else
                {
                    return count;
                }
            }
            return count;
        }

        // 주어진 타자의 최근 N게임에서 항상 빠른타순이었는지를 구한다.
        public Boolean IsAllFastNumber(String teamInitial, Int32 playerId, Int32 days)
        {
            Int32 dateNumber = MaxDateTime.Year * 10000 + MaxDateTime.Month * 100 + MaxDateTime.Day;

            // 입력된 팀의 최근 n경기를 가지고 온다.
            var tMatches = (from m in matches
                            join s in schedules
                            on m.GameId equals s.GameId
                            where (s.HomeTeam == teamInitial || s.AwayTeam == teamInitial)
                            &&
                                s.Year * 10000 + s.Month * 100 + s.Day <
                                dateNumber
                            orderby m.GameId descending
                            select m).Take(days);

            // 모든경기에 스타팅 멤버로 출전한 타자를 가지고 온다.
            var count = (from m in tMatches
                            join l in lineUps
                            on m.Id equals l.MatchId
                            where l.PlayerId == playerId && l.BatNumber <= 6
                            select m).Count();
            return count == days;
        }

        // 주어진 타자의 최근 N게임에서의 타율
        public Double GetHitRatio(String teamInitial, Int32 playerId, Int32 days)
        {
            Int32 dateNumber = MaxDateTime.Year * 10000 + MaxDateTime.Month * 100 + MaxDateTime.Day;

            // 입력된 팀의 최근 n경기를 가지고 온다.
            var tMatches = (from m in matches
                            join s in schedules
                            on m.GameId equals s.GameId
                            where (s.HomeTeam == teamInitial || s.AwayTeam == teamInitial)
                            &&
                                s.Year * 10000 + s.Month * 100 + s.Day <
                                dateNumber
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
        public Boolean IsLongHitLastGame(String teamInitial, Int32 playerId, Int32 gameCout)
        {
            Int32 dateNumber = MaxDateTime.Year * 10000 + MaxDateTime.Month * 100 + MaxDateTime.Day;
            // 입력된 팀의 최근 n경기를 가지고 온다.
            var tMatches = (from m in matches
                            join s in schedules
                            on m.GameId equals s.GameId
                            where (s.HomeTeam == teamInitial || s.AwayTeam == teamInitial)
                            &&
                                s.Year * 10000 + s.Month * 100 + s.Day <
                                dateNumber
                            orderby m.GameId descending
                            select m).Take(gameCout);

            // 모든경기에 안타가 있는지 확인
            var tBats = (from m in tMatches
                         join t in ths
                             on m.Id equals t.MatchId
                         join b in bats
                             on t.Id equals b.ThId
                         where b.HitterId == playerId
                         select b);

            foreach (var b in tBats)
            {
                var result = b.Result.Trim();
                if (result.EndsWith("2") || result.EndsWith("3") || result.EndsWith("홈"))
                {
                    return true;
                }
            }

            return false;
        }

        // 상대전적을 분석한다.
        public Double? GetAgainstHitRatio(Int32 pitcherId, Int32 hitterId)
        {
            Int32 count = 0;
            try
            {
                var vsCount = (from m in matches
                               join t in ths
                               on m.Id equals t.MatchId
                               join b in bats
                               on t.Id equals b.ThId
                               join p in players
                               on b.PitcherId equals p.PlayerId
                               where b.HitterId == hitterId
                                && b.PitcherId == pitcherId && b.PResult != PResultType.Pass
                               group b by new { b.HitterId } into g
                               select g.Count()).First();
                count = vsCount;

                if (count < 4)
                {
                    return null;
                }

                var vsRatio = (from m in matches
                               join t in ths
                               on m.Id equals t.MatchId
                               join b in bats
                               on t.Id equals b.ThId
                               join p in players
                               on b.PitcherId equals p.PlayerId
                               where b.HitterId == hitterId
                                && b.PitcherId == pitcherId && b.PResult != PResultType.Pass
                               group b by new { b.HitterId } into g
                               select g.Count(x => x.PResult == PResultType.Hit) * 1.0 / g.Count()).First();
                return vsRatio;
            }
            catch
            {
                return null; 
            }
        }

        public String GetPlayerName(Int32 playerId)
        {
            try
            {
                var name = from player in players
                           where player.PlayerId == playerId
                           select player;
                return name.First().Name;
            }
            catch { return ""; }
        }

        public String GetPlayerHand(Int32 playerId)
        {
            try
            {
                var name = from player in players
                           where player.PlayerId == playerId
                           select player;
                return name.First().Hand;
            }
            catch { return ""; }
        }
    }
}
