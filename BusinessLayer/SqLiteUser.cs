using AcessLayer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ModelLayer;

namespace BusinessLayer
{

    public class SqLiteUser
    {

        public static void CriaBD() => ASqLite.CriaDb();

        public static bool VerifyAcess()
        {
            List<Users> res = ASqLite.RecAcesso();

            if (res.Count > 0) return true;
            else return false;
        }

        public static void CadatraAcesso(string id, string login)
        {
            ASqLite.CadastraAcesso(id, login);
        }

        public static Users RecAcesso()
        {
            List<Users> res = ASqLite.RecAcesso();
            if (res.Count > 0) return res[0];
            else return null;
        }

        public static Users RecAcessoLastUpdate()
        {
            List<Users> res = ASqLite.RecAcesso();
            if (res.Count > 0) return res[0];
            else return null;

        }

        public static void AtualizaAcessoLastUpdade(string Key, DateTime LastUpdate)
        {
            ASqLite.AtualizaAcessoLastUpdade(Key, LastUpdate);
        }

        public static void DelAcesso() => ASqLite.DelAcesso();

    }
}
