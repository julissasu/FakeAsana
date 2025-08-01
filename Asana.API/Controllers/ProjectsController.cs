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

        public ProjectsController()
        {
            _projectService = ProjectServiceProxy.Current;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Project>> GetProjects()
        {
            return Ok(_projectService.Projects);
        }

        [HttpGet("{id}")]
        public ActionResult<Project> GetProject(int id)
        {
            var project = _projectService.GetProjectById(id);
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }

        [HttpPost]
        public ActionResult<Project> CreateProject(Project project)
        {
            if (project == null)
            {
                return BadRequest();
            }

            project.Id = 0; // Ensure it's treated as a new project
            var createdProject = _projectService.AddOrUpdateProject(project);
            return CreatedAtAction(nameof(GetProject), new { id = createdProject?.Id }, createdProject);
        }

        [HttpPut("{id}")]
        public ActionResult<Project> UpdateProject(int id, Project project)
        {
            if (project == null || project.Id != id)
            {
                return BadRequest();
            }

            var existingProject = _projectService.GetProjectById(id);
            if (existingProject == null)
            {
                return NotFound();
            }

            var updatedProject = _projectService.AddOrUpdateProject(project);
            return Ok(updatedProject);
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
            return NoContent();
        }
    }
}
