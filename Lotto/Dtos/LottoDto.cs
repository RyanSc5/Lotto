using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace Lotto.Dtos
{
    [Keyless]
    public class LottoDto
    {
        public int LottoId { get; set; }

        [DisplayName("獎期")]
        public int Ladder { get; set; }

        [DisplayName("開獎時間")]
        public DateTime? Opentime { get; set; }

        [DisplayName("號碼一")]
        public int? Num1 { get; set; }

        [DisplayName("號碼二")]
        public int? Num2 { get; set; }

        [DisplayName("號碼三")]
        public int? Num3 { get; set; }

        [DisplayName("號碼四")]
        public int? Num4 { get; set; }

        [DisplayName("頭獎金額")]
        public int? Prizepool { get; set; }

        [DisplayName("開出頭獎")]
        public string? Iswin { get; set; }

        [DisplayName("開獎結果")]
        public string? Status { get; set; }
    }
}
