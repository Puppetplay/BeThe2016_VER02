using BeThe.DataAnalyzer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulator2
{
    public partial class Form투수정보보기 : Form
    {
        public Form투수정보보기()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 분석할 날짜
            DateTime dateTime = new DateTime(2016, 5, 1);
            DateTime endDateTime = new DateTime(2016, 5, 17);
            List<Simulator.PickResult> pickResults = new List<Simulator.PickResult>();

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            Manager mgr = new Manager();
            List<PitcherInfo> pInfos = new List<PitcherInfo>();
            while (dateTime < endDateTime)
            {
                if (mgr.IsHaveGame(dateTime))
                {
                    // 선발투수 리스트 얻어오기
                    var pitInfos = mgr.GetFirstPitchers(dateTime);
                    foreach(var pitInfo in pitInfos)
                    {
                        pInfos.Add(pitInfo);
                    }
                }
                dateTime = dateTime.AddDays(1);
            }
            sw.Stop();
            MessageBox.Show(sw.ElapsedMilliseconds.ToString());
            dataGridView1.DataSource = pInfos;
        }
    }
}
