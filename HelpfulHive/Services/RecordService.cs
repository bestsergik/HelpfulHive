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


        public async Task<List<RecordModel>> SearchRecordsAsync(string query, bool isSearchAll, string userId)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return await GetRecordsByUserAndTabTypeAsync(isSearchAll, userId);
            }

            using var context = _contextFactory.CreateDbContext();
            var records = context.Records
                .Include(r => r.SubTab)
                .ThenInclude(st => st.ParentTab)
                .AsNoTracking()  // Добавляем AsNoTracking()
                .AsQueryable();

            if (!isSearchAll)
            {
                records = records.Where(r => r.SubTab.UserId == userId && r.SubTab.TabType == TabType.Personal);
            }
            else
            {
                records = records.Where(r => r.SubTab.TabType == TabType.Common || (r.SubTab.TabType == TabType.Personal && r.SubTab.UserId == userId));
            }

            var queryToExecute = records
                .Where(r => EF.Functions.Like(r.Title, $"%{query}%") || EF.Functions.Like(r.Content, $"%{query}%"));

            var sql = queryToExecute.ToQueryString();  // Выводим SQL-запрос

            var filteredRecords = await queryToExecute.ToListAsync();


            return filteredRecords;
        }

        public async Task<List<RecordModel>> GetRecordsByUserAndTabTypeAsync(bool isSearchAll, string userId)
        {
            using var context = _contextFactory.CreateDbContext();
            var records = context.Records
                .Include(r => r.SubTab)
                .ThenInclude(st => st.ParentTab)
                .AsNoTracking()
                .AsQueryable();

            if (!isSearchAll)
            {
                records = records.Where(r => r.SubTab.UserId == userId && r.SubTab.TabType == TabType.Personal);
            }
            else
            {
                records = records.Where(r => r.SubTab.TabType == TabType.Common || (r.SubTab.TabType == TabType.Personal && r.SubTab.UserId == userId));
            }

            return await records.ToListAsync();
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
