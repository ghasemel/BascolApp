using Mali.MaliControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BascolApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // TestForm.cs
        private void btnTest1_Click(object sender, EventArgs e)
        {
           
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            //var tester = new BascolCH340Test(txtPort.Text, 9600);
            //tester.RunAllTests();

            var tester = new BascolSerialPortStreamTest(txtPort.Text, 9600);

            // Test all methods
            //tester.RunAllSerialPortStreamTests();

            // Or test individual methods
            //bool result1 = tester.TestMethod1_SerialPortStream_NoControlSignals();
            //bool result2 = tester.TestMethod2_SerialPortStream_SetSignalsAfterOpen(false, false);
            bool result3 = tester.TestMethod3_SerialPortStream_DifferentBaudRates();

            // Test reading data
            int weight = tester.TestReadDataSerialPortStream();
        }

        private void btnReadData_Click(object sender, EventArgs e)
        {
          
        }
    }
}
