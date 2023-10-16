using HelpfulHive.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<RecordModel>> GetRecordsBySubTabUriAsync(string subTabUri)
        {
            return await _dbContext.Records
                .Include(r => r.SubTab)
                .Where(r => r.SubTab.Uri == subTabUri)
                .ToListAsync();
        }


        public async Task UpdateRecordAsync(RecordModel updatedRecord)
        {
            _dbContext.Records.Update(updatedRecord);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteRecordAsync(RecordModel recordToDelete)
        {
            _dbContext.Records.Remove(recordToDelete);
            await _dbContext.SaveChangesAsync();
        }

    }

}
