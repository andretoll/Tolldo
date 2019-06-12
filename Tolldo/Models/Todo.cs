using System.Collections.Generic;

namespace Tolldo.Models
{
    public class Todo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public int Order { get; set; }
        public bool Favorite { get; set; }

        public ICollection<TodoTask> Tasks { get; set; }
    }
}
