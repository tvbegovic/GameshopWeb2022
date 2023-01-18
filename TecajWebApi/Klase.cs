using System.Text.Json.Serialization;

namespace TecajWebApi
{
    public class TecajnaLista
    {
        public int Id { get; set; }
        public DateTime? Datum { get; set; }
        public string Drzava { get; set; }
        public string Valuta { get; set; }
        public double? Kupovni { get; set; }
        public double? Srednji { get; set; }
        public double? Prodajni { get; set; }
    }
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
