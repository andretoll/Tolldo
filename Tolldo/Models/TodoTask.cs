using System.Collections.Generic;

namespace Tolldo.Models
{
    public class TodoTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Completed { get; set; }
        public int Order { get; set; }

        public ICollection<Subtask> Subtasks { get; set; }
        public int TodoId { get; set; }
    }
}
