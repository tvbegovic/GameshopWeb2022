using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Dapper;

namespace GameshopWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IConfiguration configuration;
        public OrderController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost("")]
        public Order Create(Order order)
        {
            using (var conn = new SqlConnection(
                configuration.GetConnectionString("gameshopConnString")))
            {
                conn.Open();
                var tr = conn.BeginTransaction();
                try
                {
                    var sql =
                    @"INSERT INTO [User](firstname, lastname, address, email, city) 
                        OUTPUT inserted.id
                       VALUES (@firstname, @lastname, @address, @email, @city)";
                    order.IdUser = conn.ExecuteScalar<int>(sql, order.User, tr);
                    order.DateOrdered = DateTime.Now;
                    sql = @"INSERT INTO [Order](idUser, dateOrdered) OUTPUT inserted.id
                            VALUES(@idUser, @dateOrdered)";
                    order.Id = conn.ExecuteScalar<int>(sql, order, tr);
                    foreach(var det in order.Details)
                    {
                        det.IdOrder = order.Id;
                        sql =
                       @"INSERT INTO OrderDetail(idOrder, idGame, quantity,unitPrice)
                         VALUES(@idOrder, @idGame, @quantity, @unitPrice)";
                        conn.Execute(sql, det, tr);
                    }
                    tr.Commit();
                }
                catch (Exception)
                {
                    tr.Rollback();
                    throw;
                }
            }
            return order;
        }
    }
}
