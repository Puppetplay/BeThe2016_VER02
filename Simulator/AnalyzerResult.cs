using BeThe.DataAnalyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator
{
    public class AnalyzerData
    {
        public String TeamName { get; set; }
        public PitcherInfo PitcherInfo { get; set; }
        public List<HitterInfo> HitterInfos { get; set; }
    }
}
