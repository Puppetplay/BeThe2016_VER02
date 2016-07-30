using BeThe.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator_Batter
{
    public class HitterInfo
    {
        public Int32 Year { get; set; }

        public Int32 Month { get; set; }

        public Int32 Day { get; set; }

        // 투수 ID
        public Int32 PitcherId { get; set; }

        // 투수이름
        public String PitcherName { get; set; }

        // 투수 Hand
        public String PitcherHand { get; set; }

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
