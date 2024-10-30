using Lotto.Attribute;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Lotto.Dtos
{
    [Keyless]
    public class BetgameDto
    {
        public string Game { get; set; } = null!;

        public string PlayerName { get; set; } = null!;

        public int Betamount { get; set; }

        [NumberRange(1, 50, ErrorMessage = "號碼必須在 1 到 50 之間")]
        [Required(ErrorMessage = "必填")]
        public List<int> Numbers { get; set; } = new List<int>();
    

    }
}
