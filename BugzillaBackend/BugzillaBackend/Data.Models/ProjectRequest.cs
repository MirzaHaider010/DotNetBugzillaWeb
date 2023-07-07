namespace BugzillaBackend.Data.Models
{
    public class ProjectRequest
    {
        public string Title { get; set; } = string.Empty;
        public int ManagerId { get; set; }
    }
}
