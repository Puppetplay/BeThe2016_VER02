using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator_Batter
{
    public class HitInfo
    {
        public String Name { get; set; }

        public String Hand { get; set; }

        public Int32 Month { get; set; }

        public Int32 Day { get; set; }

        public Int32 HitCount { get; set; }

        public String PitcherTeam { get; set; }

        public String PitcherName { get; set; }

        public String PitcherHand { get; set; }

        public String BallPark { get; set; }

        public Int32 Hour { get; set; }
    }
}
