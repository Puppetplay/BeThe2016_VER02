//
// Ball
//

using System;
using System.Data.Linq.Mapping;

namespace BeThe.Item
{
    [Table]
    public class Ball : DbItemBase
    {
        // PRIMARY KEY	IDENTITY
        [Column(AutoSync = AutoSync.OnInsert, IsDbGenerated = true, IsPrimaryKey = true, CanBeNull = false)]
        public override Int64 Id { get; set; }

        // Bat Id
        [Column(CanBeNull = false)]
        public Int64 BatId { get; set; }

        [Column(CanBeNull = false)]
        public Int32? Number { get; set; }

        [Column(CanBeNull = true)]
        public Int32? Speed { get; set; }

        [Column(CanBeNull = false)]
        public String BallType { get; set; }

        [Column(CanBeNull = false)]
        public String Result { get; set; }

        [Column(CanBeNull = false)]
        public String BallCount { get; set; }
    }
}
