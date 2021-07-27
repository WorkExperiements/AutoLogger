using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace FNFulfillOrder
{
    public static class DBHelper
    {
        public static int UpdateOrders(string connStr)
        {
            // write into db
            SqlConnection cnn = new SqlConnection(connStr);
            var count = 0;
            cnn.Open();

            var orderIds = new List<int>();
            var command = cnn.CreateCommand();
            command.CommandText = $"SELECT * FROM [order] WHERE status != 'Complete'";
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                orderIds.Add(reader.GetInt32(0));
            }
            reader.Close();


            foreach (var orderId in orderIds)
            {
                command = cnn.CreateCommand();
                command.CommandText = $"UPDATE [dbo].[order] SET status='Complete', LastUpdated='{DateTime.Now}' WHERE id = '{orderId}'";
                var _ = command.ExecuteNonQuery();
                count++;
            }

            cnn.Close();
            return count;
        }
    }
}
