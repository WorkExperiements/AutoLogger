using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace FNProcessOrder
{
    public static class BlackBoxService
    {
        public static void SpookyOp(string connString, Order order)
        {

            // this will fail if day of the week is % 4 (fail every 4th day)
            var todayDay = DateTime.Now.Day;
            if (todayDay % 4 == 0)
            {
                throw new Exception();
            }

            SqlConnection cnn = new SqlConnection(connString);

            cnn.Open();

                var command = cnn.CreateCommand();
            command.CommandText = $"INSERT INTO [dbo].[Order] (OrderId, TransactionId, ProductName, ProductId, Status, LastUpdated) VALUES ('{order.OrderId}', '{order.TransactionId}', '{order.Product.Name}', '{order.Product.ID}', 'Processed', '{DateTime.Now}')";
                var count = command.ExecuteNonQuery();
            cnn.Close();
        }
    }
}
