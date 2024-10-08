using System.ComponentModel.DataAnnotations;

namespace UniverssellePeintureApi.DTO
{
    public class LoginDto
    {
        [Required]
        public string userName {  get; set; }

        [Required] 
        public string password { get; set; }
    }


}
