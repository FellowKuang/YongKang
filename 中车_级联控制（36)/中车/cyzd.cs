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
    public partial class cyzd : Form
    {
        DateTime starttime, endtime, ExeTime, GetDrawTime, DealTime, zdBeginTime, hjBeginTime, hjTime;
        private static string beginTime, getTime1, getTime2;
        private static double delta_T, xdc, xdcTemp1, xdcTemp2, dataNum=1,Xval=399;
        private static double dataNum1, dataNum2, dataNum3, dataNum4, dataNum5, dataNum6, dataNum7, dataNum8, dataNum9, dataNum10,
            dataNum11, dataNum12, dataNum13, dataNum14, dataNum15, dataNum16, dataNum17, dataNum18, dataNum19, dataNum20;
        private int num = 1, zdNum=0;
        public static int iNum;
        MySqlConnection myconn = null;
        MySqlCommand mycom = null;
        public static string receiveData;
        //1：列车管；2：副风缸；3：制动缸；4：缓解风缸
        public static double receiveData10, receiveData11, receiveData12, receiveData13, receiveData14, receiveData15;
        public static double receiveData20, receiveData21, receiveData22, receiveData23, receiveData24, receiveData25;
        public static double receiveData30, receiveData31, receiveData32, receiveData33, receiveData34, receiveData35;
        public static double receiveData40, receiveData41, receiveData42, receiveData43, receiveData44, receiveData45;
        public static ArrayList SocketArr = new ArrayList();
        public static ArrayList cyzd_list_D1 = new ArrayList();
        public static ArrayList cyzd_list_D2 = new ArrayList();
        public static ArrayList cyzd_list_D3 = new ArrayList();
        public static ArrayList cyzd_list_D4 = new ArrayList();
        public static ArrayList cyzd_list_D5 = new ArrayList();
        public static string[] arr;
        public static string Result1 = string.Empty,Result2=null,Result3=null,Result4=null;
        public static DataSet ds = new DataSet();
        public static Encoding encode = Encoding.UTF8;
        public static DataTable dt = new DataTable("cyzd_DtPressure");
        Thread Listen_Thred, recDataThread1, recDataThread2, recDataThread3, recDataThread4, dealDataThread;
        Socket acceptSocket1, acceptSocket2, acceptSocket3, acceptSocket4;
        public delegate void AddFile();
        public delegate void Draw_SaveDelegate();
        public delegate void ListenDelegate(double data0, double data1, double data2, double data3, double data4, double data5);
        public delegate void dealsocket(Socket s);
        private delegate void SaveDelegate(double data0, double data1, double data2, double data3, double data4, double data5);
        private string cbPortName = "";
        private string cbBaud = "";
        private static double hj_lcg_P, lcg_ori_P0, hjfg_P, hj_temp_P1, hj_temp_P2;
        //缓解模型变量
        bool hjTag = false,hjBegin=false;//缓解的标志
        Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static Thread dealD;

        private double lcg_P, zdg_P, Ps, S, V_lcg, P0, t0, tw, D, Pf0, ffg_P, P1, S1, k, Pf, V_ffg, V_zdg;
        //lcg_P：列车管绝对压力
        //zdg_P：制动缸升压时的绝对压力
        //Ps：风源系统绝对压力
        //S：有效节流系数
        //V_lcg：列车管体积
        //P0：列车管初始绝对压力
        //t0：两段局减总时间
        //tw：空气被传播时间
        //D：列车管直径
        //Pf：副风缸绝对压力
        //Pf0：副风缸初始绝对压力
        //ffg_P：副风缸绝对压力
        //P1：局减结束后列车管的压力值（绝对压力）
        //S1：局减时节流孔有效截面积
        //k：局减阶段对节流孔有效面积的影响参数
        //V_ffg：副风缸体积
        //V_zdg：制动缸体积
        private double Ps1, k1, S2, Pz0, t2, Pz1, Vz, t1, t3, r;
        //Ps1：一段局减结束后列车管的压力值
        //Ps：列车管初始绝对压力
        //k1：第一阶段局减时与列车管减压有关的节流孔有效截面的影响参数（取2）
        //S2：局减时列车管排气的节流孔有效截面积（取2*10^-6）
        //Pz0：制动缸初始绝对压力
        //t2：第二阶段局减时间
        //Pz1：第二阶段局减作用后，制动缸的压力值
        //Vz：制动缸体积     
        //t1：第一阶段局减时间
        //t3：压力保持时间（取1s）
        //r：减压量
        private double t, T = 298, N, L, W, Tt = 0.01, constT=Math.Sqrt(298);
        private int n = 0;
        //t：时间
        //T：绝对温度
        //N：车辆位置
        //L：列车管长度
        //W：空气波速
        private double R_lcg, R_ffg, R_zdg;
        public int con_lcg_Num, con_ffg_Num, zdTag = 0, zd_Relife;
        //SerialPort conPort;
        private SerialPort conPort = new SerialPort();

        Series series10, series11, series12, series13, series14, series20, series21, series22, series23, series24;
        Series series30, series31, series32, series33, series34, series40, series41, series42, series43, series44;
        Series series50, series51, series52, series53, series54, series60, series61, series62, series63, series64;
        Series series70, series71, series72, series73, series74, series80, series81, series82, series83, series84;
        Series series90, series91, series92, series93, series94, series100, series101, series102, series103, series104;
        Series series110, series111, series112, series113, series114, series120, series121, series122, series123, series124;
        Series series130, series131, series132, series133, series134, series140, series141, series142, series143, series144;
        Series series150, series151, series152, series153, series154, series160, series161, series162, series163, series164;
        Series series170, series171, series172, series173, series174, series180, series181, series182, series183, series184;
        Series series190, series191, series192, series193, series194, series200, series201, series202, series203, series204;


        private void btnHj_Click(object sender, EventArgs e)
        {
            if(cyzd_list_D5.Count==0)
            {
                if(receiveData10==0)
                {
                    cyzd_list_D5.Add(receiveData15);
                }
            }
            hjTag = true;
        }       

        public cyzd()
        {
            InitializeComponent();
            Draw_Save();
            InitChart();
            model_original_Num();
        }

        //画图区域初始化
        public void InitChart()
        {
            chartTimer.Interval = 1000;
            chartTimer.Tick += chartTimer_Tick;

            //chartDemo1.DoubleClick += chartDemo_DoubleClick;
            chartDemo1.ChartAreas[0].AxisX.LabelStyle.Format = "F";
            chartDemo1.ChartAreas[0].AxisX.ScaleView.Size = 500;
            chartDemo1.ChartAreas[0].AxisY.ScaleView.Size = 600;
            //chartDemo1.ChartAreas[0].AxisX.Interval = 10;
            chartDemo1.ChartAreas[0].AxisX.Minimum = 0;
            chartDemo1.ChartAreas[0].AxisY.Minimum = 0;
            chartDemo1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chartDemo1.ChartAreas[0].CursorX.AutoScroll = true;
            chartDemo1.ChartAreas[0].AxisX.Title = "时间 / S";
            chartDemo1.ChartAreas[0].AxisY.Title = "气压 / kPa";

            //chartDemo2.DoubleClick += chartDemo_DoubleClick;
            chartDemo2.ChartAreas[0].AxisX.LabelStyle.Format = "F";
            chartDemo2.ChartAreas[0].AxisX.ScaleView.Size = 500;
            chartDemo2.ChartAreas[0].AxisY.ScaleView.Size = 600;
            //chartDemo2.ChartAreas[0].AxisX.Interval = 10;
            chartDemo2.ChartAreas[0].AxisX.Minimum = 0;
            chartDemo2.ChartAreas[0].AxisY.Minimum = 0;
            chartDemo2.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo2.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chartDemo2.ChartAreas[0].CursorX.AutoScroll = true;
            chartDemo2.ChartAreas[0].AxisX.Title = "时间 / S";
            chartDemo2.ChartAreas[0].AxisY.Title = "气压 / kPa";

            //chartDemo3.DoubleClick += chartDemo_DoubleClick;
            chartDemo3.ChartAreas[0].AxisX.LabelStyle.Format = "F";
            chartDemo3.ChartAreas[0].AxisX.ScaleView.Size = 500;
            chartDemo3.ChartAreas[0].AxisY.ScaleView.Size = 600;
            //chartDemo3.ChartAreas[0].AxisX.Interval = 10;
            chartDemo3.ChartAreas[0].AxisX.Minimum = 0;
            chartDemo3.ChartAreas[0].AxisY.Minimum = 0;
            chartDemo3.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo3.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chartDemo3.ChartAreas[0].CursorX.AutoScroll = true;
            chartDemo3.ChartAreas[0].AxisX.Title = "时间 / S";
            chartDemo3.ChartAreas[0].AxisY.Title = "气压 / kPa";

            //chartDemo4.DoubleClick += chartDemo_DoubleClick;
            chartDemo4.ChartAreas[0].AxisX.LabelStyle.Format = "F";
            chartDemo4.ChartAreas[0].AxisX.ScaleView.Size = 500;
            chartDemo4.ChartAreas[0].AxisY.ScaleView.Size = 600;
            //chartDemo4.ChartAreas[0].AxisX.Interval = 10;
            chartDemo4.ChartAreas[0].AxisX.Minimum = 0;
            chartDemo4.ChartAreas[0].AxisY.Minimum = 0;
            chartDemo4.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo4.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chartDemo4.ChartAreas[0].CursorX.AutoScroll = true;
            chartDemo4.ChartAreas[0].AxisX.Title = "时间 / S";
            chartDemo4.ChartAreas[0].AxisY.Title = "气压 / kPa";

            //chartDemo5.DoubleClick += chartDemo_DoubleClick;
            chartDemo5.ChartAreas[0].AxisX.LabelStyle.Format = "F";
            chartDemo5.ChartAreas[0].AxisX.ScaleView.Size = 500;
            chartDemo5.ChartAreas[0].AxisY.ScaleView.Size = 600;
            chartDemo5.ChartAreas[0].AxisX.Minimum = 0;
            chartDemo5.ChartAreas[0].AxisY.Minimum = 0;
            chartDemo5.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo5.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chartDemo5.ChartAreas[0].CursorX.AutoScroll = true;
            chartDemo5.ChartAreas[0].AxisX.Title = "时间 / S";
            chartDemo5.ChartAreas[0].AxisY.Title = "气压 / kPa";

            chartDemo6.ChartAreas[0].AxisX.LabelStyle.Format = "F";
            chartDemo6.ChartAreas[0].AxisX.ScaleView.Size = 500;
            chartDemo6.ChartAreas[0].AxisY.ScaleView.Size = 600;
            chartDemo6.ChartAreas[0].AxisX.Minimum = 0;
            chartDemo6.ChartAreas[0].AxisY.Minimum = 0;
            chartDemo6.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo6.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chartDemo6.ChartAreas[0].CursorX.AutoScroll = true;
            chartDemo6.ChartAreas[0].AxisX.Title = "时间 / S";
            chartDemo6.ChartAreas[0].AxisY.Title = "气压 / kPa";

            chartDemo7.ChartAreas[0].AxisX.LabelStyle.Format = "F";
            chartDemo7.ChartAreas[0].AxisX.ScaleView.Size = 500;
            chartDemo7.ChartAreas[0].AxisY.ScaleView.Size = 600;
            chartDemo7.ChartAreas[0].AxisX.Minimum = 0;
            chartDemo7.ChartAreas[0].AxisY.Minimum = 0;
            chartDemo7.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo7.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chartDemo7.ChartAreas[0].CursorX.AutoScroll = true;
            chartDemo7.ChartAreas[0].AxisX.Title = "时间 / S";
            chartDemo7.ChartAreas[0].AxisY.Title = "气压 / kPa";

            chartDemo8.ChartAreas[0].AxisX.LabelStyle.Format = "F";
            chartDemo8.ChartAreas[0].AxisX.ScaleView.Size = 500;
            chartDemo8.ChartAreas[0].AxisY.ScaleView.Size = 600;
            chartDemo8.ChartAreas[0].AxisX.Minimum = 0;
            chartDemo8.ChartAreas[0].AxisY.Minimum = 0;
            chartDemo8.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo8.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chartDemo8.ChartAreas[0].CursorX.AutoScroll = true;
            chartDemo8.ChartAreas[0].AxisX.Title = "时间 / S";
            chartDemo8.ChartAreas[0].AxisY.Title = "气压 / kPa";

            chartDemo9.ChartAreas[0].AxisX.LabelStyle.Format = "F";
            chartDemo9.ChartAreas[0].AxisX.ScaleView.Size = 500;
            chartDemo9.ChartAreas[0].AxisY.ScaleView.Size = 600;
            chartDemo9.ChartAreas[0].AxisX.Minimum = 0;
            chartDemo9.ChartAreas[0].AxisY.Minimum = 0;
            chartDemo9.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo9.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chartDemo9.ChartAreas[0].CursorX.AutoScroll = true;
            chartDemo9.ChartAreas[0].AxisX.Title = "时间 / S";
            chartDemo9.ChartAreas[0].AxisY.Title = "气压 / kPa";

            chartDemo10.ChartAreas[0].AxisX.LabelStyle.Format = "F";
            chartDemo10.ChartAreas[0].AxisX.ScaleView.Size = 500;
            chartDemo10.ChartAreas[0].AxisY.ScaleView.Size = 600;
            chartDemo10.ChartAreas[0].AxisX.Minimum = 0;
            chartDemo10.ChartAreas[0].AxisY.Minimum = 0;
            chartDemo10.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo10.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chartDemo10.ChartAreas[0].CursorX.AutoScroll = true;
            chartDemo10.ChartAreas[0].AxisX.Title = "时间 / S";
            chartDemo10.ChartAreas[0].AxisY.Title = "气压 / kPa";

            chartDemo11.ChartAreas[0].AxisX.LabelStyle.Format = "F";
            chartDemo11.ChartAreas[0].AxisX.ScaleView.Size = 500;
            chartDemo11.ChartAreas[0].AxisY.ScaleView.Size = 600;
            chartDemo11.ChartAreas[0].AxisX.Minimum = 0;
            chartDemo11.ChartAreas[0].AxisY.Minimum = 0;
            chartDemo11.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo11.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chartDemo11.ChartAreas[0].CursorX.AutoScroll = true;
            chartDemo11.ChartAreas[0].AxisX.Title = "时间 / S";
            chartDemo11.ChartAreas[0].AxisY.Title = "气压 / kPa";

            chartDemo12.ChartAreas[0].AxisX.LabelStyle.Format = "F";
            chartDemo12.ChartAreas[0].AxisX.ScaleView.Size = 500;
            chartDemo12.ChartAreas[0].AxisY.ScaleView.Size = 600;
            chartDemo12.ChartAreas[0].AxisX.Minimum = 0;
            chartDemo12.ChartAreas[0].AxisY.Minimum = 0;
            chartDemo12.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo12.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chartDemo12.ChartAreas[0].CursorX.AutoScroll = true;
            chartDemo12.ChartAreas[0].AxisX.Title = "时间 / S";
            chartDemo12.ChartAreas[0].AxisY.Title = "气压 / kPa";

            chartDemo13.ChartAreas[0].AxisX.LabelStyle.Format = "F";
            chartDemo13.ChartAreas[0].AxisX.ScaleView.Size = 500;
            chartDemo13.ChartAreas[0].AxisY.ScaleView.Size = 600;
            chartDemo13.ChartAreas[0].AxisX.Minimum = 0;
            chartDemo13.ChartAreas[0].AxisY.Minimum = 0;
            chartDemo13.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo13.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chartDemo13.ChartAreas[0].CursorX.AutoScroll = true;
            chartDemo13.ChartAreas[0].AxisX.Title = "时间 / S";
            chartDemo13.ChartAreas[0].AxisY.Title = "气压 / kPa";

            chartDemo14.ChartAreas[0].AxisX.LabelStyle.Format = "F";
            chartDemo14.ChartAreas[0].AxisX.ScaleView.Size = 500;
            chartDemo14.ChartAreas[0].AxisY.ScaleView.Size = 600;
            chartDemo14.ChartAreas[0].AxisX.Minimum = 0;
            chartDemo14.ChartAreas[0].AxisY.Minimum = 0;
            chartDemo14.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo14.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chartDemo14.ChartAreas[0].CursorX.AutoScroll = true;
            chartDemo14.ChartAreas[0].AxisX.Title = "时间 / S";
            chartDemo14.ChartAreas[0].AxisY.Title = "气压 / kPa";

            chartDemo15.ChartAreas[0].AxisX.LabelStyle.Format = "F";
            chartDemo15.ChartAreas[0].AxisX.ScaleView.Size = 500;
            chartDemo15.ChartAreas[0].AxisY.ScaleView.Size = 600;
            chartDemo15.ChartAreas[0].AxisX.Minimum = 0;
            chartDemo15.ChartAreas[0].AxisY.Minimum = 0;
            chartDemo15.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo15.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chartDemo15.ChartAreas[0].CursorX.AutoScroll = true;
            chartDemo15.ChartAreas[0].AxisX.Title = "时间 / S";
            chartDemo15.ChartAreas[0].AxisY.Title = "气压 / kPa";

            chartDemo16.ChartAreas[0].AxisX.LabelStyle.Format = "F";
            chartDemo16.ChartAreas[0].AxisX.ScaleView.Size = 500;
            chartDemo16.ChartAreas[0].AxisY.ScaleView.Size = 600;
            chartDemo16.ChartAreas[0].AxisX.Minimum = 0;
            chartDemo16.ChartAreas[0].AxisY.Minimum = 0;
            chartDemo16.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo16.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chartDemo16.ChartAreas[0].CursorX.AutoScroll = true;
            chartDemo16.ChartAreas[0].AxisX.Title = "时间 / S";
            chartDemo16.ChartAreas[0].AxisY.Title = "气压 / kPa";

            chartDemo17.ChartAreas[0].AxisX.LabelStyle.Format = "F";
            chartDemo17.ChartAreas[0].AxisX.ScaleView.Size = 500;
            chartDemo17.ChartAreas[0].AxisY.ScaleView.Size = 600;
            chartDemo17.ChartAreas[0].AxisX.Minimum = 0;
            chartDemo17.ChartAreas[0].AxisY.Minimum = 0;
            chartDemo17.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo17.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chartDemo17.ChartAreas[0].CursorX.AutoScroll = true;
            chartDemo17.ChartAreas[0].AxisX.Title = "时间 / S";
            chartDemo17.ChartAreas[0].AxisY.Title = "气压 / kPa";

            chartDemo18.ChartAreas[0].AxisX.LabelStyle.Format = "F";
            chartDemo18.ChartAreas[0].AxisX.ScaleView.Size = 500;
            chartDemo18.ChartAreas[0].AxisY.ScaleView.Size = 600;
            chartDemo18.ChartAreas[0].AxisX.Minimum = 0;
            chartDemo18.ChartAreas[0].AxisY.Minimum = 0;
            chartDemo18.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo18.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chartDemo18.ChartAreas[0].CursorX.AutoScroll = true;
            chartDemo18.ChartAreas[0].AxisX.Title = "时间 / S";
            chartDemo18.ChartAreas[0].AxisY.Title = "气压 / kPa";

            chartDemo19.ChartAreas[0].AxisX.LabelStyle.Format = "F";
            chartDemo19.ChartAreas[0].AxisX.ScaleView.Size = 500;
            chartDemo19.ChartAreas[0].AxisY.ScaleView.Size = 600;
            chartDemo19.ChartAreas[0].AxisX.Minimum = 0;
            chartDemo19.ChartAreas[0].AxisY.Minimum = 0;
            chartDemo19.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo19.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chartDemo19.ChartAreas[0].CursorX.AutoScroll = true;
            chartDemo19.ChartAreas[0].AxisX.Title = "时间 / S";
            chartDemo19.ChartAreas[0].AxisY.Title = "气压 / kPa";

            chartDemo20.ChartAreas[0].AxisX.LabelStyle.Format = "F";
            chartDemo20.ChartAreas[0].AxisX.ScaleView.Size = 500;
            chartDemo20.ChartAreas[0].AxisY.ScaleView.Size = 600;
            chartDemo20.ChartAreas[0].AxisX.Minimum = 0;
            chartDemo20.ChartAreas[0].AxisY.Minimum = 0;
            chartDemo20.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo20.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chartDemo20.ChartAreas[0].CursorX.AutoScroll = true;
            chartDemo20.ChartAreas[0].AxisX.Title = "时间 / S";
            chartDemo20.ChartAreas[0].AxisY.Title = "气压 / kPa";

            chartTimer.Start();


        }

        //表格初始化
        public void Draw_Save()
        {
            //添加列
            this.dataGridView1.Columns[0].HeaderCell.Value = "ID";
            this.dataGridView1.Columns[1].HeaderCell.Value = "时间";
            this.dataGridView1.Columns[2].HeaderCell.Value = "列车管前端";
            this.dataGridView1.Columns[3].HeaderCell.Value = "副风缸";
            this.dataGridView1.Columns[4].HeaderCell.Value = "制动缸";
            this.dataGridView1.Columns[5].HeaderCell.Value = "列车管尾端";
            this.dataGridView1.Columns[6].HeaderCell.Value = "加缓风缸";
            foreach (DataGridViewColumn item in dataGridView1.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            //添加列
            this.dataGridView2.Columns[0].HeaderCell.Value = "ID";
            this.dataGridView2.Columns[1].HeaderCell.Value = "时间";
            this.dataGridView2.Columns[2].HeaderCell.Value = "列车管前端";
            this.dataGridView2.Columns[3].HeaderCell.Value = "副风缸";
            this.dataGridView2.Columns[4].HeaderCell.Value = "制动缸";
            this.dataGridView2.Columns[5].HeaderCell.Value = "列车管尾端";
            this.dataGridView2.Columns[6].HeaderCell.Value = "加缓风缸";
            foreach (DataGridViewColumn item in dataGridView2.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            //添加列
            this.dataGridView3.Columns[0].HeaderCell.Value = "ID";
            this.dataGridView3.Columns[1].HeaderCell.Value = "时间";
            this.dataGridView3.Columns[2].HeaderCell.Value = "列车管前端";
            this.dataGridView3.Columns[3].HeaderCell.Value = "副风缸";
            this.dataGridView3.Columns[4].HeaderCell.Value = "制动缸";
            this.dataGridView3.Columns[5].HeaderCell.Value = "列车管尾端";
            this.dataGridView3.Columns[6].HeaderCell.Value = "加缓风缸";
            foreach (DataGridViewColumn item in dataGridView3.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            //添加列
            this.dataGridView4.Columns[0].HeaderCell.Value = "ID";
            this.dataGridView4.Columns[1].HeaderCell.Value = "时间";
            this.dataGridView4.Columns[2].HeaderCell.Value = "列车管前端";
            this.dataGridView4.Columns[3].HeaderCell.Value = "副风缸";
            this.dataGridView4.Columns[4].HeaderCell.Value = "制动缸";
            this.dataGridView4.Columns[5].HeaderCell.Value = "列车管尾端";
            this.dataGridView4.Columns[6].HeaderCell.Value = "加缓风缸";
            foreach (DataGridViewColumn item in dataGridView4.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            //添加列
            this.dataGridView5.Columns[0].HeaderCell.Value = "ID";
            this.dataGridView5.Columns[1].HeaderCell.Value = "时间";
            this.dataGridView5.Columns[2].HeaderCell.Value = "列车管前端";
            this.dataGridView5.Columns[3].HeaderCell.Value = "副风缸";
            this.dataGridView5.Columns[4].HeaderCell.Value = "制动缸";
            this.dataGridView5.Columns[5].HeaderCell.Value = "列车管尾端";
            this.dataGridView5.Columns[6].HeaderCell.Value = "加缓风缸";
            foreach (DataGridViewColumn item in dataGridView5.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            //添加列
            this.dataGridView6.Columns[0].HeaderCell.Value = "ID";
            this.dataGridView6.Columns[1].HeaderCell.Value = "时间";
            this.dataGridView6.Columns[2].HeaderCell.Value = "列车管前端";
            this.dataGridView6.Columns[3].HeaderCell.Value = "副风缸";
            this.dataGridView6.Columns[4].HeaderCell.Value = "制动缸";
            this.dataGridView6.Columns[5].HeaderCell.Value = "列车管尾端";
            this.dataGridView6.Columns[6].HeaderCell.Value = "加缓风缸";
            foreach (DataGridViewColumn item in dataGridView6.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            //添加列
            this.dataGridView7.Columns[0].HeaderCell.Value = "ID";
            this.dataGridView7.Columns[1].HeaderCell.Value = "时间";
            this.dataGridView7.Columns[2].HeaderCell.Value = "列车管前端";
            this.dataGridView7.Columns[3].HeaderCell.Value = "副风缸";
            this.dataGridView7.Columns[4].HeaderCell.Value = "制动缸";
            this.dataGridView7.Columns[5].HeaderCell.Value = "列车管尾端";
            this.dataGridView7.Columns[6].HeaderCell.Value = "加缓风缸";
            foreach (DataGridViewColumn item in dataGridView7.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            //添加列
            this.dataGridView8.Columns[0].HeaderCell.Value = "ID";
            this.dataGridView8.Columns[1].HeaderCell.Value = "时间";
            this.dataGridView8.Columns[2].HeaderCell.Value = "列车管前端";
            this.dataGridView8.Columns[3].HeaderCell.Value = "副风缸";
            this.dataGridView8.Columns[4].HeaderCell.Value = "制动缸";
            this.dataGridView8.Columns[5].HeaderCell.Value = "列车管尾端";
            this.dataGridView8.Columns[6].HeaderCell.Value = "加缓风缸";
            foreach (DataGridViewColumn item in dataGridView8.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            //添加列
            this.dataGridView9.Columns[0].HeaderCell.Value = "ID";
            this.dataGridView9.Columns[1].HeaderCell.Value = "时间";
            this.dataGridView9.Columns[2].HeaderCell.Value = "列车管前端";
            this.dataGridView9.Columns[3].HeaderCell.Value = "副风缸";
            this.dataGridView9.Columns[4].HeaderCell.Value = "制动缸";
            this.dataGridView9.Columns[5].HeaderCell.Value = "列车管尾端";
            this.dataGridView9.Columns[6].HeaderCell.Value = "加缓风缸";
            foreach (DataGridViewColumn item in dataGridView9.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            //添加列
            this.dataGridView10.Columns[0].HeaderCell.Value = "ID";
            this.dataGridView10.Columns[1].HeaderCell.Value = "时间";
            this.dataGridView10.Columns[2].HeaderCell.Value = "列车管前端";
            this.dataGridView10.Columns[3].HeaderCell.Value = "副风缸";
            this.dataGridView10.Columns[4].HeaderCell.Value = "制动缸";
            this.dataGridView10.Columns[5].HeaderCell.Value = "列车管尾端";
            this.dataGridView10.Columns[6].HeaderCell.Value = "加缓风缸";
            foreach (DataGridViewColumn item in dataGridView10.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            //添加列
            this.dataGridView11.Columns[0].HeaderCell.Value = "ID";
            this.dataGridView11.Columns[1].HeaderCell.Value = "时间";
            this.dataGridView11.Columns[2].HeaderCell.Value = "列车管前端";
            this.dataGridView11.Columns[3].HeaderCell.Value = "副风缸";
            this.dataGridView11.Columns[4].HeaderCell.Value = "制动缸";
            this.dataGridView11.Columns[5].HeaderCell.Value = "列车管尾端";
            this.dataGridView11.Columns[6].HeaderCell.Value = "加缓风缸";
            foreach (DataGridViewColumn item in dataGridView11.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            //添加列
            this.dataGridView12.Columns[0].HeaderCell.Value = "ID";
            this.dataGridView12.Columns[1].HeaderCell.Value = "时间";
            this.dataGridView12.Columns[2].HeaderCell.Value = "列车管前端";
            this.dataGridView12.Columns[3].HeaderCell.Value = "副风缸";
            this.dataGridView12.Columns[4].HeaderCell.Value = "制动缸";
            this.dataGridView12.Columns[5].HeaderCell.Value = "列车管尾端";
            this.dataGridView12.Columns[6].HeaderCell.Value = "加缓风缸";
            foreach (DataGridViewColumn item in dataGridView12.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            //添加列
            this.dataGridView13.Columns[0].HeaderCell.Value = "ID";
            this.dataGridView13.Columns[1].HeaderCell.Value = "时间";
            this.dataGridView13.Columns[2].HeaderCell.Value = "列车管前端";
            this.dataGridView13.Columns[3].HeaderCell.Value = "副风缸";
            this.dataGridView13.Columns[4].HeaderCell.Value = "制动缸";
            this.dataGridView13.Columns[5].HeaderCell.Value = "列车管尾端";
            this.dataGridView13.Columns[6].HeaderCell.Value = "加缓风缸";
            foreach (DataGridViewColumn item in dataGridView13.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            //添加列
            this.dataGridView14.Columns[0].HeaderCell.Value = "ID";
            this.dataGridView14.Columns[1].HeaderCell.Value = "时间";
            this.dataGridView14.Columns[2].HeaderCell.Value = "列车管前端";
            this.dataGridView14.Columns[3].HeaderCell.Value = "副风缸";
            this.dataGridView14.Columns[4].HeaderCell.Value = "制动缸";
            this.dataGridView14.Columns[5].HeaderCell.Value = "列车管尾端";
            this.dataGridView14.Columns[6].HeaderCell.Value = "加缓风缸";
            foreach (DataGridViewColumn item in dataGridView14.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            //添加列
            this.dataGridView15.Columns[0].HeaderCell.Value = "ID";
            this.dataGridView15.Columns[1].HeaderCell.Value = "时间";
            this.dataGridView15.Columns[2].HeaderCell.Value = "列车管前端";
            this.dataGridView15.Columns[3].HeaderCell.Value = "副风缸";
            this.dataGridView15.Columns[4].HeaderCell.Value = "制动缸";
            this.dataGridView15.Columns[5].HeaderCell.Value = "列车管尾端";
            this.dataGridView15.Columns[6].HeaderCell.Value = "加缓风缸";
            foreach (DataGridViewColumn item in dataGridView15.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            //添加列
            this.dataGridView16.Columns[0].HeaderCell.Value = "ID";
            this.dataGridView16.Columns[1].HeaderCell.Value = "时间";
            this.dataGridView16.Columns[2].HeaderCell.Value = "列车管前端";
            this.dataGridView16.Columns[3].HeaderCell.Value = "副风缸";
            this.dataGridView16.Columns[4].HeaderCell.Value = "制动缸";
            this.dataGridView16.Columns[5].HeaderCell.Value = "列车管尾端";
            this.dataGridView16.Columns[6].HeaderCell.Value = "加缓风缸";
            foreach (DataGridViewColumn item in dataGridView16.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            //添加列
            this.dataGridView17.Columns[0].HeaderCell.Value = "ID";
            this.dataGridView17.Columns[1].HeaderCell.Value = "时间";
            this.dataGridView17.Columns[2].HeaderCell.Value = "列车管前端";
            this.dataGridView17.Columns[3].HeaderCell.Value = "副风缸";
            this.dataGridView17.Columns[4].HeaderCell.Value = "制动缸";
            this.dataGridView17.Columns[5].HeaderCell.Value = "列车管尾端";
            this.dataGridView17.Columns[6].HeaderCell.Value = "加缓风缸";
            foreach (DataGridViewColumn item in dataGridView17.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            //添加列
            this.dataGridView18.Columns[0].HeaderCell.Value = "ID";
            this.dataGridView18.Columns[1].HeaderCell.Value = "时间";
            this.dataGridView18.Columns[2].HeaderCell.Value = "列车管前端";
            this.dataGridView18.Columns[3].HeaderCell.Value = "副风缸";
            this.dataGridView18.Columns[4].HeaderCell.Value = "制动缸";
            this.dataGridView18.Columns[5].HeaderCell.Value = "列车管尾端";
            this.dataGridView18.Columns[6].HeaderCell.Value = "加缓风缸";
            foreach (DataGridViewColumn item in dataGridView18.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            //添加列
            this.dataGridView19.Columns[0].HeaderCell.Value = "ID";
            this.dataGridView19.Columns[1].HeaderCell.Value = "时间";
            this.dataGridView19.Columns[2].HeaderCell.Value = "列车管前端";
            this.dataGridView19.Columns[3].HeaderCell.Value = "副风缸";
            this.dataGridView19.Columns[4].HeaderCell.Value = "制动缸";
            this.dataGridView19.Columns[5].HeaderCell.Value = "列车管尾端";
            this.dataGridView19.Columns[6].HeaderCell.Value = "加缓风缸";
            foreach (DataGridViewColumn item in dataGridView19.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            //添加列
            this.dataGridView20.Columns[0].HeaderCell.Value = "ID";
            this.dataGridView20.Columns[1].HeaderCell.Value = "时间";
            this.dataGridView20.Columns[2].HeaderCell.Value = "列车管前端";
            this.dataGridView20.Columns[3].HeaderCell.Value = "副风缸";
            this.dataGridView20.Columns[4].HeaderCell.Value = "制动缸";
            this.dataGridView20.Columns[5].HeaderCell.Value = "列车管尾端";
            this.dataGridView20.Columns[6].HeaderCell.Value = "加缓风缸";
            foreach (DataGridViewColumn item in dataGridView20.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

        }

        //画图委托
        private void deal(object socket)
        {
            //保持连接不中断（与下位机配合）
            Socket s = (Socket)socket;
            ListenDelegate Lin = new ListenDelegate(draw);
            SaveDelegate saveDel = new SaveDelegate(dataSave);

            while (true)
            {          
                if (receiveData10 < 5)
                {
                    this.Invoke(Lin, receiveData10, receiveData11, receiveData12, receiveData13, receiveData14, receiveData15);
                    this.Invoke(saveDel, receiveData10, receiveData11, receiveData12, receiveData13, receiveData14, receiveData15);

                }
                if (receiveData20 >= 5 && receiveData20 < 10)
                {
                    this.Invoke(Lin, receiveData20, receiveData21, receiveData22, receiveData23, receiveData24, receiveData25);
                    this.Invoke(saveDel, receiveData20, receiveData21, receiveData22, receiveData23, receiveData24, receiveData25);

                }
                if (receiveData30 >= 10 && receiveData30 < 15)
                {
                    this.Invoke(Lin, receiveData30, receiveData31, receiveData32, receiveData33, receiveData34, receiveData35);
                    this.Invoke(saveDel, receiveData30, receiveData31, receiveData32, receiveData33, receiveData34, receiveData35);

                }
                if (receiveData40 >= 15 && receiveData40 < 20)
                {
                    this.Invoke(Lin, receiveData40, receiveData41, receiveData42, receiveData43, receiveData44, receiveData45);
                    this.Invoke(saveDel, receiveData40, receiveData41, receiveData42, receiveData43, receiveData44, receiveData45);

                }
            }
        }

        //端口监听
        public void socketListen()
        {         
            Socket listenSocket1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket1.Bind(new IPEndPoint(IPAddress.Any, 8087));
            //Socket listenSocket2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //listenSocket2.Bind(new IPEndPoint(IPAddress.Any, 8088));
            //Socket listenSocket3 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //listenSocket3.Bind(new IPEndPoint(IPAddress.Any, 8089));
            //Socket listenSocket4 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //listenSocket4.Bind(new IPEndPoint(IPAddress.Any, 8090));

            listenSocket1.Listen(100);
            //listenSocket2.Listen(100);
            //listenSocket3.Listen(100);
            //listenSocket4.Listen(100);

            acceptSocket1 = listenSocket1.Accept();
            rtbLog.AppendText("8087端口连接成功！\n");
            //acceptSocket2 = listenSocket2.Accept();
            //rtbLog.AppendText("8088端口连接成功！\n");
            //acceptSocket3 = listenSocket3.Accept();
            //rtbLog.AppendText("8089端口连接成功！\n");
            //acceptSocket4 = listenSocket4.Accept();
            //rtbLog.AppendText("8090端口连接成功！\n");

            sendTCP("AB1501234\r\n", "AB1501234\r\n", "AB1501234\r\n", "AB1501234\r\n");//确保程序开始时下位机处于激活状态
            rtbLog.AppendText("\n开启所有车辆采集通道！");
            sendTCP("AB2050\r\n", "AB2050\r\n", "AB2050\r\n", "AB2050\r\n");

            recDataThread1 = new Thread(dataReceive1);
            recDataThread1.IsBackground = true;

            //recDataThread2 = new Thread(dataReceive2);
            //recDataThread2.IsBackground = true;

            //recDataThread3 = new Thread(dataReceive3);
            //recDataThread3.IsBackground = true;

            //recDataThread4 = new Thread(dataReceive4);
            //recDataThread4.IsBackground = true;

            recDataThread1.Start();
            //recDataThread2.Start();
            //recDataThread3.Start();
            //recDataThread4.Start();

            dealD = new Thread(deal);
            dealD.IsBackground = true;
            dealD.Start(acceptSocket1);
            //dealD.Start(acceptSocket2);
            //dealD.Start(acceptSocket3);
            //dealD.Start(acceptSocket4);

        }

        //数据处理
        private void dataDel()
        {
            if(!hjTag)
            {
                zdControlAgrol();
            }
            else
            {
                hjControlAgrol();
            }
        }

        //数据接收
        private void dataReceive1()
        {
            
            string[] arr = null;

            //按行读取
            NetworkStream ntwStream = new NetworkStream(acceptSocket1);
            StreamReader strmReader = new StreamReader(ntwStream);

            while (true)
            {
                Result1 = strmReader.ReadLine();
                Console.WriteLine(Result1);

                if (Result1 != null)
                {
                    arr = Result1.Split('/');

                    if (arr[0].Equals("A") && arr[1] != null && arr[2] != null && arr[3] != null && arr[4] != null && arr[5] != null && arr[6] != null)
                    {
                        receiveData10 = double.Parse(arr[1]);
                        receiveData11 = Math.Abs(250 * (double.Parse(arr[2]) / 3276.8) - 250);//0-1mPa,0-5V
                        receiveData12 = Math.Abs(100 * (double.Parse(arr[3]) / 3276.8));
                        receiveData13 = Math.Abs(100 * (double.Parse(arr[4]) / 3276.8));
                        receiveData14 = Math.Abs(100 * (double.Parse(arr[5]) / 3276.8));
                        receiveData15 = Math.Abs(100 * (double.Parse(arr[6]) / 3276.8));

                        if (receiveData10 == 0 && (zdTag == 1) && (receiveData14 >= double.Parse(cyzd_list_D4[0].ToString()) - zd_Relife))
                        {
                            dealDataThread = new Thread(dataDel);
                            dealDataThread.IsBackground = true;
                            dealDataThread.Start();
                        }
                        if(hjTag && receiveData10 == 0 && (zdTag == 0))
                        {
                            dealDataThread = new Thread(dataDel);
                            dealDataThread.IsBackground = true;
                            dealDataThread.Start();
                        }

                        //ListenDelegate Lin = new ListenDelegate(draw);
                        //this.Invoke(Lin, receiveData);

                        //SaveDelegate saveDel = new SaveDelegate(dataSave);
                        //this.Invoke(saveDel);
                    }
                    else
                    {
                        string str0, str1, str2, str3, str4, str5, str6;
                        if (arr[0] == null || arr[0] != "A")
                        {
                            str0 = "数据包字头错误！\n";
                        }
                        else
                        {
                            str0 = "数据包头正常！\n";
                        }
                        if (arr[1] == null)
                        {
                            str1 = "列车编号错误！\n";
                        }
                        else
                        {
                            str1 = "第" + int.Parse(arr[1]) + 1 + "辆车：\n";
                        }
                        if (arr[2] == null)
                        {
                            str2 = "通道1空采！\n";
                        }
                        else
                        {
                            str2 = "通道1正常！\n";
                        }
                        if (arr[3] == null)
                        {
                            str3 = "通道2空采！\n";
                        }
                        else
                        {
                            str3 = "通道2正常！\n";
                        }
                        if (arr[4] == null)
                        {
                            str4 = "通道3空采！\n";
                        }
                        else
                        {
                            str4 = "通道3正常！\n";
                        }
                        if (arr[5] == null)
                        {
                            str5 = "通道4空采！\n";
                        }
                        else
                        {
                            str5 = "通道4正常！\n";
                        }
                        if (arr[6] == null)
                        {
                            str6 = "通道5空采！";
                        }
                        else
                        {
                            str6 = "通道5正常！";
                        }
                        MessageBox.Show("端口8087采集到的数据格式错误\n" + str1 + "," + str0 + "," + str2 + "," + str3 + "," + str4 + "," + str5 + "," + str6);
                    }

                }
                else
                {
                    MessageBox.Show("端口8087没有采集到数据");
                }

            }
        }

        //板2数据接收
        private void dataReceive2()
        {
            string[] arr = null;

            //按行读取
            NetworkStream ntwStream = new NetworkStream(acceptSocket2);
            StreamReader strmReader = new StreamReader(ntwStream);

            while (true)
            {
                Result2 = strmReader.ReadLine();
                Console.WriteLine(Result2);

                if (Result2 != null)
                {
                    arr = Result2.Split('/');

                    if (arr[0].Equals("A") && arr[1] != null && arr[2] != null && arr[3] != null && arr[4] != null && arr[5] != null && arr[6] != null)
                    {
                        receiveData10 = double.Parse(arr[1])+5;
                        receiveData11 = Math.Abs(250 * (double.Parse(arr[2]) / 3276.8) - 250);//0-1mPa,0-5V
                        receiveData12 = Math.Abs(100 * (double.Parse(arr[3]) / 3276.8));
                        receiveData13 = Math.Abs(100 * (double.Parse(arr[4]) / 3276.8));
                        receiveData14 = Math.Abs(100 * (double.Parse(arr[5]) / 3276.8));
                        receiveData15 = Math.Abs(100 * (double.Parse(arr[6]) / 3276.8));

                        //ListenDelegate Lin = new ListenDelegate(draw);
                        //this.Invoke(Lin, receiveData);

                        //SaveDelegate saveDel = new SaveDelegate(dataSave);
                        //this.Invoke(saveDel);
                    }
                    else
                    {
                        string str0, str1, str2, str3, str4, str5, str6;
                        if (arr[0] == null || arr[0] != "A")
                        {
                            str0 = "数据包字头错误！\n";
                        }
                        else
                        {
                            str0 = "数据包头正常！\n";
                        }
                        if (arr[1] == null)
                        {
                            str1 = "列车编号错误！\n";
                        }
                        else
                        {
                            str1 = "第" + int.Parse(arr[1]) + 1 + "辆车：\n";
                        }
                        if (arr[2] == null)
                        {
                            str2 = "通道1空采！\n";
                        }
                        else
                        {
                            str2 = "通道1正常！\n";
                        }
                        if (arr[3] == null)
                        {
                            str3 = "通道2空采！\n";
                        }
                        else
                        {
                            str3 = "通道2正常！\n";
                        }
                        if (arr[4] == null)
                        {
                            str4 = "通道3空采！\n";
                        }
                        else
                        {
                            str4 = "通道3正常！\n";
                        }
                        if (arr[5] == null)
                        {
                            str5 = "通道4空采！\n";
                        }
                        else
                        {
                            str5 = "通道4正常！\n";
                        }
                        if (arr[6] == null)
                        {
                            str6 = "通道5空采！";
                        }
                        else
                        {
                            str6 = "通道5正常！";
                        }
                        MessageBox.Show("端口8087采集到的数据格式错误\n" + str1 + "," + str0 + "," + str2 + "," + str3 + "," + str4 + "," + str5 + "," + str6);
                    }

                }
                else
                {
                    MessageBox.Show("没有采集到数据");
                }

            }
        }

        //板2数据接收
        private void dataReceive3()
        {
            string[] arr = null;

            //按行读取
            NetworkStream ntwStream = new NetworkStream(acceptSocket3);
            StreamReader strmReader = new StreamReader(ntwStream);

            while (true)
            {
                Result3 = strmReader.ReadLine();
                Console.WriteLine(Result3);

                if (Result3 != null)
                {
                    arr = Result3.Split('/');

                    if (arr[0].Equals("A") && arr[1] != null && arr[2] != null && arr[3] != null && arr[4] != null && arr[5] != null && arr[6] != null)
                    {
                        receiveData10 = double.Parse(arr[1]) + 10;
                        receiveData11 = Math.Abs(250 * (double.Parse(arr[2]) / 3276.8) - 250);//0-1mPa,0-5V
                        receiveData12 = Math.Abs(100 * (double.Parse(arr[3]) / 3276.8));
                        receiveData13 = Math.Abs(100 * (double.Parse(arr[4]) / 3276.8));
                        receiveData14 = Math.Abs(100 * (double.Parse(arr[5]) / 3276.8));
                        receiveData15 = Math.Abs(100 * (double.Parse(arr[6]) / 3276.8));

                        //ListenDelegate Lin = new ListenDelegate(draw);
                        //this.Invoke(Lin, receiveData);

                        //SaveDelegate saveDel = new SaveDelegate(dataSave);
                        //this.Invoke(saveDel);
                    }
                    else
                    {
                        string str0, str1, str2, str3, str4, str5, str6;
                        if (arr[0] == null || arr[0] != "A")
                        {
                            str0 = "数据包字头错误！\n";
                        }
                        else
                        {
                            str0 = "数据包头正常！\n";
                        }
                        if (arr[1] == null)
                        {
                            str1 = "列车编号错误！\n";
                        }
                        else
                        {
                            str1 = "第" + int.Parse(arr[1]) + 1 + "辆车：\n";
                        }
                        if (arr[2] == null)
                        {
                            str2 = "通道1空采！\n";
                        }
                        else
                        {
                            str2 = "通道1正常！\n";
                        }
                        if (arr[3] == null)
                        {
                            str3 = "通道2空采！\n";
                        }
                        else
                        {
                            str3 = "通道2正常！\n";
                        }
                        if (arr[4] == null)
                        {
                            str4 = "通道3空采！\n";
                        }
                        else
                        {
                            str4 = "通道3正常！\n";
                        }
                        if (arr[5] == null)
                        {
                            str5 = "通道4空采！\n";
                        }
                        else
                        {
                            str5 = "通道4正常！\n";
                        }
                        if (arr[6] == null)
                        {
                            str6 = "通道5空采！";
                        }
                        else
                        {
                            str6 = "通道5正常！";
                        }
                        MessageBox.Show("端口8087采集到的数据格式错误\n" + str1 + "," + str0 + "," + str2 + "," + str3 + "," + str4 + "," + str5 + "," + str6);
                    }

                }
                else
                {
                    MessageBox.Show("没有采集到数据");
                }

            }
        }

        //板2数据接收
        private void dataReceive4()
        {
            string[] arr = null;

            //按行读取
            NetworkStream ntwStream = new NetworkStream(acceptSocket2);
            StreamReader strmReader = new StreamReader(ntwStream);

            while (true)
            {
                Result2 = strmReader.ReadLine();
                Console.WriteLine(Result2);

                if (Result2 != null)
                {
                    arr = Result2.Split('/');

                    if (arr[0].Equals("A") && arr[1] != null && arr[2] != null && arr[3] != null && arr[4] != null && arr[5] != null && arr[6] != null)
                    {
                        receiveData10 = double.Parse(arr[1]) + 15;
                        receiveData11 = Math.Abs(250 * (double.Parse(arr[2]) / 3276.8) - 250);//0-1mPa,0-5V
                        receiveData12 = Math.Abs(100 * (double.Parse(arr[3]) / 3276.8));
                        receiveData13 = Math.Abs(100 * (double.Parse(arr[4]) / 3276.8));
                        receiveData14 = Math.Abs(100 * (double.Parse(arr[5]) / 3276.8));
                        receiveData15 = Math.Abs(100 * (double.Parse(arr[6]) / 3276.8));

                        //ListenDelegate Lin = new ListenDelegate(draw);
                        //this.Invoke(Lin, receiveData);

                        //SaveDelegate saveDel = new SaveDelegate(dataSave);
                        //this.Invoke(saveDel);
                    }
                    else
                    {
                        string str0, str1, str2, str3, str4, str5, str6;
                        if (arr[0] == null || arr[0] != "A")
                        {
                            str0 = "数据包字头错误！\n";
                        }
                        else
                        {
                            str0 = "数据包头正常！\n";
                        }
                        if (arr[1] == null)
                        {
                            str1 = "列车编号错误！\n";
                        }
                        else
                        {
                            str1 = "第" + int.Parse(arr[1]) + 1 + "辆车：\n";
                        }
                        if (arr[2] == null)
                        {
                            str2 = "通道1空采！\n";
                        }
                        else
                        {
                            str2 = "通道1正常！\n";
                        }
                        if (arr[3] == null)
                        {
                            str3 = "通道2空采！\n";
                        }
                        else
                        {
                            str3 = "通道2正常！\n";
                        }
                        if (arr[4] == null)
                        {
                            str4 = "通道3空采！\n";
                        }
                        else
                        {
                            str4 = "通道3正常！\n";
                        }
                        if (arr[5] == null)
                        {
                            str5 = "通道4空采！\n";
                        }
                        else
                        {
                            str5 = "通道4正常！\n";
                        }
                        if (arr[6] == null)
                        {
                            str6 = "通道5空采！";
                        }
                        else
                        {
                            str6 = "通道5正常！";
                        }
                        MessageBox.Show("端口8087采集到的数据格式错误\n" + str1 + "," + str0 + "," + str2 + "," + str3 + "," + str4 + "," + str5 + "," + str6);
                    }

                }
                else
                {
                    MessageBox.Show("没有采集到数据");
                }

            }
        }

        //数据存储
        public void dataSave(double data0, double data1, double data2, double data3, double data4, double data5)
        {
            if (receiveData10 == 0)
            {
                dataNum1++;

                //添加行
                int index = this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[index].Cells[0].Value = dataNum1;
                this.dataGridView1.Rows[index].Cells[1].Value = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
                this.dataGridView1.Rows[index].Cells[2].Value = double.Parse(string.Format("{0:F}", receiveData11));
                this.dataGridView1.Rows[index].Cells[3].Value = double.Parse(string.Format("{0:F}", receiveData12));
                this.dataGridView1.Rows[index].Cells[4].Value = double.Parse(string.Format("{0:F}", receiveData13));
                this.dataGridView1.Rows[index].Cells[5].Value = double.Parse(string.Format("{0:F}", receiveData14));
                this.dataGridView1.Rows[index].Cells[6].Value = double.Parse(string.Format("{0:F}", receiveData15));
            }
            else if (receiveData10 == 1)
            {
                dataNum2++;
                //添加行
                int index = this.dataGridView2.Rows.Add();
                this.dataGridView2.Rows[index].Cells[0].Value = dataNum2;
                this.dataGridView2.Rows[index].Cells[1].Value = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
                this.dataGridView2.Rows[index].Cells[2].Value = double.Parse(string.Format("{0:F}", receiveData11));
                this.dataGridView2.Rows[index].Cells[3].Value = double.Parse(string.Format("{0:F}", receiveData12));
                this.dataGridView2.Rows[index].Cells[4].Value = double.Parse(string.Format("{0:F}", receiveData13));
                this.dataGridView2.Rows[index].Cells[5].Value = double.Parse(string.Format("{0:F}", receiveData14));
                this.dataGridView2.Rows[index].Cells[6].Value = double.Parse(string.Format("{0:F}", receiveData15));
            }
            else if (receiveData10 == 2)
            {
                dataNum3++;
                //添加行
                int index = this.dataGridView3.Rows.Add();
                this.dataGridView3.Rows[index].Cells[0].Value = dataNum3;
                this.dataGridView3.Rows[index].Cells[1].Value = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
                this.dataGridView3.Rows[index].Cells[2].Value = double.Parse(string.Format("{0:F}", receiveData11));
                this.dataGridView3.Rows[index].Cells[3].Value = double.Parse(string.Format("{0:F}", receiveData12));
                this.dataGridView3.Rows[index].Cells[4].Value = double.Parse(string.Format("{0:F}", receiveData13));
                this.dataGridView3.Rows[index].Cells[5].Value = double.Parse(string.Format("{0:F}", receiveData14));
                this.dataGridView3.Rows[index].Cells[6].Value = double.Parse(string.Format("{0:F}", receiveData15));
            }
            else if (receiveData10 == 3)
            {
                dataNum4++;
                //添加行
                int index = this.dataGridView4.Rows.Add();
                this.dataGridView4.Rows[index].Cells[0].Value = dataNum4;
                this.dataGridView4.Rows[index].Cells[1].Value = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
                this.dataGridView4.Rows[index].Cells[2].Value = double.Parse(string.Format("{0:F}", receiveData11));
                this.dataGridView4.Rows[index].Cells[3].Value = double.Parse(string.Format("{0:F}", receiveData12));
                this.dataGridView4.Rows[index].Cells[4].Value = double.Parse(string.Format("{0:F}", receiveData13));
                this.dataGridView4.Rows[index].Cells[5].Value = double.Parse(string.Format("{0:F}", receiveData14));
                this.dataGridView4.Rows[index].Cells[6].Value = double.Parse(string.Format("{0:F}", receiveData15));
            }
            else if (receiveData10 == 4)
            {
                dataNum5++;
                //添加行
                int index = this.dataGridView5.Rows.Add();
                this.dataGridView5.Rows[index].Cells[0].Value = dataNum5;
                this.dataGridView5.Rows[index].Cells[1].Value = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
                this.dataGridView5.Rows[index].Cells[2].Value = double.Parse(string.Format("{0:F}", receiveData11));
                this.dataGridView5.Rows[index].Cells[3].Value = double.Parse(string.Format("{0:F}", receiveData12));
                this.dataGridView5.Rows[index].Cells[4].Value = double.Parse(string.Format("{0:F}", receiveData13));
                this.dataGridView5.Rows[index].Cells[5].Value = double.Parse(string.Format("{0:F}", receiveData14));
                this.dataGridView5.Rows[index].Cells[6].Value = double.Parse(string.Format("{0:F}", receiveData15));
            }
            else if (receiveData20 == 5)
            {
                dataNum6++;
                //添加行
                int index = this.dataGridView6.Rows.Add();
                this.dataGridView6.Rows[index].Cells[0].Value = dataNum6;
                this.dataGridView6.Rows[index].Cells[1].Value = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
                this.dataGridView6.Rows[index].Cells[2].Value = double.Parse(string.Format("{0:F}", receiveData21));
                this.dataGridView6.Rows[index].Cells[3].Value = double.Parse(string.Format("{0:F}", receiveData22));
                this.dataGridView6.Rows[index].Cells[4].Value = double.Parse(string.Format("{0:F}", receiveData23));
                this.dataGridView6.Rows[index].Cells[5].Value = double.Parse(string.Format("{0:F}", receiveData24));
                this.dataGridView6.Rows[index].Cells[6].Value = double.Parse(string.Format("{0:F}", receiveData25));
            }
            else if (receiveData20 == 6)
            {
                dataNum7++;
                //添加行
                int index = this.dataGridView7.Rows.Add();
                this.dataGridView7.Rows[index].Cells[0].Value = dataNum7;
                this.dataGridView7.Rows[index].Cells[1].Value = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
                this.dataGridView7.Rows[index].Cells[2].Value = double.Parse(string.Format("{0:F}", receiveData21));
                this.dataGridView7.Rows[index].Cells[3].Value = double.Parse(string.Format("{0:F}", receiveData22));
                this.dataGridView7.Rows[index].Cells[4].Value = double.Parse(string.Format("{0:F}", receiveData23));
                this.dataGridView7.Rows[index].Cells[5].Value = double.Parse(string.Format("{0:F}", receiveData24));
                this.dataGridView7.Rows[index].Cells[6].Value = double.Parse(string.Format("{0:F}", receiveData25));
            }
            else if (receiveData20 == 7)
            {
                dataNum8++;
                //添加行
                int index = this.dataGridView8.Rows.Add();
                this.dataGridView8.Rows[index].Cells[0].Value = dataNum8;
                this.dataGridView8.Rows[index].Cells[1].Value = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
                this.dataGridView8.Rows[index].Cells[2].Value = double.Parse(string.Format("{0:F}", receiveData21));
                this.dataGridView8.Rows[index].Cells[3].Value = double.Parse(string.Format("{0:F}", receiveData22));
                this.dataGridView8.Rows[index].Cells[4].Value = double.Parse(string.Format("{0:F}", receiveData23));
                this.dataGridView8.Rows[index].Cells[5].Value = double.Parse(string.Format("{0:F}", receiveData24));
                this.dataGridView8.Rows[index].Cells[6].Value = double.Parse(string.Format("{0:F}", receiveData25));
            }
            else if (receiveData20 == 8)
            {
                dataNum9++;
                //添加行9
                int index = this.dataGridView1.Rows.Add();
                this.dataGridView9.Rows[index].Cells[0].Value = dataNum9;
                this.dataGridView9.Rows[index].Cells[1].Value = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
                this.dataGridView9.Rows[index].Cells[2].Value = double.Parse(string.Format("{0:F}", receiveData21));
                this.dataGridView9.Rows[index].Cells[3].Value = double.Parse(string.Format("{0:F}", receiveData22));
                this.dataGridView9.Rows[index].Cells[4].Value = double.Parse(string.Format("{0:F}", receiveData23));
                this.dataGridView9.Rows[index].Cells[5].Value = double.Parse(string.Format("{0:F}", receiveData24));
                this.dataGridView9.Rows[index].Cells[6].Value = double.Parse(string.Format("{0:F}", receiveData25));
            }
            else if (receiveData20 == 9)
            {
                dataNum10++;
                //添加行
                int index = this.dataGridView10.Rows.Add();
                this.dataGridView10.Rows[index].Cells[0].Value = dataNum10;
                this.dataGridView10.Rows[index].Cells[1].Value = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
                this.dataGridView10.Rows[index].Cells[2].Value = double.Parse(string.Format("{0:F}", receiveData21));
                this.dataGridView10.Rows[index].Cells[3].Value = double.Parse(string.Format("{0:F}", receiveData22));
                this.dataGridView10.Rows[index].Cells[4].Value = double.Parse(string.Format("{0:F}", receiveData23));
                this.dataGridView10.Rows[index].Cells[5].Value = double.Parse(string.Format("{0:F}", receiveData24));
                this.dataGridView10.Rows[index].Cells[6].Value = double.Parse(string.Format("{0:F}", receiveData25));
            }
            else if (receiveData30 == 10)
            {
                dataNum11++;
                //添加行
                int index = this.dataGridView11.Rows.Add();
                this.dataGridView11.Rows[index].Cells[0].Value = dataNum11;
                this.dataGridView11.Rows[index].Cells[1].Value = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
                this.dataGridView11.Rows[index].Cells[2].Value = double.Parse(string.Format("{0:F}", receiveData31));
                this.dataGridView11.Rows[index].Cells[3].Value = double.Parse(string.Format("{0:F}", receiveData32));
                this.dataGridView11.Rows[index].Cells[4].Value = double.Parse(string.Format("{0:F}", receiveData33));
                this.dataGridView11.Rows[index].Cells[5].Value = double.Parse(string.Format("{0:F}", receiveData34));
                this.dataGridView11.Rows[index].Cells[6].Value = double.Parse(string.Format("{0:F}", receiveData35));
            }
            else if (receiveData30 == 11)
            {
                dataNum12++;
                //添加行
                int index = this.dataGridView12.Rows.Add();
                this.dataGridView12.Rows[index].Cells[0].Value = dataNum12;
                this.dataGridView12.Rows[index].Cells[1].Value = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
                this.dataGridView12.Rows[index].Cells[2].Value = double.Parse(string.Format("{0:F}", receiveData31));
                this.dataGridView12.Rows[index].Cells[3].Value = double.Parse(string.Format("{0:F}", receiveData32));
                this.dataGridView12.Rows[index].Cells[4].Value = double.Parse(string.Format("{0:F}", receiveData33));
                this.dataGridView12.Rows[index].Cells[5].Value = double.Parse(string.Format("{0:F}", receiveData34));
                this.dataGridView12.Rows[index].Cells[6].Value = double.Parse(string.Format("{0:F}", receiveData35));
            }
            else if (receiveData30 == 12)
            {
                dataNum13++;
                //添加行
                int index = this.dataGridView13.Rows.Add();
                this.dataGridView13.Rows[index].Cells[0].Value = dataNum13;
                this.dataGridView13.Rows[index].Cells[1].Value = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
                this.dataGridView13.Rows[index].Cells[2].Value = double.Parse(string.Format("{0:F}", receiveData31));
                this.dataGridView13.Rows[index].Cells[3].Value = double.Parse(string.Format("{0:F}", receiveData32));
                this.dataGridView13.Rows[index].Cells[4].Value = double.Parse(string.Format("{0:F}", receiveData33));
                this.dataGridView13.Rows[index].Cells[5].Value = double.Parse(string.Format("{0:F}", receiveData34));
                this.dataGridView13.Rows[index].Cells[6].Value = double.Parse(string.Format("{0:F}", receiveData35));
            }
            else if (receiveData30 == 13)
            {
                dataNum14++;
                //添加行
                int index = this.dataGridView14.Rows.Add();
                this.dataGridView14.Rows[index].Cells[0].Value = dataNum14;
                this.dataGridView14.Rows[index].Cells[1].Value = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
                this.dataGridView14.Rows[index].Cells[2].Value = double.Parse(string.Format("{0:F}", receiveData31));
                this.dataGridView14.Rows[index].Cells[3].Value = double.Parse(string.Format("{0:F}", receiveData32));
                this.dataGridView14.Rows[index].Cells[4].Value = double.Parse(string.Format("{0:F}", receiveData33));
                this.dataGridView14.Rows[index].Cells[5].Value = double.Parse(string.Format("{0:F}", receiveData34));
                this.dataGridView14.Rows[index].Cells[6].Value = double.Parse(string.Format("{0:F}", receiveData35));
            }
            else if (receiveData30 == 14)
            {
                dataNum15++;
                //添加行
                int index = this.dataGridView15.Rows.Add();
                this.dataGridView15.Rows[index].Cells[0].Value = dataNum15;
                this.dataGridView15.Rows[index].Cells[1].Value = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
                this.dataGridView15.Rows[index].Cells[2].Value = double.Parse(string.Format("{0:F}", receiveData31));
                this.dataGridView15.Rows[index].Cells[3].Value = double.Parse(string.Format("{0:F}", receiveData32));
                this.dataGridView15.Rows[index].Cells[4].Value = double.Parse(string.Format("{0:F}", receiveData33));
                this.dataGridView15.Rows[index].Cells[5].Value = double.Parse(string.Format("{0:F}", receiveData34));
                this.dataGridView15.Rows[index].Cells[6].Value = double.Parse(string.Format("{0:F}", receiveData35));
            }
            else if (receiveData40 == 15)
            {
                dataNum16++;
                //添加行
                int index = this.dataGridView16.Rows.Add();
                this.dataGridView16.Rows[index].Cells[0].Value = dataNum16;
                this.dataGridView16.Rows[index].Cells[1].Value = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
                this.dataGridView16.Rows[index].Cells[2].Value = double.Parse(string.Format("{0:F}", receiveData41));
                this.dataGridView16.Rows[index].Cells[3].Value = double.Parse(string.Format("{0:F}", receiveData42));
                this.dataGridView16.Rows[index].Cells[4].Value = double.Parse(string.Format("{0:F}", receiveData43));
                this.dataGridView16.Rows[index].Cells[5].Value = double.Parse(string.Format("{0:F}", receiveData44));
                this.dataGridView16.Rows[index].Cells[6].Value = double.Parse(string.Format("{0:F}", receiveData45));
            }
            else if (receiveData40 == 16)
            {
                dataNum17++;
                //添加行
                int index = this.dataGridView17.Rows.Add();
                this.dataGridView17.Rows[index].Cells[0].Value = dataNum17;
                this.dataGridView17.Rows[index].Cells[1].Value = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
                this.dataGridView17.Rows[index].Cells[2].Value = double.Parse(string.Format("{0:F}", receiveData41));
                this.dataGridView17.Rows[index].Cells[3].Value = double.Parse(string.Format("{0:F}", receiveData42));
                this.dataGridView17.Rows[index].Cells[4].Value = double.Parse(string.Format("{0:F}", receiveData43));
                this.dataGridView17.Rows[index].Cells[5].Value = double.Parse(string.Format("{0:F}", receiveData44));
                this.dataGridView17.Rows[index].Cells[6].Value = double.Parse(string.Format("{0:F}", receiveData45));
            }
            else if (receiveData40 == 17)
            {
                dataNum18++;
                //添加行
                int index = this.dataGridView1.Rows.Add();
                this.dataGridView18.Rows[index].Cells[0].Value = dataNum18;
                this.dataGridView18.Rows[index].Cells[1].Value = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
                this.dataGridView18.Rows[index].Cells[2].Value = double.Parse(string.Format("{0:F}", receiveData41));
                this.dataGridView18.Rows[index].Cells[3].Value = double.Parse(string.Format("{0:F}", receiveData42));
                this.dataGridView18.Rows[index].Cells[4].Value = double.Parse(string.Format("{0:F}", receiveData43));
                this.dataGridView18.Rows[index].Cells[5].Value = double.Parse(string.Format("{0:F}", receiveData44));
                this.dataGridView18.Rows[index].Cells[6].Value = double.Parse(string.Format("{0:F}", receiveData45));
            }
            else if (receiveData40 == 18)
            {
                dataNum19++;
                //添加行
                int index = this.dataGridView18.Rows.Add();
                this.dataGridView19.Rows[index].Cells[0].Value = dataNum19;
                this.dataGridView19.Rows[index].Cells[1].Value = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
                this.dataGridView19.Rows[index].Cells[2].Value = double.Parse(string.Format("{0:F}", receiveData41));
                this.dataGridView19.Rows[index].Cells[3].Value = double.Parse(string.Format("{0:F}", receiveData42));
                this.dataGridView19.Rows[index].Cells[4].Value = double.Parse(string.Format("{0:F}", receiveData43));
                this.dataGridView19.Rows[index].Cells[5].Value = double.Parse(string.Format("{0:F}", receiveData44));
                this.dataGridView19.Rows[index].Cells[6].Value = double.Parse(string.Format("{0:F}", receiveData45));
            }

            else if (receiveData40 == 19)
            {
                dataNum20++;
                //添加行
                int index = this.dataGridView20.Rows.Add();
                this.dataGridView20.Rows[index].Cells[0].Value = dataNum20;
                this.dataGridView20.Rows[index].Cells[1].Value = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
                this.dataGridView20.Rows[index].Cells[2].Value = double.Parse(string.Format("{0:F}", receiveData41));
                this.dataGridView20.Rows[index].Cells[3].Value = double.Parse(string.Format("{0:F}", receiveData42));
                this.dataGridView20.Rows[index].Cells[4].Value = double.Parse(string.Format("{0:F}", receiveData43));
                this.dataGridView20.Rows[index].Cells[5].Value = double.Parse(string.Format("{0:F}", receiveData44));
                this.dataGridView20.Rows[index].Cells[6].Value = double.Parse(string.Format("{0:F}", receiveData45));
            }

        }

        //UI画图
        public void draw(double data0, double data1, double data2, double data3, double data4, double data5)
        {
            //  
            if (num == 1)
            {
                series10 = chartDemo1.Series[0];
                series11 = chartDemo1.Series[1];
                series12 = chartDemo1.Series[2];
                series13 = chartDemo1.Series[3];
                series14 = chartDemo1.Series[4];
                series10.LegendText = "列车管前端";
                series11.LegendText = "副风缸";
                series12.LegendText = "制动缸";
                series13.LegendText = "列车管尾端";
                series14.LegendText = "加缓风缸";

                series20 = chartDemo2.Series[0];
                series21 = chartDemo2.Series[1];
                series22 = chartDemo2.Series[2];
                series23 = chartDemo2.Series[3];
                series24 = chartDemo2.Series[4];
                series20.LegendText = "列车管前端";
                series21.LegendText = "副风缸";
                series22.LegendText = "制动缸";
                series23.LegendText = "列车管尾端";
                series24.LegendText = "加缓风缸";

                series30 = chartDemo3.Series[0];
                series31 = chartDemo3.Series[1];
                series32 = chartDemo3.Series[2];
                series33 = chartDemo3.Series[3];
                series34 = chartDemo3.Series[4];
                series30.LegendText = "列车管前端";
                series31.LegendText = "副风缸";
                series32.LegendText = "制动缸";
                series33.LegendText = "列车管尾端";
                series34.LegendText = "加缓风缸";

                series40 = chartDemo4.Series[0];
                series41 = chartDemo4.Series[1];
                series42 = chartDemo4.Series[2];
                series43 = chartDemo4.Series[3];
                series44 = chartDemo4.Series[4];
                series40.LegendText = "列车管前端";
                series41.LegendText = "副风缸";
                series42.LegendText = "制动缸";
                series43.LegendText = "列车管尾端";
                series44.LegendText = "加缓风缸";

                series50 = chartDemo5.Series[0];
                series51 = chartDemo5.Series[1];
                series52 = chartDemo5.Series[2];
                series53 = chartDemo5.Series[3];
                series54 = chartDemo5.Series[4];
                series50.LegendText = "列车管前端";
                series51.LegendText = "副风缸";
                series52.LegendText = "制动缸";
                series53.LegendText = "列车管尾端";
                series54.LegendText = "加缓风缸";

                series60 = chartDemo6.Series[0];
                series61 = chartDemo6.Series[1];
                series62 = chartDemo6.Series[2];
                series63 = chartDemo6.Series[3];
                series64 = chartDemo6.Series[4];
                series60.LegendText = "列车管前端";
                series61.LegendText = "副风缸";
                series62.LegendText = "制动缸";
                series63.LegendText = "列车管尾端";
                series64.LegendText = "加缓风缸";

                series70 = chartDemo7.Series[0];
                series71 = chartDemo7.Series[1];
                series72 = chartDemo7.Series[2];
                series73 = chartDemo7.Series[3];
                series74 = chartDemo7.Series[4];
                series70.LegendText = "列车管前端";
                series71.LegendText = "副风缸";
                series72.LegendText = "制动缸";
                series73.LegendText = "列车管尾端";
                series74.LegendText = "加缓风缸";

                series80 = chartDemo8.Series[0];
                series81 = chartDemo8.Series[1];
                series82 = chartDemo8.Series[2];
                series83 = chartDemo8.Series[3];
                series84 = chartDemo8.Series[4];
                series80.LegendText = "列车管前端";
                series81.LegendText = "副风缸";
                series82.LegendText = "制动缸";
                series83.LegendText = "列车管尾端";
                series84.LegendText = "加缓风缸";

                series90 = chartDemo9.Series[0];
                series91 = chartDemo9.Series[1];
                series92 = chartDemo9.Series[2];
                series93 = chartDemo9.Series[3];
                series94 = chartDemo9.Series[4];
                series90.LegendText = "列车管前端";
                series91.LegendText = "副风缸";
                series92.LegendText = "制动缸";
                series93.LegendText = "列车管尾端";
                series94.LegendText = "加缓风缸";

                series100 = chartDemo10.Series[0];
                series101 = chartDemo10.Series[1];
                series102 = chartDemo10.Series[2];
                series103 = chartDemo10.Series[3];
                series104 = chartDemo10.Series[4];
                series100.LegendText = "列车管前端";
                series101.LegendText = "副风缸";
                series102.LegendText = "制动缸";
                series103.LegendText = "列车管尾端";
                series104.LegendText = "加缓风缸";

                series110 = chartDemo11.Series[0];
                series111 = chartDemo11.Series[1];
                series112 = chartDemo11.Series[2];
                series113 = chartDemo11.Series[3];
                series114 = chartDemo11.Series[4];
                series110.LegendText = "列车管前端";
                series111.LegendText = "副风缸";
                series112.LegendText = "制动缸";
                series113.LegendText = "列车管尾端";
                series114.LegendText = "加缓风缸";

                series120 = chartDemo12.Series[0];
                series121 = chartDemo12.Series[1];
                series122 = chartDemo12.Series[2];
                series123 = chartDemo12.Series[3];
                series124 = chartDemo12.Series[4];
                series120.LegendText = "列车管前端";
                series121.LegendText = "副风缸";
                series122.LegendText = "制动缸";
                series123.LegendText = "列车管尾端";
                series124.LegendText = "加缓风缸";

                series130 = chartDemo13.Series[0];
                series131 = chartDemo13.Series[1];
                series132 = chartDemo13.Series[2];
                series133 = chartDemo13.Series[3];
                series134 = chartDemo13.Series[4];
                series130.LegendText = "列车管前端";
                series131.LegendText = "副风缸";
                series132.LegendText = "制动缸";
                series133.LegendText = "列车管尾端";
                series134.LegendText = "加缓风缸";

                series140 = chartDemo14.Series[0];
                series141 = chartDemo14.Series[1];
                series142 = chartDemo14.Series[2];
                series143 = chartDemo14.Series[3];
                series144 = chartDemo14.Series[4];
                series140.LegendText = "列车管前端";
                series141.LegendText = "副风缸";
                series142.LegendText = "制动缸";
                series143.LegendText = "列车管尾端";
                series144.LegendText = "加缓风缸";

                series150 = chartDemo15.Series[0];
                series151 = chartDemo15.Series[1];
                series152 = chartDemo15.Series[2];
                series153 = chartDemo15.Series[3];
                series154 = chartDemo15.Series[4];
                series150.LegendText = "列车管前端";
                series151.LegendText = "副风缸";
                series152.LegendText = "制动缸";
                series153.LegendText = "列车管尾端";
                series154.LegendText = "加缓风缸";

                series160 = chartDemo16.Series[0];
                series161 = chartDemo16.Series[1];
                series162 = chartDemo16.Series[2];
                series163 = chartDemo16.Series[3];
                series164 = chartDemo16.Series[4];
                series160.LegendText = "列车管前端";
                series161.LegendText = "副风缸";
                series162.LegendText = "制动缸";
                series163.LegendText = "列车管尾端";
                series164.LegendText = "加缓风缸";

                series170 = chartDemo17.Series[0];
                series171 = chartDemo17.Series[1];
                series172 = chartDemo17.Series[2];
                series173 = chartDemo17.Series[3];
                series174 = chartDemo17.Series[4];
                series170.LegendText = "列车管前端";
                series171.LegendText = "副风缸";
                series172.LegendText = "制动缸";
                series173.LegendText = "列车管尾端";
                series174.LegendText = "加缓风缸";

                series180 = chartDemo18.Series[0];
                series181 = chartDemo18.Series[1];
                series182 = chartDemo18.Series[2];
                series183 = chartDemo18.Series[3];
                series184 = chartDemo18.Series[4];
                series180.LegendText = "列车管前端";
                series181.LegendText = "副风缸";
                series182.LegendText = "制动缸";
                series183.LegendText = "列车管尾端";
                series184.LegendText = "加缓风缸";

                series190 = chartDemo19.Series[0];
                series191 = chartDemo19.Series[1];
                series192 = chartDemo19.Series[2];
                series193 = chartDemo19.Series[3];
                series194 = chartDemo19.Series[4];
                series190.LegendText = "列车管前端";
                series191.LegendText = "副风缸";
                series192.LegendText = "制动缸";
                series193.LegendText = "列车管尾端";
                series194.LegendText = "加缓风缸";

                series200 = chartDemo20.Series[0];
                series201 = chartDemo20.Series[1];
                series202 = chartDemo20.Series[2];
                series203 = chartDemo20.Series[3];
                series204 = chartDemo20.Series[4];
                series200.LegendText = "列车管前端";
                series201.LegendText = "副风缸";
                series202.LegendText = "制动缸";
                series203.LegendText = "列车管尾端";
                series204.LegendText = "加缓风缸";

                starttime = DateTime.Now;
                num = 2;
            }

            endtime = DateTime.Now;
            double dc = ExecDateDiff(starttime, endtime);
            xdc = (dc / 1000) + xdcTemp2;

            //DealTime = DateTime.Now;
            //delta_T = (DealTime - ExeTime - (starttime - ExeTime)).TotalSeconds;
            //lcg_modelFun(delta_T);
            //ffg_modelFun(delta_T);
            //controlAlgor();//调用控制算法

            if (receiveData10 == 0)
            {
                num++;

                //Series series10 = chartDemo1.Series[0];
                //Series series11 = chartDemo1.Series[1];
                //Series series12 = chartDemo1.Series[2];
                //Series series13 = chartDemo1.Series[3];
                //Series series14 = chartDemo1.Series[4];

                //Series series15 = chartDemo1.Series[5];
                //画点
                series10.Points.AddXY(xdc, receiveData11);
                series11.Points.AddXY(xdc, receiveData12);
                series12.Points.AddXY(xdc, receiveData13);
                series13.Points.AddXY(xdc, receiveData14);
                series14.Points.AddXY(xdc, receiveData15);

                //series15.Points.AddXY(xdc, lcg_P - 100);

                if (xdc >= 400)
                {
                    chartDemo1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    chartDemo1.ChartAreas[0].AxisX.ScaleView.Position = xdc - Xval;
                }
                Console.WriteLine("draw             " + num);
            }

            //对tabControl中的chartDemo2画图
            else if (receiveData10 == 1)
            {
                //Series series20 = chartDemo2.Series[0];
                //Series series21 = chartDemo2.Series[1];
                //Series series22 = chartDemo2.Series[2];
                //Series series23 = chartDemo2.Series[3];
                //Series series24 = chartDemo2.Series[4];
                //画点
                series20.Points.AddXY(xdc, receiveData11);
                series21.Points.AddXY(xdc, receiveData12);
                series22.Points.AddXY(xdc, receiveData13);
                series23.Points.AddXY(xdc, receiveData14);
                series24.Points.AddXY(xdc, receiveData15);

                if (xdc >= 400)
                {
                    //会有误差
                    chartDemo2.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    chartDemo2.ChartAreas[0].AxisX.ScaleView.Position = xdc - Xval;
                }
            }

            //对tabControl中的chartDemo3画图
            else if (receiveData10 == 2)
            {
                //Series series30 = chartDemo3.Series[0];
                //Series series31 = chartDemo3.Series[1];
                //Series series32 = chartDemo3.Series[2];
                //Series series33 = chartDemo3.Series[3];
                //Series series34 = chartDemo3.Series[4];
                //画点
                series30.Points.AddXY(xdc, receiveData11);
                series31.Points.AddXY(xdc, receiveData12);
                series32.Points.AddXY(xdc, receiveData13);
                series33.Points.AddXY(xdc, receiveData14);
                series34.Points.AddXY(xdc, receiveData15);

                if (xdc >= 400)
                {
                    //会有误差
                    chartDemo3.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    chartDemo3.ChartAreas[0].AxisX.ScaleView.Position = xdc - Xval;
                }

            }

            //对tabControl中的chartDemo4画图
            else if (receiveData10 == 3)
            {
                //Series series40 = chartDemo4.Series[0];
                //Series series41 = chartDemo4.Series[1];
                //Series series42 = chartDemo4.Series[2];
                //Series series43 = chartDemo4.Series[3];
                //Series series44 = chartDemo4.Series[4];
                //画点
                series40.Points.AddXY(xdc, receiveData11);
                series41.Points.AddXY(xdc, receiveData12);
                series42.Points.AddXY(xdc, receiveData13);
                series43.Points.AddXY(xdc, receiveData14);
                series44.Points.AddXY(xdc, receiveData15);

                if (xdc >= 400)
                {
                    //会有误差
                    chartDemo4.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    chartDemo4.ChartAreas[0].AxisX.ScaleView.Position = xdc - Xval;
                }

            }

            //对tabControl中的chartDemo5画图
            else if (receiveData10 == 4)
            {
                //Series series50 = chartDemo5.Series[0];
                //Series series51 = chartDemo5.Series[1];
                //Series series52 = chartDemo5.Series[2];
                //Series series53 = chartDemo5.Series[3];
                //Series series54 = chartDemo5.Series[4];
                //画点
                series50.Points.AddXY(xdc, receiveData11);
                series51.Points.AddXY(xdc, receiveData12);
                series52.Points.AddXY(xdc, receiveData13);
                series53.Points.AddXY(xdc, receiveData14);
                series54.Points.AddXY(xdc, receiveData15);

                if (xdc >= 400)
                {
                    //会有误差
                    chartDemo5.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    chartDemo5.ChartAreas[0].AxisX.ScaleView.Position = xdc - Xval;
                }

            }

            //对tabControl中的chartDemo6画图
            if (receiveData20 == 5)
            {
                //Series series60 = chartDemo6.Series[0];
                //Series series61 = chartDemo6.Series[1];
                //Series series62 = chartDemo6.Series[2];
                //Series series63 = chartDemo6.Series[3];
                //Series series64 = chartDemo6.Series[4];
                //画点
                series60.Points.AddXY(xdc, receiveData21);
                series61.Points.AddXY(xdc, receiveData22);
                series62.Points.AddXY(xdc, receiveData23);
                series63.Points.AddXY(xdc, receiveData24);
                series64.Points.AddXY(xdc, receiveData25);

                if (xdc >= 400)
                {
                    //会有误差
                    chartDemo6.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    chartDemo6.ChartAreas[0].AxisX.ScaleView.Position = xdc - Xval;
                }
            }

            //对tabControl中的chartDemo7画图
            else if (receiveData20 == 6)
            {
                //Series series70 = chartDemo7.Series[0];
                //Series series71 = chartDemo7.Series[1];
                //Series series72 = chartDemo7.Series[2];
                //Series series73 = chartDemo7.Series[3];
                //Series series74 = chartDemo7.Series[4];
                //画点
                series70.Points.AddXY(xdc, receiveData21);
                series71.Points.AddXY(xdc, receiveData22);
                series72.Points.AddXY(xdc, receiveData23);
                series73.Points.AddXY(xdc, receiveData24);
                series74.Points.AddXY(xdc, receiveData25);

                if (xdc >= 400)
                {
                    //会有误差
                    chartDemo7.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    chartDemo7.ChartAreas[0].AxisX.ScaleView.Position = xdc - Xval;
                }
            }

            //对tabControl中的chartDemo8画图
            else if (receiveData20 == 7)
            {
                //Series series80 = chartDemo8.Series[0];
                //Series series81 = chartDemo8.Series[1];
                //Series series82 = chartDemo8.Series[2];
                //Series series83 = chartDemo8.Series[3];
                //Series series84 = chartDemo8.Series[4];
                //画点
                series80.Points.AddXY(xdc, receiveData21);
                series81.Points.AddXY(xdc, receiveData22);
                series82.Points.AddXY(xdc, receiveData23);
                series83.Points.AddXY(xdc, receiveData24);
                series84.Points.AddXY(xdc, receiveData25);

                if (xdc >= 400)
                {
                    //会有误差
                    chartDemo8.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    chartDemo8.ChartAreas[0].AxisX.ScaleView.Position = xdc - Xval;
                }
            }

            //对tabControl中的chartDemo9画图
            else if (receiveData20 == 8)
            {
                //Series series90 = chartDemo9.Series[0];
                //Series series91 = chartDemo9.Series[1];
                //Series series92 = chartDemo9.Series[2];
                //Series series93 = chartDemo9.Series[3];
                //Series series94 = chartDemo9.Series[4];
                //画点
                series90.Points.AddXY(xdc, receiveData21);
                series91.Points.AddXY(xdc, receiveData22);
                series92.Points.AddXY(xdc, receiveData23);
                series93.Points.AddXY(xdc, receiveData24);
                series94.Points.AddXY(xdc, receiveData25);

                if (xdc >= 400)
                {
                    //会有误差
                    chartDemo9.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    chartDemo9.ChartAreas[0].AxisX.ScaleView.Position = xdc - Xval;
                }
            }

            //对tabControl中的chartDemo10画图
            else if (receiveData20 == 9)
            {
                //Series series100 = chartDemo10.Series[0];
                //Series series101 = chartDemo10.Series[1];
                //Series series102 = chartDemo10.Series[2];
                //Series series103 = chartDemo10.Series[3];
                //Series series104 = chartDemo10.Series[4];
                //画点
                series100.Points.AddXY(xdc, receiveData21);
                series101.Points.AddXY(xdc, receiveData22);
                series102.Points.AddXY(xdc, receiveData23);
                series103.Points.AddXY(xdc, receiveData24);
                series104.Points.AddXY(xdc, receiveData25);

                if (xdc >= 400)
                {
                    //会有误差
                    chartDemo10.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    chartDemo10.ChartAreas[0].AxisX.ScaleView.Position = xdc - Xval;
                }
            }

            //对tabControl中的chartDemo11画图
            if (receiveData30 == 10)
            {
                //Series series110 = chartDemo11.Series[0];
                //Series series111 = chartDemo11.Series[1];
                //Series series112 = chartDemo11.Series[2];
                //Series series113 = chartDemo11.Series[3];
                //Series series114 = chartDemo11.Series[4];
                //画点
                series110.Points.AddXY(xdc, receiveData31);
                series111.Points.AddXY(xdc, receiveData32);
                series112.Points.AddXY(xdc, receiveData33);
                series113.Points.AddXY(xdc, receiveData34);
                series114.Points.AddXY(xdc, receiveData35);

                if (xdc >= 400)
                {
                    //会有误差
                    chartDemo11.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    chartDemo11.ChartAreas[0].AxisX.ScaleView.Position = xdc - Xval;
                }
            }

            //对tabControl中的chartDemo12画图
            else if (receiveData30 == 11)
            {
                //Series series120 = chartDemo12.Series[0];
                //Series series121 = chartDemo12.Series[1];
                //Series series122 = chartDemo12.Series[2];
                //Series series123 = chartDemo12.Series[3];
                //Series series124 = chartDemo12.Series[4];
                //画点
                series120.Points.AddXY(xdc, receiveData31);
                series121.Points.AddXY(xdc, receiveData32);
                series122.Points.AddXY(xdc, receiveData33);
                series123.Points.AddXY(xdc, receiveData34);
                series124.Points.AddXY(xdc, receiveData35);

                if (xdc >= 400)
                {
                    //会有误差
                    chartDemo12.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    chartDemo12.ChartAreas[0].AxisX.ScaleView.Position = xdc - Xval;
                }
            }

            //对tabControl中的chartDemo13画图
            else if (receiveData30 == 12)
            {
                //Series series130 = chartDemo13.Series[0];
                //Series series131 = chartDemo13.Series[1];
                //Series series132 = chartDemo13.Series[2];
                //Series series133 = chartDemo13.Series[3];
                //Series series134 = chartDemo13.Series[4];
                //画点
                series130.Points.AddXY(xdc, receiveData31);
                series131.Points.AddXY(xdc, receiveData32);
                series132.Points.AddXY(xdc, receiveData33);
                series133.Points.AddXY(xdc, receiveData34);
                series134.Points.AddXY(xdc, receiveData35);

                if (xdc >= 400)
                {
                    //会有误差
                    chartDemo13.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    chartDemo13.ChartAreas[0].AxisX.ScaleView.Position = xdc - Xval;
                }
            }

            //对tabControl中的chartDemo14画图
            else if (receiveData30 == 13)
            {
                //Series series140 = chartDemo14.Series[0];
                //Series series141 = chartDemo14.Series[1];
                //Series series142 = chartDemo14.Series[2];
                //Series series143 = chartDemo14.Series[3];
                //Series series144 = chartDemo14.Series[4];
                //画点
                series140.Points.AddXY(xdc, receiveData31);
                series141.Points.AddXY(xdc, receiveData32);
                series142.Points.AddXY(xdc, receiveData33);
                series143.Points.AddXY(xdc, receiveData34);
                series144.Points.AddXY(xdc, receiveData35);

                if (xdc >= 400)
                {
                    //会有误差
                    chartDemo14.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    chartDemo14.ChartAreas[0].AxisX.ScaleView.Position = xdc - Xval;
                }
            }

            //对tabControl中的chartDemo15画图
            else if (receiveData30 == 14)
            {
                //Series series150 = chartDemo15.Series[0];
                //Series series151 = chartDemo15.Series[1];
                //Series series152 = chartDemo15.Series[2];
                //Series series153 = chartDemo15.Series[3];
                //Series series154 = chartDemo15.Series[4];
                //画点
                series150.Points.AddXY(xdc, receiveData31);
                series151.Points.AddXY(xdc, receiveData32);
                series152.Points.AddXY(xdc, receiveData33);
                series153.Points.AddXY(xdc, receiveData34);
                series154.Points.AddXY(xdc, receiveData35);

                if (xdc >= 400)
                {
                    //会有误差
                    chartDemo15.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    chartDemo15.ChartAreas[0].AxisX.ScaleView.Position = xdc - Xval;
                }
            }

            //对tabControl中的chartDemo16画图
            if (receiveData40 == 15)
            {
                //Series series160 = chartDemo16.Series[0];
                //Series series161 = chartDemo16.Series[1];
                //Series series162 = chartDemo16.Series[2];
                //Series series163 = chartDemo16.Series[3];
                //Series series164 = chartDemo16.Series[4];
                //画点
                series160.Points.AddXY(xdc, receiveData41);
                series161.Points.AddXY(xdc, receiveData42);
                series162.Points.AddXY(xdc, receiveData43);
                series163.Points.AddXY(xdc, receiveData44);
                series164.Points.AddXY(xdc, receiveData45);

                if (xdc >= 400)
                {
                    //会有误差
                    chartDemo16.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    chartDemo16.ChartAreas[0].AxisX.ScaleView.Position = xdc - Xval;
                }
            }

            //对tabControl中的chartDemo17画图
            else if (receiveData40 == 16)
            {
                //Series series170 = chartDemo17.Series[0];
                //Series series171 = chartDemo17.Series[1];
                //Series series172 = chartDemo17.Series[2];
                //Series series173 = chartDemo17.Series[3];
                //Series series174 = chartDemo17.Series[4];
                //画点
                series170.Points.AddXY(xdc, receiveData41);
                series171.Points.AddXY(xdc, receiveData42);
                series172.Points.AddXY(xdc, receiveData43);
                series173.Points.AddXY(xdc, receiveData44);
                series174.Points.AddXY(xdc, receiveData45);

                if (xdc >= 400)
                {
                    //会有误差
                    chartDemo17.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    chartDemo17.ChartAreas[0].AxisX.ScaleView.Position = xdc - Xval;
                }
            }

            //对tabControl中的chartDemo18画图
            else if (receiveData40 == 17)
            {
                //Series series180 = chartDemo18.Series[0];
                //Series series181 = chartDemo18.Series[1];
                //Series series182 = chartDemo18.Series[2];
                //Series series183 = chartDemo18.Series[3];
                //Series series184 = chartDemo18.Series[4];
                //画点
                series180.Points.AddXY(xdc, receiveData41);
                series181.Points.AddXY(xdc, receiveData42);
                series182.Points.AddXY(xdc, receiveData43);
                series183.Points.AddXY(xdc, receiveData44);
                series184.Points.AddXY(xdc, receiveData45);

                if (xdc >= 400)
                {
                    //会有误差
                    chartDemo18.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    chartDemo18.ChartAreas[0].AxisX.ScaleView.Position = xdc - Xval;
                }
            }

            //对tabControl中的chartDemo19画图
            else if (receiveData40 == 18)
            {
                //Series series190 = chartDemo19.Series[0];
                //Series series191 = chartDemo19.Series[1];
                //Series series192 = chartDemo19.Series[2];
                //Series series193 = chartDemo19.Series[3];
                //Series series194 = chartDemo19.Series[4];
                //画点
                series190.Points.AddXY(xdc, receiveData41);
                series191.Points.AddXY(xdc, receiveData42);
                series192.Points.AddXY(xdc, receiveData43);
                series193.Points.AddXY(xdc, receiveData44);
                series194.Points.AddXY(xdc, receiveData45);

                if (xdc >= 400)
                {
                    //会有误差
                    chartDemo19.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    chartDemo19.ChartAreas[0].AxisX.ScaleView.Position = xdc - Xval;
                }
            }

            //对tabControl中的chartDemo20画图
            else if (receiveData40 == 19)
            {
                //Series series200 = chartDemo20.Series[0];
                //Series series201 = chartDemo20.Series[1];
                //Series series202 = chartDemo20.Series[2];
                //Series series203 = chartDemo20.Series[3];
                //Series series204 = chartDemo20.Series[4];
                //画点
                series200.Points.AddXY(xdc, receiveData41);
                series201.Points.AddXY(xdc, receiveData42);
                series202.Points.AddXY(xdc, receiveData43);
                series203.Points.AddXY(xdc, receiveData44);
                series204.Points.AddXY(xdc, receiveData45);

                if (xdc >= 400)
                {
                    //会有误差
                    chartDemo20.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    chartDemo20.ChartAreas[0].AxisX.ScaleView.Position = xdc - Xval;
                }
            }
            GC.Collect();
        }

        //时间转换
        public double ExecDateDiff(DateTime dateBegin, DateTime dateEnd)
        {
            TimeSpan ts1 = new TimeSpan(dateBegin.Ticks);
            TimeSpan ts2 = new TimeSpan(dateEnd.Ticks);
            TimeSpan ts3 = ts1.Subtract(ts2).Duration();

            //你想转的格式
            return ts3.TotalMilliseconds;
        }

        //模型数据初始化
        public void model_original_Num()
        {
            Ps = 700;
            //P0 = double.Parse(cyzd_list_D1[0].ToString());
            L = 11;
            N = 1;
            D = 0.03175;
            //S = 0.00000125;
            S = 0.1092 * Math.Pow(10, -6);//150辆编组
            lcg_P = 0;
            t0 = 1.5;
            k = 1.5;
            k1 = 2;
            S2 = 2 * Math.Pow(2, -6);
            t1 = 1;
            W = 336;
            V_lcg = (Math.PI * Math.Pow(D, 2)) * N * L / 4;
            tw = (N * L) / W;
            
            R_ffg = S * Math.Sqrt(T) / (8.619 * 0.01 * V_ffg);
            R_zdg = (k1 * S2 * Math.Sqrt(T)) / (8.619 * 0.01 * V_lcg);
            Ps1 = Ps * Math.Exp(-R_zdg * t2);
            Pz1 = Ps1 * S1 * Math.Sqrt(T) * t2 / (8.619 * 0.01 * Vz) + Pz0;

            hj_temp_P1 = Ps * constT * S * 0.2 / (0.08619 * V_lcg);//缓解P1
        }

        //列车管制动模型函数
        private void lcg_jianya_modelFun(double t)
        {
            if (t <= tw)
            {
                lcg_P = P0;
            }
            else if (t > tw && t <= (tw + t0))
            {
                lcg_P = P0 * Math.Exp(-R_lcg * (t - tw));
            }
            else if (t > tw + t0)
            {
                lcg_P = P1 * Math.Exp(-R_lcg * (t - tw - t0));
            }
        }

        //副风缸模型函数
        private void ffg_jianya_modelFun(double t)
        {
            if (t >= 0 && t <= tw + t1)
            {
                Pf = Pf0;
            }
            else
            {
                Pf = Pf0 * Math.Exp(-R_ffg * (t - tw - t1));
            }
        }

        //制动缸模型函数
        private void zdg_jianya_modelFun(double t)
        {
            if (t >= 0 && t <= tw + t1)
            {
                zdg_P = Pz0;
            }
            else if (t > tw + t1 && t <= tw + t1 + t2)
            {
                zdg_P = Ps1 * S1 * Math.Sqrt(T) * (t - tw - t1) / (8.619 * 0.01 * V_zdg) + Pz0;
            }
            else if (t > tw + t1 + t2 && t <= tw + t1 + t2 + t3)
            {
                zdg_P = Pz1;
            }
            else 
            {
                zdg_P = Pf * S * Math.Sqrt(T) * (t - tw - t1-t2-t3) / (8.619 * 0.01 * Vz) + Pz1;
            }
        }

        //列车管缓解模型
        private void lcg_hj_modelFun(double t)
        {
            if(t>=0&&t<=5)
            {
                hj_lcg_P = lcg_ori_P0;
            }
            else if(t>5&&t<=5.2)
            {
                hj_lcg_P = Ps * S * constT * (t - 5) / (0.08619 * V_lcg) + lcg_ori_P0;
            }
            else if(t>5.2&&t<=5.7)
            {
                hj_lcg_P = (Ps + receiveData15) * constT * 1.5 * S * (t - 5.2) + hj_temp_P1 + lcg_ori_P0;
            }
            else
            {
                hj_lcg_P = Ps * S * constT * (t - 5.7) / (0.08619 * V_lcg) + (Ps + receiveData15) * constT * 0.75 * S / (0.08619 * V_lcg) + hj_temp_P1;
            }
        }

        //TCP发送指令给下位机
        public void sendTCP(string str1, string str2, string str3, string str4)
        {
            acceptSocket1.Send(encode.GetBytes(str1));
            //acceptSocket2.Send(encode.GetBytes(str2));
            //acceptSocket3.Send(encode.GetBytes(str3));
            //acceptSocket4.Send(encode.GetBytes(str4));
        }

        private void InitCOM(string PortName)
        {
            conPort = new SerialPort(PortName);
            conPort.BaudRate = 115200;//波特率
            //conPort.Parity = Parity.None;
            //conPort.StopBits = StopBits.None;
            //conPort.Handshake = Handshake.RequestToSend;//握手协议

        }

        private void F_SerialPortForm_TransfEvent(string ComName, string ComBaud)
        {
            cbPortName = ComName;
            cbBaud = ComBaud;
        }

        private void ComSend(string CommandStr)
        {
            byte[] WriterBuffer = Encoding.ASCII.GetBytes(CommandStr);
            conPort.Write(WriterBuffer, 0, WriterBuffer.Length); //退出提示“未将对象实例化”bug
        }

        public void SetAddFile1()
        {
            Console.WriteLine("bdd={0}", AppDomain.GetCurrentThreadId().ToString());
            socketListen();
        }

        private void chartDemo1_DoubleClick(object sender, EventArgs e)
        {
            chartDemo1.ChartAreas[0].AxisX.ScaleView.Size = 1;
            chartDemo1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            //throw new NotImplementedException();
        }

        private void chartTimer_Tick(object sender, EventArgs e)
        {

        }

        private void chartDemo2_DoubleClick(object sender, EventArgs e)
        {
            chartDemo2.ChartAreas[0].AxisX.ScaleView.Size = 1;
            chartDemo2.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo2.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
        }

        private void zdControlAgrol()
        {
            if (zdNum == 0)
            {
                zdBeginTime = DateTime.Now;
                zdNum = 1;
            }
            DateTime zdEndTime = DateTime.Now;
            double zdDeltaTime = (zdEndTime - zdBeginTime).TotalSeconds;

            if (receiveData14 >= zd_Relife && receiveData10 == 0)
            {
                if (double.Parse(cyzd_list_D4[0].ToString())-receiveData14 <  zd_Relife)
                {
                    lcg_jianya_modelFun(zdDeltaTime);
                    con_lcg_Num = (int)((lcg_P) * 4.095);//((lcg_P) / 1000 * 4095)

                    if (con_lcg_Num.ToString() != null)
                    {
                        ComSend("b" + con_lcg_Num.ToString() + "\r\n");                       
                    }
                }
                if (double.Parse(cyzd_list_D4[0].ToString()) - receiveData14 >= (zd_Relife-5))
                {
                    cyzd_list_D1.Clear();
                    cyzd_list_D2.Clear();
                    cyzd_list_D3.Clear();
                    cyzd_list_D4.Clear();
                    cyzd_list_D5.Clear();
                    zdTag = 0;
                    //btnHj.Enabled = true;
                }
            }
            else
            {
                MessageBox.Show("气压不足"+zd_Relife+"Kpa", "提示");
            }
        }

        //缓解控制
        private void hjControlAgrol()
        {
            if(hjBegin)
            {
                hjBeginTime = DateTime.Now;
                hjBegin = false;
            }
            hjTime = DateTime.Now;
            double deltaHjTime = (hjTime - hjBeginTime).TotalSeconds;
            if(receiveData10==0&&receiveData15<=500)
            {
                if (double.Parse(cyzd_list_D4[0].ToString()) - receiveData14 < zd_Relife)
                {
                    lcg_hj_modelFun(deltaHjTime);
                    con_lcg_Num = (int)((hj_lcg_P) * 4.095);//((hj_lcg_P) / 1000 * 4095)

                    if (con_lcg_Num.ToString() != null)
                    {
                        ComSend("b" + con_lcg_Num.ToString() + "\r\n");
                    }
                }
            }
            if(receiveData10==0&&receiveData15>500)
            {
                hjTag = false;
            }

        }

        private void btn_yjzd_Click(object sender, EventArgs e)
        {
            //减压40kpa
            zd_Relife = 40;
            zdTag = 1;
            P0 = receiveData14;
            S1 = k * S;
            R_lcg = (S1 * Math.Sqrt(T)) / (8.619 * 0.01 * V_lcg);
            P1 = P0 * Math.Exp(-R_lcg); 
            //cyzd_list_D1.Add(receiveData1);
            //cyzd_list_D2.Add(receiveData2);
            //cyzd_list_D3.Add(receiveData3);
            cyzd_list_D4.Add(receiveData14);
            //cyzd_list_D5.Add(receiveData5);
            //zdBeginTime = DateTime.Now;


        }

        private void btn_ejzd_Click(object sender, EventArgs e)
        {
            //zdBeginTime = DateTime.Now;
            //减压57kpa
            zd_Relife = 57;
            zdTag = 1;
            P0 = receiveData14;
            S1 = k * S;
            R_lcg = (S1 * Math.Sqrt(T)) / (8.619 * 0.01 * V_lcg);
            P1 = P0 * Math.Exp(-R_lcg);
            //cyzd_list_D1.Add(receiveData1);
            //cyzd_list_D2.Add(receiveData2);
            //cyzd_list_D3.Add(receiveData3);
            cyzd_list_D4.Add(receiveData14);
            //cyzd_list_D5.Add(receiveData5);
        }

        private void btn_sjzd_Click(object sender, EventArgs e)
        {
            //zdBeginTime = DateTime.Now;
            //减压74kpa
            zd_Relife = 74;
            zdTag = 1;
            P0 = receiveData14;
            S1 = k * S;
            R_lcg = (S1 * Math.Sqrt(T)) / (8.619 * 0.01 * V_lcg);
            P1 = P0 * Math.Exp(-R_lcg);
            //cyzd_list_D1.Add(receiveData1);
            //cyzd_list_D2.Add(receiveData2);
            //cyzd_list_D3.Add(receiveData3);
            cyzd_list_D4.Add(receiveData14);
            //cyzd_list_D5.Add(receiveData5);
        }

        private void btn_sijzd_Click(object sender, EventArgs e)
        {
            //zdBeginTime = DateTime.Now;
            //减压91kpa
            zd_Relife = 91;
            zdTag = 1;
            P0 = receiveData14;
            S1 = k * S;
            R_lcg = (S1 * Math.Sqrt(T)) / (8.619 * 0.01 * V_lcg);
            P1 = P0 * Math.Exp(-R_lcg);
            //cyzd_list_D1.Add(receiveData1);
            //cyzd_list_D2.Add(receiveData2);
            //cyzd_list_D3.Add(receiveData3);
            cyzd_list_D4.Add(receiveData14);
            //cyzd_list_D5.Add(receiveData5);
        }

        private void btn_wjzd_Click(object sender, EventArgs e)
        {
            //zdBeginTime = DateTime.Now;
            //减压108kpa
            zd_Relife = 108;
            zdTag = 1;
            P0 = receiveData14;
            S1 = k * S;
            R_lcg = (S1 * Math.Sqrt(T)) / (8.619 * 0.01 * V_lcg);
            P1 = P0 * Math.Exp(-R_lcg);
            //cyzd_list_D1.Add(receiveData1);
            //cyzd_list_D2.Add(receiveData2);
            //cyzd_list_D3.Add(receiveData3);
            cyzd_list_D4.Add(receiveData14);
            //cyzd_list_D5.Add(receiveData5);
        }

        private void btn_ljzd_Click(object sender, EventArgs e)
        {
            //zdBeginTime = DateTime.Now;
            //减压125kpa
            zd_Relife = 125;
            zdTag = 1;
            P0 = receiveData14;
            S1 = k * S;
            R_lcg = (S1 * Math.Sqrt(T)) / (8.619 * 0.01 * V_lcg);
            P1 = P0 * Math.Exp(-R_lcg);
            //cyzd_list_D1.Add(receiveData1);
            //cyzd_list_D2.Add(receiveData2);
            //cyzd_list_D3.Add(receiveData3);
            cyzd_list_D4.Add(receiveData14);
            //cyzd_list_D5.Add(receiveData5);
        }

        private void btn_qjzd_Click(object sender, EventArgs e)
        {
            //zdBeginTime = DateTime.Now;
            //减压140kpa
            zd_Relife = 140;
            zdTag = 1;
            P0 = receiveData14;
            S1 = k * S;
            R_lcg = (S1 * Math.Sqrt(T)) / (8.619 * 0.01 * V_lcg);
            P1 = P0 * Math.Exp(-R_lcg);
            //cyzd_list_D1.Add(receiveData1);
            //cyzd_list_D2.Add(receiveData2);
            //cyzd_list_D3.Add(receiveData3);
            cyzd_list_D4.Add(receiveData14);
            //cyzd_list_D5.Add(receiveData5);
        }

        private void btnStart_Click_1(object sender, EventArgs e)
        {
            if (conPort.IsOpen)
            {
                conPort.Close();
            }
            else
            {
                if (cbPortName.Length == 0 || cbBaud.Length == 0)
                {
                    MessageBox.Show("串口参数未设置，请先进行串口设置！", "Error");
                }
                else
                {
                    conPort.PortName = cbPortName;
                    conPort.BaudRate = int.Parse(cbBaud);
                    try
                    {
                        conPort.Open();
                    }
                    catch (Exception ex)
                    {
                        conPort = new SerialPort();
                        MessageBox.Show(ex.Message);
                    }
                    //Listen_Thred = new Thread(new ThreadStart(SetAddFile1));
                    //Listen_Thred.IsBackground = true;
                    //Listen_Thred.Start();
                    socketListen();

                    //beginTime = DateTime.Now.ToString("HH;mm;ss.fff");
                    //ExeTime = DateTime.Now;
                    btnStart.Enabled = false;
                }
            }
        }

        //数据库存储
        public void dbSave()
        {
            myconn = new MySqlConnection("Data Source =localhost;Database=barking;Username=root;Password=123456");   //本地连接
            //myconn = new MySqlConnection("Data Source =192.168.1.105; Database=barking;User=zhongche;Password=0728");  //异地连接
            myconn.Open();

            //查询表格中已有数据行数
            string countRow = "select count(*) from cyzd_pressure";
            MySqlCommand CountRows = new MySqlCommand(countRow, myconn);
            double RowNum = Convert.ToInt32(CountRows.ExecuteScalar());
            Console.WriteLine(RowNum);

            //数据的存储
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    //i++;
                    double dgv_id = double.Parse(row.Cells[0].Value.ToString()) + RowNum;
                    string dgv_time = row.Cells[1].Value.ToString();
                    double dgv_pre1 = double.Parse(row.Cells[2].Value.ToString());
                    double dgv_pre2 = double.Parse(row.Cells[3].Value.ToString());
                    double dgv_pre3 = double.Parse(row.Cells[4].Value.ToString());
                    double dgv_pre4 = double.Parse(row.Cells[5].Value.ToString());
                    double dgv_pre5 = double.Parse(row.Cells[6].Value.ToString());
                    string INS = @"insert into cyzd_pressure (id,time,pressure001,pressure002,pressure003,pressure004,pressure005) values 
                            (" + dgv_id + ",'" + dgv_time + "'," + dgv_pre1 + "," + dgv_pre2 + "," + dgv_pre3 + "," + dgv_pre4 + "," + dgv_pre5 + ")";

                    mycom = new MySqlCommand(INS, myconn);
                    mycom.ExecuteNonQuery();

                }

            }
            MessageBox.Show("数据存储完成！", "提示");
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            Thread dataDBSave = new Thread(dbSave);
            dataDBSave.IsBackground = true;
            dataDBSave.Start();
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            mycom = myconn.CreateCommand();
            mycom.CommandText = "SELECT *FROM cyzd_pressure";
            MySqlDataAdapter adap = new MySqlDataAdapter(mycom);
            DataSet ds = new DataSet();
            adap.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0].DefaultView;
        }

        private void btnExit_Click_1(object sender, EventArgs e)
        {
            ComSend("b0\r\n");
            //if (listenSocket != null)
            //{
            //    listenSocket.Close();
            //    acceptSocket.Close();
            //    listenSocket.Dispose();
            //    acceptSocket.Dispose();
            //}
            //Listen_Thred.Abort();

            if (conPort.IsOpen)
            {
                //ComSend("0\r\n");
                conPort.Close();
            }
            myconn.Close();
            this.Close();
            //Environment.Exit(0);
            //uApplication.Exit();
            //停止线程
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            SerialPortForm F_SerialPortForm = new SerialPortForm();
            F_SerialPortForm.TransfEvent += F_SerialPortForm_TransfEvent;
            F_SerialPortForm.ShowDialog();
        }

        private void chartDemo1_Click(object sender, EventArgs e)
        {

        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            xdcTemp1 = xdc;
            recDataThread1.Suspend();
            //recDataThread2.Suspend();
            //recDataThread3.Suspend();
            //recDataThread4.Suspend();

            btnContinue.Enabled = true;
            btnPause.Enabled = false;
            chartTimer.Stop();
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            chartTimer.Start();
            recDataThread1.Resume();
            //recDataThread2.Resume();
            //recDataThread3.Resume();
            //recDataThread4.Resume();

            num = 1;
            xdcTemp2 = xdcTemp1;
            btnPause.Enabled = true;
            btnContinue.Enabled = false;
        }

        private void cyzd_Load_1(object sender, EventArgs e)
        {
            //btnHj.Enabled = false;

            //myconn = new MySqlConnection("Data Source =localhost;Database=barking;Username=root;Password=123456");   //本地连接
            ////myconn = new MySqlConnection("Data Source =192.168.1.105; Database=barking;User=zhongche;Password=0728");  //异地连接
            //myconn.Open();
        }

    }
}
