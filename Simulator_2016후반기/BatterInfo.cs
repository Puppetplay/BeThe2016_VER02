using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator_2016후반기
{
    class BatterInfo
    {
        public Int32 PlayerId { get; set; }
        public String Name { get; set; }
        public Double HitRatio { get; set; }

        // 타석수
        public Int32 PA { get; set; }
        public Int32 HitCount { get; set; }
        // 최근M경기동안 안타친 날의 수
        public Int32 HitDayCount { get; set; }
        public Int32 RHitCount { get; set; }
    }
}
