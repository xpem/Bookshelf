using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AcessLayer
{

    /// <summary>
    /// Considerei deixar os métodos de acesso ao banco local como sincronos, pois preciso dos dados dele para acessar os metodos asincronos posteriores
    /// </summary>
    public class ASqLite
    {
        protected readonly static SQLiteAsyncConnection _database = new SQLiteAsyncConnection(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Bookshelf2.db3"));

        public static void CriaDb() => _database.ExecuteAsync("create table if not exists ACESSADD (ID integer primary key autoincrement, KEY text, LOGINNOME text)");

        public static void CadastraAcesso(string id, string login) => _database.ExecuteAsync($"insert into ACESSADD (KEY,LOGINNOME) values ('{id}','{login}')");

        public static void DelAcesso() => _database.ExecuteAsync($"delete from ACESSADD");

        public static Task<List<ModelLayer.Users>> RecAcesso() => _database.QueryAsync<ModelLayer.Users>("select KEY,LOGINNOME from ACESSADD");
    }
}
