using System.ComponentModel.DataAnnotations;

namespace HelpfulHive.Models
{
    public class RecordModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ContentId { get; set; } // Внешний ключ для связи с RecordContent
        public RecordContent Content { get; set; } // Навигационное свойство
        public int SubTabId { get; set; }
        public TabItem SubTab { get; set; }
        public string ImagePath { get; set; }

    }
}
