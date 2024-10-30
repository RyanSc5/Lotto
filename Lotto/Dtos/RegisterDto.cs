using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lotto.Dtos
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "帳號為必填")]
        [RegularExpression(@"^[A-Za-z\d]{8,20}$", ErrorMessage = "帳號必須是 8 到 20 個僅包含字母和數字的字元，且必須包含至少一個字母和一個數字")]
        [DisplayName("帳號")]
        public string Login { get; set; } = null!;

        [Required(ErrorMessage = "密碼為必填")]
        [RegularExpression(@"^[A-Za-z\d]{8,20}$", ErrorMessage = "密碼必須是 8 到 20 個僅包含字母和數字的字元，且必須包含至少一個字母和一個數字")]
        [DisplayName("密碼")]
        public string Password { get; set; } = null!;

        [DisplayName("再次輸入密碼")]
        [Required]
        [Compare("Password", ErrorMessage = "兩組密碼需相同")]
        public string Password2 { get; set; } = null!;


        [Required(ErrorMessage = "E-mail為必填")]
        [EmailAddress(ErrorMessage = "須為Email格式 Ex: abcd@12345.com ")]
        [DisplayName("E-mail")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "密碼提示為必填")]
        [DisplayName("第二組密碼")]
        public string Password_hint { get; set; } = null!;

        [Required(ErrorMessage = "名稱為必填")]
        [DisplayName("玩家名稱")]
        public string PlayerName { get; set; } = null!;
    }
}
