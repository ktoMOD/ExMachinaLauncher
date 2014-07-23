using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ExMachinaLauncher
{
    /// <summary>
    /// Interaction logic for Warning.xaml
    /// </summary>
    public partial class WarningWindow : Window
    {
        public WarningWindow()
        {
            InitializeComponent();
        }
        public WarningWindow(string warningString)
        {
            InitializeComponent();
            WarningTextBlock.Text = warningString;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

    }
}
