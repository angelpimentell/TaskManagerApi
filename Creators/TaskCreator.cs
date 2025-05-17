namespace TaskManagerApi.Creators
{
    public abstract class TaskCreator
    {
        public abstract Models.Tasks.Task<string> Create(Models.Tasks.Task<string> task);
    }
}
