using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace TecajWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TecajController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public TecajController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet("")]
        public List<TecajnaLista> GetAll()
        {
            using(var conn = new SqlConnection(configuration.GetConnectionString("connString")))
            {
                return conn.Query<TecajnaLista>("SELECT * FROM TecajnaLista").ToList();
            }
        }

        [HttpGet("izracun")]
        public IActionResult Izracun(string valuta, string tip, double iznos)
        {
            using (var conn = new SqlConnection(configuration.GetConnectionString("connString")))
            {
                var zapis = conn.QueryFirstOrDefault<TecajnaLista>(
                   "SELECT * FROM TecajnaLista WHERE valuta = @valuta",
                   new { valuta });
                if (zapis == null)
                    return BadRequest("Neispravna valuta");
                if (tip == "kupovni" || tip == "k")
                    return Ok(iznos * zapis.Kupovni);
                else if (tip == "prodajni" || tip == "p")
                    return Ok(iznos * zapis.Prodajni);
                else if (tip == "srednji" || tip == "s")
                    return Ok(iznos * zapis.Srednji);
                else
                    return BadRequest("Neispravan tip");
            }
        }

        [HttpPost("")]
        [Authorize]
        public TecajnaLista Create(TecajnaLista zapis)
        {
            using (var conn = new SqlConnection(configuration.GetConnectionString("connString")))
            {
                var sql = @"INSERT INTO TecajnaLista(
                    datum,drzava,valuta,kupovni,srednji,prodajni
                    ) OUTPUT inserted.id VALUES(
                    @datum,@drzava,@valuta,@kupovni,@srednji,@prodajni
                    )";
                if (zapis.Datum == null)
                    zapis.Datum = DateTime.Now;
                zapis.Id = conn.ExecuteScalar<int>(sql, zapis);
                return zapis;
            }
        }

        [HttpPut("")]
        [Authorize]
        public void Update(TecajnaLista zapis)
        {
            using (var conn = new SqlConnection(
                configuration.GetConnectionString("connString")))
            {
                var sql = @"UPDATE TecajnaLista SET 
                datum=@datum,drzava=@drzava,valuta=@valuta,kupovni=@kupovni,
                srednji=@srednji,prodajni=@prodajni
                WHERE id = @id";
                conn.Execute(sql, zapis);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public void Delete(int id)
        {
            using (var conn = new SqlConnection(
                configuration.GetConnectionString("connString")))
            {
                var sql = "DELETE FROM TecajnaLista WHERE id = @id";
                conn.Execute(sql, new { id });
            }
        }

    }
}
