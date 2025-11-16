using System;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using RJCP.IO.Ports; // SerialPortStream namespace

namespace Mali.MaliControls
{
    /// <summary>
    /// Test class using SerialPortStream library for CH340 compatibility
    /// This bypasses .NET's problematic SerialPort class
    /// </summary>
    public class BascolSerialPortStreamTest
    {
        private SerialPortStream serialPort;

        private string portName;

        public BascolSerialPortStreamTest(string portName, int baudRate = 9600)
        {
            this.portName = portName;
            serialPort = new SerialPortStream(portName, baudRate)
            {
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One,
                Handshake = Handshake.None,
                Encoding = Encoding.ASCII,
                ReadTimeout = 5000,
                WriteTimeout = 5000
            };
        }

        public void Close()
        {
            if (serialPort != null)
            {
                string portName = serialPort.PortName;

                if (serialPort.IsOpen)
                {
                   // MessageBox.Show("Closing...",
                   //   "SerialPortStream Test 3", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    try
                    {
                        // Reset DTR and RTS signals before closing - this resets CH340 driver state
                        serialPort.DtrEnable = false;
                        serialPort.RtsEnable = false;

                        // Discard any pending data in buffers
                        serialPort.DiscardInBuffer();
                        serialPort.DiscardOutBuffer();

                        // Small delay to let driver process the changes
                        Thread.Sleep(100);
                    }
                    catch (Exception)
                    {
                        // Ignore errors during cleanup
                    }

                    serialPort.Close();
                }

                serialPort.Dispose();  // releases OS handles
                serialPort = null;

                // Reset the USB device to clear driver state (like changing settings in Device Manager)
                Thread.Sleep(200);
                bool reset = UsbDeviceReset.ResetComPort(portName);
                if (reset)
                {
                   // MessageBox.Show($"USB device {portName} reset successfully", "Device Reset", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Test Method 3: Try different baud rates using SerialPortStream
        /// </summary>
        public bool Open()
        {
            // var baudRates = new[] { 9600 };

            //foreach (var baudRate in baudRates)
            //{
            try
            {
                //MessageBox.Show($"SerialPortStream TEST 3:\nTrying BaudRate: {9600}\nUsing SerialPortStream library",
               //     "SerialPortStream Test 3", MessageBoxButtons.OK, MessageBoxIcon.Information);

                UsbDeviceReset.ResetComPort(portName);

                if (serialPort.IsOpen)
                {
                    MessageBox.Show("Closing existing SerialPortStream connection...",
                        "SerialPortStream Test 3", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }

                // Don't set BaudRate here - it's already configured in constructor
                // serialPort.BaudRate = 9600;  // REMOVED - causes issues with CH340
                serialPort.Open();
                Thread.Sleep(200);

                string result = $"SUCCESS - SerialPortStream Method 3!\n\n" +
                    $"Port: {serialPort.PortName}\n" +
                    $"BaudRate: {9600}\n" +
                    $"IsOpen: {serialPort.IsOpen}\n" +
                    $"DTR: {serialPort.DtrEnable}\n" +
                    $"RTS: {serialPort.RtsEnable}";

                //MessageBox.Show(result, $"SerialPortStream Test - SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}",
                    "SerialPortStream Test - FAILED", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                Close();
            }
            //}

            //MessageBox.Show("All baud rates failed!", "SerialPortStream Test 3 - ALL FAILED", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        /// <summary>
        /// Test reading data from the scale using SerialPortStream
        /// </summary>
        public decimal ReadData()
        {
            try
            {

                Open();
                //if (!serialPort.IsOpen)
                //{
                //    // Use the method that works (Method 1)
                //    serialPort.Open();
                //    Thread.Sleep(200);
                //}

                // MessageBox.Show("SerialPortStream port is open. Waiting for data from scale...\n\nMake sure scale is sending data.",
                //   "SerialPortStream Reading Data", MessageBoxButtons.OK, MessageBoxIcon.Information);

                int retry = 0;
                while (retry < 10)
                {
                    // Read using SerialPortStream
                    byte[] buffer = new byte[1024];
                    int bytesRead = serialPort.Read(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                    {
                        string value = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                        //MessageBox.Show($"SerialPortStream Data received!\n\nRaw: {value}\n\nTrimmed: {value.Trim()}",
                        //"SerialPortStream Data Received", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (!value.StartsWith("W:")) {
                            retry++;
                            Thread.Sleep(500);
                            continue;
                        }
                        //int index = value.LastIndexOf("p", StringComparison.OrdinalIgnoreCase);

                        value = value.Substring(value.IndexOf("W:") + 2, value.IndexOf("___") - 2);
                        return decimal.Parse(value);
                        //if (index >= 0)
                        //{
                        //    value = value.Substring(index);
                        //    if (value.Length == 8)
                        //    {
                        //        int weight = int.Parse(value.Substring(2));
                        //        MessageBox.Show($"SerialPortStream Weight parsed: {weight}", 
                        //            "SerialPortStream Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //        return weight;
                        //    }
                        //}
                    }

                    //Thread.Sleep(1000);
                    //retry++;
                }

                //MessageBox.Show("SerialPortStream: No data received after 10 seconds.",
                    //"SerialPortStream Timeout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return 0;
            }
            catch (Exception ex)
            {
               MessageBox.Show($"SerialPortStream Error reading data:\n\n{ex.Message}",
                  "SerialPortStream Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
            finally
            {
                Close();
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}