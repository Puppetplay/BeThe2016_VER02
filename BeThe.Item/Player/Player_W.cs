//
// 팀별로 선수링크를 가지고 있는 데이터
//

using System;
using System.Data.Linq.Mapping;

namespace BeThe.Item
{
    [Table]
    public class Player_W : DbItemBase
    {
        // PRIMARY KEY	IDENTITY
        [Column(AutoSync = AutoSync.OnInsert, IsDbGenerated = true, IsPrimaryKey = true, CanBeNull = false)]
        public override Int64 Id { get; set; }

        [Column(CanBeNull = false)]
        public String Team { get; set; }

        [Column(CanBeNull = false)]
        public String Href { get; set; }
    }
}
