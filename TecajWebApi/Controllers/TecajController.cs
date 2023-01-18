using Dapper;
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


    }
}
