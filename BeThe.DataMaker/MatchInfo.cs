using System;
using System.Collections.Generic;

namespace BeThe.DataMaker
{
    internal class MatchInfo
    {
        public List<PitcherInfo> HomeTeamPitcherInfos { get; set; }
        public List<PitcherInfo> AwayTeamPitcherInfos { get; set; }

        public List<HitterInfo> HomeTeamHitterInfos { get; set; }
        public List<HitterInfo> AwayTeamHitterInfos { get; set; }

        public Int32 HomeHitterCount { get; set; }
        public Int32 AwayHitterCount { get; set; }
    }
}
