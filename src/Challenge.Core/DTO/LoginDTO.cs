using System.ComponentModel.DataAnnotations;

namespace Challenge.Core.DTO
{
    public class LoginDTO
    {
        [StringLength(10, MinimumLength = 3)]
        public string Login { get; set; }

        [StringLength(10, MinimumLength = 3)]
        public string Password { get; set; }
    }
}
