//
// BoxScore_W
//

using System;
using System.Data.Linq.Mapping;

namespace BeThe.Item
{
    [Table]
    public class BoxScore_W : DbItemBase
    {
        // PRIMARY KEY	IDENTITY
        [Column(AutoSync = AutoSync.OnInsert, IsDbGenerated = true, IsPrimaryKey = true, CanBeNull = false)]
        public override Int64 Id { get; set; }

        [Column(CanBeNull = false)]
        public String GameId { get; set; }

        [Column(CanBeNull = false)]
        public String AwayHitter { get; set; }

        [Column(CanBeNull = false)]
        public String HomeHitter { get; set; }

        [Column(CanBeNull = false)]
        public String AwayPitcher { get; set; }

        [Column(CanBeNull = false)]
        public String HomePitcher { get; set; }
    }
}
