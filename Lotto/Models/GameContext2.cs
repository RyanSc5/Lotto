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

        // 下注使用的SET
        public virtual DbSet<BetgameDto> BetgameDto { get; set; } = null!;

        // 查詢交易紀錄使用的SET
        public virtual DbSet<FindtranDto> FindtranDto { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Lotto.Dtos.FindinfoDto>? FindinfoDto { get; set; }
                
    }
}
