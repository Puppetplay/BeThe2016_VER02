using BeThe.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeThe.DataAnalyzer
{
    public class HitterInfo
    {
        public Int32 PlayerId { get; set; }
        public String PlayerName { get; set; }
        public Boolean IsAllDayHit3 { get; set; }
        public Boolean IsAllDayHit5 { get; set; }
        public Boolean IsLongHit { get; set; }
        public Boolean IsAllDayFirstNumber { get; set; }
        public Int32 HitCount { get; set; }
        public Double HitRatio { get; set; }
        public Double? AgainstRatio { get; set; }
        public Int32 Level { get; set; }

        public Int32 ResultHitCount { get; set; }
    }

    public class HitterInfoDetail
    {
        public Int32 PlayerId { get; set; }
        public String PlayerName { get; set; }
        public Boolean IsAllDayHit3 { get; set; }
        public Boolean IsAllDayHit5 { get; set; }
        public Boolean IsLongHit { get; set; }
        public Boolean IsAllDayFirstNumber { get; set; }
        public Int32 HitCount { get; set; }
        public Double HitRatio { get; set; }
        public Double? AgainstRatio { get; set; }
        public Int32 Level { get; set; }

        public Int32 ResultHitCount { get; set; }
    }

    public class HitterInfoNew
    {
        // 일자
        public String Date { get; set; }

        // 투수 ID
        public Int32 PitcherId { get; set; }

        // 투수이름
        public String PitcherName { get; set; }

        // 회
        public Int32 Th { get; set; }

        // 요약결과
        public PResultType PResult { get; set; }

        // 세부결과
        public String DetailResult { get; set; }

        // 결과
        public String Result { get; set; }

        // 구질
        public String BallType { get; set; }

        // 스피드
        public Int32? Speed { get; set; }

        // 볼카운트
        public String BallCount { get; set; }

        // 구장
        public String BallPark { get; set; }

        // 시간
        public Int32? Hour { get; set; }
    }
}
