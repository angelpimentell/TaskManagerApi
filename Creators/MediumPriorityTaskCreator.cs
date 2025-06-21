namespace TaskManagerApi.Creators
{
    public class MediumPriorityTaskCreator : TaskCreator
    {
        public override Models.Task<string> Create(Models.Task<string> task)
        {
            return new Models.Task<string>
            {
                Name = task.Name,
                Description = task.Description,
                DueDate = task.DueDate,
                Status = task.Status,
                AdditionalData = "Medium Priority",
            };
        }
    }
}
