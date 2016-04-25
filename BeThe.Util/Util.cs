//
// 공통 유틸리티
//
using System;

namespace BeThe.Util
{
    public class Util
    {
        public static String GetTeamInitialFromName(String teamName)
        {
            switch (teamName.ToUpper())
            {
                case "삼성": return "SS";
                case "NC": return "NC";
                case "두산": return "OB";
                case "넥센": return "WO";
                case "한화": return "HH";
                case "KIA": return "HT";
                case "SK": return "SK";
                case "롯데": return "LT";
                case "LG": return "LG";
                case "KT": return "KT";
                default: return teamName;
            }
        }

        public static String[] Teams =
            {   "삼성", "NC", "두산","넥센","한화",
                "KIA","SK", "롯데","LG","KT"
        };
    }
}
