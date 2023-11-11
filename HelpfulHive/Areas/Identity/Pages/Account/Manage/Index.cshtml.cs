// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace HelpfulHive.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly IApplicationUserAdapter _userAdapter;
        private readonly UserManager<IdentityUser> _identityUserManager; // Используется для получения ID пользователя
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IWebHostEnvironment _env;

        public IndexModel(
            IApplicationUserAdapter userAdapter,
            UserManager<IdentityUser> identityUserManager,
            SignInManager<IdentityUser> signInManager,
            IWebHostEnvironment env)
        {
            _userAdapter = userAdapter;
            _identityUserManager = identityUserManager;
            _signInManager = signInManager;
            _env = env;
        }
        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        // Список доступных изображений профиля
        public List<string> AvailableProfileImages { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            public string NewUsername { get; set; }
            public IFormFile ProfileImage { get; set; } // Если вы хотите загружать новое изображение
            public string SelectedProfileImagePath { get; set; } // Путь к выбранному изображению
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userAdapter.GetUserNameAsync(user);
            var phoneNumber = await _userAdapter.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                NewUsername = userName,
                SelectedProfileImagePath = user.ProfileImagePath // Убедитесь, что ваш ApplicationUser имеет это свойство
            };
        }



        // Метод для получения доступных изображений
        private void LoadAvailableProfileImages()
        {
            var profileImagesFolder = Path.Combine(_env.WebRootPath, "profileimg");
            AvailableProfileImages = Directory.GetFiles(profileImagesFolder)
                                             .Select(Path.GetFileName)
                                             .ToList();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = _identityUserManager.GetUserId(User); // Используйте identityUserManager для получения ID пользователя
            var user = await _userAdapter.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Ошибка с  '{userId}'.");
            }

            LoadAvailableProfileImages();
            await LoadAsync(user);
            return Page();
        }



        public async Task<IActionResult> OnPostAsync()
        {
            var userId = _identityUserManager.GetUserId(User);

            var user = await _userAdapter.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Ошибка с '{userId}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // Обновление имени пользователя
            if (Input.NewUsername != null && Input.NewUsername != user.UserName)
            {
                user.UserName = Input.NewUsername;
                var updateResult = await _userAdapter.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    StatusMessage = "Ошибка";
                    return RedirectToPage();
                }
            }

            // Обновление изображения профиля
            if (Input.SelectedProfileImagePath != user.ProfileImagePath)
            {
                user.ProfileImagePath = Input.SelectedProfileImagePath;
                var updateResult = await _userAdapter.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    StatusMessage = "Ошибка";
                    return RedirectToPage();
                }
            }

            // Логирование успешного обновления

            await _signInManager.RefreshSignInAsync(await _userAdapter.FindByIdAsync(user.Id));
            StatusMessage = "Данные обновлены";
            return RedirectToPage();
        }



    }

    public interface IApplicationUserAdapter
    {
        Task<ApplicationUser> FindByIdAsync(string userId);
        Task<ApplicationUser> FindByNameAsync(string userName);
        Task<string> GetUserNameAsync(ApplicationUser user);
        Task<IdentityResult> UpdateAsync(ApplicationUser user);
        Task<string> GetPhoneNumberAsync(ApplicationUser user);
        Task<IdentityResult> SetPhoneNumberAsync(ApplicationUser user, string phoneNumber);
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
    }

    public class ApplicationUserAdapter : IApplicationUserAdapter
    {
        private readonly UserManager<IdentityUser> _userManager;
        ApplicationDbContext _context;

        public ApplicationUserAdapter(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            // Получение пользователя с использованием базового запроса UserManager
            var user = await _userManager.FindByIdAsync(userId);

            // Если требуется получить ProfileImagePath, можно выполнить отдельный запрос
            var profileImagePath = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => (string)_context.Entry(u).Property("ProfileImagePath").CurrentValue)
                .SingleOrDefaultAsync();

            // Создание нового ApplicationUser с данными, полученными из UserManager и отдельного запроса
            return new ApplicationUser
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                ProfileImagePath = profileImagePath
            };
        }


        public async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            var identityUser = await _userManager.FindByNameAsync(userName);
            return identityUser?.ToApplicationUser();
        }

        public async Task<string> GetUserNameAsync(ApplicationUser user)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id);
            return identityUser?.UserName;
        }



        public async Task<IdentityResult> UpdateAsync(ApplicationUser user)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id);
            if (identityUser == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Пользователь не найден" });
            }

            identityUser.UserName = user.UserName;
            identityUser.Email = user.Email;
            identityUser.PhoneNumber = user.PhoneNumber;

            var result = await _userManager.UpdateAsync(identityUser);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                }
                return result;
            }

            if (user.ProfileImagePath != null)
            {
                var sql = @"UPDATE ""AspNetUsers"" SET ""ProfileImagePath"" = {0} WHERE ""Id"" = {1}";
                await _context.Database.ExecuteSqlRawAsync(sql, user.ProfileImagePath, user.Id);
            }

            return IdentityResult.Success;
        }




        public async Task<string> GetPhoneNumberAsync(ApplicationUser user)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id);
            return identityUser?.PhoneNumber;
        }

        public async Task<IdentityResult> SetPhoneNumberAsync(ApplicationUser user, string phoneNumber)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id);
            if (identityUser == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Пользователь не найден" });
            }

            return await _userManager.SetPhoneNumberAsync(identityUser, phoneNumber);
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id);
            if (identityUser == null)
            {
                return new List<string>();
            }

            return await _userManager.GetRolesAsync(identityUser);
        }
    }

    public static class UserConversionExtensions
    {
        public static ApplicationUser ToApplicationUser(this IdentityUser identityUser)
        {
            if (identityUser == null) return null;

            return new ApplicationUser
            {
                Id = identityUser.Id,
                UserName = identityUser.UserName,
                Email = identityUser.Email,
                PhoneNumber = identityUser.PhoneNumber,

                // Синхронизируйте другие свойства, которые вам нужны
            };
        }
    }




}
