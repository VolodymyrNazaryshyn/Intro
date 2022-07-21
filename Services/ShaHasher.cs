﻿namespace Intro.Services
{
    public class ShaHasher : IHasher /*Sha - Secure hash algorithm*/
    { // Sha - американский стандарт цифровой подписи (финансовой)
        public string Hash(string message)
        {
            using (var algo = System.Security.Cryptography.SHA1.Create())
            {
                byte[] hash = algo.ComputeHash(System.Text.Encoding.UTF8.GetBytes(message));
                var sb = new System.Text.StringBuilder();

                foreach (byte b in hash)
                    sb.Append(b.ToString("X02"));

                return sb.ToString();
            }
        }
    }
}
