using Lotto.Dtos;

namespace Lotto.ViewModel
{
    public class PlayerinfoViewModel
    {
        public FindinfoDto Findinfo { get; set; } = null!;

        public List<WinningResultDto> Winningresult { get; set; } = new List<WinningResultDto>();
    }
}
