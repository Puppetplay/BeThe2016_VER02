using BeThe.DataAnalyzer;
using Simulator;
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
    public partial class Form분석 : Form
    {
        public Form분석()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 분석할 날짜
            DateTime dateTime = dateTimePicker1.Value;
            List<Simulator.PickResult> pickResults = new List<Simulator.PickResult>();

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            List<AnalyzerData> hitterInfos = null;
            Manager mgr = new Manager();
            if (mgr.IsHaveGame(dateTime))
            {
                // 추천픽가져오기
                hitterInfos = mgr.GetAnalyzeData(dateTime);
            }
            else
            {
                MessageBox.Show("경기가 없는날 입니다.");
            }

            sw.Stop();
            MessageBox.Show(sw.ElapsedMilliseconds.ToString());
            dataGridView1.DataSource = hitterInfos;
        }
    }
}
