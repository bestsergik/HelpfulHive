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
    }




}
