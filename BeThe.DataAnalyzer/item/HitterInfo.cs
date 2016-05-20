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
}
