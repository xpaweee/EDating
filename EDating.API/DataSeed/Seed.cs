using System.Collections.Generic;
using System.Linq;
using EDating.API.Data;
using EDating.API.Models;
using Newtonsoft.Json;

namespace EDating.API.DataSeed
{
    public class Seed
    {
        public static void SeedUsers(DataContext context)
        {
            if(!context.User.Any())
            {
                var userData = System.IO.File.ReadAllText("DataSeed/data.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);
                foreach (var user in users)
                {
                    byte[] passwordHash, passwordSalt;
                    CreatePasswordHash("password", out passwordHash, out passwordSalt);

                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    user.Username = user.Username.ToLower();
                    context.User.Add(user);
                } 
            context.SaveChanges();

            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}