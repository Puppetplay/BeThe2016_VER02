using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestForm
{
    public partial class Form타자분석 : Form
    {
        public Form타자분석()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var teamInitial = textBox1.Text;
            BeThe.DataAnalyzer.Manager mgr = new BeThe.DataAnalyzer.Manager();
            var players = mgr.GetStartPlayerForNDays(teamInitial, 4);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var teamInitial = textBox2.Text;
            Int32 playerId = Convert.ToInt32(textBox3.Text);
            BeThe.DataAnalyzer.Manager mgr = new BeThe.DataAnalyzer.Manager();
            var count = mgr.GetHitNumberForNDays(teamInitial, playerId, 1);
            MessageBox.Show(count.ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var teamInitial = textBox2.Text;
            Int32 playerId = Convert.ToInt32(textBox3.Text);
            BeThe.DataAnalyzer.Manager mgr = new BeThe.DataAnalyzer.Manager();
            var s = mgr.IsAllDayHit(teamInitial, playerId, 5);
            MessageBox.Show(s.ToString());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var teamInitial = textBox2.Text;
            Int32 playerId = Convert.ToInt32(textBox3.Text);
            BeThe.DataAnalyzer.Manager mgr = new BeThe.DataAnalyzer.Manager();
            var s = mgr.GetHitRatio(teamInitial, playerId, 5);
            MessageBox.Show(s.ToString());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var teamInitial = textBox2.Text;
            Int32 playerId = Convert.ToInt32(textBox3.Text);
            BeThe.DataAnalyzer.Manager mgr = new BeThe.DataAnalyzer.Manager();
            var s = mgr.IsLongHitLastGame(teamInitial, playerId);
            MessageBox.Show(s.ToString());
        }
    }
}
