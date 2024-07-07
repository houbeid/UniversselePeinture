using System.ComponentModel.DataAnnotations;

namespace UniverssellePeintureApi.DTO
{
    public class AddUserDto
    {
        [Required]
        public string userName { get; set; } = string.Empty;

        [Required]
        public string password { get; set; } = string.Empty;
        public string? email { get; set; }

        [Required]
        public string phone { get; set; } = string.Empty;

        [Required]
        public bool IsAdmin { get; set; }
    }
}
