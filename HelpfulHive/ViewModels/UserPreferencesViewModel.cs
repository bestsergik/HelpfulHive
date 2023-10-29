using HelpfulHive.Models;
using HelpfulHive.Services;

namespace HelpfulHive.ViewModels
{
    public class UserPreferencesViewModel
    {
        private readonly UserPreferencesService _userPreferencesService;

        public UserPreferencesViewModel(UserPreferencesService userPreferencesService)
        {
            _userPreferencesService = userPreferencesService ?? throw new ArgumentNullException(nameof(userPreferencesService));
        }

        public async Task UpdateOrCreateUserPreference(string userId, int recordId)
        {
            await _userPreferencesService.UpdateOrCreateUserPreference(userId, recordId);
        }

        public async Task<List<RecordModel>> GetTopNClickedRecordsAsync(int n, string userId)
        {
            return await _userPreferencesService.GetTopNClickedRecordsAsync(n, userId);
        }
    }
}
