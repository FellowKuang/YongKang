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
    public partial class jlSetting : Form
    {
        public static int BIANZU, TRAIN;

        public jlSetting()
        {
            InitializeComponent();
        }

        private void jlSetting_Load(object sender, EventArgs e)
        {

        }

        private void btnSure_Click(object sender, EventArgs e)
        {
            //标记列车编组数量
            if(clbBianzu.GetItemChecked(0))
            {
                BIANZU = 1;
            }
            else if (clbBianzu.GetItemChecked(1))
            {
                BIANZU = 80;
            }
            else if (clbBianzu.GetItemChecked(2))
            {
                BIANZU = 100;
            }
            else
            {
                BIANZU = 150;
            }

            //标记级联实际列车数量
            if(clbTrain.GetItemChecked(0))
            {
                TRAIN = 1;
            }
            else if (clbTrain.GetItemChecked(1))
            {
                TRAIN = 5;
            }
            else if(clbTrain.GetItemChecked(2))
            {
                TRAIN = 10;
            }
            else if(clbTrain.GetItemChecked(3))
            {
                TRAIN = 15;
            }
            else
            {
                TRAIN = 20;
            }
        }
    }
}
