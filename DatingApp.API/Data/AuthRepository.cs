using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _db;

        public AuthRepository(DataContext db)
        {
            this._db = db;

        }
        public async Task<User> Login(string Username, string Password)
        {
            var user = await _db.users.FirstOrDefaultAsync(x => x.Username == Username);
            if (user == null) return null;
            if (!VerifyPasswordHass(Password, user.PasswordHash, user.PasswordSalt)) return null;
            return user;
        }
        private bool VerifyPasswordHass(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var ComputeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < ComputeHash.Length; i++)
                {
                    if (ComputeHash[i] != passwordHash[i]) return false;
                }
                return true;
            }
        }
        public async Task<User> Register(User user, string Password)
        {
            byte[] PasswordHash, PasswordSalt;
            CreatePasswordHash(Password, out PasswordHash, out PasswordSalt);

            user.PasswordHash = PasswordHash;
            user.PasswordSalt = PasswordSalt;
            await _db.users.AddAsync(user);
            await _db.SaveChangesAsync();
            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {

                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }

        public async Task<bool> userExists(string Username)
        {
            if (await _db.users.AnyAsync(x => x.Username == Username)) return true;
            return false;
        }

        public async Task<User> LoginAsync(string Username, string Password)
        {
            var user = await _db.users.Include(p=>p.Photos).Include(p=>p.Photos).FirstOrDefaultAsync(x => x.Username == Username);
            if (user == null) return null;
            if (!VerifyPasswordHass(Password, user.PasswordHash, user.PasswordSalt)) return null;
            return user;
        }
    }
}