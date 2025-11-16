using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Mali.MaliControls
{
    /// <summary>
    /// Test class using standard .NET SerialPort for Bascol device
    /// </summary>
    public class BascolSerialPortTest : IDisposable
    {
        private SerialPort serialPort;
        private bool disposed = false;

        public BascolSerialPortTest(string portName, int baudRate = 9600)
        {
            serialPort = new SerialPort(portName, baudRate)
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

        public bool Open()
        {
            try
            {
                MessageBox.Show($"Opening port: {serialPort.PortName}\nBaudRate: {serialPort.BaudRate}",
                    "SerialPort Test", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (serialPort.IsOpen)
                {
                    MessageBox.Show("Port is already open, closing first...",
                        "SerialPort Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    serialPort.Close();
                    Thread.Sleep(100);
                }

                serialPort.Open();
                Thread.Sleep(200);

                string result = $"SUCCESS - SerialPort opened!\n\n" +
                    $"Port: {serialPort.PortName}\n" +
                    $"BaudRate: {serialPort.BaudRate}\n" +
                    $"IsOpen: {serialPort.IsOpen}\n" +
                    $"DTR: {serialPort.DtrEnable}\n" +
                    $"RTS: {serialPort.RtsEnable}";

                MessageBox.Show(result, "SerialPort Test - SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open port:\n\n{ex.Message}",
                    "SerialPort Test - FAILED", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        public void Close()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    MessageBox.Show("Closing serial port...",
                        "SerialPort Test", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reset signals before closing
                    serialPort.DtrEnable = false;
                    serialPort.RtsEnable = false;

                    // Discard buffers
                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();

                    Thread.Sleep(100);
                    serialPort.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error closing port: {ex.Message}",
                        "SerialPort Test", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public int ReadData()
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                    Thread.Sleep(200);
                }

                MessageBox.Show("Port is open. Waiting for data from scale...\n\nMake sure scale is sending data.",
                    "SerialPort Reading Data", MessageBoxButtons.OK, MessageBoxIcon.Information);

                int retry = 0;
                while (retry < 10)
                {
                    int bytesToRead = serialPort.BytesToRead;
                    if (bytesToRead > 0)
                    {
                        byte[] buffer = new byte[bytesToRead];
                        int bytesRead = serialPort.Read(buffer, 0, bytesToRead);

                        if (bytesRead > 0)
                        {
                            string value = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                            MessageBox.Show($"Data received!\n\nRaw: {value}\n\nTrimmed: {value.Trim()}",
                                "SerialPort Data Received", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            value = value.Trim();
                            int index = value.LastIndexOf("p", StringComparison.OrdinalIgnoreCase);

                            if (index >= 0)
                            {
                                value = value.Substring(index);
                                if (value.Length == 8)
                                {
                                    int weight = int.Parse(value.Substring(2));
                                    MessageBox.Show($"Weight parsed: {weight}",
                                        "SerialPort Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return weight;
                                }
                            }
                        }
                    }

                    Thread.Sleep(1000);
                    retry++;
                }

                MessageBox.Show("No data received after 10 seconds.",
                    "SerialPort Timeout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading data:\n\n{ex.Message}",
                    "SerialPort Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (serialPort != null)
                    {
                        Close();
                        serialPort.Dispose();
                        serialPort = null;
                    }
                }
                disposed = true;
            }
        }

        ~BascolSerialPortTest()
        {
            Dispose(false);
        }
    }
}
