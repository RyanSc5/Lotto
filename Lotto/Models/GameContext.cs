using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Lotto.Models
{
    public partial class GameContext : DbContext
    {
        public GameContext()
        {
        }

        public GameContext(DbContextOptions<GameContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Betticket> Betticket { get; set; } = null!;
        public virtual DbSet<Betticket_ARCHIVE> Betticket_ARCHIVE { get; set; } = null!;
        public virtual DbSet<LottoNumber> LottoNumber { get; set; } = null!;
        public virtual DbSet<LottoNumber_ARCHIVE> LottoNumber_ARCHIVE { get; set; } = null!;
        public virtual DbSet<Player> Player { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=127.0.0.1;Database=Game;User ID=lotto;Password=1qaz@WSX;TrustServerCertificate=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Betticket>(entity =>
            {
                entity.HasKey(e => new { e.Ladder, e.BetId });

                entity.HasIndex(e => new { e.PlayerName, e.Bettime }, "IDX_PlayerName");

                entity.Property(e => e.BetId).ValueGeneratedOnAdd();

                entity.Property(e => e.Bettime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Game)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.PlayerName).HasMaxLength(50);
            });

            modelBuilder.Entity<Betticket_ARCHIVE>(entity =>
            {
                entity.HasKey(e => new { e.Ladder, e.BetId });

                entity.HasIndex(e => new { e.PlayerName, e.Bettime }, "IDX_PlayerName");

                entity.Property(e => e.BetId).ValueGeneratedOnAdd();

                entity.Property(e => e.Bettime).HasColumnType("datetime");

                entity.Property(e => e.Game)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.PlayerName).HasMaxLength(50);
            });

            modelBuilder.Entity<LottoNumber>(entity =>
            {
                entity.HasKey(e => new { e.Ladder, e.LottoId });

                entity.Property(e => e.LottoId).ValueGeneratedOnAdd();

                entity.Property(e => e.Opentime).HasColumnType("datetime");
            });

            modelBuilder.Entity<LottoNumber_ARCHIVE>(entity =>
            {
                entity.HasKey(e => new { e.Ladder, e.LottoId });

                entity.Property(e => e.LottoId).ValueGeneratedOnAdd();

                entity.Property(e => e.Opentime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasIndex(e => new { e.Login, e.Password }, "IDX_Login");

                entity.HasIndex(e => e.PlayerName, "IDX_PlayerName");

                entity.Property(e => e.Create_Day)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.Level)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Login)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .UseCollation("Chinese_Taiwan_Stroke_CS_AS");

                entity.Property(e => e.Newid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Password)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .UseCollation("Chinese_Taiwan_Stroke_CS_AS");

                entity.Property(e => e.Password_hint)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.PlayerName).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
