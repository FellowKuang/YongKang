//第一版

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace 中车
{
    public partial class modelComparison : Form
    {
        double trainNum = ccqjlkz.trainNum;
        double Ps, P0, L, N, D, S, zd_t0, lcg_P, k, k1, S2, t1, W, V_lcg, t0, tw, T, Ss, t;

        public modelComparison()
        {
            InitializeComponent();
            lcgmodelInit();
        }

        private void modelComparison_Load(object sender, EventArgs e)
        {
            Ss = ccqjlkz.correctS;//修正计算得到的S值
            string str1 = "第";
            string str2 = "辆车的修正S值为：";
            label1.Text = str1 + trainNum + str2+Ss;

            S = 0.1092 * Math.Pow(10, -6);//150辆编组

            for (t = 0.001; t <= 400; t++)
            {
                lcgModelFun_S();
                this.chart1.Series[0].Points.AddXY(t, lcg_P);
                lcgModelFun_Ss();
                this.chart1.Series[1].Points.AddXY(t, lcg_P);
            }

        }

        private void lcgmodelInit()
        {
            Ps = 700;
            P0 = 100;
            L = 11;
            N = 1;
            D = 0.03175;
            lcg_P = 0;
            zd_t0 = 1.5;
            k = 1.5;
            k1 = 2;
            S2 = 2 * Math.Pow(2, -6);
            t1 = 1;
            W = 336;
            V_lcg = (Math.PI * Math.Pow(D, 2)) * N * L / 4;
            @t0 = -0.000000005762680 + 0.000002199411977 * N - 0.000304578453220 * Math.Pow(N, 2) +
                  0.019014019052861 * Math.Pow(N, 3) - 0.498602480934510 * Math.Pow(N, 4) +
                  10.549828138352119 * Math.Pow(N, 5) + 12.392425961348907 * Math.Pow(N, 6);
            tw = (N * L) / W;
            T = 298;
        }

        private void lcgModelFun_S()
        {
            if (lcg_P >= 0 & lcg_P < 500)
            {
                @lcg_P = (Ps * Math.Sqrt(T) * S * t) / (8.619 * 0.01 * V_lcg) + P0;
                //- (400 / Math.Pow(t0, 2)) * (Math.Pow(t, 2) - t0 * t);
            }

            else if (lcg_P >= 500)
            {
                lcg_P = 500;
            }
        }

        private void lcgModelFun_Ss()
        {
            if (lcg_P >= 0 & lcg_P < 500)
            {
                @lcg_P = (Ps * Math.Sqrt(T) * Ss * t) / (8.619 * 0.01 * V_lcg) + P0;
                //- (400 / Math.Pow(t0, 2)) * (Math.Pow(t, 2) - t0 * t);
            }

            else if (lcg_P >= 500)
            {
                lcg_P = 500;
            }
        }

    }
}
