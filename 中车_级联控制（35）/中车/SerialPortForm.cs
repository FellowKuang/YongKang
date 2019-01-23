using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace 中车
{
    public partial class SerialPortForm : Form
    {
        //声明委托 和 事件
        public delegate void TransfDelegate(String value1, String value2);
        public event TransfDelegate TransfEvent; 
        public SerialPortForm()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void SerialPortForm_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            cbPortNum.Items.AddRange(ports);
            cbPortNum.SelectedIndex = cbPortNum.Items.Count > 0 ? 0 : -1;
            cbBaud.SelectedIndex = 0;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            TransfEvent(cbPortNum.Text, cbBaud.Text);
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
