namespace HelpfulHive.Models
{
    public class RawData
    {
        public int RawDataId { get; set; } // Уникальный идентификатор
        public string RequestNumber { get; set; } // Номер заявки
        public string Comments { get; set; } // Комментарии
        public string Subject { get; set; } // Тема
        public string Inquiry { get; set; } // Запрос
        public string Response { get; set; } // Ответ
        public string Link { get; set; } // Ответ
        public bool IsObsolete { get; set; } // Поле для отметки, что запись устарела
        public int? Useful { get; set; }

    }
}
