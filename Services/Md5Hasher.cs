namespace Intro.Services
{
    public class Md5Hasher : IHasher
    {
        public string Hash(string message)
        { // криптографический хеш - алгоритм преобразования одного массива байт в другой,
          // обладает дифузией: если сменить один бит - весь хеш меняется до неузнаваемости
            using (var algo = System.Security.Cryptography.MD5.Create())
            {
                byte[] hash = algo.ComputeHash(System.Text.Encoding.UTF8.GetBytes(message));
                var sb = new System.Text.StringBuilder();

                foreach (byte b in hash)
                    sb.Append(b.ToString("X02")); // преобразовать в 16-ричную форму

                return sb.ToString();
            }
        }
    }
}
