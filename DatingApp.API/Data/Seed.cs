using System;
using System.Collections.Generic;
using DatingApp.API.Models;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        private readonly DataContext _db;
        public Seed(DataContext db)
        {
            this._db = db;

        }

        public void SeedUsers()
        {
            var  userData=System.IO.File.ReadAllText("Data/UserSeedData.json");
            var users=JsonConvert.DeserializeObject<List<User>>(userData);
            foreach (var user in users)
            {
                Byte[] passwordhash,passwordSalt;
                CreatePasswordHash("password",out  passwordhash,out  passwordSalt);
                user.PasswordHash=passwordhash;
                user.PasswordSalt=passwordSalt;
                user.Username=user.Username.ToLower();
                _db.users.Add(user);              
            }
              _db.SaveChanges();
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {

                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }
    }
}