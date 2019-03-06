/*
 * Version36、接Ver35。
 * 添加不同编组的级联实验
 */ 

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
    public delegate void TransfDelegate(string value); 

    public partial class ccqjlkz : Form
    {
        DateTime starttime, endtime, GetDrawTime, DealTime, cqBeigin, cqing, cqEnd, btnPauseTime, btnContinueTime, beginTime, zdBeginTime, dataDealTime;
        string[] arr = null;
        double[] judgeTag = new double[6];
        private static double delta_T, runNum,xdc,xdcTemp1,xdcTemp2;
        //xdc:横坐标；xdcTemp1、xdcTemp2：暂停时的临时存储
        private int num = 0, beginTag = 1, dataDealTag=1;
        public static int iNum, trainNumCase, zdConTag=1;
        MySqlConnection myconn = null;
        MySqlCommand mycom = null;
        public static string receiveData;
        //1：均衡风缸；2：副风缸；3：制动缸；4：列车管；5：加缓风缸
        double[] receiveData1 = new double[6], receiveData2 = new double[6], receiveData3 = new double[6], receiveData4 = new double[6];

        private static double dataNum1, dataNum2, dataNum3, dataNum4, dataNum5, dataNum6, dataNum7, dataNum8, dataNum9, dataNum10,
            dataNum11, dataNum12, dataNum13, dataNum14, dataNum15, dataNum16, dataNum17, dataNum18, dataNum19, dataNum20;
        public static bool addDataTag = true, sleepTag=false, btnPasueTag=false, btnContinueTag=false, dealThreadTag=true, addTag=false;
        public static string Result1 = string.Empty, Result2, Result3, Result4;
        public static Encoding encode = Encoding.UTF8;
        Socket acceptSocket1, acceptSocket2, acceptSocket3, acceptSocket4;
        public static DataSet ds = new DataSet();
        public static DataTable dt = new DataTable("jl_ccq_DtPressure");
        public delegate void ListenDelegate(double data0, double data1, double data2, double data3, double data4, double data5);
        public delegate void SavaDelegate(double data0, double data1, double data2, double data3, double data4, double data5);
        private string cbPortName = "";
        private string cbBaud = "";

        Thread Listen_Thread, recDataThread1, recDataThread2, recDataThread3, recDataThread4, calSval, dealD;

        //将数据委托显示到另一项目窗口上
        public delegate void ccqTransfDelegate( double value0, double value1, double value2, double value3, double value4, double value5);
        
        private double lcg_P, zdg_P, Ps, S, V_lcg, P0, t0, tw, D, Pf0, ffg_P, P1, S1, k, Pf, V_ffg, V_zdg, P_lcg,axisXVal=0;



        private int bianzuNum, bianzuTrainNum;
        public static ArrayList jlTime = new ArrayList();

        public static ArrayList lcgBefore = new ArrayList();
        public static ArrayList ffgPre = new ArrayList();
        public static ArrayList zdgPre = new ArrayList();
        public static ArrayList lcgAfter = new ArrayList();
        public static ArrayList jhfgPre = new ArrayList();

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
        //Series series0, series1, series2, series3, series4;
        //Series series1 = chartDemo.Series[1];
        //Series series2 = chartDemo.Series[2];
        //Series series3 = chartDemo.Series[3];
        //Series series4 = chartDemo.Series[4];

        private void btnShow_Click(object sender, EventArgs e)
        {
            ccqjlkzChart cf = new ccqjlkzChart();
            cf.Show();
        }

        //lcg_P：列车管绝对压力
        //zdg_P：制动缸升压时的绝对压力
        //Ps：风源系统绝对压力
        //S：有效节流系数
        //V_lcg：列车管体积
        //P0：列车管初始绝对压力（初充气）
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
        //P_lcg：列车管作为风源系统的绝对压力
        private double Ps1, k1, S2, Pz0, t2, Pz1, Vz, t1, t3, r, zd_P0, zd_t0;
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
        //zd_P0：制动开始时列车管初始绝对压力
        public static double T = 298, N, L, W, zd_t = 0.01, jl_cq_PreNum, correctS = 0;
        //zd_t：时间
        //T：绝对温度
        //N：车辆位置
        //L：列车管长度
        //W：空气波速
        private double R_lcg, R_ffg, R_zdg, SendCount=0, arrTag=1, arrLength, drawTag=0;
        public int con_lcg_Num, con_ffg_Num, jl_tag = 0, list_Num = 1, list_N = 0 , sentModelNum=0 ,secondTag=0 , sentArrayNum=0, tickNum=0,drawTimesTag=0;
        public static bool drawShowTag=false, cqBeginTag=true, cqTag=true, calS=true, trainNumTag=true, dataRecTag=false, drawTimeTag=false, DGVTag=false;
        //calS：计=计算S值的线程标志
        //calShow：S值在界面上显示的标志
        //SerialPort conPort;
        private SerialPort conPort = new SerialPort();
        
        public static ArrayList ccqjlkz_list_D4 = new ArrayList();
        public static ArrayList ccqjlkz_list_D6 = new ArrayList();

        //public static ArrayList drawData1 = new ArrayList();
        //public static ArrayList drawData2 = new ArrayList();
        //public static ArrayList drawData3 = new ArrayList();
        //public static ArrayList drawData4 = new ArrayList();
        //public static ArrayList drawData5 = new ArrayList();
        //public static ArrayList drawTime = new ArrayList();
        public static ArrayList SNumA = new ArrayList();
        public static ArrayList SNumB = new ArrayList();
        private static int SNumTag = 1;

        public static int drawDataNum = 0, drawDataTag=0, dataReceiveTag=0, trainNum=1, countNum=0, drawCount=1;
        //trainNun：列车的位置

        public ccqjlkz()
        {
            InitializeComponent();
            lcg_original_Num();
        }

        private void ccqjlkz_Load(object sender, EventArgs e)
        {
            bianzuNum = jlSetting.BIANZU;
            bianzuTrainNum = jlSetting.TRAIN;

            //初始化画图chart
            Chart[] charts = {chartDemo1, chartDemo2, chartDemo3, chartDemo4, chartDemo5, chartDemo6, chartDemo7, chartDemo8, chartDemo9, chartDemo10,
            chartDemo11,chartDemo12,chartDemo13,chartDemo14,chartDemo15,chartDemo16,chartDemo17,chartDemo18,chartDemo19,chartDemo20};
            foreach (Chart chart in charts)
            {
                InitChart(chart);
            }
            jl_chartTimer.Interval = 1000;
            jl_chartTimer.Tick += jl_chartTimer_Tick;

            //初始化数据存储表格
            DataGridView[] dataGridView = {dataGridView1, dataGridView2, dataGridView3, dataGridView4, dataGridView5, dataGridView6,
            dataGridView7,dataGridView8,dataGridView9,dataGridView10,dataGridView11,dataGridView12,dataGridView13,dataGridView14,
            dataGridView15,dataGridView16,dataGridView17,dataGridView18,dataGridView19,dataGridView20};
            foreach (DataGridView dgv in dataGridView)
            {
                dgvFun(dgv);
            }

            //myconn = new MySqlConnection("Data Source =localhost;Database=barking;Username=root;Password=123456");   //本地连接
            ////myconn = new MySqlConnection("Data Source =192.168.1.105; Database=barking;User=zhongche;Password=0728");  //异地连接
            //myconn.Open();

            btnContinue.Enabled = false;

            jl_chartTimer.Start();

        }

        //chart初始化
        public void InitChart(Chart chart)
        {          
            chart.ChartAreas[0].AxisX.LabelStyle.Format = "F";
            chart.ChartAreas[0].AxisX.ScaleView.Size = 500;
            chart.ChartAreas[0].AxisY.ScaleView.Size = 600;
            chart.ChartAreas[0].AxisX.Minimum = 0;
            chart.ChartAreas[0].AxisY.Minimum = 0;           
            chart.ChartAreas[0].CursorX.IsUserEnabled = false;//禁止游标
            chart.ChartAreas[0].AxisX.Title = "时间 / s";
            chart.ChartAreas[0].AxisY.Title = "气压 / kPa";

        }

        //dgv初始化
        public void dgvFun(DataGridView DGV)
        {
            //添加列
            string c0="column0",c1="column1",c2="column2",c3="column3",c4="column4",c5="column5",c6="column6",c7="column7";
            DGV.Columns.Add(c0,"ID");
            DGV.Columns.Add(c1, "位置");
            DGV.Columns.Add(c2, "时间");
            DGV.Columns.Add(c3, "列车管前端");
            DGV.Columns.Add(c4, "副风缸");
            DGV.Columns.Add(c5, "制动缸");
            DGV.Columns.Add(c6, "列车管尾端");
            DGV.Columns.Add(c7, "加缓风缸");
            //DGV.Columns[0].HeaderCell.Value = "ID";
            //DGV.Columns[1].HeaderCell.Value = "时间";
            //DGV.Columns[2].HeaderCell.Value = "列车管前端";
            //DGV.Columns[3].HeaderCell.Value = "副风缸";
            //DGV.Columns[4].HeaderCell.Value = "制动缸";
            //DGV.Columns[5].HeaderCell.Value = "列车管尾端";
            //DGV.Columns[6].HeaderCell.Value = "加缓风缸";
            foreach (DataGridViewColumn item in DGV.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        private void deal(object socket)
        {  
            Socket s = (Socket)socket;

            //保持连接不中断（与下位机配合） 
            ListenDelegate Lin = new ListenDelegate(draw);
            SavaDelegate saveDel = new SavaDelegate(dataSave);

            while (dealThreadTag)
            {
                //this.Invoke(Lin, receiveData);//采集界面退出提示错误   
                //this.Invoke(saveDel);

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
            recDataThread1 = new Thread(dataReceive1);
            recDataThread1.IsBackground = true;

            recDataThread2 = new Thread(dataReceive2);
            recDataThread2.IsBackground = true;

            recDataThread3 = new Thread(dataReceive3);
            recDataThread3.IsBackground = true;

            recDataThread4 = new Thread(dataReceive4);
            recDataThread4.IsBackground = true;
          
            dealD = new Thread(deal);
            dealD.IsBackground = true;
          
            Socket listenSocket1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket1.Bind(new IPEndPoint(IPAddress.Any, 8087));
            //Socket listenSocket2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //listenSocket2.Bind(new IPEndPoint(IPAddress.Any, 8088));
            //Socket listenSocket3 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //listenSocket3.Bind(new IPEndPoint(IPAddress.Any, 8089));
            //Socket listenSocket4 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //listenSocket4.Bind(new IPEndPoint(IPAddress.Any, 8090));

            if(bianzuTrainNum==1||bianzuTrainNum==5)
            {
                listenSocket1.Listen(100);

                acceptSocket1 = listenSocket1.Accept();
                rtbLog.AppendText("8087端口连接成功！\n");

                if(bianzuTrainNum==1)
                {
                    //AD采集控制协议帧头：AB；0/1（停止/开始）；控制AD个数；控制AD序号           
                    sendTCP("AB041234\r\n", "AB0501234\r\n", "AB0501234\r\n", "AB0501234\r\n");
                    rtbLog.AppendText("\n激活第1辆车采集！");
                }
                else
                {
                    sendTCP("AB1501234\r\n", "AB0501234\r\n", "AB0501234\r\n", "AB0501234\r\n");
                    rtbLog.AppendText("\n激活第1~5辆车采集！");
                }

                recDataThread1.Start();
                dealD.Start(acceptSocket1);

            }
            else if(bianzuTrainNum==10)
            {
                listenSocket1.Listen(100);
                //listenSocket2.Listen(100);

                acceptSocket1 = listenSocket1.Accept();
                rtbLog.AppendText("8087端口连接成功！\n");
                //acceptSocket2 = listenSocket2.Accept();
                //rtbLog.AppendText("8088端口连接成功！\n");

                sendTCP("AB1501234\r\n", "AB1501234\r\n", "AB0501234\r\n", "AB0501234\r\n");
                rtbLog.AppendText("\n激活第1~10辆车采集！");

                recDataThread1.Start();
                //recDataThread2.Start();
                dealD.Start(acceptSocket1);
                //dealD.Start(acceptSocket2);
                
            }
            else if(bianzuTrainNum==15)
            {
                listenSocket1.Listen(100);
                //listenSocket2.Listen(100);
                //listenSocket3.Listen(100);

                acceptSocket1 = listenSocket1.Accept();
                rtbLog.AppendText("8087端口连接成功！\n");
                //acceptSocket2 = listenSocket2.Accept();
                //rtbLog.AppendText("8088端口连接成功！\n");
                //acceptSocket3 = listenSocket3.Accept();
                //rtbLog.AppendText("8089端口连接成功！\n");

                sendTCP("AB1501234\r\n", "AB1501234\r\n", "AB1501234\r\n", "AB0501234\r\n");
                rtbLog.AppendText("\n激活第1~15辆车采集！");

                recDataThread1.Start();
                //recDataThread2.Start();
                //recDataThread3.Start();
                dealD.Start(acceptSocket1);
                //dealD.Start(acceptSocket2);
                //dealD.Start(acceptSocket3);
            }
            else
            {
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

                sendTCP("AB1501234\r\n", "AB1501234\r\n", "AB1501234\r\n", "AB1501234\r\n");
                rtbLog.AppendText("\n激活第1~20辆车采集！");

                recDataThread1.Start();
                //recDataThread2.Start();
                //recDataThread3.Start();
                //recDataThread4.Start();
                dealD.Start(acceptSocket1);
                //dealD.Start(acceptSocket2);
                //dealD.Start(acceptSocket3);
                //dealD.Start(acceptSocket4);
            }

            rtbLog.AppendText("\n开始采集！");

        }

        //数据接收
        public void  dataReceive1()
        {
            Thread dealDataThread1=null;
            //按行读取
            NetworkStream ntwStream = new NetworkStream(acceptSocket1);
            StreamReader strmReader = new StreamReader(ntwStream);
            if (beginTag == 1)
            {
                beginTime = DateTime.Now;
                beginTag = 2;
            }
            while (true)
            {
                dataRecTag = true;
                //btnContinueTag = false;
                Result1 = strmReader.ReadLine();
                Console.WriteLine(Result1);
                DealTime = DateTime.Now;//数据解析完成的时间
                //double db_T = (DealTime - beginTime).TotalSeconds + xdcTemp2;//为了保证采集到数据的横坐标与模型横坐标一致

                //用于释放计算S值的线程
                while (!calS)
                {
                    calSval.Abort();
                    calS = true;
                    break;
                }

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

                            if (dataDealTag == 1)
                            {
                                dealDataThread1 = new Thread(dataDeal);
                                //dealDataThread1.IsBackground = true;
                                dealDataThread1.Start();
                            }

                            ////存储为了显示button
                            //jlTime.Add(db_T);
                            //lcgBefore.Add(receiveData1);
                            //ffgPre.Add(receiveData2);
                            //zdgPre.Add(receiveData3);
                            //lcgAfter.Add(receiveData4);
                            //jhfgPre.Add(receiveData5);

                        }
                    }
                    catch
                    {
                        if(arr.Length==7)
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
                        else
                        {
                            MessageBox.Show("数据传输错误！", "error");
                        }
                    }
                   
                }

                dataRecTag = false;
            }                    
            
        }

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
                    try
                    {
                        if (arr[0].Equals("A") && arr[1] != null && arr[2] != null && arr[3] != null && arr[4] != null && arr[5] != null && arr[6] != null)
                        {
                            receiveData2[0] = double.Parse(arr[1]) + 5;
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
                    }
                    catch(Exception e)
                    {
                        if (arr.Length==7)
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
                        else
                        {
                            MessageBox.Show("数据传输错误！", "error");
                        }
                    }
                                      

                }
                else
                {
                    MessageBox.Show("没有采集到数据");
                }

            }
        }

        private void dataReceive3()
        {
            string[] arr = null;

            //按行读取
            NetworkStream ntwStream = new NetworkStream(acceptSocket2);
            StreamReader strmReader = new StreamReader(ntwStream);

            while (true)
            {
                Result3 = strmReader.ReadLine();
                Console.WriteLine(Result3);

                if (Result3 != null)
                {
                    arr = Result3.Split('/');
                    try
                    {
                        if (arr[0].Equals("A") && arr[1] != null && arr[2] != null && arr[3] != null && arr[4] != null && arr[5] != null && arr[6] != null)
                        {
                            receiveData3[0] = double.Parse(arr[1]) + 10;
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
                    }
                    catch
                    {
                        if (arr.Length == 7)
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
                        else
                        {
                            MessageBox.Show("数据传输错误！", "error");
                        }
                    }


                }
                else
                {
                    MessageBox.Show("没有采集到数据");
                }

            }
        }

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
                    try
                    {
                        if (arr[0].Equals("A") && arr[1] != null && arr[2] != null && arr[3] != null && arr[4] != null && arr[5] != null && arr[6] != null)
                        {
                            receiveData4[0] = double.Parse(arr[1]) + 15;
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
                    }
                    catch
                    {
                        if (arr.Length == 7)
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
                        else
                        {
                            MessageBox.Show("数据传输错误！", "error");
                        }
                    }


                }
                else
                {
                    MessageBox.Show("没有采集到数据");
                }

            }
        }

        public void dataDeal()
        {
            dataDealTag = 0;
            if (receiveData1[0] == 0)
            {
                if (jl_tag == 0)
                {
                    //根据模型进行充气，一次循环中只会在开始阶段执行一次
                    dataDealTime = DateTime.Now;//数据解析完成的时间
                    delta_T = (dataDealTime - beginTime).TotalSeconds + xdcTemp2;//为了保证采集到数据的横坐标与模型横坐标一致

                    controlAlgor();//调用控制算法

                    ccqjlkz_list_D4.Add(receiveData1[4]);

                    if (receiveData1[4] >= 399 && countNum < 200)
                    {
                        countNum++;
                    }

                    if (receiveData1[4] >= 399 && countNum == 200)
                    {
                        cqTag = false;
                        zd_P0 = receiveData1[4] + 100;//模型中的气压应该为绝对压力
                        S1 = k * S;
                        R_lcg = (S1 * Math.Sqrt(T)) / (0.08619 * V_lcg);
                        P1 = zd_P0 * Math.Exp(-R_lcg);
                        jl_tag = 1;
                    }

                }
                //if (jl_tag == 1 || jl_tag == 2)
                else
                {
                    //进行循环排气、充气
                    if (jl_tag == 1 && receiveData1[4] >= 5)
                    {
                        /*开始排气到0kpa(表压)*/
                        zd_controlAlgor();//排气到110kpa（大气压）

                        if (receiveData1[4] <= 8)
                        {
                            if(receiveData1[2] > 12)
                            {
                                sendTCP("AB31\r\n", "", "", "");
                            }
                            else
                            {
                                sendTCP("AB30\r\n", "", "", "");
                                zdConTag = 1;//为下次排气做准备
                                jl_tag = 2;
                                cqTag = true;
                            }
                         
                        }
                        trainNumTag = true;
                        addTag = true;
                    }

                    //排气到0kpa开始对第二辆车充气
                    else if (jl_tag == 2 && receiveData1[4] <= 405)//（初始大气压101kpa）
                    {
                        while (trainNumTag)
                        {
                            if(bianzuTrainNum==1)
                            {
                                trainNum++;
                            }
                            else if(bianzuTrainNum==5)
                            {
                                trainNum += 5;
                            }
                            else if(bianzuTrainNum==10)
                            {
                                bianzuTrainNum += 10;
                            }
                            else if(bianzuTrainNum==15)
                            {
                                bianzuTrainNum += 15;
                            }
                            else
                            {
                                bianzuTrainNum += 20;
                            }
                            SNumTag = 0;
                            trainNumTag = false;
                            break;
                        }

                        if (arrTag == 1)//调用不同的数组
                        {
                            if(addTag)
                            {
                                for (int n = 0; n < 100; n++)
                                {
                                    ccqjlkz_list_D4.Add(405);
                                }
                                addTag = false;
                            }
                            arrLength = ccqjlkz_list_D4.Count;
                            ccqjlkz_list_D6.Add(receiveData1[4]);
                        }
                        else
                        {
                            if (addTag)
                            {
                                for (int n = 0; n < 100; n++)
                                {
                                    ccqjlkz_list_D6.Add(405);
                                }
                                addTag = false;
                            }
                            arrLength = ccqjlkz_list_D6.Count;
                            ccqjlkz_list_D4.Add(receiveData1[4]);
                        }

                        //关闭排气电磁阀，开始充气
                        /*
                         * 关闭排气阀
                         */
                        //打开电磁阀进行充气（初始大气压101kpa）
                        //遍历Arraylist中的数据进行第二辆车的充气
                        if (list_Num < arrLength)
                        {
                            if (arrTag == 1)
                            {
                                if (cqBeginTag)
                                {
                                    cqBeigin = DateTime.Now;//循环充气过程开始的时间
                                    cqBeginTag = false;
                                }
                                try
                                {
                                    jl_cq_PreNum = (double.Parse(ccqjlkz_list_D4[list_Num - 1].ToString())) * 1.035;
                                    if (jl_cq_PreNum.ToString() != null)
                                    {
                                        ComSend("b" + (jl_cq_PreNum * 4.095).ToString() + "\r\n");
                                    }
                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show(e.ToString());
                                }

                            }
                            else
                            {
                                try
                                {
                                    jl_cq_PreNum = (double.Parse(ccqjlkz_list_D6[list_Num - 1].ToString())) * 1.035;
                                    if (jl_cq_PreNum.ToString() != null)
                                    {
                                        ComSend("b" + (jl_cq_PreNum * 4.095).ToString() + "\r\n");
                                    }
                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show(e.ToString());
                                }

                            }
                        }
                        cqing = DateTime.Now;//充气过程中的任意时刻

                        if (trainNum >= 2)
                        {
                            correctMethod();
                        }

                        list_Num++;

                        //if (list_Num >= arrLength)//保证把上一次存的数据发完 //会有第二辆车不排气的情况出现
                        {
                            if (receiveData1[4] >= 395)
                            {
                                cqTag = false;
                                if (arrTag == 1)
                                {
                                    arrTag = 2;
                                    ccqjlkz_list_D4.Clear();
                                }
                                else
                                {
                                    arrTag = 1;
                                    ccqjlkz_list_D6.Clear();
                                }
                                arrLength = 0;

                                list_Num = 1;
                                jl_tag = 1;

                                //充气结束计算Ss值
                                calSval = new Thread(calculateSs);
                                calSval.IsBackground = true;
                                calSval.Start();

                                zd_P0 = receiveData1[4] + 100;
                                trainNumTag = true;
                            }
                            //else
                            //{
                            //    ComSend("b" + 1625 + "\r\n");
                            //    if (arrTag == 1)//调用不同的数组
                            //    {
                            //        ccqjlkz_list_D6.Add(receiveData4);
                            //    }
                            //    else
                            //    {
                            //        ccqjlkz_list_D4.Add(receiveData4);
                            //    }
                            //}
                        }

                    }

                }
            }
            dataDealTag = 1;
        }

        public void dsFun(DataGridView dataGridView, double dataNum, double data1, double data2, double data3, double data4, double data5)
        {
            try
            {
                int index = dataGridView.Rows.Add();
                dataGridView.Rows[index].Cells[0].Value = dataNum;
                if (cqTag == true)
                {
                    dataGridView.Rows[index].Cells[1].Value = trainNum;
                }
                else
                {
                    dataGridView.Rows[index].Cells[1].Value = 0;
                }
                dataGridView.Rows[index].Cells[2].Value = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
                dataGridView.Rows[index].Cells[3].Value = double.Parse(string.Format("{0:F5}", data1));
                dataGridView.Rows[index].Cells[4].Value = double.Parse(string.Format("{0:F5}", data2));
                dataGridView.Rows[index].Cells[5].Value = double.Parse(string.Format("{0:F5}", data3));
                dataGridView.Rows[index].Cells[6].Value = double.Parse(string.Format("{0:F5}", data4));
                dataGridView.Rows[index].Cells[7].Value = double.Parse(string.Format("{0:F5}", data5));
                foreach (DataGridViewColumn item in dataGridView.Columns)
                {
                    item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString(),"Exception");
            }

        }

        //数据存储到表格
        public void dataSave(double data0, double data1, double data2, double data3, double data4, double data5)
        {
            if(bianzuTrainNum==1)
            {
                dataNum1++;
                dsFun(dataGridView1, dataNum1, receiveData1[1], receiveData1[2], receiveData1[3], receiveData1[4], receiveData1[5]);
            }
            if(bianzuTrainNum==5)
            {
                if(receiveData1[0]==0)
                {
                    dataNum1++;
                    dsFun(dataGridView1, dataNum1, receiveData1[1], receiveData1[2], receiveData1[3], receiveData1[4], receiveData1[5]);
                }
                else if(receiveData1[0]==1)
                {
                    dataNum2++;
                    dsFun(dataGridView2, dataNum2, receiveData1[1], receiveData1[2], receiveData1[3], receiveData1[4], receiveData1[5]);
                }
                else if(receiveData1[0]==2)
                {
                    dataNum3++;
                    dsFun(dataGridView3, dataNum3, receiveData1[1], receiveData1[2], receiveData1[3], receiveData1[4], receiveData1[5]);
                }
                else if(receiveData1[0]==3)
                {
                    dataNum4++;
                    dsFun(dataGridView4, dataNum4, receiveData1[1], receiveData1[2], receiveData1[3], receiveData1[4], receiveData1[5]);
                }
                else
                {
                    dataNum5++;
                    dsFun(dataGridView5, dataNum5, receiveData1[1], receiveData1[2], receiveData1[3], receiveData1[4], receiveData1[5]);
                }

            }

            ////添加行
            //DataRow dtr = dt.NewRow();
            //dtr["ID"] = dataNum;
            //if (cqTag == true)
            //{
            //    dtr["位置"] = trainNum;
            //}
            //else
            //{
            //    dtr["位置"] = 0;
            //}
            //dtr["时间"] = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
            //dtr["列前"] = double.Parse(string.Format("{0:F}", receiveData1));
            //dtr["副风缸"] = double.Parse(string.Format("{0:F}", receiveData2));
            //dtr["制动缸"] = double.Parse(string.Format("{0:F}", receiveData3));
            //dtr["列尾"] = double.Parse(string.Format("{0:F}", receiveData4));
            //dtr["加缓缸"] = double.Parse(string.Format("{0:F}", receiveData5));

            //dt.Rows.Add(dtr);

            //foreach (DataGridViewColumn item in dataGridView1.Columns)
            //{
            //    item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //}
            //dataNum++;
        }

        //画图加载初始化
        public void drawInitiFun(Chart chart, Series series0, Series series1, Series series2, Series series3, Series series4)
        {
            series0 = chart.Series[0];
            series1 = chart.Series[1];
            series2 = chart.Series[2];
            series3 = chart.Series[3];
            series4 = chart.Series[4];
            series0.LegendText = "列车管前端";
            series1.LegendText = "副风缸";
            series2.LegendText = "制动缸";
            series3.LegendText = "列车管尾端";
            series4.LegendText = "加缓风缸";
        }

        //画图函数
        public void drawFun(double time, double data1, double data2, double data3, double data4, double data5,
            Series series0, Series series1, Series series2, Series series3, Series series4)
        {
            series0.Points.AddXY(time, data1);
            series1.Points.AddXY(time, data2);
            series2.Points.AddXY(time, data3);
            series3.Points.AddXY(time, data4);
            series4.Points.AddXY(time, data5);
            if(time>500)
            {
                series0.Points.Clear();
                series1.Points.Clear();
                series2.Points.Clear();
                series3.Points.Clear();
                series4.Points.Clear();

                label1.Text = ("+" + 500 * (drawCount - 1));
                series0.Points.Dispose();
                series1.Points.Dispose();
                series2.Points.Dispose();
                series3.Points.Dispose();
                series4.Points.Dispose();

                GC.Collect();
            }
        }

        //主线程UI
        public void draw(double data0, double data1, double data2, double data3, double data4, double data5)
        {
            if(num==0)
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

                //drawInitiFun(chartDemo1, series10, series11, series12, series13, series14);
                //drawInitiFun(chartDemo2, series20, series21, series22, series23, series24);
                //drawInitiFun(chartDemo3, series30, series31, series32, series33, series34);
                //drawInitiFun(chartDemo4, series40, series41, series42, series43, series44);
                //drawInitiFun(chartDemo5, series50, series51, series52, series53, series54);
                //drawInitiFun(chartDemo6, series60, series61, series62, series63, series64);
                //drawInitiFun(chartDemo7, series70, series71, series72, series73, series74);
                //drawInitiFun(chartDemo8, series80, series81, series82, series83, series84);
                //drawInitiFun(chartDemo9, series90, series91, series92, series93, series94);
                //drawInitiFun(chartDemo10, series100, series101, series102, series103, series104);
                //drawInitiFun(chartDemo11, series110, series111, series112, series113, series114);
                //drawInitiFun(chartDemo12, series120, series121, series122, series123, series124);
                //drawInitiFun(chartDemo13, series130, series131, series132, series133, series134);
                //drawInitiFun(chartDemo14, series140, series141, series142, series143, series144);
                //drawInitiFun(chartDemo15, series150, series151, series152, series153, series154);
                //drawInitiFun(chartDemo16, series160, series161, series162, series163, series164);
                //drawInitiFun(chartDemo17, series170, series171, series172, series173, series174);
                //drawInitiFun(chartDemo18, series180, series181, series182, series183, series184);
                //drawInitiFun(chartDemo19, series190, series191, series192, series193, series194);
                //drawInitiFun(chartDemo20, series200, series201, series202, series203, series204);

                num = 1;
            }

            if (Result1 != null && Result1 != " ")
            {

                if (num == 1)
                {
                    starttime = DateTime.Now;
                    num = 2;
                }
                endtime = DateTime.Now;
                double dc = ExecDateDiff(starttime, endtime);
                //xdc = (dc / 1000) + xdcTemp2;
                xdc = (endtime - starttime).TotalSeconds + xdcTemp2;
                Console.WriteLine(xdc);

                if (bianzuTrainNum == 1)
                {
                    if (data0 == 0)
                    {
                        drawFun(xdc - 500 * (drawCount - 1), data1, data2, data3, data4, data5, series10, series11, series12, series13, series14);
                        //drawFun(xdc, receiveData1[1], receiveData1[2], receiveData1[3], receiveData1[4], receiveData1[5], series10, series11, series12, series13, series14);
                    }
                }
                else if (bianzuTrainNum == 5)
                {
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

                }
                else if (bianzuTrainNum == 10)
                {

                }
                else if (bianzuTrainNum == 15)
                {

                }
                else
                {

                }

                if (xdc >= 500 * drawCount)
                {
                    drawCount++;
                }


            }
            else
            {
                MessageBox.Show("没有采集到数据", "Tips");
            }

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
            string[] dbTableStr = { "jl_pressure1","jl_pressure2", "jl_pressure3" , "jl_pressure4", "jl_pressure5", "jl_pressure6",
                "jl_pressure7", "jl_pressure8" , "jl_pressure9" , "jl_pressure10" , "jl_pressure11" , "jl_pressure12", "jl_pressure13",
                "jl_pressure14" , "jl_pressure15", "jl_pressure16", "jl_pressure17", "jl_pressure18", "jl_pressure19", "jl_pressure20"};

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
                        double dgv_sta = double.Parse(row.Cells[1].Value.ToString());
                        string dgv_time = row.Cells[2].Value.ToString();
                        double dgv_pre1 = double.Parse(row.Cells[3].Value.ToString());
                        double dgv_pre2 = double.Parse(row.Cells[4].Value.ToString());
                        double dgv_pre3 = double.Parse(row.Cells[5].Value.ToString());
                        double dgv_pre4 = double.Parse(row.Cells[6].Value.ToString());
                        double dgv_pre5 = double.Parse(row.Cells[7].Value.ToString());
                        string INS = @"insert into " + dbTableStr[n] + " (ID,位置,时间,列车管前端,副风缸,制动缸,列车管尾端,加缓风缸) values ("
                            + dgv_id + "," + dgv_sta + ",'" + dgv_time + "'," + dgv_pre1 + "," + dgv_pre2 + "," + dgv_pre3 + "," + dgv_pre4 + "," + dgv_pre5 + ")";

                        mycom = new MySqlCommand(INS, myconn);
                        mycom.ExecuteNonQuery();

                    }

                }
            }

            MessageBox.Show("数据存储完成！", "提示");
        }

        //时间格式
        public double ExecDateDiff(DateTime dateBegin, DateTime dateEnd)
        {
            TimeSpan ts1 = new TimeSpan(dateBegin.Ticks);
            TimeSpan ts2 = new TimeSpan(dateEnd.Ticks);
            TimeSpan ts3 = ts1.Subtract(ts2).Duration();

            //你想转的格式
            return ts3.TotalMilliseconds;
        }

        //模型初始化
        public void lcg_original_Num()
        {       
            Ps = 700;
            P0 = 100;
            L = 11;
            N = 1;
            D = 0.03175;

            if(bianzuNum==1)
            {
                S = 0.00000125;//单车
            }
            else if(bianzuNum==80)
            {
                S = 0.1072 * Math.Pow(10, -6);//80辆编组
            }
            else if(bianzuNum==100)
            {
                S = 0.1092 * Math.Pow(10, -6);//150辆编组
            }
            else
            {
                S = 0.1092 * Math.Pow(10, -6);//150辆编组
            }

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
            
            //R_ffg = S * Math.Sqrt(T) / (8.619 * 0.01 * V_ffg);
            //R_zdg = (k1 * S2 * Math.Sqrt(T)) / (8.619 * 0.01 * V_lcg);
            //Ps1 = Ps * Math.Exp(-R_zdg * t2);
            //Pz1 = Ps1 * S1 * Math.Sqrt(T) * t2 / (8.619 * 0.01 * Vz) + Pz0;
        }

        //列车管初充气模型函数
        public double lcg_modelFun(double t)
        {
            if (lcg_P >= 0 & lcg_P < 505)
            {
                @lcg_P = (Ps * Math.Sqrt(T) * S * t) / (8.619 * 0.01 * V_lcg) + P0;
                    //- (400 / Math.Pow(t0, 2)) * (Math.Pow(t, 2) - t0 * t);
            }

            else if (lcg_P >= 505)
            {
                lcg_P = 505;
            }
            return lcg_P;
        }

        //副风缸初充气模型函数
        private void ffg_modelFun(double t)
        {
            if (ffg_P >= 0 & ffg_P <= 0.528 * Ps)
            {
                @ffg_P = (P_lcg * Math.Sqrt(T) * S * t) / (8.619 * 0.01 * V_lcg) + Pf0;
            }
            else if (ffg_P >= 0.528 * Ps & ffg_P < 400)
            {
                //分段定义
                double R1 = Math.Sqrt(1 - Math.Pow(Pf0 / P_lcg, 1 / 3.5));
                double R2 = Math.Sqrt(T) * S * t / (0.1561 * V_lcg);
                @lcg_P = Math.Pow(1 - Math.Pow(R1 - R2, 2), 3.5) * Ps;
            }
            else if (ffg_P >= 400)
            {
                ffg_P = 400;
            }
            GC.Collect();
        }

        //初充气控制算法
        private void controlAlgor()
        {
            //con_lcg_Num = (int)((((lcg_P-100)*(1+(lcg_P-100-receiveData4)/receiveData4)) / 1000) * 4095);
            try
            {
                con_lcg_Num = (int)((lcg_modelFun(delta_T) - 100) * 4.238325);//1.035*4.095
                if (con_lcg_Num.ToString() != null)
                {
                    ComSend("b" + con_lcg_Num.ToString() + "\r\n");
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString(),"充气");
            }
        }

        //列车管制动模型函数
        private double jl_lcg_jianya_modelFun(double t)
        {
            if (t <= tw)
            {
                lcg_P = zd_P0;
            }
            else if (t > tw && t <= (tw + zd_t0))
            {
                lcg_P = zd_P0 * Math.Exp(-R_lcg * (t - tw));
            }
            else if (t > tw + zd_t0)
            {
                lcg_P = P1 * Math.Exp(-R_lcg * (t - tw - zd_t0));
            }
            return lcg_P;
        }

        //制动控制算法
        private void zd_controlAlgor()
        {
            ////排气速度太慢
            //if(zdConTag==1)
            //{
            //    zdBeginTime = DateTime.Now;
            //    zdConTag = 0;
            //}
            //DateTime zdNowTime = DateTime.Now;
            //double zdTime = (zdNowTime - zdBeginTime).TotalSeconds;
            ////jl_lcg_jianya_modelFun(zdTime);
            //try
            //{
            //    con_lcg_Num = (int)((jl_lcg_jianya_modelFun(zdTime) -100) * 4.23835);//1.035*4.095
            //    if (con_lcg_Num.ToString() != null)
            //    {
            //        ComSend("b" + con_lcg_Num.ToString() + "\r\n");
            //    }
            //}
            //catch(Exception e)
            //{
            //    MessageBox.Show(e.ToString(), "排气");
            //}

            ComSend("b0\r\n");
        }

        //S参数修正
        private void correctMethod()
        {
            double actualPre = 0;
            double deltaCqTime = (cqing - cqBeigin).TotalSeconds;
            actualPre = lcg_modelFun(deltaCqTime);
            double Ss = 0;

            double deltaPre = Math.Abs(receiveData1[4] + 100 - actualPre);
            double deltaPercent = deltaPre / receiveData1[4];
            if (deltaCqTime >= 10)
            {
                if (deltaPercent < 0.15)
                {
                    Ss = (9.678 * Math.Pow(10, -7)) * Math.Exp(0.006626 * trainNum) + (-8.957 * Math.Pow(10, -7) * Math.Exp(-0.06325 * trainNum));
                }
                else
                {
                    Ss = ((receiveData1[4] + 100) - P0) * (0.08619 * V_lcg) / (Ps * Math.Sqrt(T) * deltaCqTime);
                    if (cqTag == true)
                    {
                        SNumA.Add(Ss);
                    }
                }
            }

        }

        //计算Ss值
        private void calculateSs()
        {
            double SNumNum = SNumA.Count;
            double caseCount = 0;
            if(bianzuNum==1)
            {
                for (int n = 0; n < SNumA.Count; n++)
                {
                    correctS += double.Parse(SNumA[n].ToString());
                }
                correctS = correctS / SNumNum;
            }else if(bianzuNum==80)
            {

            }else if(bianzuNum==100)
            {

            }
            else
            {
                for (int n = 0; n < SNumA.Count; n += 20)
                {
                    correctS += double.Parse(SNumA[n].ToString());
                    caseCount++;
                }
                correctS = correctS / caseCount;
            }
            
            //MessageBox.Show("第" + trainNum + "辆车的S值为：" + correctS.ToString());
            SNumA.Clear();

            modelComparison mC = new modelComparison();
            mC.ShowDialog();

            calS = false;
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
            Console.WriteLine("setAddFileThread={0}", AppDomain.GetCurrentThreadId().ToString());
            while (Listen_Thread.IsAlive && sleepTag)
            {
                Thread.Sleep(12000);
            }
            socketListen();
            Console.WriteLine("tongxunceshi");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            jlTime.Clear();
            lcgBefore.Clear();
            ffgPre.Clear();
            zdgPre.Clear();
            lcgAfter.Clear();
            jhfgPre.Clear();

            ComSend("b0\r\n");//是否需要大排气
            Thread[] threads = { recDataThread1, recDataThread2, recDataThread3, recDataThread4 };
            foreach(Thread thread in threads)
            {
                try
                {
                    thread.Abort();
                }
                catch
                {

                }

            }

            if (conPort.IsOpen)
            {
                conPort.Close();
            }
            //myconn.Close();
            this.Close();
            //Environment.Exit(0);
            //uApplication.Exit();
            //停止线程
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //传递显示（逻辑还没有调好）
            //ccqjlkzDGV dgvForm = new ccqjlkzDGV();
            //dgvForm.ShowDialog();

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

                    socketListen();

                    //Listen_Thread = new Thread(new ThreadStart(SetAddFile1));
                    //Listen_Thread.IsBackground = true;
                    //Listen_Thread.Start();

                    btnStart.Enabled = false;
                    
                }
            }

        }

        private void jl_chartTimer_Tick(object sender, EventArgs e)
        {
            runNum++;
            //Console.WriteLine(runNum);

        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            xdcTemp1 = xdc;
            //pauseTimer.Start();
            //btnPasueTag = true;
            if(bianzuTrainNum==1||bianzuTrainNum==5)
            {
                recDataThread1.Suspend();
            }
            else if(bianzuTrainNum==10)
            {
                recDataThread1.Suspend();
                recDataThread2.Suspend();
            }
            else if(bianzuTrainNum==15)
            {
                recDataThread1.Suspend();
                recDataThread2.Suspend();
                recDataThread3.Suspend();
            }
            else
            {
                recDataThread1.Suspend();
                recDataThread2.Suspend();
                recDataThread3.Suspend();
                recDataThread4.Suspend();
            }

            dealThreadTag = false;
            dealD.Suspend();
            sendTCP("AB0501234\r\n", "AB0501234\r\n", "AB0501234\r\n", "AB0501234\r\n");
            btnContinue.Enabled = true;
            jl_chartTimer.Stop();

        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            //btnContinueTag = true;
            jl_chartTimer.Start();
            if(bianzuTrainNum==1)
            {
                sendTCP("AB110\r\n", "AB0501234\r\n", "AB0501234\r\n", "AB0501234\r\n");//开启单车的采集
                recDataThread1.Resume();
            }
            else if(bianzuTrainNum==5)
            {
                sendTCP("AB1501234\r\n", "AB0501234\r\n", "AB0501234\r\n", "AB0501234\r\n");
                recDataThread1.Resume();
            }
            else if(bianzuTrainNum==10)
            {
                sendTCP("AB1501234\r\n", "AB1501234\r\n", "AB0501234\r\n", "AB0501234\r\n");
                recDataThread1.Resume();
                recDataThread2.Resume();
            }
            else if(bianzuTrainNum==15)
            {
                sendTCP("AB1501234\r\n", "AB1501234\r\n", "AB1501234\r\n", "AB0501234\r\n");
                recDataThread1.Resume();
                recDataThread2.Resume();
                recDataThread3.Resume();
            }
            else
            {
                sendTCP("AB1501234\r\n", "AB1501234\r\n", "AB1501234\r\n", "AB0501234\r\n");
                recDataThread1.Resume();
                recDataThread2.Resume();
                recDataThread3.Resume();
                recDataThread4.Resume();
            }
            
            dealThreadTag = true;
            dealD.Resume();
            //pauseTimer.Stop();
            num = 1;
            xdcTemp2 = xdcTemp1;

        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            SerialPortForm F_SerialPortForm = new SerialPortForm();
            F_SerialPortForm.TransfEvent += F_SerialPortForm_TransfEvent;
            F_SerialPortForm.ShowDialog();
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            string fileName = "";
            string saveFileName = "";
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = "xlsx";
            saveDialog.Filter = "Excel文件|*.xlsx";
            saveDialog.FileName = fileName;
            saveDialog.ShowDialog();
            saveFileName = saveDialog.FileName;
            if (saveFileName.IndexOf(":") < 0) return; //被点了取消
            System.Reflection.Missing miss = System.Reflection.Missing.Value;
            //Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            dynamic xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                MessageBox.Show("无法创建Excel对象，您的电脑可能未安装Excel");
                return;
            }
            dynamic workbooks = xlApp.Workbooks;
            //Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
            dynamic workbook= workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            //Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            workbook.Sheets.Add(miss, workbook.Sheets[1], 19, miss);
            //Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[20];//取得sheet1 
            DataGridView[] dataGridView = {dataGridView1, dataGridView2, dataGridView3, dataGridView4, dataGridView5, dataGridView6,
            dataGridView7,dataGridView8,dataGridView9,dataGridView10,dataGridView11,dataGridView12,dataGridView13,dataGridView14,
            dataGridView15,dataGridView16,dataGridView17,dataGridView18,dataGridView19,dataGridView20};
            //写入标题             
            for (int n = 0; n < bianzuTrainNum; n++)
            {
                dynamic ws= workbook.Worksheets[n + 1];
                //Microsoft.Office.Interop.Excel.Worksheet ws = workbook.Worksheets[n + 1];
                for (int i = 0; i < dataGridView[n].ColumnCount; i++)
                {
                    ws.Cells[1, i + 1] = dataGridView[n].Columns[i].HeaderText;
                }
                //写入数值
                for (int r = 0; r < dataGridView1.Rows.Count; r++)
                {
                    for (int i = 0; i < dataGridView1.ColumnCount; i++)
                    {
                        ws.Cells[r + 2, i + 1] = dataGridView[n].Rows[r].Cells[i].Value;
                    }
                    System.Windows.Forms.Application.DoEvents();
                }
                ws.Columns.EntireColumn.AutoFit();//列宽自适应
            }
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

        private void btnCorrect_Click(object sender, EventArgs e)
        {
            //double S_Num = SNum.Count;
            //double S_Sum=0;
            //for (int i = 0; i < S_Num; i++)
            //{
            //    S_Sum += double.Parse(SNum[i].ToString());
            //}
            //double SS = S_Sum / S_Num;
            //MessageBox.Show("修正后的S值为" + SS, "Tip");
        }

        private void pauseTimer_Tick(object sender, EventArgs e)
        {
            //pauseNum++;
            //Console.WriteLine(pauseNum);
        }

    }

}
