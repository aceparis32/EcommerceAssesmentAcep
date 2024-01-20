using Application.Interfaces;
using System.Security.Claims;

namespace AssesmentAcep.WebAPI.Services
{
    public class UserRepositoryService : IUserRepositoryService
    {
        public UserRepositoryService(IHttpContextAccessor httpContextAccessor)
        {
            Username = httpContextAccessor.HttpContext.User.FindFirstValue("Username");
            Fullname = httpContextAccessor.HttpContext.User.FindFirstValue("Fullname");
            Email = httpContextAccessor.HttpContext.User.FindFirstValue("Email");
            Role = httpContextAccessor.HttpContext.User.FindFirstValue("Role");
        }

        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
