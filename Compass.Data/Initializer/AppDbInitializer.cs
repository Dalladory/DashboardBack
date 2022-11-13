using Compass.Data.Data.Context;
using Compass.Data.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compass.Data.Initializer
{
    public class AppDbInitializer
    {         
        public static async Task Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                UserManager<AppUser> userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                if (userManager.FindByNameAsync("master").Result == null)
                {
                    AppUser admin = new AppUser()
                    {
                        UserName = "master",
                        Email = "master@email.com",
                        EmailConfirmed = true,
                        Name = "John",
                        Surname = "Snow"
                    };

                    AppUser user = new AppUser()
                    {
                        UserName = "user",
                        Email = "user@email.com",
                        EmailConfirmed = true,
                        Name = "Bart",
                        Surname = "Simpson"
                    };

                    context.Roles.AddRange(
                        new IdentityRole()
                        {
                            Name = "Administrators",
                            NormalizedName = "ADMINISTRATORS"
                        },
                        new IdentityRole()
                        {
                            Name = "Users",
                            NormalizedName = "USERS"
                        }
                    );

                    await context.SaveChangesAsync();

                    IdentityResult resultadmin = userManager.CreateAsync(admin, "Qwerty-1").Result;
                    IdentityResult resultuser = userManager.CreateAsync(user, "Qwerty-1").Result;

                    if (resultadmin.Succeeded)
                    {
                        userManager.AddToRoleAsync(admin, "Administrators").Wait();
                    }
                    if (resultuser.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Users").Wait();
                    }
                }
            }
               
        }
    }
}
