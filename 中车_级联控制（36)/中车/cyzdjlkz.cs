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
    public partial class cyzdjlkz : Form
    {
        MySqlConnection myconn = null;
        MySqlCommand mycom = null;
        DateTime starttime, endtime, ExeTime, GetDrawTime, DealTime;
        private static string beginTime, getTime1, getTime2;
        private static double delta_T;
        private int num = 1;
        public static int iNum;
        public static string receiveData;
        //1：列车管；2：副风缸；3：制动缸；4：缓解风缸
        public static double receiveData0, receiveData1, receiveData2, receiveData3, receiveData4, receiveData5;
        public static ArrayList SocketArr = new ArrayList();
        public static string[] arr;
        public static string Result = string.Empty;
        public static DataSet ds = new DataSet();
        public static Encoding encode = Encoding.UTF8;
        //public static Encoding encode = Encoding.Default;
        public static DataTable dt = new DataTable("jlkz_DtPressure");
        Thread DS_Thred, Listen_Thred, recDataThread;
        Socket acceptSocket;
        public delegate void AddFile();
        public delegate void Draw_SaveDelegate();
        public delegate void ListenDelegate(String a);
        public delegate void dealsocket(Socket s);
        public static ArrayList jlkz_list_Beigin = new ArrayList();
        public static ArrayList jlkz_list_D1 = new ArrayList();
        public static ArrayList jlkz_list_D2 = new ArrayList();
        public static ArrayList jlkz_list_D3 = new ArrayList();
        public static ArrayList jlkz_list_D4 = new ArrayList();
        public static ArrayList jlkz_list_D5 = new ArrayList();
        public static ArrayList jlkz_list_D6 = new ArrayList();

        private string cbPortName = "";
        private string cbBaud = "";
        private static double countNum1 = 0, countNum2 = 0;

        Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static Thread dealD;

        private double lcg_P, zdg_P, Ps, S, V_lcg, P0, t0, tw, D, Pf0, ffg_P, P1, S1, k, Pf, V_ffg, V_zdg, P_lcg;
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
        private double Ps1, k1, S2, Pz0, t2, Pz1, Vz, t1, t3, zd_P0, zd_t0, zd_Relife;
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
        //zd_Relife：减压量
        //zd_P0：制动开始时列车管初始绝对压力
        private double T = 298, N, L, W, zd_t = 0.01, jl_zd_PreNum, sendCount=0;
        //zd_t：时间
        //T：绝对温度
        //N：车辆位置
        //L：列车管长度
        //W：空气波速
        private double hj_lcg_P, hj_t0=5, hj_t1=0.2, hj_t2=0.5, Ps2, hj_P1, hj_P2, hj_k;
        //hj_lcg_P：再充气缓解过程列车管气压
        //hj_t0：保压阶段时间（5s）
        //hj_t1：活塞移动时间（0.2s）
        //hj_t2：局部增压作用时间（0.5s）
        //Ps2：加速缓解风缸绝对压力
        //hj_P1：主活塞移动到缓解位时列车管的绝对压力
        //hj_P2：加速缓解作用结束时列车管的绝对压力
        //hj_k：模拟列车管充气实际情况，自定义节流喷嘴有效截面积变化参数
        private double R_lcg, R_ffg, R_zdg;
        public int con_lcg_Num, con_ffg_Num, jl_tag = 0, list_Num = 1, arrTag = 1, cqCountNum=0, arrLength=0;
        //SerialPort conPort;
        private SerialPort conPort = new SerialPort();

        public cyzdjlkz()
        {
            InitializeComponent();
            model_original_Num();
        }

        private void cyzdjlkz_Load(object sender, EventArgs e)
        {
            this.cbZdLevel.SelectedIndex = 0;
            btnContine.Enabled = false;

            //myconn = new MySqlConnection("Data Source =localhost;Database=barking;Username=root;Password=123456");   //本地连接
            ////myconn = new MySqlConnection("Data Source =192.168.1.105; Database=barking;User=zhongche;Password=0728");  //异地连接
            //myconn.Open();
            ////Console.WriteLine("ccb={0}", AppDomain.GetCurrentThreadId().ToString());
        }

        public void InitChart()
        {
            DateTime time = DateTime.Now;
            cyzdjl_chartTimer.Interval = 1000;
            cyzdjl_chartTimer.Tick += cyzdjl_chartTimer_Tick;
            chartDemo.DoubleClick += chartDemo_DoubleClick;
            chartDemo.ChartAreas[0].AxisX.LabelStyle.Format = "F";
            //chartDemo1.ChartAreas[0].AxisX.ScaleView.Size = 5;
            chartDemo.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            cyzdjl_chartTimer.Start();
        }

        private void deal(object socket)
        {
            //保持连接不中断（与下位机配合）
            Socket s = (Socket)socket;
            //while (true)
            //{
            //    receiveData = Receive(s, 0); //5 seconds timeout.
            //    //s.Send(encode.GetBytes("ok"));
            //    //DestroySocket(s); //import
            //    ListenDelegate Lin = new ListenDelegate(draw);
            //    this.Invoke(Lin, receiveData);
            //}
        }

        public void SetAddFile()
        {
            Listen();
        }

        //端口监听
        public void Listen()
        {
            //int port;
            //Console.WriteLine("lll={0}", AppDomain.GetCurrentThreadId().ToString());
            // Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(new IPEndPoint(IPAddress.Any, 8087));
            listenSocket.Listen(100);
            //Console.WriteLine("Listen " + port + " ...");

            acceptSocket = listenSocket.Accept();
            //Thread dealD = new Thread(deal);
            dealD = new Thread(deal);
            dealD.IsBackground = true;
            dealD.Start(acceptSocket);

            sendTCP("AB2020\r\n");

            recDataThread = new Thread(dataReceive);
            recDataThread.IsBackground = true;
            recDataThread.Start();

        }

        //数据接收线程
        private void dataReceive()
        {
            string[] arr = null;

            //按行读取
            NetworkStream ntwStream = new NetworkStream(acceptSocket);
            StreamReader strmReader = new StreamReader(ntwStream);

            while (true)
            {
                Result = strmReader.ReadLine();
                Console.WriteLine(Result);

                if (Result != null)
                {
                    arr = Result.Split('/');

                    if (arr[0] != null && arr[1] != null && arr[2] != null && arr[3] != null && arr[4] != null && arr[5] != null)
                    {
                        receiveData0 = double.Parse(arr[0]);
                        receiveData1 = Math.Abs(250 * (2 * double.Parse(arr[1]) / 6553.5) - 250);//0-1mPa,0-5V
                        receiveData2 = 200 * (double.Parse(arr[2]) / 6553.5);
                        receiveData3 = 200 * (double.Parse(arr[3]) / 6553.5);
                        receiveData4 = Math.Abs(200 * (double.Parse(arr[4]) / 6553.5));
                        receiveData5 = 200 * (double.Parse(arr[5]) / 6553.5);
                        //接收到数据再进行委托
                        ListenDelegate Lin = new ListenDelegate(draw);
                        this.Invoke(Lin, receiveData);
                    }
                    else
                    {
                        MessageBox.Show("没有采集到数据");
                    }

                    while (jl_tag == 0)
                    {
                        DealTime = DateTime.Now;
                        delta_T = (DealTime - ExeTime - (starttime - ExeTime)).TotalSeconds;
                        num++;

                        lcg_modelFun(delta_T);
                        //ffg_modelFun(delta_T);
                        cq_controlAlgor();//调用充气控制算法

                        if (receiveData4 >= 390)
                        {
                            countNum1++;
                            break;
                        }

                        //判断充气是否充满
                        while (receiveData4 >= 390 && countNum1 == 200)
                        {
                            //初始化排气模型函数参数
                            zd_P0 = receiveData4 + 100;
                            S1 = k * S;
                            R_lcg = (S1 * Math.Sqrt(T)) / (8.619 * 0.01 * V_lcg);
                            P1 = zd_P0 * Math.Exp(-R_lcg);
                            jl_tag = 1;//制动排气标志
                            break;
                        }
                        break;
                    }

                    while (jl_tag == 1 || jl_tag == 2 || jl_tag == 3)
                    {
                        if (jl_tag == 1 && receiveData2 >= (zd_P0 - zd_Relife - 100))//根据制动模型进行第一次排气
                        {

                            jlkz_list_D1.Add(receiveData1);
                            jlkz_list_D2.Add(receiveData2);
                            jlkz_list_D3.Add(receiveData3);
                            jlkz_list_D4.Add(receiveData4);
                            jlkz_list_D5.Add(receiveData5);

                            zd_controlAlgor();

                            if (receiveData4 <= (zd_P0 - zd_Relife - 100 + 5))
                            {
                                int arrList_Num = jlkz_list_D4.Count - 1;
                                double peakPre = double.Parse(jlkz_list_D4[arrList_Num].ToString());
                                jlkz_list_D4.Add(peakPre);
                                countNum2++;
                            }

                            if (receiveData2 <= (zd_P0 - zd_Relife + 1) && countNum2 == 50)
                            {
                                countNum2 = 0;
                                arrTag = 2;
                                jl_tag = 2;//循环充气的标志
                                break;
                            }
                        }

                        if (jl_tag == 2 && receiveData2 <= 500)
                        {
                            //开始循环充气
                            ComSend("b" + 2048 + "\r\n");//直接充到500
                            num++;
                         

                            while (receiveData2 >= 498)
                            {
                                cqCountNum++;
                            }

                            while (cqCountNum == 50 && receiveData2 >= 498)
                            {
                                cqCountNum = 0;
                                jl_tag = 3;//完成充气过程进入排气，循环
                                zd_t = 0;
                                break;
                            }
                            break;

                        }

                        if (jl_tag == 3 && receiveData2 >= 10)//排气
                        {
                            if (arrTag == 1)//调用不同的数组
                            {
                                arrLength = jlkz_list_D4.Count;
                                jlkz_list_D6.Add(receiveData2);
                            }
                            else
                            {
                                arrLength = jlkz_list_D6.Count;
                                jlkz_list_D4.Add(receiveData2);
                            }


                            //根据数组排气
                            if (arrTag == 1)
                            {
                                if (jlkz_list_D4[list_Num - 1] != null)
                                {
                                    jl_zd_PreNum = double.Parse(jlkz_list_D4[list_Num - 1].ToString());

                                }
                                if (jl_zd_PreNum.ToString() != null)
                                {
                                    ComSend("b" + (jl_zd_PreNum / 1000 * 4095).ToString() + "\r\n");
                                    //Thread.Sleep(1000);
                                }
                            }
                            else
                            {
                                if (jlkz_list_D6[list_Num - 1] != null)
                                {
                                    jl_zd_PreNum = double.Parse(jlkz_list_D6[list_Num - 1].ToString());

                                }
                                if (jl_zd_PreNum.ToString() != null)
                                {
                                    ComSend("b" + (jl_zd_PreNum / 1000 * 4095).ToString() + "\r\n");
                                    //Thread.Sleep(1000);
                                }
                            }
                            list_Num++;

                            num++;
                           

                            while (receiveData4 <= 10 && list_Num == arrLength)
                            {
                                if (arrTag == 1)
                                {
                                    arrTag = 2;
                                    jlkz_list_D4.Clear();
                                }
                                else
                                {
                                    arrTag = 1;
                                    jlkz_list_D6.Clear();
                                }
                                jl_tag = 2;//跳转到直接充气过程
                                list_Num = 1;
                            }
                        }
                    }



                }
                else
                {
                    MessageBox.Show("没有采集到数据");
                }

            }
        }

        //UI画图
        public void draw(string data)
        {
            //  
            if (num == 1)
            {
                starttime = DateTime.Now;
                getTime1 = DateTime.Now.ToString("HH:mm:ss.fff");
                //textBox2.Text = getTime1;
                GetDrawTime = DateTime.Now;
            }
            endtime = DateTime.Now;
            double dc = ExecDateDiff(starttime, endtime);
            double xdc = dc / 1000;

            num++;
            //对tabControl中的chartDemo1画图
            Series series0 = chartDemo.Series[0];
            Series series1 = chartDemo.Series[1];
            Series series2 = chartDemo.Series[2];
            Series series3 = chartDemo.Series[3];
            Series series4 = chartDemo.Series[4];
            //画点
            series0.Points.AddXY(xdc, receiveData1);
            series1.Points.AddXY(xdc, receiveData2);
            series2.Points.AddXY(xdc, receiveData3);
            series3.Points.AddXY(xdc, receiveData4);
            series4.Points.AddXY(xdc, receiveData5);

            if (series0.Points.Count > 300 || series1.Points.Count > 300 || series2.Points.Count > 300 || series3.Points.Count > 300 || series4.Points.Count > 300)
            {
                //会有误差
                chartDemo.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                chartDemo.ChartAreas[0].AxisX.ScaleView.Position = series0.Points.Count / 10 - 30;
                chartDemo.ChartAreas[0].AxisX.ScaleView.Position = series1.Points.Count / 10 - 30;
                chartDemo.ChartAreas[0].AxisX.ScaleView.Position = series2.Points.Count / 10 - 30;
                chartDemo.ChartAreas[0].AxisX.ScaleView.Position = series3.Points.Count / 10 - 30;
                chartDemo.ChartAreas[0].AxisX.ScaleView.Position = series4.Points.Count / 10 - 30;
            }

        }

        //TCP发送指令给下位机
        public void sendTCP(string str)
        {
            acceptSocket.Send(encode.GetBytes(str));
        }

        //模型初始化
        private void model_original_Num()
        {
            Ps = 700;
            P0 = 100;
            L = 11;
            N = 1;
            D = 0.03175;
            S = 0.00000125;//单车
            //S = 0.1072 * Math.Pow(10, -6);//80辆编组
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

        }

        //列车管初充气模型函数
        private void lcg_modelFun(double t)
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

        //列车管减压模型函数
        private void lcg_jianya_modelFun(double t)
        {
            if (t <= tw)
            {
                lcg_P = zd_P0;
            }
            else if (t > tw && t <= (tw + t0))
            {
                lcg_P = zd_P0 * Math.Exp(-R_lcg * (t - tw));
            }
            else if (t > tw + t0)
            {
                lcg_P = P1 * Math.Exp(-R_lcg * (t - tw - t0));
            }
        }

        //列车管在充气缓解过程模型函数
        private void hj_lcg_modelFun(double t)
        {
            if (t >= 0 & t <= t0)
            {
                hj_lcg_P = P0;
            }
            else if (t >= hj_t0 & t <= hj_t0 + hj_t1)
            {
                hj_lcg_P = Ps * S * Math.Sqrt(T) * (t - hj_t0) / (8.619 * 0.01 * V_lcg) + P0;
            }
            else if (t >= hj_t0 + hj_t1 & t <= hj_t0 + hj_t1 + hj_t2)
            {
                hj_lcg_P = (Ps + Ps2) * Math.Sqrt(T) * hj_k * S * (t - hj_t0 - hj_t1) / (8.619 * 0.01 * V_lcg) + hj_P1;
            }
            else
            {
                hj_lcg_P = Ps * S * Math.Sqrt(T) * (t - hj_t0 - hj_t1 - hj_t2) / (8.619 * 0.01 * V_lcg) + hj_P2;
            }
        }

        //初充气控制算法
        private void cq_controlAlgor()
        {
            con_lcg_Num = (int)(((lcg_P-100) / 1000) * 4095);
            if (con_lcg_Num.ToString() != null)
            {
                ComSend("b" + con_lcg_Num.ToString() + "\r\n");
            }
        }

        //制动控制算法
        private void zd_controlAlgor()
        {
           
            if (receiveData4 >= zd_Relife)
            {
                if (double.Parse(jlkz_list_Beigin[0].ToString()) - receiveData4 < zd_Relife)
                {
                    lcg_jianya_modelFun(zd_t);
                    con_lcg_Num = (int)((lcg_P-100) / 1000 * 4095);
                    if (con_lcg_Num.ToString() != null)
                    {
                        if (con_lcg_Num.ToString() != null)
                        {
                            ComSend("b" + con_lcg_Num.ToString() + "\r\n");
                            zd_t = zd_t + 0.1;
                        }
                    }
                }
                if (double.Parse(jlkz_list_Beigin[0].ToString()) - receiveData4 >= (zd_Relife - 1))
                {
                    jlkz_list_Beigin.Clear();
                }

            }
            else
            {
                MessageBox.Show("气压不足" + zd_Relife + "kpa", "提示");
            }


        }

        //串口传递
        private void F_SerialPortForm_TransfEvent(string ComName, string ComBaud)
        {
            cbPortName = ComName;
            cbBaud = ComBaud;
        }

        //串口发送指令
        private void ComSend(string CommandStr)
        {
            byte[] WriterBuffer = Encoding.ASCII.GetBytes(CommandStr);
            conPort.Write(WriterBuffer, 0, WriterBuffer.Length); //退出提示“未将对象实例化”bug
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

        private void cyzdjl_chartTimer_Tick(object sender, EventArgs e)
        {

        }

        private void chartDemo_DoubleClick(object sender, EventArgs e)
        {
            chartDemo.ChartAreas[0].AxisX.ScaleView.Size = 1;
            chartDemo.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
        }

        private void chartDemo_Click(object sender, EventArgs e)
        {
            chartDemo.ChartAreas[0].AxisX.ScaleView.Size = 1;
            chartDemo.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartDemo.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            string zdLevel = cbZdLevel.Text;
            if (zdLevel.Trim().Equals("一级制动"))
            {
                zd_Relife = 40;
                jlkz_list_Beigin.Add(receiveData4);
            }
            else if (zdLevel.Trim().Equals("二级制动"))
            {
                zd_Relife = 57;
                jlkz_list_Beigin.Add(receiveData4);
            }
            else if (zdLevel.Trim().Equals("三级制动"))
            {
                zd_Relife = 74;
                jlkz_list_Beigin.Add(receiveData4);
            }
            else if (zdLevel.Trim().Equals("四级制动"))
            {
                zd_Relife = 91;
                jlkz_list_Beigin.Add(receiveData4);
            }
            else if (zdLevel.Trim().Equals("五级制动"))
            {
                zd_Relife = 108;
                jlkz_list_Beigin.Add(receiveData4);
            }
            else if (zdLevel.Trim().Equals("六级制动"))
            {
                zd_Relife = 125;
                jlkz_list_Beigin.Add(receiveData4);
            }
            else
            {
                zd_Relife = 140;
                jlkz_list_Beigin.Add(receiveData4);
            }

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
                    Listen_Thred = new Thread(new ThreadStart(SetAddFile));
                    Listen_Thred.IsBackground = true;
                    Listen_Thred.Start();
                    beginTime = DateTime.Now.ToString("HH;mm;ss.fff");
                    //textBox1.Text = beginTime;
                    ExeTime = DateTime.Now;
                    btnStart.Enabled = false;
                }
            }



        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            SerialPortForm F_SerialPortForm = new SerialPortForm();
            F_SerialPortForm.TransfEvent += F_SerialPortForm_TransfEvent;
            F_SerialPortForm.ShowDialog();
        }



    }
}
