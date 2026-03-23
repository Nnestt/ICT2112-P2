using System.Reflection;
using Npgsql;
using ProRental.Data.Interfaces;
using ProRental.Domain.Entities;

namespace ProRental.Data.Gateways
{
    public class POLineItemMapper : IPOLineItemMapper
    {
        private readonly string _connectionString;

        public POLineItemMapper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("Default connection string not found.");
        }

        public void InsertItems(int poId, List<Polineitem> items)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            foreach (var item in items)
            {
                const string sql = @"
                    INSERT INTO polineitem (poid, productid, qty, unitprice, linetotal)
                    VALUES (@poid, @productid, @qty, @unitprice, @linetotal);
                ";

                using var cmd = new NpgsqlCommand(sql, conn);

                var productId = GetPrivateFieldValue<int?>(item, "_productid");
                var qty = GetPrivateFieldValue<int?>(item, "_qty");
                var unitPrice = GetPrivateFieldValue<decimal?>(item, "_unitprice");
                var lineTotal = GetPrivateFieldValue<decimal?>(item, "_linetotal");

                cmd.Parameters.AddWithValue("@poid", poId);
                cmd.Parameters.AddWithValue("@productid", (object?)productId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@qty", (object?)qty ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@unitprice", (object?)unitPrice ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@linetotal", (object?)lineTotal ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        public List<Polineitem> FindItemsByPO(int poId)
        {
            var items = new List<Polineitem>();

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            const string sql = @"
                SELECT polineitemid, poid, productid, qty, unitprice, linetotal
                FROM polineitem
                WHERE poid = @poid
                ORDER BY polineitemid;
            ";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@poid", poId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var item = new Polineitem();

                SetPrivateField(item, "_polineitemid", reader.GetInt32(reader.GetOrdinal("polineitemid")));
                SetPrivateField(item, "_poid", reader["poid"] == DBNull.Value ? null : Convert.ToInt32(reader["poid"]));
                SetPrivateField(item, "_productid", reader["productid"] == DBNull.Value ? null : Convert.ToInt32(reader["productid"]));
                SetPrivateField(item, "_qty", reader["qty"] == DBNull.Value ? null : Convert.ToInt32(reader["qty"]));
                SetPrivateField(item, "_unitprice", reader["unitprice"] == DBNull.Value ? null : Convert.ToDecimal(reader["unitprice"]));
                SetPrivateField(item, "_linetotal", reader["linetotal"] == DBNull.Value ? null : Convert.ToDecimal(reader["linetotal"]));

                items.Add(item);
            }

            return items;
        }

        public void DeleteItemsByPO(int poId)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            const string sql = @"DELETE FROM polineitem WHERE poid = @poid;";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@poid", poId);
            cmd.ExecuteNonQuery();
        }

        public void ReplaceItems(int poId, List<Polineitem> items)
        {
            DeleteItemsByPO(poId);
            InsertItems(poId, items);
        }

        private static void SetPrivateField(object target, string fieldName, object? value)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (field == null)
                throw new InvalidOperationException(
                    $"Field '{fieldName}' was not found on type '{target.GetType().Name}'.");

            field.SetValue(target, value);
        }

        private static T? GetPrivateFieldValue<T>(object target, string fieldName)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (field == null)
                throw new InvalidOperationException(
                    $"Field '{fieldName}' was not found on type '{target.GetType().Name}'.");

            return (T?)field.GetValue(target);
        }
    }
}