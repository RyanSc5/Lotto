using System;
using System.Collections.Generic;
using Lotto.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Lotto.Models
{
    public partial class GameContext2 : GameContext
    {
        public GameContext2()
        {
        }

        public GameContext2(DbContextOptions<GameContext> options)
            : base(options)
        {
        }
        // 查詢樂透號碼使用的SET
        public virtual DbSet<LottoDto> LottoDto { get; set; } = null!;

        // 查詢個人資料使用的SET
        public virtual DbSet<FindinfoDto> FindinfoDto { get; set; } = null!;

        // 下注使用的SET
        public virtual DbSet<BetgameDto> BetgameDto { get; set; } = null!;

        // 查詢交易紀錄使用的SET
        public virtual DbSet<FindtranDto> FindtranDto { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        
    }
}
