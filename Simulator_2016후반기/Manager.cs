using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeThe.DataAnalyzer;
using BeThe.Item;
using BeThe.Util;

namespace Simulator_2016후반기
{
    class Manager
    {
        public List<BatterInfo> Pick(DateTime datetime)
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
                           where m.GameId.StartsWith(datetime.ToString("yyyyMMdd"))
                           select m;

            // 해당일의 경기별로 돌면서 작업
            List<BatterInfo> pickPlayers = new List<BatterInfo>();
            foreach (var match in tMatches)
            {
                // 홈팀가져오기 
                String teamInitial = match.GameId.Substring(10, 2);
                var pPlayers = GetAnalyzerData(match, AttackType.Home, teamInitial, datetime);
                pickPlayers = pickPlayers.Concat(pPlayers).ToList();

                // 어웨이팀 가져오기
                teamInitial = match.GameId.Substring(8, 2);
                pPlayers = GetAnalyzerData(match, AttackType.Away, teamInitial, datetime);
                pickPlayers = pickPlayers.Concat(pPlayers).ToList();
            }

            // 선택된 타자를 타율순으로 정렬한다.
            Sort(pickPlayers);

            return pickPlayers;
        }

        private void Sort(List<BatterInfo> batterInfos)
        {
            batterInfos.Sort(delegate (BatterInfo x, BatterInfo y)
            {
                return y.HitRatio.CompareTo(x.HitRatio);
            });
        }

        // 해당일에 경기가 있는지 확인
        public Boolean IsHaveGame(DateTime dateTime)
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
                           where m.GameId.StartsWith(dateTime.ToString("yyyyMMdd"))
                           select m;
            return tMatches.Count() > 0;
        }

        //  한경기의 한쪽 팀의 분석정보를 가져온다.
        private List<BatterInfo> GetAnalyzerData(Match match, AttackType attckType,
            String teamInitial, DateTime dateTime)
        {
            // 경기를 가져온다.
            DatabaseManager dbMgr = new DatabaseManager();
            var players = dbMgr.SelectAll<Player>();
            var matches = dbMgr.SelectAll<Match>();
            var schedules = dbMgr.SelectAll<Schedule>();
            var ths = dbMgr.SelectAll<Th>();
            var bats = dbMgr.SelectAll<Bat>();

            ThType tyType = attckType == AttackType.Home ? ThType.말 : ThType.초;

            // 지난 데이터의 분석기 생성
            BeThe.DataAnalyzer.Manager analyzer = new BeThe.DataAnalyzer.Manager();
            analyzer.MaxDateTime = dateTime;

            var batterInfos = new List<BatterInfo>();
            Int32 pitcherId;

            // 당일 선발투수 가져오기(분석시 입력 혹은 크롤링 구현)
            try
            {
                pitcherId = (from t in ths
                             join b in bats
                             on t.Id equals b.ThId
                             where t.MatchId == match.Id
                             && t.Number == 1 && t.ThType == tyType
                             select b).First().PitcherId;



                //// 투수 정보 가져오기
                //Boolean isQualityStart = analyzer.IsQualityStart(pitcherId);
                //if (isQualityStart)
                //{
                //    return batterInfos;
                //}
            }
            catch
            {
                return batterInfos;
            }

            // N경기동안 M번 선발출장한 선수들 가져오기
            var hitters = analyzer.GetPlayersStartingGame(teamInitial, 5, 4);

            // 타자조건 실행하기
            foreach (var hitter in hitters)
            {
                try
                {
                    // 타석수 구하기
                    var pa = analyzer.GetPA(teamInitial, hitter.PlayerId);

                    // 안타수 구하기
                    var hitCount = analyzer.GetHitCout(teamInitial, hitter.PlayerId);

                    // 안타확률 구하기
                    var hitRatio = (hitCount * 1.0) / pa;

                    // 팀의 지난 M경기동안 안타를 친날 구하기
                    var hitDayCount = analyzer.GetHitDayCountForNDays(teamInitial, hitter.PlayerId, 6);

                    var batterInfo = new BatterInfo();
                    if (hitRatio > 0.2 && hitDayCount >= 5)
                    {
                        batterInfo.PlayerId = hitter.PlayerId;
                        batterInfo.Name = hitter.Name;
                        batterInfo.PA = pa;
                        batterInfo.HitCount = hitCount;
                        batterInfo.HitRatio = hitRatio;
                        batterInfo.HitDayCount = hitDayCount;
                        batterInfos.Add(batterInfo);
                    }
                }
                catch
                {

                }
            }
            return batterInfos;

        }

        // 선택된 데이터의 결과 확인하기
        public void GetPickResult(DateTime dateTime, List<BatterInfo> hitterInfos)
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
                           where m.GameId.StartsWith(dateTime.ToString("yyyyMMdd"))
                           select m;

            foreach (var hitterInfo in hitterInfos)
            {
                var batts = from m in tMatches
                            join t in ths
                            on m.Id equals t.MatchId
                            join b in bats
                            on t.Id equals b.ThId
                            where
                            b.PResult == PResultType.Hit &&
                            b.HitterId == hitterInfo.PlayerId
                            select b;

                var count = (from m in tMatches
                             join t in ths
                             on m.Id equals t.MatchId
                             join b in bats
                             on t.Id equals b.ThId
                             where b.PResult == PResultType.Hit
                             && b.HitterId == hitterInfo.PlayerId
                             select b).Count();
                hitterInfo.RHitCount = count;
            }
        }
    }
}
