using ModelLayer;
using Firebase.Database.Query;
using System.Threading.Tasks;
using System.Linq;

namespace AcessLayer.Firebase
{
  public  class AUsers
    {
        public async static Task CadastraUsuario(Users user)
        {
            await AcessFirebase.firebase.Child("Users").PostAsync(user);
        }

        public async static Task<bool> VerificaCadUsuario(string vLoginNome, string vEmail)
        {
            if ((await AcessFirebase.firebase
            .Child("Users")
            .OnceAsync<Users>()).Where(a => (a.Object.Nick == vLoginNome && a.Object.Email == vEmail) || (a.Object.UserName == vLoginNome || a.Object.Email == vEmail)).Select(item => new Users
            {
                Key = item.Key,
            }).ToList().Count > 0)
                return false;
            else return true;
        }

        public async static Task<Users> RecUser(string vLoginNome, string vSenha)
        {
            return ((Users)(await AcessFirebase.firebase.Child("Users").OnceAsync<Users>()).Where(a => (a.Object.Nick == vLoginNome && a.Object.Passworld == vSenha)).Select(item => new Users
            {
                Key = item.Key,
                Nick = item.Object.Nick,
            }).FirstOrDefault());
        }

        public async static Task<Users> RecUserEmail(string vEmail)
        {
            return ((Users)(await AcessFirebase.firebase.Child("Users").OnceAsync<Users>()).Where(a => (a.Object.Email == vEmail)).Select(item => new Users
            {
                Key = item.Key,
                Nick = item.Object.Nick,
            }).FirstOrDefault());
        }


        public async static Task UpdateUserPassworld(Users user)
        {
            var toUpdateBookStatus = (await AcessFirebase.firebase
             .Child("Users")
             .OnceAsync<Users>()).Where(a => a.Key == user.Key && a.Object.Key == user.Key).FirstOrDefault().Object;

            toUpdateBookStatus = user;

            await AcessFirebase.firebase
              .Child("Users")
              .Child(user.Key)
              .PutAsync(toUpdateBookStatus);
        }

    }
}
