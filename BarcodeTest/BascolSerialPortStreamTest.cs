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

        public BascolSerialPortStreamTest(string portName, int baudRate = 9600)
        {
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
                if (serialPort.IsOpen)
                {
                    MessageBox.Show("Closing...",
                      "SerialPortStream Test 3", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    serialPort.Close();
                }

                serialPort.Dispose();  // ADD THIS - releases OS handles
                serialPort = null;
            }
        }

        /// <summary>
        /// Test Method 3: Try different baud rates using SerialPortStream
        /// </summary>
        public bool TestMethod3_SerialPortStream_DifferentBaudRates()
        {
            // var baudRates = new[] { 9600 };

            //foreach (var baudRate in baudRates)
            //{
            try
            {
                MessageBox.Show($"SerialPortStream TEST 3:\nTrying BaudRate: {9600}\nUsing SerialPortStream library",
                    "SerialPortStream Test 3", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (serialPort.IsOpen)
                {
                    MessageBox.Show("Closing existing SerialPortStream connection...",
                        "SerialPortStream Test 3", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    serialPort.Close();
                }

                // Create new instance with different baud rate
                serialPort.BaudRate = 9600;
                serialPort.Open();
                Thread.Sleep(200);

                string result = $"SUCCESS - SerialPortStream Method 3!\n\n" +
                    $"Port: {serialPort.PortName}\n" +
                    $"BaudRate: {9600}\n" +
                    $"IsOpen: {serialPort.IsOpen}\n" +
                    $"DTR: {serialPort.DtrEnable}\n" +
                    $"RTS: {serialPort.RtsEnable}";

                MessageBox.Show(result, $"SerialPortStream Test 3 - SUCCESS (BaudRate: {9600})", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}",
                    "SerialPortStream Test - FAILED", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                if (serialPort.IsOpen)
                    serialPort.Close();
                // Continue to next baud rate
            }
            //}

            MessageBox.Show("All baud rates failed!", "SerialPortStream Test 3 - ALL FAILED", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        /// <summary>
        /// Test reading data from the scale using SerialPortStream
        /// </summary>
        public int TestReadDataSerialPortStream()
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    // Use the method that works (Method 1)
                    serialPort.Open();
                    Thread.Sleep(200);
                }

                MessageBox.Show("SerialPortStream port is open. Waiting for data from scale...\n\nMake sure scale is sending data.", 
                    "SerialPortStream Reading Data", MessageBoxButtons.OK, MessageBoxIcon.Information);

                int retry = 0;
                while (retry < 10)
                {
                    // Read using SerialPortStream
                    byte[] buffer = new byte[1024];
                    int bytesRead = serialPort.Read(buffer, 0, buffer.Length);
                    
                    if (bytesRead > 0)
                    {
                        string value = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        
                        MessageBox.Show($"SerialPortStream Data received!\n\nRaw: {value}\n\nTrimmed: {value.Trim()}", 
                            "SerialPortStream Data Received", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        value = value.Trim();
                        int index = value.LastIndexOf("p", StringComparison.OrdinalIgnoreCase);

                        if (index >= 0)
                        {
                            value = value.Substring(index);
                            if (value.Length == 8)
                            {
                                int weight = int.Parse(value.Substring(2));
                                MessageBox.Show($"SerialPortStream Weight parsed: {weight}", 
                                    "SerialPortStream Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return weight;
                            }
                        }
                    }

                    Thread.Sleep(1000);
                    retry++;
                }

                MessageBox.Show("SerialPortStream: No data received after 10 seconds.", 
                    "SerialPortStream Timeout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                if (serialPort.IsOpen)
                    serialPort.Close();
            }
        }

        public void Dispose()
        {
            if (serialPort != null)
            {
                if (serialPort.IsOpen)
                    serialPort.Close();
                serialPort.Dispose();
            }
        }
    }
}