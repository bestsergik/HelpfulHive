using HelpfulHive.Models;

namespace HelpfulHive.Services
{
    public class RecordService
    {
        private readonly ApplicationDbContext _dbContext;

        public RecordService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddRecordAsync(RecordModel newRecord)
        {
            _dbContext.Records.Add(newRecord);
            await _dbContext.SaveChangesAsync();
        }
    }

}
