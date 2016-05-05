//
// Schedule
//

using System;
using System.Data.Linq.Mapping;

namespace BeThe.Item
{
    [Table]
    public class Schedule : DbItemBase
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
        public Int32? Hour { get; set; }

        [Column(CanBeNull = true)]
        public Int32? Minute { get; set; }

        [Column(CanBeNull = true)]
        public String HomeTeam { get; set; }

        [Column(CanBeNull = true)]
        public String AwayTeam { get; set; }

        [Column(CanBeNull = true)]
        public Int32? HomeTeamScore { get; set; }

        [Column(CanBeNull = true)]
        public Int32? AwayTeamScore { get; set; }

        [Column(CanBeNull = true)]
        public String Href { get; set; }

        [Column(CanBeNull = true)]
        public String GameId { get; set; }

        [Column(CanBeNull = true)]
        public Int32? LeagueId { get; set; }

        [Column(CanBeNull = true)]
        public Int32? SeriesId { get; set; }

        [Column(CanBeNull = true)]
        public String BallPark { get; set; }

        [Column(CanBeNull = true)]
        public String Etc { get; set; }
    }
}
