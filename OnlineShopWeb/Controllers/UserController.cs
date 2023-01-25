using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopWeb.JWT;
using System.Data.SqlClient;
using System.Security.Claims;

namespace OnlineShopWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IJwtAuthManager jwtAuthManager;

        public UserController(IConfiguration configuration,
            IJwtAuthManager jwtAuthManager)
        {
            this.configuration = configuration;
            this.jwtAuthManager = jwtAuthManager;
        }

        [HttpGet("login")]
        public LoginResult Login(string username, string password)
        {
            using (var conn = new SqlConnection(
                configuration.GetConnectionString("connString")))
            {
                var employee = conn.QueryFirstOrDefault<Employee>(
                    "SELECT * FROM [Employee] WHERE username = @username AND password = @password",
                    new { username, password });
                if (employee != null)
                {
                    var claims = new Claim[] { new Claim(ClaimTypes.Name, username) };
                    var result = jwtAuthManager.GenerateTokens(username, claims, DateTime.Now);
                    return new LoginResult
                    {
                        Employee = employee,
                        AccessToken = result.AccessToken,
                        RefreshToken = result.RefreshToken.TokenString
                    };
                }
            }
            return null;
        }
    }

    public class LoginResult
    {
        public Employee Employee { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
