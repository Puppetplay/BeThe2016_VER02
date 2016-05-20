//
// 투수에 대한 분석결과
//

using System;

namespace BeThe.DataAnalyzer
{
    public class PitcherInfo
    {
        // 투수의 플레이어 아이디
        public Int32 PlayerId { get; set; }

        public String PlayerName { get; set; }

        public String Hand { get; set; }

        // 상대타자의 출루율
        public Double OnBaseRatio { get; set; }

        // 안타확률
        public Double HitRatio { get; set; }

        // 상대가 좌타자일때 안타확률
        public Double HitRatioLHand { get; set; }

        // 상대가 우타자일때 안타확률
        public Double HitRatioRHand { get; set; }
    }
}
