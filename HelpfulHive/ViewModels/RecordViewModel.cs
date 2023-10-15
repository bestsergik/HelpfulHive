using HelpfulHive.Models;
using HelpfulHive.Services;

namespace HelpfulHive.ViewModels
{
    public class RecordViewModel
    {
        private readonly RecordService _recordService;

        public RecordViewModel(RecordService recordService)
        {
            _recordService = recordService ?? throw new ArgumentNullException(nameof(recordService));
        }

        public async Task AddRecordAsync(RecordModel newRecord)
        {
            await _recordService.AddRecordAsync(newRecord);
            // Можно добавить логику обновления UI или уведомления пользователя здесь
        }

        //public async Task<IEnumerable<RecordModel>> GetRecordsBySubTabIdAsync(int subTabId)
        //{
        //    try
        //    {
        //        await Task.Delay(1000);
        //        Console.WriteLine($"Fetching records for subtab {subTabId}");
        //        return new List<RecordModel>
        //{
        //    new RecordModel { Title = $"Record for subtab {subTabId}", ImagePath = "icon1" }
        //};
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error fetching records: {ex.Message}");
        //        return Enumerable.Empty<RecordModel>();
        //    }
        //}

    }

}
