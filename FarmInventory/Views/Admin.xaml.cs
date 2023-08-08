using FarmInventory.Controllers;
using FarmInventory.Models;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;

namespace FarmInventory.Views
{
   /// <summary>
    /// Interaction logic for Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        private AdminController adminController;
        private List<Product> products;
        public Admin()
        {
            InitializeComponent();
            Loaded += AdminLoaded; // calling AdminLoaded method
            adminController = new AdminController(); 
        }

        // Show data once the Admin window is opened
        private void AdminLoaded(object sender, RoutedEventArgs e)
        {
            ShowAllData();
        }

        // Action when the Insert button is pressed
        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            // create a new thread for insert operation
            Thread insertThread = new Thread(() =>
            {
                // initialize the variables 
                string productName = string.Empty;
                int productId = 0;
                double amountKg = 0;
                decimal pricePerKg = 0;

                // Dispatcher.Invoke method is used to safely access the UI thread and retrieve the values from the text boxes
                Dispatcher.Invoke(() =>
                {
                    // assign input entered by user to the corresponding variables
                    productName = tbProductName.Text;
                    productId = int.Parse(tbProductID.Text);
                    amountKg = double.Parse(tbAmountKg.Text);
                    pricePerKg = decimal.Parse(tbPricePerKg.Text);
                });
                adminController.InsertData(productName, productId, amountKg, pricePerKg); // call InsertData method from AdminController
                ShowAllData(); // refresh the product list on data grid
            });
            insertThread.Start();
        }

        // Action when the Show All button is pressed 
        private void btnShowAll_Click(object sender, RoutedEventArgs e)
        {
            // create a new thread for showing the product list
            Thread showAllThread = new Thread(ShowAllData);
            showAllThread.Start();
        }
        
        // Action when Search button is pressed
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            // set condition to ensure the user enter the product id
            if (string.IsNullOrWhiteSpace(tbSearchId.Text))
            {
                MessageBox.Show("Please enter a product ID.");
                return;
            }
            int searchId = int.Parse(tbSearchId.Text);

            // create a new thread for searching data
            Thread searchThread = new Thread(() => 
            {
                // call SearchData method from adminController
                List<Product> searchResults = adminController.SearchData(searchId);

                // Dispatcher.Invoke method is used to safely access the UI thread 
                Dispatcher.Invoke(() => 
                {
                    if (searchResults != null && searchResults.Any()) // if searched product exists
                    {
                        dataView.ItemsSource = searchResults; // show the result on dataView
                    }
                    else // if the searched product does not exists
                    {
                        dataView.ItemsSource = null; // empty dataView
                        MessageBox.Show("No result found.");
                    }
                });
            });
            searchThread.Start();
        }

        // Action when Delete button is pressed
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected row from the DataGrid
            Product selectedRow = dataView.SelectedItem as Product;

            // Set condition to ensure a row is selected
            if (selectedRow == null)
            {
                MessageBox.Show("Please select a row to delete.");
                return;
            }

            // Get the ID of the selected row
            int selectedId = selectedRow.id;

            // Create a new thread to perform the deletion
            Thread deleteThread = new Thread(() => 
            {
                adminController.DeleteData(selectedId); // call DeleteData method from adminController
                ShowAllData(); // display the updated production list  after deletion
            });
            deleteThread.Start();
        }

        // Action when the + button is pressed
        private void btnPlus_Click(object sender, RoutedEventArgs e)
        {
            // Get the product ID from the text box
            if (!int.TryParse(tbIdUpdate.Text, out int productId)) // try parsing text in tbIdUpdate as an integer, then store it in productId
            {
                MessageBox.Show("Invalid product ID."); // if the parsing failed
                return;
            }

            // Get the amount to add from the text box
            if (!double.TryParse(tbAmountKgUpdate.Text, out double amountToAdd)) // try parsing text in tbAmountKgUpdate as a double, then store it in amountToAdd
            {
                MessageBox.Show("Invalid amount."); // if the parsing failed
                return;
            }

            // Create a new thread to perform the addition
            Thread addAmountThread = new Thread(() =>
            {
                adminController.AddAmount(productId, amountToAdd); // call AddAmount method from adminController
                ShowAllData(); // display updated values in dataView
            });
            addAmountThread.Start();
        }

        // Action when - button is pressed
        private void btnMinus_Click(object sender, RoutedEventArgs e)
        {
            // Get the product ID from the text box
            if (!int.TryParse(tbIdUpdate.Text, out int productId)) // try parsing text in tbIdUpdate as an integer, then store it in productId
            {
                MessageBox.Show("Invalid product ID."); // if parsing failed
                return;
            }

            // Get the amount to minus from the text box
            if (!double.TryParse(tbAmountKgUpdate.Text, out double amountToMinus)) // try parsing text in tbAmountKgUpdate as a double, then store it in amountToMinus
            {
                MessageBox.Show("Invalid amount."); // if parsing failed
                return;
            }

            // Create a new thread to perform the subtraction
            Thread minusAmountThread = new Thread(() =>
            {
                adminController.MinusAmount(productId, amountToMinus); // call MinusAmount method from adminController
                ShowAllData(); // display updated values in dataView
            });
            minusAmountThread.Start();
        }

        // Action when Direct Update button is pressed
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            // Get the product ID from the text box
            if (!int.TryParse(tbIdUpdate.Text, out int productId)) // try parsing text in tbIdUpdate as an integer, then store it in productId
            {
                MessageBox.Show("Invalid product ID."); // if parsing failed
                return;
            }

            // Get the amount to add from the text box
            if (!double.TryParse(tbAmountKgUpdate.Text, out double amountToUpdate)) // try parsing text in tbAmountKgUpdate as an integer, then store it in amountToUpdate
            {
                MessageBox.Show("Invalid amount."); // if parsing failed
                return;
            }

            // Create a new thread to perform the update
            Thread updateAmountThread = new Thread(() =>
            {
                adminController.UpdateAmount(productId, amountToUpdate); // call UpdateAmount method from adminController
                ShowAllData(); // display updated values in dataView
            });
            updateAmountThread.Start();
        }

        // Method to show all data on DataGrid
        private void ShowAllData()
        {
            List<Product> products = adminController.GetAllData();

            // Dispatcher.Invoke method is used to safely access the UI thread
            Dispatcher.Invoke(() => UpdateUIWithProductList(products)); // call UpdateUIWithProductList method
        }

        // Method to update the product list
        private void UpdateUIWithProductList(List<Product> products)
        {
            // Dispatcher.Invoke method is used to safely access the UI thread
            Dispatcher.Invoke(() =>
            {
                dataView.ItemsSource = products; // set ItemsSource of dataView as the list of products
            });
        }
    }
}
