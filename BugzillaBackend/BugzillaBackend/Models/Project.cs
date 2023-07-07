namespace BugzillaBackend.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        // Navigation properties
        public int ManagerId { get; set; } // Manager assigned to the project
        public User? Manager { get; set; }
        public List<User>? Developers { get; set; } // Developers associated with the project
        public List<User>? QAs { get; set; } // QAs associated with the project
        public List<Bug>? Bugs { get; set; } // Bugs in the project
    }
}
