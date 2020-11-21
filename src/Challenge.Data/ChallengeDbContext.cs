using System;
using Challenge.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;

namespace Challenge.Data
{
    public class ChallengeDbContext : DbContext
    {
        // These strings can be moved to config file
        private readonly byte[] encryptionKey = Convert.FromBase64String("AAECAwQFBgcICQoLDA0ODw==");
        private readonly byte[] encryptionIV = Convert.FromBase64String("AAECAwQFBgcICQoLDA1ODx==");
        private readonly IEncryptionProvider _provider;

        public ChallengeDbContext(DbContextOptions options) : base(options)
        {
            this._provider = new AesProvider(this.encryptionKey, this.encryptionIV);
        }

        public DbSet<PaymentStatus> PaymentStatus { get; set; }

        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

        public DbSet<Merchant> Merchant { get; set; }

        public DbSet<Currency> Currency { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseEncryption(this._provider);

            modelBuilder.Entity<PaymentStatus>()
                .ToTable("PaymentStatus");

            modelBuilder.Entity<PaymentStatus>().HasData(
                new PaymentStatus
                {
                    Id = 1,
                    StatusDescription = "Payment successful"
                    
                },
                new PaymentStatus
                {
                    Id = 2,
                    StatusDescription = "Payment denied"

                }, new PaymentStatus
                {
                    Id = 3,
                    StatusDescription = "Payment pending"

                },
                new PaymentStatus
                {
                    Id = 4,
                    StatusDescription = "Payment process error"

                }
            );

            modelBuilder.Entity<Merchant>().HasData(
                new Merchant
                {
                    Id = new Guid("3FA85F64-5717-4562-B3FC-2C963F66AFA6"),
                    Name = "Challenge Merchant Admin",
                    Login = "challenge",
                    Password = "password",
                    IsAdmin = true
                }
             ); ;

            modelBuilder.Entity<Currency>().HasData(
                new Currency
                {
                    Id = 1,
                    Name = "Dollar"
                },
                new Currency
                {
                    Id = 2,
                    Name = "Euro"
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
