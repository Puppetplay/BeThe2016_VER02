using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PickSimulator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btRun_Click(object sender, EventArgs e)
        {
            Manager mgr = new Manager();

            // 자 여기서 5월 달 부터 시뮬레이션을 코딩하라.
            // 픽구해오기 4월 26일 부터하자
            DateTime dateTime = new DateTime(2016, 6, 10);
            DateTime endDateTime = new DateTime(2016, 6, 24);
            List<PickResult> pickResults = new List<PickResult>();
            while (dateTime < endDateTime)
            {
                if (mgr.IsHaveGame(dateTime))
                {
                    // 추천픽가져오기
                    var hitterInfos = mgr.Pick(dateTime, 3);

                    // 결과를 만든다
                    PickResult result = new PickResult();
                    result.DateTime = dateTime.ToString("yyyyMMdd");
                    mgr.GetPickResult(dateTime, hitterInfos);
                    if (hitterInfos.Count > 0)
                    {
                        result.Name0 = hitterInfos[0].Name;
                        result.HitCount0 = hitterInfos[0].RHitCount;
                    }

                    if (hitterInfos.Count > 1)
                    {
                        result.Name1 = hitterInfos[1].Name;
                        result.HitCount1 = hitterInfos[1].RHitCount;
                    }

                    if (hitterInfos.Count > 2)
                    {
                        result.Name2 = hitterInfos[2].Name;
                        result.HitCount2 = hitterInfos[2].RHitCount;
                    }

                    if (hitterInfos.Count > 3)
                    {
                        result.Name3 = hitterInfos[3].Name;
                        result.HitCount3 = hitterInfos[3].RHitCount;
                    }

                    if (hitterInfos.Count > 4)
                    {
                        result.Name4 = hitterInfos[4].Name;
                        result.HitCount4 = hitterInfos[4].RHitCount;
                    }
                    pickResults.Add(result);
                }
                dateTime = dateTime.AddDays(1);
            }

            dataGridView1.DataSource = pickResults;
        }
    }
}
