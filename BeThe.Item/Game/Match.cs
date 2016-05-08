//
// Match
//

using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;

namespace BeThe.Item
{
    [Table]
    public class Match : DbItemBase
    {
        // PRIMARY KEY	IDENTITY
        [Column(AutoSync = AutoSync.OnInsert, IsDbGenerated = true, IsPrimaryKey = true, CanBeNull = false)]
        public override Int64 Id { get; set; }

        // GameId
        [Column(CanBeNull = false)]
        public String GameId { get; set; }

        public List<Th> Ths { get; set; }
    }
}
