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
using MySql.Data.MySqlClient;
using Excel=Microsoft.Office.Interop.Excel;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections;
using System.IO.Ports;

namespace 中车
{
    public partial class ccqjlDatabase : Form
    {
        MySqlConnection myconn = null;
        MySqlCommand mycom = null;

        public static DataSet ds = new DataSet();
        public static DataTable dt = new DataTable("jl_ccq_DtPressure");
        private static double receiveData0, receiveData1, receiveData2, receiveData3, receiveData4, receiveData5;
        public static bool addDataTag=false;

        public ccqjlDatabase()
        {
            InitializeComponent();
            writeDatabase();
        }

        private void ccqjlDatabase_Load(object sender, EventArgs e)
        {
            timer1.Start();
            myconn = new MySqlConnection("Data Source =localhost;Database=barking;Username=root;Password=123456");   //本地连接
            //myconn = new MySqlConnection("Data Source =192.168.1.105; Database=barking;User=zhongche;Password=0728");  //异地连接
            myconn.Open();
            
            ccqjlkz ccqjlDataForm = new ccqjlkz();
            //ccqjlDataForm.ccqDataTransf+=ccqjlDataForm_ccqDataTransf;
            //ccqjlDataForm.ShowDialog();

        }

        public void ccqjlDataForm_ccqDataTransf( double data0, double data1, double data2, double data3, double data4, double data5)
        {
            addDataTag = true;
            receiveData0 = data0;
            receiveData1 = data1;
            receiveData2 = data2;
            receiveData3 = data3;
            receiveData4 = data4;
            receiveData5 = data5;
        }

        public void writeDatabase()
        {
            //创建DataGridView的列
            //Console.WriteLine("dss={0}", AppDomain.GetCurrentThreadId().ToString());
            DataColumn dtc = new DataColumn();
            dtc = new DataColumn("ID", typeof(Int16));
            dt.Columns.Add(dtc);
            dtc = new DataColumn("Time", typeof(String));
            dt.Columns.Add(dtc);
            dtc = new DataColumn("Pressure001", typeof(Double));
            dt.Columns.Add(dtc);
            dtc = new DataColumn("Pressure002", typeof(Double));
            dt.Columns.Add(dtc);
            dtc = new DataColumn("Pressure003", typeof(Double));
            dt.Columns.Add(dtc);
            dtc = new DataColumn("Pressure004", typeof(Double));
            dt.Columns.Add(dtc);
            dtc = new DataColumn("Pressure005", typeof(Double));
            dt.Columns.Add(dtc);
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
           
            dataGridView1.DataSource = ds.Tables["jl_ccq_DtPressure"].DefaultView;
            //dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
            dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[0];
        }

        public void addData()
        {
            //创建行
            DataRow dtr = dt.NewRow();
            dtr["ID"] = receiveData0;
            dtr["Time"] = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
            dtr["Pressure001"] = receiveData1;
            dtr["Pressure002"] = receiveData2;
            dtr["Pressure003"] = receiveData3;
            dtr["Pressure004"] = receiveData4;
            dtr["Pressure005"] = receiveData5;
            dt.Rows.Add(dtr);
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            string fileName = "";
            string saveFileName = "";
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = "xls";
            saveDialog.Filter = "Excel文件|*.xls";
            saveDialog.FileName = fileName;
            saveDialog.ShowDialog();
            saveFileName = saveDialog.FileName;
            if (saveFileName.IndexOf(":") < 0) return; //被点了取消
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                MessageBox.Show("无法创建Excel对象，您的电脑可能未安装Excel");
                return;
            }
            Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];//取得sheet1 
            //写入标题             
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            { 
                worksheet.Cells[1, i + 1] = dataGridView1.Columns[i].HeaderText; 
            }
            //写入数值
            for (int r = 0; r < dataGridView1.Rows.Count; r++)
            {
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    worksheet.Cells[r + 2, i + 1] = dataGridView1.Rows[r].Cells[i].Value;
                }
                System.Windows.Forms.Application.DoEvents();
            }
            worksheet.Columns.EntireColumn.AutoFit();//列宽自适应
            MessageBox.Show(fileName + "资料保存成功", "提示", MessageBoxButtons.OK);
            if (saveFileName != "")
            {
                try
                {
                    workbook.Saved = true;
                    workbook.SaveCopyAs(saveFileName);  //fileSaved = true;                 
                }
                catch (Exception ex)
                {//fileSaved = false;                      
                    MessageBox.Show("导出文件时出错,文件可能正被打开！\n" + ex.Message);
                }
            }
            xlApp.Quit();
            GC.Collect();//强行销毁           
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //查询表格中已有数据行数
            string countRow = "select count(*) from jl_ccq_pressure";
            MySqlCommand CountRows = new MySqlCommand(countRow, myconn);
            double RowNum = Convert.ToInt32(CountRows.ExecuteScalar());
            Console.WriteLine(RowNum);

            //数据的存储
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                //double i = 0;
                if (row.Cells["ID"].Value != null)
                {
                    //i++;
                    double dgv_id = double.Parse(row.Cells["ID"].Value.ToString()) + RowNum;
                    string dgv_time = row.Cells["Time"].Value.ToString();
                    double dgv_pre1 = double.Parse(row.Cells["Pressure001"].Value.ToString());
                    double dgv_pre2 = double.Parse(row.Cells["Pressure002"].Value.ToString());
                    double dgv_pre3 = double.Parse(row.Cells["Pressure003"].Value.ToString());
                    double dgv_pre4 = double.Parse(row.Cells["Pressure004"].Value.ToString());
                    double dgv_pre5 = double.Parse(row.Cells["Pressure005"].Value.ToString());
                    string INS = @"insert into jl_ccq_pressure (id,time,pressure001,pressure002,pressure003,pressure004,pressure005) values 
                            (" + dgv_id + ",'" + dgv_time + "'," + dgv_pre1 + "," + dgv_pre2 + "," + dgv_pre3 + "," + dgv_pre4 + "," + dgv_pre5 + ")";
                    mycom = new MySqlCommand(INS, myconn);
                    mycom.ExecuteNonQuery();

                    //if (row.Cells["ID"].Value == i.ToString())
                    //{
                    //    Console.WriteLine(row.Cells["ID"].Value);
                    //    //判断数据有没有存储结束
                    //    string cR = "select count(*) from jl_ccq_pressure";
                    //    MySqlCommand CRs = new MySqlCommand(cR, myconn);
                    //    double RN = Convert.ToInt32(CRs.ExecuteScalar());
                    //    MessageBox.Show("数据存储完成", "提示");
                    //}
                }
                //else
                //{
                //    MessageBox.Show("没有采集到数据", "Error");
                //}
            }
            MessageBox.Show("数据存储完成！", "提示");
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            DataBaseQueryForm fs = new DataBaseQueryForm();
            fs.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //while (addDataTag)
            //{
            //    addData();
            //    addDataTag = false;
            //}
            addData();
        }

        //private void ccqDataTransf_Event(double data0, double data1, double data2, double data3, double data4, double data5)
        //{
        //    receiveData0 = data0;
        //    receiveData1 = data1;
        //    receiveData2 = data2;
        //    receiveData3 = data3;
        //    receiveData4 = data4;
        //    receiveData5 = data5;
        //}
    }
}
