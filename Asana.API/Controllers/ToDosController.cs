using Asana.Library.Models;
using Asana.Library.Services;
using Microsoft.AspNetCore.Mvc;

namespace Asana.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToDosController : ControllerBase
    {
        private readonly ToDoServiceProxy _todoService;

        public ToDosController()
        {
            _todoService = ToDoServiceProxy.Current;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ToDo>> GetToDos()
        {
            return Ok(_todoService.ToDos);
        }

        [HttpGet("{id}")]
        public ActionResult<ToDo> GetToDo(int id)
        {
            var todo = _todoService.GetToDoById(id);
            if (todo == null)
            {
                return NotFound();
            }
            return Ok(todo);
        }

        [HttpGet("project/{projectId}")]
        public ActionResult<IEnumerable<ToDo>> GetToDosByProject(int projectId)
        {
            var todos = _todoService.ToDos.Where(t => t.ProjectId == projectId);
            return Ok(todos);
        }

        [HttpPost]
        public ActionResult<ToDo> CreateToDo(ToDo todo)
        {
            if (todo == null)
            {
                return BadRequest();
            }

            todo.Id = 0; // Ensure it's treated as a new todo
            var createdToDo = _todoService.AddOrUpdateToDo(todo);
            return CreatedAtAction(nameof(GetToDo), new { id = createdToDo?.Id }, createdToDo);
        }

        [HttpPut("{id}")]
        public ActionResult<ToDo> UpdateToDo(int id, ToDo todo)
        {
            if (todo == null || todo.Id != id)
            {
                return BadRequest();
            }

            var existingToDo = _todoService.GetToDoById(id);
            if (existingToDo == null)
            {
                return NotFound();
            }

            var updatedToDo = _todoService.AddOrUpdateToDo(todo);
            return Ok(updatedToDo);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteToDo(int id)
        {
            var todo = _todoService.GetToDoById(id);
            if (todo == null)
            {
                return NotFound();
            }

            _todoService.DeleteToDo(id);
            return NoContent();
        }

        [HttpPatch("{id}/complete")]
        public ActionResult<ToDo> ToggleComplete(int id)
        {
            var todo = _todoService.GetToDoById(id);
            if (todo == null)
            {
                return NotFound();
            }

            todo.IsComplete = !todo.IsComplete;
            var updatedToDo = _todoService.AddOrUpdateToDo(todo);
            return Ok(updatedToDo);
        }
    }
}
