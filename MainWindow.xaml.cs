using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace serial {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {
        private SerialPort serialPort;
        private bool connect = false;
        public MainWindow() {
            InitializeComponent();
            ContentRendered += (s, e) => {
                foreach (string portName in SerialPort.GetPortNames())
                    portComboBox.Items.Add(portName);
            };
        }

        private void toggleButton_Click(object sender, RoutedEventArgs e) {
            if (!connect) {
                serialPort = new SerialPort();
                serialPort.PortName = portComboBox.SelectedItem.ToString();
                serialPort.BaudRate = 9600;
                serialPort.DataBits = 8;
                serialPort.Parity = Parity.None;
                serialPort.StopBits = StopBits.One;
                serialPort.Handshake = Handshake.None;
                serialPort.Encoding = Encoding.UTF8;

                try {
                    serialPort.Open();
                    serialPort.DataReceived += serialDataReceived;
                    if (serialPort.IsOpen) {
                        sendText.Clear();
                        receiveText.Clear();
                        receiveText.AppendText("open");
                        toggleButton.Content = "通信終了";
                        portComboBox.IsEnabled = false;
                        connect = true;
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
            } else {
                try {
                    if (serialPort.IsOpen) {
                        serialPort.Close();
                        toggleButton.Content = "通信開始";
                        portComboBox.IsEnabled = true;
                        connect = false;
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void sendButton_Click(object sender, RoutedEventArgs e) {
            string data = sendText.Text;
            if (!string.IsNullOrWhiteSpace(data)) {
                try {
                    serialPort.Write(data);
                    sendText.Clear();
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void serialDataReceived(object sender, EventArgs e) {
            if (serialPort.IsOpen) {
                try {
                    string data = serialPort.ReadExisting();
                    if (!string.IsNullOrEmpty(data)) {
                        Dispatcher.Invoke(new Action(() => {
                            receiveText.AppendText(data);
                            receiveText.ScrollToEnd();
                        }));
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
            }
        }

    }
}
