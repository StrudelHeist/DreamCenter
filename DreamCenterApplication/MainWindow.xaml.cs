using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DreamCenterApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _connected;

        public string ConnectionStatus
        {
            get
            {
                return _connected ? "Connected" : "Disconnected";
            }
        }
        public Color ConnectionColor
        {
            get
            {
                return _connected ? Color.FromRgb(0x00, 0xFF, 0x00) : Color.FromRgb(0xFF, 0x00, 0x00);
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            // Set default values
            _connected = false;

            // Initialize communication
            IRunner runner;
            if (Properties.Settings.Default.IsSlave)
                runner = new SlaveRunner();
            else
                runner = new MasterRunner();
            runner.Start();
        }
    }
}
