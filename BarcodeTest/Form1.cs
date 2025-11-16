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
        BascolSerialPortStreamTest tester;

        public Form1()
        {
            InitializeComponent();

            this.btnClose.Enabled = false;
            this.btnReadData.Enabled = false;
        }

        // TestForm.cs
        private void btnTest1_Click(object sender, EventArgs e)
        {
           
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            tester = new BascolSerialPortStreamTest(txtPort.Text, 9600);
            bool result3 = tester.TestMethod3_SerialPortStream_DifferentBaudRates();

            if (result3)
            {
                this.btnOpen.Enabled = false;
                this.btnClose.Enabled = true;
                this.btnReadData.Enabled = true;
            }            
        }

        private void btnReadData_Click(object sender, EventArgs e)
        {
            // Test reading data
            int weight = tester.TestReadDataSerialPortStream();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (tester != null)
            {
                tester.Close();
                MessageBox.Show("Serial port closed.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.btnOpen.Enabled = true;
                this.btnClose.Enabled = false;
                this.btnReadData.Enabled = false;
            }
        }
    }
}
