using HelpfulHive.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpfulHive.Services
{
    public class TabService
    {
        private readonly ApplicationDbContext _dbContext;

        public TabService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<TabItem>> GetTabsAsync()
        {
            return await _dbContext.Tabs.ToListAsync();
        }

        public async Task AddTabAsync(TabItem tab)
        {
            _dbContext.Tabs.Add(tab);
            await _dbContext.SaveChangesAsync();
        }

        // Другие методы для удаления, обновления и т.д.
    }

}
