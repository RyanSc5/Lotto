using System.ComponentModel;

namespace Lotto.Dtos
{
    public class FindpasswordDto
    {
        [DisplayName("玩家名稱")]
        public string PlayerName { get; set; } = null!;

        [DisplayName("第二組密碼")]
        public string Password_hint { get; set; } = null!;
    }
}
