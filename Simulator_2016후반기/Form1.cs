using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulator_2016후반기
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btRun_Click(object sender, EventArgs e)
        {
            // Pick선택
            Manager mgr = new Manager();

            // 자 여기서 5월 달 부터 시뮬레이션을 코딩하라.
            // 픽구해오기 4월 26일 부터하자
            DateTime dateTime = dateTimePicker1.Value;
            List<PickResult> pickResults = new List<PickResult>();

            List<BatterInfo> hitterInfos = new List<BatterInfo>();
            // 날짜별로 돌면서 픽을 가져온다.
            if (mgr.IsHaveGame(dateTime))
            {
                // 추천픽가져오기
                hitterInfos = mgr.Pick(dateTime);
                mgr.GetPickResult(dateTime, hitterInfos);
            }

            dataGridView1.DataSource = hitterInfos;
        }
    }
}
