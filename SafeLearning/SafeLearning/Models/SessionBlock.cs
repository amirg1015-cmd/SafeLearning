namespace SafeLearning.Models
{
    // טבלת קישור: איזו תוכנה נסגרה, באיזה סשן, מתי וכמה פעמים
    public class SessionBlock
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public int BlockedAppId { get; set; }
        public DateTime BlockedAt { get; set; }
        public int CloseCount { get; set; }
    }
}