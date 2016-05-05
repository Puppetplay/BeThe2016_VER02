//
// Schedule_W 
//

using System;
using System.Data.Linq.Mapping;

namespace BeThe.Item
{
    [Table]
    public class Schedule_W : DbItemBase
    {
        // PRIMARY KEY	IDENTITY
        [Column(AutoSync = AutoSync.OnInsert, IsDbGenerated = true, IsPrimaryKey = true, CanBeNull = false)]
        public override Int64 Id { get; set; }

        [Column(CanBeNull = false)]
        public Int32 Year { get; set; }

        [Column(CanBeNull = false)]
        public Int32 Month { get; set; }

        [Column(CanBeNull = false)]
        public Int32 Day { get; set; }

        [Column(CanBeNull = true)]
        public String Time { get; set; }

        [Column(CanBeNull = true)]
        public String Play { get; set; }

        [Column(CanBeNull = true)]
        public String Relay { get; set; }

        [Column(CanBeNull = true)]
        public String BallPark { get; set; }

        [Column(CanBeNull = true)]
        public String Etc { get; set; }
    }
}
