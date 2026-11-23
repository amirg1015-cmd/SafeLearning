namespace SafeLearning.Models
{
    // סשן לימוד
    public class Session
    {
        public int Id { get; set; }
        public int? TaskId { get; set; }
        public int SubjectId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int DurationMinutes { get; set; }
        public int BlockedAttempts { get; set; }
    }
}