﻿using System.ComponentModel.DataAnnotations;

namespace softvago_API.Models
{
    public class Job
    {
        [Required] public int id { get; set; }
        [Required] public string title { get; set; }
        [Required] public string enterprise { get; set; }
        [Required] public string urlRedirection { get; set; }
        [Required] public string shortDescription { get; set; }
        [Required] public double wage { get; set; }
        [Required] public int idLocation { get; set; }
        [Required] public int idModality { get; set; }
        [Required] public int clicks { get; set; }
        [Required] public bool enable { get; set; }
    }

    public class Location
    {
        [Required] public int id { get; set; }
        [Required] public string city { get; set; }
        [Required] public string country { get; set; }
        [Required] public bool enable { get; set; }
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
        [Required] public string name { get; set; }
        [Required] public string lastName { get; set; }
        [Required] public string email { get; set; }
        [Required] public string password { get; set; }
        [Required] public string registrationDate { get; set; }
        [Required] public int idRol { get; set; }
        [Required] public bool enable { get; set; }
    }

    public class ResponseAPI
    {
        public bool success { get; set; }
        public string message { get; set; }
        public dynamic data { get; set; }
    }
}