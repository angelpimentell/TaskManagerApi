using TaskManagerApi.Creators;

namespace TaskManagerApi.Factories
{
    public class TaskFactory
    {
        public const int HIGH_PRIORITY = 3;
        public const int MEDIUM_PRIORITY = 2;
        public const int LOW_PRIORITY = 1;

        public static TaskCreator CreateInstance(int type)
        {
            switch (type)
            {
                case HIGH_PRIORITY:
                    return new HighPriorityTaskCreator();
                case MEDIUM_PRIORITY:
                    return new MediumPriorityTaskCreator();
                case LOW_PRIORITY:
                    return new LowPriorityTaskCreator();
                default:
                    return new LowPriorityTaskCreator();
            }
        }
    }
}
