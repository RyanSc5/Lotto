using System.ComponentModel;

namespace Lotto.Dtos
{
    public class WinningResultDto
    {
        [DisplayName("獎項")]
        public string Ismatch { get; set; } = null!;

        [DisplayName("中獎次數")]
        public int Count { get; set; }
    }
}
