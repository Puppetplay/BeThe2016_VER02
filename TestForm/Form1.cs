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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Int32 playerId = Convert.ToInt32(textBox1.Text);
            BeThe.DataAnalyzer.Manager mgr = new BeThe.DataAnalyzer.Manager();
            var pitcherInfo = mgr.GetPitcherInfo(playerId, 4);

            textBox2.Text = pitcherInfo.OnBaseRatio.ToString();
            textBox3.Text = pitcherInfo.HitRatio.ToString();
            textBox4.Text = pitcherInfo.HitRatioLHand.ToString();
            textBox5.Text = pitcherInfo.HitRatioRHand.ToString();
        }
    }
}
