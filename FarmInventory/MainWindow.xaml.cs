using FarmInventory.Views;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FarmInventory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Action when Product button is pressed
        private void btnProduct_Click(object sender, object e)
        {
            Admin admin = new Admin(); // create a Admin instance
            admin.Show(); // open Admin window
        }

        // Action when Sales button is pressed
        private void btnSales_Click(object sender, RoutedEventArgs e)
        {
            Sales sales = new Sales(); // create a Sales instance
            sales.Show(); // open Sales window
        }

    }
}
