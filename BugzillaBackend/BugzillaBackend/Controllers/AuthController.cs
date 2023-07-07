using BugzillaBackend.Data;
using BugzillaBackend.Data.Models;
using BugzillaBackend.Models;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Text;
using MimeKit;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using MailKit.Net.Smtp;

namespace BugzillaBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //public static User user = new User();

        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataContext _context;


        public AuthController(IConfiguration configuration, IUserService userService,
            DataContext dataContext, IHttpContextAccessor httpContextAccessor) 
        {
            _configuration = configuration;
            _userService = userService;
            _context = dataContext;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet, Authorize]
        public ActionResult<string> GetMe()
        {
            var userName = _userService.GetMyName();
            return Ok(userName);

            //var userName = User?.Identity?.Name;
            //var userName2 = User.FindFirstValue(ClaimTypes.Name);
            //var role = User.FindFirstValue(ClaimTypes.Role);
            //return Ok(new { userName, userName2, role});
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserRegisterRequest request)
        {
            User user = new User();
            if (_context.Users.Any(u => u.Email == request.Email))
            {
                return BadRequest("User already Exists");
            }
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.Username = request.Username;
            user.Email = request.Email;
            user.Role = request.Role;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.VerificationToken = CreateRandomToken();

            _context.Users.Add(user);
            await _context.SaveChangesAsync();




            // SendEmail

            var verificationUrl = CreateVerificationUrl(user.VerificationToken);
            var body = $"<h3>Please click on the following link to verify your email</h3>\n</h5>{verificationUrl}</h5>";
            var subject = "Verification Email";
            SendEmail(body, user.Email, subject);


            return Ok("User successfully created");
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginRequest request)
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (currentUser == null)
            {
                return BadRequest("User not found");
            }

            if(currentUser.VerifiedAt == null)
            {
                return BadRequest("User not Verified!");
            }

            if (!verifyPasswordHash(request.Password, currentUser.PasswordHash, currentUser.PasswordSalt))
            {
                return BadRequest("Wrong password");
            }

            string token = CreateToken(currentUser);

            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken, currentUser);
            return Ok(token);
        }

        [HttpGet("verify")]
        public async Task<ActionResult<string>> Verify(string token)
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
            if (currentUser == null)
            {
                return BadRequest("Invalid Token");
            }

            currentUser.VerifiedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok("User Verified!");
        }

        [HttpGet("forgot-password")]
        public async Task<ActionResult<string>> ForgotPassword(string email)
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (currentUser == null)
            {
                return BadRequest("User not found");
            }

            currentUser.PasswordResetToken = CreateRandomToken();
            currentUser.ResetTokenExpires = DateTime.Now.AddDays(1);
            await _context.SaveChangesAsync();

            // SendEmail

            var resetPasswordUrl = CreateResetPasswordUrl(currentUser.PasswordResetToken);
            var body = $"<h3>Please click on the following link to reset your password</h3>\n</h5>{resetPasswordUrl}</h5>";
            var subject = "Reset Passowrd Link";
            SendEmail(body, currentUser.Email, subject);

            return Ok("You may now reset your password. Check your email");
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<string>> ResetPassword(ResetPasswordRequest request)
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token);
            if (currentUser == null || currentUser.ResetTokenExpires < DateTime.Now)
            {
                return BadRequest("Invalid Token");
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            currentUser.PasswordHash = passwordHash;
            currentUser.PasswordSalt = passwordSalt;
            currentUser.ResetTokenExpires = null;
            currentUser.PasswordResetToken = null;

            await _context.SaveChangesAsync();
            return Ok("Password successfully reset.");
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken(User user)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (!user.RefreshToken.Equals(refreshToken))
            {
                return Unauthorized("Invalid Refresh Token.");
            }
            else if (user.TokenExpires < DateTime.Now)
            {
                return Unauthorized("Token expired.");
            }

            string token = CreateToken(user);
            var refreshTokenHash = GenerateRefreshToken();
            SetRefreshToken(refreshTokenHash, user);
            return Ok(token);
        }

        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken()
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };
            return refreshToken;
        }

        private void SetRefreshToken(RefreshToken newRefreshToken, User user)
        {
            var cookieOptions = new CookieOptions()
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            user.RefreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.Created;
            user.TokenExpires = newRefreshToken.Expires;
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) 
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool verifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        } 

        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

        private void SendEmail(string message, string recieverEmail, string subject)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("haider.ejaz@devsinc.com"));
            email.To.Add(MailboxAddress.Parse(recieverEmail));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = message };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("haider.ejaz@devsinc.com", "bwdwvmxlrsiuvkyb");
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        private string CreateVerificationUrl(string token)
        {
            var appUrl = getApplicationUrl();
            return $"{appUrl}/api/auth/verify?token={token}";
        }

        private string CreateResetPasswordUrl(string token)
        {
            var clientUrl = "http://localhost:4200";
            return $"{clientUrl}/reset-password?token={token}";
        }

        private string getApplicationUrl()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var request = httpContext.Request;
            var applicationUrl = $"{request.Scheme}://{request.Host}";
            return applicationUrl;
        }

    }
}
