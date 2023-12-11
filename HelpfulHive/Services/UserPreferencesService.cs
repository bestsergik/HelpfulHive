using HelpfulHive.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpfulHive.Services
{
    public class UserPreferencesService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public UserPreferencesService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task UpdateOrCreateUserPreference(string userId, int recordId)
        {
            using var context = _contextFactory.CreateDbContext();
            var preference = await context.UserPreferences
                .FirstOrDefaultAsync(up => up.UserId == userId && up.RecordId == recordId);

            if (preference == null)
            {
                var newUserPreference = new UserPreferences
                {
                    UserId = userId,
                    RecordId = recordId,
                    ClickCount = 1
                };
                context.UserPreferences.Add(newUserPreference);
            }
            else
            {
                preference.ClickCount++;
            }

            await context.SaveChangesAsync();
        }

        public async Task<List<RecordModel>> GetTopNClickedRecordsAsync(int n, string userId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.UserPreferences
                .Where(up => up.UserId == userId)
                .OrderByDescending(up => up.ClickCount)
                .Take(n)
                .Select(up => up.Record)
                .ToListAsync();
        }


        public async Task<bool> IsFavoriteAsync(string userId, int recordId)
        {
            using var context = _contextFactory.CreateDbContext();
            var preference = await context.UserPreferences
                .FirstOrDefaultAsync(up => up.UserId == userId && up.RecordId == recordId);

            return preference?.IsFavorite ?? false;
        }

        public async Task ToggleFavoriteAsync(string userId, int recordId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("UserId cannot be null or empty");
            }

            using var context = _contextFactory.CreateDbContext();
            var userExists = await context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                throw new InvalidOperationException($"User with Id {userId} does not exist");
            }
            var preference = await context.UserPreferences
                .FirstOrDefaultAsync(up => up.UserId == userId && up.RecordId == recordId);

            if (preference == null)
            {
                var newUserPreference = new UserPreferences
                {
                    UserId = userId,
                    RecordId = recordId,
                    IsFavorite = true,
                    ClickCount = 0
                };
                context.UserPreferences.Add(newUserPreference);
            }
            else
            {
                preference.IsFavorite = !preference.IsFavorite;
            }

            await context.SaveChangesAsync();
        }


        public async Task MarkAllRecordsAsViewed(string userId)
        {
            using var context = _contextFactory.CreateDbContext();

            // Получение всех общих записей
            var commonRecords = await context.Records
                                             .Where(r => r.SubTab.TabType == TabType.Common)
                                             .ToListAsync();

            // Получение всех личных записей для текущего пользователя
            var personalRecords = await context.Records
                                               .Where(r => r.SubTab.TabType == TabType.Personal && r.SubTab.UserId == userId)
                                               .ToListAsync();

            // Объединение списков записей
            var allRelevantRecords = commonRecords.Concat(personalRecords);

            foreach (var record in allRelevantRecords)
            {
                var preference = await context.UserPreferences
                    .FirstOrDefaultAsync(up => up.UserId == userId && up.RecordId == record.Id);

                if (preference == null)
                {
                    // Создание новой записи в UserPreferences
                    var newUserPreference = new UserPreferences
                    {
                        UserId = userId,
                        RecordId = record.Id,
                        HasViewedNewCommonRecord = true,
                        ClickCount = 0,
                        IsFavorite = false
                    };
                    context.UserPreferences.Add(newUserPreference);
                }
                else
                {
                    // Обновление существующей записи
                    preference.HasViewedNewCommonRecord = true;
                }
            }

            await context.SaveChangesAsync();
        }




        public async Task<List<UserPreferences>> GetUserPreferences(string userId)
        {
            using var context = _contextFactory.CreateDbContext();
            var preferences = await context.UserPreferences
                                           .Where(up => up.UserId == userId)
                                           .ToListAsync();

       

            return preferences;
        }


        public async Task<Dictionary<int, int>> GetUnviewedRecordsCountByTab(string userId)
        {
            using var context = _contextFactory.CreateDbContext();
            var unviewedCountByTab = new Dictionary<int, int>();

            var unviewedPreferences = await context.UserPreferences
                .Include(up => up.Record)
                .ThenInclude(r => r.SubTab)
                .Where(up => up.UserId == userId && !up.HasViewedNewCommonRecord)
                .ToListAsync();

            foreach (var pref in unviewedPreferences)
            {
                if (pref.Record != null && pref.Record.SubTab != null)
                {
                    // Увеличиваем счетчик для подвкладки
                    int subTabId = pref.Record.SubTabId;
                    unviewedCountByTab[subTabId] = unviewedCountByTab.GetValueOrDefault(subTabId) + 1;

                    // Увеличиваем счетчик для родительской вкладки, если она есть
                    int? parentTabId = pref.Record.SubTab.ParentTabId;
                    if (parentTabId.HasValue)
                    {
                        unviewedCountByTab[parentTabId.Value] = unviewedCountByTab.GetValueOrDefault(parentTabId.Value) + 1;
                    }
                }
            }

            return unviewedCountByTab;
        }





        public async Task MarkAsViewed(string userId, int recordId)
        {
            using var context = _contextFactory.CreateDbContext();
            var preference = await context.UserPreferences
                .FirstOrDefaultAsync(up => up.UserId == userId && up.RecordId == recordId);

            if (preference == null)
            {
                // Создание новой записи в UserPreferences, если она не найдена
                var newUserPreference = new UserPreferences
                {
                    UserId = userId,
                    RecordId = recordId,
                    HasViewedNewCommonRecord = true, // Установка флага просмотра
                    ClickCount = 0, // Инициализация других полей по умолчанию
                    IsFavorite = false
                };
                context.UserPreferences.Add(newUserPreference);
            }
            else
            {
                // Обновление существующей записи
                preference.HasViewedNewCommonRecord = true;
            }

            await context.SaveChangesAsync();
        }

    }




}
