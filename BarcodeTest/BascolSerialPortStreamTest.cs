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

        /// <summary>
        /// Test Method 1: Open port without touching DTR/RTS at all using SerialPortStream
        /// </summary>
        public bool TestMethod1_SerialPortStream_NoControlSignals()
        {
            try
            {
                MessageBox.Show("SerialPortStream TEST 1:\nOpening port WITHOUT setting DTR/RTS\nUsing SerialPortStream library", 
                    "SerialPortStream Test 1", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (serialPort.IsOpen)
                    serialPort.Close();

                // Just open - don't touch DTR/RTS
                serialPort.Open();
                Thread.Sleep(200);

                string result = $"SUCCESS - SerialPortStream Method 1!\n\n" +
                    $"Port: {serialPort.PortName}\n" +
                    $"IsOpen: {serialPort.IsOpen}\n" +
                    $"DTR (driver default): {serialPort.DtrEnable}\n" +
                    $"RTS (driver default): {serialPort.RtsEnable}\n" +
                    $"CTS Holding: {serialPort.CtsHolding}\n" +
                    $"DSR Holding: {serialPort.DsrHolding}\n" +
                    $"CD Holding: {serialPort.CDHolding}";

                MessageBox.Show(result, "SerialPortStream Test 1 - SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"FAILED - SerialPortStream Method 1\n\nError: {ex.Message}\n\nType: {ex.GetType().Name}\n\nHResult: 0x{ex.HResult:X8}", 
                    "SerialPortStream Test 1 - FAILED", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                if (serialPort.IsOpen)
                    serialPort.Close();
            }
        }

        /// <summary>
        /// Test Method 2: Open port, then set DTR/RTS after opening using SerialPortStream
        /// </summary>
        public bool TestMethod2_SerialPortStream_SetSignalsAfterOpen(bool dtr, bool rts)
        {
            try
            {
                MessageBox.Show($"SerialPortStream TEST 2:\nOpening port first\nThen setting DTR={dtr}, RTS={rts}\nUsing SerialPortStream library", 
                    "SerialPortStream Test 2", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (serialPort.IsOpen)
                    serialPort.Close();

                // Open first
                serialPort.Open();
                Thread.Sleep(100);

                // Now set DTR/RTS
                serialPort.DtrEnable = dtr;
                serialPort.RtsEnable = rts;
                Thread.Sleep(200);

                string result = $"SUCCESS - SerialPortStream Method 2!\n\n" +
                    $"Port: {serialPort.PortName}\n" +
                    $"IsOpen: {serialPort.IsOpen}\n" +
                    $"DTR: {serialPort.DtrEnable}\n" +
                    $"RTS: {serialPort.RtsEnable}\n" +
                    $"CTS Holding: {serialPort.CtsHolding}\n" +
                    $"DSR Holding: {serialPort.DsrHolding}\n" +
                    $"CD Holding: {serialPort.CDHolding}";

                MessageBox.Show(result, "SerialPortStream Test 2 - SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"FAILED - SerialPortStream Method 2\n\nError: {ex.Message}\n\nType: {ex.GetType().Name}\n\nHResult: 0x{ex.HResult:X8}", 
                    "SerialPortStream Test 2 - FAILED", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                if (serialPort.IsOpen)
                    serialPort.Close();
            }
        }

        /// <summary>
        /// Test Method 3: Try different baud rates using SerialPortStream
        /// </summary>
        public bool TestMethod3_SerialPortStream_DifferentBaudRates()
        {
            var baudRates = new[] { 4800, 9600, 19200, 38400, 57600, 115200 };
            
            foreach (var baudRate in baudRates)
            {
                try
                {
                    MessageBox.Show($"SerialPortStream TEST 3:\nTrying BaudRate: {baudRate}\nUsing SerialPortStream library", 
                        "SerialPortStream Test 3", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (serialPort.IsOpen)
                        serialPort.Close();

                    // Create new instance with different baud rate
                    serialPort.BaudRate = baudRate;
                    serialPort.Open();
                    Thread.Sleep(200);

                    string result = $"SUCCESS - SerialPortStream Method 3!\n\n" +
                        $"Port: {serialPort.PortName}\n" +
                        $"BaudRate: {baudRate}\n" +
                        $"IsOpen: {serialPort.IsOpen}\n" +
                        $"DTR: {serialPort.DtrEnable}\n" +
                        $"RTS: {serialPort.RtsEnable}";

                    MessageBox.Show(result, $"SerialPortStream Test 3 - SUCCESS (BaudRate: {baudRate})", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"FAILED BaudRate {baudRate}\n\nError: {ex.Message}\n\nTrying next baud rate...", 
                        "SerialPortStream Test 3 - FAILED", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    
                    if (serialPort.IsOpen)
                        serialPort.Close();
                    // Continue to next baud rate
                }
            }

            MessageBox.Show("All baud rates failed!", "SerialPortStream Test 3 - ALL FAILED", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        /// <summary>
        /// Run all SerialPortStream tests in sequence
        /// </summary>
        public void RunAllSerialPortStreamTests()
        {
            MessageBox.Show($"Starting SerialPortStream CH340 Tests\n\nPort: {serialPort.PortName}\nLibrary: SerialPortStream (bypasses .NET SerialPort)", 
                "SerialPortStream CH340 Tests", MessageBoxButtons.OK, MessageBoxIcon.Information);

            bool test1 = TestMethod1_SerialPortStream_NoControlSignals();
            if (test1)
            {
                MessageBox.Show("SerialPortStream Test 1 PASSED! This method works.\n\nContinuing to test other methods...", 
                    "Progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            bool test2False = TestMethod2_SerialPortStream_SetSignalsAfterOpen(false, false);
            if (test2False)
            {
                MessageBox.Show("SerialPortStream Test 2 (DTR=F, RTS=F) PASSED!\n\nContinuing...", 
                    "Progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            bool test2True = TestMethod2_SerialPortStream_SetSignalsAfterOpen(true, true);
            if (test2True)
            {
                MessageBox.Show("SerialPortStream Test 2 (DTR=T, RTS=T) PASSED!\n\nContinuing...", 
                    "Progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            bool test3 = TestMethod3_SerialPortStream_DifferentBaudRates();

            // Summary
            string summary = "═══ SerialPortStream TEST SUMMARY ═══\n\n" +
                $"Test 1 (No DTR/RTS control): {(test1 ? "✓ PASS" : "✗ FAIL")}\n" +
                $"Test 2 (DTR=F, RTS=F after open): {(test2False ? "✓ PASS" : "✗ FAIL")}\n" +
                $"Test 2 (DTR=T, RTS=T after open): {(test2True ? "✓ PASS" : "✗ FAIL")}\n" +
                $"Test 3 (Different baud rates): {(test3 ? "✓ PASS" : "✗ FAIL")}\n\n" +
                "════════════════════════════════════";

            MessageBox.Show(summary, "SerialPortStream Tests Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
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