using System.Security.Cryptography;
using System.Text;

namespace Application.Common
{
    public class PasswordHashing
    {
        public Task<string> Hash(string password)
        {
            var sha512 = SHA512.Create();
            var hashByte = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Task.FromResult(Convert.ToBase64String(hashByte));
        }
        
        public async Task<string> HashAsync(string password)
        {
            return await Hash(password);
        }
    }
}
