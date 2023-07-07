using BugzillaBackend.Data;
using BugzillaBackend.Data.Models;
using BugzillaBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BugzillaBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        public static Project project = new Project();

        private readonly DataContext _context;

        public ProjectsController(DataContext dataContext)
        {
            _context = dataContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var projects = await _context.Projects.ToListAsync();
            return Ok(projects);
        }

        [HttpPost]
        public async Task<IActionResult> AddProject(ProjectRequest request)
        {
            var project = new Project();

            project.ManagerId = request.ManagerId;
            project.Title = request.Title;

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return Ok(project);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetProject([FromRoute] int id)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }


        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateProject([FromRoute] int id, ProjectRequest request)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            project.Title = request.Title;
            project.ManagerId = request.ManagerId;

            await _context.SaveChangesAsync();
            return Ok(project);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteProject([FromRoute] int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return Ok(project);
        }
    }
}
