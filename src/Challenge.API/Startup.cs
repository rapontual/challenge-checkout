using System;
using System.Threading;
using Challenge.API.Extensions;
using Challenge.API.Models;
using Challenge.Core.Settings;
using Challenge.Data;
using Challenge.Data.Repository;
using Challenge.Data.Repository.Interfaces;
using Challenge.Service;
using Challenge.Service.BankClient;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Prometheus;

namespace Challenge.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //services.AddCors(opt => 
            //{
            //    opt.AddPolicy(
            //        "AllowAll",
            //        policy => { policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); });
            //});

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });


            // Settings
            services.Configure<ChallengeSettings>(Configuration.GetSection("ChallengeSettings"));

            // EF & Repositories
            services.AddDbContext<ChallengeDbContext>(options =>  
            {
                options.UseSqlServer(
                    Configuration.GetConnectionString("ChallengeDbContext"),
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

            // Services
            services.AddScoped<IApprovePaymentService, ApprovePaymentService>();
            services.AddScoped<ITokenService, TokenService>();

            // External BankService 
            services.AddScoped<IBankApprovalService, BrankApprovalServiceFake>();

            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ChallegeAPI V1",
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                   {
                     new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       },
                       Scheme = "Bearer"
                      },
                      new string[] { }
                    }
                  });
            });


            // Authentication
            var settings = Configuration.GetSection("ChallengeSettings").Get<ChallengeSettings>();

            var key = System.Text.Encoding.ASCII.GetBytes(settings.SecretKey);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateTokenReplay = false
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ChallengeDbContext context, ILogger<ErrorDetails> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }

            // EF
            context.Database.EnsureCreated();

            // General error handling and logging
            app.ConfigureExceptionHandler(logger);

            // Swagger
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChallegeAPI V1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCors("AllowAll");

            // Prometheus
            app.UseMetricServer();
            app.UseHttpMetrics();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMetrics();
                endpoints.MapControllers();
            });
        }
    }
}
