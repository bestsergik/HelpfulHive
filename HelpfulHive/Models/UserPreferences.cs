namespace HelpfulHive.Models
{
    public class UserPreferences
    {
        public int Id { get; set; } // Первичный ключ

        public string UserId { get; set; }
        public ApplicationUser User { get; set; } // Связь с пользователем

        public int RecordId { get; set; }
        public RecordModel Record { get; set; } // Связь с записью

        public bool IsFavorite { get; set; } // Запись добавлена в избранное пользователем

        public int ClickCount { get; set; } // Количество кликов по записи пользователем
    }

}
