using System.ComponentModel.DataAnnotations;

namespace TaskManagerApi.Models.Tasks
{

    public class Task<T>

    {
        [Key]
        public int Id { get; set; } 
        public required string Name { get; set; }
        public required string Description { get; set; }
        public DateTime DueDate { get; set; }
        public required string Status { get; set; } 
        public T? AdditionalData { get; set; } 
    }

}
