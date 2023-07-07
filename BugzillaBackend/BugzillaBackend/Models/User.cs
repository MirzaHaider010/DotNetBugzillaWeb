namespace BugzillaBackend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;    
        public byte[] PasswordHash { get; set; } = new byte[32];
        public byte[] PasswordSalt { get; set; } = new byte[32];
        public string? VerificationToken { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }





        public UserType UserType { get; set; }

        // Navigation properties
        public List<Project>? Projects { get; set; } // Projects the user is associated with
        public List<Bug>? CreatedBugs { get; set; } // Bugs created by the user
        public List<Bug>? AssignedBugs { get; set; } // Bugs assigned to the user

    }

    public enum UserType
    {
        Developer,
        Manager,
        QA
    }
}
