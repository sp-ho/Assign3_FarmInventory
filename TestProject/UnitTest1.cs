using FarmInventory;
using FarmInventory.Controllers;
using FarmInventory.Models;
using Npgsql;
using NuGet.Frameworks;
using System.Data.Common;
using CustomDbConnection = FarmInventory.DbConnection; // Add this alias

namespace TestProject
{
    public class AdminControllerTests
    {
        private AdminController adminController;    
        
        [SetUp]
        public void Setup()
        {
            // Arrange: Initialize the AdminController instance
            adminController = new AdminController();
        }

        [Test]
        public void TestInsertData()
        {
            // Arrange: Prepare test data
            string name = "Mango";
            int id = 111111;
            double amount = 26;
            decimal price = 2.10m;

            // Act: Call the method being tested
            adminController.InsertData(name, id, amount, price);

            // Assert: Check if the product was inserted successfully
            // Query the database to check if the inserted data matches the expected values
            using (var connection = CustomDbConnection.Connection)
            {
                string query = "SELECT * FROM products WHERE id = @ID";
                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        Assert.IsTrue(reader.Read()); // Check if the record was found
                        Assert.AreEqual(name, reader.GetString(reader.GetOrdinal("name")));
                        Console.WriteLine(reader.GetString(reader.GetOrdinal("name")));
                        Assert.AreEqual(amount, reader.GetDouble(reader.GetOrdinal("amount")));
                        Assert.AreEqual(price, reader.GetDecimal(reader.GetOrdinal("price")));

                    }
                }
            }
        }

        [Test]
        public void TestGetAllData()
        {
            // Act: Call the method being tested
            List<Product> actualProducts = adminController.GetAllData();
            Console.WriteLine(actualProducts.Count);

            Assert.IsTrue(actualProducts.Count > 0); // count must be greater than 0 if record is found
        }

        [Test]
        public void TestDeleteData()
        {
            // Arrange: Prepare test data
            int idToDelete = 124567; // ID of the product you want to delete

            // Act: Call the method being tested
            adminController.DeleteData(idToDelete);

            // Assert: Check if the product with the given ID was deleted from the database
            // Query the database to check if the record with the specified ID no longer exists
            using (var connection = CustomDbConnection.Connection)
            {
                string query = "SELECT COUNT(*) FROM products WHERE id = @ID";
                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", idToDelete);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    Assert.AreEqual(0, count); // if the data with the id is deleted, the count should be zero
                }
            }
        }

        [Test]
        public void TestAddAmount()
        {
            // Arrange: Prepare test data
            int id = 124567; // Apple
            double initialAmount = 23;  // original amount: 23
            double amountToAdd = 5;

            // Act: Call the method being tested
            adminController.AddAmount(id, amountToAdd);

            // Assert: Check if the amount was updated successfully
            using (var connection = CustomDbConnection.Connection)
            {
                string query = "SELECT amount FROM products WHERE id = @ID";
                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        Assert.IsTrue(reader.Read()); // Check if the record was found
                        double updatedAmount = reader.GetDouble(reader.GetOrdinal("amount"));
                        Assert.AreEqual(initialAmount + amountToAdd, updatedAmount);
                    }
                }
            }
        }

        [Test]
        public void TestMinusAmount()
        {
            // Arrange: Prepare test data
            int id = 124567; // apple
            double initialAmount = 23; // original amount: 23
            double amountToMinus = 2;

            // Act: Call the method being tested
            adminController.MinusAmount(id, amountToMinus);

            // Assert: Check if the amount was updated successfully
            using (var connection = CustomDbConnection.Connection)
            {
                string query = "SELECT amount FROM products WHERE id = @ID";
                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        Assert.IsTrue(reader.Read()); // Check if the record was found
                        double updatedAmount = reader.GetDouble(reader.GetOrdinal("amount"));
                        Assert.AreEqual(initialAmount - amountToMinus, updatedAmount);
                        /*Assert.AreEqual(initialAmount - amountToMinus, 16);*/
                    }
                }
            }
        }

        [Test]
        public void TestUpdateAmount()
        {
            // Arrange: Prepare test data
            int id = 124567;
            double updatedAmount = 50; // original amount: 23

            // Act: Call the method being tested
            adminController.UpdateAmount(id, updatedAmount);

            // Assert: Check if the amount was updated successfully
            using (var connection = CustomDbConnection.Connection)
            {
                string query = "SELECT amount FROM products WHERE id = @ID";
                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        Assert.IsTrue(reader.Read()); // Check if the record was found
                        double amountAfterUpdate = reader.GetDouble(reader.GetOrdinal("amount"));
                        Assert.AreEqual(updatedAmount, amountAfterUpdate);
                    }
                }
            }
        }
    }
}