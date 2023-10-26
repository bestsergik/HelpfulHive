using HelpfulHive.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpfulHive.Services
{
    public class RecordService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public RecordService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task AddRecordAsync(RecordModel newRecord)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Records.Add(newRecord);
            await context.SaveChangesAsync();
        }

        public async Task<List<RecordModel>> GetRecordsBySubTabUriAsync(string subTabUri)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Records
                .Include(r => r.SubTab)
                .Where(r => r.SubTab.Uri == subTabUri)
                .ToListAsync();
        }

        public async Task UpdateRecordAsync(RecordModel updatedRecord)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Records.Update(updatedRecord);
            await context.SaveChangesAsync();
        }

        public async Task DeleteRecordAsync(RecordModel recordToDelete)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Records.Remove(recordToDelete);
            await context.SaveChangesAsync();
        }

        public async Task<List<RecordModel>> GetTopNClickedRecordsAsync(int n)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Records
                .AsNoTracking()
                .OrderByDescending(r => r.ClickCount)
                .Take(n)
                .ToListAsync();
        }
    }


}
