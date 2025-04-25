using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using Task = TaskManagerApi.Models.Tasks.Task<string>;
using Threading = System.Threading.Tasks;

namespace TaskManagerApi.Controllers.Tasks
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

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
            var query = _context.Tasks.AsQueryable();

            if (dueDate.HasValue)
            {
                query = query.Where(t => t.DueDate.Date == dueDate.Value.Date);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status == status);
            }

            var data = await query.ToListAsync();

            return new JsonResult(new
            {
                data = data,
                success = true,
                message = "Successfully readed!",
                statusCode = 200
            })
            { StatusCode = 200 };
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        public async Threading.Task<ActionResult<Task>> GetTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                throw new Exception("Resource not found");
            }


            return new JsonResult(new
            {
                data = task,
                success = true,
                message = "Successfully readed!",
                statusCode = 200
            })
            { StatusCode = 200 };
        }

        // POST: api/Tasks
        [HttpPost]
        public async Threading.Task<IActionResult> PostTask(Task task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);


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
                throw new Exception("Resource not found");
            }

            // Update properties
            existingTask.Name = task.Name;
            existingTask.Description = task.Description;
            existingTask.DueDate = task.DueDate;
            existingTask.Status = task.Status;
            existingTask.AdditionalData = task.AdditionalData;

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
                throw new Exception("Resource not found");
            }

            _context.Tasks.Remove(task);

            await _context.SaveChangesAsync();


            return new JsonResult(new
            {
                data = null as object,
                success = true,
                message = "Successfully removed!",
                statusCode = 200
            })
            { StatusCode = 200 };
        }


    }
}
