namespace TaskManagerApi.Creators
{
    public abstract class TaskCreator
    {
        public abstract Models.Task<string> Create(Models.Task<string> task);
    }
}
