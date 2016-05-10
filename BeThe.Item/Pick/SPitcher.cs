//
// 선발투수
//

using System;
using System.Data.Linq.Mapping;

namespace BeThe.Item
{
    [Table]
    public class SPitcher : DbItemBase
    {
        // PRIMARY KEY	IDENTITY
        [Column(AutoSync = AutoSync.OnInsert, IsDbGenerated = true, IsPrimaryKey = true, CanBeNull = false)]
        public override Int64 Id { get; set; }

        // 팀
        [Column(CanBeNull = false)]
        public String Team { get; set; }

        [Column(CanBeNull = false)]
        public Int32 Year { get; set; }

        [Column(CanBeNull = false)]
        public Int32 Month { get; set; }

        [Column(CanBeNull = false)]
        public Int32 Day { get; set; }

        // 선수ID
        [Column(CanBeNull = false)]
        public Int32 PlayerId { get; set; }
    }
}
