using TaskManagerApi.Attributes;

namespace TaskManagerApi.Creators
{
    public class HighPriorityTaskCreator : TaskCreator
    {
        public override Models.Task<string> Create(Models.Task<string> task)
        {
            return new Models.Task<string>
            {
                Name = task.Name,
                Description = task.Description,
                DueDate = task.DueDate,
                Status = task.Status,
                AdditionalData = "High Priority",
            };
        }
    }
}
