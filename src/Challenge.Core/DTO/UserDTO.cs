using System.ComponentModel.DataAnnotations;

namespace Challenge.Core.DTO
{
    public class UserDTO
    {
        [StringLength(10, MinimumLength = 3, ErrorMessage = "Must be between 3 and 10 characters")]
        [Required]
        public string Login { get; set;  }

        [StringLength(10, MinimumLength = 3, ErrorMessage = "Must be between 3 and 10 characters")]
        [Required]
        public string Password { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Must be between 3 and 50 characters")]
        [Required]
        public string Name { get; set; }
    }
}
