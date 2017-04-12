using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlTypes;

namespace Lawnhiro.API
{
    public static class Utility
    {
        public static SqlBinary ComputeRandomSalt()
        {
            Random r = new Random();
            byte[] bytes = new byte[12];
            r.NextBytes(bytes);
            return new SqlBinary(bytes);
        }

        public static SqlBinary ComputeHash(string password, SqlBinary salt)
        {
            byte[] encodedPassword = Encoding.UTF8.GetBytes(password);
            byte[] saltedPassword = new byte[encodedPassword.Length];
            for (int i = 0; i < saltedPassword.Length; i++)
            {
                saltedPassword[i] = (byte)(encodedPassword[i] ^ salt[salt.Length % i]);
            }
            return System.Security.Cryptography.SHA256.Create().ComputeHash(saltedPassword);
        }

        public static bool IsPasswordCorrect(string suppliedPassword, SqlBinary salt, SqlBinary passwordHash)
        {
            return (bool)(ComputeHash(suppliedPassword, salt) == passwordHash);
        }
    }
}
