
using BookLendingBackUp.Application.Interfaces;
using BookLendingBackUp.Application.Settings;
using BookLendingBackUp.Infrastructure.Common.Mappings;
using BookLendingBackUp.Infrastructure.Entities;
using BookLendingBackUp.Infrastructure.Interfaces;
using BookLendingBackUp.Infrastructure.Persistence;
using BookLendingBackUp.Infrastructure.Services;
using Hangfire;
using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

namespace BookLendingBackUp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
            builder.Services.Configure<JwtSettings>(jwtSettingsSection);
            var jwtSettings = jwtSettingsSection.Get<JwtSettings>();


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("BookLendingBackUp.Infrastructure")
                )
            );
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
            })


           .AddEntityFrameworkStores<ApplicationDbContext>()
           .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
            .AddJwtBearer("JwtBearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    RoleClaimType = ClaimTypes.Role
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy =>
                    policy.RequireRole("Admin"));


                options.AddPolicy("AdminOrMember", policy =>
                    policy.RequireRole("Admin", "Member"));
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularClient",
              policy => policy.WithOrigins("http://localhost:4200")
                       .AllowAnyMethod()
                       .AllowAnyHeader());
                //options.AddPolicy("BookPolicy", policy =>
                //{
                //    policy.WithOrigins("https://books.frontend.com")
                //          .AllowAnyHeader()
                //          .AllowAnyMethod();
                //});

                //options.AddPolicy("BorrowPolicy", policy =>
                //{
                //    policy.WithOrigins("https://borrow.frontend.com")
                //          .AllowAnyHeader()
                //          .AllowAnyMethod();
                //});

            }); 



            builder.Services.AddHangfire(config =>
                config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddHangfireServer();


            builder.Services.AddAutoMapper(typeof(MappingProfile));


            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IEmailSender, SendGridEmailSender>();
            builder.Services.AddScoped<ICatalog, CatalogService>();
            builder.Services.AddScoped<IBook, BookService>();
            builder.Services.AddScoped<IBorrowBook, BorrowBookService>();
            builder.Services.AddScoped<DelayedBookCheckerService>();


            builder.Services.AddControllers();
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "BookLending API",
                    Version = "v1",
                    Description = "📚 Book Lending System API - Manage users, books, and borrowing logic",
                    Contact = new OpenApiContact
                    {
                        Name = "Basma Khalaf",
                        Email = "basmakhalaf974@gmail.com"
                    }
                });

                
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
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
            Array.Empty<string>()
        }
    });

                
          //      var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
             //   c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();


            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                string[] roles = { "Admin", "Member" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }

            using (var scope = app.Services.CreateScope())
            {
                var recurringJobs = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

                recurringJobs.AddOrUpdate<DelayedBookCheckerService>(
                    "check-delayed-books",
                    job => job.CheckOverdueBooksAsync(),
                    Cron.Daily
                );
            }



            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("AllowAngularClient");
            app.UseHangfireDashboard();

            app.MapControllers();

            app.Run();
        }
    }
}
