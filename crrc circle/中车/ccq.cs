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
    public partial class ccq : Form
    {
        DateTime starttime, endtime, ExeTime, GetDrawTime, DealTime, ccqBeginTime, beginTime;
        private static string getTime1, getTime2;
        private static double delta_T, xdc, xdcTemp1, xdcTemp2, dataNum=0;
        private static double dataNum1, dataNum2, dataNum3, dataNum4, dataNum5, dataNum6, dataNum7, dataNum8, dataNum9, dataNum10,
            dataNum11, dataNum12, dataNum13, dataNum14, dataNum15, dataNum16, dataNum17, dataNum18, dataNum19, dataNum20;
        //dataNum：为了数据库的主键
        private int num = 1,Xval=399,beginTag1=1,beginTag2=1,beginTag3=1,beginTag4=1;
        public static int iNum, drawCount=1;
        MySqlConnection myconn = null;
        MySqlCommand mycom = null;
        public static string receiveData;
        //1：列车管；2：副风缸；3：制动缸；4：缓解风缸
        double[] receiveData1 = new double[6], receiveData2 = new double[6], receiveData3 = new double[6], receiveData4 = new double[6];
        //public static double receiveData10, receiveData11, receiveData12, receiveData13, receiveData14, receiveData15;
        //public static double receiveData20, receiveData21, receiveData22, receiveData23, receiveData24, receiveData25;
        //public static double receiveData30, receiveData31, receiveData32, receiveData33, receiveData34, receiveData35;
        //public static double receiveData40, receiveData41, receiveData42, receiveData43, receiveData44, receiveData45;

        public static ArrayList SocketArr = new ArrayList();
        public static string[] arr;
        public static string Result1 = string.Empty, Result2 = null,Result3=null,Result4=null;
        public static DataSet ds = new DataSet();
        public static Encoding encode = Encoding.UTF8;
        //public static Encoding encode = Encoding.Default;
        public static DataTable dt = new DataTable("ccq_DtPressure");
        Thread Listen_Thred, dataDBSave, recDataThread1, recDataThread2, recDataThread3, recDataThread4, dealD,dealDataThred;
        Socket acceptSocket1, acceptSocket2, acceptSocket3, acceptSocket4;
        public delegate void AddFile();
        public delegate void Draw_SaveDelegate();
        public delegate void ListenDelegate(double data0, double data1, double data2, double data3, double data4, double data5);
        public delegate void dealsocket(Socket s);
        private delegate void SaveDelegate(double data0, double data1, double data2, double data3, double data4, double data5);

        private string cbPortName = "";
        private string cbBaud = "";

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


        //Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private double lcg_P, Ps, S, V, P0, t0, D, Pf0, ffg_P, Pl;
        //lcg_P：列车管绝对压力
        //Ps：风源系统绝对压力
        //S：有效节流系数
        //V：列车管体积
        //P0：列车管初始绝对压力
        //t0：完成初充气的时间
        //D：列车管直径
        //Pf0：副风缸初始绝对压力
        //ffg_P：副风缸绝对压力
        //Pl：列车管作为风源系统的绝对压力
        private double T = 298, N, L;
        private int n = 0;

        //t：时间
        //T：绝对温度
        //N：车辆位置
        //L：列车管长度
        public int con_lcg_Num, con_ffg_Num;
        //SerialPort conPort;
        private SerialPort conPort = new SerialPort();

        public ccq()
        {
            InitializeComponent();
            Draw_Save();
            lcg_original_Num();
 
            chartTimer.Start();
        }

        private void ccq_Load(object sender, EventArgs e)
        {
            Chart[] charts = {chartDemo1, chartDemo2, chartDemo3, chartDemo4, chartDemo5, chartDemo6, chartDemo7, chartDemo8, chartDemo9, chartDemo10,
            chartDemo11,chartDemo12,chartDemo13,chartDemo14,chartDemo15,chartDemo16,chartDemo17,chartDemo18,chartDemo19,chartDemo20};
            foreach(Chart chart in charts)
            {
                InitChart(chart);
            }
            chartTimer.Interval = 1000;
            chartTimer.Tick += chartTimer_Tick;

            btnContinue.Enabled = false;

            myconn = new MySqlConnection("Data Source =localhost;Database=barking;Username=root;Password=123456");   //本地连接
            //myconn = new MySqlConnection("Data Source =192.168.1.105; Database=barking;User=zhongche;Password=0728");  //异地连接
            //myconn.Open();
            //Console.WriteLine("ccb={0}", AppDomain.GetCurrentThreadId().ToString());
        }

        public void InitChart(Chart chart)
        {
            chart.ChartAreas[0].AxisX.LabelStyle.Format = "F";
            chart.ChartAreas[0].AxisX.ScaleView.Size = 500;
            chart.ChartAreas[0].AxisY.ScaleView.Size = 600;
            //chart.ChartAreas[0].AxisX.Interval = 10;
            chart.ChartAreas[0].AxisX.Minimum = 0;
            chart.ChartAreas[0].AxisY.Minimum = 0;
            chart.ChartAreas[0].CursorX.IsUserEnabled = false;//禁止游标
            //chart.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            //chart.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            //chart.ChartAreas[0].CursorX.AutoScroll = true;
            chart.ChartAreas[0].AxisX.Title = "时间 / S";
            chart.ChartAreas[0].AxisY.Title = "气压 / kPa";

        }

        public void dgvFun(DataGridView DGV)
        {
            //添加列
            DGV.Columns[0].HeaderCell.Value = "ID";
            DGV.Columns[1].HeaderCell.Value = "时间";
            DGV.Columns[2].HeaderCell.Value = "列车管前端";
            DGV.Columns[3].HeaderCell.Value = "副风缸";
            DGV.Columns[4].HeaderCell.Value = "制动缸";
            DGV.Columns[5].HeaderCell.Value = "列车管尾端";
            DGV.Columns[6].HeaderCell.Value = "加缓风缸";
            foreach (DataGridViewColumn item in DGV.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        //表格初始化
        public void Draw_Save()
        {
            dgvFun(dataGridView1);
            dgvFun(dataGridView2);
            dgvFun(dataGridView3);
            dgvFun(dataGridView4);
            dgvFun(dataGridView5);
            dgvFun(dataGridView6);
            dgvFun(dataGridView7);
            dgvFun(dataGridView8);
            dgvFun(dataGridView9);
            dgvFun(dataGridView10);
            dgvFun(dataGridView11);
            dgvFun(dataGridView12);
            dgvFun(dataGridView13);
            dgvFun(dataGridView14);
            dgvFun(dataGridView15);
            dgvFun(dataGridView16);
            dgvFun(dataGridView17);
            dgvFun(dataGridView18);
            dgvFun(dataGridView19);
            dgvFun(dataGridView20);

        }

        private void deal(object socket)
        {
            //保持连接不中断（与下位机配合）
            Socket s = (Socket)socket;
            ListenDelegate Lin = new ListenDelegate(draw);
            SaveDelegate saveDel = new SaveDelegate(dataSave);

            while (true)
            {
                try
                {
                    if (receiveData1[0] < 5)
                    {
                        this.Invoke(Lin, receiveData1[0], receiveData1[1], receiveData1[2], receiveData1[3], receiveData1[4], receiveData1[5]);
                        this.Invoke(saveDel, receiveData1[0], receiveData1[1], receiveData1[2], receiveData1[3], receiveData1[4], receiveData1[5]);

                    }
                    if (receiveData2[0] >= 5 && receiveData2[0] < 10)
                    {
                        this.Invoke(Lin, receiveData2[0], receiveData2[1], receiveData2[2], receiveData2[3], receiveData2[4], receiveData2[5]);
                        this.Invoke(saveDel, receiveData2[0], receiveData2[1], receiveData2[2], receiveData2[3], receiveData2[4], receiveData2[5]);

                    }
                    if (receiveData3[0] >= 10 && receiveData3[0] < 15)
                    {
                        this.Invoke(Lin, receiveData3[0], receiveData3[1], receiveData3[2], receiveData3[3], receiveData3[4], receiveData3[5]);
                        this.Invoke(saveDel, receiveData3[0], receiveData3[1], receiveData3[2], receiveData3[3], receiveData3[4], receiveData3[5]);

                    }
                    if (receiveData4[0] >= 15 && receiveData4[0] < 20)
                    {
                        this.Invoke(Lin, receiveData4[0], receiveData4[1], receiveData4[2], receiveData4[3], receiveData4[4], receiveData4[5]);
                        this.Invoke(saveDel, receiveData4[0], receiveData4[1], receiveData4[2], receiveData4[3], receiveData4[4], receiveData4[5]);

                    }
                }
                catch
                {

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

            sendTCP("AB041234\r\n", "AB1501234\r\n", "AB1501234\r\n", "AB1501234\r\n");//确保程序开始时下位机处于激活状态
            rtbLog.AppendText("\n开启所有车辆采集通道！");
            sendTCP("AB2100\r\n", "AB2050\r\n", "AB2050\r\n", "AB2050\r\n");

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
            DealTime = DateTime.Now;
            delta_T = (DealTime - beginTime).TotalSeconds + xdcTemp2;//计算时间间隔
            lcg_modelFun(delta_T);
            controlAlgor();//调用控制算法
        }

        //板1数据接收
        private void dataReceive1()
        {
            string[] arr = null;
            //按行读取
            NetworkStream ntwStream = new NetworkStream(acceptSocket1);
            StreamReader strmReader = new StreamReader(ntwStream);
            if (beginTag1 == 1)
            {
                beginTime = DateTime.Now;
                beginTag1 = 0;
            }
            while (true)
            {
                Result1 = strmReader.ReadLine();
                Console.WriteLine(Result1);

                if (Result1 != null)
                {
                    try
                    {
                        arr = Result1.Split('/');

                        if (arr[0].Equals("A") && arr[1] != null && arr[2] != null && arr[3] != null && arr[4] != null && arr[5] != null && arr[6] != null)
                        {
                            receiveData1[0] = double.Parse(arr[1]);
                            receiveData1[1] = Math.Abs(250 * (double.Parse(arr[2]) / 3276.8) - 250);//0-1mPa,0-5V
                            receiveData1[2] = Math.Abs(100 * (double.Parse(arr[3]) / 3276.8));
                            receiveData1[3] = Math.Abs(100 * (double.Parse(arr[4]) / 3276.8));
                            receiveData1[4] = Math.Abs(100 * (double.Parse(arr[5]) / 3276.8));
                            receiveData1[5] = Math.Abs(100 * (double.Parse(arr[6]) / 3276.8));

                            if (receiveData1[0] == 0)
                            {
                                dealDataThred = new Thread(dataDel);
                                dealDataThred.IsBackground = true;
                                dealDataThred.Start();
                            }

                            //ListenDelegate Lin = new ListenDelegate(draw);
                            //this.Invoke(Lin, receiveData);

                            //SaveDelegate saveDel = new SaveDelegate(dataSave);
                            //this.Invoke(saveDel);
                        }
                    }
                    catch
                    {
                        //string str0, str1, str2, str3, str4, str5, str6;
                        //if (arr[0] == null || arr[0] != "A")
                        //{
                        //    str0 = "数据包字头错误！\n";
                        //}
                        //else
                        //{
                        //    str0 = "数据包头正常！\n";
                        //}
                        //if (arr[1] == null)
                        //{
                        //    str1 = "列车编号错误！\n";
                        //}
                        //else
                        //{
                        //    str1 = "第" + int.Parse(arr[1]) + 1 + "辆车：\n";
                        //}
                        //if (arr[2] == null)
                        //{
                        //    str2 = "通道1空采！\n";
                        //}
                        //else
                        //{
                        //    str2 = "通道1正常！\n";
                        //}
                        //if (arr[3] == null)
                        //{
                        //    str3 = "通道2空采！\n";
                        //}
                        //else
                        //{
                        //    str3 = "通道2正常！\n";
                        //}
                        //if (arr[4] == null)
                        //{
                        //    str4 = "通道3空采！\n";
                        //}
                        //else
                        //{
                        //    str4 = "通道3正常！\n";
                        //}
                        //if (arr[5] == null)
                        //{
                        //    str5 = "通道4空采！\n";
                        //}
                        //else
                        //{
                        //    str5 = "通道4正常！\n";
                        //}
                        //if (arr[6] == null)
                        //{
                        //    str6 = "通道5空采！";
                        //}
                        //else
                        //{
                        //    str6 = "通道5正常！";
                        //}
                        //MessageBox.Show("端口8087采集到的数据格式错误\n" + str1 + "," + str0 + "," + str2 + "," + str3 + "," + str4 + "," + str5 + "," + str6);
                    
                }
    
                }
                else
                {
                    MessageBox.Show("端口8087没有采集到数据！");
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
                        receiveData2[0] = double.Parse(arr[1])+5;
                        receiveData2[1] = Math.Abs(250 * (double.Parse(arr[2]) / 3276.8) - 250);//0-1mPa,0-5V
                        receiveData2[2] = Math.Abs(100 * (double.Parse(arr[3]) / 3276.8));
                        receiveData2[3] = Math.Abs(100 * (double.Parse(arr[4]) / 3276.8));
                        receiveData2[4] = Math.Abs(100 * (double.Parse(arr[5]) / 3276.8));
                        receiveData2[5] = Math.Abs(100 * (double.Parse(arr[6]) / 3276.8));

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
                            str1 = "第" + int.Parse(arr[1])+1 + "辆车：\n";
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
                        receiveData3[0] = double.Parse(arr[1])+10;
                        receiveData3[1] = Math.Abs(250 * (double.Parse(arr[2]) / 3276.8) - 250);//0-1mPa,0-5V
                        receiveData3[2] = Math.Abs(100 * (double.Parse(arr[3]) / 3276.8));
                        receiveData3[3] = Math.Abs(100 * (double.Parse(arr[4]) / 3276.8));
                        receiveData3[4] = Math.Abs(100 * (double.Parse(arr[5]) / 3276.8));
                        receiveData3[5] = Math.Abs(100 * (double.Parse(arr[6]) / 3276.8));

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
                Result4 = strmReader.ReadLine();
                Console.WriteLine(Result4);

                if (Result4 != null)
                {
                    arr = Result4.Split('/');

                    if (arr[0].Equals("A") && arr[1] != null && arr[2] != null && arr[3] != null && arr[4] != null && arr[5] != null && arr[6] != null)
                    {
                        receiveData4[0] = double.Parse(arr[1])+15;
                        receiveData4[1] = Math.Abs(250 * (double.Parse(arr[2]) / 3276.8) - 250);//0-1mPa,0-5V
                        receiveData4[2] = Math.Abs(100 * (double.Parse(arr[3]) / 3276.8));
                        receiveData4[3] = Math.Abs(100 * (double.Parse(arr[4]) / 3276.8));
                        receiveData4[4] = Math.Abs(100 * (double.Parse(arr[5]) / 3276.8));
                        receiveData4[5] = Math.Abs(100 * (double.Parse(arr[6]) / 3276.8));

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

        public void dsFun(DataGridView dataGridView, double dataNum, double data1, double data2, double data3, double data4, double data5)
        {
            //double[] dataNum={ dataNum1, dataNum2, dataNum3, dataNum4, dataNum5, dataNum6, dataNum7, dataNum8, dataNum9, dataNum10,
            //dataNum11, dataNum12, dataNum13, dataNum14, dataNum15, dataNum16, dataNum17, dataNum18, dataNum19, dataNum20 };

            //DataGridView[] dataGridView = {dataGridView1, dataGridView2, dataGridView3, dataGridView4, dataGridView5, dataGridView6,
            //dataGridView7,dataGridView8,dataGridView9,dataGridView10,dataGridView11,dataGridView12,dataGridView13,dataGridView14,
            //dataGridView15,dataGridView16,dataGridView17,dataGridView18,dataGridView19,dataGridView20};
            //foreach(DataGridView dgv in dataGridView)
            //{

            //}
            int index = dataGridView.Rows.Add();
            dataGridView.Rows[index].Cells[0].Value = dataNum;
            dataGridView.Rows[index].Cells[1].Value = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
            dataGridView.Rows[index].Cells[2].Value = double.Parse(string.Format("{0:F5}", data1));
            dataGridView.Rows[index].Cells[3].Value = double.Parse(string.Format("{0:F5}", data2));
            dataGridView.Rows[index].Cells[4].Value = double.Parse(string.Format("{0:F5}", data3));
            dataGridView.Rows[index].Cells[5].Value = double.Parse(string.Format("{0:F5}", data4));
            dataGridView.Rows[index].Cells[6].Value = double.Parse(string.Format("{0:F5}", data5));
            foreach (DataGridViewColumn item in dataGridView.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

        }

        //数据存储
        public void dataSave(double data0, double data1, double data2, double data3, double data4, double data5)
        {

            if (receiveData1[0] == 0)
            {
                dataNum1++;
                dsFun(dataGridView1,  dataNum1, receiveData1[1], receiveData1[2], receiveData1[3], receiveData1[4], receiveData1[5]);
            }
            else if (receiveData1[0] == 1)
            {
                dataNum2++;
                dsFun(dataGridView2, dataNum2, receiveData1[1], receiveData1[2], receiveData1[3], receiveData1[4], receiveData1[5]);
            }
            else if (receiveData1[0] == 2)
            {
                dataNum3++;
                dsFun(dataGridView3, dataNum3, receiveData1[1], receiveData1[2], receiveData1[3], receiveData1[4], receiveData1[5]);

            }
            else if (receiveData1[0] == 3)
            {
                dataNum4++;
                dsFun(dataGridView4, dataNum4, receiveData1[1], receiveData1[2], receiveData1[3], receiveData1[4], receiveData1[5]);

            }
            else if (receiveData1[0] == 4)
            {
                dataNum5++;
                dsFun(dataGridView5, dataNum5, receiveData1[1], receiveData1[2], receiveData1[3], receiveData1[4], receiveData1[5]);

            }
            else if (receiveData2[0] == 5)
            {
                dataNum6++;
                dsFun(dataGridView6, dataNum6, receiveData2[1], receiveData2[2], receiveData2[3], receiveData2[4], receiveData2[5]);

            }
            else if (receiveData2[0] == 6)
            {
                dataNum7++;
                dsFun(dataGridView7, dataNum7, receiveData2[1], receiveData2[2], receiveData2[3], receiveData2[4], receiveData2[5]);

            }
            else if (receiveData2[0] == 7)
            {
                dataNum8++;
                dsFun(dataGridView8, dataNum8, receiveData2[1], receiveData2[2], receiveData2[3], receiveData2[4], receiveData2[5]);

            }
            else if (receiveData2[0] == 8)
            {
                dataNum9++;
                dsFun(dataGridView9, dataNum9, receiveData2[1], receiveData2[2], receiveData2[3], receiveData2[4], receiveData2[5]);

            }
            else if (receiveData2[0] == 9)
            {
                dataNum10++;
                dsFun(dataGridView10, dataNum10, receiveData2[1], receiveData2[2], receiveData2[3], receiveData2[4], receiveData2[5]);

            }
            else if (receiveData3[0] == 10)
            {
                dataNum11++;
                dsFun(dataGridView11, dataNum11, receiveData3[1], receiveData3[2], receiveData3[3], receiveData3[4], receiveData3[5]);

            }
            else if (receiveData3[0] == 11)
            {
                dataNum12++;
                dsFun(dataGridView12, dataNum12, receiveData3[1], receiveData3[2], receiveData3[3], receiveData3[4], receiveData3[5]);

            }
            else if (receiveData3[0] == 12)
            {
                dataNum13++;
                dsFun(dataGridView13, dataNum13, receiveData3[1], receiveData3[2], receiveData3[3], receiveData3[4], receiveData3[5]);

            }
            else if (receiveData3[0] == 13)
            {
                dataNum14++;
                dsFun(dataGridView14, dataNum14, receiveData3[1], receiveData3[2], receiveData3[3], receiveData3[4], receiveData3[5]);

            }
            else if (receiveData3[0] == 14)
            {
                dataNum15++;
                dsFun(dataGridView15, dataNum15, receiveData3[1], receiveData3[2], receiveData3[3], receiveData3[4], receiveData3[5]);

            }
            else if (receiveData4[0] == 15)
            {
                dataNum16++;
                dsFun(dataGridView16, dataNum16, receiveData4[1], receiveData4[2], receiveData4[3], receiveData4[4], receiveData4[5]);

            }
            else if (receiveData4[0] == 16)
            {
                dataNum17++;
                dsFun(dataGridView17, dataNum17, receiveData4[1], receiveData4[2], receiveData4[3], receiveData4[4], receiveData4[5]);

            }
            else if (receiveData4[0] == 17)
            {
                dataNum18++;
                dsFun(dataGridView18, dataNum18, receiveData4[1], receiveData4[2], receiveData4[3], receiveData4[4], receiveData4[5]);

            }
            else if (receiveData4[0] == 18)
            {
                dataNum19++;
                dsFun(dataGridView19, dataNum19, receiveData4[1], receiveData4[2], receiveData4[3], receiveData4[4], receiveData4[5]);

            }

            else if (receiveData4[0] == 19)
            {
                dataNum20++;
                dsFun(dataGridView20, dataNum20, receiveData4[1], receiveData4[2], receiveData4[3], receiveData4[4], receiveData4[5]);

            }

        }

        public void drawFun(double time, double data1, double data2, double data3, double data4, double data5,
            Series series0, Series series1, Series series2, Series series3, Series series4)
        {
            series0.Points.AddXY(time, data1);
            series1.Points.AddXY(time, data2);
            series2.Points.AddXY(time, data3);
            series3.Points.AddXY(time, data4);
            series4.Points.AddXY(time, data5);
            if (time > 500)
            {
                series0.Points.Clear();
                series1.Points.Clear();
                series2.Points.Clear();
                series3.Points.Clear();
                series4.Points.Clear();

                label2.Text = ("+" + 500 * (drawCount - 1));
                series0.Points.Dispose();
                series1.Points.Dispose();
                series2.Points.Dispose();
                series3.Points.Dispose();
                series4.Points.Dispose();

                GC.Collect();
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

                num = 2;
            }
            if(num==2)
            {
                starttime = DateTime.Now;
                num = 3;
            }
            endtime = DateTime.Now;
            double dc = ExecDateDiff(starttime, endtime);
            xdc = (dc / 1000) + xdcTemp2;

            //DealTime = DateTime.Now;
            //delta_T = (DealTime - ExeTime - (starttime - ExeTime)).TotalSeconds;
            //lcg_modelFun(delta_T);
            //ffg_modelFun(delta_T);
            //controlAlgor();//调用控制算法

            if (data0 == 0)
            {
                drawFun(xdc - 500 * (drawCount - 1), data1, data2, data3, data4, data5, series10, series11, series12, series13, series14);
                //drawFun(xdc, receiveData1[1], receiveData1[2], receiveData1[3], receiveData1[4], receiveData1[5], series10, series11, series12, series13, series14);
            }
            else if (data0 == 1)
            {
                drawFun(xdc - 500 * (drawCount - 1), data1, data2, data3, data4, data5, series20, series21, series22, series23, series24);
                //drawFun(xdc, receiveData1[1], receiveData1[2], receiveData1[3], receiveData1[4], receiveData1[5], series20, series21, series22, series23, series24);
            }
            else if (data0 == 2)
            {
                drawFun(xdc - 500 * (drawCount - 1), data1, data2, data3, data4, data5, series30, series31, series32, series33, series34);
                //drawFun(xdc, receiveData1[1], receiveData1[2], receiveData1[3], receiveData1[4], receiveData1[5], series30, series31, series32, series33, series34);
            }
            else if (data0 == 3)
            {
                drawFun(xdc - 500 * (drawCount - 1), data1, data2, data3, data4, data5, series40, series41, series42, series43, series44);
                //drawFun(xdc, receiveData1[1], receiveData1[2], receiveData1[3], receiveData1[4], receiveData1[5], series40, series41, series42, series43, series44);
            }
            else if (data0 == 4)
            {
                drawFun(xdc - 500 * (drawCount - 1), data1, data2, data3, data4, data5, series50, series51, series52, series53, series54);
                //drawFun(xdc, receiveData1[1], receiveData1[2], receiveData1[3], receiveData1[4], receiveData1[5], series50, series51, series52, series53, series54);
            }

            if(data0==5)
            {
                drawFun(xdc - 500 * (drawCount - 1), data1, data2, data3, data4, data5, series60, series61, series62, series63, series64);
            }
            else if (data0 == 6)
            {
                drawFun(xdc - 500 * (drawCount - 1), data1, data2, data3, data4, data5, series70, series71, series72, series73, series74);
            }
            else if (data0 == 7)
            {
                drawFun(xdc - 500 * (drawCount - 1), data1, data2, data3, data4, data5, series80, series81, series82, series83, series84);
            }
            else if (data0 == 8)
            {
                drawFun(xdc - 500 * (drawCount - 1), data1, data2, data3, data4, data5, series90, series91, series92, series93, series94);
            }
            else if (data0 == 9)
            {
                drawFun(xdc - 500 * (drawCount - 1), data1, data2, data3, data4, data5, series100, series101, series102, series103, series104);
            }

            if (data0 == 10)
            {
                drawFun(xdc - 500 * (drawCount - 1), data1, data2, data3, data4, data5, series110, series111, series112, series113, series114);
            }
            else if (data0 == 11)
            {
                drawFun(xdc - 500 * (drawCount - 1), data1, data2, data3, data4, data5, series120, series121, series122, series123, series124);
            }
            else if(data0 == 12)
            {
                drawFun(xdc - 500 * (drawCount - 1), data1, data2, data3, data4, data5, series130, series131, series132, series133, series134);
            }
            else if(data0 == 13)
            {
                drawFun(xdc - 500 * (drawCount - 1), data1, data2, data3, data4, data5, series140, series141, series142, series143, series144);
            }
            else if (data0 == 14)
            {
                drawFun(xdc - 500 * (drawCount - 1), data1, data2, data3, data4, data5, series150, series151, series152, series153, series154);
            }

            if (data0 == 15)
            {
                drawFun(xdc - 500 * (drawCount - 1), data1, data2, data3, data4, data5, series160, series161, series162, series163, series164);
            }
            else if (data0 == 16)
            {
                drawFun(xdc - 500 * (drawCount - 1), data1, data2, data3, data4, data5, series170, series171, series172, series173, series174);
            }
            else if (data0 == 17)
            {
                drawFun(xdc - 500 * (drawCount - 1), data1, data2, data3, data4, data5, series180, series181, series182, series183, series184);
            }
            else if (data0 == 18)
            {
                drawFun(xdc - 500 * (drawCount - 1), data1, data2, data3, data4, data5, series190, series191, series192, series193, series194);
            }
            else if (data0 == 19)
            {
                drawFun(xdc - 500 * (drawCount - 1), data1, data2, data3, data4, data5, series200, series201, series202, series203, series204);
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

        //模型初始化
        public void lcg_original_Num()//模型中参数的初始化
        {
            Ps = 700;
            P0 = 100;
            L = 11;
            N = 1;
            D = 0.03175;
            //S = 0.00000125;
            S = 0.1092 * Math.Pow(10, -6);//150辆编组
            lcg_P = 0;
            V = (Math.PI * Math.Pow(D, 2)) * N * L / 4;
            @t0 = -0.000000005762680 + 0.000002199411977 * N - 0.000304578453220 * Math.Pow(N, 2) +
                0.019014019052861 * Math.Pow(N, 3) - 0.498602480934510 * Math.Pow(N, 4) +
                10.549828138352119 * Math.Pow(N, 5) + 12.392425961348907 * Math.Pow(N, 6);
        }

        //列车管模型函数
        private void lcg_modelFun(double t)
        {
            //if (lcg_P >= 0 & lcg_P < 0.528 * Ps)
            //{
            //    @lcg_P = (Ps * Math.Sqrt(T) * S * t) / (8.619 * 0.01 * V) + P0
            //        - (400 / Math.Pow(t0, 2)) * (Math.Pow(t, 2) - t0 * t);
            //}

            //else if (lcg_P >= 0.528 * Ps & lcg_P < 400)
            //{
            //    //分段定义
            //    double R1 = Math.Sqrt(1 - Math.Pow(P0 / Ps, 1 / 3.5));
            //    double R2 = Math.Sqrt(T) * S * t / (0.1561 * V);
            //    @lcg_P = Math.Pow(1 - Math.Pow(R1 - R2, 2), 3.5) * Ps
            //        - (400 / Math.Pow(t0, 2)) * (Math.Pow(t, 2) - t0 * t);
            //}
            //else if (lcg_P >= 400)
            //{
            //    lcg_P = 400;
            //}

            //if (lcg_P >= 0 & lcg_P < 200)
            //{
            //    @lcg_P = (Ps * Math.Sqrt(T) * S * t) / (8.619 * 0.01 * V) + P0
            //        - (400 / Math.Pow(t0, 2)) * (Math.Pow(t, 2) - t0 * t);
            //}
            //else if (lcg_P >= 200)
            //{
            //    lcg_P = 200;
            //}

            if (lcg_P >= 0 & lcg_P < 500)
            {
                @lcg_P = (Ps * Math.Sqrt(T) * S * t) / (8.619 * 0.01 * V) + P0;
                   // - (400 / Math.Pow(t0, 2)) * (Math.Pow(t, 2) - t0 * t);
            }

            else if (lcg_P >= 500)
            {
                lcg_P = 500;
            }
            //else if (lcg_P >= 400)
            //{
            //    lcg_P = 400;
            //}
        }

        //副风缸模型函数
        private void ffg_modelFun(double t)
        {
            if (ffg_P >= 0 & ffg_P <= 0.528 * Ps)
            {
                @ffg_P = (Pl * Math.Sqrt(T) * S * t) / (8.619 * 0.01 * V) + Pf0;
            }
            else if (ffg_P >= 0.528 * Ps & ffg_P < 400)
            {
                //分段定义
                double R1 = Math.Sqrt(1 - Math.Pow(Pf0 / Pl, 1 / 3.5));
                double R2 = Math.Sqrt(T) * S * t / (0.1561 * V);
                @lcg_P = Math.Pow(1 - Math.Pow(R1 - R2, 2), 3.5) * Ps;
            }
            else if (ffg_P >= 400)
            {
                ffg_P = 400;
            }
        }

        //控制算法
        private void controlAlgor()
        {
            //根据列车管的压力进行控制(比例阀的特性是4-20ma电流对应0-1MPa气压，DAC输出电压进行电流转换)
            //con_lcg_Num = (int)(((lcg_P - 100) * (1 + (lcg_P - 100 - receiveData4) / receiveData4) / 1000) * 4095);
            con_lcg_Num = (int)(((lcg_P - 100)*1.035 / 1000) * 4095);
            if (con_lcg_Num.ToString() != null)
            {
                ComSend("b" + con_lcg_Num.ToString() + "\r\n");
            }
        }

        private void InitCOM(string PortName)
        {
            conPort = new SerialPort(PortName);
            conPort.BaudRate = 115200;//波特率
            //conPort.Parity = Parity.None;
            //conPort.StopBits = StopBits.None;
            //conPort.Handshake = Handshake.RequestToSend;//握手协议

        }

        private void ComSend(string CommandStr)
        {
            byte[] WriterBuffer = Encoding.ASCII.GetBytes(CommandStr);
            conPort.Write(WriterBuffer, 0, WriterBuffer.Length); //退出提示“未将对象实例化”bug
        }

        //TCP发送指令给下位机
        public void sendTCP(string str1,string str2,string str3,string str4)
        {
            acceptSocket1.Send(encode.GetBytes(str1));
            //acceptSocket2.Send(encode.GetBytes(str2));
            //acceptSocket3.Send(encode.GetBytes(str3));
            //acceptSocket4.Send(encode.GetBytes(str4));
        }

        public void SetAddFile1()
        {
            Console.WriteLine("bdd={0}", AppDomain.GetCurrentThreadId().ToString());
            //this.Invoke(new AddFile(Listen));
            //new AddFile(Listen)();
            //chartDemo1.Invoke(new AddFile(Listen));
            socketListen();
            Console.WriteLine("tongxunceshi");
            //this.Invoke(new AddFile(Draw_Save));
        }

        //数据库存储
        public void dbSave()
        {
            myconn = new MySqlConnection("Data Source =localhost;Database=crrc;Username=root;Password=123456");   //本地连接
            //myconn = new MySqlConnection("Data Source =192.168.1.105; Database=barking;User=zhongche;Password=0728");  //异地连接
            myconn.Open();

            DataGridView[] dataGridView = {dataGridView1, dataGridView2, dataGridView3, dataGridView4, dataGridView5, dataGridView6,
                dataGridView7,dataGridView8,dataGridView9,dataGridView10,dataGridView11,dataGridView12,dataGridView13,dataGridView14,
                dataGridView15,dataGridView16,dataGridView17,dataGridView18,dataGridView19,dataGridView20};
            string[] dbTableStr = { "ccq_pressure1","ccq_pressure2", "ccq_pressure3" , "ccq_pressure4", "ccq_pressure5", "ccq_pressure6",
                "ccq_pressure7", "ccq_pressure8" , "ccq_pressure9" , "ccq_pressure10" , "ccq_pressure11" , "ccq_pressure12", "ccq_pressure13",
                "ccq_pressure14" , "ccq_pressure15", "ccq_pressure16", "ccq_pressure17", "ccq_pressure18", "ccq_pressure19", "ccq_pressure20"};

            for (int n = 0; n < 20; n++)
            {
                //查询表格中已有数据行数
                string countRow = "select count(*) from " + dbTableStr[n];
                MySqlCommand CountRows = new MySqlCommand(countRow, myconn);
                double RowNum = Convert.ToInt32(CountRows.ExecuteScalar());
                //Console.WriteLine(RowNum);

                //数据的存储
                foreach (DataGridViewRow row in dataGridView[n].Rows)
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
                        string INS = @"insert into " + dbTableStr[n] + " (ID,时间,列车管前端,副风缸,制动缸,列车管尾端,加缓风缸) values ("
                            + dgv_id + ",'" + dgv_time + "'," + dgv_pre1 + "," + dgv_pre2 + "," + dgv_pre3 + "," + dgv_pre4 + "," + dgv_pre5 + ")";

                        mycom = new MySqlCommand(INS, myconn);
                        mycom.ExecuteNonQuery();

                    }

                }
            }

            MessageBox.Show("数据存储完成！", "提示");
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            dataDBSave = new Thread(dbSave);
            dataDBSave.IsBackground = true;
            dataDBSave.Start();
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            mycom = myconn.CreateCommand();
            mycom.CommandText = "SELECT *FROM ccq_pressure";
            MySqlDataAdapter adap = new MySqlDataAdapter(mycom);
            DataSet ds = new DataSet();
            adap.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0].DefaultView;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            //if (listenSocket != null)
            //{
            //    listenSocket.Close();
            //    acceptSocket.Close();
            //    listenSocket.Dispose();
            //    acceptSocket.Dispose();
            //}
            //Listen_Thred.Abort();

            if (acceptSocket1!=null&& acceptSocket2 != null&& acceptSocket3 != null&& acceptSocket4 != null)
            {
                sendTCP("AB0501234\r\n", "AB0501234\r\n", "AB0501234\r\n", "AB0501234\r\n");
                acceptSocket1.Dispose();
                acceptSocket2.Dispose();
                acceptSocket3.Dispose();
                acceptSocket4.Dispose();

            }
            try { Listen_Thred.Abort(); }
            catch { }

            try { recDataThread1.Abort(); }
            catch { }

            try { recDataThread2.Abort(); }
            catch { }

            try { recDataThread3.Abort(); }
            catch { }

            try { recDataThread4.Abort(); }
            catch { }

            try { conPort.Close(); }
            catch { }

            try { myconn.Close(); }
            catch { }

            this.Close();
            //Environment.Exit(0);
            //uApplication.Exit();
            //停止线程
        }

        private void chartDemo2_DoubleClick(object sender, EventArgs e)
        {
            chartDemo2.ChartAreas[0].AxisX.ScaleView.Size = 1;
            chartDemo2.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo2.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //if (conPort.IsOpen)
            //{
            //    conPort.Close();
            //}
            //else
            // {
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

        private void F_SerialPortForm_TransfEvent(string ComName, string ComBaud)
        {
            cbPortName = ComName;
            cbBaud = ComBaud;
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            SerialPortForm F_SerialPortForm = new SerialPortForm();
            F_SerialPortForm.TransfEvent += F_SerialPortForm_TransfEvent;
            F_SerialPortForm.ShowDialog();
        }

        private void btnExhaust_Click(object sender, EventArgs e)
        {
            ComSend("b0\r\n");//可能会造成紧急制动
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            xdcTemp1 = xdc;
            try
            {
                recDataThread1.Suspend();
                recDataThread2.Suspend();
                recDataThread3.Suspend();
                recDataThread4.Suspend();
            }
            catch
            {

            }
            sendTCP("AB0501234\r\n", "AB0501234\r\n", "AB0501234\r\n", "AB0501234\r\n");
            dealD.Suspend();
            btnContinue.Enabled = true;
            btnPause.Enabled = false;
            chartTimer.Stop();
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            chartTimer.Start();
            sendTCP("AB1501234\r\n", "AB1501234\r\n", "AB1501234\r\n", "AB1501234\r\n");
            try
            {
                recDataThread1.Resume();
                recDataThread2.Resume();
                recDataThread3.Resume();
                recDataThread4.Resume();
            }
            catch
            {

            }
            dealD.Resume();
            num = 2;
            xdcTemp2 = xdcTemp1;
            btnPause.Enabled = true;
            btnContinue.Enabled = false;
        }

    }

}
