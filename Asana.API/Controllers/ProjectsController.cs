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
        private readonly ProjectServiceProxy _projectService;
        private readonly FilebasePersistenceService _persistenceService;

        public ProjectsController(FilebasePersistenceService persistenceService)
        {
            _projectService = ProjectServiceProxy.Current;
            _persistenceService = persistenceService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ProjectDto>> GetProjects()
        {
            var projects = _projectService.Projects.Select(ProjectMapper.ToDto);
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public ActionResult<ProjectDto> GetProject(int id)
        {
            var project = _projectService.GetProjectById(id);
            if (project == null)
            {
                return NotFound();
            }
            return Ok(ProjectMapper.ToDto(project));
        }

        [HttpPost]
        public ActionResult<ProjectDto> CreateProject(CreateProjectDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var project = ProjectMapper.ToModel(dto);
            var createdProject = _projectService.AddOrUpdateProject(project);

            if (createdProject == null)
            {
                return BadRequest("Failed to create project.");
            }

            _persistenceService.SaveProjects();

            var projectDto = ProjectMapper.ToDto(createdProject);
            return CreatedAtAction(nameof(GetProject), new { id = projectDto.Id }, projectDto);
        }

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

            var existingProject = _projectService.GetProjectById(id);
            if (existingProject == null)
            {
                return NotFound();
            }

            var project = ProjectMapper.ToModel(dto);
            var updatedProject = _projectService.AddOrUpdateProject(project);

            if (updatedProject == null)
            {
                return BadRequest("Failed to update project.");
            }

            _persistenceService.SaveProjects();

            return Ok(ProjectMapper.ToDto(updatedProject));
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProject(int id)
        {
            var project = _projectService.GetProjectById(id);
            if (project == null)
            {
                return NotFound();
            }

            _projectService.DeleteProject(project);

            _persistenceService.SaveProjects();

            return NoContent();
        }
    }
}
