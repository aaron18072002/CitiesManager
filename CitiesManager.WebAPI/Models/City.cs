using System.ComponentModel.DataAnnotations;

namespace CitiesManager.WebAPI.Models
{
    public class City
    {
        [Key]
        public Guid CityId { get; set; }

        [Required]
        [MaxLength(40)]
        public string? CityName { get; set; }
    }
}
