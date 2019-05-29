namespace Tolldo.Models
{
    public class Subtask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Completed { get; set; }
        public int TodoTaskId { get; set; }
    }
}
