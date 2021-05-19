﻿using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ModelLayer;
using AcessLayer.Firebase;

namespace BusinessLayer
{
    public class BUser
    {
        public static bool VerificaCadUsuario(string LoginNome, string Email)
        {
            
            bool ret = false;
            Task.Run(async () => ret = await AUsers.VerificaCadUsuario(LoginNome, Email)).Wait();
            return ret;
        }

        public static async Task CadastraUsuario(Users user) => await AUsers.CadastraUsuario(user);

        public async static Task<bool> RecoverUser(string LoginNome, string Senha)
        {
            Senha = CPEncrypt(Senha, Senha.Length);
            Users user = await AUsers.RecUser(LoginNome, Senha);
            if (user != null)
            {
                SqLiteUser.CadatraAcesso(user.Key, user.Nick);
                return true;
            }
            else return false;
        }


        public static Users RecoverUserEmail(string Email)
        {
            Users user = new Users();

            Task.Run(async () => user = await AUsers.RecUserEmail(Email)).Wait();

            if (user == null) return null;
            else
            {
                return user;
            }
        }

        public async static Task UpdateUserPassworld(string userKey, string passworld)
        {
            Users user = new Users();
            user.Key = userKey;
            user.Passworld = passworld;
             await AUsers.UpdateUserPassworld(user);
        }

        /// <returns>true para válido</returns>
        public static bool Valida_email(string email)
        {
            if (Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string CPEncrypt(string input, int key)
        {
            StringBuilder result = new StringBuilder();
            char[] charArray;

            for (int i = key; i >= 1; i--)
                result.Append(input.Substring(i - 1, 1));

            charArray = result.ToString().ToCharArray();
            result = new StringBuilder();

            for (int i = 0; i < key; i++)
                result.Append(Convert.ToChar(charArray[i] + (i + 1) * 2));

            return result.ToString();
        }



    }
}
