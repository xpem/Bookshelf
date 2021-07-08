using Firebase.Database.Query;
using ModelLayer;
using System.Linq;
using System.Threading.Tasks;

namespace AcessLayer.Firebase
{
    public class AUsersFb
    {

        /// <summary>
        /// Add a user in fb
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task AddUser(Users user)
        {
            _ = await AcessFirebase.firebase.Child("Users").PostAsync(user);
        }

        /// <summary>
        /// Verify register of user in database by name or email
        /// </summary>
        /// <param name="vLoginNome"></param>
        /// <param name="vEmail"></param>
        /// <returns></returns>
        public async Task<bool> VerifyUser(string vLoginNome, string vEmail)
        {
            return (await AcessFirebase.firebase.Child("Users").OnceAsync<Users>())
                .Where(a => (a.Object.Nick == vLoginNome && a.Object.Email == vEmail) || a.Object.UserName == vLoginNome || a.Object.Email == vEmail)
                .Select(item => new Users
                {
                    Key = item.Key,
                }).ToList().Count <= 0;
        }

        public async Task<Users> GetUser(string vLoginNome, string vSenha)
        {
            return (await AcessFirebase.firebase.Child("Users").OnceAsync<Users>()).Where(a => a.Object.Nick == vLoginNome && a.Object.Passworld == vSenha).Select(item => new Users
            {
                Key = item.Key,
                Nick = item.Object.Nick,
            }).FirstOrDefault();
        }

        public async Task<Users> GetUserByEmail(string vEmail)
        {
            return (await AcessFirebase.firebase.Child("Users").OnceAsync<Users>()).Where(a => (a.Object.Email == vEmail)).Select(item => new Users
            {
                Key = item.Key,
                Nick = item.Object.Nick,
            }).FirstOrDefault();
        }

        public async Task UpdateUserPassworld(Users user)
        {
            Users toUpdateBookStatus = (await AcessFirebase.firebase.Child("Users").OnceAsync<Users>())
                .FirstOrDefault(a => a.Key == user.Key && a.Object.Key == user.Key).Object;

            toUpdateBookStatus = user;

            await AcessFirebase.firebase.Child("Users").Child(user.Key).PutAsync(toUpdateBookStatus);
        }

    }
}
