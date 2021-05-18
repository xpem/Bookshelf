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

        public static Users RecAcesso()
        {
            Users res = ASqLite.RecAcesso();
            if (res != null) return res;
            else return null;
        }

        public static void CadatraAcesso(string id, string login)
        {
            ASqLite.CadastraAcesso(id, login);
        } 

        public static void AtualizaAcessoLastUpdade(string Key, DateTime LastUpdate)
        {
            ASqLite.AtualizaAcessoLastUpdade(Key, LastUpdate);
        }

        public static void DelAcesso() => ASqLite.DelAcesso();

    }
}
