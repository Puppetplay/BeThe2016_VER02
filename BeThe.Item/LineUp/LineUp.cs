//
// LineUp
//

using System;
using System.Data.Linq.Mapping;

namespace BeThe.Item
{
    [Table]
    public class LineUp : DbItemBase
    {
        // PRIMARY KEY	IDENTITY
        [Column(AutoSync = AutoSync.OnInsert, IsDbGenerated = true, IsPrimaryKey = true, CanBeNull = false)]
        public override Int64 Id { get; set; }

        // MathId
        [Column(CanBeNull = false)]
        public Int64 MatchId { get; set; }

        [Column(CanBeNull = false)]
        public AttackType AttackType { get; set; }

        [Column(CanBeNull = false)]
        public Int32 PlayerId { get; set; }

        [Column(CanBeNull = false)]
        public Int32 BatNumber { get; set; }

        [Column(CanBeNull = false)]
        public EntryType EntryType { get; set; }
    }

    public enum AttackType
    {
        Home,
        Away
    }

    public enum EntryType
    {
        Starting,
        Change
    }
}
