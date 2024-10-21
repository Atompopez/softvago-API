using System.ComponentModel.DataAnnotations;

namespace softvago_API.Models
{
    public class Job
    {
        [Required] public string title;
        [Required] public string enterprise;
        [Required] public string urlRedirection;
        [Required] public string shortDescription;
        [Required] public double wage;
        [Required] public int idLocation;
        [Required] public int idModality;
        [Required] public int clicks;
    }
}
    