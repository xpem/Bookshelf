using Bookshelf.AcessLayer.SqLite;
using Microsoft.Data.Sqlite;
using ModelLayer;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static ModelLayer.Books;

namespace AcessLayer
{

    /// <summary>
    /// Considerei deixar os métodos de acesso ao banco local como sincronos, pois preciso dos dados dele para acessar os metodos asincronos posteriores
    /// </summary>
    public class ASqLite
    {
        protected readonly static SQLiteAsyncConnection _database = new SQLiteAsyncConnection(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Bookshelf.db3"));

        protected readonly static SqliteConnection db = new SqliteConnection($"Filename={System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Bookshelf.db3")}");

        public static void CriaDb()
        {
            db.Open();
            String command = "create table if not exists ACESSADD (ID integer primary key autoincrement, KEY text, LOGINNOME text, LastUpdate DATETIME);";
            SqliteCommand createTable = new SqliteCommand(command, db);
            createTable.ExecuteReader();
            command = "create table if not exists BOOK (KEY text, UserKey text, Title text, SubTitle text, Authors text, Year integer, Volume text, Pages integer, Genre text, LastUpdate DATETIME);";
            createTable = new SqliteCommand(command, db);
            createTable.ExecuteReader();
            command = "create table if not exists BOOKSITUATIONS (BookKey text, UserKey text, Situation integer, Rate integer, Comment text);";
            createTable = new SqliteCommand(command, db);
            createTable.ExecuteReader();
            db.Close();
        }

        /// <summary>
        /// cadastra o livro e sua situação do banco de dados local
        /// </summary>
        /// <param name="book"></param>
        public static void CadastraLivroLocal(Book book)
        {
            db.Open();

            SqliteCommand insertCommand = new SqliteCommand
            {
                Connection = db,
                CommandText = "insert into BOOK(KEY, UserKey, Title, SubTitle, Authors, Year, Volume, Pages, Genre, LastUpdate) values(@Key, @UserKey, @Title, @SubTitle, @Authors, @Year, @Volume, @Pages, @Genre, @LastUpdate)"
            };
            insertCommand.Parameters.AddWithNullableValue("@Key", book.Key);
            insertCommand.Parameters.AddWithNullableValue("@UserKey", book.UserKey);
            insertCommand.Parameters.AddWithNullableValue("@Title", book.Title);
            insertCommand.Parameters.AddWithNullableValue("@SubTitle", book.SubTitle);
            insertCommand.Parameters.AddWithNullableValue("@Authors", book.Authors);
            insertCommand.Parameters.AddWithNullableValue("@Year", book.Year);
            insertCommand.Parameters.AddWithNullableValue("@Volume", book.Volume);
            insertCommand.Parameters.AddWithNullableValue("@Pages", book.Pages);
            insertCommand.Parameters.AddWithNullableValue("@Genre", book.Genre);
            insertCommand.Parameters.AddWithNullableValue("@LastUpdate", book.LastUpdate);
            insertCommand.ExecuteReader();

            insertCommand = new SqliteCommand
            {
                Connection = db,
                CommandText = "insert into BOOKSITUATIONS(BookKey,UserKey,Situation,Rate,Comment) values ( @Key, @UserKey, @Situation, @Rate, @Comment)"
            };
            insertCommand.Parameters.AddWithNullableValue("@Key", book.Key);
            insertCommand.Parameters.AddWithNullableValue("@UserKey", book.UserKey);
            insertCommand.Parameters.AddWithNullableValue("@Situation", book.BooksSituations.Situation);
            insertCommand.Parameters.AddWithNullableValue("@Rate", book.BooksSituations.Rate);
            insertCommand.Parameters.AddWithNullableValue("@Comment", book.BooksSituations.Comment);
            insertCommand.ExecuteReader();

            db.Close();
        }


        #region books

        public static List<Books.BookSituation> GetBookshelfTotais(string userKey)
        {
            db.Open();

            SqliteCommand selectCommand = new SqliteCommand("select Situation from BOOKSITUATIONS where UserKey = @userKey", db);
            selectCommand.Parameters.AddWithValue("@userKey", userKey);
            SqliteDataReader query = selectCommand.ExecuteReader();

            List<Books.BookSituation> lista = new List<BookSituation>();

            while (query.Read())
            {
                lista.Add(new Books.BookSituation() { Situation = query.GetInt32(0) });
            }

            db.Close();

            return lista;

        }

        #endregion

        public static void CadastraAcesso(string id, string login) => _database.ExecuteAsync($"insert into ACESSADD (KEY,LOGINNOME) values ('{id}','{login}')");

        public static void DelAcesso() => _database.ExecuteAsync($"delete from ACESSADD");

        public static Task<List<ModelLayer.Users>> RecAcesso() => _database.QueryAsync<ModelLayer.Users>("select KEY,LOGINNOME from ACESSADD");

        public static Task<List<ModelLayer.Users>> RecAcessoLastUpdate() => _database.QueryAsync<ModelLayer.Users>("select KEY,LastUpdate from ACESSADD");

        public static void AtualizaAcessoLastUpdade(string id, DateTime LastUpdate)
        {
            string query = $"update ACESSADD set LastUpdate = '{LastUpdate}' where id = '{id}'";
            _database.ExecuteAsync($"update ACESSADD set LastUpdate = '{LastUpdate}' where key = '{id}'");
        }

    }
}
