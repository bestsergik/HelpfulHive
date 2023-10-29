using HelpfulHive.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpfulHive.Services
{
    public class UserPreferencesService
    {
        private readonly ApplicationDbContext _context;

        public UserPreferencesService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task UpdateOrCreateUserPreference(string userId, int recordId)
        {
            var preference = await _context.UserPreferences
                .FirstOrDefaultAsync(up => up.UserId == userId && up.RecordId == recordId);

            if (preference == null)
            {
                var newUserPreference = new UserPreferences
                {
                    UserId = userId,
                    RecordId = recordId,
                    ClickCount = 1
                };
                _context.UserPreferences.Add(newUserPreference);
            }
            else
            {
                preference.ClickCount++;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<RecordModel>> GetTopNClickedRecordsAsync(int n, string userId)
        {
            return await _context.UserPreferences
                .Where(up => up.UserId == userId)
                .OrderByDescending(up => up.ClickCount)
                .Take(n)
                .Select(up => up.Record)
                .ToListAsync();
        }
    }



}
