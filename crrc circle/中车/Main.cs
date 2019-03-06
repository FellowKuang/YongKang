using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 中车
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ccq f1 = new ccq();
            f1.Show();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            cyzd f2 = new cyzd();
            f2.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            hjgc f3 = new hjgc();
            f3.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            jjzd f4 = new jjzd();
            f4.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            jlSetting f5 = new jlSetting();
            f5.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            cyzdjlkz f6 = new cyzdjlkz();
            f6.Show();
        }
    }
}
