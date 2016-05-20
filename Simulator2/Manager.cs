using BeThe.DataAnalyzer;
using BeThe.Item;
using BeThe.Util;
using Simulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator2
{
    class Manager
    {
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

        public List<HitterInfo> Pick(DateTime datetime)
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


            List<AnalyzerData> analyzerDatas = new List<AnalyzerData>();

            // 해당일의 경기별로 돌면서 작업
            foreach (var match in tMatches)
            {
                // 홈팀가져오기 
                String teamInitial = match.GameId.Substring(10, 2);
                var analyzerData = GetAnalyzerData(match, AttackType.Home, teamInitial, datetime);
                analyzerDatas.Add(analyzerData);

                // 어웨이팀 가져오기
                teamInitial = match.GameId.Substring(8, 2);
                analyzerData = GetAnalyzerData(match, AttackType.Away, teamInitial, datetime);
                analyzerDatas.Add(analyzerData);
            }

            // 지난 데이터의 분석기 생성
            BeThe.DataAnalyzer.Manager analyzer = new BeThe.DataAnalyzer.Manager();
            analyzer.MaxDateTime = datetime;

            // 상대전분석
            foreach (var analyzerData in analyzerDatas)
            {
                foreach (var hitData in analyzerData.HitterInfos)
                {
                    hitData.AgainstRatio =
                    analyzer.GetAgainstHitRatio(analyzerData.PitcherInfo.PlayerId, hitData.PlayerId);
                }
            }

            // 픽을 결정한다.
            List<HitterInfo> hitInfosLevel1 = GetHitInfos(1, analyzerDatas);
            List<HitterInfo> hitInfosLevel2 = GetHitInfos(2, analyzerDatas);
            List<HitterInfo> hitInfosLevel3 = GetHitInfos(3, analyzerDatas);

            Int32 maxCount = 5;
            Dictionary<Int32, HitterInfo> dicPicks = new Dictionary<Int32, HitterInfo>();

            Sort(hitInfosLevel1);
            Sort(hitInfosLevel2);
            Sort(hitInfosLevel3);

            foreach (var hitterInfo in hitInfosLevel1)
            {
                if (dicPicks.Count >= maxCount)
                {
                    break;
                }
                if (dicPicks.ContainsKey(hitterInfo.PlayerId))
                {
                    continue;
                }
                else
                {
                    dicPicks.Add(hitterInfo.PlayerId, hitterInfo);
                }
            }
            if (dicPicks.Count > 2)
            {
                return dicPicks.Values.ToList();
            }

            foreach (var hitterInfo in hitInfosLevel2)
            {
                if (dicPicks.Count >= maxCount)
                {
                    break;
                }
                if (dicPicks.ContainsKey(hitterInfo.PlayerId))
                {
                    continue;
                }
                else
                {
                    dicPicks.Add(hitterInfo.PlayerId, hitterInfo);
                }
            }
            if (dicPicks.Count > 2)
            {
                return dicPicks.Values.ToList();
            }

            foreach (var hitterInfo in hitInfosLevel3)
            {
                if (dicPicks.Count >= maxCount)
                {
                    break;
                }
                if (dicPicks.ContainsKey(hitterInfo.PlayerId))
                {
                    continue;
                }
                else
                {
                    dicPicks.Add(hitterInfo.PlayerId, hitterInfo);
                }
            }

            return dicPicks.Values.ToList();
        }

        // 선택된 데이터의 결과 확인하기
        public void GetPickResult(DateTime dateTime, List<HitterInfo> hitterInfos)
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
                hitterInfo.ResultHitCount = count;
            }
        }


        private void Sort(List<HitterInfo> hitterInfos)
        {
            hitterInfos.Sort(delegate (HitterInfo x, HitterInfo y)
            {
                return y.HitRatio.CompareTo(x.HitRatio);
            });
        }

        private List<HitterInfo> GetHitInfos(Int32 level, List<AnalyzerData> analyzerDatas)
        {
            Double pOnBaseRatio = 1.0;
            Double pHitRatio = 1.0;
            Double hitRatio = 0.0;
            // 픽을 결정한다.
            if (level == 1)
            {
                pOnBaseRatio = 0.30;
                pHitRatio = 0.28;
                hitRatio = 0.4;
            }
            else if (level == 2)
            {
                pOnBaseRatio = 0.28;
                pHitRatio = 0.27;
                hitRatio = 0.35;
            }
            else if (level == 3)
            {
                pOnBaseRatio = 0.28;
                pHitRatio = 0.27;
                hitRatio = 0.33;
            }

            List<HitterInfo> hitInfos = new List<HitterInfo>();
            foreach (var analyzerData in analyzerDatas)
            {
                // 투수
                var pitcherInfo = analyzerData.PitcherInfo;

                // 1. 출루율이 낮은 에이스 투수는 피한다.
                if (pitcherInfo.OnBaseRatio < pOnBaseRatio)
                {
                    continue;
                }

                // 2. 피안타율이 낮은 에이스 투수는 피한다.
                if (pitcherInfo.HitRatio < pHitRatio)
                {
                    continue;
                }

                // 타자
                foreach (var hitData in analyzerData.HitterInfos)
                {
                    if (hitData.Level != 0)
                    {
                        continue;
                    }

                    if (hitData.IsAllDayFirstNumber == false)
                    {
                        continue;
                    }

                    // 1. 연속안타중이 아니면 선택하지 않는다.
                    if (level == 1)
                    {
                        if (hitData.IsAllDayHit5 == false)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (hitData.IsAllDayHit3 == false)
                        {
                            continue;
                        }
                    }

                    // 2. 최근N에 안타수가 M개이하면 선택하지 않는다.
                    if (hitData.HitCount <= 4)
                    {
                        continue;
                    }

                    // 3. 최근경기의 타율이 4할이 넘지 않으면 선택하지 않는다.
                    if (hitData.HitRatio < hitRatio)
                    {
                        continue;
                    }

                    // 직전N경기에 2루타 이상이 있지 않으면 선택하지 않는다.
                    if (hitData.IsLongHit == false)
                    {
                        if (level != 3)
                        {
                            continue;
                        }
                    }

                    // 상대전적이 2할보다 낮으면 선택하지 않는다.
                    if (hitData.AgainstRatio != null && hitData.AgainstRatio <= 0.2)
                    {
                        continue;
                    }

                    // 픽을 추가한다.
                    hitData.Level = level;
                    hitInfos.Add(hitData);
                }
            }
            return hitInfos;
        }

        //  한경기의 한쪽 팀의 분석정보를 가져온다.
        private AnalyzerData GetAnalyzerData(Match match, AttackType attckType,
            String teamInitial, DateTime dateTime)
        {
            // 분석된 데이터 만들기
            AnalyzerData analyzerData = new AnalyzerData();
            analyzerData.HitterInfos = new List<HitterInfo>();

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

            // 당일 선발투수 가져오기(분석시 입력 혹은 크롤링 구현)
            var pitcherId = (from t in ths
                             join b in bats
                             on t.Id equals b.ThId
                             where t.MatchId == match.Id
                             && t.Number == 1 && t.ThType == tyType
                             select b).First().PitcherId;

            // 투수 정보 가져오기
            analyzerData.PitcherInfo = analyzer.GetPitcherInfo(pitcherId, 4);

            // 타자리스트 얻어오기
            var hitters = analyzer.GetStartPlayerForNDays(teamInitial, 4);
            foreach (var hitter in hitters)
            {
                // 각타자의 정보 얻어오기
                var HitInfo = GetHitterInfo(hitter.PlayerId, analyzer, teamInitial);
                analyzerData.HitterInfos.Add(HitInfo);
            }
            return analyzerData;
        }

        // 타자의 분석정보를 가져온다.
        private HitterInfo GetHitterInfo(Int32 playerId,
            BeThe.DataAnalyzer.Manager analyzer, String teamInitial)
        {
            Boolean isAllDayHit5 = analyzer.IsAllDayHit(teamInitial, playerId, 5);
            Boolean isAllDayHit3 = analyzer.IsAllDayHit(teamInitial, playerId, 4);
            Boolean isLongHit = analyzer.IsLongHitLastGame(teamInitial, playerId, 2);
            Boolean isAllDayFirstNumber = analyzer.IsAllFastNumber(teamInitial, playerId, 4);
            Int32 hitCount = analyzer.GetHitNumberForNDays(teamInitial, playerId, 3);
            Double hitRatio = analyzer.GetHitRatio(teamInitial, playerId, 3);
            String name = analyzer.GetPlayerName(playerId);

            return new HitterInfo
            {
                IsAllDayFirstNumber = isAllDayFirstNumber,
                PlayerId = playerId,
                PlayerName = name,
                IsAllDayHit3 = isAllDayHit3,
                IsAllDayHit5 = isAllDayHit5,
                IsLongHit = isLongHit,
                HitCount = hitCount,
                HitRatio = hitRatio,
            };
        }


        // 타자의 분석정보를 가져온다.
        private HitterInfoDetail GetDetailHitterInfo(Int32 playerId,
            BeThe.DataAnalyzer.Manager analyzer, String teamInitial)
        {
            // 연속안타일수를 구한다.
            Int32 hitDay = analyzer.GetHitDay(teamInitial, playerId);
            // HitterInfoDetail 클래스 다시 코딩

            Boolean isAllDayHit5 = analyzer.IsAllDayHit(teamInitial, playerId, 5);
            Boolean isAllDayHit3 = analyzer.IsAllDayHit(teamInitial, playerId, 4);
            Boolean isLongHit = analyzer.IsLongHitLastGame(teamInitial, playerId, 2);
            Boolean isAllDayFirstNumber = analyzer.IsAllFastNumber(teamInitial, playerId, 4);
            Int32 hitCount = analyzer.GetHitNumberForNDays(teamInitial, playerId, 3);
            Double hitRatio = analyzer.GetHitRatio(teamInitial, playerId, 3);
            String name = analyzer.GetPlayerName(playerId);

            return new HitterInfoDetail
            {
                IsAllDayFirstNumber = isAllDayFirstNumber,
                PlayerId = playerId,
                PlayerName = name,
                IsAllDayHit3 = isAllDayHit3,
                IsAllDayHit5 = isAllDayHit5,
                IsLongHit = isLongHit,
                HitCount = hitCount,
                HitRatio = hitRatio,
            };
        }

        public List<PitcherInfo> GetFirstPitchers(DateTime datetime)
        {
            List<PitcherInfo> pitInfos = new List<PitcherInfo>();

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
            foreach (var match in tMatches)
            {
                // 홈팀가져오기 
                String teamInitial = match.GameId.Substring(10, 2);
                var pitcherInfo = GetFirstPitchers(match, AttackType.Home, teamInitial, datetime);
                pitInfos.Add(pitcherInfo);
                pitcherInfo = GetFirstPitchers(match, AttackType.Away, teamInitial, datetime);
                pitInfos.Add(pitcherInfo);
            }

            return pitInfos;
        }

        // 투수정보를 가지고 온다.
        private PitcherInfo GetFirstPitchers(Match match, AttackType attckType,
            String teamInitial, DateTime dateTime)
        {
            // 경기를 가져온다.
            DatabaseManager dbMgr = new DatabaseManager();
            var players = dbMgr.SelectAll<Player>();
            var matches = dbMgr.SelectAll<Match>();
            var schedules = dbMgr.SelectAll<Schedule>();
            var ths = dbMgr.SelectAll<Th>();
            var bats = dbMgr.SelectAll<Bat>();

            // 지난 데이터의 분석기 생성
            BeThe.DataAnalyzer.Manager analyzer = new BeThe.DataAnalyzer.Manager();
            analyzer.MaxDateTime = dateTime;

            ThType tyType = attckType == AttackType.Home ? ThType.말 : ThType.초;

            var pitcherId = (from t in ths
                             join b in bats
                             on t.Id equals b.ThId
                             where t.MatchId == match.Id
                             && t.Number == 1 && t.ThType == tyType
                             select b).First().PitcherId;

            // 투수 정보 가져오기
            var pitcherInfo = analyzer.GetPitcherInfo(pitcherId, 4);

            return pitcherInfo;
        }

        // 해당일의 분석을 가져온다.
        public List<AnalyzerData> GetAnalyzeData(DateTime datetime)
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


            List<AnalyzerData> analyzerDatas = new List<AnalyzerData>();

            // 해당일의 경기별로 돌면서 작업
            foreach (var match in tMatches)
            {
                // 홈팀가져오기 
                String teamInitial = match.GameId.Substring(10, 2);
                var analyzerData = GetAnalyzerData(match, AttackType.Home, teamInitial, datetime);
                analyzerDatas.Add(analyzerData);

                // 어웨이팀 가져오기
                teamInitial = match.GameId.Substring(8, 2);
                analyzerData = GetAnalyzerData(match, AttackType.Away, teamInitial, datetime);
                analyzerDatas.Add(analyzerData);
            }

            // 지난 데이터의 분석기 생성
            BeThe.DataAnalyzer.Manager analyzer = new BeThe.DataAnalyzer.Manager();
            analyzer.MaxDateTime = datetime;

            // 상대전분석
            foreach (var analyzerData in analyzerDatas)
            {
                foreach (var hitData in analyzerData.HitterInfos)
                {
                    hitData.AgainstRatio =
                    analyzer.GetAgainstHitRatio(analyzerData.PitcherInfo.PlayerId, hitData.PlayerId);
                }
            }

            return analyzerDatas;
        }
    }
}
