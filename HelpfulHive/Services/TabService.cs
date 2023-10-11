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

        public async Task<IEnumerable<TabItem>> GetRootTabsAsync()
        {
            return await _dbContext.Tabs
                .Include(t => t.SubTabs)  // Чтобы загрузить подвкладки
                .Where(t => t.ParentTabId == null)
                .ToListAsync();
        }

        public async Task<IEnumerable<TabItem>> GetTabsAsync()
        {
            return await _dbContext.Tabs.ToListAsync();
        }

        public async Task AddTabAsync(TabItem newTab, TabItem? parentTab = null)
        {
            if (parentTab != null)
            {
                newTab.ParentTabId = parentTab.Id;
            }

            // Транслитерация названия вкладки и замена пробелов дефисами
            newTab.Uri = Transliterate(newTab.Name?.Replace(" ", "-") ?? "");

            // Убедитесь, что URI уникален, добавив суффикс при необходимости
            newTab.Uri = await EnsureUniqueUri(newTab.Uri);

            _dbContext.Tabs.Add(newTab);
            await _dbContext.SaveChangesAsync();
        }

        public static string Transliterate(string str)
        {
            string[] rus = "А,Б,В,Г,Д,Е,Ё,Ж,З,И,Й,К,Л,М,Н,О,П,Р,С,Т,У,Ф,Х,Ц,Ч,Ш,Щ,Ъ,Ы,Ь,Э,Ю,Я,а,б,в,г,д,е,ё,ж,з,и,й,к,л,м,н,о,п,р,с,т,у,ф,х,ц,ч,ш,щ,ъ,ы,ь,э,ю,я".Split(',');
            string[] eng = "A,B,V,G,D,E,E,Zh,Z,I,Y,K,L,M,N,O,P,R,S,T,U,F,H,C,Ch,Sh,Sch,,I,,E,Yu,Ya,a,b,v,g,d,e,e,zh,z,i,y,k,l,m,n,o,p,r,s,t,u,f,h,c,ch,sh,sch,,i,,e,yu,ya".Split(',');

            for (int i = 0; i < rus.Length; i++)
            {
                str = str.Replace(rus[i], eng[i]);
            }

            return str;
        }


        private async Task<string> EnsureUniqueUri(string baseUri)
        {
            string uri = baseUri;
            int counter = 1;

            // Проверка существования URI в БД
            while (await UriExists(uri))
            {
                uri = $"{baseUri}-{counter}";
                counter++;
            }

            return uri;
        }


        private async Task<bool> UriExists(string uri)
        {
            return await _dbContext.Tabs.AnyAsync(t => t.Uri == uri);
        }

        public async Task DeleteTabAsync(TabItem tab)
        {
            _dbContext.Tabs.Remove(tab);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateTabAsync(TabItem tab)
        {
            _dbContext.Tabs.Update(tab);
            await _dbContext.SaveChangesAsync();
        }

        public bool CanDeleteTab(TabItem tab)
        {
            return !_dbContext.Tabs.Any(t => t.ParentTabId == tab.Id);
        }

    }

}
