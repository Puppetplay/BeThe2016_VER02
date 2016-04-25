//
// 선수
//

using System;
using System.Data.Linq.Mapping;

namespace BeThe.Item
{
    [Table]
    public class Player : DbItemBase
    {
        // PRIMARY KEY	IDENTITY
        [Column(AutoSync = AutoSync.OnInsert, IsDbGenerated = true, IsPrimaryKey = true, CanBeNull = false)]
        public override Int64 Id { get; set; }

        // 팀
        [Column(CanBeNull = false)]
        public String Team { get; set; }

        // 선수ID
        [Column(CanBeNull = false)]
        public Int32 PlayerId { get; set; }

        // 사진링크주소
        [Column(CanBeNull = true)]
        public String SCR { get; set; }

        // 등번호
        [Column(CanBeNull = true)]
        public Int32? BackNumber { get; set; }

        // 이름
        [Column(CanBeNull = false)]
        public String Name { get; set; }

        // 포지션
        [Column(CanBeNull = false)]
        public String Position { get; set; }

        // 투타타입
        [Column(CanBeNull = false)]
        public String Hand { get; set; }

        // 생년월일
        [Column(CanBeNull = false)]
        public String BirthDate { get; set; }

        // 키
        [Column(CanBeNull = false)]
        public Int32 Height { get; set; }

        // 몸무게
        [Column(CanBeNull = false)]
        public Int32 Weight { get; set; }

        // 경력
        [Column(CanBeNull = true)]
        public String Career { get; set; }

        // 계약금
        [Column(CanBeNull = true)]
        public String Deposit { get; set; }

        // 연봉
        [Column(CanBeNull = true)]
        public String Salary { get; set; }

        // 지명순위
        [Column(CanBeNull = true)]
        public String Rank { get; set; }

        // 입단연도
        [Column(CanBeNull = true)]
        public String JoinYear { get; set; }
    }
}
