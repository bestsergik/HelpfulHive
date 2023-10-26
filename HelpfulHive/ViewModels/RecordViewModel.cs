using HelpfulHive.Models;
using HelpfulHive.Services;
using Microsoft.EntityFrameworkCore;

namespace HelpfulHive.ViewModels
{
    public class RecordViewModel
    {
        private readonly RecordService _recordService;
        public List<RecordModel> Records { get; private set; } 

        public event Action? OnRecordChanged;

        public RecordViewModel(RecordService recordService)
        {
            _recordService = recordService ?? throw new ArgumentNullException(nameof(recordService));
        }

        public async Task AddRecordAsync(RecordModel newRecord)
        {
            await _recordService.AddRecordAsync(newRecord);
            if (Records == null)
            {
                Records = new List<RecordModel>();
            }
            Records.Add(newRecord);
            OnRecordChanged?.Invoke();
        }

        public async Task HandleClick(RecordModel record)
{
    record.ClickCount++;
    await UpdateRecordAsync(record);
    OnRecordChanged?.Invoke();
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
            return await _recordService.GetTopNClickedRecordsAsync(n);
        }

    }

}
