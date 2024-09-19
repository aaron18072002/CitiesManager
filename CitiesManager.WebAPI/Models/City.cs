using System.ComponentModel.DataAnnotations;

namespace CitiesManager.WebAPI.Models
{
    public class City
    {
        [Key]
        public Guid CityId { get; set; }

        [Required(ErrorMessage = "City Name is required.")]
        [MaxLength(40, ErrorMessage = "City Name cannot exceed 40 characters.")]
        public string? CityName { get; set; }
    }
}
