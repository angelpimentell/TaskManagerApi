using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using Threading = System.Threading.Tasks;
using TaskManagerApi.Factories;
using Task = TaskManagerApi.Models.Tasks.Task<string>;
using TaskManagerApi.Creators;
using System.Collections.Concurrent;
using System.Reactive.Subjects;

namespace TaskManagerApi.Controllers.Tasks
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        delegate IQueryable<Task> FilterTask(IQueryable<Task> query);

        Action<string> sendNotification = message => Console.WriteLine(message);

        private static readonly ConcurrentQueue<Task> CreateTaskQueue = new();

        private static readonly ConcurrentQueue<Task> UpdateTaskQueue = new();

        private static readonly ConcurrentQueue<Task> RemoveTaskQueue = new();

        private static readonly object QueueLock = new();

        private static Dictionary<DateTime, (int completed, int finished)> DailyStatsCache = new();


        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Tasks
        [HttpGet]
        public async Threading.Task<ActionResult<IEnumerable<Task>>> GetTasks(
            [FromQuery] DateTime? dueDate,
            [FromQuery] string? status
            )
        {

            FilterTask filterTaskByDueDate = query => query.Where(t => t.DueDate.Date == dueDate.Value.Date);
            FilterTask filtersStatus = query => query.Where(t => t.Status == status);

            var query = _context.Tasks.AsQueryable();

            if (dueDate.HasValue)
                query = filterTaskByDueDate(query);

            if (!string.IsNullOrEmpty(status))
                query = filtersStatus(query);

            var data = await query.ToListAsync();

            return Ok(new { data, success = true, message = "Successfully read!", statusCode = 200 });
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        public async Threading.Task<ActionResult<Task>> GetTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
                return NotFound(new { success = false, message = "Resource not found", statusCode = 404 });

            return Ok(new { data = task, success = true, message = "Successfully read!", statusCode = 200 });
        }

        // POST: api/Tasks
        [HttpPost]
        public async Threading.Task<IActionResult> PostTask(Task task)
        {
            TaskCreator taskCreator;

            if (task.RemainingDays <= 1)
            {
                taskCreator = Factories.TaskFactory.CreateInstance(Factories.TaskFactory.HIGH_PRIORITY);
            }
            else if (task.RemainingDays > 1 && task.RemainingDays <= 10)
            {
                taskCreator = Factories.TaskFactory.CreateInstance(Factories.TaskFactory.MEDIUM_PRIORITY);
            }
            else
            {
                taskCreator = Factories.TaskFactory.CreateInstance(Factories.TaskFactory.LOW_PRIORITY);
            }

            task = taskCreator.Create(task);

            CreateTaskQueue.Enqueue(task);

            lock (QueueLock)
            {
                while (CreateTaskQueue.TryDequeue(out var nextTask))
                {
                    _context.Tasks.Add(nextTask);
                }
            }

            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                data = task,
                success = true,
                message = "Successfully created!",
                statusCode = 201
            })
            { StatusCode = 201 };
        }

        // PUT: api/Tasks/5
        [HttpPut("{id}")]
        public async Threading.Task<IActionResult> PutTask(int id, Task task)
        {

            var existingTask = await _context.Tasks.FindAsync(id);

            if (existingTask == null)
            {
                return NotFound(new { success = false, message = "Task not found", statusCode = 404 });
            }


            UpdateTaskQueue.Enqueue(task);

            lock (QueueLock)
            {
                while (UpdateTaskQueue.TryDequeue(out var nextTask))
                {
                    existingTask.Name = task.Name;
                    existingTask.Description = task.Description;
                    existingTask.DueDate = task.DueDate;
                    existingTask.Status = task.Status;
                    existingTask.AdditionalData = task.AdditionalData;
                }
            }

            await _context.SaveChangesAsync();


            return new JsonResult(new
            {
                data = existingTask,
                success = true,
                message = "Successfully updated!",
                statusCode = 200
            })
            { StatusCode = 200 };
        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async Threading.Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound(new { success = false, message = "Task not found", statusCode = 404 });
            }

            RemoveTaskQueue.Enqueue(task);

            lock (QueueLock)
            {
                while (RemoveTaskQueue.TryDequeue(out var nextTask))
                {
                    _context.Tasks.Remove(nextTask);
                }
            }

            await _context.SaveChangesAsync();

            sendNotification("Task removed!");

            return new JsonResult(new
            {
                data = null as object,
                success = true,
                message = "Successfully removed!",
                statusCode = 200
            })
            { StatusCode = 200 };
        }

        // GET: api/Tasks
        [HttpGet("Count")]
        public async Threading.Task<ActionResult<IEnumerable<Task>>> GetTaskCount([FromQuery] DateTime? date = null)
        {

            var targetDate = date?.Date ?? DateTime.Now.Date;

            if (DailyStatsCache.ContainsKey(targetDate))
            {
                var cached = DailyStatsCache[targetDate];
                return Ok(new
                {
                    data = new
                    {
                        date = targetDate,
                        completedTasks = cached.completed,
                        finishedTasks = cached.finished,
                        total = cached.completed + cached.finished
                    },
                    success = true,
                    message = $"Cached stats retrieved for {targetDate:yyyy-MM-dd}",
                    statusCode = 200
                });
            }


            var completedCount = await _context.Tasks
                .Where(t => t.Status == "Completed" && t.DueDate.Date == targetDate)
                .CountAsync();

            var finishedCount = await _context.Tasks
                .Where(t => t.Status != "Completed" && t.DueDate.Date == targetDate)
                .CountAsync();


            DailyStatsCache[targetDate] = (completedCount, finishedCount);


            return Ok(new
            {
                data = new
                {
                    date = targetDate,
                    completedTasks = completedCount,
                    finishedTasks = finishedCount,
                    total = completedCount + finishedCount
                },
                success = true,
                message = $"Fresh stats calculated for {targetDate:yyyy-MM-dd}",
                statusCode = 200
            });
        }


    }
}
