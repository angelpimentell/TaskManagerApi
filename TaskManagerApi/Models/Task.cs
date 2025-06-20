using System.ComponentModel.DataAnnotations;
using TaskManagerApi.Attributes;

namespace TaskManagerApi.Models
{

    public class Task<T>

    {
        Func<DateTime, int> calculateRemainingDays = (dueDate) => Math.Max(0, (dueDate.Date - DateTime.Now.Date).Days);

        [Key]
        public int Id { get; set; } 
        public required string Name { get; set; }
        public required string Description { get; set; }

        [FutureDate]
        public DateTime DueDate { get; set; }
        public required string Status { get; set; } 
        public T? AdditionalData { get; set; } 

        public int RemainingDays => calculateRemainingDays(DueDate);
    }

}
