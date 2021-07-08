using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ModelLayer;
using AcessLayer.Firebase;
using AcessLayer.SqLite;

namespace BusinessLayer
{
    public class BUser
    {
        readonly AUsersFb aUsersFB = new AUsersFb();
        readonly ASqLite aSqLite = new ASqLite();

        public void CreateDbLocal() => aSqLite.CreateDb();

        public bool VerificaCadUsuario(string LoginNome, string Email)
        {
            bool ret = false;
            Task.Run(async () => ret = await aUsersFB.VerifyUser(LoginNome, Email)).Wait();
            return ret;
        }

        /// <summary>
        /// Add a user in fb
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task AddUser(Users user) => await aUsersFB.AddUser(user);


        /// <summary>
        /// Get user in fb and add him in sqlite
        /// </summary>
        /// <param name="LoginNome"></param>
        /// <param name="Senha"></param>
        /// <returns></returns>
        public async Task<bool> GetUser(string LoginNome, string Senha)
        {
            Senha = CPEncrypt(Senha, Senha.Length);
            Users user = await aUsersFB.GetUser(LoginNome, Senha);
            if (user != null)
            {
                aSqLite.AddUserLocal(user.Key, user.Nick);
                return true;
            }
            else
                return false;
        }


        /// <summary>
        /// get user in the db local
        /// </summary>
        /// <returns></returns>
        public Users GetUserLocal() => aSqLite.GetUserLocal();

        public Users GetUserByEmail(string Email)
        {
            Users user = new Users();

            Task.Run(async () => user = await aUsersFB.GetUserByEmail(Email)).Wait();

            return user;
        }

        public void DelAcesso() => aSqLite.DelUserLocal();

        public void UpdateUserLastUpdadeLocal(string Key, DateTime LastUpdate) => aSqLite.UpdateUserLastUpdadeLocal(Key, LastUpdate);

        public async Task UpdateUserPassworld(string userKey, string passworld)
        {
            await aUsersFB.UpdateUserPassworld(new Users() { Key = userKey, Passworld = passworld });
        }

        /// <returns>true para válido</returns>
        public static bool ValidateEmail(string email)
        {
            return Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }

        public static string CPEncrypt(string input, int key)
        {
            StringBuilder result = new StringBuilder();
            char[] charArray;

            for (int i = key; i >= 1; i--)
            {
                result.Append(input.Substring(i - 1, 1));
            }

            charArray = result.ToString().ToCharArray();
            result = new StringBuilder();

            for (int i = 0; i < key; i++)
            {
                result.Append(Convert.ToChar(charArray[i] + (i + 1) * 2));
            }

            return result.ToString();
        }

    }
}
