using System.ComponentModel.DataAnnotations;

namespace HelpfulHive.Models
{
    public class RecordModel
    {
        public int Id { get; set; } // Первичный ключ
        public string Title { get; set; }
        public string Content { get; set; }
        public int SubTabId { get; set; }
        public TabItem SubTab { get; set; } // Навигационное свойство
        public string ImagePath { get; set; }

    }
}
