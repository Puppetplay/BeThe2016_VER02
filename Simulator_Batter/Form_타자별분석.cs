using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BeThe.Item;

namespace Simulator_Batter
{
    public partial class Form_타자별분석 : Form
    {
        public Form_타자별분석()
        {
            InitializeComponent();
        }

        private void btAnalyze_Click(object sender, EventArgs e)
        {
            BeThe.Util.DatabaseManager dbMgr = new BeThe.Util.DatabaseManager();

            var aSchedules = dbMgr.SelectAll<Schedule>();
            var aMatches = dbMgr.SelectAll<Match>();
            var aThs = dbMgr.SelectAll<Th>();
            var aBats = dbMgr.SelectAll<Bat>();
            var aBalls = dbMgr.SelectAll<Ball>();
            var aPlayers = dbMgr.SelectAll<Player>();

            var name = tbHitterName.Text;
            // 타자 아이디 구하기
            var hitterId = (from p in aPlayers
                            where p.Name == tbHitterName.Text
                            select p.PlayerId).First();

            //var hitterId = Convert.ToInt32(tbHitterName.Text);
            //name = (from p in aPlayers
            //        where p.PlayerId == hitterId
            //        select p.Name).First();

            // 팀구하기
            var team = (from p in aPlayers
                        where p.PlayerId == hitterId
                        select p.Team).First();

            // 팀구하기
            var hand = (from p in aPlayers
                        where p.PlayerId == hitterId
                        select p.Hand).First();

            var fSchedules = from s in aSchedules
                             where s.Year == 2016 &&
                             (s.HomeTeam == team || s.AwayTeam == team)
                             select s;

            // 분석 정보 얻기
            var query =  from s in fSchedules
                         join m in aMatches
                         on s.GameId equals m.GameId
                         join th in aThs
                         on m.Id equals th.MatchId
                         join bat in aBats
                         on th.Id equals bat.ThId
                         join p in aPlayers
                         on bat.PitcherId equals p.PlayerId
                         join ball in aBalls
                         on bat.Id equals ball.BatId
                         where bat.HitterId == hitterId
                         orderby bat.Id
                         select
                         new HitterInfo
                         {
                             Year = 2016,
                             Month = s.Month,
                             Day = s.Day,
                             PitcherId = bat.PitcherId,
                             PitcherName = p.Name,
                             PitcherHand = p.Hand,
                             PResult = bat.PResult,
                             BallPark = s.BallPark,
                             DetailResult = bat.DetailResult,
                             Hour = s.Hour,
                             BallCount = ball.BallCount,
                             BallType = ball.BallType,
                             Result = ball.Result,
                             Speed = ball.Speed,
                         };

            dataGridView1.DataSource = query;

            // 각날짜별 안타수 구하기

            // 분석 정보 얻기
            var query2 = from s in fSchedules
                         join m in aMatches
                         on s.GameId equals m.GameId
                         join th in aThs
                         on m.Id equals th.MatchId
                         join bat in aBats
                         on th.Id equals bat.ThId
                         join p in aPlayers
                         on bat.PitcherId equals p.PlayerId
                         where bat.HitterId == hitterId
                         group new { s, bat } by new { s.Year, s.Month, s.Day } into g
                         select new HitInfo
                         {
                             Month = g.Key.Month,
                             Day = g.Key.Day,
                             HitCount = g.Count(x => x.bat.PResult == PResultType.Hit)
                         };                    

            dataGridView2.DataSource = query2;


            // 분석 정보 얻기
            var scheduleIds = from s in fSchedules
                            join m in aMatches
                            on s.GameId equals m.GameId
                            join th in aThs
                            on m.Id equals th.MatchId
                            join bat in aBats
                            on th.Id equals bat.ThId
                            where bat.HitterId == hitterId
                            group new { s, bat } by new { s.Id } into g
                            where g.Count(x => x.bat.PResult == PResultType.Hit) == 0
                            select g.Key.Id;

            var query3 = from s in fSchedules
                         join si in scheduleIds
                         on s.Id equals si
                         join m in aMatches
                         on s.GameId equals m.GameId
                         join th in aThs
                         on m.Id equals th.MatchId
                         join bat in aBats
                         on th.Id equals bat.ThId
                         join p in aPlayers
                         on bat.PitcherId equals p.PlayerId
                         where th.Number == 1 && p.Team != team
                         group new { s, p } by new { s, p } into g
                         orderby g.Key.s.Year, g.Key.s.Month, g.Key.s.Day
                         select new HitInfo
                         {
                             Name = tbHitterName.Text,
                             Hand = hand,
                             Month = g.Key.s.Month,
                             Day = g.Key.s.Day,
                             BallPark = g.Key.s.BallPark,
                             Hour = g.Key.s.Hour.Value,
                             HitCount = 0,
                             PitcherHand = g.Key.p.Hand,
                             PitcherName = g.Key.p.Name,
                             PitcherTeam = g.Key.p.Team,
                         };

            dataGridView3.DataSource = query3;

        }
    }
}
