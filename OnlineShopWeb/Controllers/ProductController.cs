using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace OnlineShopWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public ProductController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet("")]
        public List<Product> GetAll()
        {
            using (var conn = new SqlConnection(
                configuration.GetConnectionString("connString")))
            {
                var sql = "SELECT * FROM Product";
                return conn.Query<Product>(sql).ToList();
            }
        }

        [HttpGet("{id}")]
        public Product GetById(int id)
        {
            using (var conn = new SqlConnection(
                configuration.GetConnectionString("connString")))
            {
                var sql = "SELECT * FROM Proizvod WHERE id = @id";
                return conn.QueryFirstOrDefault<Product>(sql, new { id });
            }
        }

        [HttpGet("search")]
        public List<Product> Search(string name = null, decimal? priceFrom = null,
            decimal? priceTo = null)
        {
            var sql = @"SELECT * FROM Product WHERE
                        (name = @name OR @name IS NULL) AND
                        (price >= @priceFrom OR @priceFrom IS NULL) AND 
                        (price <= @priceTo OR @priceTo IS NULL)
                        ";
            using (var conn = new SqlConnection(
                configuration.GetConnectionString("connString")))
            {
                return conn.Query<Product>(sql, new { name, priceFrom, priceTo }).ToList();
            }
        }

    }
}
