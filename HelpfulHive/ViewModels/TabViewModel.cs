using HelpfulHive.Models;
using HelpfulHive.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Collections.ObjectModel;
using System.Data;
using System.Security.Claims;

namespace HelpfulHive.ViewModels
{
    public class TabViewModel
    {
        private readonly TabService _tabService;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        public event Action? OnTabAdded;
      

        public string UserId { get; set; }
        public TabType ActiveTab { get; set; } // Добавляем переменную для текущего активного типа вкладки

        public ObservableCollection<TabItem> Tabs { get; set; } = new ObservableCollection<TabItem>();

        public TabViewModel(TabService tabService, AuthenticationStateProvider authenticationStateProvider)
        {
            _tabService = tabService;
            _authenticationStateProvider = authenticationStateProvider;
            InitializeUserIdAndLoadTabs();
        }

        private async Task InitializeUserIdAndLoadTabs()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            UserId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(UserId))
            {
                ActiveTab = TabType.Personal;
                await LoadTabs(ActiveTab);

            }
            else
            {
                // Handle the case when user is not authenticated or UserId could not be determined
            }
        }

        public async Task LoadTabs(TabType tabType)
        {
            Tabs.Clear();  // Очистите коллекцию перед добавлением новых вкладок.
            var rootTabs = await _tabService.GetRootTabsAsync(UserId, tabType);
            foreach (var tab in rootTabs)
            {
                Tabs.Add(tab);
            }
            OnTabAdded?.Invoke();
        }


        public async Task AddTab(TabItem newTab, TabItem? parentTab = null)
        {
            await _tabService.AddTabAsync(newTab, UserId, parentTab);
            if (parentTab == null)
            {
                if ((newTab.TabType == ActiveTab && newTab.UserId == UserId) ||
                    (ActiveTab == TabType.Common && newTab.UserId == null))
                {
                    Tabs.Add(newTab);
                }
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
                
                return false; 
            }
        }

        public async Task UpdateTab(TabItem tab)
        {
            await _tabService.UpdateTabAsync(tab);
            OnTabAdded?.Invoke(); // Можете использовать другое событие для обновления UI
        }
    }

}
