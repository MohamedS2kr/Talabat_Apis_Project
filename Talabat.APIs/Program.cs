using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.APIs.Middlewares;
using Talabat.Core.Interfaces;
using Talabat.Repository.Data;
using Talabat.Repository.Repositories;
using Talabat.APIs.Extensions;
using StackExchange.Redis;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Repository.Identity;
using Microsoft.AspNetCore.Identity;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.Interfaces;
using Talabat.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Talabat.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            #region Configure Services

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<StoreDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            //Extensions Method ���������� ��� � Configure Services ���� ������� � ����� ���� �� �� Method ��  
            builder.Services.AddApplicationServices();
            builder.Services.AddSingleton<IConnectionMultiplexer>(Options =>
            {
                var Connection = builder.Configuration.GetConnectionString("RedisConnection"); 
                return ConnectionMultiplexer.Connect(Connection);
            });

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });


            builder.Services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));
            builder.Services.AddIdentity<AppUser,IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            }).AddEntityFrameworkStores<AppIdentityDbContext>();

            builder.Services.AddScoped<ITokenService, TokenServices>();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                            .AddJwtBearer(options => 
                            {
                                options.TokenValidationParameters = new TokenValidationParameters()
                                {
                                    ValidateIssuer = true,
                                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                                    ValidateAudience = true,
                                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                                    ValidateLifetime = true,
                                    ValidateIssuerSigningKey = true,
                                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
                                };
                            });
            #endregion


            /*
            : ��� ����� ��� ��� ����� ���
            
            StoreDbContext context = StoreDbContext();// �� ��� ����� �� ���� CLR����� ��� ���� �� object�� ���� ��� �� ���� ��
            context.Database.MigrateAsync(); //��� ��� ������� Function �� ��

            * Main �� �� ConfigureServices ��� ���� �� Program ���� �� constrictor  �� ��� �� ���� ���� �� ���� �� ���� ����� 
            * ��� ������ CLR ����� ��� ���� ���� ���� ��� ���� ��� ��
            * �������� ������ �� ��� :
            * 1. Allow Dependance Injection 
            * 2. Build ���� 
            * 3. Build �� �� ��� ����� ���  app ��� ��� ������ �� 
            * 4. var Scope ������� ���� �� CreateScope() ����� �����  Services ���� �� 
            * 5. var Services ������ ��  ServiceProvider ����� ���� �� Scope ��� �� 
            * 6. Var Context  ������ ��  GetRequiredService<StoreDbContext>() �� ���� Services��� ��
            * 7. ����� ���  Object �� ��� �� 
            */
            var app = builder.Build();
            
            using var Scope = app.Services.CreateScope();

            var Services = Scope.ServiceProvider;

            var _Context = Services.GetRequiredService<StoreDbContext>(); //DbContext �� �� Object ���� ������ ��� ��
            var _IdentityContext = Services.GetRequiredService<AppIdentityDbContext>(); //DbContext �� �� Object ���� ������ ��� ��
            var UserManager = Services.GetRequiredService<UserManager<AppUser>>(); //DbContext �� �� Object ���� ������ ��� ��

            var loggerFactory = Services.GetRequiredService<ILoggerFactory>();

            try
            {
                await _Context.Database.MigrateAsync();//Update Database
                await _IdentityContext.Database.MigrateAsync(); //Update Database (Identity) 
                //Data Seeding -> Just one of life time in project
                await StoreDbContextSeed.SeedAsync(_Context); //Data Seeding
                await AppIdentityDbContextSeed.SeedUserAsync(UserManager);

            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An Error has been Occurred During Appling the Migrations");
            }
            // Configure the HTTP request pipeline.


            app.UseMiddleware<ExceptionMiddleware>(); //��� ���� Middleware��� �� ��� ������ ���� ��


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseAuthentication();
           
            app.MapControllers();

            app.Run();
        }
    }
}
