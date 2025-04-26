using CoffeeAndCakeShop.Data;
using CoffeeAndCakeShop.Entities;
using CoffeeAndCakeShop.Setting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CoffeeAndCakeShop.Configuration
{
    public static class ConfigurationService
    {
        public static void ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                        .AddEntityFrameworkStores<ApplicationDbContext>()
                        .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "CakeCoffeeCookie";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.LoginPath = "/login";
                options.SlidingExpiration = true;
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                options.Lockout.MaxFailedAccessAttempts = 3;
            });
        }
        public static async Task SeedData(this WebApplication webApplication, IConfiguration configuration)
        {
            using (var scope = webApplication.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var defaultUser = configuration.GetSection("DefaultUsers")?.Get<DefaultUser>() ?? new DefaultUser();
                var defaultRole = configuration.GetValue<string>("DefaultRole") ?? "Admin";

                try
                {
                    if (!await roleManager.RoleExistsAsync(defaultRole))
                    {
                        await roleManager.CreateAsync(new IdentityRole(defaultRole));
                    }

                    var existUser = await userManager.FindByNameAsync(defaultUser.Username);

                    if (existUser == null)
                    {
                        var user = new ApplicationUser
                        {
                            MobilePhone = defaultUser.MobilePhone,
                            UserName = defaultUser.Username,
                            Fullname = defaultUser.Fullname,
                            Email = defaultUser.Email,
                            EmailConfirmed = true,
                            NormalizedEmail = defaultUser.Email.ToUpper(),
                            AccessFailedCount = 0,
                            IsActive = true,
                        };

                        var identityUser = await userManager.CreateAsync(user, defaultUser.Password);

                        if (identityUser.Succeeded)
                        {
                            await userManager.AddToRoleAsync(user, defaultRole);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }   
    }
}
