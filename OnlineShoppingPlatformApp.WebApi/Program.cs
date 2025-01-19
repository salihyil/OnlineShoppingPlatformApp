using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineShoppingPlatformApp.Business.DataProtection;
using OnlineShoppingPlatformApp.Business.Operations.Order;
using OnlineShoppingPlatformApp.Business.Operations.Product;
using OnlineShoppingPlatformApp.Business.Operations.Setting;
using OnlineShoppingPlatformApp.Business.Operations.User;
using OnlineShoppingPlatformApp.Data.Context;
using OnlineShoppingPlatformApp.Data.Repositories;
using OnlineShoppingPlatformApp.Data.UnitOfWork;
using OnlineShoppingPlatformApp.WebApi.Middlewares;
using Scalar.AspNetCore;
using System.Text;

namespace OnlineShoppingPlatformApp.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // CORS configuration
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.AddOpenApi("v1", options => { options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); });

            // db bağlantı
            builder.Services.AddDbContext<OnlineShoppingPlatformAppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // data protection
            builder.Services.AddScoped<IDataProtection, DataProtection>();
            var keysDirectory = new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "App_Data", "Keys"));
            // başka projelerde de kullanılabilir olması için
            builder.Services.AddDataProtection()
                            .SetApplicationName("OnlineShoppingPlatformApp")
                            .PersistKeysToFileSystem(keysDirectory);

            //jwt
            builder.Services.AddAuthentication(options =>
            {
                // [Authorize] 302 alıyoduk -> 401 unauthorized hatası almak için
                // jwt ile authentication yapılacak
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                // jwt ile challenge yapılacak
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                // jwt ile scheme yapılacak
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // issuer: token'ın hangi sunucudan geldiğini kontrol eder  
                        ValidateIssuer = true,
                        // audience: token'ın hangi kullanıcıya ait olduğunu kontrol eder
                        ValidateAudience = true,
                        // lifetime: token'ın süresi dolup dolmadığını kontrol eder
                        ValidateLifetime = true,
                        // signing key: token'ın imzalanıp imzalanmadığını kontrol eder
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
                    };
                });

            // generic olduğu için typeof kullanıldı.
            //IRepository<> türünden bir servis talep edildiğinde, DI konteyneri Repository<> türünden bir örnek döndürür.
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // dependency injection yapacağımız için lifetime scoped servisleri ekliyoruz
            //"Scoped" lifetime kullan(yani her HTTP isteği için yeni bir örnek)
            builder.Services.AddScoped<IUserService, UserManager>();
            builder.Services.AddScoped<IProductService, ProductManager>();
            builder.Services.AddScoped<IOrderService, OrderManager>();
            builder.Services.AddScoped<ISettingService, SettingManager>();
            //"Ne zaman ISettingService istenirse, SettingManager sınıfının bir örneğini oluştur"


            var app = builder.Build();

            app.MapScalarApiReference(options =>
           {
               options.WithTitle("Online Shopping Platform API")
                      .WithTheme(ScalarTheme.Default)
                      .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Fetch);

               options.Authentication = new ScalarAuthenticationOptions
               {
                   PreferredSecurityScheme = "Bearer"
               };
           });


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            // Exception handling middleware 
            app.UseGlobalExceptionHandler();

            app.UseHttpsRedirection();

            // Enable CORS
            app.UseCors();

            // Authentication & Authorization
            app.UseAuthentication();
            app.UseRequestLogging();
            app.UseAuthorization();

            app.UseMaintenanceMode();

            app.MapControllers();

            app.Run();
        }
    }
}
