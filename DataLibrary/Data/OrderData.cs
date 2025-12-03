using Dapper;
using DataLibrary.Db;
using DataLibrary.Models;
using System.Data;

namespace DataLibrary.Data
{
    public class OrderData : IOrderData
    {
        private readonly IDataAccess _dataAcces;
        private readonly ConnectionStringData _connectionString;

        public OrderData(IDataAccess dataAcces, ConnectionStringData connectionString)
        {
            _dataAcces = dataAcces;
            _connectionString = connectionString;
        }

        public async Task<int> CreateOrder(OrderModel order)
        {
            DynamicParameters p = new DynamicParameters();

            p.Add("OrderName", order.OrderName);
            p.Add("OrderDate", order.OrderDate);
            p.Add("FoodId", order.FoodId);
            p.Add("Quantity", order.Quantity);
            p.Add("Total", order.Total);
            p.Add("Id", DbType.Int32, direction: ParameterDirection.Output);

            await _dataAcces.SaveData("dbo.spOrders_Insert", p, _connectionString.SqlConnectionName);

            return p.Get<int>("Id");
        }

        public Task<int> UpdateOrderName(int orderId, string orderName)
        {
            return _dataAcces.SaveData("dbo.spOrders_UpdateName",
                                       new { Id = orderId, OrderName = orderName },
                                       _connectionString.SqlConnectionName);
        }

        public Task<int> DeleteOrder(int orderId)
        {
            return _dataAcces.SaveData("dbo.spOrders_Delete",
                                       new { Id = orderId },
                                       _connectionString.SqlConnectionName);
        }

        public async Task<OrderModel> GetOrderById(int orderId)
        {
            var recs = await _dataAcces.LoadData<OrderModel, dynamic>("spOrders_GetById",
                                                                      new { Id = orderId },
                                                                      _connectionString.SqlConnectionName);

            return recs.FirstOrDefault();
        }
    }
}
