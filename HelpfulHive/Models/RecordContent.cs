namespace HelpfulHive.Models
{
    public class RecordContent
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
    }


}
