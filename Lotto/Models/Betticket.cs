using System;
using System.Collections.Generic;

namespace Lotto.Models
{
    public partial class Betticket
    {
        public int BetId { get; set; }
        public string? Game { get; set; }
        public string? PlayerName { get; set; }
        public Guid? Newid { get; set; }
        public int Ladder { get; set; }
        public DateTime? Bettime { get; set; }
        public int? Betamount { get; set; }
        public int Num1 { get; set; }
        public int Num2 { get; set; }
        public int Num3 { get; set; }
        public int Num4 { get; set; }
        public int Ismatch { get; set; }
        public string? update { get; set; }
    }
}
