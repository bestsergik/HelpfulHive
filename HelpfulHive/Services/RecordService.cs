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
            try
            {
                context.Records.Add(newRecord);
                await context.SaveChangesAsync();
                Console.WriteLine("Record added successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding record: " + ex.ToString());
            }
        }
        public async Task<List<RecordModel>> GetRecordsBySubTabUriAsync(string subTabUri)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Records
                .Include(r => r.Content) // Добавляем подгрузку связанного свойства Content
                .Include(r => r.SubTab)  // Если нужно, подгружаем и другие связанные свойства
                .Where(r => r.SubTab.Uri == subTabUri)
                .ToListAsync();
        }


        public async Task UpdateRecordAsync(RecordModel updatedRecord)
        {
            await Console.Out.WriteLineAsync("вкладка изменнена на " + "----------" + updatedRecord.SubTabId);     
            await Console.Out.WriteLineAsync("название записи изменнено на " + "----------" + updatedRecord.Title);

            using var context = _contextFactory.CreateDbContext();
            context.Entry(updatedRecord).State = EntityState.Modified;

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
     .Where(r => EF.Functions.Like(r.Title, $"%{query}%") || EF.Functions.Like(r.Content.Text, $"%{query}%"));

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


        public async Task<List<RecordModel>> GetTopNClickedRecordsAsync(int n, string userId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.UserPreferences
                .Where(up => up.UserId == userId)
                .OrderByDescending(up => up.ClickCount)
                .Take(n)
                .Select(up => up.Record)
                .ToListAsync();
        }
    }


}
