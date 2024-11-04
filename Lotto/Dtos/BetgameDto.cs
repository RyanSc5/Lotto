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

       public List<int> Numbers { get; set; } = new List<int>();
    

    }
}
