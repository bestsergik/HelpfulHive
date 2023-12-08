namespace HelpfulHive.Models
{
    public class ProcessedData
    {
        public int ProcessedDataId { get; set; } // Уникальный идентификатор
        public int RawDataId { get; set; } // Ссылка на исходную запись в RawData
        public string ProcessedComments { get; set; } // Обработанные комментарии
        public string ProcessedSubject { get; set; } // Обработанная тема
        public string ProcessedInquiry { get; set; } // Обработанный запрос
        public string ProcessedResponse { get; set; } // Обработанный ответ

        // Новые поля для лайков и дизлайков
        public int LikesCount { get; set; } // Количество лайков
        public int DislikesCount { get; set; } // Количество дизлайков

        // Навигационное свойство для связи с RawData
        public RawData RawData { get; set; }
    }


}
