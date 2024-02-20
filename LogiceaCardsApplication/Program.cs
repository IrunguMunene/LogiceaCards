using LogiceaCardDomain;
using LogiceaCardsApplication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace LogiceaCardsApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);         

            builder.Services.AddDbContext<LogiceaCardDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("CardsContext")));

            builder.Services.AddScoped(provider =>
            {
                var secretKey = builder.Configuration["Jwt:SecretKey"]; 
                var issuer = builder.Configuration["Jwt:Issuer"]; 
                var audience = builder.Configuration["Jwt:Audience"];

                return new JwtService(secretKey, issuer, audience);
            });

            // Configure JWT authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])),
                };
            });

            builder.Services.AddScoped<LookUpService>();

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Logicae Cards API", Version = "v1" });

                // Include the Authorization header in Swagger requests
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new List<string>()
                    }
                });
            });

            var app = builder.Build();

            // Create the database and seed it with data.
            using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var logiceaCardDbContext = serviceScope.ServiceProvider.GetService<LogiceaCardDbContext>();
                logiceaCardDbContext.Database.Migrate();
                LogiceaDbInitializer.Seed(logiceaCardDbContext);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                _ = endpoints.MapControllers();
            });

            app.Run();
        }
    }
}
