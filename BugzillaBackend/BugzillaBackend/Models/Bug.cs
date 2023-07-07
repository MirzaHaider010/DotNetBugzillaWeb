using BugzillaBackend.Models;

namespace BugzillaBackend.Models
{
    public class Bug
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Screenshot { get; set; } = string.Empty;
        public BugType Type { get; set; }
        public BugStatus Status { get; set; }
        public DateTime Deadline { get; set; }

        // Navigation properties
        public int CreatorId { get; set; } // User who created the bug
        public User? Creator { get; set; }
        public int DeveloperId { get; set; } // User assigned to solve the bug
        public User? Developer { get; set; }
        public int ProjectId { get; set; } // Project to which the bug belongs
        public Project? Project { get; set; }
    }

    public enum BugType
    {
        Feature,
        Bug
    }

    public enum BugStatus
    {
        New,
        Started,
        Completed,
        Resolved
    }
}