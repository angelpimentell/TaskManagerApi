using TaskManagerApi.Attributes;

namespace TaskManagerApi.Creators
{
    public class HighPriorityTaskCreator : TaskCreator
    {
        public override Models.Tasks.Task<string> Create(Models.Tasks.Task<string> task)
        {
            return new Models.Tasks.Task<string>
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
