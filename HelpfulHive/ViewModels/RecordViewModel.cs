using HelpfulHive.Models;
using HelpfulHive.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HelpfulHive.ViewModels
{
    public class RecordViewModel
    {
        private readonly RecordService _recordService;
        public List<RecordModel> Records { get; private set; }




        private readonly AuthenticationStateProvider _authenticationStateProvider;
        public string UserId { get; private set; }

        public event Action? OnRecordChanged;
        private readonly UserPreferencesViewModel _userPreferencesVM;

        public RecordViewModel(
        RecordService recordService,
        AuthenticationStateProvider authenticationStateProvider,
        UserPreferencesViewModel userPreferencesVM)
        {
            _userPreferencesVM = userPreferencesVM ?? throw new ArgumentNullException(nameof(userPreferencesVM));

            _recordService = recordService ?? throw new ArgumentNullException(nameof(recordService));
            _authenticationStateProvider = authenticationStateProvider ?? throw new ArgumentNullException(nameof(authenticationStateProvider));
            InitializeUserId();
        }

        private async Task InitializeUserId()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            UserId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Вы можете здесь вызвать метод загрузки записей или другие методы, которые зависят от UserId
        }


        public async Task<List<RecordModel>> GetRecordsBySubTabUriAsync(string subTabUri)
        {
            return await _recordService.GetRecordsBySubTabUriAsync(subTabUri, UserId);
        }

        public async Task AddRecordAsync(RecordModel newRecord, int subTabId)
        {
            await _recordService.AddRecordAsync(newRecord, subTabId);
            if (Records == null)
            {
                Records = new List<RecordModel>();
            }
            Records.Add(newRecord);
            OnRecordChanged?.Invoke();
        }


        public async Task HandleClick(RecordModel record)
        {
            await _userPreferencesVM.UpdateOrCreateUserPreference(UserId, record.Id);
            await UpdateRecordAsync(record);
            OnRecordChanged?.Invoke();
        }
      public async Task<List<RecordModel>> SearchRecordsAsync(string query, bool isSearchAll, string selectedSubTabId)
    {
        return await _recordService.SearchRecordsAsync(query, isSearchAll, UserId, selectedSubTabId);
    }



        public async Task UpdateRecordAsync(RecordModel updatedRecord)
        {
            await _recordService.UpdateRecordAsync(updatedRecord);

            if (Records != null)
            {
                var index = Records.FindIndex(r => r.Id == updatedRecord.Id);
                if (index != -1)
                {
                    Records[index] = updatedRecord;
                }
            }
            OnRecordChanged?.Invoke();
        }

        public async Task DeleteRecordAsync(RecordModel recordToDelete)
        {
            await _recordService.DeleteRecordAsync(recordToDelete);

            if (Records != null)
            {
                Records.Remove(recordToDelete);
            }

            OnRecordChanged?.Invoke();
        }

        public async Task<List<RecordModel>> GetTopNClickedRecordsAsync(int n)
        {
            return await _recordService.GetTopNClickedRecordsAsync(n, UserId);
        }


        public async Task RecordViewed(RecordModel record)
        {
            await _userPreferencesVM.MarkAsViewed(UserId, record.Id);
            // Обновите любые нужные данные или вызовите OnRecordChanged, если это необходимо
        }


      


    }

}