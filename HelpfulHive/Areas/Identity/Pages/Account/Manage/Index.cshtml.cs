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

namespace HelpfulHive.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _env;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment env) // Убедитесь, что этот параметр присутствует
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _env = env; // И инициализируйте _env здесь
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
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                // Предполагаем, что у вас есть свойство для хранения пути изображения в InputModel
                NewUsername = userName, // Если вы хотите загрузить и текущее имя пользователя
                SelectedProfileImagePath = user.ProfileImagePath // Добавьте это для загрузки изображения
            };
        }


        // Метод для получения доступных изображений
        private void LoadAvailableProfileImages()
        {
            var profileImagesFolder = Path.Combine(_env.WebRootPath,  "profileimg");
            AvailableProfileImages = Directory.GetFiles(profileImagesFolder)
                                             .Select(Path.GetFileName)
                                             .ToList();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            LoadAvailableProfileImages(); // Загрузка доступных изображений
            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Приведение типа user к ApplicationUser
            var appUser = user as ApplicationUser;
            if (appUser == null)
            {
                throw new InvalidOperationException("The user is not of type ApplicationUser.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // Остальная логика...
            if (Input.SelectedProfileImagePath != appUser.ProfileImagePath)
            {
                appUser.ProfileImagePath = Input.SelectedProfileImagePath;
                var updateResult = await _userManager.UpdateAsync(appUser);
                if (!updateResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set profile image.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(appUser);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }


        // Вам нужно добавить IWebHostEnvironment в конструктор, чтобы использовать _env.WebRootPath

    }

}
