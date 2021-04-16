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

        public readonly static SqliteConnection db = new SqliteConnection($"Filename={System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Bookshelf.db3")}");

        public static void CriaDb()
        {
            SqLiteFunctions.OpenIfClosed();
            String command = "create table if not exists ACESSADD (ID integer primary key autoincrement, KEY text, LOGINNOME text, LastUpdate DATETIME);";
            SqliteCommand createTable = new SqliteCommand(command, db);
            createTable.ExecuteReader();
            command = "create table if not exists BOOK (KEY text, UserKey text, Title text, SubTitle text, Authors text, Year integer, Volume text, Pages integer, Isbn text, Genre text, LastUpdate DATETIME);";
            createTable = new SqliteCommand(command, db);
            createTable.ExecuteReader();
            command = "create table if not exists BOOKSITUATIONS (BookKey text, UserKey text, Situation integer, Rate integer, Comment text);";
            createTable = new SqliteCommand(command, db);
            createTable.ExecuteReader();
            SqLiteFunctions.CloseIfOpen();
        }

        public static void CadastraAcesso(string id, string login)
        {
            SqLiteFunctions.OpenIfClosed();
            SqliteCommand insertCommand = new SqliteCommand
            {
                Connection = db,
                CommandText = "insert into ACESSADD (KEY,LOGINNOME,LASTUPDATE) values (@KEY,@LOGINNOME,@LASTUPDATE)"
            };
            insertCommand.Parameters.AddWithNullableValue("@KEY", id);
            insertCommand.Parameters.AddWithNullableValue("@LOGINNOME", login);
            insertCommand.Parameters.AddWithNullableValue("@LASTUPDATE", DateTime.MinValue);
            insertCommand.ExecuteReader();
            SqLiteFunctions.CloseIfOpen();
        }

        public static void DelAcesso()
        {
            SqLiteFunctions.OpenIfClosed();
            SqliteCommand insertCommand = new SqliteCommand { Connection = db, CommandText = "delete from ACESSADD" };
            insertCommand.ExecuteReader();
            SqLiteFunctions.CloseIfOpen();
        }

        public static List<ModelLayer.Users> RecAcesso()
        {
            SqLiteFunctions.OpenIfClosed();

            SqliteCommand selectCommand = new SqliteCommand("select KEY,LOGINNOME,LASTUPDATE from ACESSADD", db);
            SqliteDataReader query = selectCommand.ExecuteReader();

            List<ModelLayer.Users> lista = new List<ModelLayer.Users>();

            while (query.Read())
            {
                lista.Add(new ModelLayer.Users() { Key = query.GetString(0), Nick = query.GetString(1), LastUpdate = query.GetDateTime(2) });
            }

            SqLiteFunctions.CloseIfOpen();

            return lista;

        }

        public static void AtualizaAcessoLastUpdade(string Key, DateTime LastUpdate)
        {
            SqLiteFunctions.OpenIfClosed();
            SqliteCommand insertCommand = new SqliteCommand
            {
                Connection = db,
                CommandText = "update ACESSADD set LastUpdate = @LastUpdate where key = @Key"
            };
            insertCommand.Parameters.AddWithNullableValue("@Key", Key);
            insertCommand.Parameters.AddWithNullableValue("@LastUpdate", LastUpdate);
            insertCommand.ExecuteReader();
            SqLiteFunctions.CloseIfOpen();
        }

    }
}
