namespace SafeLearning.Models
{
    // תוכנה חסומה
    public class BlockedApp
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string ProcessName { get; set; } = "";
    }
}