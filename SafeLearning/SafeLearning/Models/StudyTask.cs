namespace SafeLearning.Models
{
    // מטלה (טבלת Tasks)
    public class StudyTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public int SubjectId { get; set; }
        public DateTime DueDate { get; set; }
        public int Priority { get; set; }
        public bool IsDone { get; set; }
    }
}