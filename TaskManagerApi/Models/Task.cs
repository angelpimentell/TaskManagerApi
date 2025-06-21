using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskManagerApi.Attributes;
using TaskManagerApi.Converters;

namespace TaskManagerApi.Models
{
    public class Task<T>

    {
        Func<DateTime, int> calculateRemainingDays = (dueDate) => Math.Max(0, (dueDate.Date - DateTime.Now.Date).Days);


        [Key]
        public int Id { get; set; }

        [JsonConverter(typeof(NameStrictStringConverter))]
        public required string Name { get; set; }

        [JsonConverter(typeof(DescriptionStrictStringConverter))]
        public required string Description { get; set; }

        [FutureDate]
        [JsonConverter(typeof(DueDateStrictDateConverter))]
        public required DateTime DueDate { get; set; }

        [JsonConverter(typeof(StatusStrictStringConverter))]
        public required string Status { get; set; }
        public T? AdditionalData { get; set; }

        public int RemainingDays => calculateRemainingDays(DueDate);
    }

}
