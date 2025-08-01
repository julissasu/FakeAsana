using Asana.API.DTOs;
using Asana.API.Mappers;
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
        public ActionResult<IEnumerable<ToDoDto>> GetToDos()
        {
            var todos = _todoService.ToDos.Select(ToDoMapper.ToDto);
            return Ok(todos);
        }

        [HttpGet("{id}")]
        public ActionResult<ToDoDto> GetToDo(int id)
        {
            var todo = _todoService.GetToDoById(id);
            if (todo == null)
            {
                return NotFound();
            }
            return Ok(ToDoMapper.ToDto(todo));
        }

        [HttpGet("project/{projectId}")]
        public ActionResult<IEnumerable<ToDoDto>> GetToDosByProject(int projectId)
        {
            var todos = _todoService.ToDos.Where(t => t.ProjectId == projectId).Select(ToDoMapper.ToDto);
            return Ok(todos);
        }

        [HttpPost]
        public ActionResult<ToDoDto> CreateToDo(CreateToDoDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var todo = ToDoMapper.ToModel(dto);
            var createdToDo = _todoService.AddOrUpdateToDo(todo);
            
            if (createdToDo == null)
            {
                return BadRequest("Failed to create todo.");
            }

            var todoDto = ToDoMapper.ToDto(createdToDo);
            return CreatedAtAction(nameof(GetToDo), new { id = todoDto.Id }, todoDto);
        }

        [HttpPut("{id}")]
        public ActionResult<ToDoDto> UpdateToDo(int id, UpdateToDoDto dto)
        {
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

            var todo = ToDoMapper.ToModel(dto);
            var updatedToDo = _todoService.AddOrUpdateToDo(todo);
            
            if (updatedToDo == null)
            {
                return BadRequest("Failed to update todo.");
            }

            return Ok(ToDoMapper.ToDto(updatedToDo));
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
        public ActionResult<ToDoDto> ToggleComplete(int id)
        {
            var todo = _todoService.GetToDoById(id);
            if (todo == null)
            {
                return NotFound();
            }

            todo.IsComplete = !todo.IsComplete;
            var updatedToDo = _todoService.AddOrUpdateToDo(todo);
            
            if (updatedToDo == null)
            {
                return BadRequest("Failed to update todo.");
            }

            return Ok(ToDoMapper.ToDto(updatedToDo));
        }
    }
}
