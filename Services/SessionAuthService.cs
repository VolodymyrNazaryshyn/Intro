using Intro.DAL.Context;
using Intro.DAL.Entities;

namespace Intro.Services
{
    public class SessionAuthService : IAuthService
    {
        private readonly IntroContext _context;
        public User User { get; set; }

        public SessionAuthService(IntroContext context)
        {
            _context = context;
        }

        public void Set(string id)
        {
            User = _context.Users.Find(System.Guid.Parse(id));
        }
    }
}
