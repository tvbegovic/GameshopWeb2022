using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace VjezbaBlic1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PolaznikController : ControllerBase
    {
        private IConfiguration configuration;
        public PolaznikController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet("")]
        public List<Polaznik> GetAll()
        {
            using(var conn = new SqlConnection(
                configuration.GetConnectionString("blicConnString")))
            {
                return conn.Query<Polaznik>("SELECT * FROM Polaznik").ToList();
            }
        }

        [HttpGet("{id}")]
        public List<Polaznik> GetByMjesto(int id)
        {
            using (var conn = new SqlConnection(
                configuration.GetConnectionString("blicConnString")))
            {
                return conn.Query<Polaznik>("SELECT * FROM Polaznik WHERE idMjesto = @id",
                    new { id }).ToList();
            }
        }

        [HttpGet("search/{text}")]
        public List<Polaznik> Search(string text)
        {
            using (var conn = new SqlConnection(
                configuration.GetConnectionString("blicConnString")))
            {
                return conn.Query<Polaznik>(
                    @"SELECT * FROM Polaznik 
                      WHERE ime LIKE @text OR prezime LIKE @text",
                    new { text = $"%{text}%" }).ToList();
            }
        }
    }
}
