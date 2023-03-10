using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Authorization;

namespace GameshopWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class GameController : ControllerBase
    {
        private IConfiguration configuration; 
        public GameController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet("genres")]
        public List<Genre> GetGenres()
        {
            using (var conn = new SqlConnection(
                configuration.GetConnectionString("gameshopConnString")))
            {
                return conn.Query<Genre>("SELECT * FROM Genre").ToList();
            }
        }

        [HttpGet("companies")]
        public List<Company> GetCompanies()
        {
            using (var conn = new SqlConnection(
                configuration.GetConnectionString("gameshopConnString")))
            {
                return conn.Query<Company>("SELECT * FROM Company").ToList();
            }
        }

        [HttpGet("bygenre/{id}")]
        public List<Game> GetByGenre(int id)
        {
            using (var conn = new SqlConnection(
                configuration.GetConnectionString("gameshopConnString")))
            {
                return conn.Query<Game>(
                "SELECT * FROM Game WHERE idGenre = @id", new { id }).ToList();
            }
        }

        [HttpGet("bycompany/{id}")]
        public List<Game> GetByCompany(int id)
        {
            using (var conn = new SqlConnection(
                configuration.GetConnectionString("gameshopConnString")))
            {
                return conn.Query<Game>(
                "SELECT * FROM Game WHERE (idPublisher = @id OR idDeveloper = @id)", 
                new { id }).ToList();
            }
        }

        [HttpGet("search/{text}")]
        public List<Game> Search(string text)
        {
            using (var conn = new SqlConnection(
                configuration.GetConnectionString("gameshopConnString")))
            {
                return conn.Query<Game>(
                 @"SELECT Game.* FROM Game
                    INNER JOIN Genre ON Game.idGenre = Genre.id
                    INNER JOIN Company Developer ON Game.idDeveloper = Developer.id
                    INNER JOIN Company Publisher ON Game.idPublisher = Publisher.id
                   WHERE Title LIKE @text OR Genre.name LIKE @text OR
                    Developer.name LIKE @text OR Publisher.name LIKE @text",
                 new { text = $"%{text}%" }
                    ).ToList();
            }
        }

        [HttpGet("listModel")]
        [Authorize]
        public ListModel GetListModel()
        {
            using (var conn = new SqlConnection(
                configuration.GetConnectionString("gameshopConnString")))
            {
                //var listModel = new ListModel();                
                var listModel = new ListModel
                {
                    Genres = conn.Query<Genre>("SELECT * FROM genre").ToList(),
                    Companies = conn.Query<Company>("SELECT * FROM company").ToList(),
                    Games = conn.Query<Game>("SELECT * FROM Game").ToList()
                };
                foreach (var game in listModel.Games)
                {
                    game.Developer = listModel.Companies.FirstOrDefault(c => c.Id == game.IdDeveloper);
                    game.Publisher = listModel.Companies.FirstOrDefault(c => c.Id == game.IdPublisher);
                    game.Genre = listModel.Genres.FirstOrDefault(g => g.Id == game.IdGenre);
                }
                return listModel;
            }
        }

        [HttpGet("editModel/{id}")]
        [Authorize]
        public EditModel GetEditModel(int id)
        {
            using (var conn = new SqlConnection(
                configuration.GetConnectionString("gameshopConnString")))
            {
                var genres = conn.Query<Genre>("SELECT * FROM genre").ToList();
                var companies = conn.Query<Company>("SELECT * FROM company").ToList();
                Game game;
                if (id == 0)
                    game = new Game();
                else
                    game = conn.QueryFirstOrDefault<Game>("SELECT * FROM game WHERE id = @id", new { id });
                return new EditModel
                {
                    Companies = companies,
                    Genres = genres,
                    Game = game
                };
            }
        }

        [HttpPost("")]
        [Authorize]
        public Game Create(Game game)
        {
            using (var conn = new SqlConnection(
                configuration.GetConnectionString("gameshopConnString")))
            {
                var sql = @"INSERT INTO [dbo].[Game]
                       ([title],[idGenre],[idPublisher]
                       ,[price],[idDeveloper],[releaseDate]
                       ,[image]) OUTPUT inserted.id
                 VALUES
                       (@title,@idGenre,@idPublisher
                       ,@price,@idDeveloper,@releaseDate
                       ,@image)";
                game.Id = conn.ExecuteScalar<int>(sql, game);
                return game;
            }
        }

        [HttpPut("")]
        [Authorize]
        public Game Update(Game game)
        {
            using (var conn = new SqlConnection(
                configuration.GetConnectionString("gameshopConnString")))
            {
                var sql = @"UPDATE [dbo].[Game]
                   SET [title] = @title
                      ,[idGenre] = @idGenre
                      ,[idPublisher] = @idPublisher
                      ,[price] = @price
                      ,[idDeveloper] = @idDeveloper
                      ,[releaseDate] = @releaseDate
                      ,[image] = @image
                 WHERE id = @id";
                conn.Execute(sql, game);
                return game;
            }
        }
    }

    

    public class ListModel
    {
        public List<Genre> Genres { get; set; }
        public List<Company> Companies { get; set; }
        public List<Game> Games { get; set; }
    }

    public class EditModel
    {
        public List<Genre> Genres { get; set; }
        public List<Company> Companies { get; set; }
        public Game Game { get; set; }
    }
}
