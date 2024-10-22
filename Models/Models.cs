using System.ComponentModel.DataAnnotations;

namespace softvago_API.Models
{
    public class Job
    {
        [Required] public int id;
        [Required] public string title;
        [Required] public string enterprise;
        [Required] public string urlRedirection;
        [Required] public string shortDescription;
        [Required] public double wage;
        [Required] public int idLocation;
        [Required] public int idModality;
        [Required] public int clicks;
        [Required] public bool enable;
    }

    public class Location
    {
        [Required] public int id;
        [Required] public string city;
        [Required] public string country;
        [Required] public bool enable;
    }

    public class Api
    {
        [Required] public int id;
        [Required] public string apiName;
        [Required] public string baseUrl;
        [Required] public string description;
        [Required] public bool enable;
    }

    public class Modality
    {
        [Required] public int id;
        [Required] public string name;
        [Required] public string description;
        [Required] public bool enable;
    }

    public class Rol
    {
        [Required] public int id;
        [Required] public string name;
        [Required] public string description;
        [Required] public bool enable;
    }

    public class User
    {
        [Required] public int id;
        [Required] public string name;
        [Required] public string lastName;
        [Required] public string email;
        [Required] public string password;
        [Required] public string registrationDate;
        [Required] public int idRol;
        [Required] public bool enable;
    }

    public class ResponseAPI
    {
        public bool success;
        public string message;
        public dynamic data;
    }
}
    