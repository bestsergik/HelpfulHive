using HelpfulHive.Models;
using HelpfulHive.Services;
using Microsoft.EntityFrameworkCore;

namespace HelpfulHive.ViewModels
{
    public class RecordViewModel
    {
        private readonly RecordService _recordService;
        public List<RecordModel> Records { get; private set; }


        public RecordViewModel(RecordService recordService)
        {
            _recordService = recordService ?? throw new ArgumentNullException(nameof(recordService));
        }

        public async Task AddRecordAsync(RecordModel newRecord)
        {
            await _recordService.AddRecordAsync(newRecord);
            // Можно добавить логику обновления UI или уведомления пользователя здесь
        }

        //public async Task LoadRecordsAsync(int subTabId)
        //{
        //    Records = await _recordService.GetRecordsBySubTabIdAsync(subTabId);
        //    // Уведомить подписчиков о том, что данные были обновлены (если вы используете такой подход)
        //}


    }

}
