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
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectServiceProxy _projectService; // Singleton service for project management
        private readonly FilebasePersistenceService _persistenceService; // Service for persistence operations

        public ProjectsController(FilebasePersistenceService persistenceService)
        {
            _projectService = ProjectServiceProxy.Current;
            _persistenceService = persistenceService;
        }

        // GET: api/projects
        [HttpGet]
        public ActionResult<IEnumerable<ProjectDto>> GetProjects()
        {
            var projects = _projectService.Projects.Select(ProjectMapper.ToDto);
            return Ok(projects); // Return all projects as DTOs
        }

        // GET: api/projects/{id}
        [HttpGet("{id}")]
        public ActionResult<ProjectDto> GetProject(int id)
        {
            var project = _projectService.GetProjectById(id);
            if (project == null)
            {
                return NotFound();
            }
            return Ok(ProjectMapper.ToDto(project)); // Return the project as a DTO
        }

        // POST: api/projects
        [HttpPost]
        public ActionResult<ProjectDto> CreateProject(CreateProjectDto dto)
        {
            // Validate the incoming DTO
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Map DTO to model
            var project = ProjectMapper.ToModel(dto);
            var createdProject = _projectService.AddOrUpdateProject(project);

            if (createdProject == null)
            {
                return BadRequest("Failed to create project.");
            }

            _persistenceService.SaveProjects(); // Persist the changes

            var projectDto = ProjectMapper.ToDto(createdProject);
            return CreatedAtAction(nameof(GetProject), new { id = projectDto.Id }, projectDto); // Return the created project as a DTO
        }

        // PUT: api/projects/{id}
        [HttpPut("{id}")]
        public ActionResult<ProjectDto> UpdateProject(int id, UpdateProjectDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dto.Id != id)
            {
                return BadRequest("ID mismatch between route and body.");
            }

            // Ensure the project exists before updating
            var existingProject = _projectService.GetProjectById(id);
            if (existingProject == null)
            {
                return NotFound();
            }

            var project = ProjectMapper.ToModel(dto); // Map DTO to model
            var updatedProject = _projectService.AddOrUpdateProject(project); // Update the project

            if (updatedProject == null)
            {
                return BadRequest("Failed to update project.");
            }

            _persistenceService.SaveProjects(); // Persist the changes

            return Ok(ProjectMapper.ToDto(updatedProject)); // Return the updated project as a DTO
        }

        // DELETE: api/projects/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteProject(int id)
        {
            var project = _projectService.GetProjectById(id);
            if (project == null)
            {
                return NotFound();
            }

            _projectService.DeleteProject(project); // Delete the project

            _persistenceService.SaveProjects(); // Persist the changes

            return NoContent();
        }
    }
}
