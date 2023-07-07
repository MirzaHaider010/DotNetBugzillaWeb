using BugzillaBackend.Models;

namespace BugzillaBackend.Data.Models
{
    public class BugRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Screenshot { get; set; }
        public BugType Type { get; set; }
        public BugStatus Status { get; set; }
        public DateTime Deadline { get; set; }
        public int CreatorId { get; set; }
        public int DeveloperId { get; set; }
        public int ProjectId { get; set; }
    }
}
