using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Models.Tasks;
using System.Threading.Tasks;

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
        public async System.Threading.Tasks.Task<ActionResult<IEnumerable<Models.Tasks.Task<string>>>> GetTasks(
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

            return await query.ToListAsync();
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        public async System.Threading.Tasks.Task<ActionResult<Models.Tasks.Task<string>>> GetTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return task;
        }

        // POST: api/Tasks
        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> PostTask(Models.Tasks.Task<string> task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        // PUT: api/Tasks/5
        [HttpPut("{id}")]
        public async System.Threading.Tasks.Task<IActionResult> PutTask(int id, Models.Tasks.Task<string> task)
        {

            var existingTask = await _context.Tasks.FindAsync(id);

            if (existingTask == null)
            {
                return NotFound();
            }

            // Update properties
            existingTask.Name = task.Name;
            existingTask.Description = task.Description;
            existingTask.DueDate = task.DueDate;
            existingTask.Status = task.Status;
            existingTask.AdditionalData = task.AdditionalData;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async System.Threading.Tasks.Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
