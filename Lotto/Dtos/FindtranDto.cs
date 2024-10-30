using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lotto.Dtos
{
    [Keyless]
    public class FindtranDto
    {
        [DisplayName("遊戲")]
        public string Game { get; set; } = null!;

        [DisplayName("玩家名稱")]
        public string PlayerName { get; set; } = null!;

        [DisplayName("獎期")]
        public int Ladder { get; set; }

        [DisplayName("下注日期")]
        public DateTime Bettime { get; set; }

        [DisplayName("金額")]
        public int Betamount { get; set; }

        [DisplayName("號碼一")]
        public int Num1 { get; set; }

        [DisplayName("號碼二")]
        public int Num2 { get; set; }

        [DisplayName("號碼三")]
        public int Num3 { get; set; }

        [DisplayName("號碼四")]
        public int Num4 { get; set; }

        [DisplayName("獎項")]
        public string Ismatch { get; set; } = null!;

        [DisplayName("狀態")]
        public string Status { get; set; } = null!;

    }
}
