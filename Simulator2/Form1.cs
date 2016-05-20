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
    public partial class Form1 : Form
    {
        public Form1()
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
            while(dateTime < endDateTime)
            {
                if(mgr.IsHaveGame(dateTime))
                {
                    // 추천픽가져오기
                    var hitterInfos = mgr.Pick(dateTime);
                }
                dateTime = dateTime.AddDays(1);
            }
            sw.Stop();
            MessageBox.Show(sw.ElapsedMilliseconds.ToString());
        }
    }
}
