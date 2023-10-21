using BlazorMonaco.Editor;

namespace HelpfulHive.Models
{
    public class XmlTab
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public StandaloneCodeEditor SendEditorRef { get; set; }
        public StandaloneCodeEditor CheckEditorRef { get; set; }
    }
}
