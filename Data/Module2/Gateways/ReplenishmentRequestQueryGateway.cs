using System.Reflection;
using Npgsql;
using ProRental.Data.Interfaces;
using ProRental.Domain.Entities;

namespace ProRental.Data.Gateways
{
    public class ReplenishmentRequestQueryGateway : IReplenishmentRequestQuery
    {
        private readonly string _connectionString;

        public ReplenishmentRequestQueryGateway(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("Default connection string not found.");
        }

        public Replenishmentrequest? GetRequest(int reqId)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            const string sql = @"
                SELECT requestid, requestedby, createdat, remarks, completedat, completedby
                FROM replenishmentrequest
                WHERE requestid = @reqId;
            ";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@reqId", reqId);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            var request = new Replenishmentrequest();

            SetPrivateField(request, "_requestid", reader.GetInt32(reader.GetOrdinal("requestid")));
            SetPrivateField(request, "_requestedby", reader["requestedby"] == DBNull.Value ? null : reader["requestedby"]?.ToString());
            SetPrivateField(request, "_createdat", reader["createdat"] == DBNull.Value ? null : Convert.ToDateTime(reader["createdat"]));
            SetPrivateField(request, "_remarks", reader["remarks"] == DBNull.Value ? null : reader["remarks"]?.ToString());
            SetPrivateField(request, "_completedat", reader["completedat"] == DBNull.Value ? null : Convert.ToDateTime(reader["completedat"]));
            SetPrivateField(request, "_completedby", reader["completedby"] == DBNull.Value ? null : reader["completedby"]?.ToString());

            return request;
        }

        public List<Lineitem> GetLineItems(int reqId)
        {
            var items = new List<Lineitem>();

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            const string sql = @"
                SELECT lineitemid, requestid, productid, quantityrequest, remarks
                FROM lineitem
                WHERE requestid = @reqId
                ORDER BY lineitemid;
            ";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@reqId", reqId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var item = new Lineitem();

                SetPrivateField(item, "_lineitemid", reader.GetInt32(reader.GetOrdinal("lineitemid")));
                SetPrivateField(item, "_requestid", reader.GetInt32(reader.GetOrdinal("requestid")));
                SetPrivateField(item, "_productid", reader.GetInt32(reader.GetOrdinal("productid")));
                SetPrivateField(item, "_quantityrequest", reader.GetInt32(reader.GetOrdinal("quantityrequest")));
                SetPrivateField(item, "_remarks", reader["remarks"] == DBNull.Value ? null : reader["remarks"]?.ToString());

                items.Add(item);
            }

            return items;
        }

        private static void SetPrivateField(object target, string fieldName, object? value)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (field == null)
                throw new InvalidOperationException(
                    $"Field '{fieldName}' was not found on type '{target.GetType().Name}'.");

            field.SetValue(target, value);
        }
    }
}