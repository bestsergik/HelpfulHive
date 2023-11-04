using Microsoft.EntityFrameworkCore;

namespace HelpfulHive.Services
{
    public class UserService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly ILogger<UserService> _logger; // Добавляем логгер

        public UserService(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<UserService> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<string> GetUserProfileImagePath(string userName)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var user = await context.Users
                    .Where(u => u.UserName == userName)
                    .Select(u => new { u.Id, ProfileImagePath = EF.Property<string>(u, "ProfileImagePath") })
                    .FirstOrDefaultAsync();

                if (user != null && !string.IsNullOrEmpty(user.ProfileImagePath))
                {
                    _logger.LogInformation($"Найден пользователь: {userName} с путем к изображению.");
                    return "/profileimg/" + user.ProfileImagePath;
                }
                else
                {
                    _logger.LogInformation(user != null
                        ? $"Путь к изображению профиля для пользователя {userName} не задан, используется изображение по умолчанию."
                        : $"Пользователь {userName} не найден.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при попытке получить путь к изображению профиля пользователя {userName}.");
            }

            return "/default-profile.png";
        }

    }


}
