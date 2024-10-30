using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lotto.Dtos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "帳號不得空白")]
        [DisplayName("帳號")]
        public string Login { get; set; } = null!;

        [Required(ErrorMessage = "密碼不得空白")]
        [DisplayName("密碼")]
        public string Password { get; set; } = null!;
    }
}
