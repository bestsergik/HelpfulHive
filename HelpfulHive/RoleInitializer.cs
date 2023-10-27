using Microsoft.AspNetCore.Identity;

namespace HelpfulHive
{
    public static class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Moderator", "User" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // Создаем роли, если они еще не существуют
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Создаем пользователя с ролью Admin, если такого еще нет
            IdentityUser admin = await userManager.FindByEmailAsync("sergynia88@mail.ru");
            if (admin == null)
            {
                IdentityUser user = new IdentityUser()
                {
                    UserName = "admin",
                    Email = "sergynia88@mail.ru",
                };
                IdentityResult result = await userManager.CreateAsync(user, "Bestwerwolfa_7");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }

}
