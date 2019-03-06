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
    public partial class ccqjlkzDGV : Form
    {
        //public static DataTable dt = new DataTable("jl_ccq_DtPressure");
        //bool cqTag = ccqjlkz.cqTag;
        //bool dgvTag = ccqjlkz.DGVTag;
        //double trainNum = ccqjlkz.trainNum;
        //double receiveData1 = ccqjlkz.receiveData1;
        //double receiveData2 = ccqjlkz.receiveData2;
        //double receiveData3 = ccqjlkz.receiveData3;
        //double receiveData4 = ccqjlkz.receiveData4;
        //double receiveData5 = ccqjlkz.receiveData5;

        //private delegate void dataDGV(string a);

        //public ccqjlkzDGV()
        //{
        //    InitializeComponent();
        //}

        //private void ccqjlkzDGV_Load(object sender, EventArgs e)
        //{
        //    Thread dgvAdd = new Thread(dgvDataShow);
        //    dgvAdd.Start();

        //}

        //private void dgvDataShow()
        //{
        //    while (true)
        //    {
        //        while (dgvTag)
        //        {
        //            dataSave();
        //            dgvTag = false;
        //            break;
        //        }
        //    }
        //}

        ////数据存储到表格
        //public void dataSave()
        //{
        //    //添加行
        //    DataRow dtr = dt.NewRow();
        //    if (cqTag == true)
        //    {
        //        dtr["ID"] = trainNum;
        //    }
        //    else
        //    {
        //        dtr["ID"] = 0;
        //    }
        //    dtr["Time"] = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
        //    dtr["列前"] = double.Parse(string.Format("{0:F}", receiveData1));
        //    dtr["副风缸"] = double.Parse(string.Format("{0:F}", receiveData2));
        //    dtr["制动缸"] = double.Parse(string.Format("{0:F}", receiveData3));
        //    dtr["列尾"] = double.Parse(string.Format("{0:F}", receiveData4));
        //    dtr["加缓缸"] = double.Parse(string.Format("{0:F}", receiveData5));

        //    dt.Rows.Add(dtr);

        //    foreach (DataGridViewColumn item in dataGridView1.Columns)
        //    {
        //        item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        //    }
        //}

    }
}
