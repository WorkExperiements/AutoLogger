using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace FNProcessOrder
{
    public static class DbHelper
    {
        public static void CreateOrderInDb(string connString, Order order)
        {
            SqlConnection cnn = new SqlConnection(connString);

            cnn.Open();

                var command = cnn.CreateCommand();
            command.CommandText = $"INSERT INTO [dbo].[Order] (OrderId, TransactionId, ProductName, ProductId, Status, LastUpdated) VALUES ('{order.OrderId}', '{order.TransactionId}', '{order.Product.Name}', '{order.Product.ID}', 'Processed', '{DateTime.Now}')";
                var count = command.ExecuteNonQuery();
            cnn.Close();
        }
    }
}
