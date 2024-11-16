using System.ComponentModel.DataAnnotations;

namespace softvago_API.Models
{
    public class Job
    {
        [Required] public long id { get; set; }
        [Required] public string title { get; set; }
        [Required] public string enterprise { get; set; }
        [Required] public string urlRedirection { get; set; }
        [Required] public string shortDescription { get; set; }
        [Required] public string location { get; set; }
        [Required] public double wage { get; set; }
        [Required] public int idModality { get; set; }
        [Required] public int clicks { get; set; }
        [Required] public bool enable { get; set; } = true;
    }

    public class JobSearchParameters
    {
        public string Keywords { get; set; } = "it";
        public string? Location { get; set; }
        public int? MinSalary { get; set; }
        public int? MaxSalary { get; set; }
        public int? DatePublication { get; set; }
        public int? IdModality { get; set; }
    }

    public class Api
    {
        [Required] public int id { get; set; }
        [Required] public string apiName { get; set; }
        [Required] public string baseUrl { get; set; }
        [Required] public string description { get; set; }
        [Required] public bool enable { get; set; }
    }

    public class Modality
    {
        [Required] public int id { get; set; }
        [Required] public string name { get; set; }
        [Required] public string description { get; set; }
        [Required] public bool enable { get; set; }
    }

    public class Rol
    {
        [Required] public int id { get; set; }
        [Required] public string name { get; set; }
        [Required] public string description { get; set; }
        [Required] public bool enable { get; set; }
    }

    public class User
    {
        [Required] public int id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MinLength(1, ErrorMessage = "El nombre no puede estar vacío")]
        public string name { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [MinLength(1, ErrorMessage = "El apellido no puede estar vacío")]
        public string lastName { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        public string email { get; set; }

        [Required(ErrorMessage = "La fecha de registro es obligatoria")]
        public string registrationDate { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio")]
        public int idRol { get; set; }

        [Required(ErrorMessage = "El estado de habilitación es obligatorio")]
        public bool enable { get; set; }

        [Required(ErrorMessage = "La información de login es obligatoria")]
        public Login? login { get; set; }
    }

    public class JWT
    {
        [Required] public string Key { get; set; }
        [Required] public string Issuer { get; set; }
        [Required] public string Audience { get; set; }
        [Required] public string Subject { get; set; }
    }

    public class Login
    {
        [Required] public string username { get; set; }
        [Required] public string password { get; set; }
    }

    public class userAuth
    {
        [Required] public User user { get; set; }
        [Required] public string token { get; set; }
    }
}