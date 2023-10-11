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
            var rootTabs = await _tabService.GetRootTabsAsync();
            foreach (var tab in rootTabs)
            {
                Tabs.Add(tab);
            }
            OnTabAdded?.Invoke();
        }


        public async Task AddTab(TabItem newTab, TabItem? parentTab = null)
        {
            await _tabService.AddTabAsync(newTab, parentTab);
            if (parentTab == null)
            {
                Tabs.Add(newTab);
            }
            else
            {
                parentTab.SubTabs?.Add(newTab);
            }
            OnTabAdded?.Invoke();
        }

        public async Task<bool> DeleteTab(TabItem tab)
        {
            if (_tabService.CanDeleteTab(tab))
            {
                await _tabService.DeleteTabAsync(tab);
                Tabs.Remove(tab);
                OnTabAdded?.Invoke(); // Можете использовать другое событие для обновления UI
                return true; // Возвращает true, если удаление было успешным
            }
            else
            {
                // Логика или сообщение, которое информирует пользователя о том, что удаление не возможно
                return false; // Возвращает false, если удаление не было выполнено
            }
        }

        public async Task UpdateTab(TabItem tab)
        {
            await _tabService.UpdateTabAsync(tab);
            OnTabAdded?.Invoke(); // Можете использовать другое событие для обновления UI
        }
    }

}
