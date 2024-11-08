using System;
using System.Collections.Generic;

namespace Lotto.Models
{
    public partial class LottoNumber_ARCHIVE
    {
        public int LottoId { get; set; }
        public int Ladder { get; set; }
        public DateTime? Opentime { get; set; }
        public int? Num1 { get; set; }
        public int? Num2 { get; set; }
        public int? Num3 { get; set; }
        public int? Num4 { get; set; }
        public int? Prizepool { get; set; }
        public bool? Iswin { get; set; }
        public bool? Valid { get; set; }
        public int? Status { get; set; }
    }
}
