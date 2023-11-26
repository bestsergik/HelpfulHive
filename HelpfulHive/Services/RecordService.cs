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
            }
            catch (Exception ex)
            {
            }
        }
        public async Task<List<RecordModel>> GetRecordsBySubTabUriAsync(string subTabUri, string userId)
        {
            using var context = _contextFactory.CreateDbContext();

            // Проверяем, принадлежит ли подвкладка текущему пользователю
            var subTab = await context.Tabs.FirstOrDefaultAsync(t => t.Uri == subTabUri);
            if (subTab == null || (subTab.TabType == TabType.Personal && subTab.UserId != userId))
            {
                // Если подвкладка не найдена или не принадлежит пользователю, возвращаем пустой список
                return new List<RecordModel>();
            }

            // Если всё в порядке, загружаем записи
            return await context.Records
                                .Include(r => r.Content)
                                .Where(r => r.SubTab.Uri == subTabUri)
                                .ToListAsync();
        }


        public async Task UpdateRecordAsync(RecordModel updatedRecord)
        {

            using var context = _contextFactory.CreateDbContext();

            context.Records.Update(updatedRecord);
            context.Entry(updatedRecord).State = EntityState.Modified;

            await context.SaveChangesAsync();
        }

        public async Task DeleteRecordAsync(RecordModel recordToDelete)
        {
            using var context = _contextFactory.CreateDbContext();

            var existingRecord = await context.Records
                                              .Include(r => r.Content)
                                              .FirstOrDefaultAsync(r => r.Id == recordToDelete.Id);

            if (existingRecord != null)
            {
                try
                {
                    if (existingRecord.Content != null && existingRecord.Content.ImageUrls != null)
                    {
                        foreach (var imageUrl in existingRecord.Content.ImageUrls)
                        {
                            await DeleteImageFileAsync(imageUrl);
                        }
                    }

                    context.RecordsContent.Remove(existingRecord.Content);
                    context.Records.Remove(existingRecord);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Логирование исключения
                    // Дополнительная логика обработки исключения
                    throw;
                }
            }
        }

        private async Task DeleteImageFileAsync(string imageUrl)
        {
            // Предполагаем, что imageUrl - это относительный путь, например, "content_images/74996892-6428-4824-b553-bde3bb20653a.png"
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imageUrl);

            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    // Логирование ошибки
                }
            }
            else
            {
                // Логирование: Файл не найден
            }

            await Task.CompletedTask;
        }

        public async Task<List<RecordModel>> GetFavoriteRecordsByUserAsync(string userId)
        {
            using var context = _contextFactory.CreateDbContext();
            var records = await context.UserPreferences
                .Where(up => up.UserId == userId && up.IsFavorite)
                .Include(up => up.Record)
                    .ThenInclude(record => record.Content) // Включаем загрузку контента
                .ToListAsync();
            return records.Select(up => up.Record).ToList();
        }




        public async Task<List<RecordModel>> SearchRecordsAsync(string query, bool isSearchAll, string userId, string selectedSubTabId)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var records = context.Records
                    .Include(r => r.SubTab)
                    .ThenInclude(st => st.ParentTab)
                    .Include(r => r.Content)
                    .AsNoTracking()
                    .AsQueryable();

                Console.WriteLine($"Before filters - Count: {records.Count()}");

                // Фильтр по строке запроса
                if (!string.IsNullOrWhiteSpace(query))
                {
                    query = query.ToLower();
                    records = records
                        .Where(r =>
                            EF.Functions.Like(r.Title.ToLower(), $"%{query}%") || EF.Functions.Like(r.Content.Text.ToLower(), $"%{query}%"));
                }

                // Дополнительные фильтры для учета типа вкладки и владельца
                if (!isSearchAll)
                {
                    records = records
                        .Where(r => (r.SubTab.UserId == userId && r.SubTab.TabType == TabType.Personal) || r.SubTab.TabType == TabType.Common);
                }

                var result = await records.ToListAsync();
                Console.WriteLine($"After filters - Count: {result.Count}");

                foreach (var record in result)
                {
                    Console.WriteLine($"Record Id: {record.Id}, TabType: {record.SubTab.TabType}, UserId: {record.SubTab.UserId}");
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SearchRecordsAsync: {ex.Message}");
                return new List<RecordModel>();
            }
        }










        public async Task<List<RecordModel>> GetRecordsByUserAndTabTypeAsync(bool isSearchAll, string userId)
        {
            using var context = _contextFactory.CreateDbContext();
            var records = context.Records
                .Include(r => r.SubTab)
                .ThenInclude(st => st.ParentTab)
                .AsNoTracking()
                .AsQueryable();
                records = records.Where(r => r.SubTab.UserId == userId || r.SubTab.TabType == TabType.Common);

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
