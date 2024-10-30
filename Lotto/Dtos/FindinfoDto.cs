using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace Lotto.Dtos
{
    [Keyless]
    public class FindinfoDto
    {
        [DisplayName("玩家名稱")]
        public string PlayerName { get; set; } = null!;

        [DisplayName("錢包")]
        public int Wallet { get; set; }

        [DisplayName("會員等級")]
        public string Level { get; set; } = null!;

        [DisplayName("玩家帳號")]
        public string Login { get; set; } = null!;

        [DisplayName("E-mail")]
        public string Email { get; set; } = null!;

        [DisplayName("建立日期")]
        public DateTime? Create_Day { get; set; }

    }
}
