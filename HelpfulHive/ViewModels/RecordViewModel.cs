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
    }

}
