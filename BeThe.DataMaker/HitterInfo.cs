using System;
using System.Collections.Generic;

namespace BeThe.DataMaker
{
    internal class HitterInfo
    {
        public Int32 PlayerId { get; set; }
        public List<HitterResult> HitterResults { get; set; }
    }

    internal class HitterResult
    {
        public Int32 Number { get; set; }
        public String Result { get; set; }
    }
}
