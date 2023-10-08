using HelpfulHive.Models;
using HelpfulHive.Services;
using System.Collections.ObjectModel;

namespace HelpfulHive.ViewModels
{
    public class TabViewModel
    {
        private readonly TabService _tabService;
        public event Action? OnTabAdded;

        public ObservableCollection<TabItem> Tabs { get; set; } = new ObservableCollection<TabItem>();

        public TabViewModel(TabService tabService)
        {
            _tabService = tabService;
            LoadTabs();
        }

        public async Task LoadTabs()
        {
            var tabs = await _tabService.GetTabsAsync();
            foreach (var tab in tabs)
            {
                Tabs.Add(tab);
            }
            OnTabAdded?.Invoke(); // Вызывает событие после загрузки вкладок
        }


        public async Task AddTab(TabItem newTab)
        {
            await _tabService.AddTabAsync(newTab);
            Tabs.Add(newTab);
            OnTabAdded?.Invoke();
        }

        // Другие методы и логика по необходимости
    }

}
