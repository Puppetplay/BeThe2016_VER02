//
// Th회
//

using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;

namespace BeThe.Item
{
    [Table]
    public class Th : DbItemBase
    {
        // PRIMARY KEY	IDENTITY
        [Column(AutoSync = AutoSync.OnInsert, IsDbGenerated = true, IsPrimaryKey = true, CanBeNull = false)]
        public override Int64 Id { get; set; }

        // MathId
        [Column(CanBeNull = false)]
        public Int64 MatchId { get; set; }

        [Column(CanBeNull = false)]
        public ThType ThType { get; set; }

        [Column(CanBeNull = false)]
        public Int32 Number { get; set; }

        public List<Bat> Bats { get; set; }
    }

    public enum ThType
    {
        초,
        말,
    }
}
