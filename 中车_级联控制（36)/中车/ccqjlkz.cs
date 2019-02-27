
//version.17    单线程采集 

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
        private static double delta_T, runNum,xdc,xdcTemp1,xdcTemp2, dataNum=1;
        //xdc:横坐标；xdcTemp1、xdcTemp2：暂停时的临时存储
        private int num = 1, beginTag = 1, dataDealTag=1;
        public static int iNum, trainNumCase, zdConTag=1;
        MySqlConnection myconn = null;
        MySqlCommand mycom = null;
        public static string receiveData;
        //1：均衡风缸；2：副风缸；3：制动缸；4：列车管；5：加缓风缸
        public static double receiveData0, receiveData1, receiveData2, receiveData3, receiveData4, receiveData5;
        public static bool addDataTag = true, sleepTag=false, btnPasueTag=false, btnContinueTag=false, dealThreadTag=true;
        public static string Result = string.Empty;
        public static Encoding encode = Encoding.UTF8;
        Socket acceptSocket1, acceptSocket2, acceptSocket3, acceptSocket4;
        public static DataSet ds = new DataSet();
        public static DataTable dt = new DataTable("jl_ccq_DtPressure");
        public delegate void ListenDelegate(String a);
        public delegate void SavaDelegate();
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

        Series series0, series1, series2, series3, series4;
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
            InitChart();
            lcg_original_Num();
        }

        private void ccqjlkz_Load(object sender, EventArgs e)
        {
            bianzuNum = jlSetting.BIANZU;
            bianzuTrainNum = jlSetting.TRAIN;

             series0 = chartDemo.Series[0];
             series1 = chartDemo.Series[1];
             series2 = chartDemo.Series[2];
             series3 = chartDemo.Series[3];
             series4 = chartDemo.Series[4];

            this.cbTrainNum.SelectedIndex = 0;

            //创建DataGridView的列
            Console.WriteLine("loadThread={0}", AppDomain.GetCurrentThreadId().ToString());
            DataColumn dtc = new DataColumn();
            dtc = new DataColumn("ID", typeof(Int16));
            dt.Columns.Add(dtc);
            dtc = new DataColumn("位置", typeof(Int16));
            dt.Columns.Add(dtc);
            dtc = new DataColumn("时间", typeof(String));
            dt.Columns.Add(dtc);
            dtc = new DataColumn("列前", typeof(Double));
            dt.Columns.Add(dtc);
            dtc = new DataColumn("副风缸", typeof(Double));
            dt.Columns.Add(dtc);
            dtc = new DataColumn("制动缸", typeof(Double));
            dt.Columns.Add(dtc);
            dtc = new DataColumn("列尾", typeof(Double));
            dt.Columns.Add(dtc);
            dtc = new DataColumn("加缓缸", typeof(Double));
            dt.Columns.Add(dtc);
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            dataGridView1.DataSource = ds.Tables["jl_ccq_DtPressure"].DefaultView;
            //dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
            //dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[0];
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;

            //myconn = new MySqlConnection("Data Source =localhost;Database=barking;Username=root;Password=123456");   //本地连接
            ////myconn = new MySqlConnection("Data Source =192.168.1.105; Database=barking;User=zhongche;Password=0728");  //异地连接
            //myconn.Open();

            btnContinue.Enabled = false;
        }

        public void InitChart()
        {
            //DateTime time = DateTime.Now;
            //pauseTimer.Interval = 1;
            //pauseTimer.Stop();

            jl_chartTimer.Interval = 1000;
            jl_chartTimer.Tick += jl_chartTimer_Tick;
            //chartDemo.DoubleClick += chartDemo_DoubleClick;
            chartDemo.ChartAreas[0].AxisX.LabelStyle.Format = "F";
            chartDemo.ChartAreas[0].AxisX.ScaleView.Size = 300;
            chartDemo.ChartAreas[0].AxisY.ScaleView.Size = 600;
            //chartDemo.ChartAreas[0].AxisX.Interval = 10;
            chartDemo.ChartAreas[0].AxisX.Minimum = 0;
            chartDemo.ChartAreas[0].AxisY.Minimum = 0;
            //chartDemo.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            //chartDemo.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            //chartDemo.ChartAreas[0].CursorX.AutoScroll = true;

            chartDemo.ChartAreas[0].CursorX.IsUserEnabled = false;//禁止游标

            chartDemo.ChartAreas[0].AxisX.Title = "时间 / s";
            chartDemo.ChartAreas[0].AxisY.Title = "气压 / kPa";

            jl_chartTimer.Start();

        }

        private void deal(object socket)
        {  
            Socket s = (Socket)socket;

            //保持连接不中断（与下位机配合） 
            ListenDelegate Lin = new ListenDelegate(draw);
            SavaDelegate saveDel = new SavaDelegate(dataSave);

            while (dealThreadTag)
            {
                this.Invoke(Lin, receiveData);//采集界面退出提示错误   

                this.Invoke(saveDel);
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
        public void  dataReceive()
        {
            Thread dealDataThread1=null;
            //按行读取
            NetworkStream ntwStream = new NetworkStream(acceptSocket);
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
                Result = strmReader.ReadLine();
                Console.WriteLine(Result);
                DealTime = DateTime.Now;//数据解析完成的时间
                double db_T = (DealTime - beginTime).TotalSeconds + xdcTemp2;//为了保证采集到数据的横坐标与模型横坐标一致

                //用于释放计算S值的线程
                while (!calS)
                {
                    calSval.Abort();
                    calS = true;
                    break;
                }

                if (Result != null)
                {
                    arr = Result.Split('/');

                    if (arr[0].Equals("A") && arr[1] != null && arr[2] != null && arr[3] != null && arr[4] != null && arr[5] != null && arr[6] != null)
                    {
                        receiveData0 = double.Parse(arr[1]);
                        receiveData1 = Math.Abs(250 * (double.Parse(arr[2]) / 3276.8) - 250);//0-1mPa,0-5V
                        receiveData2 = Math.Abs(100 * (double.Parse(arr[3]) / 3276.8));
                        receiveData3 = Math.Abs(100 * (double.Parse(arr[4]) / 3276.8));
                        receiveData4 = Math.Abs(100 * (double.Parse(arr[5]) / 3276.8));
                        receiveData5 = Math.Abs(100 * (double.Parse(arr[6]) / 3276.8));

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

                dataRecTag = false;
            }                    
            
        }

        public void dataDeal()
        {
            dataDealTag = 0;
            if (receiveData0 == 0)
            {
                if (jl_tag == 0)
                {
                    //根据模型进行充气，一次循环中只会在开始阶段执行一次
                    dataDealTime = DateTime.Now;//数据解析完成的时间
                    delta_T = (dataDealTime - beginTime).TotalSeconds + xdcTemp2;//为了保证采集到数据的横坐标与模型横坐标一致
                    
                    lcg_modelFun(delta_T);
                    controlAlgor();//调用控制算法

                    ccqjlkz_list_D4.Add(receiveData4);

                    if (receiveData4 >= 399 && countNum<200)
                    {
                        countNum++;
                    }

                    if (receiveData4 >= 399 && countNum == 200)
                    {
                        cqTag = false;
                        zd_P0 = receiveData4 + 100;//模型中的气压应该为绝对压力
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
                    if (jl_tag == 1 && receiveData4 >= 5)
                    {
                        /*开始排气到0kpa(表压)*/
                        zd_controlAlgor();//排气到110kpa（大气压）

                        if (receiveData4 <= 10)
                        {
                            zdConTag = 1;//为下次排气做准备
                            jl_tag = 2;
                            cqTag = true;
                        }
                        trainNumTag = true;
                    }

                    //排气到0kpa开始对第二辆车充气
                    else if (jl_tag == 2 && receiveData4 <= 405)//（初始大气压101kpa）
                    {
                        while (trainNumTag)
                        {
                            trainNum++;
                            SNumTag = 0;
                            trainNumTag = false;
                            break;
                        }

                        if (arrTag == 1)//调用不同的数组
                        {
                            arrLength = ccqjlkz_list_D4.Count;
                            ccqjlkz_list_D6.Add(receiveData4);
                        }
                        else
                        {
                            arrLength = ccqjlkz_list_D6.Count;
                            ccqjlkz_list_D4.Add(receiveData4);
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
                                catch(Exception e)
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
                                catch(Exception e)
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
                            if (receiveData4 >= 395)
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

                                zd_P0 = receiveData4 + 100;
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

        //数据存储到表格
        public void dataSave()
        {
            //添加行
            DataRow dtr = dt.NewRow();
            dtr["ID"] = dataNum;
            if (cqTag == true)
            {
                dtr["位置"] = trainNum;
            }
            else
            {
                dtr["位置"] = 0;
            }
            dtr["时间"] = DateTime.Now.ToString("MM-dd HH:mm:ss.fff");
            dtr["列前"] = double.Parse(string.Format("{0:F}", receiveData1));
            dtr["副风缸"] = double.Parse(string.Format("{0:F}", receiveData2));
            dtr["制动缸"] = double.Parse(string.Format("{0:F}", receiveData3));
            dtr["列尾"] = double.Parse(string.Format("{0:F}", receiveData4));
            dtr["加缓缸"] = double.Parse(string.Format("{0:F}", receiveData5));

            dt.Rows.Add(dtr);
            
            foreach (DataGridViewColumn item in dataGridView1.Columns)
            {
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            dataNum++;
        }

        //主线程UI
        public void draw(object socket)
        {
            //Series series0 = chartDemo.Series[0];
            //Series series1 = chartDemo.Series[1];
            //Series series2 = chartDemo.Series[2];
            //Series series3 = chartDemo.Series[3];
            //Series series4 = chartDemo.Series[4];
            //Series series5 = chartDemo.Series[5];            //模型曲线
            if (Result != null && Result!=" ")
            {
                if (receiveData0 == 0)
                {
                    if (num == 1)
                    {
                        starttime = DateTime.Now;
                        num = 2;
                    }
                    endtime = DateTime.Now;
                    double dc = ExecDateDiff(starttime, endtime);
                    //xdc = (dc / 1000) + xdcTemp2;
                    xdc = (endtime - starttime).TotalSeconds-100*(drawCount-1);
                    Console.WriteLine(xdc);

                    //if (series0.Points.Count > 600 && xdc>=90)
                    //{
                    //    chartDemo.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    //    chartDemo.ChartAreas[0].AxisX.ScaleView.Position = xdc - 90;
                    //    chartDemo.ChartAreas[0].AxisX.ScaleView.Position = xdc - 90;
                    //    chartDemo.ChartAreas[0].AxisX.ScaleView.Position = xdc - 90;
                    //    chartDemo.ChartAreas[0].AxisX.ScaleView.Position = xdc - 90;
                    //    chartDemo.ChartAreas[0].AxisX.ScaleView.Position = xdc - 90;
                    //}

                    //对tabControl中的chartDemo1画图                                      
                    //画点
                    series0.Points.AddXY(xdc, receiveData1);
                    series1.Points.AddXY(xdc, receiveData2);
                    series2.Points.AddXY(xdc, receiveData3);
                    series3.Points.AddXY(xdc, receiveData4);
                    series4.Points.AddXY(xdc, receiveData5);        
                    //series5.Points.AddXY(xdc, lcg_P);

                    if (xdc >= 300)
                    {
                        series0.Points.Clear();
                        series1.Points.Clear();
                        series2.Points.Clear();
                        series3.Points.Clear();
                        series4.Points.Clear();

                        //chartDemo.ChartAreas.Dispose();
                        //chartDemo.ChartAreas.Clear();
                        //chartDemo.ChartAreas[0].AxisX.Minimum = 100 * drawCount;
                        drawCount++;
                        label1.Text = ("+" + 100 * (drawCount - 1));
                        //chartDemo.ChartAreas[0].AxisX.Maximum = 100 * drawCount;
                    }

                    drawTag++;
                    Console.WriteLine("draw" + "   " + drawTag + "  " + receiveData4);

                    series0.Points.Dispose();
                    series1.Points.Dispose();
                    series2.Points.Dispose();
                    series3.Points.Dispose();
                    series4.Points.Dispose();

                    GC.Collect();
                }
            }
            else
            {
                MessageBox.Show("没有采集到数据", "Tips");
            }                  

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
        public void lcg_modelFun(double t)
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
                con_lcg_Num = (int)((lcg_P - 100) * 4.238325);//1.035*4.095
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
            if(zdConTag==1)
            {
                zdBeginTime = DateTime.Now;
                zdConTag = 0;
            }
            DateTime zdNowTime = DateTime.Now;
            double zdTime = (zdNowTime - zdBeginTime).TotalSeconds;
            //jl_lcg_jianya_modelFun(zdTime);
            try
            {
                con_lcg_Num = (int)((jl_lcg_jianya_modelFun(zdTime) -100) * 4.23835);//1.035*4.095
                if (con_lcg_Num.ToString() != null)
                {
                    ComSend("b" + con_lcg_Num.ToString() + "\r\n");
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString(), "排气");
            }
        }

        //S参数修正
        private void correctMethod()
        {
            double deltaCqTime = (cqing - cqBeigin).TotalSeconds;
            lcg_modelFun(deltaCqTime);
            double Ss = 0;


            double deltaPre = Math.Abs(receiveData4 + 100 - lcg_P);
            double deltaPercent = deltaPre / receiveData4;
            if (deltaCqTime >= 10)
            {
                if (deltaPercent < 0.15)
                {
                    Ss = (9.678 * Math.Pow(10, -7)) * Math.Exp(0.006626 * trainNum) + (-8.957 * Math.Pow(10, -7) * Math.Exp(-0.06325 * trainNum));
                }
                else
                {
                    Ss = ((receiveData4 + 100) - P0) * (0.08619 * V_lcg) / (Ps * Math.Sqrt(T) * deltaCqTime);
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
            if(trainNumCase==0)
            {
                for (int n = 0; n < SNumA.Count; n++)
                {
                    correctS += double.Parse(SNumA[n].ToString());
                }
                correctS = correctS / SNumNum;
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            jlTime.Clear();
            lcgBefore.Clear();
            ffgPre.Clear();
            zdgPre.Clear();
            lcgAfter.Clear();
            jhfgPre.Clear();

            ComSend("b0\r\n");//是否需要大排气

            if(recDataThread!=null)
            {
                if(recDataThread.IsAlive)
                {
                    recDataThread.Abort();
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

                    if (cbTrainNum.Text.Trim().Equals("单车"))
                    {
                        trainNumCase = 0;
                    }
                    else if(cbTrainNum.Text.Trim().Equals("80辆编组"))
                    {
                        trainNumCase = 1;
                    }
                    else
                    {
                        trainNumCase = 2;
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

        //private void chartDemo_Click(object sender, EventArgs e)
        //{
        //    chartDemo.ChartAreas[0].AxisX.ScaleView.Size = 1;
        //    chartDemo.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
        //    chartDemo.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
        //}

        //private void chartDemo_DoubleClick(object sender, EventArgs e)
        //{
        //    chartDemo.ChartAreas[0].AxisX.ScaleView.Size = 1;
        //    chartDemo.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
        //    chartDemo.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
        //}

        private void btnPause_Click(object sender, EventArgs e)
        {
            xdcTemp1 = xdc;
            //pauseTimer.Start();
            //btnPasueTag = true;
            recDataThread.Suspend();
            dealThreadTag = false;
            dealD.Suspend();
            sendTCP("a\r\n");
            btnContinue.Enabled = true;
            jl_chartTimer.Stop();

        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            //btnContinueTag = true;
            jl_chartTimer.Start();
            sendTCP("b\r\n");
            recDataThread.Resume();
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
