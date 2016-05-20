using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Manager mgr = new Manager();
            //mgr.Pick(new DateTime(2016, 5, 14));

            // 자 여기서 5월 달 부터 시뮬레이션을 코딩하라.
            // 픽구해오기 4월 26일 부터하자
            DateTime dateTime = new DateTime(2016, 5, 1);
            DateTime endDateTime = new DateTime(2016, 5, 17);
            List<PickResult> pickResults = new List<PickResult>();
            while (dateTime < endDateTime)
            {
                if (mgr.IsHaveGame(dateTime))
                {
                    // 추천픽가져오기
                    var hitterInfos = mgr.Pick(dateTime);

                    // 추천픽 결과 검증하기
                    mgr.GetPickResult(dateTime, hitterInfos);

                    PickResult result = new PickResult();
                    if(hitterInfos.Count > 0)
                    {
                        result.GameDate = dateTime;
                        result.PlayerName1 = hitterInfos[0].PlayerName;
                        result.HitCount1 = hitterInfos[0].ResultHitCount;
                    }

                    if (hitterInfos.Count > 1)
                    {
                        result.GameDate = dateTime;
                        result.PlayerName2 = hitterInfos[1].PlayerName;
                        result.HitCount2 = hitterInfos[1].ResultHitCount;
                    }

                    if (hitterInfos.Count > 2)
                    {
                        result.GameDate = dateTime;
                        result.PlayerName3 = hitterInfos[2].PlayerName;
                        result.HitCount3 = hitterInfos[2].ResultHitCount;
                    }

                    if (hitterInfos.Count > 3)
                    {
                        result.GameDate = dateTime;
                        result.PlayerName4 = hitterInfos[3].PlayerName;
                        result.HitCount4 = hitterInfos[3].ResultHitCount;
                    }

                    if (hitterInfos.Count > 4)
                    {
                        result.GameDate = dateTime;
                        result.PlayerName5 = hitterInfos[4].PlayerName;
                        result.HitCount5 = hitterInfos[4].ResultHitCount;
                    }

                    if (hitterInfos.Count > 5)
                    {
                        result.GameDate = dateTime;
                        result.PlayerName6 = hitterInfos[5].PlayerName;
                        result.HitCount6 = hitterInfos[6].ResultHitCount;
                    }
                    pickResults.Add(result);
                }
                dateTime = dateTime.AddDays(1);

            }

            dataGridView1.DataSource = pickResults;
        }
    }
}
