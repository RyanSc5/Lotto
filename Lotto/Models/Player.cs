using System;
using System.Collections.Generic;

namespace Lotto.Models
{
    public partial class Player
    {
        public int ID { get; set; }
        public Guid? Newid { get; set; }
        public string PlayerName { get; set; } = null!;
        public int Wallet { get; set; }
        public int Bettotal { get; set; }
        public string Level { get; set; } = null!;
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password_hint { get; set; } = null!;
        public DateTime? Create_Day { get; set; }
    }
}
