using Asana.API.DTOs;
using Asana.API.Mappers;
using Asana.API.Services;
using Asana.Library.Models;
using Asana.Library.Services;
using Microsoft.AspNetCore.Mvc;

namespace Asana.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToDosController : ControllerBase
    {
        private readonly ToDoServiceProxy _todoService; // Singleton service for todo management
        private readonly FilebasePersistenceService _persistenceService; // Service for persistence operations

        public ToDosController(FilebasePersistenceService persistenceService)
        {
            _todoService = ToDoServiceProxy.Current;
            _persistenceService = persistenceService;
        }

        // GET: api/todos
        [HttpGet]
        public ActionResult<IEnumerable<ToDoDto>> GetToDos()
        {
            var todos = _todoService.ToDos.Select(ToDoMapper.ToDto); // Convert todos to DTOs
            return Ok(todos);
        }

        // GET: api/todos/{id}
        [HttpGet("{id}")]
        public ActionResult<ToDoDto> GetToDo(int id)
        {
            var todo = _todoService.GetToDoById(id);
            if (todo == null)
            {
                return NotFound();
            }
            return Ok(ToDoMapper.ToDto(todo)); // Return the todo as a DTO
        }

        // GET: api/todos/project/{projectId}
        [HttpGet("project/{projectId}")]
        public ActionResult<IEnumerable<ToDoDto>> GetToDosByProject(int projectId)
        {
            var todos = _todoService.ToDos.Where(t => t.ProjectId == projectId).Select(ToDoMapper.ToDto); // Filter todos by project ID
            return Ok(todos); // Return todos associated with the specified project
        }

        // POST: api/todos
        [HttpPost]
        public ActionResult<ToDoDto> CreateToDo(CreateToDoDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var todo = ToDoMapper.ToModel(dto); // Map DTO to model
            var createdToDo = _todoService.AddOrUpdateToDo(todo); // Add or update the todo

            if (createdToDo == null)
            {
                return BadRequest("Failed to create todo.");
            }

            _persistenceService.SaveToDos(); // Persist the changes

            var todoDto = ToDoMapper.ToDto(createdToDo);
            return CreatedAtAction(nameof(GetToDo), new { id = todoDto.Id }, todoDto); // Return the created todo as a DTO
        }

        // PUT: api/todos/{id}
        [HttpPut("{id}")]
        public ActionResult<ToDoDto> UpdateToDo(int id, UpdateToDoDto dto)
        {
            // Validate the incoming DTO
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dto.Id != id)
            {
                return BadRequest("ID mismatch between route and body.");
            }

            var existingToDo = _todoService.GetToDoById(id);
            if (existingToDo == null)
            {
                return NotFound();
            }

            var todo = ToDoMapper.ToModel(dto); // Map DTO to model
            var updatedToDo = _todoService.AddOrUpdateToDo(todo); // Update the todo

            if (updatedToDo == null)
            {
                return BadRequest("Failed to update todo.");
            }

            _persistenceService.SaveToDos(); // Persist the changes

            return Ok(ToDoMapper.ToDto(updatedToDo)); // Return the updated todo as a DTO
        }

        // DELETE: api/todos/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteToDo(int id)
        {
            var todo = _todoService.GetToDoById(id);
            if (todo == null)
            {
                return NotFound();
            }

            _todoService.DeleteToDo(id); // Delete the todo by ID

            _persistenceService.SaveToDos(); // Persist the changes

            return NoContent(); // Return 204 No Content on successful deletion
        }

        // PATCH: api/todos/{id}/complete
        [HttpPatch("{id}/complete")]
        public ActionResult<ToDoDto> ToggleComplete(int id)
        {
            var todo = _todoService.GetToDoById(id);
            if (todo == null)
            {
                return NotFound();
            }

            todo.IsComplete = !todo.IsComplete; // Toggle the completion status
            var updatedToDo = _todoService.AddOrUpdateToDo(todo); // Update the todo

            if (updatedToDo == null)
            {
                return BadRequest("Failed to update todo.");
            }

            _persistenceService.SaveToDos(); // Persist the changes

            return Ok(ToDoMapper.ToDto(updatedToDo)); // Return the updated todo as a DTO
        }
    }
}
