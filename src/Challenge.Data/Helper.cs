using System;
using System.Security.Cryptography;
using System.Text;
using Challenge.Data.Repository;
using Challenge.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Challenge.Data
{
    public static class Helper
    {
        public static string GetHash(string text)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));

                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        public static void ConfigureDatabaseAndRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ChallengeDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("ChallengeDbContext"),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                    });
            });

            services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
            services.AddScoped<ISecurityRepository, SecurityRepository>();
        }
    }
}
