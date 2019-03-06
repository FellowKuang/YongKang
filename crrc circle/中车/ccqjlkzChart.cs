using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//添加命名空间
using System.Windows.Forms.DataVisualization.Charting;
using MySql.Data.MySqlClient;//添加引用(.dll)
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections;
using System.IO.Ports;


namespace 中车
{
    public partial class ccqjlkzChart : Form
    {
        ArrayList jlTime = ccqjlkz.jlTime;
        ArrayList lcgBefore = ccqjlkz.lcgBefore;
        ArrayList ffgPre = ccqjlkz.ffgPre;
        ArrayList zdgPre = ccqjlkz.zdgPre;
        ArrayList lcgAfter = ccqjlkz.lcgAfter;
        ArrayList jhfgPre = ccqjlkz.jhfgPre;
        public ccqjlkzChart()
        {
            InitializeComponent();
        }

        private void chart1_Click(object sender, EventArgs e)
        {
            Series series0 = chartDemo.Series[0];
            Series series1 = chartDemo.Series[1];
            Series series2 = chartDemo.Series[2];
            Series series3 = chartDemo.Series[3];
            Series series4 = chartDemo.Series[4];

            for (int n = 0; n < jhfgPre.Count; n++)
            {
                series0.Points.AddXY(jlTime[n], lcgBefore[n]);
                series1.Points.AddXY(jlTime[n], ffgPre[n]);
                series2.Points.AddXY(jlTime[n], zdgPre[n]);
                series3.Points.AddXY(jlTime[n], lcgAfter[n]);
                series4.Points.AddXY(jlTime[n], jhfgPre[n]);
            }
        }
    }
}
