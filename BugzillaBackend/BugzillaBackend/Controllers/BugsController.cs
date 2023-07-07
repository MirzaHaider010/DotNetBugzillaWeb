using BugzillaBackend.Data;
using BugzillaBackend.Data.Models;
using BugzillaBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BugzillaBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BugsController : ControllerBase
    {
        public static Bug bug = new Bug();

        private readonly DataContext _context;

        public BugsController(DataContext dataContext) 
        {
            _context = dataContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetBugs()
        {
            var bugs = await _context.Bugs.ToListAsync();
            return Ok(bugs);
        }

        [HttpPost]
        public async Task<IActionResult> AddBug(BugRequest request)
        {
            bug.Title = request.Title;
            bug.Description = request.Description;
            bug.Screenshot = request.Screenshot;
            bug.Status = request.Status;
            bug.Type = request.Type;
            bug.Deadline = request.Deadline;
            bug.CreatorId = request.CreatorId;
            bug.DeveloperId = request.DeveloperId;
            bug.ProjectId = request.ProjectId;

            _context.Bugs.Add(bug);
            await _context.SaveChangesAsync();

            return Ok(request);
        }


        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateBug([FromRoute] int id, BugRequest request)
        {
            var bug = await _context.Bugs.FindAsync(id);
            if (bug == null)
            {
                return NotFound();
            }

            bug.Title = request.Title;
            bug.Description = request.Description;
            bug.Screenshot = request.Screenshot;
            bug.Status = request.Status;
            bug.Type = request.Type;
            bug.Deadline = request.Deadline;
            bug.CreatorId = request.CreatorId;
            bug.DeveloperId = request.DeveloperId;
            bug.ProjectId = request.ProjectId;

            await _context.SaveChangesAsync();
            return Ok(bug);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteBug([FromRoute] int id)
        {
            var bug = await _context.Bugs.FindAsync(id);

            if (bug == null)
            {
                return NotFound();
            }

            _context.Bugs.Remove(bug);
            await _context.SaveChangesAsync();
            return Ok(bug);
        }

    }
}
