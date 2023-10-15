namespace HelpfulHive.Models
{
    public class TabItem
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Uri { get; set; } 
        public int? ParentTabId { get; set; } // NULL для корневых вкладок
        public TabItem? ParentTab { get; set; } // Навигационное свойство
        public ICollection<TabItem>? SubTabs { get; set; } // Подвкладки

        public string? UserId { get; set; } // Cвойство для связи с пользователем
        public ICollection<RecordModel> Records { get; set; }
    }
}
