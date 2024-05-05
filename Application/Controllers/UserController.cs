using BL.Common;
using BL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BL.Controllers
{
    public class UserController
    {
        private IDbContext _context;

        public UserController(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Authenticate(string login, string password) 
        {
            PasswordHashing hashing = new PasswordHashing();
            var passwordHash = await hashing.HashAsync(password);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login.ToLower() == login.ToLower() && u.Password == passwordHash);
            return user != null;
        }

        public async Task<bool> LoginIsUniq(string login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login.ToLower() == login.ToLower());
            return user != null;
        }
    }
}
